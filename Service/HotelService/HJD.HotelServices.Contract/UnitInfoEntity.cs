using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
using System.Collections.Generic;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店信息
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class UnitInfoEntity
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string name { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string ename { get; set; }





        /// <summary>
        /// 地址
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string Address { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "", IsOptional = true)]
        public string LocationName { get; set; }

        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Location { get; set; }



        [DataMemberAttribute()]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int DistrictID { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "", IsOptional = true)]
        public string DistrictName { get; set; }



        [DataMember]
        [DBColumn(DefaultValue = "0", IsOptional = true)]
        public int Zone { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "", IsOptional = true)]
        public string ZoneName { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "", IsOptional = true)]
        public string description { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "", IsOptional = true)]
        public string sleeps { get; set; }


        [DataMember]
        [DBColumn(DefaultValue = "", IsOptional = true)]
        public string livingroom { get; set; }


        [DataMember]
        [DBColumn(DefaultValue = "", IsOptional = true)]
        public string bathroom { get; set; }


        [DataMember]
        [DBColumn(DefaultValue = "", IsOptional = true)]
        public string bedroom { get; set; }


        [DataMember]
        [DBColumn(DefaultValue = "0", IsOptional = true)]
        public int bedcount { get; set; }


  

      
        
    }
}



