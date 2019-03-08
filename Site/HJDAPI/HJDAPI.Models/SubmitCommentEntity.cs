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
    public class SubmitCommentEntity:BaseParam
    {
        public SubmitCommentEntity()
        {
            TagIDs = new List<int>();
            addInfos = new List<AddationalInfo>();
            addScores = new List<AdditionalScore>();
            appVer = "";
            HotelName = "";
            photoInfos = new List<CommentPhotoInsertEntity>();
        }

        /// <summary>
        /// 点评ID
        /// </summary>
        [DataMember]
        public int CommentID { get; set; }
        [DataMember]
        public int HotelID { get; set; }
        [DataMember]
        public string HotelName { get; set; }
        [DataMember]
        public long OrderID { get; set; }
        [DataMember]
        public bool recommend { get; set; }
        [DataMember]
        public float score { get; set; }
        [DataMember]
        public string appVer { get; set; }
        [DataMember]
        public long UserID { get; set; }
        [DataMember]
        public List<int> TagIDs { get; set; }
        [DataMember]
        public List<AddationalInfo> addInfos { get; set; }
        /// <summary>
        /// 更多打分项
        /// </summary>
        [DataMember]
        public List<AdditionalScore> addScores { get; set; }
        [DataMember]
        public List<CommentPhotoInsertEntity> photoInfos { get; set; }
    }

    [Serializable]
    [DataContract]
    public class AddCommentContentEntity : BaseParam
    {
        public AddCommentContentEntity()
        {
            addInfos = new List<AddationalInfo>();
        }

        [DataMember]
        public int CommentID { get; set; }

        [DataMember]
        public List<AddationalInfo> addInfos { get; set; }
    }
    
    [DataContract]
    public class AddationalInfo
    {
        [DataMember]
        public int CategoryID { get; set; }
        [DataMember]
        public string AddationalComment { get; set; }
    }

    [DataContract]
    public class AddHotelVerifyResult
    {
        [DataMember]
        public bool CanAdd { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}