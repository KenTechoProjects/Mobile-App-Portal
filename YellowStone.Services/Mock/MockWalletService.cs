using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Services.Processors;
using YellowStone.Services.WalletService.DTOs;

namespace YellowStone.Services.Mock
{
    public class MockWalletService : IWalletService
    {
        private static List<WalletEnquiryResponse> _walletEnquiryResponse;

        public MockWalletService()
        {
            LoadFakeWallets();
        }

        private void LoadFakeWallets()
        {
            _walletEnquiryResponse = new List<WalletEnquiryResponse>()
            {
            new WalletEnquiryResponse
                {
                    Name = "Senegalaise de Reassurance",
                    Balance = new Random().Next(20,100000),
                    WalletAccountNumber = "1234567890",
                    PhoneNumber = "+221-33-822-8089",
                    Email = "ibrahimdiaye@senre.sn",
                    WalletType = "Basic"
                },
            new WalletEnquiryResponse
                {
                    Name = "TREM LA SENEGALAISE SA",
                    Balance = new Random().Next(20,100000) ,
                    WalletAccountNumber = "1234567891",
                    PhoneNumber = "+(221)33870-8081",
                    Email = "faiye@senre.sn",
                    WalletType = "Premium"
                },
            new WalletEnquiryResponse
                {
                    Name = "Gabriel Okolie",
                    Balance = new Random().Next(20,100000),
                    WalletAccountNumber = "1234567892",
                    PhoneNumber = "+221(33)870-8082",
                    Email = "okanrende_lanre@yahoo.com",
                    WalletType = "Premium"
                },

        };

        }

        private string CleanUpInput(string input)
        {
            input = input.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "");
            return input;
        }

        public  Task<WalletEnquiryResponse> GetWalletAsync(string walletId)
        {


            var result = _walletEnquiryResponse.FirstOrDefault(x => CleanUpInput(x.PhoneNumber) == walletId);

            return Task.FromResult(result);

        }

        //Task<FundWalletResponse> IWalletService.FundWalletAsync(FundWalletRequest request)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
