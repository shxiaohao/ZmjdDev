using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class DeviceInfoModel
    {
        public DeviceInfoModel()
        {
            AppVer = 0; AppType = 0; UserID = 0;
        }
        public int AppType { get; set; }
        public int AppVer { get; set; }
        public string DeviceID { get; set; }
        public string DeviceInfo { get; set; }
        public string OS { get; set; }
        public long UserID { get; set; }
    }
}
