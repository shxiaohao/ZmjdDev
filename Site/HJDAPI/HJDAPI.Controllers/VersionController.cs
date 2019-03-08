using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJDAPI.Controllers.Adapter;

namespace HJDAPI.Controllers
{
    public class VersionController: BaseApiController
    {
        [ActionName("GetLatestAppVersion")]
        public Dictionary<string, string> GetLatestAppVersion()
        {
            return new VersionAdapter().GetLatestAppVersion();
        }

        [ActionName("GetLatestZMJDDataVersion")]
        public Dictionary<string, string> GetLatestZMJDDataVersion()
        {
            return new VersionAdapter().GetLatestZMJDDataVersion();
        }

        [ActionName("GetZMJDAndroidVersionInfo")]
        public Dictionary<string, string> GetZMJDAndroidVersionInfo()
        {
            return new VersionAdapter().GetZMJDAndroidVersionInfo();
        }

        [ActionName("GetAttractionHotelAndroidVersionInfo")]
        public Dictionary<string, string> GetAttractionHotelAndroidVersionInfo()
        {
            return new VersionAdapter().GetAttractionHotelAndroidVersionInfo();
        }

        /// <summary>
        /// 情侣酒店数据版本
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetLatestZMJDLoveDataVersion()
        {
            return new VersionAdapter().GetCommDict(606);
        }

        /// <summary>
        /// 情侣酒店Android版本信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetZMJDLoveAndroidVersionInfo()
        {
            return new VersionAdapter().GetCommDict(605);
        }
    }
}
