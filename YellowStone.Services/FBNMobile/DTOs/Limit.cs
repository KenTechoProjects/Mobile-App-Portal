using System;
using System.Collections.Generic;
using System.Text;
using YellowStone.Models.Enums;

namespace YellowStone.Services.FBNMobile.DTOs
{
    public class Limit
    {
        public long ID { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal SingleLimit { get; set; }
        public decimal DailyLimit { get; set; }
    }
}
