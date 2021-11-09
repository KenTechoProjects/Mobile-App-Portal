using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YellowStone.Models.Enums
{
    public enum RequestStatus
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Active")]
        Active,

        [Description("Inactive")]
        Inactive,

        [Description("Rejected")]
        Rejected,

        [Description("Approved")]
        Approved,

        [Description("Closed")]
        Closed,

        [Description("Successful")]
        Successful,

        [Description("Failed")]
        Failed
    }
}
