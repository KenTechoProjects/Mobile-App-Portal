using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
//using YellowStone.Services.Interfaces;
namespace YellowStone.Services.Mock
{
    public class MockTokenService : ITokenService
    {
        public async Task<TokenResponse> ValidateToken(string staffId, string token)
        {
            var result = new TokenResponse(false);
            if (token == "123456")
            {
                result.IsSuccessful = true;
            }
            else
            {
                result.Message = "Invalid token";
            }
            return await Task.FromResult(result);
        }
    }
}
