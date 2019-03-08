using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
     [DataContract]
    public class ModifyPasswordItem: BaseParam
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
         public long userid { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string oldpassword { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string newpassword { get; set; }

          
        [DataMember]
        public string updateIP { get; set; }

      
    } 
}
