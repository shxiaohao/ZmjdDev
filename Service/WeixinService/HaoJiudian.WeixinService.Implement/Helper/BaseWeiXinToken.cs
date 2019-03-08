using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.WeixinService.Implement.Helper
{
   

    public class WeiXinTokenBase
    {

        private string token = "";
        private DateTime tokenExpiresTime = DateTime.Now;

        private string ticket = "";
        private DateTime ticketExpiresTime = DateTime.Now;

        private string APPID { get; set; }
        private string WeiXinSecret { get; set; }

        public WeiXinTokenBase(string appID, string weiXinSecret)
        {
            APPID = appID;
            WeiXinSecret = weiXinSecret;
        }

        public string GetToken()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                GenToken();
            }
            return token;
        }

        public string GenToken()
        {

            string url = "https://api.weixin.qq.com/cgi-bin/token";
            string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", APPID, WeiXinSecret);

            string json = HttpHelper.HttpGet(url, pars);

            TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

            tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
            token = t.access_token;
            return token;
        }

        public string GetTicket()
        {
            if (token == "" || tokenExpiresTime < DateTime.Now.AddSeconds(5))
            {
                string APPID = Configs.WeiXinAPPID;
                string WeiXinSecret = Configs.WeiXinSecret;
                string url = "https://api.weixin.qq.com/cgi-bin/token";
                string pars = string.Format("grant_type=client_credential&appid={0}&secret={1}", APPID, WeiXinSecret);

                string json = HttpHelper.HttpGet(url, pars);

                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                tokenExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                token = t.access_token;

                //此时过期或没有
                if (ticket == "" || ticketExpiresTime < DateTime.Now.AddSeconds(5))
                {
                    string url2 = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                    string pars2 = string.Format("access_token={0}&type=jsapi", token);

                    string json2 = HttpHelper.HttpGet(url2, pars2);
                    //token返回值与ticket相同 公用一个类存值
                    TockenClass t2 = JsonConvert.DeserializeObject<TockenClass>(json2);

                    ticketExpiresTime = DateTime.Now.AddSeconds(t2.expires_in);
                    ticket = t2.ticket;
                }
            }
            else if (ticket == "")
            {
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";
                string pars = string.Format("access_token={0}&type=jsapi", token);

                string json = HttpHelper.HttpGet(url, pars);
                //token返回值与ticket相同 公用一个类存值
                TockenClass t = JsonConvert.DeserializeObject<TockenClass>(json);

                ticketExpiresTime = DateTime.Now.AddSeconds(t.expires_in);
                ticket = t.ticket;
            }
            return ticket;
        }
    }
}
