using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
    public class AppDownloadViewModel
    {
        public string appDownloadURL { get; set; }

        public bool isInWeixin { get; set; }
        public bool isIPhone = false;

        public string appleAppURL = "https://itunes.apple.com/cn/app/zhou-mo-jiu-dian/id763264901";
        public string androidAppURL = "http://whfront.b0.upaiyun.com/android/WHotel_zmjd.apk";
    }
}