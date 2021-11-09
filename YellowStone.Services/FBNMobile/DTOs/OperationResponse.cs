using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FBNMobile.DTOs
{
    public class OperationResponse
    {
        public bool IsSuccessful { get; set; }
        public ErrorResponse Error { get; set; }
    }

    public class ErrorResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }
}
