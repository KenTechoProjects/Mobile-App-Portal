using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Models;

namespace Apollo.ViewModels
{
    public class AdminApprovalViewModel : PageLayout
    {
        public List<Apollo.Models.Permission> Resources { get; set; }
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
        public List<UserView> UserViews { get; set; }
        public List<Department> Departments { get; set; }
    }

    public class UserView
    {
        [DisplayName("ID")]
        public int UserID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar(20)")]
        [DisplayName("SN")]
        [StringLength(8)]
        public string SNNumber { get; set; }
        public string Department { get; set; }
        public int DepartmentID { get; set; }
        public bool? Enabled { get; set; } = false;
        [DisplayName("Last Login Date")]
        public DateTime? LastLoginDate { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Notes { get; set; }
    }
}
