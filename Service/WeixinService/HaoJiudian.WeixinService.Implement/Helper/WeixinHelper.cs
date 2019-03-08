using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.WeixinServices.Contract;
using System.ServiceModel;
using System.Configuration;
using HJD.Framework.Interface;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HJD.WeixinServices.Contracts;
using System.Text.RegularExpressions;
using HJD.WeixinService.Implement.Helper;
using System.Net;
using Newtonsoft.Json;
using HJD.AccessService.Contract;
using HJD.Framework.WCF;
using RabbitMQ.Client;
using HJD.WeixinService.Contract;

namespace HJD.WeixinServices.Implement.Helper
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class WeixinHelper
    {
        public static IAccessService accessService = ServiceProxyFactory.Create<IAccessService>("IAccessService");

        public static ICacheProvider LocalCache1h = CacheManagerFactory.Create("DynamicCacheFor1h");

        private static IMemcacheProvider LocalCache = MemcacheManagerFactory.Create("DynamicCacheForType2");
        public static string CacheHotCitysConfig { get { return ConfigurationManager.AppSettings["hotCitys"]; } }

        public static Dictionary<string, List<string>> ThemeDictionary = new Dictionary<string, List<string>>();

        private static DateTime lastRuleUpdateTime = DateTime.Now.AddDays(-1);

        public static Dictionary<string, string> weixinDict;
         
        public static Dictionary<string, WeiXinChannelCode> dicWeiXinOrgIWeiXinChannelCode = new Dictionary<string, WeiXinChannelCode>();
       
        /// <summary>
        /// 通过微信原始ID获得微信枚举
        /// </summary>
        /// <param name="weixincode"></param>
        /// <returns></returns>
        public static WeiXinChannelCode GetWeiXinChannelCodeByWeiXinOrgID(string weixincode)
        {
            if (dicWeiXinOrgIWeiXinChannelCode.Count == 0)
            {
                foreach (WeiXinOrgID item in Enum.GetValues(typeof(WeiXinOrgID)))
                {
                    dicWeiXinOrgIWeiXinChannelCode.Add(item.ToString(), (WeiXinChannelCode)item);
                }
            }

            if (dicWeiXinOrgIWeiXinChannelCode.ContainsKey(weixincode))
            {
                return dicWeiXinOrgIWeiXinChannelCode[weixincode];
            }
            else
            {
                return WeiXinChannelCode.周末酒店订阅号;
            }
        }


        public static string getResponseForTextInput2(string input, string ToUserName, string FromUserName, string CreateTime)
        {
            
            if (lastRuleUpdateTime < DateTime.Now.AddMinutes(-10)) //每十分钟更新一次
            {
                lastRuleUpdateTime = DateTime.Now;
                InitWeiXinDict();
            }
        //    LogHelper.WriteLog( string.Format("{0}:{1}:{2}", input, weixinDict.ContainsKey(input),weixinDict.Count));
            if (weixinDict.ContainsKey(input))
            {
                return weixinDict[input].Replace("{ToUserName}", ToUserName).Replace("{FromUserName}", FromUserName).Replace("{CreateTime}", CreateTime);
            }
            else
            {
                return "";
            }
        }

//        新增永久图文素材
//        https://api.weixin.qq.com/cgi-bin/material/add_news?access_token=ACCESS_TOKEN
//调用示例

//{
//  "articles": [{
//       "title": TITLE,
//       "thumb_media_id": THUMB_MEDIA_ID,
//       "author": AUTHOR,
//       "digest": DIGEST,
//       "show_cover_pic": SHOW_COVER_PIC(0 / 1),
//       "content": CONTENT,
//       "content_source_url": CONTENT_SOURCE_URL
//    },
//    //若新增的是多图文素材，则此处应还有几段articles结构
// ]
//}

        /// <summary>
        /// 新增永久图文素材
        /// </summary>
        /// <param name="articles"></param>
        /// <returns></returns>
        public static ResultEntity Add_News(List<NewsEntity> articles)
        {
       
            string token = WeiXinToken.GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token=" + token;

            string data = JsonConvert.SerializeObject(articles);
            string json = HttpHelper.HttpPostForJson(url, "{ \"articles\":" +  data + "}");


            return JsonConvert.DeserializeObject<ResultEntity>(json);
        }

        public static ResultEntity Update_News(string media_id, NewsEntity article)
        {
           
            string token = WeiXinToken.GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/material/update_news?access_token=" + token;

            string data = JsonConvert.SerializeObject(article);
            string json = HttpHelper.HttpPostForJson(url, "{  \"media_id\":\"" + media_id + "\",\"index\":0, \"articles\":" + data + "}");

            return JsonConvert.DeserializeObject<ResultEntity>(json);
        }
        public static ResultEntity Del_Material(string media_id)
        {
           
            string token = WeiXinToken.GetToken();

            string url = "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token=" + token;

           string json = HttpHelper.HttpPostForJson(url, "{  \"media_id\":\"" + media_id + "\"}");

           return JsonConvert.DeserializeObject<ResultEntity>(json);
        }

        public static ResultEntity Add_Material(string fileUrl, string fileName)
        {
        
            string url = "https://api.weixin.qq.com/cgi-bin/material/add_material" + "?access_token=" + WeiXinToken.GetToken();
       
           string json = HttpHelper.HttpPostWithFileForWeiXin(url, fileUrl,  fileName);
           return JsonConvert.DeserializeObject<ResultEntity>(json);
        
        }
        

        public static void InitWeiXinDict()
        {
            List<CommDictEntity> lcd = GetCommDictList(302);
            weixinDict = new Dictionary<string, string>();
            foreach (CommDictEntity d in lcd)
            {
                d.DicValue = Regex.Replace(d.DicValue, "，", ",");
                var dicKeys = Regex.Split(d.DicValue, ",");
                for (int k = 0; k < dicKeys.Length; k++)
                {
                    var key = dicKeys[k];
                    if (weixinDict.ContainsKey(key))
                    {
                        weixinDict[key] = d.Descript;
                    }
                    else
                    {
                        weixinDict.Add(key, d.Descript);
                    }
                }
            }
        }

        /// <summary>
        /// 主题/玩点的回复机制
        /// </summary>
        /// <param name="key"></param>
        /// <param name="toUser"></param>
        /// <param name="fromuserName"></param>
        /// <returns></returns>
        public static string ThemeDataMachine(string key, string toUser, string fromuserName)
        {
            var themeValue = "";

            try
            {
                if (ThemeDictionary.Count <= 0)
                {
                    //主题酒店键值对
                    CommDictEntity themeDicStr = GetCommDict(10008, 3);
                    var descript = themeDicStr.Descript;

                    //解析并获得主题=>主题搜索关键字集合的字典，并同时判断/找到符合当前用户输入关键字对应的主题
                    var list = descript.Split(new char[] { '\r', '\n', ' ' }).Where(d => !string.IsNullOrEmpty(d) && d.Contains(":"));
                    foreach (var item in list)
                    {
                        var theme = item.Split(new char[] { ':' })[0];
                        var keylist = item.Split(new char[] { ':' })[1].Split(new char[] { ',' }).ToList(); //.Where(d => !string.IsNullOrEmpty(d));
                        if (!ThemeDictionary.ContainsKey(theme))
                        {
                            ThemeDictionary[theme] = keylist;
                        }
                    }
                }

                //遍历主题字典，找到是否有当前用户输入关键字对应主题
                if (ThemeDictionary.Count > 0 && ThemeDictionary.Count(d => d.Value.Contains(key.ToLower()) || d.Value.Contains(key.ToUpper())) > 0)
                {
                    var first = ThemeDictionary.First(d => d.Value.Contains(key.ToLower()) || d.Value.Contains(key.ToUpper()));
                    themeValue = first.Key;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(string.Format(@"D:\Log\WeixinService\ErrorLog_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), "【ThemeDataMachine】" + ex.Message + "\r\n" + ex.StackTrace + "\r\n");
            }

            return !string.IsNullOrEmpty(themeValue) ? string.Format("您是想查找{0}酒店？点击以下连接查找{0}酒店http://www.zmjiudian.com", themeValue) : "";
        }

        /// <summary>
        /// 城市地点的回复机制
        /// </summary>
        /// <param name="key"></param>
        /// <param name="toUser"></param>
        /// <param name="fromuserName"></param>
        /// <returns></returns>
        public static string CityDataMachine(string key, string toUser, string fromuserName)
        {
            var get = new CityEntity();

            try
            {
                var city = GetZMJDCityData2();
                get = city.HotArea.Find(area => area.Name.ToLower() == key.ToLower());
                if (get == null)
                {
                    get = city.Citys.Find(c => c.Name.ToLower() == key.ToLower());
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(string.Format(@"D:\Log\WeixinService\ErrorLog_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), "【CityDataMachine】" + ex.Message + "\r\n" + ex.StackTrace + "\r\n");
            }

            return get != null ? string.Format("您是想查找{0}酒店？点击以下连接查找{0}酒店 http://www.zmjiudian.com/city{1}", get.Name, get.ID) : "";
        }

        /// <summary>
        /// 格式化微信返回内容
        /// </summary>
        /// <param name="Content">返回内容</param>
        /// <param name="ToUserName"></param>
        /// <param name="FromUserName"></param>
        /// <param name="CreateTime"></param>
        /// <returns></returns>
        public static string GenWeiXinTextReturn(string Content, string ToUserName, string FromUserName, string CreateTime)
        {
            string textFormat = @"<xml>
 <ToUserName><![CDATA[{ToUserName}]]></ToUserName>
 <FromUserName><![CDATA[{FromUserName}]]></FromUserName>
 <CreateTime>{CreateTime}</CreateTime>
 <MsgType><![CDATA[text]]></MsgType>
 <Content><![CDATA[{Content}]]></Content>
</xml>";

            return textFormat.Replace("{Content}", Content)
               .Replace("{ToUserName}", ToUserName)
                                               .Replace("{FromUserName}", FromUserName)
                                               .Replace("{CreateTime}", CreateTime);
        }

        //多客服
        public static string GenWeiXinTextForTransfer_Customer_Service(string ToUserName, string FromUserName, string CreateTime)
        {
            string textFormat = @"<xml>
<ToUserName><![CDATA[{ToUserName}]]></ToUserName>
<FromUserName><![CDATA[{FromUserName}]]></FromUserName>
<CreateTime>{CreateTime}</CreateTime>
<MsgType><![CDATA[transfer_customer_service]]></MsgType>
</xml>";

            return textFormat
               .Replace("{ToUserName}", ToUserName)
                                               .Replace("{FromUserName}", FromUserName)
                                               .Replace("{CreateTime}", CreateTime);
        }

        /// <summary>
        /// 格式化微信返回内容(链接模块的方式)
        /// </summary>
        /// <param name="Content">返回内容</param>
        /// <param name="ToUserName"></param>
        /// <param name="FromUserName"></param>
        /// <param name="CreateTime"></param>
        /// <returns></returns>
        public static string GenWeiXinTextForLinkArticleReturn(string Content, string ToUserName, string FromUserName, string CreateTime, int ArticleCount)
        {
            string textFormat = @"<xml>
    <ToUserName><![CDATA[{ToUserName}]]></ToUserName>
    <FromUserName><![CDATA[{FromUserName}]]></FromUserName>
    <CreateTime>{CreateTime}</CreateTime>
    <MsgType><![CDATA[news]]></MsgType>
    <Content><![CDATA[]]></Content>
    <ArticleCount>{ArticleCount}</ArticleCount>
    <Articles>
        {Content}
    </Articles>
    <FuncFlag>0</FuncFlag>
</xml>";

            return textFormat
                .Replace("{Content}", Content)
                .Replace("{ArticleCount}", ArticleCount.ToString())
                .Replace("{ToUserName}", ToUserName)
                .Replace("{FromUserName}", FromUserName)
                .Replace("{CreateTime}", CreateTime);
        }

        public static string GenWeiXinTextForLinkItem(string Title, string Description, string PicUrl, string Url)
        {
            string textFormat = @"<item>
            <Title><![CDATA[{Title}]]></Title>
            <Description><![CDATA[{Description}]]></Description>
            <PicUrl><![CDATA[{PicUrl}]]></PicUrl>
            <Url><![CDATA[{Url}]]></Url>
        </item>";

            return textFormat
                .Replace("{Title}", Title)
                .Replace("{Description}", Description)
                .Replace("{PicUrl}", PicUrl)
                .Replace("{Url}", Url);
        }

        //获取字典列表
        public static List<CommDictEntity> GetCommDictList(int type)
        {
            return CommDAL.GetCommDictList(type);
        }

        //获取指定的某一个字典项
        public static CommDictEntity GetCommDict(int typeid, int dictKey)
        {
            return CommDAL.GetCommDict(typeid, dictKey);
        }

        /// <summary>
        /// 获取酒店城市地
        /// </summary>
        /// <returns></returns>
        public static CityList GetZMJDCityData2()
        {
            return LocalCache.GetData<CityList>("GetZMJDCityData2", () =>
            {
                CityList cl = new CityList();

                cl.Citys = GetZMJDCityData();
                cl.HotArea = GenHotArea(cl.Citys);
                return cl;
            });
        }

        //城市列表
        private static List<CityEntity> GetZMJDCityData()
        {
            return LocalCache.GetData<List<CityEntity>>("ZMJDCityData", () => { return HotelDAL.GetZMJDCityData(); });
        }

        //热门省份/直辖市
        private static List<CityEntity> GenHotArea(List<CityEntity> citys)
        {
            List<CityEntity> cl = new List<CityEntity>();
            var hotCitys = CacheHotCitysConfig;   //<add key="hotCitys" value="北京,上海,广州,深圳,武汉,南京,成都" />

            foreach (string city in hotCitys.Split(','))
            {
                if (citys.Where(c => c.Name == city).Count() > 0)
                {
                    cl.Add(citys.Where(c => c.Name == city).FirstOrDefault());
                }
            }

            return cl;
        }

        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        public static bool GetContentOrderId(string content, string keywords ,out long orderID)
        {
            var filterContent = (!string.IsNullOrEmpty(keywords) ? filterContentByKey(content, keywords) : content);

            bool isNum = long.TryParse(filterContent, out orderID);
            if (isNum == false && filterContent.Length >= 5)
            {
                string pattern = @"\D";
                Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);   // MatchEvaluator myEvaluator = new MatchEvaluator(Match.;
                string numstr = reg.Replace(filterContent, "");
                if (numstr.Length >= 5)
                {
                    isNum = long.TryParse(numstr, out orderID);
                }
            }

            return isNum;
        }

        /// <summary>
        /// 检查用户的输入内容是否包含指定关键字/关键字词组
        /// </summary>
        /// <param name="acKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool ContainsActiveKeywords(string acKey, string content)
        {
            var keyList = acKey.Split('|');
            foreach (var key in keyList)
            {
                if (content.ToLower().Trim().StartsWith(key.ToLower().Trim()))
                {
                    return true;
                }
            }

            return false;
        }

        public static string filterContentByKey(string content, string keywords)
        {
            content = content.ToLower().Trim();

            var keyList = keywords.Split('|');
            foreach (var key in keyList)
            {
                var _k = key.ToLower().Trim();
                if (content.StartsWith(_k))
                {
                    content = content.Replace(_k, "");
                }
            }

            return content;
        }

        #region 自定义规则处理

        public static void CacheCustomHelloTip(string fromName)
        {
            //key
            var loveThingKey = string.Format("LoveThingKey_" + fromName);

            //get cache
            var loveThingStep = LocalCache1h.GetData<List<string>>(loveThingKey, () => { return new List<string>(); });
            if (loveThingStep != null)
            {
                if (!loveThingStep.Contains("HELLO"))
                    LocalCache1h.Remove(loveThingKey); var _cache = LocalCache1h.GetData<List<string>>(loveThingKey, () => { loveThingStep.Add("HELLO"); return loveThingStep; });
            }
            else
            {
                LocalCache1h.Remove(loveThingKey);
                var _cache = LocalCache1h.GetData<List<string>>(loveThingKey, () => { loveThingStep = new List<string> { "HELLO" }; return loveThingStep; });
            }
        }

        public static bool CustomAction(string input, string fromName, ref string customEvent, ref string customContent)
        {
            var c = false;

            #region 爱的故事 活动处理

            if (!string.IsNullOrEmpty(input) && input.Contains("爱的故事"))
            {
                //爱的故事包含3个条件（“HELLO”，“TEXT”，“IMAGE”）

                //key
                var loveThingKey = string.Format("LoveThingKey_" + fromName);

                //get cache
                var loveThingStep = LocalCache1h.GetData<List<string>>(loveThingKey, () => { return new List<string>(); });

                //首先获取看当前用户是否已经提交“爱的故事”关键字
                if (loveThingStep != null && loveThingStep.Contains("HELLO"))
                {
                    //如果故事和图片都已经发布，则跳过，什么都不做...
                    if (loveThingStep.Contains("TEXT") && loveThingStep.Contains("IMAGE"))
                    {
                        return false;
                    }
                    else if (loveThingStep.Contains("TEXT"))
                    {
                        //已经包含文字了，如果还是继续发送文字，则什么都不返回 (但不进入多客服，直到1小时缓存结束或者故事&图片都已经发齐)
                        customEvent = "";
                        customContent = "";
                        return true;
                    }
                    else if (loveThingStep.Contains("IMAGE"))
                    {
                        //已经包含发过图片，本次又发送了文字，则代表该用户已经发齐，则提示完成
                        customEvent = "";
                        customContent = "感谢参与！我们将从投稿者中挑选若干篇供大家评选，请继续关注。";

                        //更新缓存
                        LocalCache1h.Remove(loveThingKey); var _cache = LocalCache1h.GetData<List<string>>(loveThingKey, () => { loveThingStep.Add("TEXT"); return loveThingStep; });
                        return true;
                    }
                    else
                    {
                        //第一次回复故事内容
                        customEvent = "";
                        customContent = "";

                        //更新缓存
                        LocalCache1h.Remove(loveThingKey); var _cache = LocalCache1h.GetData<List<string>>(loveThingKey, () => { loveThingStep.Add("TEXT"); return loveThingStep; });
                        return true;
                    }
                }
            }

            #endregion

            return c;
        }

        public static bool CustomActionImage(string input, string fromName, ref string customEvent, ref string customContent)
        {
            var c = false;

            #region 爱的故事 活动处理

            //爱的故事包含3个条件（“HELLO”，“TEXT”，“IMAGE”）

            //key
            var loveThingKey = string.Format("LoveThingKey_" + fromName);

            //get cache
            var loveThingStep = LocalCache1h.GetData<List<string>>(loveThingKey, () => { return new List<string>(); });

            //首先获取看当前用户是否已经提交“爱的故事”关键字
            if (loveThingStep != null && loveThingStep.Contains("HELLO"))
            {
                //如果故事和图片都已经发布，则跳过，什么都不做...
                if (loveThingStep.Contains("TEXT") && loveThingStep.Contains("IMAGE"))
                {
                    return false;
                }
                else if (loveThingStep.Contains("IMAGE"))
                {
                    //已经包含图片了，如果还是继续发送图片，则什么都不返回 (但不进入多客服，直到1小时缓存结束或者故事&图片都已经发齐)
                    customEvent = "";
                    customContent = "";
                    return true;
                }
                else if (loveThingStep.Contains("TEXT"))
                {
                    //已经包含发过文字，本次又发送了图片，则代表该用户已经发齐，则提示完成
                    customEvent = "";
                    customContent = "感谢参与！我们将从投稿者中挑选若干篇供大家评选，请继续关注。";

                    //更新缓存
                    LocalCache1h.Remove(loveThingKey); var _cache = LocalCache1h.GetData<List<string>>(loveThingKey, () => { loveThingStep.Add("IMAGE"); return loveThingStep; });
                    return true;
                }
                else
                {
                    //第一次发图片
                    customEvent = "";
                    customContent = "";

                    //更新缓存
                    LocalCache1h.Remove(loveThingKey); var _cache = LocalCache1h.GetData<List<string>>(loveThingKey, () => { loveThingStep.Add("IMAGE"); return loveThingStep; });
                    return true;
                }
            }

            #endregion

            return c;
        }

        #endregion

        #region 匹配酒店检索

        /// <summary>
        /// 获取与查询值最相关的5天酒店相关推荐信息
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="articleCount"></param>
        /// <returns></returns>
        public static string SearchRelationHotelItems(string keywords, ref int articleCount)
        {
            //搜索前，首先对搜索值进行验证
            if (!CheckSearchKeywords(keywords))
            {
                return "";
            }

            var resultStr = "";
            var resultLog = string.Format("【{0}】 | {1} | ", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), keywords);

            try
            {
                //获取最相关的前5条酒店记录
                var searchResult = accessService.StrictSearch(keywords, 5);
                if (searchResult != null && searchResult.Count > 0)
                {
                    #region 拿出酒店信息

                    var hotelResultStrList = new List<string>();

                    if (searchResult.Exists(s => s.Type == SearchType.Hotel && s.HotelList != null && s.HotelList.Count > 0))
                    {
                        var hotelSearchResult = searchResult.Find(s => s.Type == SearchType.Hotel).HotelList;
                        for (int hotelNum = 0; hotelNum < hotelSearchResult.Count; hotelNum++)
                        {
                            var hotelEntity = hotelSearchResult[hotelNum];

                            resultStr += GenWeiXinTextForLinkItem(hotelEntity.HotelName, hotelEntity.HotelDesc, hotelEntity.HotelCoverPicUrl, string.Format("http://www.zmjiudian.com/hotel/{0}", hotelEntity.HotelId));

                            hotelResultStrList.Add(string.Format("{0} http://www.zmjiudian.com/hotel/{1}", hotelEntity.HotelName, hotelEntity.HotelId));

                            //记录日志数据
                            if (articleCount > 0) resultLog += " ,";
                            resultLog += string.Format("{0} http://www.zmjiudian.com/hotel/{1}", hotelEntity.HotelName, hotelEntity.HotelId);

                            articleCount++;
                        }
                    }

                    #endregion

                    #region 拿出目的地记录

                    var districtResultStrList = new List<string>();

                    if (searchResult.Exists(s => s.Type == SearchType.District && s.DistrictList != null && s.DistrictList.Count > 0))
                    {
                        var districtSearchResult = searchResult.Find(s => s.Type == SearchType.District).DistrictList;
                        for (int disNum = 0; disNum < districtSearchResult.Count; disNum++)
                        {
                            var districtEntity = districtSearchResult[disNum];

                            resultStr += GenWeiXinTextForLinkItem(string.Format("查看{0}所有主题酒店", districtEntity.Name), string.Format("{0}的所有主题酒店", districtEntity.Name), "", string.Format("http://www.zmjiudian.com/city{0}", districtEntity.DistrictId));

                            districtResultStrList.Add(string.Format("您是想查找{0}酒店？点击以下连接查找{0}酒店 http://www.zmjiudian.com/city{1}", districtEntity.Name, districtEntity.DistrictId));

                            //记录日志数据
                            if (articleCount > 0) resultLog += " ,";
                            resultLog += string.Format("{0} http://www.zmjiudian.com/city{1}", districtEntity.Name, districtEntity.DistrictId);

                            articleCount++;
                        }
                    }

                    #endregion

                    #region 拿出目的地+主题记录

                    var themeResultStrList = new List<string>();

                    if (searchResult.Exists(s => s.Type == SearchType.Theme && s.ThemeList != null && s.ThemeList.Count > 0))
                    {
                        var themeSearchResult = searchResult.Find(s => s.Type == SearchType.Theme).ThemeList;
                        for (int themeNum = 0; themeNum < themeSearchResult.Count; themeNum++)
                        {
                            var themeEntity = themeSearchResult[themeNum];

                            resultStr += GenWeiXinTextForLinkItem(string.Format("查看{0}{1}酒店", themeEntity.DistrictName, themeEntity.Name), themeEntity.DistrictName + themeEntity.Name, "", string.Format("http://www.zmjiudian.com/city{0}/theme{1}", themeEntity.DistrictId, themeEntity.ThemeId));

                            themeResultStrList.Add(string.Format("{0}{1}酒店 {2}", themeEntity.DistrictName, themeEntity.Name, string.Format("http://www.zmjiudian.com/city{0}/theme{1}", themeEntity.DistrictId, themeEntity.ThemeId)));

                            //记录日志数据
                            if (articleCount > 0) resultLog += " ,";
                            resultLog += string.Format("{0}{1} http://www.zmjiudian.com/city{2}/theme{3}", themeEntity.DistrictName, themeEntity.Name, themeEntity.DistrictId, themeEntity.ThemeId);

                            articleCount++;
                        }
                    }

                    #endregion

                    #region 当没有酒店记录，没有目的地+主题记录，只有目的地记录时，以文本方式返回目的地链接信息

                    if (hotelResultStrList.Count == 0 && themeResultStrList.Count == 0 && districtResultStrList.Count > 0)
                    {
                        resultStr = districtResultStrList[0];

                        //追加人工客服提示
                        resultStr += string.Format(@"
{0}", "回复“服务”咨询人工客服");

                        articleCount = 0;
                    }
                    else if (!string.IsNullOrEmpty(resultStr))
                    {
                        resultStr += GenWeiXinTextForLinkItem("回复“服务”咨询人工客服", "回复“服务”咨询人工客服（工作时间：8:00——24:00)", "", "http://www.zmjiudian.com/contactus");
                        articleCount++;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(string.Format(@"D:\Log\WeixinService\ErrorLog_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), string.Format(@"
{0}", ex.Message));
            }

            //记录搜索日志
            var logPath = @"D:\Log\WeixinService\SearchRelationHotelHistory_{0}.txt";
            try
            {
                File.AppendAllText(string.Format(logPath, DateTime.Now.ToString("yyyyMMdd")), string.Format(@"
{0}", resultLog));
            }
            catch (Exception ex)
            {
                File.AppendAllText(string.Format(logPath, DateTime.Now.ToString("yyyyMMdd")), string.Format(@"
{0}", resultLog));
            }


            return resultStr;
        }

        public static string[] SearchFilterWords = new string[] { 
        "你好", "hello", "嗨", "hi", "帮助", "帮忙", "帮个忙", "help", "thanks", "tks", "thank u", "tku", "tk u", "thank you", "好的", "ok", "嗯嗯", "嗯呢", "好吧", "好吗", "好嘛", "好呀", "行吗", "行嘛", "如何", "可否", "能否", "务必", "怎么样", "怎样", "不好", "不行", "no", "可以", "不可以", "什么", "什么时候", "什么情况", "必须", "不会", "不会吧"
        , "我们", "你们", "他们", "她们", "它们", "自己", "几个", "一个", "一群", "一帮", "一群人", "一帮人", "一个人", "很多", "很多人", "大家", "you", "me", "him", "she", "it", "is", "id", "and", "or", "see", "say", "can", "not", "yes", "yeah", "oh", "fuck", "fk", "go", "card", "code", "what", "how"
        , "请问", "好不好", "怎么办", "吃饭", "喝茶", "茶水", "喝酒", "酒水", "啤酒", "抽烟", "咨询", "今天", "明天", "昨天", "今年", "明年", "去年"
        , "再见", "在吗", "再会", "拜拜", "拜", "bye", "byebye", "说话", "吃饭", "单早", "双早", "无早", "早餐", "中餐", "晚餐", "夜宵", "早晚", "天气", "看见", "看见了", "看不见", "看到", "看到吗", "看不到", "早安", "晚安", "早上好", "中午好", "下午好"
        , "男", "女", "男人", "女人", "男孩", "女孩", "小孩", "儿童", "小朋友", "朋友", "大人", "达人", "游戏", "游玩", "玩耍", "玩具" , "烧烤", "小吃" , "草坪", "黑暗料理", "料理" , "米饭" , "面条" 
        , "这里", "那里", "这边", "那边", "哪里", "哪儿", "哪位", "学校", "当地", "当地人", "外地", "外地人", "老乡", "地方"
        , "体育", "健身", "运动", "篮球", "乒乓球", "足球", "排球", "网球", "台球", "泳池", "游泳", "golf", "出差", "旅行", "旅游"
        , "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天", "星期日", "周一", "周二", "周三", "周四", "周五", "周六", "周末", "周日", "下周", "周几", "几号"
        , "good", "great", "nice", "luck", "like", "body", "boy", "girl"
        , "小狗", "小猫", "猫狗"
        };

        public static string[] SearchNoContainsWords = new string[] { 
        "服务"
        ,"周末酒店","尚旅","尚酒店","品鉴"
        ,"客服","呼叫","哈喽","拜拜","谢谢","合作","推广","咨询","问题","解决","支持","粉丝","招聘","应聘"
        ,"携程","去哪儿","途牛","捷旅","驴妈妈","同程","美团","大众点评","淘宝","天猫","京东","微信","微博","支付","结算","付款","汇款","银联","众筹"
        , "好吗", "行吗", "行嘛", "是否", "在吗", "在不在", "在嘛", "在么", "好的", "后来", "原来", "赞同", "同意", "不喜欢", "建议", "尊重"
        , "我", "你", "他", "她", "它", "谁", "誰", "自己", "别人"
        , "什么时候", "什么东东", "什么东西", "什么意思", "什么玩意", "怎么", "为什么", "为何", "干什么", "应该", "不要", "只能", "只可以", "只是"
        , "why", "what", "how", "please"
        , "出来", "出去", "打不开", "打开", "气愤", "生气", "郁闷", "费解", "啰嗦", "浪费", "感慨", "情愿", "矛盾", "别扭", "愉快", "无聊", "没意思", "有趣"
        , "一间", "二间", "两间", "三间", "一层", "一夜", "一晚", "第一天", "第二天", "第三天", "许多", "很多", "特别多", "特别大", "特别小", "特别脏", "特别干净"
        ,"取消","发票","门票","订单","app","下载","客户端","网站","现金券","优惠券","账号","帐号","账户","帐户","手机","密码","验证码","退出","手续","身份证","护照","签证","户口","购买","单据"
        };

        /// <summary>
        /// 检查搜索值得合法性
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private static bool CheckSearchKeywords(string keywords)
        {
            var result = true;

            keywords = keywords.ToLower().Trim();

            #region 一个字不搜索

            if (keywords.Length < 2)
            {
                return false;
            }

            #endregion

            #region 不能等于指定词

            if (SearchFilterWords.ToList().Exists(sk => sk.ToLower().Trim() == keywords))
            {
                return false;
            }

            #endregion

            #region 纯数字不搜索（暂时的，以后如果通过一些数字进行特定的搜索再解除 2015-10-16 haoy）

            try
            {
                var testNumber = Convert.ToInt64(keywords.Replace(" ", ""));
                return false;
            }
            catch (Exception ex)
            {

            }

            #endregion

            #region 不能包含指定词

            foreach (var noContainsWord in SearchNoContainsWords)
            {
                if (keywords.Contains(noContainsWord))
                {
                    return false;
                }
            }

            #endregion

            return result;
        }

        #endregion

        #region 微信活动相关处理

        /// <summary>
        /// 发布增加新抽奖码的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public bool PublishWeixinLuckCodeTask(ActiveWeixinLuckCode aluck)
        {
            try
            {
                var taskStr = JsonConvert.SerializeObject(aluck);

                ActiveWeixinLuckCodeChannel.BasicPublish("", wxLuckQueueKey, null, Encoding.UTF8.GetBytes(taskStr));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        #region 抽奖码队列实例

        static IModel mWxLuckChannel;
        static IConnection wxLuckConnection;
        static int initWxLuckChannel = 0;
        static string wxLuckQueueKey = "PublishWeixinLuckCodeTask";

        public IModel ActiveWeixinLuckCodeChannel
        {
            get
            {
                if (initWxLuckChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = RabbitmqConfig.RabbitmqHostName;
                    factory.UserName = RabbitmqConfig.RabbitmqUserName;
                    factory.Password = RabbitmqConfig.RabbitmqPassword;

                    wxLuckConnection = factory.CreateConnection();
                    mWxLuckChannel = wxLuckConnection.CreateModel();
                    mWxLuckChannel.QueueDeclare(wxLuckQueueKey, false, false, false, null);

                    initWxLuckChannel = 1;
                }
                return mWxLuckChannel;
            }
        }

        #endregion

        /// <summary>
        /// 发布增加宝石的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public bool PublishWeixinGemTask(ActiveWeixinLuckCode aluck)
        {
            try
            {
                var taskStr = JsonConvert.SerializeObject(aluck);

                ActiveWeixinGemChannel.BasicPublish("", wxGemQueueKey, null, Encoding.UTF8.GetBytes(taskStr));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        #region 奖励宝石队列实例

        static IModel mWxGemChannel;
        static IConnection wxGemConnection;
        static int initWxGemChannel = 0;
        static string wxGemQueueKey = "PublishWeixinGemTask";

        public IModel ActiveWeixinGemChannel
        {
            get
            {
                if (initWxGemChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = RabbitmqConfig.RabbitmqHostName;
                    factory.UserName = RabbitmqConfig.RabbitmqUserName;
                    factory.Password = RabbitmqConfig.RabbitmqPassword;

                    wxGemConnection = factory.CreateConnection();
                    mWxGemChannel = wxGemConnection.CreateModel();
                    mWxGemChannel.QueueDeclare(wxGemQueueKey, false, false, false, null);

                    initWxGemChannel = 1;
                }
                return mWxGemChannel;
            }
        }

        #endregion

        #endregion

        #region 微信公众号操作相关

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="backJson"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUserSubscribeInfo(string openid, WeiXinChannelCode ChannelCode = WeiXinChannelCode.周末酒店订阅号)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";

            string token = WeiXinToken.GetWeixinTokenByCode(ChannelCode);
            url = string.Format(url, token, openid);
            var backJson = HttpHelper.HttpGet(url, "");
            var obj = JsonConvert.DeserializeObject<WeixinUser>(backJson);
            if (obj != null && !string.IsNullOrEmpty(obj.SubscribeTimeStr))
            {
                try
                {
                    obj.SubscribeTime = StampToDateTime(obj.SubscribeTimeStr);
                }
                catch (Exception ex)
                {
                    
                }
            }

            LogHelper.WriteLog(string.Format("GetWeixinUserSubscribeInfo:{0} {1} {2}", openid, ChannelCode, backJson));

            return obj;
        }

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="ChannelCode"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUserSubscribeInfo2(string openid, int ChannelCode)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";

            string token = WeiXinToken.GetWeixinTokenByCode((WeiXinChannelCode)ChannelCode);
            url = string.Format(url, token, openid);
            var backJson = HttpHelper.HttpGet(url, "");
            var obj = JsonConvert.DeserializeObject<WeixinUser>(backJson);
            if (obj != null && !string.IsNullOrEmpty(obj.SubscribeTimeStr))
            {
                try
                {
                    obj.SubscribeTime = StampToDateTime(obj.SubscribeTimeStr);
                    obj.CreateTime = DateTime.Now;
                }
                catch (Exception ex)
                {

                }
            }

            LogHelper.WriteLog(string.Format("GetWeixinUserSubscribeInfo2:{0} {1} {2}", openid, ChannelCode, backJson));

            return obj;
        }

        public static DateTime StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 发送指定微信token的自定义信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        public static void SendWeixinMsg(string token, object obj)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}", token);
            CookieContainer cc = new CookieContainer();
            string json = HttpHelper.PostJson(url, obj, ref cc);
            //JsonConvert.DeserializeObject<string>(json);
        }

        #endregion
    }
}
