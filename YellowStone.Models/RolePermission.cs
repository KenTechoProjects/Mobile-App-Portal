using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Models
{
    public class RolePermission :Base
    {
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    }
}
