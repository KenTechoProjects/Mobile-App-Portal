using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowStone.Models;

namespace YellowStone.Repository.Interfaces
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        IQueryable<Permission> Permissions { get; }
    }
}
