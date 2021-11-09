using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Models;

namespace Apollo.ViewModels
{
    public class ContactApproveViewModel : PageLayout
    {

        public List<CustomerHistoryView> CustomerHistoryViews { get; set; }
        public List<CusInformationHistory> CusInformationHistories { get; set; }
        public List<TransactionView> TransactionViews { get; set; }

        public List<NotificationView> NotificationViews { get; set; }
        public PaginatedList<NotificationView> NotificationViewsList { get; set; }
    }
}
