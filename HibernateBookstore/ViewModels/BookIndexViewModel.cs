using HibernateBookstore.Enums;
using HibernateBookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HibernateBookstore.ViewModels
{
    public class BookIndexViewModel : BaseBookViewModel
    {
        public BookIndexViewModel() 
        {
            Init();
        }

        public BookIndexViewModel(PagedList.IPagedList<Book> books)
        {
            this.Books = books;
            Init();
        }

        protected override void Init()
        {
 	        base.Init();
        }

        public PagedList.IPagedList<Book> Books { get; set; }
    }
}