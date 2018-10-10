using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OctopusCodesShoppingCartPaypal.Core.Models.MessageModels;
using OctopusCodesShoppingCartPaypal.Core.Models.Orders;
using OctopusCodesShoppingCartPaypal.Core.Provider;
using OctopusCodesShoppingCartPaypal.Core.Resources;
using OctopusCodesShoppingCartPaypal.Core.Services.Coupons;
using OctopusCodesShoppingCartPaypal.Core.Services.Products;
using OctopusCodesShoppingCartPaypal.Core.Services.Settings;
using OctopusCodesShoppingCartPaypal.Core.Services.Users;
using PayPal.Api;

namespace OctopusCodes.App.Controllers
{
    public class CartController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        public CartController(ICouponService couponService, IProductService productService, IUserService userService, ISettingService settingService)
            : base(userService, settingService)
        {
            _productService = productService;
            _couponService = couponService;
        }
        // GET: Cart
        [HttpPost]
        public ActionResult AddItem(int productId)
        {
            //If Session[cart] exist then return it. Otherwise return object new CartViewModel
            var cart = GetSessionCart();
            try
            {
                var product = _productService.FindById(productId);
                //If product == null OR product.status != Stocking THEN
                if (product == null || product.Status != (int)Constants.EnumProductStatus.Stocking)
                {
                    //return 
                    /*
                        public class CartResultViewModel : NotifyModel
                        {
                            public int Amount { get; set; }
                        }

                        public class NotifyModel
                        {
                            public bool Result { get; set; }
                            public string Message { get; set; }
                        }
                    */
                    //Result = false, 
                    //Message = (Name : DataIsNotFound, Value :	{0} is not found)	
                    //Message = (Name : Item, Value : Item)	
                    //Amount = cart.Items.Count (cart is instance of CartViewModel) -> get count of cart.Items per productId. 
                    //Ex : 
                    //product 
                    //  { {productId : 1}, {productName : Baju1}, {productQty : 2} }, 
                    //  { {productId : 2}, {productName : Baju2}, {productQty : 3} },
                    //Amount = 2 (productId 1 dan productId 2)
                    /*
                  
                    public class CartViewModel
                    {
                        public CartViewModel()
                        {
                            Items = new Collection<CartItemViewModel>();
                        }
                        public string Token { get; set; }
                        public string UserName { get; set; }
                        public ICollection<CartItemViewModel> Items { get; set; }
                        public double Total { get { return Items.Sum(x => x.Total); } }
                        public string CouponId { get; set; }
                    }  

                    public class CartItemViewModel
                    {
                        public int ProductId { get; set; }
                        public string ProductName { get; set; }
                        public string Image { get; set; }
                        public string Description { get; set; }
                        public double? Price { get; set; }
                        public int Quantity { get; set; }
                        public double Total { get { return (!Price.HasValue ? 0 : Price.Value) * Quantity; } }
                    }  
                     
                    */

                    return
                        Json(new CartResultViewModel()
                        {
                            Result = false,
                            Message = string.Format(Resource.DataIsNotFound, Resource.Item),
                            Amount = cart.Items.Count
                        });
                }
                //If there is already product with Id = productId in cart
                if (cart.Items.Any(a => a.ProductId == product.Id))
                {
                    //Add Qty of CartItemViewModel.Items By 1
                    cart.Items.First(a => a.ProductId == product.Id).Quantity++;
                }
                //If there is not yet any product with Id = productId in cart
                else
                {
                    //get main image of the product 
                    var image = product.Images.FirstOrDefault(a => a.Main.HasValue && a.Main.Value);

                    //add the product of instance CartItemViewModel in cart.Items with Qty = 1 and another attribute
                    cart.Items.Add(new CartItemViewModel()
                    {
                        Description = product.Description,
                        Price = product.Price,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Image = image != null ? image.Name : Constants.DefaultImg
                    });
                }

                //Set Session[cart] = cart
                Session[Constants.CookieCart] = cart;

                //return Json 
                //Resources Name - Value
                //AddedToCart	Added to cart	
                //Amount = cart.Items.Count

                return Json(new CartResultViewModel()
                {
                    Result = true,
                    Message = string.Format(Resource.AddedToCart),
                    Amount = cart.Items.Count
                });
            }
            catch (Exception)
            {
                //Resources Name - Value
                //InternalException	Internal exceptions	
                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message = string.Format(Resource.InternalException),
                    Amount = cart.Items.Count
                });
            }
        }

        [HttpPost]
        public ActionResult UpdateItem(int productId, int amount)
        {
            var cart = GetSessionCart();
            try
            {
                var product = _productService.FindById(productId);
                if (product == null || product.Status != (int)Constants.EnumProductStatus.Stocking)
                    return
                            Json(new CartResultViewModel()
                            {
                                Result = false,
                                Message = string.Format(Resource.DataIsNotFound, Resource.Item),
                                Amount = cart.Items.Count
                            });
                if (amount < 1)
                    return Json(new CartResultViewModel()
                    {
                        Result = false,
                        Message = string.Format(Resource.InvalidData, Resource.AmountItem),
                        Amount = cart.Items.Count
                    });
                if (cart.Items.Any(a => a.ProductId == product.Id))
                {
                    cart.Items.First(a => a.ProductId == product.Id).Quantity++;
                }
                Session[Constants.CookieCart] = cart;
                return Json(new CartResultViewModel()
                {
                    Result = true,
                    Message = Resource.HaveUpdatedCart,
                    Amount = cart.Items.Count
                });
            }
            catch (Exception ex)
            {

                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message = Resource.InternalException,
                    Amount = cart.Items.Count
                });
            }

        }

        [HttpPost]
        public ActionResult DeleteItem(int productId)
        {
            var cart = GetSessionCart();
            try
            {
                if (cart.Items.Any(a => a.ProductId == productId))
                {
                    var item = cart.Items.First(a => a.ProductId == productId);
                    cart.Items.Remove(item);
                    Session[Constants.CookieCart] = cart;
                    return Json(new CartResultViewModel()
                    {
                        Result = true,
                        Message = string.Format(Resource.DeleteItemSuccess, Resource.CartItem.ToLower()),
                        Amount = cart.Items.Count
                    });
                }
                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message = string.Format(Resource.DataIsNotFound, Resource.CartItem.ToLower()),
                    Amount = cart.Items.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message = Resource.InternalException,
                    Amount = cart.Items.Count
                });
            }
        }

        [HttpPost]
        public ActionResult CheckCoupon(string couponCode)
        {
            var cart = GetSessionCart();
            var now = DateTime.Now;
            try
            {
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = _couponService.FindById(couponCode);
                    if (coupon == null || now < coupon.Start_Date || now > coupon.End_Date)
                    {
                        return Json(new CouponResultViewModel()
                        {
                            Result = false,
                            Message = string.Format(Resource.DataIsNotFound, Resource.HeaderCoupon),
                            Discount = 0,
                            Total = Convert.ToDecimal(cart.Total)
                        });
                    }
                    return Json(new CouponResultViewModel()
                    {
                        Result = true,
                        Message = string.Format(Resource.DataIsNotFound, Resource.HeaderCoupon),
                        Discount = coupon.Discount,
                        Total = Convert.ToDecimal(cart.Total) - coupon.Discount
                    });
                }
                return Json(new CouponResultViewModel()
                {
                    Result = false,
                    Message = string.Format(Resource.InvalidData, Resource.HeaderCoupon),
                    Discount = 0,
                    Total = Convert.ToDecimal(cart.Total)
                });
            }
            catch (Exception ex)
            {
                return Json(new CouponResultViewModel()
                {
                    Result = true,
                    Message = Resource.InternalException,
                    Discount = 0,
                    Total = Convert.ToDecimal(cart.Total)
                });
            }
        }
        #region Helper

        #endregion

    }
}