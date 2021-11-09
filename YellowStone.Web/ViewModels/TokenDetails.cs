using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YellowStone.Web.ViewModels
{
    public class TokenDetails
    {
        [Required]
        public string StaffId { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
