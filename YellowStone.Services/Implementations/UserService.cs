using System.IO;
using YellowStone.Models;
using YellowStone.Models.DTO;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IFbnService _fbnService;

        public UserService(IFbnService fbnService)
        {
            _fbnService = fbnService;
        }

        public async Task<LoginResponse> Authenticate(string username, string password)
        {
            var result = new LoginResponse(false);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var serviceResponse = await _fbnService.ValidateUser(username, password);


                if (serviceResponse.IsSuccessful())
                {
                    result.IsSuccessful = true;
                    result.Message = serviceResponse.ResponseMessage;
                    result.StaffId = serviceResponse.Username;
                    result.Department = serviceResponse.Department;
                }
                else
                {
                    result.Message = "Invalid credentials";
                   }

                return await Task.FromResult(result);
            }

            return result;
        }

        public async Task<FinacleDetails> GetFinacleDetails(string username)
        {
            var result = new FinacleDetails(false);
            if (!string.IsNullOrEmpty(username))
            {
                var serviceResponse = await _fbnService.GetFinacleDetails(username);


                if (serviceResponse.IsSuccessful())
                {
                    result.IsSuccessful = true;
                    result.Message = serviceResponse.ResponseMessage;
                    result.RoleId = serviceResponse.RoleId;
                    result.FinacleUserId = serviceResponse.FinacleUserId;
                    result.BranchCode = serviceResponse.BranchCode;
                    result.BranchName = serviceResponse.BranchName;
                }
                else
                {
                    result.Message = "Invalid credentials";
                }

                return await Task.FromResult(result);
            }

            return result;
        }

        public async Task<UserDetails> Validate(string username)
        {
            var result = new UserDetails(false);
            if (!string.IsNullOrEmpty(username))
            {
                var serviceResponse = await _fbnService.GetStaffDetails(username);


                if (serviceResponse.IsSuccessful())
                {
                    result.IsSuccessful = true;
                    result.StaffId = serviceResponse.Username;
                    result.Department = serviceResponse.Department;
                    result.ManagerName = serviceResponse.ManagerName;
                    result.ManagerDepartment = serviceResponse.ManagerDepartment;
                    result.MobileNo = serviceResponse.MobileNo;
                    result.Groups = serviceResponse.Groups;
                    result.DisplayName = serviceResponse.DisplayName;
                    result.Email = serviceResponse.Email;

                    var finDetails = await _fbnService.GetFinacleDetails(username);
                    if (finDetails.IsSuccessful())
                    {
                        result.BranchCode = finDetails.BranchCode;

                    }
                    else
                    {
                        result.BranchCode = "No BranchCode Found For User";
                    }
                }
                else
                {
                    result.Message = serviceResponse.ResponseMessage;
                }

                    return await Task.FromResult(result);
                }

                return result;
            }

      

    }
}

