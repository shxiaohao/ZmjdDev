using HJDAPI.Controllers.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public  class EMChatAdapter
    {
     string reqUrlFormat = "https://a1.easemob.com/{0}/{1}/";
        public string clientID { get; set; }
        public string clientSecret { get; set; }
        public string appName { get; set; }
        public string orgName { get; set; }
        public string token { get; set; }
        public string easeMobUrl { get { return string.Format(reqUrlFormat, orgName, appName); } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="easeAppClientID">client_id</param>
        /// <param name="easeAppClientSecret">client_secret</param>
        /// <param name="easeAppName">应用标识之应用名称</param>
        /// <param name="easeAppOrgName">应用标识之登录账号</param>
        public EMChatAdapter(string easeAppClientID, string easeAppClientSecret, string easeAppName, string easeAppOrgName)
        {
            this.clientID = easeAppClientID;
            this.clientSecret = easeAppClientSecret;
            this.appName = easeAppName;
            this.orgName = easeAppOrgName;
            this.token = QueryToken();
        }

        public EMChatAdapter()
        {
            this.clientID = "YXA6iAmEUKecEeWn8jkHypnHHA";
            this.clientSecret = "YXA6U_jDx2wMHit9UX9s555Euugm-Kw";
            this.appName = "zmjd100";
            this.orgName = "zmjiudian";// "zmjdAdmin";
            this.token = QueryToken();
        }

        /// <summary>
        /// 使用app的client_id 和 client_secret登陆并获取授权token
        /// </summary>
        /// <returns></returns>
        string QueryToken()
        {
            if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret)) { return string.Empty; }
            string cacheKey = clientID + clientSecret;
            if (System.Web.HttpRuntime.Cache.Get(cacheKey) != null &&
                System.Web.HttpRuntime.Cache.Get(cacheKey).ToString().Length > 0)
            {
                return System.Web.HttpRuntime.Cache.Get(cacheKey).ToString();
            }
            else
            {

                string postUrl = easeMobUrl + "token";
                StringBuilder _build = new StringBuilder();
                _build.Append("{");
                _build.AppendFormat("\"grant_type\": \"client_credentials\",\"client_id\": \"{0}\",\"client_secret\": \"{1}\"", clientID, clientSecret);
                _build.Append("}");

                string postResultStr = ReqUrl(postUrl, "POST", _build.ToString(), string.Empty);
                string token = string.Empty;
                int expireSeconds = 0;
                try
                {
                    JObject jo = JObject.Parse(postResultStr);
                    token = jo.GetValue("access_token").ToString();
                    int.TryParse(jo.GetValue("expires_in").ToString(), out expireSeconds);
                    //设置缓存
                    if (!string.IsNullOrEmpty(token) && token.Length > 0 && expireSeconds > 0)
                    {
                        expireSeconds = expireSeconds - 10 < 10 ? expireSeconds : expireSeconds - 10;
                        System.Web.HttpRuntime.Cache.Insert(cacheKey, token, null, DateTime.Now.AddSeconds(expireSeconds - 10), System.TimeSpan.Zero);
                    }
                }
                catch { return postResultStr; }
                return token;
            }
        }


        public string GetChatLogs(long  lastTimeStamp )
        {
            string getData = string.Format("ql=select * where timestamp>{0}&limit=1000 ", lastTimeStamp);

            return GetMessages("chatmessages", getData);
        }

        /// <summary>
        /// 将微信消息转发给环信客服
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgId"></param>
        /// <param name="openid"></param>
        /// <param name="mp"></param>
        /// <param name="userNickname"></param>
        /// <returns></returns>
        public string SendWeixinMsgToKeFu(string msg, string msgId, string openid, string mp, string userNickname, string msgType = "text", string picURL = "")
        {
            EMChatAdapter emChat = new EMChatAdapter();
            string target_type = "users";
            string target = "kevincai";  //在环信移动客服中设置的关联IM账号
         //   string msg = "这里是微信粉丝发送给公众号的文本信息 KevinCai";
            string from = "kefu001_callback"; //用于开通实时消息回调功能的IM账号
          //   string source = "weixin";//来源渠道微信，固定值
           // string msgId = "0c9a31ed-430b-4edf-9aaf-6029f2674534"; //消息id
        //    string openid = "oAGnbt89fTgfzOlybTmAh_7s3Z_g";
         //   string mp = "codelife"; //微信公众号ID
          //  string userNickname = "微信粉丝昵称KevinCai";

            Dictionary<string, string> dicExt = new Dictionary<string, string>();

            string json = string.Format(@"{{""visitor"":{{""source"":""weixin"",""msgId"":""{0}"",""openid"":""{1}"",""mp"":""{2}"",""userNickname"":""{3}""}}}}", msgId, openid, mp, userNickname);

            dicExt.Add("weichat", json);
            //  dicExt.Add("agentName", "kefu003@zmjiudian.com");
            msg = System.Web.HttpUtility.HtmlEncode(msg).Replace("\r\n", "<br>").Replace("\r", "<br>").Replace("\n", "<br>").Replace("\\","\\\\").Replace("\"","\\\"");
            string result = "";
            switch( msgType )
            {
                case "text":
                    result = SendTxtMessageToUser(target_type, new List<string> { target }, msg, from, dicExt);
                    break;
                case "image":
                    result = SendImgMessageToUser(target_type, new List<string> { target }, picURL,msgId +".jpg", from, dicExt);
                    break;
            }


            //Log.WriteLog(string.Format("SendWeixinMsgToKeFu: {0} {1} {2} {3}", msg, picURL, json, result ));

            return result;
        }

        private string SendImgMessageToUser(string target_type, List<string> targetList, string picURL, string fileName, string from, Dictionary<string, string> dicExt)
        {

//            { "target_type":"users",
//  "target":["kefu001"], //在环信移动客服中设置的关联IM账号
//  "msg":{
//      "type":"img", //固定值
//      "filename":"name.jpg", //任意指定
//      "secret":"secretsecretsecretsecret",  //任意指定，具体作用可见官网API文档
//      "url":"http://pic1.ooopic.com/uploadfilepic/sheji.jpg"  //微信粉丝发送的图片url
//      },
//  "from":"kefu001_callback", //用于开通实时消息回调功能的IM账号
//   "ext" : {
//       "weichat":{
//           "visitor":{ 
//                  "source" : "weixin",  //固定值
//                  "msgId" : "0c9a31ed-430b-4edf-9aaf-6029f2674534",  //微信消息id
//                  "openid": "oAGnbt89fTgfzOlybTmAh_7s3Z_g",
//                  "mp":"gh_9856dc55a1ea", //公众号ID
//                  "userNickname":"微信粉丝昵称", //粉丝昵称
//           }
//       }
//   }
//}

            StringBuilder _build = new StringBuilder();
            _build.Append("{");
            _build.AppendFormat("\"target_type\":\"{0}\",\"target\":[{1}],", target_type, string.Join(",", targetList.Select(t => "\"" + t + "\"")));
            _build.Append("\"msg\":{");
            _build.Append("\"type\":\"img\",");
            _build.AppendFormat("\"filename\":\"{0}\",", fileName);
            _build.Append("\"secret\":\"\",");
            _build.AppendFormat("\"url\":\"{0}\"", picURL); 
            _build.Append("}");
            if (from.Length > 0)
            {
                _build.AppendFormat(",\"from\":\"{0}\"", from);
            }
            if (dicExt.Count > 0)
            {
                _build.Append(",\"ext\":{");
                _build.AppendFormat("{0}", string.Join(",", dicExt.Select(d => "\"" + d.Key + "\":" + (d.Value.StartsWith("{") ? d.Value : "\"" + d.Value + "\""))));
                _build.Append("}");
            }
            _build.Append("}");


            string strResult = PostMessages("messages", _build.ToString());

            //  Log.WriteLog(string.Format("SendTxtMessageToUser:{0}   {1}", _build.ToString() , strResult));

            return strResult;
        }


    //                {
    //"target_type" : "users", // users 给用户发消息, chatgroups 给群发消息, chatrooms 给聊天室发消息
    //"target" : ["u1", "u2", "u3"], // 注意这里需要用数组,数组长度建议不大于20, 即使只有一个用户,   
    //                               // 也要用数组 ['u1'], 给用户发送时数组元素是用户名,给群组发送时  
    //                               // 数组元素是groupid
    //"msg" : {
    //    "type" : "txt",
    //    "msg" : "hello from rest" //消息内容，参考[[start:100serverintegration:30chatlog|聊天记录]]里的bodies内容
    //    },
    //"from" : "jma2", //表示消息发送者, 无此字段Server会默认设置为"from":"admin",有from字段但值为空串("")时请求失败
    //"ext" : { //扩展属性, 由app自己定义.可以没有这个字段，但是如果有，值不能是“ext:null“这种形式，否则出错
    //    "attr1" : "v1",
    //    "attr2" : "v2"
    //} 

      
        public string SendTxtMessageToUser(string target_type , List<string> targetList,string msg, string from, Dictionary<string,string> dicExt )
        {

            //StringBuilder _build = new StringBuilder();
            //_build.Append("{");
            //_build.AppendFormat("\"target_type\":\"{0}\",\"target\":[{1}],",target_type,string.Join(",", targetList.Select(t=> "\"" + t + "\"")));
            //_build.Append("\"msg\":{");
            //_build.AppendFormat("\"type\":\"txt\",\"msg\":\"{0}\"", msg);
            //_build.Append("}");
            //if (from.Length > 0)
            //{
            //    _build.AppendFormat(",\"from\":\"{0}\"", from);
            //}
            //if (dicExt.Count > 0)
            //{
            //    _build.Append(",\"ext\":{");
            //    _build.AppendFormat("\"{0}\"",  string.Join(",", dicExt.Select(d => "\"" + d.Key + "\":\"" + d.Value + "\"")));
            //    _build.Append("}");
            //}
            //_build.Append("}");

            StringBuilder _build = new StringBuilder();
            _build.Append("{");
            _build.AppendFormat("\"target_type\":\"{0}\",\"target\":[{1}],", target_type, string.Join(",", targetList.Select(t => "\"" + t + "\"")));
            _build.Append("\"msg\":{");
            _build.AppendFormat("\"type\":\"txt\",\"msg\":\"{0}\"", msg);
            _build.Append("}");
            if (from.Length > 0)
            {
                _build.AppendFormat(",\"from\":\"{0}\"", from);
            }
            if (dicExt.Count > 0)
            {
                _build.Append(",\"ext\":{");
                _build.AppendFormat("{0}", string.Join(",", dicExt.Select(d => "\"" + d.Key + "\":" + (d.Value.StartsWith("{") ? d.Value : "\"" + d.Value + "\""))));
                _build.Append("}");
            }
            _build.Append("}");


            string strResult = PostMessages("messages", _build.ToString());

          //  Log.WriteLog(string.Format("SendTxtMessageToUser:{0}   {1}", _build.ToString() , strResult));

            return strResult;
        }
        public string SendPicTxtMessageToUser(string target_type, List<string> targetList, string msg, string from, Dictionary<string, string> dicExt)
        {
            StringBuilder _build = new StringBuilder();
            _build.Append("{");
            _build.AppendFormat("\"target_type\":\"{0}\",\"target\":[{1}],", target_type, string.Join(",", targetList.Select(t => "\"" + t + "\"")));
            _build.Append("\"msg\":{");
            _build.AppendFormat("\"type\":\"txt\",\"msg\":\"{0}\"", msg);
            _build.Append("}");
            if (from.Length > 0)
            {
                _build.AppendFormat(",\"from\":\"{0}\"", from);
            }
            if (dicExt.Count > 0)
            {
                _build.Append(",\"ext\":{");
                _build.AppendFormat("{0}", string.Join(",", dicExt.Select(d => "\"" + d.Key + "\":" + (d.Value.StartsWith("{") ? d.Value : "\"" + d.Value + "\""))));
                _build.Append("}");
            }
            _build.Append("}");


            string strResult = PostMessages("messages", _build.ToString());

            return strResult;
        }
        /// <summary>
        /// 创建用户(可以批量创建)
        /// </summary>
        /// <param name="postData">创建账号JSON数组--可以一个，也可以多个</param>
        /// <returns>创建成功的用户JSON</returns>
        public string PostMessages(string methodName, string postData) { return ReqUrl(easeMobUrl + methodName, "POST", postData, token); }
        public string GetMessages(string methodName, string getData) { return ReqUrl(easeMobUrl + methodName + "?" + getData, "GET", "", token); }


        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <returns>创建成功的用户JSON</returns>
          public  string AccountCreate(string userName, string password, string nickName)
        {
            StringBuilder _build = new StringBuilder();
            _build.Append("{");
            _build.AppendFormat("\"username\": \"{0}\",\"password\": \"{1}\"", userName, password);////,\"nickname\": \"{2}\"", userName, password, nickName);
            _build.Append("}");

            return AccountCreate(_build.ToString());
        }

        /// <summary>
        /// 创建用户(可以批量创建)
        /// </summary>
        /// <param name="postData">创建账号JSON数组--可以一个，也可以多个</param>
        /// <returns>创建成功的用户JSON</returns>
        public  string AccountCreate(string postData) { return ReqUrl(easeMobUrl + "users", "POST", postData, token); }

        /// <summary>
        /// 获取指定用户详情
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns>会员JSON</returns>
        public string AccountGet(string userName) { return ReqUrl(easeMobUrl + "users/" + userName, "GET", string.Empty, token); }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>重置结果JSON(如：{ "action" : "set user password",  "timestamp" : 1404802674401,  "duration" : 90})</returns>
        public string AccountResetPwd(string userName, string newPassword) { return ReqUrl(easeMobUrl + "users/" + userName + "/password", "PUT", "{\"newpassword\" : \"" + newPassword + "\"}", token); }


        /// <summary>
        /// 更新用户昵称
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="nickName">昵称</param>
        /// <returns>重置结果JSON(如：{ "action" : "set user password",  "timestamp" : 1404802674401,  "duration" : 90})</returns>
        public string AccountUpdateNickName(string userName, string nickName) { return ReqUrl(easeMobUrl + "users/" + userName, "PUT", "{\"nickname\" : \"" + nickName + "\"}", token); }



        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns>成功返回会员JSON详细信息，失败直接返回：系统错误信息</returns>
        public string AccountDel(string userName) { return ReqUrl(easeMobUrl + "users/" + userName, "DELETE", string.Empty, token); }

        public string ReqUrl(string reqUrl, string method, string paramData, string token)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(reqUrl) as HttpWebRequest;
                request.Method = method.ToUpperInvariant();

                if (!string.IsNullOrEmpty(token) && token.Length > 1) { request.Headers.Add("Authorization", "Bearer " + token); }
                if (request.Method.ToString() != "GET" && !string.IsNullOrEmpty(paramData) && paramData.Length > 0)
                {
                    request.ContentType = "application/json";
                    byte[] buffer = Encoding.UTF8.GetBytes(paramData);
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }

                using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        string result = stream.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception ex) { return ex.ToString(); }
        }
    }

}
