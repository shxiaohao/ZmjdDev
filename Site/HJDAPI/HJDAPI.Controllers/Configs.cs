using System.Configuration;
using System.Web;
using HJDAPI.Common.Helpers;
using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo;

namespace HJDAPI.Controllers
{
    public static class Configs
    {

        public static readonly string icon4City = "116OPMU1";
        public static readonly string icon4Hotel = "116OPMU0";

        public static readonly string searchIcon4Foods = "http://whfront.b0.upaiyun.com/www/img/search/img_foods.png";
        public static readonly string searchIcon4Hotel = "http://whfront.b0.upaiyun.com/www/img/search/img_hotel.png";
        public static readonly string searchIcon4Lab = "http://whfront.b0.upaiyun.com/www/img/search/img_lab.png";
        public static readonly string searchIcon4Old = "http://whfront.b0.upaiyun.com/www/img/search/img_old.png";
        public static readonly string searchIcon4Play = "http://whfront.b0.upaiyun.com/www/img/search/img_play.png";

        public static string   strNeedAddationalCheck = ""; 
        public static bool    boolIsNeedAddationalCheck = true; 


        private static List<long> mCanWriteCommentUserIDs = new List<long>();
        public static  List<long> CanWriteCommentUserIDs
        {
            get
            {
                if (mCanWriteCommentUserIDs.Count == 0)
                {
                    foreach (string s in System.Configuration.ConfigurationManager.AppSettings["CanWriteCommentUserIDs"].Split(','))
                    {
                        mCanWriteCommentUserIDs.Add(long.Parse(s));
                    }
                }
                return mCanWriteCommentUserIDs;
            }
        }

        public static bool IsNeedAddationalCheck
        {
            get
            {
                if (strNeedAddationalCheck.Length == 0)
                {
                    if (ConfigurationManager.AppSettings["NeedAddationalCheck"] == null)
                    {
                        strNeedAddationalCheck = "true";
                    }
                    else
                    {
                        strNeedAddationalCheck = ConfigurationManager.AppSettings["NeedAddationalCheck"];
                    }

                    boolIsNeedAddationalCheck = strNeedAddationalCheck == "true";

                }
                return boolIsNeedAddationalCheck;
            }
        }

        /// <summary>
        /// IOS强制更新版本号
        /// </summary>
        public static string IOSMandatoryVersion
        {
            get { return Apollo.Get("IOSMandatoryVersion"); }
        }

        /// <summary>
        /// IOS当前最新版本号
        /// </summary>
        public static string IOSProposedVersion
        {
            get { return Apollo.Get("IOSProposedVersion"); }
        }

        /// <summary>
        /// Android强制更新版本号
        /// </summary>
        public static string AndroidMandatoryVersion
        {
            get { return Apollo.Get("AndroidMandatoryVersion"); }
        }

        /// <summary>
        /// Android当前最新版本号
        /// </summary>
        public static string AndroidProposedVersion
        {
            get { return Apollo.Get("AndroidProposedVersion"); }
        }

        /// <summary>
        /// Android更新下载链接
        /// </summary>
        public static string APPUpdateandroidURL
        {
            get { return Apollo.Get("APPUpdateandroidURL"); }
        }

        /// <summary>
        /// 当前版本跳出提示，再次跳出同一提示相隔的天数
        /// </summary>
        public static int APPUpdateDays
        {
            get { return int.Parse(Apollo.Get("APPUpdateDays")); }
        }

        /// <summary>
        /// 更新提示标题
        /// </summary>
        public static string APPUpdateTitle
        {
            get { return Apollo.Get("APPUpdateTitle"); }
        }

        /// <summary>
        /// 更新提示详情
        /// </summary>
        public static string APPUpdateTips
        {
            get { return Apollo.Get("APPUpdateTips"); }
        }


        public static string SMSSuffix
        {
            get { return  ConfigurationManager.AppSettings["smsSuffix"]; }
        }

        public static int ActiveID
        {
            get { return int.Parse( ConfigurationManager.AppSettings["ActiveID"]); }
        }

        public static string ProductSaleUrl
        {
            get { return ConfigurationManager.AppSettings["ProductSaleUrl"]; }
        }

        public static string GroupProductSaleUrl
        {
            get { return ConfigurationManager.AppSettings["GroupProductSaleUrl"]; }
        }

        public static string AppConfig
        {
            get { return ConfigurationManager.AppSettings["AppConfig"]; }
        }

        public static string WWWURL
        {
            get { return ConfigurationManager.AppSettings["WWWURL"]; }
        }

        public static string PackageURL
        {
            get { return ConfigurationManager.AppSettings["PackageUrl"]; }
        }
        public static string WhotelAppUrl
        {
            get { return ConfigurationManager.AppSettings["WhotelAppUrl"]; }
        }
        
        public static string ZMJDPayUrl
        {
            get { return ConfigurationManager.AppSettings["ZMJDPayUrl"]; }
        }

        public static string WalletMenuUrl
        {
            get { return ConfigurationManager.AppSettings["WalletMenuUrl"]; }
        }
        
        public static string APIURL
        {
            get { return ConfigurationManager.AppSettings["APIURL"]; }
        }  
        
        public static string BGURL
        {
            get { return ConfigurationManager.AppSettings["BGURL"]; }
        }

        public static string BoTaoAPI
        {
            get { return ConfigurationManager.AppSettings["BoTaoAPI"]; }
        }

        public static string BoTaoAPIKey
        {
            get { return ConfigurationManager.AppSettings["BoTaoAPIKey"]; }
        }

        public static string BoTaoSignKey
        {
            get { return ConfigurationManager.AppSettings["BoTaoSignKey"]; }
        }

        public static int winnerNum
        {
            get { return int.Parse(ConfigurationManager.AppSettings["winnerNum"]); }
        }
        
        public static string activeKeyWord
        {
            get { return ConfigurationManager.AppSettings["activeKeyWord"]; }
        }
        
        public static string activeFinishDateTime
        {
            get { return ConfigurationManager.AppSettings["activeFinishDateTime"]; }
        }

        public static string activeUrl
        {
            get { return ConfigurationManager.AppSettings["activeUrl"]; }
        }
        
        public static string HotCitys
        {
            get { return ConfigurationManager.AppSettings["hotCitys"]; }
        }

        public static string HotOverseaCitys
        {
            get { return ConfigurationManager.AppSettings["hotOverseaCitys"]; }
        }

        public static string HMTCitys
        {
            get { return ConfigurationManager.AppSettings["HMTCitys"]; }
        }

        public static string SouthEastAsiaCitys
        {
            get { return ConfigurationManager.AppSettings["SouthEastAsiaCitys"]; }
        }

        public static string BoutiqueCity
        {
            get { return ConfigurationManager.AppSettings["BoutiqueCity"]; }
        }

        public static string HotFacilitys
        {
            get { return ConfigurationManager.AppSettings["hotFacilitys"]; }
        }

        /// <summary>
        /// 周末酒店订阅号
        /// </summary>
        public static string WeiXinAPPID
        {
            get { return ConfigurationManager.AppSettings["WeiXinAPPID"]; }
        }
        
        public static string WeiXinSecret
        {
            get { return ConfigurationManager.AppSettings["WeiXinSecret"]; }
        }
        
        public static string WeiXinWelcome
        {
            get { return ConfigurationManager.AppSettings["WeiXinWelcome"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string WeiXinAPPID2
        {
            get { return ConfigurationManager.AppSettings["WeiXinAPPID2"]; }
        }
        
        public static string WeiXinSecret2
        {
            get { return ConfigurationManager.AppSettings["WeiXinSecret2"]; }
        }

        /// <summary>
        /// 尚旅游成都
        /// </summary>
        public static string WeiXinAPPIDShanglvcd
        {
            get { return ConfigurationManager.AppSettings["WeiXinAPPIDShanglvcd"]; }
        }

        public static string WeiXinSecretShanglvcd
        {
            get { return ConfigurationManager.AppSettings["WeiXinSecretShanglvcd"]; }
        }

        /// <summary>
        /// 美味至尚
        /// </summary>
        public static string WeiXinAPPIDMeiweizhishang
        {
            get { return ConfigurationManager.AppSettings["WeiXinAPPIDMeiweizhishang"]; }
        }

        public static string WeiXinSecretMeiweizhishang
        {
            get { return ConfigurationManager.AppSettings["WeiXinSecretMeiweizhishang"]; }
        }

        public static string WeiXinTemplatePath
        {
            get { return ConfigurationManager.AppSettings["WeiXinTemplatePath"]; }
        }
              
        public static string WeiXinLog
        {
            get { return ConfigurationManager.AppSettings["WeiXinLog"]; }
        }

        public static string LogPath
        {
            get { return ConfigurationManager.AppSettings["LogPath"]; }
        }

        public static string AppHotelClass
        {
            get { return ConfigurationManager.AppSettings["AppHotelClass"]; }
        }
 
        public static string HotelListPhotoUrl
        {
            get { return ConfigurationManager.AppSettings["HotelListPhotoUrl"]; }
        }
        public static string Key
        {
            get { return ConfigurationManager.AppSettings["Key"]; }
        }
        public static string WHApp
        {
            get { return ConfigurationManager.AppSettings["WHApp"]; }
        }

        public static string WHMo
        {
            get { return ConfigurationManager.AppSettings["WHMo"]; }
        }
        public static string LoginUrl
        {
            get { return ConfigurationManager.AppSettings["LoginUrl"]; }
        }
        public static string RegisterUrl
        {
            get { return ConfigurationManager.AppSettings["RegisterUrl"]; }
        }
        public static string PostHotelReviewUrl
        {
            get { return ConfigurationManager.AppSettings["PostHotelReviewUrl"]; }
        }

        public static string VerifyKey
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return WHApp;
                }

                return HttpContext.Current.Request.Headers["WH-Verify-Key"];
            }
        }

        public static string VerifyEncrypte
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return SecurityHelper.Md5(VerifyKey + WHMo);
                }

                return HttpContext.Current.Request.Headers["WH-Verify-Encrypte"];
            }
        }

        public static string SendNotice_Production_Mode
        {
            get { return ConfigurationManager.AppSettings["SendNotice_Production_Mode"]; }
        }

        public static string MD5Key
        {
            get { return ConfigurationManager.AppSettings["MD5Key"]; }
        }
        public static string UploadFileHttpPath
        {
            get { return ConfigurationManager.AppSettings["UploadFileHttpPath"]; }
        }
        
    }
}