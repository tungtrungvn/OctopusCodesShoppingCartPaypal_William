using OctopusCodesShoppingCartPaypal.Core.Models.Orders;
using OctopusCodesShoppingCartPaypal.Models.Entities;
using System.Linq;

namespace OctopusCodesShoppingCartPaypal.Core.Services.Orders
{
    public interface IOrdersService : IBaseService<Order>
    {
        IQueryable<Order> GetAll();
        Order FindById(int id);
        int Insert(CartViewModel model);
        void Update(Order model);
        void Delete(int id);
    }
}