using YellowStone.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Services.FBNService.DTOs;

namespace YellowStone.Services
{
    public interface IUserService
    {
        Task<LoginResponse> Authenticate(string username, string password);
        Task<UserDetails> Validate(string username);
        Task<FinacleDetails> GetFinacleDetails(string username);
      
    }
}
