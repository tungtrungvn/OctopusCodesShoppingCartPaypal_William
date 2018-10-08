using OctopusCodesShoppingCartPaypal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctopusCodesShoppingCartPaypal.Core.Services.Categories
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAll();
        Category FindById(int id);
        void Insert(Category entity);
        void Update(Category entity);
        void Delete(int id);
    }
}
