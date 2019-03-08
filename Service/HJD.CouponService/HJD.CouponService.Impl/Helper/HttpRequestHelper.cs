using System.IO;
using System.Net;
using System.Text;
using System;

namespace HJD.CouponService.Impl
{
    public class HttpRequestHelper
    {
        static string EncodeType = "utf-8";// "gb2312";

        /// <summary>
        /// ͨ��POST��ʽ��������
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postDataStr">Post����</param>
        /// <param name="cookie">Cookie����</param>
        /// <returns></returns>
        public static string Post(string url, string postDataStr, ref CookieContainer cookie)
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

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.GetEncoding(EncodeType).GetBytes(postDataStr).Length; 
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding(EncodeType));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(EncodeType));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        
        /// <summary>
        /// ͨ��GET��ʽ��������
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postDataStr">GET����</param>
        /// <param name="cookie">GET����</param>
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
        
        public static string PostXml(string url, string strPost)
        {
            string result = "";
            byte[] utf8 = Encoding.UTF8.GetBytes(strPost);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml";//�ύxml
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
    }
}