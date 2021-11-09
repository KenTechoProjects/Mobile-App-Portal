using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.Enums;

namespace YellowStone.Web.ViewModels
{
    public class InitiateOnboardingViewModel
    {
    
        public long? Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BranchCode { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
    }
}
