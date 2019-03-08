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
        /// �����Ƶ�Tags
        /// </summary>
        [DataMember]
        public List<FilterTag> HotelRelFilterTags { get; set; }

        /// <summary>
        /// �Ƶ�����ĵ���
        /// </summary>
        [DataMember]
        public List<HotelRelComment> HotelRelComments { get; set; }

        /// <summary>
        /// �Ƶ������
        /// </summary>
        [DataMember]
        public string ShortName { get; set; }

        /// <summary>
        /// �ײ�ID
        /// </summary>
        [DataMember]
        public int PackageId { get; set; }

        /// <summary>
        /// �ײͼ��
        /// </summary>
        [DataMember]
        public string PackageBrief { get; set; }

        /// <summary>
        /// �ײͱ�ǩ
        /// </summary>
        [DataMember]
        public string PackageLabel { get; set; }

        /// <summary>
        /// �ײ���ס��ʼʱ��
        /// </summary>
        [DataMember]
        public System.DateTime PackageStartDate { get; set; }

        /// <summary>
        /// �ײ���ס����ʱ��
        /// </summary>
        [DataMember]
        public System.DateTime PackageEndDate { get; set; }

        /// <summary>
        /// �ײͳ��ۿ�ʼʱ��
        /// </summary>
        [DataMember]
        public System.DateTime SaleStartTime { get; set; }

        /// <summary>
        /// �ײͳ��۽���ʱ��
        /// </summary>
        [DataMember]
        public System.DateTime SaleEndTime { get; set; }

        /// <summary>
        /// �Ƿ����ײ�
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
        /// ��������
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// �����ǳ�
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// ����ͷ��
        /// </summary>
        [DataMember]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        [DataMember]
        public long AuthorUserID { get; set; }
    }

    /// <summary>
    /// ȥ"�ظ�"ʱ��ıȽ���(ֻҪAuthorUserID��ͬ������Ϊ����ͬ��¼)
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