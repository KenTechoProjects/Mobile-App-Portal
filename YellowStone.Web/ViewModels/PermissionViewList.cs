using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels

{
    public class PermissionViewList
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public string Category { get; set; }

    }
}