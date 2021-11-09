using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YellowStone.Models.Enums
{
    public enum RequestTypes
    {
        [Description("Onboarding")]
        Onboarding = 1,

        [Description("Reset Profile")]
        ResetProfile,

        [Description("Lock Profile")]
        LockProfile,

        [Description("Unlock Profile")]
        UnlockProfile,

        [Description("Deactivate Device")]
        DeactivateDevice,

        [Description("Activate Device")]
        ActivateDevice,

        [Description("Fund Wallet")]
        FundWallet,

        [Description("Link Account")]
        LinkAccount,

        [Description("Release Device")]
        ReleaseDevice
    }
}
