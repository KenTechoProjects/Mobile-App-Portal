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
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly FBNMDashboardContext context;

        public UserRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<User> Users => context.Users.OrderBy(x => x.Name)
            .Include(x => x.Role);
        

        public async Task<User> FindUserAsync(string staffId)
        {
            var activeStatuses = new List<int>
            {
                (int)UserStatus.Active,
                (int)UserStatus.Locked,
                (int)UserStatus.Dormant,
                (int)UserStatus.Pending_Unlock
            };
            var user = await context.Users.FirstOrDefaultAsync(x => x.StaffId == staffId && activeStatuses.Contains((int)x.Status));

            if (user == null)
            {
                return null;
            }
            var activeRoles = new List<int>
            {
               (int)RequestStatus.Active, (int)RequestStatus.Approved
            };

            var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);

            if (role == null)
            {
                return null;
            }

            if (!activeRoles.Contains((int)role.Status))
            {
                return null;

            }

            if (!string.Equals(role.Name, "SuperAdmin"))
            {
                var department = await context.Departments.FirstOrDefaultAsync(x => x.Id == role.DepartmentId);

                if (department == null)
                {
                    return null;
                }

                if (!activeRoles.Contains((int)department.Status))
                {
                    return null;

                }

            }


            return user;

        }

        public async Task<User> GetUserByStaffId(string staffId)
        {
            var activeStatuses = new List<int>
            {
                (int)UserStatus.Active,
                (int)UserStatus.Locked,
                (int)UserStatus.Pending_Unlock
            };
            return await context.Users.FirstOrDefaultAsync(x => x.StaffId == staffId && activeStatuses.Contains((int)x.Status));

         

        }
    }

}

