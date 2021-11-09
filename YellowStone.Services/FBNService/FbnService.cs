using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Service.Utilities;
using YellowStone.Services.FBNService.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.FBNService
{
    public class FbnService : IFbnService
    {
        private readonly HttpClient _client;
        private readonly FbnServiceSettings _fbnSettings;
        private readonly SystemSettings _systemSettings;
        private readonly ILogger _logger;

        public FbnService(IOptions<FbnServiceSettings> settingsProvider,
            IOptions<SystemSettings> systemSettings,
            IHttpClientFactory factory, ILogger<FbnService> logger)
        {
            _fbnSettings = settingsProvider.Value;
            _systemSettings = systemSettings.Value;
            _client = factory.CreateClient("HttpMessageHandler");
            BuildFbnClient();
            _logger = logger;
        }

        private void BuildFbnClient()
        {
            _client.BaseAddress = new Uri(_fbnSettings.BaseAddress);
            _client.DefaultRequestHeaders.Add("AppId", _fbnSettings.AppId);
            _client.DefaultRequestHeaders.Add("AppKey", _fbnSettings.AppKey);
        }


        public async Task<FinacleDetailsResponse> GetFinacleDetails(string userName)
        {
            var request = new FinacleDetailsRequest()
            {
                CountryId = _systemSettings.CountryId,
                RequestId = GenerateReference(),
                Username = userName
            };

            return await PostMessage<FinacleDetailsResponse, FinacleDetailsRequest>(request, "user/get-user-finacle-details", _client);
        }

        public async Task<UserDetailsResponse> ValidateUser(string userName, string password)
        {
            var request = new UserDetailsRequest()
            {
                CountryId = _systemSettings.CountryId,
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password)),
                RequestId = GenerateReference(),
                Username = userName
            };

            var result= await PostMessage<UserDetailsResponse, UserDetailsRequest>(request, "user/validate-user", _client);

            return result;
        }

        private static string GenerateReference()
        {
            return Guid.NewGuid().ToString();
        }
        private async Task<T> PostMessage<T, U>(U request, string path, HttpClient client)
        {
            var message = new StringContent(Util.SerializeAsJson<U>(request), Encoding.UTF8, "application/json");
            var rawResponse = await client.PostAsync(path, message);
            _logger.LogInformation($"rawResponse : {Util.SerializeAsJson(rawResponse)}");

            if (!rawResponse.IsSuccessStatusCode)
            {
                throw new Exception("Service invocation failure");
            }
            var body = await rawResponse.Content.ReadAsStringAsync();
            _logger.LogInformation($"body: {body}");
            return Util.DeserializeFromJson<T>(body);
        }

        public async Task<UserDetailsResponse> GetStaffDetails(string userName)
        {
            var request = new UserDetailsRequest()
            {
                CountryId = _systemSettings.CountryId,
                RequestId = GenerateReference(),
                Username = userName
            };

            return await PostMessage<UserDetailsResponse, UserDetailsRequest>(request, "user/get-user-details-with-username", _client);
        }

        public Task<TellerTillResponse> GetTellerTillAccountAsync(TellerTillRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
