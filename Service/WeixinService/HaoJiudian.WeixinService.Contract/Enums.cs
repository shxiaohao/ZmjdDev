using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.WeixinService.Contract
{
    public enum WeiXinChannelCode
    {
        UNKNOW = 0,
        周末酒店订阅号 = 1,    // 1周末酒店  
        周末酒店服务号 = 2,    // 2周末酒店服务号
        尚旅游订阅号 = 3,      // 3尚旅游
        尚旅游成都订阅号 = 4,  // 4尚旅游成都
        美味至尚订阅号 = 5,    // 5美味至尚
        尚旅游北京订阅号 = 6,   // 6尚旅游北京
        周末酒店服务号_皓颐 = 7,    // 周末酒店服务号_皓颐
        周末酒店苏州服务号_皓颐 = 8,    // 周末酒店苏州服务号_皓颐
        周末酒店成都服务号_皓颐 = 9,    // 周末酒店成都服务号_皓颐
        周末酒店深圳服务号_皓颐 = 10,   // 周末酒店深圳服务号_皓颐
        遛娃指南服务号_皓颐 = 11,   // 遛娃指南服务号_皓颐
        遛娃指南 = 12,
        遛娃指南南京服务号_皓颐 = 13,   // 遛娃指南南京服务号_皓颐
        遛娃指南无锡服务号_皓颐 = 14,   // 遛娃指南无锡服务号_皓颐
        遛娃指南广州服务号_皓颐 = 15,   // 遛娃指南广州服务号_皓颐
        遛娃指南杭州服务号_皓颐 = 16,   // 遛娃指南杭州服务号_皓颐
        遛娃指南深圳订阅号_皓颐 = 17,   // 遛娃指南深圳订阅号_皓颐
        小程序_周末酒店Lite = 100,   // 小程序_周末酒店Lite
    }

    public enum WeiXinOrgID
    {
        gh_00367eba4731 = 1,//                    channelCode = WeiXinChannelCode.周末酒店订阅号;
        gh_cbfd3323e3d1 = 2,//                    channelCode = WeiXinChannelCode.周末酒店服务号;
        gh_d5050909b780 = 3,//                    channelCode = WeiXinChannelCode.尚旅游订阅号;
        gh_cb226128781f = 4, //                  channelCode = WeiXinChannelCode.尚旅游成都订阅号;
        gh_67733185d58b = 5, //                 channelCode = WeiXinChannelCode.美味至尚订阅号; 
        gh_7622f728d6c1 = 7, //周末酒店服务号_皓颐
        gh_978eb3b72734 = 8, //周末酒店苏州服务号_皓颐
        gh_9ff65c56d233 = 11,
        gh_f3f10b7c79b1 = 12,
    }

}
