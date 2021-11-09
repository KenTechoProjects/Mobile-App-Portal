using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.Onboarding.DTOs
{
    public class OnboardingResponse : IOnboardingBaseResponse
    {
        public bool IsSuccessful { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CifId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public OnboardingResponse(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }

    public interface IOnboardingBaseResponse
    {
        bool IsSuccessful { get; set; }
        string Code { get; set; }
        string Message { get; set; }

    }

}
