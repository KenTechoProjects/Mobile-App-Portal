using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FIService.DTOs
{
    public class AccountDetailsResponse : BaseResponse
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
    }
}
