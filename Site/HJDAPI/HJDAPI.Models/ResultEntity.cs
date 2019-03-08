using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class ResultEntity
    {
        [DataMember]
        public int Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    /// <summary>
    /// page3 page4 page5 page6 page7 page8 page9 page10等弥补4.1版本 iOS判断推荐品鉴师数量翻页错误导致的问题
    /// </summary>
    [DataContract]
    public class InspectorListResult
    {
        [DataMember]
        public List<RecommendedInspectorModel> Items { get; set; }

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int page3 { get; set; }

        [DataMember]
        public int page4 { get; set; }

        [DataMember]
        public int page5 { get; set; }

        [DataMember]
        public int page6 { get; set; }

        [DataMember]
        public int page7 { get; set; }

        [DataMember]
        public int page8 { get; set; }

        [DataMember]
        public int page9 { get; set; }

        [DataMember]
        public int page10 { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class UserTagOption
    {
        [DataMember]
        public List<HJD.AccountServices.Entity.UserTagRelEntity> Tags { get; set; }

        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public int TypeId { get; set; }

        [DataMember]
        public int MaxCount { get; set; }

        [DataMember]
        public int MinCount { get; set; }
    }
}