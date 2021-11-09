using System;
namespace YellowStone.Services.WalletService.DTOs
{
    public class WalletEnquiryResponse
    {
        public decimal Balance { get; set; }
        public string WalletAccountNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string WalletType { get; set; }
        public string IsRestricted { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
