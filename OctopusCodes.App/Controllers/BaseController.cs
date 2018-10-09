using OctopusCodesShoppingCartPaypal.Core.Models.Orders;
using OctopusCodesShoppingCartPaypal.Core.Provider;
using OctopusCodesShoppingCartPaypal.Core.Services.Settings;
using OctopusCodesShoppingCartPaypal.Core.Services.Users;
using OctopusCodesShoppingCartPaypal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OctopusCodes.App.Controllers
{
    public class BaseController : Controller
    {
        //const => having to declare the value at the time of a definition 
        //readonly => values can be computed dynamically but need to be assigned before the constructor exits.. after that it is frozen.
        //https://stackoverflow.com/questions/55984/what-is-the-difference-between-const-and-readonly

        protected readonly IUserService _userService;
        private readonly ISettingService _settingService;
        private Dictionary<string, string> _Setting;

        public BaseController(IUserService userService, ISettingService settingService)
        {
            _userService = userService;
            _settingService = settingService;
            ViewBag.BaseSetting = GetBaseSetting();
        }

        protected bool IsCurrentuser()
        {
            if (!User.Identity.IsAuthenticated) return false;
            var entity = _userService.FindByName(User.Identity.Name);
            return entity != null;
        }

        protected Account GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated) return null;
            var entity = _userService.FindByName(User.Identity.Name);
            return entity;
        }
        protected IDictionary<string, string> GetSettingPaypal()
        {
            return _settingService.GetAll().Where(a => a.Id.StartsWith("Client")).ToDictionary(a => a.Id, a => a.Value);
        }
        protected string GetSettingCurrency()
        {
            return _settingService.GetAll().Any(a => a.Id.Equals("Currency")) ? _settingService.GetAll().First(a => a.Id.Equals("Currency")).Value : "USD";
        }

        protected Dictionary<string,string> GetBaseSetting()
        {
            return _settingService.GetAll()
                .Where(a => a.Type.Equals("BaseSetting"))
                .ToDictionary(a => a.Id, a => a.Value);
        }

        protected CartViewModel GetSessionCart()
        {
            var cartObj = Session[Constants.CookieCart];
            CartViewModel cart = null;
            try
            {
                if (cartObj != null) cart = (CartViewModel)cartObj;
            }
            catch (Exception)
            {
                return new CartViewModel();
            }
            return cart ?? new CartViewModel();
        }
    }
}