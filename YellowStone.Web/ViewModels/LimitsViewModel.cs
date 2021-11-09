using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.Enums;

namespace YellowStone.Web.ViewModels
{
    public class LimitsViewModel 
    {
      
        public TransactionType TransactionType { get; set; }
        public decimal SingleLimit { get; set; }
        public decimal DailyLimit { get; set; }
        public long ID { get; set; }
    }
}
