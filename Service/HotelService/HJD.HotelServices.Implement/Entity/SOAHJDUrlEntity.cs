using System;
using HJD.Framework.Entity;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace HJD.HotelServices.Entity
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class SOAHJDUrlEntity
    {
        int districtid = 0;

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0")]
        public int Districtid
        {
            get { return districtid; }
            set { districtid = value; }
        }

        string name = string.Empty;

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string eName = string.Empty;

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "")]
        public string EName
        {
            get { return eName; }
            set { eName = value; }
        }
    }
}
