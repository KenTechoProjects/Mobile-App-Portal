using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository.Implementation
{
    public class TransactionAuditRepository : Repository<TransactionAudit>, ITransactionAuditRepository
    {
        private readonly FBNMDashboardContext context;
        public TransactionAuditRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        private IQueryable<TransactionAudit> TransactionAudit => context.TransactionAudit
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

        public Task<TransactionAudit> GetByTransactionReference(string transactionReference)
        {
            var result = context
                .TransactionAudit.Where(x => x.TransactionReference == transactionReference).SingleOrDefaultAsync();
            return result;
        }
    }
}
