using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FBNMobile
{
    public class FbnMobileSettings
    {
        public string BaseAddress { get; set; }

    }

    public class FbnMobileMiddlewareSettings
    {
        public string BaseUrl { get; set; }
    }
}


public class Rootobject
{
    public Statement[] statement { get; set; }
}

public class Statement
{
    public string amount { get; set; }
    public string currency { get; set; }
    public string narration { get; set; }
    public string txnstamp { get; set; }
    public string drcrflag { get; set; }
}
