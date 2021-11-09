using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels
{
    public class CustomerInfoViewModel : BaseViewModel
    {
        public string CustomerId { get; set; }
        public string Name { get { return $"{FirstName} {MiddleName} {LastName}"; } }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AccountNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool HasAccount { get; set; }
        public bool HasWallet { get; set; }
        public string BankId { get; set; }
        
        public string ProfileStatus { get; set; }
        public string DateEnrolled { get; set; }
        public CustomerManagementViewModel customerManagementViewModel { get; set; }
        

        public List<CustomerActivityViewModel> CustomerActivitiesViewModel { get; set; }
        public List<TransactionHistoryViewModel>  TransactionsViewModel { get; set; }
        public CustomerInfoViewModel(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
            CustomerActivitiesViewModel = new List<CustomerActivityViewModel>();
            TransactionsViewModel = new List<TransactionHistoryViewModel>();
        }
    }
}
