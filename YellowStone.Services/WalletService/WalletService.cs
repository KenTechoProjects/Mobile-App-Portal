using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YellowStone.Models;
using YellowStone.Repository.Interfaces;
using YellowStone.Service.Utilities;
using YellowStone.Services.Processors;
using YellowStone.Services.WalletService.DTOs;

namespace YellowStone.Services.WalletService
{
    public class WalletService : IWalletService
    {
        private readonly WalletServiceSettings _walletServiceSettings;
        private readonly SystemSettings _systemSettings;
        private readonly HttpClient _client;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IOptions<WalletServiceSettings> settingsProvider,
            IOptions<SystemSettings> systemSettings,
            IHttpClientFactory factory,
            ILogger<WalletService> logger)
        {
            _walletServiceSettings = settingsProvider.Value;
            _systemSettings = systemSettings.Value;
            _client = factory.CreateClient("HttpMessageHandler");
            BuildWalletClient();
            _logger = logger;
        }

        private void BuildWalletClient()
        {
            _client.BaseAddress = new Uri(_walletServiceSettings.BaseUrl);
            _client.DefaultRequestHeaders.Add("appId", _walletServiceSettings.AppId);
            _client.DefaultRequestHeaders.Add("secret", _walletServiceSettings.Secret);
        }

        public async Task<WalletEnquiryResponse> GetWalletAsync(string walletId)
        {
            string path = $"wallet/enquire?walletId={walletId}";
            var response = await GetMessageAsync<WalletEnquiryResponse>(path, _client);
            return response;
        }

        //public async Task<FundWalletResponse> FundWalletAsync(FundWalletRequest request)
        //{
        //    var transaction = await _transactionAuditRepo.GetByTransactionReference(request.TransactionReference);
        //    if (transaction == null)
        //    {
        //        throw new Exception("Transaction doesn't exist meaning Firstleg did not occur");
        //    }

           
        //    var response = await PostMessage<FundWalletResponse, FundWalletRequest>(request, "", _client);

        //    if (response != null)
        //    {
        //        transaction.IsSuccessful = false;
        //    }
        //    transaction.IsSuccessful = true;
        //    await _transactionAuditRepo.Update(transaction);
        //    return response;
        //}

        //private async Task<T> PostMessage<T, U>(U request, string path, HttpClient client)
        //{
        //    var message = new StringContent(Util.SerializeAsJson<U>(request), Encoding.UTF8, "application/json");
        //    var rawResponse = await client.PostAsync(path, message);
        //    if (!rawResponse.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Service invocation failure");
        //    }
        //    var body = await rawResponse.Content.ReadAsStringAsync();
        //    _logger.LogInformation($"body: {body}");
        //    return Util.DeserializeFromJson<T>(body);
        //}

        private async Task<T> GetMessageAsync<T>(string path, HttpClient client)
        {
            var rawResponse = await client.GetAsync(path);
            var body = await rawResponse.Content.ReadAsStringAsync();

            _logger.LogInformation($"body: {body}");
            //if (!rawResponse.IsSuccessStatusCode)
            //{
            //    throw new Exception(rawResponse.ReasonPhrase);
            //}
            return Util.DeserializeFromJson<T>(body);
        }

    }
}
