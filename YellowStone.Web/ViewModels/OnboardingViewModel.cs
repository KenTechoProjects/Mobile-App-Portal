using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Web.ViewModels;

namespace YellowStone.Web.ViewModels
{
    public class OnboardingViewModel : PageLayout
    {
        public List<OnboardingHistoryViewModel> OnboardingHistoryViewModel { get; set; }
    }

}
