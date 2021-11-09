using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YellowStone.Web.ViewModels
{
    public class LoginDetails
    {
        [Required]
        public string StaffId { get; set; }
        [Required]
        public string Password { get; set; }
       
    }
}
