using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class UserCanWriteCommentResult
    {
        public bool canWrite { get; set; }
        public long orderID { get; set; }
        public string msg { get; set; }
    }

    /// <summary>
    /// App 4.0 版本添加
    /// </summary>
    public class CanWriteCommentResult
    {
        public bool canWrite { get; set; }
        public long orderID { get; set; }
        public string Message { get; set; }
        //兼容android4.2版本 bug
        public string msg { get; set; }
    }
}
