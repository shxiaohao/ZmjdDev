using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelSEOKeyword
    {
        [DataMemberAttribute()]
        public string Name { get; set; }

        [DataMemberAttribute()]
        public string URL { get; set; }

        [DataMemberAttribute()]
        public string Title { get; set; }

        [DataMemberAttribute()]
        public int Num { get; set; }

        [DataMemberAttribute()]
        public int Id { get; set; }
    }
}
