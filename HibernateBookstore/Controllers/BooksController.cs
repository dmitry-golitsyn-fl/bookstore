using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using NHibernate.Linq;
using HibernateBookstore.Models;
using HibernateBookstore.Enums;
using HibernateBookstore.ViewModels;
using NHibernate.Criterion;

using PagedList;
using PagedList.Mvc;

namespace HibernateBookstore.Controllers
{
    public class BooksController : Controller
    {
        // Зачем постоянно использовать транзакции NHibernate? SO:Q3304347
        //When we don't define our own transactions, it falls back into implicit transaction mode, 
        //where every statement to the database runs in its own transaction, resulting in a large 
        //performance cost (database time to build and tear down transactions), and reduced consistency.

        //Even if we are only reading data, we should use a transaction, because using transactions 
        //ensures that we get consistent results from the database. NHibernate assumes that all access 
        //to the database is done under a transaction, and strongly discourages any use of the session 
        //without a transaction.

        //
        // GET: /Books/

        private const int DEFAULT_ITEMS_PER_PAGE_VALUE = 2;

        public ActionResult Index(BookIndexViewModel search,
                                  int? page,
                                  int? itemsPerPage,
                                  string sortField = "",
                                  SortOrder sortOrder = SortOrder.Ascending)
        {
            using (var wrapper = new SessionWrapper())
            {
                using (ITransaction transaction = wrapper.Session.BeginTransaction())
                {
                    BookIndexViewModel responseViewModel = search;
                    if (String.IsNullOrEmpty(responseViewModel.SentFromPost))
                    {
                        responseViewModel.ItemsPerPage = itemsPerPage ?? DEFAULT_ITEMS_PER_PAGE_VALUE;
                        responseViewModel.Order = sortOrder;
                        if (!String.IsNullOrEmpty(sortField))
                        {
                            responseViewModel.SortField = sortField; 
                        }
                    }
                    
                    IEnumerable<Book> sortedBooks;                                        
                    if (sortOrder == SortOrder.Descending)
                    {
                        sortedBooks = wrapper.Session.CreateCriteria<Book>()
                                             .AddOrder(Order.Desc(responseViewModel.SortField))
                                             .List<Book>();
                    }
                    else
                    {
                        sortedBooks = wrapper.Session.CreateCriteria<Book>()
                                             .AddOrder(Order.Asc(responseViewModel.SortField))
                                             .List<Book>();
                    }

                    int pageNumber = page ?? 1;

                    IEnumerable<Book> filteredBooks;
                    if (!string.IsNullOrEmpty(responseViewModel.SearchName))
                    {
                        filteredBooks = sortedBooks.Where(o => o.Name.Contains(responseViewModel.SearchName));
                        responseViewModel.Books = filteredBooks.ToPagedList(pageNumber,
                                                                            responseViewModel.ItemsPerPage);
                    }
                    else responseViewModel.Books = sortedBooks.ToPagedList(pageNumber,
                                                                           responseViewModel.ItemsPerPage);
                    
                    return View(responseViewModel);
                }
            }
        }

        // Переходим по адресу без параметров
        public ActionResult Create()
        {
            var viewModel = new BookEditViewModel()
            {
                BookItem = new Book(),
                EditingMode = ItemEditingMode.Create
            };

            return View("EditBook", viewModel);
        }

        [HttpPost]
        public ActionResult Create(BookEditViewModel viewModel)
        {
            try
            {
                using (var wrapper = new SessionWrapper())
                {
                    using (ITransaction transaction = wrapper.Session.BeginTransaction())
                    {
                        long id;
                        foreach (var idString in viewModel.SelectedAuthors)
                        {                            
                            if (long.TryParse(idString, out id))
                            {
                                viewModel.BookItem.Authors.Add(wrapper.Session.Get<Author>(id));
                            }
                        }

                        wrapper.Session.Save(viewModel.BookItem);
                        transaction.Commit();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                return RedirectToAction("EditBook", viewModel);
            }
        }

        //
        // GET: /Books/Edit/5

        public ActionResult Edit(long id)
        {
//            return View(viewModel);
            using (var wrapper = new SessionWrapper())
            {
                Book book;
                using (ITransaction transaction = wrapper.Session.BeginTransaction())
                {
                    book = wrapper.Session.Get<Book>(id);
                }

                var viewModel = new BookEditViewModel()
                {
                    BookItem = book,
                    EditingMode = ItemEditingMode.Edit
                };
                return View("EditBook", viewModel);

            }
        }


        [HttpPost]
        public ActionResult Edit(BookEditViewModel viewModel)
        {
            using (var wrapper = new SessionWrapper())
            {
                long id;
                foreach (var idString in viewModel.SelectedAuthors)
                {
                    if (long.TryParse(idString, out id))
                    {
                        viewModel.BookItem.Authors.Add(wrapper.Session.Get<Author>(id));
                    }
                }

                var bookToUpdate = wrapper.Session.Get<Book>(viewModel.BookItem.Id);

                bookToUpdate.Name = viewModel.BookItem.Name;
                bookToUpdate.PublicationDate = viewModel.BookItem.PublicationDate;
                bookToUpdate.ISBN = viewModel.BookItem.ISBN;
                bookToUpdate.Authors = viewModel.BookItem.Authors;

                using (ITransaction transaction = wrapper.Session.BeginTransaction())
                {
                    wrapper.Session.Save(bookToUpdate);
                    transaction.Commit();
                }
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Books/Delete/5

        public ActionResult Delete(long id)
        {
            using (var wrapper = new SessionWrapper())
            {
                Book book;
                using (ITransaction transaction = wrapper.Session.BeginTransaction())
                {
                    book = wrapper.Session.Get<Book>(id);
                    NHibernateUtil.Initialize(book.Authors);
                }

                var viewModel = new BookEditViewModel()
                {
                    BookItem = book,
                    EditingMode = ItemEditingMode.Delete
                };
                return View("EditBook", viewModel);
            }
        }

        [HttpPost]
        public ActionResult Delete(BookEditViewModel viewModel)
        {
            try
            {
                using (var wrapper = new SessionWrapper())
                {
                    using (ITransaction transaction = wrapper.Session.BeginTransaction())
                    {
                        wrapper.Session.Delete(viewModel.BookItem);
                        transaction.Commit();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                return View("EditBook", viewModel);
            }
        }
    }
}
