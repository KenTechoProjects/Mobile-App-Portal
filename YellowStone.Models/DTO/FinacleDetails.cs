using System;
using System.Collections.Generic;
using System.Text;

namespace YellowStone.Models.DTO
{
  public  class FinacleDetails
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string FinacleUserId { get; set; }
        public string RoleId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public FinacleDetails(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
