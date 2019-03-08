using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelServices.Contracts;
using HJD.WeixinService.Contract;
using HJD.WeixinServices.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    /// <summary>
    /// 尚旅游订阅号
    /// </summary>
    public class WeixinShanglvyouController : WeixinBaseController
    {
        public WeixinShanglvyouController()
        {
            base.ChannelCode = WeiXinChannelCode.尚旅游订阅号;
            base.logFile = Configs.LogPath + string.Format("WeiXinShangLvYouLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_shanglvyou";

        }
      
  
        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity3();

            return WeiXinAdapter.weixinActivityList3.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }         
    }

    /// <summary>
    /// 尚旅游成都订阅号
    /// </summary>
    public class WeixinShanglvyouCDController : WeixinBaseController
    {
        public WeixinShanglvyouCDController()
        {
            base.ChannelCode = WeiXinChannelCode.尚旅游成都订阅号;
            base.logFile = Configs.LogPath + string.Format("WeiXinShangLvYouCDLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_shanglvyou_cd";

        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity4();

            return WeiXinAdapter.weixinActivityList4.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 周末酒店服务号 浩颐    EncodingAESKey: THErxVee82js8DfsLkFP649CxrRfJTBTtPpqeTXFZL0
    /// </summary>
    public class WeixinHaoYiServiceController : WeixinBaseController
    {
        public WeixinHaoYiServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.周末酒店服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice";

        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity7();

            return WeiXinAdapter.weixinActivityList7.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }
     

    /// <summary>
    /// 周末酒店苏州服务号
    /// </summary>
    public class WeixinHaoYiSZServiceController : WeixinBaseController
    {
        public WeixinHaoYiSZServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.周末酒店苏州服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiSZServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_sz";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity8();

            return WeiXinAdapter.weixinActivityList8.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 周末酒店成都服务号
    /// </summary>
    public class WeixinHaoYiCDServiceController : WeixinBaseController
    {
        public WeixinHaoYiCDServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.周末酒店成都服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiCDServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_cd";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity9();

            return WeiXinAdapter.weixinActivityList9.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 周末酒店深圳服务号
    /// </summary>
    public class WeixinHaoYiShenZServiceController : WeixinBaseController
    {
        public WeixinHaoYiShenZServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.周末酒店深圳服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiShenZServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_shenz";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity10();

            return WeiXinAdapter.weixinActivityList10.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 遛娃指南深圳 订阅号
    /// </summary>
    public class WeixinLiuwaSHZHController : WeixinBaseController
    {
        public WeixinLiuwaSHZHController()
        {
            base.ChannelCode = WeiXinChannelCode.遛娃指南深圳订阅号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiLiuwaSHZHLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyi_liuwa_shzh";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity17();

            return WeiXinAdapter.weixinActivityList17.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 美味至尚订阅号
    /// </summary>
    public class WeixinMeiweizhishangController : WeixinBaseController
    {
        public WeixinMeiweizhishangController()
        {
            base.ChannelCode = WeiXinChannelCode.美味至尚订阅号;
            base.logFile = Configs.LogPath + string.Format("WeiXinMeiweizhishangLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_meiweizhishang";

        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity5();

            return WeiXinAdapter.weixinActivityList5.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 尚旅游北京订阅号
    /// </summary>
    public class WeixinShanglvyouBJController : WeixinBaseController
    {
        public WeixinShanglvyouBJController()
        {
            base.ChannelCode = WeiXinChannelCode.尚旅游北京订阅号;
            base.logFile = Configs.LogPath + string.Format("WeiXinShangLvYouBJLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_shanglvyou_bj";

        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity6();

            return WeiXinAdapter.weixinActivityList6.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 遛娃指南服务号
    /// </summary>
    public class WeixinLiuwaServiceController : WeixinBaseController
    {
        public WeixinLiuwaServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.遛娃指南服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiLiuwaServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_liuwa";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity11();

            return WeiXinAdapter.weixinActivityList11.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 遛娃指南南京服务号
    /// </summary>
    public class WeixinLiuwaNJServiceController : WeixinBaseController
    {
        public WeixinLiuwaNJServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.遛娃指南南京服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiLiuwaNJServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_liuwa_nj";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity13();

            return WeiXinAdapter.weixinActivityList13.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 遛娃指南无锡服务号
    /// </summary>
    public class WeixinLiuwaWXServiceController : WeixinBaseController
    {
        public WeixinLiuwaWXServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.遛娃指南无锡服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiLiuwaWXServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_liuwa_wx";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity14();

            return WeiXinAdapter.weixinActivityList14.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 遛娃指南广州服务号
    /// </summary>
    public class WeixinLiuwaGZServiceController : WeixinBaseController
    {
        public WeixinLiuwaGZServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.遛娃指南广州服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiLiuwaGZServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_liuwa_gz";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity15();

            return WeiXinAdapter.weixinActivityList15.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 遛娃指南杭州服务号
    /// </summary>
    public class WeixinLiuwaHZHServiceController : WeixinBaseController
    {
        public WeixinLiuwaHZHServiceController()
        {
            base.ChannelCode = WeiXinChannelCode.遛娃指南杭州服务号_皓颐;
            base.logFile = Configs.LogPath + string.Format("WeiXinHaoYiLiuwaHZHServiceLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyiservice_liuwa_hzh";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity16();

            return WeiXinAdapter.weixinActivityList16.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    /// <summary>
    /// 遛娃指南 
    /// </summary>
    public class WeixinLiuwaController : WeixinBaseController
    {
        public WeixinLiuwaController()
        {
            base.ChannelCode = WeiXinChannelCode.遛娃指南;
            base.logFile = Configs.LogPath + string.Format("WeiXinLiuwaLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_haoyi_liuwa";
        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity11();

            return WeiXinAdapter.weixinActivityList11.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    #region 小程序相关服务

    /// <summary>
    /// 周末酒店Lite 小程序
    /// </summary>
    public class WeixinZmjdLiteController : WeixinBaseController
    {
        public WeixinZmjdLiteController()
        {
            base.ChannelCode = WeiXinChannelCode.小程序_周末酒店Lite;
            base.logFile = Configs.LogPath + string.Format("WeixinZmjdLiteLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            base.subscribeEventCode = "subscribe_zmjdlite";

        }


        //http://api.zmjiudian.com/api/Weixin/Data?signature=si&timestamp=23423423&nonce=123&echostr=echostr
        public override List<WeixinActivityEntity> GetWeixinActivityList()
        {
            WeiXinAdapter.LoadWeixinActivity100();

            return WeiXinAdapter.weixinActivityList100.Where(a => a.ActivityFinishDateTime > DateTime.Now).ToList();

        }

        [HttpGet]
        [ActionName("Data")]
        public string Valid(string signature, string timestamp, string nonce, string echostr)
        {
            return base.ValidData(signature, timestamp, nonce, echostr);
        }


        [HttpPost]
        [ActionName("Data")]
        public string Data()
        {
            return DataAction();
        }
    }

    #endregion
}
