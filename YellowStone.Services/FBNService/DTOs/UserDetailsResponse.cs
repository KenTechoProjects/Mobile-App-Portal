using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FBNService.DTOs
{
    public class UserDetailsResponse : BaseResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string MobileNo { get; set; }
        public string ManagerName { get; set; }
        public string ManagerDepartment { get; set; }
        public string Groups { get; set; }
    }
}
