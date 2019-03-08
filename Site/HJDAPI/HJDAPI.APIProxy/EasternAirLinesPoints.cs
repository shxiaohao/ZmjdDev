using HJD.HotelManagementCenter.Domain;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class EasternAirLinesPoints : BaseProxy
    {
        public int GetEasternAirLinesPoints(string userID)
        {
            string url = APISiteUrl + "api/EasternAirLines/GetEasternAirLinesPoints";
            string postDataStr = string.Format("userID={0}"
              , userID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public EasternAirLinesCardEntity GetPointsPreview(string userID)
        {
            string url = APISiteUrl + "api/EasternAirLines/GetPointsPreview";
            string postDataStr = string.Format("userID={0}"
              , userID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<EasternAirLinesCardEntity>(json);
        }
        public List<EasternAirLinesRecordEntity> GetPointsList(string userID)
        {
            string url = APISiteUrl + "api/EasternAirLines/GetPointsList";
            string postDataStr = string.Format("userID={0}"
              , userID);

            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<EasternAirLinesRecordEntity>>(json);
        }

        public BaseResponse AddEACard(EasternAirLinesCardParam param)
        {
            string url = APISiteUrl + "api/EasternAirLines/AddEACard";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, param, ref cc);
            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }

        public BaseResponse ChangeEACard(string ID)
        {
            string url = APISiteUrl + "api/EasternAirLines/ChangeEACard";
            string postDataStr = string.Format("ID={0}"
            , ID);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }

        public BaseResponse CheckCardNO(string cardNO)
        {
            string url = APISiteUrl + "api/EasternAirLines/CheckCardNO";
            string postDataStr = string.Format("cardNO={0}"
            , cardNO);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<BaseResponse>(json);
        }
    }
}
