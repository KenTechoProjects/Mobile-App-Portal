using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        IQueryable<User> Users { get; }
        //void LogOutOtherSessions(string staffId, string sessionId);
        Task<User> FindUserAsync(string staffId);
        Task<User> GetUserByStaffId(string staffId);
    }
}
