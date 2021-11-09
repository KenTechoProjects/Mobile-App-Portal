using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YellowStone.Models.Enums;

namespace YellowStone.Models
{
    public class OnboardingRequest : Base
    {
        [StringLength(50)]
        [Required]
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountBranchCode { get; set; }

        [Required]
        public RequestStatus Status { get; set; }

        public string RequestBranchCode { get; set; }

    }
}
