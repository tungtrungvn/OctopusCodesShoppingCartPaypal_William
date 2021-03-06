﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OctopusCodesShoppingCartPaypal.Models.Repositories
{
    public interface IRepositoryBase<T> : IDisposable where T : class
    {
        void Insert(T entity);
        void Insert(ICollection<T> entities);
        void Update(T entity);
        void Update(ICollection<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null);
        T Get(object key);
    }
}
