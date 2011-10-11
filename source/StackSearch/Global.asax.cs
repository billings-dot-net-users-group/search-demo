using System;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using BillingsDotNet.Search;

namespace StackSearch
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "search", action = "index", id = UrlParameter.Optional } // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            DependencyResolver.SetResolver(CreateDependencyResolver());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        private IDependencyResolver CreateDependencyResolver()
        {
            var lucenePath = Server.MapPath("~/App_Data/Lucene");

            var builder = new ContainerBuilder();
            builder.Register(c => new IndexTankIndex("http://:GKwE+QM84IReDp@dkjak.api.indextank.com", "billingsdotnet")).AsSelf().As<IIndex>().SingleInstance();
            builder.Register(c => new LuceneIndex(lucenePath)).As<IIndex>().SingleInstance();
            builder.RegisterModule<AutofacWebTypesModule>();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            return new AutofacDependencyResolver(builder.Build());
        }
    }
}