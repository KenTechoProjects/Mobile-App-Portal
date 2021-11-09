using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository.Implementation
{
    public class AuditTrailLogRepository : Repository<AuditLog>, IAuditTrailLog
    {
        private readonly FBNMDashboardContext context;

        public AuditTrailLogRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public void SaveAuditTrailLog(string staffId, string IpAddress, string action, string urlAccessed)
        {
            var log = new AuditLog
            {
                StaffId = staffId,
                IpAddress = IpAddress,
                ActionPerformed = action,
                UrlAccessed = urlAccessed,
                CreatedDate = DateTime.Now
            };
            context.Add(log);
            context.SaveChanges();
        }

        public IQueryable<AuditLog> AuditLogs => context.AuditLogs.OrderByDescending(x => x.CreatedDate);

       // public IEnumerable<AuditLog> AuditLogss => context.AuditLogs.OrderByDescending(x => x.CreatedDate).ToList();
    }
}
