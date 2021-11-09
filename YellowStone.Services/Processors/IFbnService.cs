using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Services.FBNService.DTOs;

namespace YellowStone.Services.Processors
{
    public interface IFbnService
    {
        Task<UserDetailsResponse> ValidateUser(string userName, string password);
        Task<UserDetailsResponse> GetStaffDetails(string userName);
        Task<FinacleDetailsResponse> GetFinacleDetails(string userName);
        Task<TellerTillResponse> GetTellerTillAccountAsync(TellerTillRequest request);
        //Task<WalletDetailResponse>
    }
}
