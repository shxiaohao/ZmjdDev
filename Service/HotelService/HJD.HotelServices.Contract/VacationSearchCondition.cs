using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;
using System.Runtime.Serialization;
namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class VacationSearchCondition
    {
         [DataMember]
        [DBColumn(DefaultValue = "0")]
        public int DistrictID { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "0")]
        public List<int> Location { get; set; }

         [DataMember]
        [DBColumn(DefaultValue = "0")]
        public List<int> Zone { get; set; }

         [DataMember]
        [DBColumn]
        public VacationOrderType VacationPyOrderType { get; set; }

         [DataMember]
        [DBColumn(DefaultValue = "0")]
        public int StartIndex { get; set; }

         [DataMember]
        [DBColumn(DefaultValue = "0")]
        public int ReturnCount { get; set; }


        /// <summary>
        /// 类型
        /// </summary>
         [DataMember]
        [DBColumn(DefaultValue = "0")]
        public List<int> VacationClass { get; set; }

    }
    [Serializable]
    [DataContract]
    public enum VacationOrderType
    {
        /// <summary>
        /// 排序 按拼音 
        /// </summary>
        [EnumMember]
        Py_Up,
        [EnumMember]
        Py_Down,
    }
}
