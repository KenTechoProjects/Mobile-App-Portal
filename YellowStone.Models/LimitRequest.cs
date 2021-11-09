using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YellowStone.Models.Enums;

namespace YellowStone.Models
{
    public class LimitRequest : Base
    {
        public TransactionType TransactionType { get; set; }
        public decimal SingleLimit { get; set; }
        public decimal DailyLimit { get; set; }
        public RequestStatus Status { get; set; }

    }
}
