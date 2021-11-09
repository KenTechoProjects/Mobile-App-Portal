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
    public class LimitsRepository : Repository<LimitRequest>, ILimitRepository
    {
        private readonly FBNMDashboardContext context;

        public LimitsRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<LimitRequest> Limits => context.Limits
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

    }
}
