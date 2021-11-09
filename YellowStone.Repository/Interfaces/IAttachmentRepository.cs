using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;

namespace YellowStone.Repository.Interfaces
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        IQueryable<Attachment> Attachments { get; }

    }
}
