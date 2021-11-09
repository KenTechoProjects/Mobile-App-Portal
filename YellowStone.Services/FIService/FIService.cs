using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.DTO;
using YellowStone.Repository.Interfaces;
using YellowStone.Service.Utilities;
using YellowStone.Services.FBNService.DTOs;
using YellowStone.Services.FIService.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.FBNService
{
    public class FIService : IFIService
    {
        private readonly HttpClient _client;
        private readonly FISettings _fiSettings;
        private readonly SystemSettings _systemSettings;
        private readonly ILogger _logger;

        public FIService(IOptions<FISettings> settingsProvider,
            IOptions<SystemSettings> systemSettings,
            IHttpClientFactory factory,
            ILogger<FbnService> logger)
    
        {
            _fiSettings = settingsProvider.Value;
            _systemSettings = systemSettings.Value;
            _client = factory.CreateClient("HttpMessageHandler");
            BuildFIClient();
            _logger = logger;
           // _transactionAuditRepo = transactionAuditRepo;
        }

        private void BuildFIClient()
        {
            _client.BaseAddress = new Uri(_fiSettings.BaseAddress);
            _client.DefaultRequestHeaders.Add("AppId", _fiSettings.AppId);
            _client.DefaultRequestHeaders.Add("AppKey", _fiSettings.AppKey);
        }

        private static string GenerateReference()
        {
            return Guid.NewGuid().ToString();
        }
        private async Task<T> PostMessage<T, U>(U request, string path, HttpClient client)
        {
            var message = new StringContent(Util.SerializeAsJson<U>(request), Encoding.UTF8, "application/json");
            _logger.LogInformation($"request : {Util.SerializeAsJson(request)}");
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


        public async Task<AccountDetailsResponse> GetAccountDetailsAsync(string accountNumber)
        {
            var request = new AccountDetailsRequest()
            {
                CountryId = _systemSettings.CountryId,
                RequestId = GenerateReference(),
                AccountNumber = accountNumber
            };

            return await PostMessage<AccountDetailsResponse, AccountDetailsRequest>(request, "account/get-account-details", _client);

        }

        //public async Task<TransferResponse> TransferAsync(TransferRequest request)
        //{
        //    var payload = new TransferRequest()
        //    {
        //        Amount = request.Amount,
        //        SourceAccountNumber = request.SourceAccountNumber,
        //        DestinationAccountNumber = request.DestinationAccountNumber,
        //        ClientReferenceId = request.ClientReferenceId,
        //        Narration = request.Narration,
        //        CountryId = _systemSettings.CountryId
        //    };
        //    var response = await PostMessage<TransferResponse, TransferRequest>(payload, "", _client);
        //    var transactionAudit = new TransactionAudit
        //    {
        //        Amount = request.Amount,
        //        ApprovedDate = DateTime.Now,
        //        CreatedDate = DateTime.Now,
        //        DestinationAccount = request.DestinationAccountNumber,
        //        SourceAccount = request.SourceAccountNumber,
        //        TransactionReference = request.ClientReferenceId,
        //        IsSuccessful = true
        //    };
        //    if (response.ResponseCode != null)
        //    {
        //        transactionAudit.IsSuccessful = false;
        //    }
        //    var result = await _transactionAuditRepo.Create(transactionAudit);
        //    if (result == null)
        //    {
        //        throw new Exception("Failed to save transaction to transaction audit log");
        //    }
        //    return response;
        //}
    }
}
