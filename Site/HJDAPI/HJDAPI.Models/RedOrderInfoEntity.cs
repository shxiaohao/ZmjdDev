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
    public class RedOrderInfoEntity
    {
        public RedOrderInfoEntity()
        {
            RedShare = new ShareModel();
        }

        [DataMember]
        public string SmallPicture { get; set; }

        [DataMember]
        public string BigPicture { get; set; }

        [DataMember]
        public string ResultTitle { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ButtonText { get; set; }

        [DataMember]
        public ShareModel RedShare { get; set; }

        [DataMember]
        public int RedState { get; set; }
    }


    [Serializable]
    [DataContract]
    public class RedShareInfoEntity
    {
        [DataMember]
        public string ShareTitle { get; set; }

        [DataMember]
        public string ShareDescription { get; set; }

        [DataMember]
        public string ShareSmallPicture { get; set; }

        [DataMember]
        public string ShareUrl { get; set; }

        [DataMember]
        public string ShareBackUrl { get; set; }

    }
}
