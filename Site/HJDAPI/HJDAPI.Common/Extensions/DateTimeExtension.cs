using System;

namespace HJDAPI.Common.Extensions
{
    public static class DateTimeExtension
    {
         public static string Format(this DateTime date,string format="yyyy-MM-dd")
         {
             return date.ToString(format);
         }
    }
}