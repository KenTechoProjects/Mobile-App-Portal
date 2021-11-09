using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels

{
    public class PermissionModel
    {
        public bool Roles { get; set; } = false;
        public bool Departments { get; set; } = false;
        public bool Users { get; set; } = false;
        public bool Resources { get; set; } = false;

        public bool ApproveRoles { get; set; } = false;
        public bool ApproveDepartments { get; set; } = false;
        public bool ApproveUsers { get; set; } = false;
    }
}
