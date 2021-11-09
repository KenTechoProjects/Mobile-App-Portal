using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository.Implementation
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {

        private readonly FBNMDashboardContext context;

        public PermissionRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<Permission> Permissions => context.Permissions;
    }
}
