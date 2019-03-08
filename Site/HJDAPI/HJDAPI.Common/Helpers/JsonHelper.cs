using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Common.Helpers
{
    public class JsonHelper
    {
        public static string ObjToString(object obj)
        {
          return   JsonConvert.SerializeObject(obj);
        }
    }
}
