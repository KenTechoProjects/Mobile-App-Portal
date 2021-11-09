using System;
using System.ComponentModel.DataAnnotations;

namespace YellowStone.Services.WalletService.DTOs
{
    public class FundWalletRequest
    {
        public string TransactionReference { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string WalletId { get; set; }
        public string Narration { get; set; }
    }
}
