using System;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Repository.Implementation;

namespace YellowStone.Repository.Interfaces
{
    public interface ITransactionAuditRepository : IRepository<TransactionAudit>
    {
        Task<TransactionAudit> GetByTransactionReference(string transactionReference);
    }
}
