using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace HJD.Search.CommonLibrary.Helper
{
    public static class ExtensionHelper
    {
        public static string GetAttributeValue(this XElement xml, string code)
        {
            if (xml == null) return "";

            var node = xml.Attribute(code);

            return node == null ? "" : node.Value ?? "";
        }

        public static string GetElementValue(this XElement xml, string code)
        {
            if (xml == null) return "";

            var node = xml.Element(code);

            return node == null ? "" : node.Value ?? "";
        }

        public static string ToObjectString(this object sorceString)
        {
            if (sorceString == null)
                return string.Empty;
            return sorceString.ToString();
        }

        public static string SubStringForLength(this string stringValue, int length)
        {
            if (stringValue.Length > length)
                return stringValue.Substring(0, length);
            return stringValue;
        }

        public static int ToIntString(this object sorceString)
        {
            int value = 0;
            if (sorceString == null)
                return value;
            int.TryParse(sorceString.ToString(), out value);

            return value;
        }

        public static string GetRequerstUser(this IPrincipal User)
        {
            if (User == null)
                return string.Empty;
            if (User.Identity == null)
                return string.Empty;

            return User.Identity.Name;
        }

        public static int ToIntString(this bool value)
        {
            return value == true ? 1 : 0;
        }

        private static readonly string timeFormat = "yyyy-MM-dd";

        public static string GetFormatDateTimeOrNull(this DateTime? time)
        {
            if (time == null) return "";
            return time.Value.ToString(timeFormat);
        }

        public static string GetFormatDateTime(this DateTime time)
        {
            return time.ToString(timeFormat);
        }

        public static string GetChineseFormatDateTime(this DateTime time)
        {
            return time.ToString("yyyy年MM月dd日");
        }


        public static string GetChineseFormatDateTimeOrNull(this DateTime? time)
        {
            if (time == null) return "";
            return time.Value.ToString("yyyy年MM月dd日");
        }



        /// <summary>
        /// 把字符串转化整形,无异常，如果转化失败，则返回0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            int i = 0;

            int.TryParse(str, out i);

            return i;
        }

        /// <summary>
        /// 是否为数字
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static bool IsNumberic(this string stringValue)
        {
            Regex rex = new Regex(@"^\d+$");
            return rex.IsMatch(stringValue);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/521146/c-sharp-split-string-but-keep-split-chars-separators?rq=1
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delims"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitAndKeep(this string s, char[] delims)
        {
            int start = 0;
            int index = 0;

            while ((index = s.IndexOfAny(delims, start)) != -1)
            {
                index++;
                index = Interlocked.Exchange(ref start, index);

                yield return s.Substring(index, start - index - 1);
                yield return s.Substring(start - 1, 1);
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T">当前数据类型</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="pageIndex">要返回的结果页的索引。使用 0 来指示第一页,pageIndex 是从零开始的。</param>
        /// <param name="pageSize">要返回的结果页的大小。</param>
        /// <param name="total">记录的总数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">source不可为空</exception>
        /// <exception cref="ArgumentException">pageIndex 小于零。- 或 -pageSize 小于 1。</exception>
        public static List<T> GetPageList<T>(this IList<T> source, int pageIndex, int pageSize, out int total)
        {
            if (source == null) throw new ArgumentNullException("source不可为空");

            if (pageIndex < 0 || pageSize < 1) throw new ArgumentException("pageIndex 小于零。- 或 -pageSize 小于 1");

            total = source.Count();

            return source.Skip(pageSize * pageIndex).Take(pageSize).ToList();
        }

        public static string BindXsltForXml(this XElement xel, string xsltPath)
        {
            // Helper.WriteLogs(DateTime.Now, DateTime.Now.Subtract(Test.StartTime).TotalMilliseconds.ToString(), "绑定XML与XSLT");
            if (xel == null || !File.Exists(xsltPath)) return string.Empty;
            string html = string.Empty;
            XslCompiledTransform xsltTransform = new XslCompiledTransform();
            XsltSettings xsltSettings = new XsltSettings();
            xsltSettings.EnableScript = true;
            xsltTransform.Load(xsltPath, xsltSettings, null);

            Stream oStream = new MemoryStream();
            Encoding outputEncoding = Encoding.UTF8;
            using (XmlReader xmlreader = xel.CreateReader())
            {
                TextWriter tr = new StreamWriter(oStream, outputEncoding);
                xsltTransform.Transform(xmlreader, null, tr);

                byte[] arrBuffer = new byte[oStream.Length];

                oStream.Position = 0;

                int iLastLength = oStream.Read(arrBuffer, 0, (int)oStream.Length);

                if (iLastLength > 0)
                {
                    html = outputEncoding.GetString(arrBuffer).Substring(1);
                }

            }

            // Helper.WriteLogs(DateTime.Now, DateTime.Now.Subtract(Test.StartTime).TotalMilliseconds.ToString(), "绑定XML与XSLT结束");
            return html;

        }

        /// <summary>
        /// 转化成特定类型，该类型必须为结构类型，如果转化失败则返回T的默认值 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToStruct<T>(this string str) where T : struct
        {
            var defaultValue = default(T);

            if (string.IsNullOrEmpty(str)) return defaultValue;

            if (typeof(T).IsEnum)
            {
                Enum.TryParse(str, out defaultValue);

                return defaultValue;

            }
            else
            {
                var method = typeof(T).GetMethods().Where(x => x.Name == "TryParse" && x.GetParameters().Count() == 2).FirstOrDefault();

                if (method == null) return defaultValue;

                var objects = new object[] { str, defaultValue };

                method.Invoke(null, objects);

                return (T)objects[1];
            }
        }

    }
}
