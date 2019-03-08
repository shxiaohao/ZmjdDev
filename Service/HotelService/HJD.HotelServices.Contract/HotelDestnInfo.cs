using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class HotelDestnInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelID { get; set; }
        /// <summary>
        /// 目的地ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DistrictID { get; set; }
        /// <summary>
        /// 目的地名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string DistrictName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float lat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public float lon { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool InChina { get; set; }
    }

    //[System.SerializableAttribute()]
    //[System.Runtime.Serialization.DataContractAttribute()]
    //[HJD.Framework.Entity.DefaultColumnAttribute()]
    //public class HotelDestInfoList
    //{
    //    public HotelDestInfoList()
    //    {
    //        dicHotelDestInfoList = new List<dicHotelDestInfoList>();
    //    }
    //    [System.Runtime.Serialization.DataMemberAttribute()]
    //    [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
    //    List<dicHotelDestInfoList> dicHotelDestInfoList { get; set; }
    //}
    //[System.SerializableAttribute()]
    //[System.Runtime.Serialization.DataContractAttribute()]
    //[HJD.Framework.Entity.DefaultColumnAttribute()]
    //public class dicHotelDestInfoList
    //{
    //    public dicHotelDestInfoList()
    //    {
    //        Description = "";
    //        HotelDestInfoList = new HotelDestnInfo();
    //    }
    //    [System.Runtime.Serialization.DataMemberAttribute()]
    //    [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
    //    public string Description { get; set; }

    //    [System.Runtime.Serialization.DataMemberAttribute()]
    //    [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
    //    public HotelDestnInfo HotelDestInfoList { get; set; }
    //}
}
