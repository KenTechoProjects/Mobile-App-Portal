using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Services.FBNMobile.DTOs;
using YellowStone.Services.FIService.DTOs;
using YellowStone.Services.Onboarding.DTOs;

namespace YellowStone.Services.Processors
{
   public interface IOnboardingProcessor
    {
        Task<OnboardingResponse> InitiateOnboarding(string accountNumber, string branchCode);
        Task<OperationResponse> LinkAccount(string CustomerId, string WalletNumber);

        Task<OperationResponse> ResetProfile(string userName);
    }
}   
