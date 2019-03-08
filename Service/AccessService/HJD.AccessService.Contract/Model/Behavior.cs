using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model
{
    [Serializable]
    [DataContract]
    public class Behavior
    {
        [DataMemberAttribute()]
        public Guid ID = Guid.NewGuid();

        [DataMemberAttribute()]
        public string Code { set; get; }

        [DataMemberAttribute()]
        public string Value { set; get; }

        public string Page { set; get; }

        [DataMemberAttribute()]
        public string Event { set; get; }

        /// <summary>
        /// zmjd用户ID
        /// </summary>
        [DataMemberAttribute()]
        public int UserId { set; get; }

        /// <summary>
        /// zmjd用户手机号
        /// </summary>
        [DataMemberAttribute()]
        public string Phone { set; get; }

        /// <summary>
        /// 一般存储SessionId
        /// </summary>
        [DataMemberAttribute()]
        public string AppKey { set; get; }

        /// <summary>
        /// 终端类型（www/wap/weixin/android/ios等）
        /// </summary>
        [DataMemberAttribute()]
        public string ClientType { set; get; }

        /// <summary>
        /// App版本号 如3.x 4.x (一般APP内访问才会有；如果是访问纯网页，应该没有版本号)
        /// </summary>
        [DataMemberAttribute()]
        public string AppVer { set; get; }

        /// <summary>
        /// 请求源的IP地址
        /// </summary>
        [DataMemberAttribute()]
        public string IP { set; get; }

        /// <summary>
        /// 信息记录层（www层/api层/service层/其他（如抓包））
        /// </summary>
        [DataMemberAttribute()]
        public string RecordLayer { set; get; }

        /// <summary>
        /// 信息记录时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime RecordTime = DateTime.Now;
    }

    #region 注释代码

    //public class Behavior
    //{
    //    public Guid ID;
        
    //    public string PageCode;
    //    public string PageValue;

    //    public string EventCode;
    //    public string EventValue;

    //    public string TargetCode;
    //    public string TargetValue;

    //    public string AppKey;
    //    public string ClientType;

    //    public DateTime RecordTime;
    //}

    #endregion
}
