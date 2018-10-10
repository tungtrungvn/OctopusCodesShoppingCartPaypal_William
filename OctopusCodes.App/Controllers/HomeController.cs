using OctopusCodesShoppingCartPaypal.Core.Models.datatables;
using OctopusCodesShoppingCartPaypal.Core.Models.Products;
using OctopusCodesShoppingCartPaypal.Core.Provider;
using OctopusCodesShoppingCartPaypal.Core.Services.Products;
using OctopusCodesShoppingCartPaypal.Core.Services.Settings;
using OctopusCodesShoppingCartPaypal.Core.Services.Users;
using OctopusCodesShoppingCartPaypal.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OctopusCodes.App.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService, IUserService userService, ISettingService settingService)
            : base(userService, settingService)
        {
            _productService = productService;
        }

        //
        // GET: /Home/
        public ActionResult Index(int? id)
        {
            ViewBag.ShowMenu = true;
            return View();
        }

        //datatableRequest Property Value
        //-----------------------------------
        //Data=null
        //Index=1
        //Page=0
        //Size=9
        //StartIndex=0
        //Total=0
        public ActionResult GetItems(DatatableData<ProductViewModel> datatableRequest, int? categoryId)
        {
            var imgPath = Constants.ImagePath + "/";
            //entities is of type IQueryable<Product>
            var entities = _productService.GetAll().Where(a => a.Status == (int)Constants.EnumProductStatus.Stocking);
            if (categoryId.HasValue && categoryId > 0)
            {
                entities = entities.Where(a => a.CategoryId == categoryId.Value || a.Category.ParentId == categoryId);
            }
            //models is of type IQueryable<ProductViewModel>
            //Map Product.Id to ProductViewModel.Id, begitu juga pada Price, ProductName, Image
            //Image -> get product images if any -> get main image -> then append it to image path , otherwise it's null
            //Order by price ascending
            //Convert Product which is mapped above to ProductViewModel
            var models = entities.Select(a => new ProductViewModel()
            {
                Id = a.Id,
                Price = a.Price,
                ProductName = a.Name,
                Image = a.Images.Any() ?
                    a.Images.Any(x => x.Main == true) ?
                         imgPath + a.Images.FirstOrDefault(x => x.Main == true).Name : imgPath + a.Images.FirstOrDefault().Name :
                      null
            }).OrderBy(a => a.Price);
            return Json(DatatableModel.Refresh(models, datatableRequest.Size, datatableRequest.Index));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Detail(int? id)
        {
            var imgPath = Constants.ImagePath + "/";
            if (!id.HasValue)
            {
                TempData[Constants.NotifyMessage] = string.Format(Resource.MsgIsNotFound, "Product item");
                return RedirectToAction("index");
            }
            var entity = _productService.FindById(id.Value);
            if (entity == null || entity.Status == (int)Constants.EnumProductStatus.OutOfStock)
            {
                TempData[Constants.NotifyMessage] = string.Format(Resource.MsgIsNotFound, "Product item");
                return RedirectToAction("index");
            }
            var imgProducts = entity.Images.Any() ? entity.Images.Select(a => imgPath + a.Name).ToList() : new List<string>();
            var model = new ProductDetailViewModel()
            {
                Id = entity.Id,
                Price = entity.Price.HasValue ? entity.Price.Value : 0,
                Quantity = 1,
                Description = entity.Description,
                ProductName = entity.Name,
                Status = Constants.ProductStatusDictionary[entity.Status],
                Imgs = imgProducts.Any() ? imgProducts.ToArray() : null,
                MainImg =
                    entity.Images.Any(a => a.Main == true)
                        ? imgPath + entity.Images.First(a => a.Main == true).Name
                        : imgProducts.FirstOrDefault()
            };
            return View(model);
        }
    }
}