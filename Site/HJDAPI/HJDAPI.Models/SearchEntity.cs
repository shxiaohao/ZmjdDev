using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class SearchEntity
    {
        public SearchEntity()
        {
            SearchSort = new List<SearchItemEntity>();
            DestinationList = new List<SearchItemEntity>();
        }
        /// <summary>
        /// 排序条件
        /// </summary>
        [DataMember]
        public List<SearchItemEntity> SearchSort { get; set; }

        /// <summary>
        /// 目的地列表
        /// </summary>
        [DataMember]
        public List<SearchItemEntity> DestinationList { get; set; }
    }

    [DataContract]
    public class SearchItemEntity
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Values { get; set; }
    }
    /// <summary>
    ///省市 集合
    /// </summary>
    [DataContract]
    public class GroupDistrictEntity
    {
        public GroupDistrictEntity()
        {
            DestinationList = new List<SearchItemEntity>();
        }
        /// <summary>
        /// 省名称
        /// </summary>
        [DataMember]
        public string RootName { get; set; }
        /// <summary>
        /// 目的地列表
        /// </summary>
        [DataMember]
        public List<SearchItemEntity> DestinationList { get; set; }
    }
}
