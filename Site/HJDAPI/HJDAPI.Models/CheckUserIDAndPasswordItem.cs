using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
       [DataContract]
    public class CheckUserIDAndPasswordItem: BaseParam
    {
      [DataMember]
        public long userid { get; set; }

        [DataMember]
        public string password { get; set; }
    }
}
