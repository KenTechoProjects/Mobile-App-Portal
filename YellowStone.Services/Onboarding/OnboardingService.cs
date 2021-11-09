using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Service.Utilities;
using YellowStone.Services.FBNMobile.DTOs;
using YellowStone.Services.Onboarding.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Onboarding
{
    public class OnboardingService : IOnboardingProcessor
    {
        private readonly HttpClient _client;
        private readonly OnboardingServiceSettings _settings;
        private readonly ILogger _logger;

        public OnboardingService(IOptions<OnboardingServiceSettings> settings,
            IHttpClientFactory factory, ILogger<OnboardingService> logger)
        {
            _settings = settings.Value;
            _client = factory.CreateClient("HttpMessageHandler");
            BuildOnboardingClient();
            _logger = logger;
        }

        private void BuildOnboardingClient()
        {
            _client.BaseAddress = new Uri(_settings.BaseUrl);
            _client.DefaultRequestHeaders.Add("AppId", _settings.AppId);
            _client.DefaultRequestHeaders.Add("AppKey", _settings.AppKey);
            _client.DefaultRequestHeaders.Add("lang", "en");

        }

        public async Task<OnboardingResponse> InitiateOnboarding(string accountNumber, string branchCode)
        {
            var request = new OnboardingResquest()
            {
                AccountNumber = accountNumber,
                BranchCode = branchCode
            };

            return await PostOnboardingMessage(request, "", _client);
        }

        public async Task<OperationResponse> ResetProfile(string username)
        {

            var operationResponse = new OperationResponse();
            var request = new
            {
                PhoneNumber = username,
                OtpTargetFeature = "ResetSecurityQuestions"
            };

            var result = await PostLinkAccountMessage(request, "customer/otp", _client);
            operationResponse.IsSuccessful = result.IsSuccessful;
            operationResponse.Error = new ErrorResponse() { ResponseCode = result.Code, ResponseDescription = result.Message };

            return operationResponse;

            // return await PostMessage(payload, "customer/otp", _client);
        }


        private async Task<OnboardingResponse> PostOnboardingMessage(OnboardingResquest request, string path, HttpClient client)
        {
            var message = new StringContent(Util.SerializeAsJson(request), Encoding.UTF8, "application/json");
            var rawResponse = await client.PostAsync(path, message);
            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = Util.DeserializeFromJson<OnboardingResponse>(body);
            if (rawResponse.IsSuccessStatusCode)
            {
                response.IsSuccessful = true;
            }
            _logger.LogInformation($"body: {body}");
            return response;
        }


        public async Task<OperationResponse> LinkAccount(string CustomerId, string WalletNumber)
        {
            var operationResponse = new OperationResponse();
            var request = new
            {
                walletId = WalletNumber,
                customerId = CustomerId
            };

            var result = await PostLinkAccountMessage(request, "customer/link", _client);
            operationResponse.IsSuccessful = result.IsSuccessful;
            operationResponse.Error = new ErrorResponse() { ResponseCode = result.Code, ResponseDescription = result.Message };

            return operationResponse;
        }

        private async Task<LinkAccountResponse> PostLinkAccountMessage(dynamic request, string path, HttpClient client)
        {
            var response = new LinkAccountResponse(false);

            var message = new StringContent(Util.SerializeAsJson(request), Encoding.UTF8, "application/json");
            var rawResponse = await client.PostAsync(path, message);
            if (rawResponse.IsSuccessStatusCode)
            {
                response.IsSuccessful = true;
            }
            else
            {
                response.Message = "Request Failed";
            }
        
            return response;
        }

    }
}
