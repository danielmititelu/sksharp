using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SkSharp.Utils
{
    internal static class MessageUtils
    {
        private static Random _random = new Random();

        public static string NewClientMessageId()
        {
            var a = RandomString(19);
            return a.Length > 20 || (a.Length == 20 && ulong.Parse(a) > 18446744073709551615) ? NewClientMessageId() : a;
        }

        private static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
