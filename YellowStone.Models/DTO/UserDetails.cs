using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Models.DTO
{
  public  class UserDetails
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string StaffId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string BranchCode { get; set; }
        public string MobileNo { get; set; }
        public string ManagerName { get; set; }
        public string ManagerDepartment { get; set; }
        public string Groups { get; set; }

        public UserDetails(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
