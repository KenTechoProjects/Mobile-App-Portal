using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Service.Utilities;
using YellowStone.Services.FBNMobile.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.FBNMobile
{
    public class FbnMobileService : IFbnMobileService
    {
        private readonly HttpClient _client;
        private readonly FbnMobileSettings _settings;
        private readonly ILogger _logger;

        public FbnMobileService(IOptions<FbnMobileSettings> settings,
            IHttpClientFactory factory, ILogger<FbnMobileService> logger)
        {
            _settings = settings.Value;
            _client = factory.CreateClient("HttpMessageHandler");
            BuildOnboardingClient();
            _logger = logger;
        }

        private void BuildOnboardingClient()
        {
            _client.BaseAddress = new Uri(_settings.BaseAddress);
        }

        public async Task<Customer> GetCustomerInformation(string username)
        {
            _logger.LogInformation($"USERNAME: {username}");
            var rawResponse = await _client.GetAsync($"customer/details/{username}");
            var body = await rawResponse.Content.ReadAsStringAsync();
            _logger.LogInformation($"RESPONSE_MESSAGE: {body}");
            var response = Util.DeserializeFromJson<Customer>(body);
            if (rawResponse.IsSuccessStatusCode)
            {
                response.IsSuccessful = true;
            }
            _logger.LogInformation($"body: {body}");
            return response;
        }



       
        public async Task<OperationResponse> LockProfile(string username)
        {
            var payload = new
            {
                username
            };
            return await PutMessage(payload, "customer/profile/lock", _client);

        }

        public async Task<OperationResponse> UnlockProfile(string username)
        {
            var payload = new
            {
                username
            };
            return await PutMessage(payload, "customer/profile/unlock", _client);

        }


        private async Task<OperationResponse> PutMessage<T>(T request, string path, HttpClient client)
        {
            var message = new StringContent(Util.SerializeAsJson(request), Encoding.UTF8, "application/json");
            var rawResponse = await client.PutAsync(path, message);
            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = new OperationResponse();
            if (rawResponse.IsSuccessStatusCode)
            {
                response.IsSuccessful = true;
            }
            else
            {
                response = Util.DeserializeFromJson<OperationResponse>(body);
            }
            _logger.LogInformation($"body: {body}");
            return response;
        }

        private async Task<OperationResponse> PostMessage<T>(T request, string path, HttpClient client)
        {
            var message = new StringContent(Util.SerializeAsJson(request), Encoding.UTF8, "application/json");
            var rawResponse = await client.PostAsync(path, message);
            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = new OperationResponse();
            if (rawResponse.IsSuccessStatusCode)
            {
                response.IsSuccessful = true;
            }
            else
            {
                response = Util.DeserializeFromJson<OperationResponse>(body);
            }
            _logger.LogInformation($"body: {body}");
            return response;
        }

        private async Task<Customer> Get(string path, HttpClient client)
        {
            var rawResponse = await client.GetAsync(path);
            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = Util.DeserializeFromJson<Customer>(body);
            if (rawResponse.IsSuccessStatusCode)
            {
                response.IsSuccessful = true;
            }
            _logger.LogInformation($"body: {body}");
            return response;
        }

        public async Task<IEnumerable<Limit>> GetLimits()
        {
            var rawResponse = await _client.GetAsync("");
            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = Util.DeserializeFromJson<IEnumerable<Limit>>(body);
            if (rawResponse.IsSuccessStatusCode)
            {
                //response.IsSuccessful = true;
            }
            _logger.LogInformation($"body: {body}");
            return response;
        }

        public async Task<OperationResponse> UpdateLimits(Limit limits)
        {
            return await PutMessage(limits, "", _client);

        }


        public async Task<DeviceResponse> GetCustomerDevices(string username)
        {
            var result = new DeviceResponse(false);
            var rawResponse = await _client.GetAsync($"customer/devices/{username}");
            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = Util.DeserializeFromJson<IEnumerable<Device>>(body);
            if (rawResponse.IsSuccessStatusCode)
            {
                result.IsSuccessful = true;
                result.Devices = response;
            }
            _logger.LogInformation($"body: {body}");
            return result;
        }

        public async Task<OperationResponse> DeactivateDevice(string deviceId)
        {
            var payload = new
            {
                deviceId
            };
            return await PutMessage(payload, "customer/device/deactivate", _client);

        }

        public async Task<OperationResponse> ActivateDevice(string deviceId)
        {
            var payload = new
            {
                deviceId
            };
            return await PutMessage(payload, "customer/device/activate", _client);

        }

        public async Task<OperationResponse> ReleaseDevice(string deviceId)
        {
            var payload = new
            {
                deviceId
            };
            return await PutMessage(payload, "customer/device/release", _client);

        }
    }
}
