using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctopusCodesShoppingCartPaypal.Core.Models.Products;
using OctopusCodesShoppingCartPaypal.Models.Entities;

namespace OctopusCodesShoppingCartPaypal.Core.Services.Permissions
{
    public interface IPermissionService : IBaseService<Role>
    {
        IQueryable<Role> GetAll();
        Role FindById(int id);
        void Insert(Role model);
        void Update(Role model);
        void Delete(int id);
    }
}
