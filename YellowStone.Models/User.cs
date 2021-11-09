using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowStone.Models.Enums;

namespace YellowStone.Models
{
    public class User : IdentityUser

    {
        //[DisplayName("ID")]
        //public int UserID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar(20)")]
        [DisplayName("Staff Id")]
        [StringLength(8)]
        public string StaffId { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Pending_Approval;
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public long RoleId { get; set; }
        public bool IsAdmin { get; set; } = false;
        [DisplayName("Last Login Date")]
        public DateTime? LastLoginDate { get; set; }

        [DisplayName("Last Login Date")]
        public DateTime? PreviousLoginDate { get; set; }
        [JsonIgnore]
        public virtual Role Role { get; set; }

        public string StaffAdDepartment { get; set; }
        public string StaffBranchCode { get; set; }
    }
}
