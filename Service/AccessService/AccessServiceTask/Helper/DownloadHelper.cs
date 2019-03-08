using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Helper
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
                sw.WriteLine(value);
                sw.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return true;
        }
    }
}
