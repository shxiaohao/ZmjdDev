using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class CommentPhotoInsertEntity:BaseParam
    {
        [DataMember]
        public int AppID { get; set; }
        [DataMember]
        public int CommentID { get; set; }
        [DataMember]
        public string PhotoSURL { get; set; }
        [DataMember]
        public string PhotoSecret { get; set; }
        [DataMember]
        public string PhotoType { get; set; }
        [DataMember]
        public int PhotoSize { get; set; }
        [DataMember]
        public int PhotoWidth { get; set; }
        [DataMember]
        public int PhotoHeight { get; set; }
        [DataMember]
        public int TagBlockCategory { get; set; }
        /// <summary>
        /// 一张图配的一句话 可以没有
        /// </summary>
        [DataMember]
        public string PicBrief { get; set; }
        /// <summary>
        /// 一张图配的一句话 在整个点评中的顺序 从1开始
        /// </summary>
        [DataMember]
        public Int32 SequenceNo { get; set; }
        /// <summary>
        /// 提交正常点评还是补充点评的照片?
        /// </summary>
        [DataMember]
        public bool IsAdditionalComment { get; set; }
        /// <summary>
        /// appVer
        /// </summary>
        [DataMember]
        public string AppVer { get; set; }
    }
}