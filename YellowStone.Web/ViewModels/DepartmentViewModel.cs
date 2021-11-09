using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Web.ViewModels
{
    public class DepartmentViewModel : PageLayout
    {
        public List<Department> Departments { get; set; }
        public Department Department { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<Role> Roles { get; set; }
    }


}
