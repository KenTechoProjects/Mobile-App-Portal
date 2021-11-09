using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Services.FBNMobile.DTOs;
using YellowStone.Services.FBNService.DTOs;
using YellowStone.Services.Onboarding.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Mock
{
    public class MockOnboardingService : IOnboardingProcessor
    {


        public async Task<OnboardingResponse> InitiateOnboarding(string accountNumber, string branchCode)
        {
            var response = new OnboardingResponse(true)
            {
                Email = "email@email.com",
                AccountNumber = accountNumber,
                CifId = "123456",
                FirstName = "OLANREWAJU",
                LastName = "OKANRENDE",
                MiddleName = "OPEYEMI",
                PhoneNumber = "08029039468"
            };

            return await Task.FromResult(response);
        }

        public async Task<OperationResponse> LinkAccount(string CustomerId, string WalletNumber)
        {
            var response = new OperationResponse() { IsSuccessful = true };
            return await Task.FromResult(response);

        }

        public async Task<OperationResponse> ResetProfile(string userName)
        {
            var response = new OperationResponse() { IsSuccessful = true };
            return await Task.FromResult(response);
        }
    }
}
