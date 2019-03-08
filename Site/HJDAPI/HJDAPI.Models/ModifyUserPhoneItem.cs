using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
      [DataContract]
    public class ModifyUserPhoneItem:BaseParam
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
        public string password { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string newPhone { get; set; }

          
        [DataMember]
        public string confirmCode { get; set; }

      
    } 
}
