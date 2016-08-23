using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HibernateBookstore.Models
{
    public class Book
    {
        public static string DEFAULT_SORT_PROPERTY = "Name";

        public virtual long Id { get; set; }
        [DisplayName("Название")]
        public virtual string Name { get; set; }
        [DisplayName("Год публикации")]        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime? PublicationDate { get; set; }
        [DisplayName("Код ISBN")]
        public virtual string ISBN { get; set; }

        private ISet<Author> _authors;
        [DisplayName("Автор(ы)")]
        public virtual ISet<Author> Authors
        {
            get
            {
                return _authors ?? (_authors = new HashSet<Author>());
            }
            set { _authors = value; }
        }
    }

    // Маппинг в коде
    public class BookMap : ClassMapping<Book> 
    {
        public BookMap()
        {
            Table("books");

            Id(x => x.Id, map => map.Generator(Generators.Native));
            Property(x => x.Name);
            Property(x => x.PublicationDate);
            Property(x => x.ISBN);

            Set(a => a.Authors,
                c =>
                {
                    c.Cascade(Cascade.Persist);
                    c.Key(k => k.Column("BookId"));
                    c.Table("authors_to_books");
                },
                r => r.ManyToMany(m => m.Column("AuthorId")));
        }
	} 
}