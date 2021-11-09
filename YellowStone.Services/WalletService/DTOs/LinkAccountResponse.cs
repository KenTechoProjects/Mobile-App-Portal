using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.WalletService.DTOs
{
    public class LinkAccountRequest
    {
        public string CIF { get; set; }
        public string WalletId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BankPhoneNumber { get; set; }
        public string WalletPhoneNumber { get; set; }
    }
}
