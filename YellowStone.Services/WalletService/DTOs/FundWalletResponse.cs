using System;
namespace YellowStone.Services.WalletService.DTOs
{
    public class FundWalletResponse
    {
        public bool IsSuccessful { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
