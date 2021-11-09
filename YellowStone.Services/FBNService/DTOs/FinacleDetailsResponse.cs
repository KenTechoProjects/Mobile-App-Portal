using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FBNService.DTOs
{
    public class FinacleDetailsResponse : BaseResponse
    {
        public string FinacleUserId { get; set; }
        public string RoleId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string StaffId { get; set; }
    }
}
