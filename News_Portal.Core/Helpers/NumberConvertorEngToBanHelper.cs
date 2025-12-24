using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Helpers
{
    public static class NumberConvertorEngToBanHelper
    {
        private static readonly char[] _banglaDigits = new char[] { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };
        public static string ToBanglaDigits(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (ch >= '0' && ch <= '9')
                {
                    var banglaDigit = _banglaDigits[ch - '0'];
                    sb.Append(banglaDigit);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
    }
}
