using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJD.Framework.Entity;
namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 条件
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class DownLoadCtripCommentEntity
    {
   
        [DataMember]
        [DBColumn(DefaultValue = "2012-08-30")]
        public DateTime StartTime { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "0")]
        public int Pagesum { get; set; }

        [DataMember]
        [DBColumn(DefaultValue = "0")]
        public int Pagecum { get; set; }

    }  
}


