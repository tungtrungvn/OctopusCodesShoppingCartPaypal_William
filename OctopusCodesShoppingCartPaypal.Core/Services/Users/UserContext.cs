using OctopusCodesShoppingCartPaypal.Core.Utils;
using OctopusCodesShoppingCartPaypal.Models.Entities;
using OctopusCodesShoppingCartPaypal.Models.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctopusCodesShoppingCartPaypal.Core.Services.Users
{
    public class UserContext : BaseService<Account>, IUserContext
    {
        public UserContext(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Account Login(string username, string password)
        {
            password = UtilEncrypt.GetMd5Hash(password);
            return Repository.GetAll(a => a.Username.Equals(username) && a.Password.Equals(password)).FirstOrDefault();

        }

        public Account GetUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
