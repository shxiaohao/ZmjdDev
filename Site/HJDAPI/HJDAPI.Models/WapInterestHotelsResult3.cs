using System.Collections.Generic;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [DataContract]
    public class WapInterestHotelsResult3
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public int InterestID { get; set; }

        [DataMember]
        public int InterestType { get; set; }

        [DataMember]
        public string InterestName { get; set; }


        [DataMember]
        public IEnumerable<ListHotelItem3> Result { get; set; }

        [DataMember]
        public HotelListMenu filters { get; set; }
    }

    [DataContract]
    public class SearchHotelListResult
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int Start { get; set; }
        
        [DataMember]
        public List<ListHotelItem3> Result { get; set; }

        [DataMember]
        public List<ListHotelItemV43> Result20 { get; set; }

        [DataMember]
        public List<HotelListSortOption> SortOptions { get; set; }

        [DataMember]
        public HotelListMenu FilterMenus { get; set; }

        [DataMember]
        public FilterBlockModel FilterBlocks { get; set; }

        [DataMember]
        public CommentShareModel ShareModel { get; set; }
    }

    [DataContract]
    public class ListHotelItemV43 : ListHotelItem3
    {
        /// <summary>
        /// 关联酒店Tags
        /// </summary>
        [DataMember]
        public List<FilterTag> HotelRelFilterTags { get; set; }

        /// <summary>
        /// 酒店关联的点评
        /// </summary>
        [DataMember]
        public List<HotelRelComment> HotelRelComments { get; set; }

        /// <summary>
        /// 酒店短命称
        /// </summary>
        [DataMember]
        public string ShortName { get; set; }

        /// <summary>
        /// 套餐ID
        /// </summary>
        [DataMember]
        public int PackageId { get; set; }

        /// <summary>
        /// 套餐简介
        /// </summary>
        [DataMember]
        public string PackageBrief { get; set; }

        /// <summary>
        /// 套餐标签
        /// </summary>
        [DataMember]
        public string PackageLabel { get; set; }

        /// <summary>
        /// 套餐入住开始时间
        /// </summary>
        [DataMember]
        public System.DateTime PackageStartDate { get; set; }

        /// <summary>
        /// 套餐入住结束时间
        /// </summary>
        [DataMember]
        public System.DateTime PackageEndDate { get; set; }

        /// <summary>
        /// 套餐出售开始时间
        /// </summary>
        [DataMember]
        public System.DateTime SaleStartTime { get; set; }

        /// <summary>
        /// 套餐出售结束时间
        /// </summary>
        [DataMember]
        public System.DateTime SaleEndTime { get; set; }

        /// <summary>
        /// 是否是套餐
        /// </summary>
        [DataMember]
        public bool IsInterest { get; set; }
    }

    [DataContract]
    public class HotelListSortOption
    {
        [DataMember]
        public int Sort { get; set; }
        [DataMember]
        public string SortName { get; set; }
    }

    [DataContract]
    public class HotelRelComment
    {
        public HotelRelComment()
        {
            Title = "";
            Author = "";
            AvatarUrl = "";
        }

        /// <summary>
        /// 点评标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 作者昵称
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// 作者头像
        /// </summary>
        [DataMember]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 作者ID
        /// </summary>
        [DataMember]
        public long AuthorUserID { get; set; }
    }

    /// <summary>
    /// 去"重复"时候的比较器(只要AuthorUserID相同，即认为是相同记录)
    /// </summary>
    public class HotelRelCommentNoComparer : IEqualityComparer<HotelRelComment>
    {
        public bool Equals(HotelRelComment p1, HotelRelComment p2)
        {
            if (p1 == null)
                return p2 == null;
            return p1.AuthorUserID == p2.AuthorUserID;
        }

        public int GetHashCode(HotelRelComment p)
        {
            if (p == null)
                return 0;
            return p.AuthorUserID.GetHashCode();
        }
    }
}