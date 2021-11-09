using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YellowStone.Web.ViewModels
{
    public class DepartmentSummary
    {
        [DisplayName("ID")]
        public long Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Users Count")]
        public int UserCount { get; set; }
    }
}
