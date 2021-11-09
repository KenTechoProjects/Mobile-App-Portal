using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Web.ViewModels
{
    public class SaveRoleViewModel
    {
        public Role role { get; set; }
        public List<PermissionViewList> PermissionsList { get; set; }
        public List<Department> DepartmentList { get; set; }

        public long DepartmentId { get; set; }
        public string RoleType { get; set; }
        public long[] Selected { get; set; }

        public List<RoleType> RoleTypes
        {
            get
            {
                return new List<RoleType>()
                {
                    new RoleType() { Name = "AdminInitiator" },
                    new RoleType() { Name = "NonAdminInitiator" },
                    new RoleType() { Name = "AdminApprover" },
                    new RoleType() { Name = "NonAdminApprover" },
                    new RoleType() { Name = "Viewer" }
                };
            }
        }

    }

    public class RoleType
    {
        public string Name { get; set; }
    }




}
