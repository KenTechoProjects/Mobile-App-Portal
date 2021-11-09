using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.Enums;

namespace YellowStone.Extensions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(int permission)
        {
            Permission = permission;
        }

        public int Permission { get; }
    }

   
}
