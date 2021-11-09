using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels
{
    public class AccountDetailsViewModel : BaseViewModel
    {
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }
        public string CurrencyCode { get; set; }
        public string BranchCode { get; set; }
        public string Branch { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }

        public AccountDetailsViewModel(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
