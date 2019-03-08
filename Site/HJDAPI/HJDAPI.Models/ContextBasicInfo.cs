using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    /// <summary>
    /// 上下文基本信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ContextBasicInfo
    {
        /// <summary>
        /// 当前上下文是否APP环境
        /// </summary>
        [DataMember]
        public bool IsApp
        {
            get;set;
        }

        /// <summary>
        /// 是否在Web环境
        /// </summary>
        [DataMember]
        public bool IsWeb
        {
            get;
            set;
        }

        /// <summary>
        /// 当前环境下的userid
        /// </summary>
        [DataMember]
        public Int64 AppUserID
        {
            get;
            set;
        }

        /// <summary>
        /// 是否iOS环境
        /// </summary>
        [DataMember]
        public bool IsIOS
        {
            get;set;
        }

        /// <summary>
        /// 是否Android环境
        /// </summary>
        [DataMember]
        public bool IsAndroid
        {
            get;
            set;
        }

        /// <summary>
        /// 区别iOS或Android设备
        /// </summary>
        [DataMember]
        public string AppType
        {
            get;
            set;
        }

        /// <summary>
        /// App版本号 如3.x  4.x
        /// </summary>
        [DataMember]
        public string AppVer
        {
            get;
            set;
        }

        /// <summary>
        /// App版本打包号 如322 67
        /// </summary>
        [DataMember]
        public Int64 AppBundleVer
        {
            get;
            set;
        }

        /// <summary>
        /// 终端类型
        /// </summary>
        public enum TerminalType
        {
            www = 1,
            iOS = 2,
            Android = 3,
            WeiXin = 4,
            UNKNOWN = 100
        }

        public TerminalType GetTerminalType()
        {
            return IsWeb ? TerminalType.www : IsIOS ? TerminalType.iOS : IsAndroid ? TerminalType.Android : TerminalType.WeiXin;
        }

        /// <summary>
        /// 当前请求源的IP
        /// </summary>
        [DataMember]
        public string UserHostAddress
        {
            get;
            set;
        }

        #region app的版本比较变量

        /// <summary>
        /// 大于等于App4.4版本（下面的变量以此类推）
        /// </summary>
        [DataMember]
        public bool IsThanVer4_4
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer4_6
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer4_6_1
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer4_7
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer4_8
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_0
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_1
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_2
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_3
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_4
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_6
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_6_1
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_6_2
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_7
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer5_9
        {
            get;
            set;
        }

        [DataMember]
        public bool IsThanVer6_0
        {
            get;
            set;
        }
        [DataMember]
        public bool IsThanVer6_2
        {
            get;
            set;
        }
        [DataMember]
        public bool IsThanVer6_2_1
        {
            get;
            set;
        }
        [DataMember]
        public bool IsThanVer6_4_2
        {
            get;
            set;
        }
        #endregion
    }
}
