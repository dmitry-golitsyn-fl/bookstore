using HibernateBookstore.Enums;
using HibernateBookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HibernateBookstore.ViewModels
{
    public class BaseBookViewModel
    {
        protected virtual void Init()
        {
            SearchName = string.Empty;
            SearchPublicationDate = string.Empty;
            SortField = Book.DEFAULT_SORT_PROPERTY;
        }

        public string SentFromPost { get; set; }

        public SortOrder Order { get; set; }
        public string SortField { get; set; }

        public string SearchName { get; set; }
        public string SearchPublicationDate { get; set; }

        public int ItemsPerPage { get; set; }

        public SelectList AvailableItemsPerPageSource
        {
            get
            {
                return (new SelectList(new List<int> { 1, 2, 3 },
                                       selectedValue: ItemsPerPage));
            }
        }
    }
}