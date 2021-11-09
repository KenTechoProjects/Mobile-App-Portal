using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models;
using YellowStone.Models.DTO;
using YellowStone.Repository.Interfaces;
using YellowStone.Services.FBNService.DTOs;
using YellowStone.Services.FIService.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Mock
{
    public class MockFIService : IFIService
    {

        private static List<AccountDetailsResponse> _accounts;
        public MockFIService()
        {
            LoadFakeAccounts();
        }

        private string CleanUpInput(string input)
        {
            input = input.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "");
            return input;
        }
        private  void LoadFakeAccounts()
        {
            _accounts = new List<AccountDetailsResponse>()
            {
                new AccountDetailsResponse()
                 {
                        BranchCode = "01001",
                        Branch = "ROUME",
                        AccountName = "SOCIETE SENEGALAISE DE REASSURANCE SA",
                        AccountNumber = "0100121600000001",
                        CustomerId = "C001",
                        AccountStatus = "D",
                        AccountType = "CURRENT",
                        CurrencyCode = "XOF",
                        Email = "IBRAHIMANDIAYE@SENRE.SN",
                        MobileNo = "12345",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                },

                new AccountDetailsResponse()
                {
                        BranchCode = "01001",
                        Branch = "ROUME",
                        AccountName = "TREM LA SENEGALAISE SA",
                        AccountNumber = "0100121600000002",
                        CustomerId = "C002",
                        AccountStatus = "A",
                        AccountType = "CURRENT",
                        CurrencyCode = "XOF",
                        Email = "FAIYE@SENRE.SN",
                        MobileNo = "8029039468",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                },

                 new AccountDetailsResponse()
                {
                        BranchCode = "01001",
                        Branch = "ROUME",
                        AccountName = "DU VOIZ DE LACASA",
                        AccountNumber = "0100121600000003",
                        CustomerId = "C003",
                        AccountStatus = "A",
                        AccountType = "CURRENT",
                        CurrencyCode = "XOF",
                        Email = "test@test.com",
                        MobileNo = "8123456789",
                        ResponseCode = "00",
                        ResponseMessage = "Successful"
                },

            };
        }

        public async Task<AccountDetailsResponse> GetAccountDetailsAsync(string accountNumber)
        {
            // throw new TimeoutException();
            var result = _accounts.FirstOrDefault(x => x.AccountNumber == accountNumber);
            return result;
        }

        //public async Task<TransferResponse> TransferAsync(TransferRequest request)
        //{
        //    var response = await Task.FromResult(new TransferResponse()
        //    {
        //        ResponseCode = "00",
        //        Date = DateTime.Now,
        //        Reference = Guid.NewGuid().ToString()
        //    });
        //    var transactionAudit = new TransactionAudit
        //    {
        //        Amount = request.Amount,
        //        ApprovedDate = DateTime.Now,
        //        CreatedDate = DateTime.Now,
        //        DestinationAccount = request.DestinationAccountNumber,
        //        SourceAccount = request.SourceAccountNumber,
        //        TransactionReference = request.ClientReferenceId,
        //        IsSuccessful = false
        //    };
        //    TransactionAudit result = null;
        //    try
        //    {
        //        result = await _transactionAuditRepo.Create(transactionAudit);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Failed to save transaction to transaction audit log");
        //    }
        //    return response;
        //}
    }
}
