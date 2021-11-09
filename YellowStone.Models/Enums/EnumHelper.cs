using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YellowStone.Models.Enums
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }
       

        public static string GetProfileStatusName(bool isLockedOut)
        {
            if (isLockedOut)
            {
                return "<span style='color: green'>Unlocked </span>to <span style='color: red'>Locked</span>";
            }
            return "<span style='color: red'>Locked</span> to <span style='color: green'>Unlocked</span>";
        }
    }
}
