using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    public class CommDictEntity
    {
        [DataMemberAttribute()]
        [DBColumnAttribute]
        public int DicKey { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string DicValue { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute]
        public string Descript { get; set; }
    }
}
