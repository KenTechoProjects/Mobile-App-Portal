using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Models;

namespace Apollo.ViewModels
{
    public class ContactViewModel : PageLayout
    {
        public List<CustomerView> CustomerViews { get; set; }
        public List<CusInformationHistory> CusInformationHistories { get; set; }
        public List<TransactionView> TransactionViews { get; set; }
        // public int NotificationCount { get; set; } = 0;
        public List<NotificationView> NotificationViews { get; set; }
        public PaginatedList<NotificationView> NotificationViewsList { get; set; }
    }

    //public class DepartmentViewModel : PageLayout
    //{
    //    public List<Department> Departments { get; set; }
    //    public Department Department { get; set; }
    //    public PaginatedList<Department> DepartmentList { get; set; }
    //    public List<Role> Roles { get; set; }
    //}

}
