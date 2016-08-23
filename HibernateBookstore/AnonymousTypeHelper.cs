using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HibernateBookstore
{
    public static class AnonymousTypeHelper
    {
        public static PropertyDescriptor GetPropertyByName(this Type type, string propertyName)
        {
            return TypeDescriptor.GetProperties(type).Find(propertyName, false);
        }
    }
}