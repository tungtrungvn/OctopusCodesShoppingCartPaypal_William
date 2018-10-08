using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using OctopusCodesShoppingCartPaypal.Core.Services;
using OctopusCodesShoppingCartPaypal.Models.Entities;
using OctopusCodesShoppingCartPaypal.Models.UnitOfWork;
using System.Web.Optimization;
using datatables.Utils.DatatableModels;
using datatables.Utils.ModelBinder;
using System.Web.Security;
using Newtonsoft.Json;
using OctopusCodes.App.Securities;
using OctopusCodesShoppingCartPaypal.Core.Services.Users;
using OctopusCodesShoppingCartPaypal.Core.Services.Products;
using OctopusCodesShoppingCartPaypal.Core.Services.Categories;
using OctopusCodesShoppingCartPaypal.Core.Services.Orders;
using OctopusCodesShoppingCartPaypal.Core.Services.Settings;
using OctopusCodesShoppingCartPaypal.Core.Services.Permissions;
using OctopusCodesShoppingCartPaypal.Core.Services.Coupons;

//Test Commit
namespace OctopusCodes.App
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            //Register your MVC Controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //OPTIONAL: Register Model Binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());

            //OPTIONAL : Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            //OPTIONAL: Enable property injection in veiw pages;
            builder.RegisterSource(new ViewRegistrationSource());

            //OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            #region components

            var assembly = Assembly.GetAssembly(typeof(BaseService<>));

            builder.RegisterType<OctopusCodesShoppingCartPaypalEntities>().InstancePerRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<UserContext>().As<IUserContext>().InstancePerDependency();
            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<CategoryService>().As<ICategoryService>();
            builder.RegisterType<OrdersService>().As<IOrdersService>();
            builder.RegisterType<SettingService>().As<ISettingService>();
            builder.RegisterType<PermissionService>().As<IPermissionService>();
            builder.RegisterType<CouponService>().As<ICouponService>();
            builder.RegisterType<UserService>().As<IUserService>();

            #endregion

            //Set the dependency resolver to be Autofac
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Custom binding
            ModelBinders.Binders.Add(typeof(DataTablesParam), new DataTablesModelBinder());
            ModelBinders.Binders.Add(typeof(Decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(Decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(string), new StringModelBinder());
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(cookie.Value);
                var ma = JsonConvert.DeserializeObject<MyAccount>(authTicket.UserData);
                var mp = new MyPrincipal(ma.Username) { Ma = ma };
                HttpContext.Current.User = mp;
            }
        }
    }
}
















