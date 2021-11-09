using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Models.DTO
{
  public  class TokenResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public TokenResponse(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
