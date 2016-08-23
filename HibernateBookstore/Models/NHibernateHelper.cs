#define HIBERNATE_MAP_CODE
//#define HIBERNATE_CFG_CODE

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

// К вопросу о времени жизни сессии
//Here are some general guidelines when deciding on the lifetime of the context:

//When working with long-running context consider the following:
//As you load more objects and their references into memory, the memory consumption 
//of the context may increase rapidly. This may cause performance issues.
//
//Remember to dispose of the context when it is no longer required.
//If an exception causes the context to be in an unrecoverable state, 
//the whole application may terminate.
//The chances of running into concurrency-related issues increase as the gap 
//between the time when the data is queried and updated grows.
//
//When working with Web applications, use a context instance per request.
//
//When working with Windows Presentation Foundation (WPF) or Windows Forms, 
//use a context instance per form. This lets you use change-tracking functionality 
//that context provides. 

namespace HibernateBookstore.Models
{
    public class NHibernateHelper
    {
        private const string CURRENT_SESSION_KEY = "nhibernate.current_session";
        private static readonly ISessionFactory sessionFactory;

        static NHibernateHelper()
        {
            NHibernate.Cfg.Configuration cfg = null;

#if HIBERNATE_CFG_CODE
            cfg = new NHibernate.Cfg.Configuration()
                            .DataBaseIntegration(db =>
                            {
                                db.ConnectionString = ConfigurationManager.
                                    ConnectionStrings[Constants.DB_CONNECTION_STRING_NAME].
                                    ConnectionString;
                                db.Dialect<MsSql2008Dialect>();
                                db.Driver<NHibernate.Driver.SqlClientDriver>();
                                db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                            });
#else
            cfg = new NHibernate.Cfg.Configuration().Configure();
#endif

#if HIBERNATE_MAP_CODE
            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            HbmMapping mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            cfg.AddMapping(mapping);
#else
            // Автоматический маппинг всей сборки, содержащей типы сущностей
            configuration.AddAssembly(typeof(<type_name>).Assembly);
#endif

            new SchemaUpdate(cfg).Execute(true, true);
            sessionFactory = cfg.BuildSessionFactory();
        }

        public static ISession GetCurrentSession()
        {
            HttpContext context = HttpContext.Current;
            ISession currentSession = context.Items[CURRENT_SESSION_KEY] as ISession;

            if (currentSession == null)
            {
                currentSession = sessionFactory.OpenSession();
                context.Items[CURRENT_SESSION_KEY] = currentSession;
            }

            return currentSession;
        }

        public static void CloseSession()
        {
            HttpContext context = HttpContext.Current;
            ISession currentSession = context.Items[CURRENT_SESSION_KEY] as ISession;

            if (currentSession == null)
            {
                return;
            }

            currentSession.Close();
            context.Items.Remove(CURRENT_SESSION_KEY);
        }

        public static void CloseSessionFactory()
        {
            if (sessionFactory != null)
            {
                sessionFactory.Close();
            }
        }   
    }

    public class SessionWrapper : IDisposable
    {
        public SessionWrapper()
        {
            this.Session = NHibernateHelper.GetCurrentSession();
        }

        public ISession Session { get; internal set; }

        public void Dispose()
        {
            NHibernateHelper.CloseSession();
        }

        //public static IQueryable<T> OrderBy<T>(this IQueryable<T> source,
        //                                        string propertyName)
        //{
 
        //    if (source == null) throw new ArgumentNullException("source");
        //    if (propertyName == null) throw new ArgumentNullException("propertyName");
 
        //    Type type = typeof(T);
        //    ParameterExpression arg = Expression.Parameter(type, "x");
 
        //    PropertyInfo pi = type.GetProperty(propertyName);
        //    Expression expr = Expression.Property(arg, pi);
        //    var lambda = Expression.Lambda(expr, arg);
        //    return source.OrderBy((Expression)lambda);
        //}
    }
}