using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YellowStone.Models.Enums;

namespace YellowStone.Models
{
    public class Role : Base
    {
       
        [StringLength(50)]
        [Required]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Invalid input")]
        public string Name { get; set; }
        [Required]
        public RequestStatus Status { get; set; }

        public long? DepartmentId { get; set; }
        [JsonIgnore]
        public virtual Department Department { get; set; }
        public String HomePage { get; set; } = "home";
        
        [JsonIgnore]
        public IList<RolePermission> RolePermissions { get; set; }
    }
}
