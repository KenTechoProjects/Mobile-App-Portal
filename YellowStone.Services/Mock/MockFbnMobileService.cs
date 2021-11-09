using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Models.Enums;
using YellowStone.Services.FBNMobile.DTOs;
using YellowStone.Services.FBNService.DTOs;
using YellowStone.Services.Onboarding.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Mock
{
    public class MockFbnMobileService : IFbnMobileService
    {
        private readonly IEnumerable<Limit> _limits;
        public MockFbnMobileService()
        {
            _limits = new List<Limit>() { new Limit() { ID = 1, DailyLimit = 10000m, SingleLimit = 5000m, TransactionType = TransactionType.NationalTransfer },
                                         new Limit() { ID = 1, DailyLimit = 1000000m, SingleLimit = 50000m, TransactionType = TransactionType.SelfTransfer }};


        }

        public Task<OperationResponse> ActivateDevice(string deviceId)
        {
            return Task.FromResult(new OperationResponse() { IsSuccessful = true });

        }

        public Task<OperationResponse> ReleaseDevice(string deviceId)
        {
            return Task.FromResult(new OperationResponse() { IsSuccessful = true });

        }

        public Task<OperationResponse> DeactivateDevice(string deviceId)
        {
            return Task.FromResult(new OperationResponse() { IsSuccessful = true });
        }

            public Task<DeviceResponse> GetCustomerDevices(string accountNumber)
        {
            var response = new DeviceResponse(true)
            {
                Devices = new List<Device>() { new Device() { Customer_Id = 31,
                    DateCreated = DateTime.Now,
                    DeviceId = "ndow9e82ouwqrwa",
                    Id = 2,
                    IsActive = false,
                    Model = "Samsung SGF0113" }
                }
            };

            return Task.FromResult(response);
        }

        public async Task<Customer> GetCustomerInformation(string accountNumber)
        {
            var response = new Customer(true)
            {
                EmailAddress = "email@email.com",
                AccountNumber = accountNumber,
                CustomerId = "123456",
                FirstName = "OLANREWAJU",
                LastName = "OKANRENDE",
                MiddleName = "OPEYEMI",
                PhoneNumber = "08029039468",
                IsActive = true,
                EnrollmentDate = DateTime.Now, HasAccount = true,
                Transactions = new List<Transactions>()
                {
                     new Transactions(){ DateCreated = DateTime.Now, TransactionStatus =  TransactionStatus.Successful, TransactionType = TransactionType.NationalTransfer, Amount = 2000m, DestinationAccountNumber = "0100120300000126", SourceAccountNumber="0100121600000003", Currency="XOF" },
                     new Transactions(){ DateCreated = DateTime.Now, TransactionStatus =  TransactionStatus.Failed, TransactionType =  TransactionType.SelfTransfer, Amount = 100m, DestinationAccountNumber="0100120300000126", SourceAccountNumber = "0100120800000011", Currency="XOF" },
                     new Transactions(){ DateCreated = DateTime.Now, TransactionStatus =  TransactionStatus.Failed, TransactionType =  TransactionType.NationalTransfer, Amount = 1090m, DestinationAccountNumber= "0100120300000126", SourceAccountNumber ="0100120800000011", Currency="XOF" }
                },

                UserActivities = new List<UserActivity>()
                {
                      new UserActivity(){ Activity = "Reset Pasword", ActivityDate = DateTime.Now },
                      new UserActivity(){ Activity = "Changed PIN", ActivityDate = DateTime.Now },
                      new UserActivity(){ Activity = "Login", ActivityDate = DateTime.Now }
                }
            };

            return await Task.FromResult(response);
        }

        public Task<IEnumerable<Limit>> GetLimits()
        {

            return Task.FromResult(_limits);
        }

        public Task<OperationResponse> LockProfile(string accountNumber)
        {
            return Task.FromResult(new OperationResponse() { IsSuccessful = true });
        }

        public Task<OperationResponse> UnlockProfile(string accountNumber)
        {
            return Task.FromResult(new OperationResponse() { IsSuccessful = true });
        }

        public Task<OperationResponse> ResetProfile(string accountNumber)
        {
            return Task.FromResult(new OperationResponse() { IsSuccessful = true });

        }

        public Task<OperationResponse> UpdateLimits(Limit limits)
        {
            return Task.FromResult(new OperationResponse() { IsSuccessful = true });
        }
    }
}
