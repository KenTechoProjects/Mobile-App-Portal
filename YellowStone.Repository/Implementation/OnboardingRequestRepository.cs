using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository.Implementation
{
    public class OnboardingRequestRepository : Repository<OnboardingRequest>, IOnboardingRequestRepository
    {
        private readonly FBNMDashboardContext context;

        public OnboardingRequestRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<OnboardingRequest> OnboardingRequests => context.OnboardingRequests
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

    }
}
