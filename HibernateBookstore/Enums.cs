using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HibernateBookstore.Enums
{
    public enum SortOrder
    { 
        Ascending,
        Descending
    }

    public enum ItemEditingMode
    {
        Create,
        Edit,
        Delete
    }
}