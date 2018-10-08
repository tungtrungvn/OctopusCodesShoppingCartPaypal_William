using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctopusCodesShoppingCartPaypal.Core.Models.Settings;
using OctopusCodesShoppingCartPaypal.Models.Entities;

namespace OctopusCodesShoppingCartPaypal.Core.Services.Settings
{
    public interface ISettingService : IBaseService<Setting>
    {
        IQueryable<Setting> GetAll();
        Setting FindById(string id);
        void Insert(SettingVIewModel model);
        void Update(SettingVIewModel model);
        void Delete(int id);
    }
}
