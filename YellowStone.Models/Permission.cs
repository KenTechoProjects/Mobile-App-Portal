using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YellowStone.Models
{
    public class Permission : Base
    {
        //[DisplayName("ID")]
        //public int ResourceID { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        [DisplayName("Resource Type")]
        [StringLength(50)]
        [Required]
        public string ResourceType { get; set; }

        public string PermissionCategory { get; set; }
        [JsonIgnore]
        public IList<RolePermission> RolePermissions { get; set; }

    }
}
