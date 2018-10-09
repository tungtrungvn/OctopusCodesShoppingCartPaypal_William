using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OctopusCodes.App.Securities;
using OctopusCodesShoppingCartPaypal.Core.Models.Users;
using OctopusCodesShoppingCartPaypal.Core.Services.Settings;
using OctopusCodesShoppingCartPaypal.Core.Services.Users;

namespace OctopusCodes.App.Controllers
{
    public class UserController : BaseController
    {
        //
        // GET: /User/
        public UserController(IUserService userService, ISettingService settingService) : base(userService, settingService)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
        [MyAuthorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [MyAuthorize]
        public ActionResult Profile()
        {
            var user = GetCurrentUser();
            var model = new UserInfoViewModel()
            {
                Phone = user.Phone,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                UserName = user.Username
            };
            return View(model);
        }
        [HttpPost]
        [MyAuthorize]
        public ActionResult Profile(UserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userService.Update(model);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                }
            }
            return View(model);
        }
    }
}