using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Services.FBNService.DTOs
{
    public class UserDetailsRequest
    {
        public string RequestId { get; set; }
        public string CountryId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
