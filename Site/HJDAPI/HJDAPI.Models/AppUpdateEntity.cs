using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class AppUpdateEntity
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string MandatoryVersion { get; set; }

        [DataMember]
        public string ProposedVersion { get; set; }

        [DataMember]
        public int Days { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string URL { get; set; }


        [DataMember]
        public string Tips { get; set; }

    }
}
