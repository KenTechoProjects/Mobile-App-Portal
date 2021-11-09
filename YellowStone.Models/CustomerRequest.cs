using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YellowStone.Models.Enums;

namespace YellowStone.Models
{
    public class CustomerRequest : Base
    {
        [StringLength(50)]
        [Required]
        public string AccountNumber { get; set; }
        public string WalletNumber { get; set; }
        public string AccountName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceModel { get; set; }
        public string CustomerId { get; set; }
        public string PhoneModel { get; set; }
        public RequestTypes RequestType { get; set; }
        public string AccountBranchCode { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string RequestBranchCode { get; set; }
        public RequestState RequestState { get; set; }
        public ActionReasons ActionReason { get; set; }
        public string CallingPhoneNumber { get; set; }

    }
}
