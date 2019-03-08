using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccessService.Contract.Model.Dialog;

namespace HJD.AccessService.Contract
{
    [ServiceContract(Namespace = "http://www.zmjiudian.com/")]
    public interface IAccessService
    {

        #region MagiCall 相关接口
         

        [OperationContract]
        List<HotelSearchResult> FillHotelsInfo(List<HotelSearchResult> Hotels, string checkIn = "", string checkOut = "", int minPrice = -1, int maxPrice = -1);


        [OperationContract]
        List<QaParticipleEntity> GenQuestionWords(string keywords, string newWords = "", string userWordOptions = ""); 

     

       [OperationContract]
       List<UserWordOptionItem>  ParseInput( string  Text ,long  ItemID   );

        #endregion

        [OperationContract]
        void RecordBehavior(Behavior behavior);

        [OperationContract]
        void RecordBehaviorQueue(List<Behavior> behavior);

        [OperationContract]
        BehaviorField GetBehaviorProfile(string strFieldName);

        #region 搜索相关处理

        [OperationContract]
        List<HotelSearchResult> SearchHotel(string keywords, int limitCount);

        [OperationContract]
        List<SearchResult> StrictSearch(string keywords, int hotelLimitCount);

        #endregion

        #region Qa 搜索相关处理

        [OperationContract]
        QaSearchResult QaSearchHotel(string keywords, int limitCount, string checkIn = "", string checkOut = "", int minPrice = -1, int maxPrice = -1, string newWords = "", string userWordOptions = "");

        [OperationContract]
        int SubFeedback(QaFeedback fb);

        [OperationContract]
        int RecordSearchBehavior(QaSearchBehavior qsb);

        #endregion

        #region 索引相关处理

        [OperationContract]
        void AddIndexDocument(string id, SearchType type);

        [OperationContract]
        void RemoveIndexDocument(string id, SearchType type);

        #endregion

        #region HotelPriceSlot

        [OperationContract]
        void AddPriceSlot(int hotelid);

        #endregion

        #region 注释代码

        //[OperationContract]
        //void RecordBehaviorIndexSearchHotel(BehaviorIndexSearchHotel behaviorParams);

        //[OperationContract]
        //void RecordBehaviorZoneSearchHotel(BehaviorZoneSearchHotel behaviorParams);

        //[OperationContract]
        //void RecordBehaviorHotelSearchHotel(BehaviorHotelSearchHotel behaviorParams);

        //[OperationContract]
        //void RecordBehaviorPackageSearchHotel(BehaviorPackageSearchHotel behaviorParams);

        //[OperationContract]
        //void RecordBehaviorBookSearchHotel(BehaviorBookSearchHotel behaviorParams);

        #endregion
    }
}
