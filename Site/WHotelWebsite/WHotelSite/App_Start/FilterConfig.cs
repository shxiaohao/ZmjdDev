using System.Web;
using System.Web.Mvc;

namespace WHotelSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new WHotelSite.Filters.GlobalExceptionAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}