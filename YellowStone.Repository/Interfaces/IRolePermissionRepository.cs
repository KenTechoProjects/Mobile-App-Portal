using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.Enums;

namespace YellowStone.Repository.Interfaces
{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        IQueryable<RolePermission> RolePermission { get; }
        Task<bool> SaveRolePermission(Role role, List<Permission> resources);
        bool CheckPermissionForUser(User user, long permission);
    }
}
