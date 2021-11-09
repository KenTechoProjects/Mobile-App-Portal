using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YellowStone.Models.Enums
{
    public enum ActionReasons
    {
        [Description("Customer Validated")]
        CustomerValidated = 1,

        [Description("Errorneous Action")]
        ErrorneousAction,

        [Description("Other")]
        Other
    }
}
