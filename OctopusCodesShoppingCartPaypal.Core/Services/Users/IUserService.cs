using OctopusCodesShoppingCartPaypal.Core.Models.Users;
using OctopusCodesShoppingCartPaypal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctopusCodesShoppingCartPaypal.Core.Services.Users
{
    public interface IUserService : IBaseService<Account>
    {
        Account FindByName(string userName);
        Account FindById(int id);
        IQueryable<Account> GetAll();
        void Insert(UserViewModel user);
        void Insert(RegisterViewModel model);
        void Update(UserViewModel user);
        void Update(UserInfoViewModel user);
        void Delete(int id);
    }
}
