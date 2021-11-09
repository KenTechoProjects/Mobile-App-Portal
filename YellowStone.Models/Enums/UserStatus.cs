using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YellowStone.Models.Enums
{
    public enum UserStatus
    {
        [Description("Inactive")]
        Inactive,

        [Description("Active")]
        Active,

        [Description("Pending Approval")]
        Pending_Approval,

        [Description("Locked")]
        Locked,

        [Description("Pending Unlock")]
        Pending_Unlock,

        [Description("Pending Deactivation")]
        Pending_Deactivation,

        [Description("Rejected")]
        Rejected,

        [Description("Dormant")]
        Dormant
    }
}
