using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{ 
    [Serializable]
    [DataContract]
    public class AppErrorParam : BaseParam
    { 
        /// <summary>
        /// 建议内容
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; } 
    } 
}
