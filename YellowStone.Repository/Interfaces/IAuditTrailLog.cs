using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowStone.Models;

namespace YellowStone.Repository.Interfaces
{
    public interface IAuditTrailLog : IRepository<AuditLog>
    {
        void SaveAuditTrailLog(string staffId, string IpAddress, string action, string urlAccessed);
        IQueryable<AuditLog> AuditLogs { get; }
       // IEnumerable<AuditLog> AuditLogss { get; }
    }
}
