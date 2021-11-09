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
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly FBNMDashboardContext context;

        public RoleRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<Role> Roles => context.Roles
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Include(x => x.Department)
            .Include(x => x.RolePermissions);

        public async Task activateRole(long roleId)
        {
            Role role = await context.Roles.SingleAsync(x => x.Id == roleId);
            role.Status = RequestStatus.Active;
            context.Roles.Update(role);
            await context.SaveChangesAsync();
        }

        public async Task deactivateRole(long roleId) { 


            var users =  context.Users.Where(x => x.RoleId == roleId && x.Status == UserStatus.Active).ToList();
            Role role = await context.Roles.SingleAsync(x => x.Id == roleId);
            if (users.Count() == 0)
            {
                role.Status = RequestStatus.Inactive;
            }
            else {
                role.Status = RequestStatus.Active;
            }
            context.Roles.Update(role);
            await context.SaveChangesAsync();
       
        }
    }
}
