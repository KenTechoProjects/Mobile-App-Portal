using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Web.ViewModels
{
    public class AuditLogViewModel : PageLayout
    {
       // public PaginatedList<AuditLog> AuditLogList { get; set; }
         public List<AuditLog> AuditLogList { get; set; }
        public AuditLog AuditLog { get; set; }
    }
}
