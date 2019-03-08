using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HJD.WeixinService.Implement.Helper
{
    public static class HttpHelper
    {
        /// <summary>
        /// 设置访问代理
        /// </summary>
        /// <param name="webRequest"></param>
        public static void SetProxy(WebRequest webRequest)
        {
            string proxyAddress = ConfigurationManager.AppSettings["proxyAddress"];
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxyAddress);
                webRequest.Proxy = myProxy;
            }
        }

        public static string Get(string url, string encoding, Dictionary<string, object> headers = null)
        {

            string ret = null;
            // while (ret == null)
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                if (headers != null && headers.Count > 0)
                    headers.ToList().ForEach(a => { webRequest.Headers[a.Key] = a.Value.ToString(); });
                ret = Download(webRequest, encoding);
            }
            return ret;
        }

        private static string Download(HttpWebRequest webRequest, string encoding)
        {
            webRequest.AllowAutoRedirect = false;
            try
            {
                webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.22 (KHTML, like Gecko) Chrome/25.0.1364.97 Safari/537.22";
                webRequest.CookieContainer = new CookieContainer();
                if (cookieDic.ContainsKey(cookieKey))
                {

                    cookieDic[cookieKey].ToList().ForEach(
                        cookie =>
                        {
                            if (cookie != null)
                                webRequest.CookieContainer.Add(webRequest.RequestUri, cookie);
                        });
                    //webRequest.CookieContainer.Add(webRequest.RequestUri, cookieDic[cookieKey]);
                    //string cookie = null;
                    //for(int i = 0 ; i < cookieDic[cookieKey].Count; i++)
                    //{
                    //    cookie += cookieDic[cookieKey][i].ToString() + ";";
                    //}
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    webResponse.Cookies = webRequest.CookieContainer.GetCookies(webRequest.RequestUri);
                    Cookie[] tmp = new Cookie[100];
                    webResponse.Cookies.CopyTo(tmp, 0);
                    cookieDic.AddOrUpdate(cookieKey, tmp, (a, b) => b = tmp);
                    using (StreamReader Reader = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    {
                        string s = Reader.ReadToEnd();
                        return s;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.ToString().Contains("远程服务器返回错误: (404) 未找到。"))
                    return "404";
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2000);
                return null;
            }
            catch (Exception ex)
            {
                Thread.Sleep(2000);
                return null;
            }
        }
        public static void ClearCookieData()
        {
            if (cookieDic.ContainsKey(cookieKey))
            {
                Cookie[] tmp = null;
                cookieDic.TryRemove(cookieKey, out tmp);
            }
        }

        private static string cookieKey
        {
            get { return Thread.CurrentThread.ManagedThreadId.ToString(); }
        }

        private static ConcurrentDictionary<string, Cookie[]> cookieDic = new ConcurrentDictionary<string, Cookie[]>();

        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">参数字符串</param>
        /// <returns>请求返回值</returns>
        public static Byte[] HttpGetPicData(string picUrl)
        {
            Byte[] buf = new Byte[10240];
            WebRequest myHttpWebRequest = WebRequest.Create(new Uri(picUrl));
            HttpHelper.SetProxy(myHttpWebRequest);
            int i = 0;
            try
            {
                using (HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse())
                {
                    if (myHttpWebResponse.ContentLength > 0)
                    {
                        Byte[] photo = new Byte[myHttpWebResponse.ContentLength];

                        MemoryStream ms = new MemoryStream(photo);
                        using (Stream receiveStream = myHttpWebResponse.GetResponseStream())
                        {
                            using (BinaryReader readStream = new BinaryReader(receiveStream))
                            {
                                using (BinaryWriter res = new BinaryWriter(ms))
                                {

                                    while ((i = readStream.Read(buf, 0, 10240)) > 0)
                                    {
                                        res.Write(buf, 0, i);
                                    }

                                    res.Flush();
                                    res.Close();
                                }
                                readStream.Close();
                            }
                            receiveStream.Close();
                        }
                        myHttpWebResponse.Close();

                        return photo;
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                throw (e);
            }
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


        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="paras">请求参数列表</param>
        /// <returns>请求返回值</returns>
        public static string HttpGet(string url, List<QueryParameter> paras)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpGet(url, querystring);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
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
        /// Post请求（去除了 JsonConvert.SerializeObject(obj) ）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string PostJson2(string url, string strJson, ref CookieContainer cookie)
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

        /// <summary>
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">参数字符串</param>
        /// <returns>请求返回值</returns>
        public static string HttpPost(string url, string queryString)
        {
            StreamWriter requestWriter = null;

            string responseData = null;

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            string proxyAddress = ConfigurationManager.AppSettings["proxyAddress"];
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxyAddress);
                webRequest.Proxy = myProxy;
            }
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;
            webRequest.KeepAlive = false;

            //try
            //{
            //POST the data.
            requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(queryString);
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            requestWriter.Close();
            requestWriter = null;
            //}

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

        public static string HttpPostForJson(string url, string data)
        {
            StreamWriter requestWriter = null;
            string responseData = null;
            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "POST";
            string proxyAddress = ConfigurationManager.AppSettings["proxyAddress"];
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxyAddress);
                webRequest.Proxy = myProxy;
            }
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = byteArray.Length;

            //try
            //{
            requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(data);
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            requestWriter.Close();
            requestWriter = null;
            //}

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
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="paras">请求参数列表</param>
        /// <returns>请求返回值</returns>
        public static string HttpPost(string url, List<QueryParameter> paras)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpPost(url, querystring);
        }

        public static string HttpPostWithFileForWeiXin(string url, string fileUrl, string fileName)
        {
            Stream requestStream = null;
            string responseData = null;
            string boundary = DateTime.Now.Ticks.ToString("x");

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            string proxyAddress = ConfigurationManager.AppSettings["proxyAddress"];
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxyAddress);
                webRequest.Proxy = myProxy;
            }
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;
            webRequest.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            webRequest.Method = "POST";
            webRequest.KeepAlive = false;
            webRequest.Credentials = CredentialCache.DefaultCredentials;


            Stream memStream = new MemoryStream();

            byte[] beginBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");


            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";



            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"{2}\"\r\n\r\n";

            string contentType = "application/octetstream";

            // 写入头
            memStream.Write(beginBoundary, 0, beginBoundary.Length);

            string header = string.Format(headerTemplate, "media", fileName, contentType);

            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            memStream.Write(headerbytes, 0, headerbytes.Length);


            HttpWebRequest request = (HttpWebRequest)System.Net.HttpWebRequest.Create(fileUrl);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                Byte[] buffer = new Byte[1024];
                int current = 0;
                while ((current = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, current);
                }
            }

            // memStream.Write(boundarybytes, 0, boundarybytes.Length);
            // 写入结尾
            memStream.Write(endBoundary, 0, endBoundary.Length);

            //fileStream.Close();

            webRequest.ContentLength = memStream.Length;

            requestStream = webRequest.GetRequestStream();

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);

            requestStream.Close();
            requestStream = null;
            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
        }


        /// <summary>
        /// 同步方式发起http post请求，可以同时上传文件
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">请求参数字符串</param>
        /// <param name="files">上传文件列表</param>
        /// <returns>请求返回值</returns>
        public static string HttpPostWithFile(string url, string queryString, List<QueryParameter> files)
        {
            Stream requestStream = null;
            string responseData = null;
            string boundary = DateTime.Now.Ticks.ToString("x");

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            string proxyAddress = ConfigurationManager.AppSettings["proxyAddress"];
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxyAddress);
                webRequest.Proxy = myProxy;
            }
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;
            webRequest.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            webRequest.Method = "POST";
            webRequest.KeepAlive = false;
            webRequest.Credentials = CredentialCache.DefaultCredentials;


           Stream memStream = new MemoryStream();

            byte[] beginBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

         
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            List<QueryParameter> listParams = HttpUtil.GetQueryParameters(queryString);

            foreach (QueryParameter param in listParams)
            {
                // 写入头
                memStream.Write(beginBoundary, 0, beginBoundary.Length);

                string formitem = string.Format(formdataTemplate, param.Name, param.Value);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // memStream.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"{2}\"\r\n\r\n";

            foreach (QueryParameter param in files)
            {
                string name = param.Name;
                string filePath = param.Value;
                string file = Path.GetFileName(filePath);
                string contentType = HttpUtil.GetContentType(file);

                // 写入头
                memStream.Write(beginBoundary, 0, beginBoundary.Length);

                string header = string.Format(headerTemplate, name, filePath, contentType);

                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                memStream.Write(headerbytes, 0, headerbytes.Length);

              
                FileWebRequest request = (FileWebRequest)System.Net.FileWebRequest.Create(filePath);
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    Byte[] buffer = new Byte[1024];
                    int current = 0;
                    while ((current = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, current);
                    }
                }

                // memStream.Write(boundarybytes, 0, boundarybytes.Length);
                // 写入结尾
                memStream.Write(endBoundary, 0, endBoundary.Length);

                //fileStream.Close();
            }

            webRequest.ContentLength = memStream.Length;

            requestStream = webRequest.GetRequestStream();

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
         
            requestStream.Close();
            requestStream = null;
            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
        }


        /// <summary>
        /// 同步方式发起http post请求，可以同时上传文件
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="paras">请求参数列表</param>
        /// <param name="files">上传文件列表</param>
        /// <returns>请求返回值</returns>
        public static string HttpPostWithFile(string url, List<QueryParameter> paras, List<QueryParameter> files)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpPostWithFile(url, querystring, files);
        }
    }

}

