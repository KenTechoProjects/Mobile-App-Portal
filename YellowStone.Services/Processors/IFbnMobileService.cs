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
   public interface IFbnMobileService
    {
        Task<Customer> GetCustomerInformation(string userName);
        Task<DeviceResponse> GetCustomerDevices(string userName);
       
        Task<OperationResponse> LockProfile(string userName);
        Task<OperationResponse> UnlockProfile(string userName);
        Task<OperationResponse> DeactivateDevice(string deviceId); 
        Task<OperationResponse> ReleaseDevice(string deviceId);
        Task<OperationResponse> ActivateDevice(string deviceId);
        Task<IEnumerable<Limit>> GetLimits();
        Task<OperationResponse> UpdateLimits(Limit limit);

    }
}   
