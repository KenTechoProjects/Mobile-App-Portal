using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Web.ViewModels
{

    public class RoleViewModel : PageLayout
    {
        public List<Permission> Permissions { get; set; }
        public List<Role> RoleList { get; set; }
        public List<Role> Roles { get; set; }
        public Role Role { get; set; }
        public SaveRoleViewModel SaveRole { get; set; }

        public List<PermissionCategory> PermissionCategories
        {
            get
            {
                return new List<PermissionCategory>()
                {
                    new PermissionCategory { ID = "Initiation", Name = "Initiation" },
                    new PermissionCategory { ID = "Approval", Name = "Approval" },
                    new PermissionCategory { ID = "SystemControl", Name = "SystemControl" }
                };
            }
        }
        public class PermissionCategory
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
    }
}
