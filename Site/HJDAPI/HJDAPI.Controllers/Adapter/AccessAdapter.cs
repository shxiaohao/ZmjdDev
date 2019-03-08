using HJD.Framework.Interface;
using HJD.Framework.WCF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using System.Xml;
using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using System.Web;
using HJD.AccessService.Contract.Model.Dialog;

namespace HJDAPI.Controllers.Adapter
{
    public class AccessAdapter
    {
        public static ICacheProvider LocalCache5Min = CacheManagerFactory.Create("DynamicCacheFor5Min");
        public static IAccessService AccessService = ServiceProxyFactory.Create<IAccessService>("IAccessService");
         
   


        public int RecordBehaviorQueue(List<Behavior> behaviorQueue)
        {
            try
            {
                AccessService.RecordBehaviorQueue(behaviorQueue);
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }



        public static string  GenShortUrlWithDM12(string longURL)
        {
            return  "http://dm12.me/" + CacheAdapter.LocalCache30Min.GetData<string>("ShortUrl:" + longURL, () =>
           {
               var apiUrl = "http://dm12.me/api/ShortUrl/Gen";
               ShortUrlRequestParam postDataStr = new ShortUrlRequestParam { url = longURL };
               CookieContainer cc = new CookieContainer();
               string json = HttpRequestHelper.PostJson(apiUrl, postDataStr, ref cc);

               ShortUrlResponse result = JsonConvert.DeserializeObject<ShortUrlResponse>(json);

               return   result.shortUrl;
           }); 
        }




        public static string GenShortUrl(int t, string url)
        {
            var shortUrl = url;
            try
            {
                switch (t)
                {
                    #region 生成短链接

                    ////微信分享已无效
                    //case -1:
                    //    {
                    //        string apiUrl = "http://www.v3l.cn/api/shoturl.php";
                    //        string postDataStr = string.Format("url={0}", url);
                    //        CookieContainer cc = new CookieContainer();
                    //        string json = HttpRequestHelper.Get(apiUrl, postDataStr, ref cc);
                    //        var surl = json;// JsonConvert.DeserializeObject<string>(json);
                    //        if (!string.IsNullOrEmpty(surl))
                    //        {
                    //            shortUrl = surl;
                    //        }
                    //        break;
                    //    }
                    ////微信分享已无效
                    //case -2:
                    //    {
                    //        string apiUrl = "http://985.so/api.php";
                    //        string postDataStr = string.Format("url={0}", url);
                    //        CookieContainer cc = new CookieContainer();
                    //        string json = HttpRequestHelper.Get(apiUrl, postDataStr, ref cc);
                    //        var surl = json;// JsonConvert.DeserializeObject<string>(json);
                    //        if (!string.IsNullOrEmpty(surl))
                    //        {
                    //            shortUrl = surl;
                    //        }
                    //        break;
                    //    }
                    ////微信分享已无效
                    //case -3:
                    //    {
                    //        string apiUrl = "http://50r.cn/short_url.json";
                    //        string postDataStr = string.Format("url={0}", url);
                    //        CookieContainer cc = new CookieContainer();
                    //        string json = HttpRequestHelper.Get(apiUrl, postDataStr, ref cc);
                    //        var shortUrlEntity50r = JsonConvert.DeserializeObject<ShortUrlEntity50r>(json);
                    //        if (!string.IsNullOrEmpty(shortUrlEntity50r.Url))
                    //        {
                    //            shortUrl = shortUrlEntity50r.Url;
                    //        }
                    //        break;
                    //    }

                    case -1:
                    case -2:
                    case -3:
                        {
                            shortUrl = GenShortUrlWithDM12(url);
                            break;
                        }
                    #endregion

                    #region 域名更换

                    /*
                        * zmjiudian.com
                        * shangjiudian.com
                        * zmjd100.com
                        * shang-ke.cn
                        * zmjd123.com
                        * zmjd001.com
                        * zmjd100.cn
                        * shangclub.cn
                        */

                    case 0:
                    case 1:
                        {
                            shortUrl = HttpUtility.UrlDecode(shortUrl);
                            shortUrl = shortUrl.Replace("WWW.ZMJIUDIAN.COM", "www.zmjiudian.com").Replace("www.zmjiudian.com", "www.shangjiudian.com");
                            break;
                        }
                    case 2:
                        {
                            shortUrl = HttpUtility.UrlDecode(shortUrl);
                            shortUrl = shortUrl.Replace("WWW.ZMJIUDIAN.COM", "www.zmjiudian.com").Replace("www.zmjiudian.com", "www.zmjd100.com").Replace("WWW.SHANGJIUDIAN.COM", "www.shangjiudian.com").Replace("www.shangjiudian.com", "www.zmjd100.com");
                            break;
                        }
                    case 3:
                        {
                            shortUrl = HttpUtility.UrlDecode(shortUrl);
                            shortUrl = shortUrl.Replace("WWW.ZMJIUDIAN.COM", "www.zmjiudian.com").Replace("www.zmjiudian.com", "www.shang-ke.cn").Replace("WWW.SHANGJIUDIAN.COM", "www.shangjiudian.com").Replace("www.shangjiudian.com", "www.shang-ke.com");
                            break;
                        }
                    case 4:
                        {
                            shortUrl = HttpUtility.UrlDecode(shortUrl);

                            //临时处理：4也返回shangjiudian域名，因为尚旅游成都订阅号免费住活动用 2017.05.16 haoy
                            shortUrl = shortUrl.Replace("WWW.ZMJIUDIAN.COM", "www.zmjiudian.com").Replace("www.zmjiudian.com", "www.shangjiudian.com").Replace("www.zmjd123.com", "www.shangjiudian.com");

                            //shortUrl = shortUrl.Replace("WWW.ZMJIUDIAN.COM", "www.zmjiudian.com").Replace("www.zmjiudian.com", "www.zmjd123.com").Replace("WWW.SHANGJIUDIAN.COM", "www.shangjiudian.com").Replace("www.shangjiudian.com", "www.zmjd123.com");
                            break;
                        }
                    case 5:
                        {
                            shortUrl = HttpUtility.UrlDecode(shortUrl);
                            shortUrl = shortUrl.Replace("WWW.ZMJIUDIAN.COM", "www.zmjiudian.com").Replace("www.zmjiudian.com", "www.zmjd001.com").Replace("WWW.SHANGJIUDIAN.COM", "www.shangjiudian.com").Replace("www.shangjiudian.com", "www.zmjd001.com");
                            break;
                        }
                    case 6:
                        {
                            shortUrl = HttpUtility.UrlDecode(shortUrl);
                            shortUrl = shortUrl.Replace("WWW.ZMJIUDIAN.COM", "www.zmjiudian.com").Replace("www.zmjiudian.com", "www.shangclub.cn").Replace("WWW.SHANGJIUDIAN.COM", "www.shangjiudian.com").Replace("www.shangjiudian.com", "www.shangclub.com");
                            break;
                        }

                    #endregion

                    default:
                        {
                            break;
                        }
                }
            }
            catch(Exception ex){
                
            }

            return shortUrl;
        }
    }
}
