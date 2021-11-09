using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Helpers
{
    public static class DisplayName
    {
        public static string ProfileStatusName(bool status) {
            return (status) ? "Locked" : "UnLocked";
        }
    }
}
