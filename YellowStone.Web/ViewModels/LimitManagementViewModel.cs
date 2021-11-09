using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Services.FBNMobile.DTOs;

namespace YellowStone.Web.ViewModels
{
    public class LimitManagementViewModel : PageLayout
    {
        public User User { get; set; }
        public IEnumerable<LimitsViewModel> Limits { get; set; }
        public IEnumerable<LimitRequest> LimitRequests { get; set; }

        public LimitManagementViewModel()
        {
            LimitRequests = new List<LimitRequest>();
            Limits = new List<LimitsViewModel>();
        }
    }
}
