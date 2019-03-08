using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HJD.HotelServices.Contracts;
using HJD.HotelServices.Implement.Entity;

namespace HJD.HotelServices
{

    public partial class HotelService : IHotelService
    {
        private string Hotel3CacheKey { get { return "Hotel3" + CacheHotelReviewVer; } }
        private string HotelSightCacheKey { get { return "HotelSight" + CacheHotelReviewVer; } }

        private string SearchHotelKey { get { return "SearchHotel" + CacheHotelVer; } }
        private string GetHotelKey { get { return "GetHotel" + CacheHotelVer; } }
        private string SearchHotelCountKey { get { return "SearchHotelCount" + CacheHotelVer; } }
        private string GetOrderHotelsKey { get { return "GetOrderHotels" + CacheHotelVer; } }
        private string GetSimpleHotelsKey { get { return "GetSimpleHotelsk" + CacheHotelVer; } }
        private string GetListHotelItemsKey { get { return "GetListHotelItemsKey" + CacheHotelVer; } }
        private string GetSimpleHotelsRankKey { get { return "GetSimpleHotelRanksk" + CacheHotelVer; } }
        private string GetListHotelWeixinItemsKey { get { return "GetListHotelWeixinItemsKey" + CacheHotelVer; } }
        private string MostValuedDistrict { get { return "MostValuedDistrict" + CacheHotelVer; } }
        private string MostUniqueDistrict { get { return "MostUniqueDistrict" + CacheHotelVer; } }

        private string HotelRoomTypeFilterTagListCacheKey { get { return "RoomTypeTagList" + CacheHotelVer; } }


        public List<FeaturedCommentEntity> GetHotelFeaturedCommentInfo(int hotelid)
        {
            return HotelDAL.GetHotelFeaturedCommentInfo(hotelid);
        }
        
        public List<int> GetHotelFeaturedInfo(int hotelid)
        {
            return HotelDAL.GetHotelFeaturedInfo(hotelid);
        }

        public List<AttractionCategoryRelEntity> GetAttractionCategoryRel()
        {
            return HotelDAL.GetAttractionCategoryRel();
        }

        public void BindUserAccountAndOrders(long userid, string phone)
        {
            HotelDAL.BindUserAccountAndOrders(userid, phone);
        }

        public List<InterestEntity> QueryInterest(int districtid, float lat, float lng, int distance = 300000)
        {
            return HotelDAL.QueryInterest(districtid, lat, lng, distance);
        }

        public List<InterestEntity> QueryInterest4AD(int districtid, float glat, float glon, int distance = 300000)
        {
            return HotelDAL.QueryInterest4AD(districtid, glat, glon, distance);
        }

        public List<InterestEntity> QueryInterest4ADSelected(int districtid, float glat, float glon, int distance = 300000)
        {
            return HotelDAL.QueryInterest4ADSelected(districtid, glat, glon, distance);
        }

        public InterestQueryEntity QueryInterestAndHotelCount(int districtid, float lat, float lng, int distance = 300000)
        {
            InterestQueryEntity iq = new InterestQueryEntity();
            iq.interestList = HotelDAL.QueryInterest(districtid, lat, lng, distance);
            iq.hotelCount = HotelDAL.QueryInterestHotelCount(districtid, lat, lng, distance);

            return iq;
        }

        public int QueryInterestHotelCount(int districtid, float lat, float lng, int distance = 300000)
        {
            return HotelDAL.QueryInterestHotelCount(districtid, lat, lng, distance);
        }

        public int QueryInterestHotelCountSelected(int districtid, float lat, float lng, int distance = 300000)
        {
            return HotelDAL.QueryInterestHotelCountSelected(districtid, lat, lng, distance);
        }

        public List<RoomInfoEntity> GetRoomInfo(int hotelid)
        {
            return HotelDAL.GetRoomInfo(hotelid);
        }

        public List<CityEntity> GetZMJDCityData()
        {
            return HotelDAL.GetZMJDCityData();
        }

        public List<CityEntity> GetZMJDSelectedCityData()
        {
            return HotelDAL.GetZMJDSelectedCityData();
        }

        public List<CityEntity> GetZMJDAllCityData()
        {
            return HotelDAL.GetZMJDAllCityData();
        }

        public List<CityEntity> GetZMJDLoveCityData()
        {
            return HotelDAL.GetZMJDLoveCityData();
        }

        public bool UpdateHotelInfo(HotelInfoEditEntity hi)
        {
            HotelDAL.UpdateHotelInfo(hi);
            memCacheHotel.Remove(hi.HotelID.ToString(), GetHotelKey);
            memCacheHotel.Remove(hi.HotelID.ToString(), HotelInfoExCacheKey);
            return true;
        }

        public List<Hotel3Entity> GetHotel3(int hotelid)
        {
            return memCacheHotel.GetData<List<Hotel3Entity>>(hotelid.ToString(),Hotel3CacheKey , () =>
            {
                return HotelDAL.GetHotel3(hotelid);
            });
        }

        public List<HotelSightEntity> GetHotelSightList(int hotelid)
        {
            return memCacheHotel.GetData<List<HotelSightEntity>>(hotelid.ToString(), HotelSightCacheKey , () =>
            {
                return HotelDAL.GetHotelSight(hotelid);
            });
        }

        public List<HotelRestaurantEntity> GetHotelRestaurantList(int hotelid)
        {
            return memCacheHotel.GetData<List<HotelRestaurantEntity>>( hotelid.ToString(),HotelSightCacheKey , () =>
            {
                return HotelDAL.GetHotelRestaurant(hotelid);
            });
        }

        public List<HotelThemeEntity> GetAllHotelTheme()
        {
            return memCacheHotel.GetData<List<HotelThemeEntity>>("ThemeList" + CacheHotelVer, () =>
            {
                return HotelDAL.GetAllHotelTheme();
            });
        }

        public List<InterestEntity2> GetAllInterestList()
        {
            return memCacheHotel.GetData<List<InterestEntity2>>("AllInterestList" + CacheHotelVer, () =>
            {
                return HotelDAL.GetAllInterest();
            });
        }

        public List<HotelThemeEntity> GetHotelTheme(int districtid, float lat, float lon)
        {
            if (districtid > 0)
            {
                return memCacheHotel.GetData<List<HotelThemeEntity>>("DistrictThemeList" + districtid.ToString() + CacheHotelVer, () =>
                {
                    return HotelDAL.GetDistrictHotelTheme(districtid);
                });
            }
            else
            {
                return HotelDAL.GetNearbyHotelTheme(lat, lon);
            }
        }

        public List<FeaturedEntity> GetFeaturedList()
        {
            return memCacheHotel.GetData<List<FeaturedEntity>>("FeaturedList" + CacheHotelVer, () =>
            {
                return HotelDAL.GetFeaturedList();
            });
        }

        //public HotelReviewSaveResult HotelReviewAddtionContentSubmit(HotelReviewAddtionEntity hotelreviewAddcontent)
        //{
        //    HotelReviewSaveResult hrr =
        //     HotelDAL.HotelReviewAddtionContentSubmit(hotelreviewAddcontent);
        //    if (hrr.Retcode == 0 && hotelreviewAddcontent.Writing > 0)
        //    {
        //        List<int> _i = new List<int> { hotelreviewAddcontent.Writing };
        //        CheckHotelReviewByWriting(_i);
        //        ClearCacheHotelReviewEx(hotelreviewAddcontent.Writing);
        //    }

        //    return hrr;
        //}
        //public List<HotelItem> SearchHotel(HotelSearchParas argu ,out long count)
        //{
        //    var hotels = GetHotelListByQuery(argu, out count);

        //    if (hotels.Count == 0) return new List<HotelItem>();
        //    var res = GetHotels(hotels.Select(h => h._id).ToList());
        //    res.AsParallel().ForAll(r => r.MinPrice = hotels.Where(h => r.Id == h._id).FirstOrDefault().AvgPrice);
        //    return res;
        //}

        public List<OTACodeEntity> GetOTAHotelCode(int channelid, List<int> otahotelidlist)
        {
            return HotelDAL.GetOTAHotelCode(channelid, otahotelidlist);
        }

        public List<SimpleHotelItem> GetSimpleHotels(List<int> ids)
        {
            if (ids.Count == 0)
            {
                return new List<SimpleHotelItem>();
            }
            var res = memCacheHotel.GetMultiDataEx<int, SimpleHotelItem>(ids, GetSimpleHotelsKey, noList =>
            {
                return GetSimpleHotelCore(noList);
            }
            , ReturnValue => ReturnValue.Id
                , new SimpleHotelItem { Id = -1 });

            return res.Where(h => h.Id > 0).ToList();
        }

        public List<SimpleHotelRankEntity> GetHotelRanks(List<int> ids, int ResourceID, int hoteltype, RankType rt)
        {
            if (ids.Count == 0)
            {
                return new List<SimpleHotelRankEntity>();
            }
            var res = memCacheHotel.GetMultiDataEx<int, SimpleHotelRankEntity>(ids, string.Format("{0}:{1}:{2}:{3}", GetSimpleHotelsRankKey, ResourceID, hoteltype, rt)
             , noList =>
            {
                return HotelService.GetHotelRank(noList, ResourceID, hoteltype, rt);
            }
            , ReturnValue => ReturnValue.HotelID
                , new SimpleHotelRankEntity { HotelID = -1 });

            return res.Where(h => h.HotelID > 0).ToList();
        }

        public List<HotelItem> GetHotels(List<int> ids)
        {
            if (ids.Count == 0)
            {
                return new List<HotelItem>();
            }
            var res = memCacheHotel.GetMultiData(ids, GetHotelKey, GetHotel);
            return res.Where(p => p != null).ToList();
        }

        public HotelItem GetHotel(int id)
        {
            if (id <= 0) return null;

            var hotel = memCacheHotel.GetData(id.ToString(), GetHotelKey, () => GetHotelCore(id));
            return hotel;
        }

        public void ClearCacheHotel(int id)
        {
            try
            {
                memCacheHotel.Remove(id.ToString(), GetHotelKey);
            }
            catch
            {
            }
        }

        public int GetReviewRatingCount(QueryReviewResult qrr, RatingType rt)
        {
            return qrr.FilterCount.ContainsKey(HotelReviewFilterPrefix.Rate + (int)rt) ?
                    qrr.FilterCount[HotelReviewFilterPrefix.Rate + (int)rt] : 0;
        }

        public List<int> GetPackageHotelList()
        {
            return HotelDAL.GetPackageHotelList();
        }

        private HotelItem GetHotelCore(int id)
        {
            var hotel = HotelDAL.GetHotel(id);
            if (hotel == null) return null;

            var hc = GetHotelClassByHotel(new List<int> { id }).Where(p => p.ClassID != 0);
            var ho = GetHotelChannelList(new List<int> { id }).FirstOrDefault();
            hotel.Types = hc.Select(p => p.ClassName).ToList();
            hotel.Facilities = HotelDAL.GetFacilityList(new List<int> { id });



            ArguHotelReview q = new ArguHotelReview();

            QueryReviewResult qrr = null;

            q.Hotel = hotel.Id;
            qrr = QueryReview(q);

            hotel.ReviewCount = qrr.TotalCount;
            hotel.VoteComponenets = string.Format("{0},{1},{2},{3},{4}",
               GetReviewRatingCount(qrr, RatingType.Best),
                 GetReviewRatingCount(qrr, RatingType.Better),
                  GetReviewRatingCount(qrr, RatingType.Good),
                   GetReviewRatingCount(qrr, RatingType.Normal),
                    GetReviewRatingCount(qrr, RatingType.Terrible));



            if (ho != null)
            {
                hotel.OTAID = new Dictionary<string, string>();
                ho.HotelOriIDList.ForEach(c =>
                    {
                        if (!hotel.OTAID.ContainsKey(c.Channel))
                            hotel.OTAID.Add(c.Channel, string.Format("{0}|{1}", c.HotelOriID, c.CanSyncPrice));
                    });
            }

            hotel.FeaturedList = GetHotelFeatured(hotel.Id);
            return hotel;
        }

        public List<SimpleHotelItem> GetSimpleHotelCore(List<int> hotelIDs)
        {

            var hotels = HotelDAL.GetSimpleHotel(hotelIDs);
            if (hotels == null) return null;

            //    var css = GetHotelReviewSummary(hotelIDs);
            var hos = GetHotelChannelList(hotelIDs);

            ArguHotelReview p = new ArguHotelReview();

            //  QueryReviewResult qrr = null;

            foreach (SimpleHotelItem hotel in hotels)
            {
                p.Hotel = hotel.Id;
                //qrr = QueryReview(p);
                //hotel.ReviewCount = qrr.TotalCount;
                //hotel.VoteComponenets = string.Format("{0},{1},{2},{3},{4}",
                //   GetReviewRatingCount(qrr, RatingType.Best),
                //     GetReviewRatingCount(qrr, RatingType.Better),
                //      GetReviewRatingCount(qrr, RatingType.Good),
                //       GetReviewRatingCount(qrr, RatingType.Normal),
                //        GetReviewRatingCount(qrr, RatingType.Terrible));

                hotel.VoteComponenets = "0,0,0,0,0";
                hotel.FeaturedList = GetHotelFeatured(hotel.Id);

                var ho = hos.Where(h => h.HotelID == hotel.Id).FirstOrDefault();
                if (ho != null)
                {
                    hotel.OTAID = new Dictionary<string, string>();
                    ho.HotelOriIDList.ForEach(c =>
                    {
                        if (!hotel.OTAID.ContainsKey(c.Channel))
                            hotel.OTAID.Add(c.Channel, c.HotelOriID.ToString());
                    });
                }
            }

            return hotels;
        }

        //public List<HotelReviewExEntity> GetHotelReviews(long userid, int hotel, int review, int start, int count)
        //{
        //    var argu = new ArguHotelReview
        //                   {
        //                       HideUserID = userid,
        //                       Hotel = hotel,
        //                       HotelReviewOrderType = HotelReviewOrderType.Time_Down,
        //                       UserID = userid,
        //                       Start = start,
        //                       Count = count,
        //                       RatingType = RatingType.All,
        //                       UserIdentityType = UserIdentityType.All
        //                   };
        //    var list = GetHotelReviewList(argu);
        //    return list;
        //}

        public int GetHotelReviewCount(int hotel)
        {
            var res = GetHotelReviewSummary(new List<int> { hotel }).FirstOrDefault();
            if (res == null) return 0;
            return res.CountHotelReview;
        }

        public void UpdateHotelTagWithWriting(int writing)
        {

            ZhongdangTag.UpdateHotelTag(writing);
        }

        public void UpdateHotelTag(DateTime dt)
        {
            UpdateHotelTag2(dt, DateTime.Now);
        }

        public void UpdateHotelTag2(DateTime beforeDate, DateTime now)
        {
            string before = beforeDate.Date.ToShortDateString();
            string today = now.Date.ToShortDateString();

            List<HotelReview4TagKeywordEntity> list = HotelDAL.GetNewHotelReviewData(before, today);
            ZhongdangTag.UpdateZhongdangTag(list);
        }

        public void UpdateHotelSeoKeyword()
        {
            var writing = HotelDAL.GetSeoKeywordReviewID4Do();
            SeoKeyword.UpdateSeoKeyword(writing);
        }

        public List<ZhongdangHotelEntity> GetZhongdangHotelInfo()
        {
            return HotelDAL.GetZhongdangHotelInfo();
        }

        public void UpdateZhongdangHotel()
        {
            //ZhongdangTag.ChooseZhongdangHotel();
        }

        public List<ValuedDistrictEntity> GetMostValuedDistrict()
        {
            return memCacheHotel.GetData<List<ValuedDistrictEntity>>(MostValuedDistrict, () =>
                {
                    return HotelDAL.GetMostValuedDistrict();
                });
        }

        public List<UniqueDistrictEntity> GetMostUniqueDistrict()
        {
            return memCacheHotel.GetData<List<UniqueDistrictEntity>>(MostUniqueDistrict, () =>
                {
                    return HotelDAL.GetMostUniqueDistrict();
                });
        }

        public List<CommDictEntity> GetCommDictList(int type)
        {
            return HotelDAL.GetCommDictList(type);
        }

        public void GetValuedHotel()
        {
            List<ValuedHotelEntity> list = HotelDAL.GetBaseValuedHotel();
            string PriceSection = System.Configuration.ConfigurationManager.AppSettings["PriceSection"];
            string[] prices = PriceSection.Split('|');
            List<ChaoZhiPriceSection> priceList = new List<ChaoZhiPriceSection>();
            for (int i = 0; i < prices.Length - 1; i++)
            {
                ChaoZhiPriceSection cs = new ChaoZhiPriceSection(double.Parse(prices[i]), double.Parse(prices[i + 1]));
                priceList.Add(cs);
            }

            List<Dictionary<int, List<ValuedHotelEntity>>> districtSetList = new List<Dictionary<int, List<ValuedHotelEntity>>>();
            foreach (ValuedHotelEntity ci in list)
            {
                if (ci.AllReviewNum > 0)
                {
                    ci.WritingRate = (double)(ci.GoodReviewNum) / (double)(ci.AllReviewNum) * Math.Atan((double)ci.AllReviewNum / 50);
                    if (ci.WritingRate < 0.03 * Math.PI / 2 || ci.GoodReviewNum < 5)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                ci.RankRate = (double)ci.Score / 5;
                int mi = 0;
                if (ci.Price > 0 && ci.Price < 300)
                {
                    mi = 8;
                }
                else if (ci.Price >= 300 && ci.Price < 500)
                {
                    mi = 10;
                }
                else if (ci.Price >= 500 && ci.Price < 700)
                {
                    mi = 13;
                }
                else if (ci.Price >= 700 && ci.Price < 1000)
                {
                    mi = 15;
                }
                else if (ci.Price >= 1000)
                {
                    mi = 18;
                }

                ci.RankRate = Math.Pow(ci.RankRate, mi) / (double)ci.Price;


                foreach (ChaoZhiPriceSection cs in priceList)
                {
                    if ((double)ci.Price <= cs.Max && (double)ci.Price > cs.Min)
                    {
                        if (cs.Dic.ContainsKey(ci.DistrictId))
                        {
                            cs.Dic[ci.DistrictId].Add(ci);
                        }
                        else
                        {
                            cs.Dic.Add(ci.DistrictId, new List<ValuedHotelEntity>());
                            cs.Dic[ci.DistrictId].Add(ci);
                        }
                    }
                }
            }

            HotelDAL.TruncateValued();

            foreach (ChaoZhiPriceSection cs in priceList)
            {
                int type = 0;
                switch ((int)cs.Max)
                {
                    case 150:
                        type = 1;
                        break;
                    case 300:
                        type = 2;
                        break;
                    case 500:
                        type = 3;
                        break;
                    case 700:
                        type = 4;
                        break;
                    case 1000:
                        type = 5;
                        break;
                    case 999999:
                        type = 6;
                        break;
                    default:
                        break;
                }
                foreach (int districtId in cs.Dic.Keys)
                {
                    List<ValuedHotelEntity> infos = cs.Dic[districtId];
                    double MaxRankScore = 0;
                    double MaxWritingScore = 0;
                    foreach (ValuedHotelEntity ci in infos)
                    {
                        if (ci.WritingRate > MaxWritingScore)
                        {
                            MaxWritingScore = ci.WritingRate;
                        }
                        if (ci.RankRate > MaxRankScore)
                        {
                            MaxRankScore = ci.RankRate;
                        }
                    }
                    foreach (ValuedHotelEntity ci in infos)
                    {
                        if (MaxWritingScore > 0)
                        {
                            ci.WritingRate /= MaxWritingScore;
                        }
                        else
                        {
                            ci.WritingRate = 0;
                        }

                        if (MaxRankScore > 0)
                        {
                            ci.RankRate /= MaxRankScore;
                        }
                        else
                        {
                            ci.RankRate = 0;
                        }



                        ci.FinalScore = (ci.RankRate * 0.7 + ci.WritingRate * 0.3) * 100;



                    }

                    infos.Sort();
                    foreach (ValuedHotelEntity hi in infos)
                    {
                        if (hi.FinalScore >= 40.0)
                        {
                            HotelDAL.AddValuedHotel(hi.HotelId, hi.FinalScore, type);
                        }

                    }

                }
            }
        }

        public List<HotelSEOKeyword> GetHotelSEOKeyword(int hotelID)
        {
            return HotelDAL.GetHotelSEOKeyword(hotelID);
        }

        public List<HotelSEOKeyword> GetAllHotelSEOKeyword()
        {
            return HotelDAL.GetHotelSEOKeyword();
        }

        public void MoveReview17U(MoveReview17U mr)
        {
            HotelDAL.MoveReview(mr);
        }


        public void MoveReviewElong(MoveReviewElong mr)
        {
            HotelDAL.MoveReview(mr);
        }

        public void MoveReviewCtrip(MoveReviewCtrip mr)
        {
            HotelDAL.MoveReview(mr);
        }

        public int InitReviewOTA(MoveReviewOTA mr)
        {
            return HotelDAL.InitReviewOTA(mr);
        }

        public void InitReview17U(MoveReview17U mr)
        {
            HotelDAL.InitReview17U(mr);
        }

        public void InitReviewElong(MoveReviewElong mr)
        {
            HotelDAL.InitReviewElong(mr);
        }
        //public void InitReviewQunar(MoveReviewElong mr)
        //{
        //    HotelDAL.InitReviewQunar(mr);
        //}

        public int InitReviewCtrip(MoveReviewCtrip mr)
        {
            int writing = 0;
            //過濾
            //     if (!string.IsNullOrEmpty(mr.reviewType))
            {
                try
                {
                    writing = HotelDAL.InitReviewCtrip(mr);
                }
                catch (Exception e)
                {
                }
            }

            return writing;
        }

        public int InitReviewBooking(MoveReviewBooking mr)
        {
            int writing = 0;
            try
            {
                writing = HotelDAL.InitReviewBooking(mr);
            }
            catch (Exception e)
            {

            }

            return writing;
        }

        public List<HotelFilterColEntity> GetManyHotelFilterCols(IEnumerable<int> hotelIds)
        {
            return HotelDAL.GetManyHotelFilterCols(hotelIds);
        }

        /// <summary>
        /// 获得酒店详情页展示的标签(房型除外)
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public List<HotelFilterColEntity> GetHotelDisplayFilterTagList(int hotelId)
        {
            return HotelDAL.GetHotelDisplayFilterTagList(hotelId);
        }

        /// <summary>
        /// 出所有不重复的房型
        /// 每个房型下可能有标签 可能没有
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public List<HotelRoomTypeFilterTagEntity> GetHotelRoomTypeFilterTagList(int hotelId)
        {
            return memCacheHotel.GetData<List<HotelRoomTypeFilterTagEntity>>(hotelId.ToString(), HotelRoomTypeFilterTagListCacheKey, () =>
            {
                return GenHotelRoomTypeFilterTagList(hotelId);
            });
        }

        public List<HotelRoomTypeFilterTagEntity> GenHotelRoomTypeFilterTagList(int hotelId)
        {
            var resultList = HotelDAL.GetHotelRoomTypeFilterTagList(hotelId);
            if (resultList != null && resultList.Count != 0)
            {
                resultList.ForEach((_) => { _.HotelId = hotelId; });
            }
            return resultList;
        }

        public List<PRoomInfoEntity> GetProomInfoList(int hotelId)
        {
            return HotelDAL.GetProomInfoList(hotelId);
        }

        #region 铂韬结算订单数据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int InsertBotaoSettlementRecord(BotaoSettlementEntity entity)
        {
            return HotelDAL.InsertBotaoSettlementRecord(entity);
        }

        /// <summary>
        /// 获取待结算订单列表
        /// </summary>
        /// <returns></returns>
        public List<BotaoSettleOrderEntity> GetBotaoSettleOrderList()
        {
            return HotelDAL.GetBotaoSettleOrderList();
        }

        #endregion

        /// <summary>
        /// 由酒店名字获得酒店ID
        /// </summary>
        /// <param name="hotelName"></param>
        /// <returns></returns>
        public int GetHotelIdByName(string hotelName)
        {
            return HotelDAL.GetHotelIdByName(hotelName);
        }

        public List<NearbyHotelEntity> GetHotelListNearPOI(int poiId)
        {
            return HotelDAL.GetHotelListNearPOI(poiId);
        }

        public List<CanSellDistrictCheapHotel> GetCanSellDistrictCheapHotelList(HJD.HotelServices.Contracts.HotelServiceEnums.HotelDistrictRange range)
        {
            return HotelDAL.GetCanSellDistrictCheapHotelList(range);
        }

        public List<HotelTop1PackageInfoEntity> GetHotelPackageFirstAvailablePriceByPIds(IEnumerable<int> pIds, DateTime? checkIn, DateTime? checkOut)
        {
            return HotelDAL.GetHotelPackageFirstAvailablePriceByPIds(pIds, checkIn, checkOut);
        }

        public List<TopScoreHotelEntity> GetTopScoreHotelList(BsicSearchParam param, out int totalCount)
        {
            totalCount = 0;
            return HotelDAL.GetTopScoreHotelList(param, out totalCount);
        }

        public List<PackageAlbumsEntity> GetPackageAlbumByGeoInfo(int districtID, float lat, float lng)
        {
            return HotelDAL.GetPackageAlbumByGeoInfo(districtID, lat, lng);
        }

        public int UpdateTopScoreHotel(int Id, float RankScore)
        {
            return HotelDAL.UpdateTopScoreHotel(Id, RankScore);
        }
        
        /// <summary>
        /// 批量获取酒店是否可显示携程价格
        /// </summary>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        public Dictionary<int, bool> CanShowCtripPrice(IEnumerable<int> hotelIds)
        {
            return HotelDAL.CanShowCtripPrice(hotelIds);
        }

        public List<TopScoreHotelEntity> GetHotelBrowsingRecordList(BsicSearchParam param, out int totalCount)
        {
            return HotelDAL.GetHotelBrowsingRecordList(param, out totalCount);
        }

        public List<BrowsingRecordEntity> GetBrowsingRecordList(BsicSearchParam param, out int totalCount)
        {
            return HotelDAL.GetBrowsingRecordList(param, out totalCount);
        }

        public List<TopScoreHotelEntity> GetHotelWithInDistance(float lat, float lng, int count, int start, out int totalCount)
        {
            return HotelDAL.GetHotelWithInDistance(lat, lng, count, start, out totalCount); 
        }

        public List<SearchRecordEntity> GetSearchRecordList(CommonRecordQueryParam param, out int totalCount)
        {
            return HotelDAL.GetSearchRecordList(param, out totalCount);
        }
    }
}