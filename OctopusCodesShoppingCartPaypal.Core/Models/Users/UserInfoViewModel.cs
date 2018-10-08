using System.ComponentModel;

namespace OctopusCodesShoppingCartPaypal.Core.Models.Users
{
    public class UserInfoViewModel
    {
        public string UserName { get; set; }
        [DisplayName("Name")]
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
    }
}