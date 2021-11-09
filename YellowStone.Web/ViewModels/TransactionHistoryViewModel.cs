using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels
{
    public class TransactionHistoryViewModel 
    {
        public string TransactionType { get; set; }
        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }

    }
}
