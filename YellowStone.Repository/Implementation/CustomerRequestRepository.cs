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
    public class CustomerRequestRepository : Repository<CustomerRequest>, ICustomerRequestRepository
    {
        private readonly FBNMDashboardContext context;

        public CustomerRequestRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<CustomerRequest> CustomerRequests => context.CustomerRequests
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

    }
}
