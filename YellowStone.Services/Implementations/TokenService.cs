using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.DTO;
using YellowStone.Services.Entrust;
//using YellowStone.Services.Interfaces;

namespace YellowStone.Services.Implementations
{
    
    public class TokenService : ITokenService
    {
        private readonly EntrustSettings _configSettings;
        private readonly EntrustService _service;

        public TokenService(IOptions<EntrustSettings> appSettings)
        {
            _configSettings = appSettings.Value;
            _service = new EntrustService(_configSettings.EntrustUrl);
        }
        public async Task<TokenResponse> ValidateToken(string staffId, string token)
        {
            var result = new TokenResponse(false);

            var response = await _service.ValidateToken(staffId, token);

            result.IsSuccessful = response.Authenticated;
            result.Message = response.Message;
            return result;
        }
    }
}
