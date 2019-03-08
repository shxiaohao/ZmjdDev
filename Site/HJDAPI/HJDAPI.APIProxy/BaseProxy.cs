using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class BaseProxy
    {
        protected static bool IsProductEvn = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["APIProxy_IsProductEvn"]);
        protected static string APISiteUrl = System.Configuration.ConfigurationManager.AppSettings["APISiteUrl"];
        
    }
}
