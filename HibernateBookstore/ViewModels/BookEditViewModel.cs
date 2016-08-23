using HibernateBookstore.Enums;
using HibernateBookstore.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace HibernateBookstore.ViewModels
{
    public class BookEditViewModel : BaseBookViewModel
    {
        public BookEditViewModel() 
        {
            //this.BookItem = book;
            //Init();
        }

        protected override void Init()
        {
            using (var wrapper = new SessionWrapper())
            {
                base.Init();

                var authors = wrapper.Session.CreateCriteria<Author>().List<Author>();

                if (BookItem != null)
                {
                    var book = wrapper.Session.Get<Book>(BookItem.Id);
                    if (book != null)
                    {
                        NHibernateUtil.Initialize(book.Authors);
                    }
                }
                AvailableAuthors = authors.Select(x =>
                    new SelectListItem()
                    {
                        Text = string.Concat(x.FirstName, " ", x.MidName, " ", x.LastName),
                        Value = x.Id.ToString(), 
                        Selected = IsAuthorOfBook(x.Id)
                    });
            }
        }

        private bool IsAuthorOfBook(long id)
        { 
            if (BookItem == null) return false;

            foreach (var author in BookItem.Authors)
            {
                if (author.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        private Book _bookItem;
        public Book BookItem 
        {
            get { return _bookItem; }
            set
            {
                _bookItem = value;
                Init();
            } 
        }

        public IEnumerable<SelectListItem> AvailableAuthors { get; set; }
        public string[] SelectedAuthors { get; set; }

        public ItemEditingMode EditingMode { get; set; }
    }
}