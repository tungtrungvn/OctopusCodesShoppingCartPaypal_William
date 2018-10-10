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
using OctopusCodesShoppingCartPaypal.Core.Models.MessageModels;

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

        //http://octopuscodesshoppingcartpaypal_william/Home/Detail/1036
        //id = 1036 (productId)
        public ActionResult Detail(int? id)
        {
            ///imgPath = Upload/Product
            var imgPath = Constants.ImagePath + "/";

            //if id is null
            //Example : http://octopuscodesshoppingcartpaypal_william/Home/Detail/ instead of http://octopuscodesshoppingcartpaypal_william/Home/Detail/1
            if (!id.HasValue)
            {
                //https://stackoverflow.com/questions/10487008/mvc-c-sharp-tempdata
                //TempData is meant to be a very short-lived instance, and you should only use it during the current and the subsequent requests only!

                //public const string NotifyMessage = "MessageError";
                //MsgIsNotFound	-> {0} is not found	

                //Change By William
                //Description : Change code in line 100 to 101 -> so that we can use notifyModel (Result, Message) instead of only string
                //TempData[Constants.NotifyMessage] = string.Format(Resource.MsgIsNotFound, "Product item");    //From
                TempData[Constants.NotifyMessage] = new NotifyModel() { Result = false, Message = string.Format(Resource.MsgIsNotFound, "Product item") };  //To
                //Change By William

                return RedirectToAction("index");
            }

            //Find product by Id
            var entity = _productService.FindById(id.Value);

            //If product not found or product status is out of stock (3)
            if (entity == null || entity.Status == (int)Constants.EnumProductStatus.OutOfStock)
            {
                //Change By William
                //Description : Change code in line 100 to 101 -> so that we can use notifyModel (Result, Message) instead of only string
                //TempData[Constants.NotifyMessage] = string.Format(Resource.MsgIsNotFound, "Product item");    //From
                TempData[Constants.NotifyMessage] = new NotifyModel() { Result = false, Message = string.Format(Resource.MsgIsNotFound, "Product item") };  //To
                //Change By William
                return RedirectToAction("index");
            }
            //If there are images of the product then select image path. Otherwise new List<string>
            var imgProducts = entity.Images.Any() ? entity.Images.Select(a => imgPath + a.Name).ToList() : new List<string>();
            //Set model (instance of ProductDetailViewModel) by converting product attribute to model attribute
            var model = new ProductDetailViewModel()
            {
                Id = entity.Id,
                Price = entity.Price.HasValue ? entity.Price.Value : 0,
                Quantity = 1,
                Description = entity.Description,
                ProductName = entity.Name,
                Status = Constants.ProductStatusDictionary[entity.Status],
                //if there are any product image then display imgProducts.ToArray() otherwise null
                Imgs = imgProducts.Any() ? imgProducts.ToArray() : null,
                //If there is image with field Main == true then display it otherwise display any first image 
                MainImg =
                    entity.Images.Any(a => a.Main == true)
                        ? imgPath + entity.Images.First(a => a.Main == true).Name
                        : imgProducts.FirstOrDefault()
            };
            return View(model);
        }
    }
}