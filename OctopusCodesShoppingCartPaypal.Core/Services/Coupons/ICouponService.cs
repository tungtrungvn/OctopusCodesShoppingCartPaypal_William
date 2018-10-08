using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OctopusCodesShoppingCartPaypal.Core.Models.Coupons;
using OctopusCodesShoppingCartPaypal.Models.Entities;

namespace OctopusCodesShoppingCartPaypal.Core.Services.Coupons
{
    public interface ICouponService : IBaseService<Coupon>
    {
        IQueryable<Coupon> GetAll();
        Coupon FindById(string id);
        void Insert(CouponViewModel model);
        void Update(CouponViewModel model);
        void Delete(string id);
    }
}
