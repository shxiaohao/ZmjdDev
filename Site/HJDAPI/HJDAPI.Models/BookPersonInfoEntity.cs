using ProductService.Contracts.Entity;
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
    public  class BookPersonInfoEntity
    {
        public BookPersonInfoEntity()
        {
            //BookUserDate = new BookUserDateInfoEntity();
            //BookUserList = new List<BookUserInfoEntity>();
            //BookTempDate = "";
            TemplateData = new TemplateDataEntity();
        }
        //[DataMember]
        //public List<BookUserInfoEntity> BookUserList { get; set; }


        //[DataMember]
        //public BookUserDateInfoEntity BookUserDate { get; set; }

        //[DataMember]
        //public string BookTempDate { get; set; }

        [DataMember]
        public TemplateDataEntity TemplateData { get; set; }

        [DataMember]
        public long UserID { get; set; }


        [DataMember]
        public List<int> TravelId { get; set; }

        [DataMember]
        public List<int> ExchangCouponIds { get; set; }

        [DataMember]
        public int BookDateId { get; set; }

        [DataMember]
        public int BookDetailId { get; set; }

        [DataMember]
        public int skuid { get; set; }

    }
}
