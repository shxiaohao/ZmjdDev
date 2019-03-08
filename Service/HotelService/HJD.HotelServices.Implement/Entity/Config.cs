using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Entity
{
    public class Config
    {

        public static string RabbitmqHostName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqHostName"];
            }
        }

        public static string RabbitmqUserName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqUserName"];
            }
        }

        public static string RabbitmqPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqPassword"];
            }
        }
    }
}
