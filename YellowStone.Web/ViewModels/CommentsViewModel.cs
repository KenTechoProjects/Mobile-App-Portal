using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.Enums;

namespace YellowStone.Web.ViewModels
{

    public class CommentsViewModel : BaseViewModel
    {
     public   List<CommentItems> commentsItems { get; set; }

        public CommentsViewModel(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }

    public class CommentItems
    {
        public long Id { get; set; }
        public string CommentText { get; set; }
        public RequestTypes  RequestTypes { get; set; }
        public string DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string RequestId { get; set; }
    }
}
