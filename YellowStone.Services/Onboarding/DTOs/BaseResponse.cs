using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.Onboarding.DTOs
{
    public class BaseResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool IsSuccessful()
        {
            return ResponseCode == "00";
        }
    }
}
