using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YellowStone.Models.Enums;

namespace YellowStone.Models
{
    public class Comment : Base
    {
        public long RequestId { get; set; }
        public string CommentText { get; set; }
        public RequestTypes RequestType { get; set; }

    }
}
