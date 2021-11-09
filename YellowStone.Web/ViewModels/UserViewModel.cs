using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Web.ViewModels
{
    public class UserViewModel : PageLayout
    {
        public List<User> Users { get; set; }
        public User User { get; set; }
        public List<User> UserList { get; set; }
        public List<DepartmentSummary> DepartmentSummaries { get; set; }
    }
}
