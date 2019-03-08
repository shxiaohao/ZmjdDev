using HJD.HotelManagementCenter.Domain;
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
    public class GetChannelMBankAccountListResult
    {
        [DataMember]
        public List<PayBankAccountLibEntity> list { get; set; }
    }
}
