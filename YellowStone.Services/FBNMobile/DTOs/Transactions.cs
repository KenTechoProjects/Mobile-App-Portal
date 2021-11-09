using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using YellowStone.Models.Enums;

namespace YellowStone.Services.FBNMobile.DTOs
{
   public class Transactions
    {
        public TransactionType TransactionType { get; set; }
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
    }

   
}
