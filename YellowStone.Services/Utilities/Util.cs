using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace YellowStone.Service.Utilities
{
    public class Util
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };
        public static bool IsEmail(string input)
        {
            var regEx = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

            return regEx.IsMatch(input);
        }

        public static bool IsValidString(string input)
        {
            var regEx = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$");

            return regEx.IsMatch(input);
        }

        public static string MaskPhoneNumber(string phonenumber)
        {
            if(phonenumber.Length > 3)
            {
                var lastDigits = phonenumber.Substring(phonenumber.Length - 4, 4);
                var maskedPhonenumber = string.Concat(new String('*', phonenumber.Length - lastDigits.Length), lastDigits);
                phonenumber = maskedPhonenumber;
            }

            return phonenumber;
        }

        public static string SerializeAsJson<T>(T item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public static T DeserializeFromJson<T>(string input)
        {
            
            return JsonConvert.DeserializeObject<T>(input, settings);
        }
    }
}
