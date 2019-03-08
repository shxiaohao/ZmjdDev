using CommLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Implement.Helper
{
    public class DownloadHelper
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">下载文件地址</param>
        /// <param name="Filename">下载后另存为（全路径）</param>
        public static bool DownloadFile(string URL, string filename)
        {
            try
            {
                HttpWebRequest Myrq = (HttpWebRequest)HttpWebRequest.Create(URL);
                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
                myrp.Close();
                Myrq.Abort();
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public static string ReadUrlHtml(string URL)
        {
            string pageHtml = "";

            HttpWebRequest Myrq = (HttpWebRequest)HttpWebRequest.Create(URL);
            HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
            Stream st = myrp.GetResponseStream();
            byte[] by = new byte[1024];
            int osize = st.Read(by, 0, (int)by.Length);
            pageHtml = Encoding.Default.GetString(by);

            return pageHtml;
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="readUrl"></param>
        /// <returns></returns>
        public static string ReadInfo(string readUrl)
        {
            string str;
            StreamReader sr = new StreamReader(readUrl, Encoding.Default);
            str = sr.ReadToEnd().ToString();
            sr.Close();
            return str;
        }

        /// <summary>
        /// 写内容到指定路径的文件中（覆盖）
        /// </summary>
        /// <param name="writeUrl">保存路径（包含文件名）</param>
        /// <param name="value">写入内容</param>
        /// <returns></returns>
        public static bool WriteInfo(string writeUrl, string value)
        {
            try
            {
                StreamWriter sw = new StreamWriter(writeUrl, false, Encoding.Default);
                sw.Write(value);
                sw.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return true;
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

        public static string PostJson(string url, object obj, ref CookieContainer cookie)
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

        /// <summary>
        /// 通过GET方式发送数据
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postDataStr">GET数据</param>
        /// <param name="cookie">GET容器</param>
        /// <returns></returns>
        public static string Get(string url, string postDataStr, ref CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
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

        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">参数字符串</param>
        /// <returns>请求返回值</returns>
        public static string HttpGet(string url, string queryString)
        {
            string responseData = null;

            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + queryString.Trim(' ', '?', '&');
            }

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            string proxyAddress = ConfigurationManager.AppSettings["proxyAddress"];
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxyAddress);
                webRequest.Proxy = myProxy;
            }
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;
            webRequest.KeepAlive = false;

            //try
            //{
            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        /// <summary>
        /// 获取返回结果http get请求
        /// </summary>
        /// <param name="webRequest">webRequest对象</param>
        /// <returns>请求返回值</returns>
        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            try
            {
                string responseData = String.Empty;
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    StreamReader responseReader = null;

                    responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                    responseData = responseReader.ReadToEnd();

                    webRequest.GetResponse().GetResponseStream().Close();
                    responseReader.Close();
                    responseReader = null;
                }
                return responseData;
            }
            catch
            {
                return "";
            }
        }
    }
}
