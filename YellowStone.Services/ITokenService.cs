using YellowStone.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YellowStone.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> ValidateToken(string staffId, string token);
    }
}
