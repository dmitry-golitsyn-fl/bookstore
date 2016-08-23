using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HibernateBookstore.Models
{
    public class Author
    {
        public virtual long Id { get; set; }
        [DisplayName("Имя")]
        public virtual string FirstName { get; set; }
        [DisplayName("Отчество")]
        public virtual string MidName { get; set; }
        [DisplayName("Фамилия")]
        public virtual string LastName { get; set; }
        [DisplayName("Дата рождения")]
        public virtual DateTime? DateOfBirth { get; set; }

        private ISet<Book> _books;
        public virtual ISet<Book> Books
        {
            get
            {
                return _books ?? (_books = new HashSet<Book>());
            }
            set { _books = value; }
        }
    }

    // Маппинг в коде
    public class AuthorMap : ClassMapping<Author>
    {
        public AuthorMap()
        {
            Table("authors");

            Id(x => x.Id, map => map.Generator(Generators.Native));
            Property(x => x.FirstName); 
            Property(x => x.MidName);
            Property(x => x.LastName);
            Property(x => x.DateOfBirth);

            Set(a => a.Books,
                c =>
                {
                    c.Cascade(Cascade.Persist);
                    c.Key(k => k.Column("AuthorId"));
                    c.Table("authors_to_books");
                },
                r => r.ManyToMany(m => m.Column("BookId")));
        }
    } 
}