using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FIService.DTOs
{
    public class AccountDetailsRequest
    {
        public string RequestId { get; set; }
        public string CountryId { get; set; }
        public string AccountNumber { get; set; }

    }
}
