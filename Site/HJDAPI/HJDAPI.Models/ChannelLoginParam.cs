using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class ChannelLoginParam:BaseParam
    {
        #region 字段
        private Int32 ctype = 1;
        #endregion

        [DataMember]
        public string OperatorName { get; set; }
        [DataMember]
        public string PassWord { get; set; }

        [DataMember]
        public string ConfirmCode { get; set; }
        [DataMember]
        public string PhoneNum { get; set; }

        [DataMember]
        public int CType
        {
            get
            {
                return this.ctype;
            }
            set
            {
                this.ctype = value;
            }
        }
    }
}
