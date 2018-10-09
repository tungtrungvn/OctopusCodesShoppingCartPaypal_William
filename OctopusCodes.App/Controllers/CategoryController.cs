using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OctopusCodesShoppingCartPaypal.Core.Services.Categories;
using OctopusCodesShoppingCartPaypal.Core.Services.Settings;
using OctopusCodesShoppingCartPaypal.Core.Services.Users;

namespace OctopusCodes.App.Controllers
{
    public class CategoryController : BaseController
    {
        //
        // GET: /Category/
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService, IUserService userService, ISettingService settingService) : base(userService, settingService)
        {
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetCategories()
        {
            return Json(new { result = _categoryService.GetAll().ToList() });
        }

        public ActionResult GetMenu()
        {
            return PartialView("_CategoryMenu", _categoryService.GetAll().OrderBy(a => a.Ancestor).ToList());
        }
    }
}