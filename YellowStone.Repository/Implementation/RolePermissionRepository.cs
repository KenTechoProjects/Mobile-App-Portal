using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository
{
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
    {
        private readonly FBNMDashboardContext context;

        public RolePermissionRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<RolePermission> RolePermission => context.RolePermissions.AsNoTracking().OrderByDescending(x => x.PermissionId).Include(x => x.Role).Include(x => x.Permission);

        public bool CheckPermissionForUser(User user, long permission)
        {
            var rolePermission = context.RolePermissions.Where(x => x.RoleId == user.RoleId && x.PermissionId == permission).SingleOrDefault();
            if (rolePermission != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SaveRolePermission(Role role, List<Permission> permissions)
        {
            bool success = false;

            if (role != null && permissions != null)
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (role.Id == 0)
                        {
                            context.Add(role);
                        }
                        else
                        {
                            context.Update(role);
                            List<RolePermission> rolePermissionList = new List<RolePermission>();
                            rolePermissionList = context.RolePermissions.Where(x => x.RoleId == role.Id).ToList();

                            foreach (var rPermission in rolePermissionList)
                            {
                                context.Remove(rPermission);
                            }
                        }

                        if (permissions.Count() > 0)
                        {

                            foreach (Permission permission in permissions)
                            {
                                RolePermission rolePermission = new RolePermission();
                                rolePermission.RoleId = role.Id;
                                rolePermission.PermissionId = permission.Id;
                                rolePermission.CreatedBy = role.CreatedBy;
                                rolePermission.CreatedDate = DateTime.Now;
                                context.Add(rolePermission);
                            }

                        }


                        await context.SaveChangesAsync();
                        trans.Commit();

                        success = true;

                    }
                    catch (Exception er) { trans.Rollback(); throw er; }
                }

            }

            return success;
        }

    }
}
