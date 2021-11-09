using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.Enums;

namespace YellowStone.Web.ViewModels
{
    public class ApprovalViewModel
    {
        public long? Id { get; set; }
        public string Comment { get; set; }
        public RequestTypes RequestType  { get; set; }
}
}
