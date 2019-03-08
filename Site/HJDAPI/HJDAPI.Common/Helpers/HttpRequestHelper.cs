using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace HJDAPI.Common.Helpers
{
    public class HttpRequestHelper
    {
        static string EncodeType = "utf-8";// "gb2312";

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        /// <summary>
        /// 通过POST方式发送数据
        /// 
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postDataStr">Post数据</param>
        /// <param name="cookie">Cookie容器</param>
        /// <returns></returns>
        public static string Post(string url, string postDataStr, ref CookieContainer cookie, string encodeType = null)
        {
            var encoding = Encoding.GetEncoding(string.IsNullOrWhiteSpace(encodeType) ? EncodeType : encodeType);
            
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }

            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            
            var encodingStr = encoding.GetBytes(postDataStr);
            int bytesLen = encodingStr.Length;
            request.ContentLength = bytesLen;
            
            Stream myRequestStream = request.GetRequestStream();

            myRequestStream.Write(encodingStr, 0, bytesLen);
            myRequestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        
        public static string PostJson(string url, string strJson, ref CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            byte[] utf8 = Encoding.UTF8.GetBytes(strJson);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = utf8.Length;// Encoding.GetEncoding("utf-8").GetBytes(jsonStr).Length; 
            Stream myRequestStream = request.GetRequestStream();

            myRequestStream.Write(utf8, 0, utf8.Length);
            myRequestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string PostJson(string url,object obj,ref CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            byte[] utf8 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
         
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = utf8.Length;// Encoding.GetEncoding("utf-8").GetBytes(jsonStr).Length; 
            Stream myRequestStream = request.GetRequestStream();
            
            myRequestStream.Write(utf8,0,utf8.Length);
            myRequestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 通过GET方式发送数据
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postDataStr">GET数据</param>
        /// <param name="cookie">GET容器</param>
        /// <returns></returns>
        public static string Get(string url, string postDataStr, ref CookieContainer cookie)
        {
            url = url + (postDataStr == "" ? "" : "?") + postDataStr;
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            
            
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        
        public static string PostXml(string url, string strPost)
        {
            string result = "";
            byte[] utf8 = Encoding.UTF8.GetBytes(strPost);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml";//提交xml
            request.ContentLength = utf8.Length;

            Stream myRequestStream = request.GetRequestStream();

            myRequestStream.Write(utf8, 0, utf8.Length);
            myRequestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            result = myStreamReader.ReadToEnd();
            
            myStreamReader.Close();
            myResponseStream.Close();

            return result;
        }
        
        /// <summary>
        /// 获取当前
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            return "";
        }
    }
}