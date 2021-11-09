using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Services.FBNService.DTOs;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Mock
{
    public class MockFbnService : IFbnService
    {
        private static readonly List<FinacleDetailsResponse> _finacleDetailsResponses = new List<FinacleDetailsResponse>();
        public MockFbnService()
        {
            InitializeData();
        }

        public async Task<FinacleDetailsResponse> GetFinacleDetails(string userName)
        {
            var response = _finacleDetailsResponses.FirstOrDefault(x => x.StaffId == userName);

            if (response == null)
            {
                response = new FinacleDetailsResponse()
                {
                    ResponseCode = "00",
                    BranchCode = "01001",
                    BranchName = "ROUME",
                    FinacleUserId = "RM1213",
                    RoleId = "25222",
                    StaffId = userName
                };

            }
            return await Task.FromResult(response);

        }

        public async Task<UserDetailsResponse> GetStaffDetails(string userName)
        {
            // var response = new UserDetailsResponse();
            //if (userName.Equals("SN029034", StringComparison.InvariantCultureIgnoreCase))
            //{
            var response = new UserDetailsResponse()
            {
                ResponseCode = "00",
                Department = "E-BUSINESS GROUP",
                Username = userName,
                DisplayName = "Olanrewaju Okanrende",
                Email = "olanrewaju.o.okanrende@firstbanknigeria.com",
                Groups = "Platinum|FDLTeam|"
            };
            //    };
            //}
            //else
            //{
            //    response = new UserDetailsResponse()
            //    {
            //        ResponseCode = "01",
            //        ResponseMessage = "Not Found"
            //    };
            //}
            return await Task.FromResult(response);
        }

        public Task<TellerTillResponse> GetTellerTillAccountAsync(TellerTillRequest request)
        {
            return Task.Run(() =>
            {
                if (request == null)
                {
                    throw new ArgumentNullException("teller till request can't be null");
                }
                if (!string.IsNullOrEmpty(request.Username))
                {
                    return new TellerTillResponse { RequestId = request.RequestId, TillAccount = "1234567890" };
                }
                return new TellerTillResponse { ResponseCode = "01", ResponseMessage = "Failed to get teller account" };
            });
        }

        public Task<UserDetailsResponse> ValidateUser(string userName, string password)
        {
            var response = new UserDetailsResponse();

            if (password == "password")
            {
                response.ResponseCode = "00";
                response.Department = "E-BUSINESS GROUP";
                response.Username = userName;
            }
            else
            {
                response.ResponseCode = "99";
                response.Username = userName;
            }
            return Task.FromResult(response);
        }

        private List<FinacleDetailsResponse> InitializeData()
        {
            return new List<FinacleDetailsResponse>()
        {
             new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01001",
                 BranchName = "ROUME",
                 FinacleUserId = "RM1213",
                 RoleId = "01",
                 StaffId = "SN010000"
             },

              new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01001",
                 BranchName = "ROUME",
                 FinacleUserId = "RM1214",
                 RoleId = "02",
                 StaffId = "SN010001"
             },

               new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01001",
                 BranchName = "ROUME",
                 FinacleUserId = "RM1215",
                 RoleId = "25222",
                 StaffId = ""
             },

                new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01002",
                 BranchName = "MEMORZ",
                 FinacleUserId = "RM1216",
                 RoleId = "03",
                 StaffId = "SN010004"
             },

                 new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01002",
                 BranchName = "MEMORZ",
                 FinacleUserId = "RM1217",
                 RoleId = "04",
                 StaffId = "SN010007"
             },

             new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01003",
                 BranchName = "HEDZA",
                 FinacleUserId = "RM1218",
                 RoleId = "05",
                 StaffId = "SN010006"
             },

              new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01003",
                 BranchName = "HEDZA",
                 FinacleUserId = "RM1219",
                 RoleId = "06",
                 StaffId = "SN010008"
             },

            new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01004",
                 BranchName = "WASSIN",
                 FinacleUserId = "RM1220",
                 RoleId = "07",
                 StaffId = "SN010002"
             },

            new FinacleDetailsResponse{
                 ResponseCode = "00",
                 BranchCode = "01004",
                 BranchName = "WASSIN",
                 FinacleUserId = "RM1221",
                 RoleId = "08",
                 StaffId = "SN010003"
             },

        };
        }

    }
}
