using Newtonsoft.Json;
namespace HJDAPI.Common.Extensions
{
    public static class StringExtension
    {
         public static string ToJson(this object obj)
         {
             return JsonConvert.SerializeObject(obj);
         }
    }
}