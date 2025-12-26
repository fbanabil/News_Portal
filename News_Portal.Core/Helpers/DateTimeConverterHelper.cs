using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace News_Portal.Core.Helpers
{
    public static class DateTimeConverterHelper
    {
        private static readonly char[] _banglaDigits = new char[] { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };

        private static readonly string[] _banglaMonths = new string[]
        {
            "জানুয়ারি", "ফেব্রুয়ারি", "মার্চ", "এপ্রিল", "মে", "জুন",
            "জুলাই", "আগস্ট", "সেপ্টেম্বর", "অক্টোবার", "নভেম্বর", "ডিসেম্বর"
        };


        public static string ConvertFrmonEnglishToBangla(this DateTime dateTime)
            => ConvertFromEnglishToBangla(dateTime);

        public static string ConvertFromEnglishToBangla(this DateTime dateTime)
        {
            var day = ToBanglaDigits(dateTime.Day.ToString("00"));

            var month = _banglaMonths[dateTime.Month - 1];

            var year = ToBanglaDigits(dateTime.Year.ToString());

            var hour12 = dateTime.ToString("hh"); 
            var minute = dateTime.ToString("mm");
            var banglaHour = ToBanglaDigits(hour12);
            var banglaMinute = ToBanglaDigits(minute);

            var meridiem = dateTime.ToString("tt"); 
            var banglaMeridiem = meridiem switch
            {
                "AM" => "পূর্বাহ্ন",
                "PM" => "অপরাহ্ন",
                _ => ToBanglaDigits(meridiem) 
            };

            return $"{day} {month} {year}, {banglaHour}:{banglaMinute} {banglaMeridiem}";
        }


        public static string GetBengaliDate(this DateTime dateTime)
        {
            var day = ToBanglaDigits(dateTime.Day.ToString("00"));
            var month = _banglaMonths[dateTime.Month - 1];
            var year = ToBanglaDigits(dateTime.Year.ToString());
            return $"{day} {month}, {year}";
        }

        public static string GetBengaliDay(this DateTime dateTime)
        {
            return ToBanglaDigits(dateTime.Day.ToString("00"));
        }

        public static string GetBengaliMonth(this DateTime dateTime)
        {
            return _banglaMonths[dateTime.Month - 1];
        }

        public static string GetBengaliYear(this DateTime dateTime)
        {
            return ToBanglaDigits(dateTime.Year.ToString());
        }

        public static string GetBanglaHour(this DateTime dateTime)
        {
            var hour12 = dateTime.ToString("hh");
            return hour12.ToBanglaDigits();
        }

        public static string GetBanglaMinute(this DateTime dateTime)
        {
            var minute = dateTime.ToString("mm");
            return minute.ToBanglaDigits();
        }

        private static string ToBanglaDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (ch >= '0' && ch <= '9')
                {
                    sb.Append(_banglaDigits[ch - '0']);
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
