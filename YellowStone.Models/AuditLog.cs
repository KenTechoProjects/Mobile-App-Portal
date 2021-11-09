using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YellowStone.Models
{
    public class AuditLog : Base
    {
        public string StaffId { get; set; }
        public string IpAddress { get; set; }
        public string AccountNumber { get; set; }
        public string UserBranch { get; set; }
        public string ActionPerformed { get; set; }
        public string UrlAccessed { get; set; }
        public string Extra { get; set; }
    }
}
