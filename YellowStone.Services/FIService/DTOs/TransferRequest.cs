using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FIService.DTOs
{
    public class TransferRequest
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber{ get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public string IsCustomerInduced => "Y";
        public string CountryId { get; set; }
        public string ClientReferenceId { get; set; }

    }
}
