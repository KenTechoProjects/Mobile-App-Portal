using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YellowStone.Models.Enums
{
    public enum TransactionStatus
    {
        New = 1,
        Pending,
        Successful,
        Failed
    }

    public enum TransactionType
    {
        [Description("National Transfer")]
        NationalTransfer = 1,

        [Description("Self Transfer")]
        SelfTransfer,

        [Description("Subsidiary Transfer")]
        SubsidiaryTransfer,

        [Description("Airtime Transfer")]
        Airtime,

        [Description("Bill Payments")]
        BillPayment
    }
}
