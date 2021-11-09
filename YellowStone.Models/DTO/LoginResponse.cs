using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Models.DTO
{
  public  class LoginResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string StaffId { get; set; }
        public string Department { get; set; }

        public LoginResponse(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
