using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.Enums;

namespace YellowStone.Web.ViewModels
{
    public class CustomerRequestHistory
    {
        public long Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public RequestStatus Activity { get; set; }
        public string AccountBranchCode { get; set; }
        public string InitiatedBy { get; set; }
        public string DateApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string WalletId { get; set; }
        public RequestTypes RequestType { get; set; }
    }

}
