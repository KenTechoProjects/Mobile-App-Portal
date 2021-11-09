using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.Enums;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository.Implementation
{
    public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
    {
        private readonly FBNMDashboardContext context;

        public AttachmentRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<Attachment> Attachments => context.Attachments
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

    }
}
