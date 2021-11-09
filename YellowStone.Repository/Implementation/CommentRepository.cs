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
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly FBNMDashboardContext context;

        public CommentRepository(FBNMDashboardContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<Comment> Comments => context.Comments
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

    }
}
