using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.Onboarding.DTOs
{
    public class LinkAccountResponse : IOnboardingBaseResponse
    {
        public bool IsSuccessful { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public LinkAccountResponse(bool isSuccessful)
        {
            this.IsSuccessful = isSuccessful;
        }
    }
}
