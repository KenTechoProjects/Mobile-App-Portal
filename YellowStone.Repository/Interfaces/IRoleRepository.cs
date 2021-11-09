using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Repository.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        IQueryable<Role> Roles { get; }
        Task activateRole(long roleId);
        Task deactivateRole(long roleId);

    }
}
