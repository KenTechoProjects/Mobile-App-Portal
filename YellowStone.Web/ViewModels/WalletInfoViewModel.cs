using System;
namespace YellowStone.Web.ViewModels
{
    public class WalletInfoViewModel : BaseViewModel
    {
        public string AccountName { get; set; }
        public string WalletAccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string WalletType { get; set; }
        public string DestinationAccountId { get; set; }
        public string CustomerId { get; set; }
        public bool IsLinked { get { return CustomerId != null; } }
        public string WalletId { get; set; }
        public WalletInfoViewModel(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
