using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.AccessService.Contract;
using System.ServiceModel;
using System.Configuration;
using HJD.Framework.Interface;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HJD.AccessService.Implement.Helper;
using System.Text.RegularExpressions;
using HJD.AccessService.Implement;
using HJD.AccessService.Contract.Model;
using RabbitMQ.Client;
using HJD.AccessService.Implement.Entity;
using HJD.AccessService.Contract.Params;
using RabbitMQ.Client.Events;
using HJD.Search.CommonLibrary.Engine;
using HJD.Search.CommonLibrary.Model;
using HJD.HotelServices.Contracts;
using HJD.Framework.WCF;
using HJD.AccessService.Contract.Model.Dialog;
using HJD.Search.CommonLibrary.Helper;

namespace HJD.AccessService.Implement
{
    /// <summary>
    /// 数据记录与分析
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class AccessServices : IAccessService
    {
        private static IHotelService hotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");

        private static IMemcacheProvider HotelDistrictCache = MemcacheManagerFactory.Create("HotelDistrictCache");

        #region MagiCall 相关接口实现

    


      //public void MagiCallClientHeart(long userid)
      //  {
      //      MagiCallEngine.MagiCallClientHeart(userid);
      //  }

       //public List<DialogItemsEntity> GetLastDialogItems(long lastItemID)
       //{
       //    return MagiCallEngine.GetLastDialogItems(lastItemID);
       //}

  
      public List<UserWordOptionItem> ParseInput(string Text, long IDX)
      {
          return ParseEngine.ParseUserWord(new UserWordItem { Text = Text, ItemID = IDX });
      }


        #endregion

        #region 行为队列实例

        static IModel mChannel;
        static IConnection connection;
        static int initChannel = 0;
        static string queueKey = "BehaviorQueue";

        public IModel GetChannel
        {
            get
            {
                if (initChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = Config.RabbitmqHostName;
                    factory.UserName = Config.RabbitmqUserName;
                    factory.Password = Config.RabbitmqPassword;

                    connection = factory.CreateConnection();
                    mChannel = connection.CreateModel();
                    mChannel.QueueDeclare(queueKey, false, false, false, null);

                    initChannel = 1;
                }
                return mChannel;
            }
        }

        #endregion

        #region 记录行为

        /// <summary>
        /// 记录行为
        /// </summary>
        /// <param name="behavior">行为对象</param>
        public void RecordBehavior(Behavior behavior)
        {
            try
            {
                var behaviorProfile = GetBehaviorProfile(behavior.Code);
                if (behaviorProfile != null && behaviorProfile.Enable == "1")
                {
                    behavior.Page = behaviorProfile.Page;
                    behavior.Event = behaviorProfile.Event;

                    var behaviorString = BehaviorHelper.FormatBehaviorString(behavior);
                    GetChannel.BasicPublish("", queueKey, null, Encoding.UTF8.GetBytes(behaviorString));
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 批量记录
        /// </summary>
        /// <param name="behavior"></param>
        public void RecordBehaviorQueue(List<Behavior> behaviorQueue)
        {
            foreach (var behavior in behaviorQueue)
            {
                RecordBehavior(behavior);
            }
        }

        /// <summary>
        /// 获取行为配置操作对象
        /// </summary>
        /// <returns></returns>
        public BehaviorField GetBehaviorProfile(string strFieldName)
        {
            return BehaviorProfile.GetInstance().GetBehaviorField(strFieldName);
        }

        #endregion

        #region 搜索相关

        /// <summary>
        /// 搜索酒店相关信息
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="limitCount"></param>
        /// <returns></returns>
        public List<HotelSearchResult> SearchHotel(string keywords, int limitCount)
        {
            //创建搜索引擎对象
            var searchEngine = SearchEngine.GetInstance(SearchType.Hotel);

            //搜索开始时间
            searchEngine.SearchStartTime = DateTime.Now;

            //执行搜索
            var list = new List<BaseSearchResult>();

            try
            {
                list = searchEngine.Search(keywords, limitCount, SearcherType.Single);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(string.Format(@"D:\Log\AccessService\AccessLog_{0}.txt", DateTime.Now.ToString("MM_dd")), ex.Message);
            }

            //将搜索的id转换为对象
            var searchResultList = HotelEngine.GetHotelSearchResult(list);

            //return
            return searchResultList;
        }

        /// <summary>
        /// 酒店信息的严格搜索处理
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="limitCount"></param>
        /// <returns></returns>
        public List<SearchResult> StrictSearch(string keywords, int hotelLimitCount)
        {
            var searchList = new List<SearchResult>();

            keywords = keywords.ToLower().Trim();

            try
            {
                var cacheKeywords = keywords;
                searchList = HotelDistrictCache.GetData<List<SearchResult>>(string.Format("StrictSearchResult_{0}_{1}", cacheKeywords, hotelLimitCount), () => { return StrictSearchBasic(keywords, hotelLimitCount); });
            }
            catch (Exception ex)
            {
                searchList = StrictSearchBasic(keywords, hotelLimitCount);
            }

            return searchList;
        }

        public List<SearchResult> StrictSearchBasic(string keywords, int hotelLimitCount)
        {
            var searchList = new List<SearchResult>();

            #region 检索出搜索值中的目的地、主题、品牌等分词

            //首先得到当前搜索值的分词集
            var ppList = SearchEngine.GetWords(keywords);

            DistrictInfoEntity districtKey = null;
            var hotelHaveDistrict = false;

            BrandEntity brandKey = null;
            var hotelHaveBrand = false;

            InterestInfoEntity themeKey = null;
            var hotelHaveTheme = false;

            #region 检索出目的地分词

            //获取所有目的地数据
            var districtList = HotelDistrictCache.GetData<List<DistrictInfoEntity>>("HotelDistrictList", () => { return HotelEngine.GetDistrictList(); });

            //得到当前搜索值中的目的地
            districtKey = GetDistrictKey(ppList.ToList(), districtList);

            #endregion

            #region 检索出品牌分词

            //获取所有酒店品牌数据
            var brandList = HotelDistrictCache.GetData<List<BrandEntity>>("HotelBrandList", () => { return HotelEngine.GetBrandList(); });

            //得到当前搜索值中的酒店品牌
            brandKey = GetBrandKey(ppList.ToList(), brandList);

            #endregion

            #region 检索出主题分词

            //获取所有酒店主题数据
            var themeList = HotelDistrictCache.GetData<List<InterestInfoEntity>>("HotelThemeList", () => { return HotelEngine.GetInterestList(); });

            //得到当前搜索值中的酒店主题
            themeKey = GetInterestKey(ppList.ToList(), themeList);

            #endregion

            #endregion

            #region 酒店结果生成

            var hotelResultList = new List<HotelSearchResult>();

            //过滤掉搜索值一些关键字
            keywords = FilterKeywords(keywords);

            //首先搜索出基础搜索列表
            var basicSearchCount = 50;
            var basicHotelList = SearchHotel(keywords, basicSearchCount);
            if (basicHotelList.Count > 0)
            {
                #region 首先划分出目的地范围

                if (districtKey != null && basicHotelList.Exists(s => s.HotelName.ToLower().Contains(districtKey.Name.ToLower())))
                {
                    basicHotelList = basicHotelList.Where(s => s.HotelName.ToLower().Contains(districtKey.Name.ToLower())).ToList();

                    //如果有目的地关键词，后面的筛选则剔除掉目的地
                    keywords = keywords.Replace(districtKey.Name.ToLower(), "");

                    hotelHaveDistrict = true;
                }

                #endregion

                #region 区分出酒店品牌范围

                if (brandKey != null && basicHotelList.Exists(s => s.HotelName.ToLower().Contains(brandKey.BrandName.ToLower())))
                {
                    basicHotelList = basicHotelList.Where(s => s.HotelName.ToLower().Contains(brandKey.BrandName.ToLower())).ToList();

                    //如果有目的地关键词，后面的筛选则剔除掉目的地
                    keywords = keywords.Replace(brandKey.BrandName.ToLower(), "");

                    hotelHaveBrand = true;
                }

                #endregion

                #region 取得酒店列表匹配需要的分词

                //重新得出搜索值分词组
                var hotelPartiList = SearchEngine.GetWords(keywords);

                //过滤掉单字的分词（一个字的词没有什么意义）
                hotelPartiList = hotelPartiList.Where(p => p.Length > 1).ToList();

                #endregion

                if (basicHotelList.Count > 0)
                {
                    if (hotelPartiList.Count > 0)
                    {
                        //遍历每一个搜索结果
                        for (int i = 0; i < basicHotelList.Count; i++)
                        {
                            var hotelEntity = basicHotelList[i];

                            //同样跟搜索值一样过滤掉同样的关键字
                            var hotelName = FilterKeywords(hotelEntity.HotelName);

                            //然后也得到结果酒店名称的分词
                            var pnameList = SearchEngine.GetWords(hotelName);

                            #region 匹配/过滤

                            var matchScore = 0;

                            //然后通过搜索语句分词，来计算出 搜索值与结果 之间的匹配度（目前暂以搜索分词匹配50%以上为准）
                            foreach (var pword in hotelPartiList)
                            {
                                if (hotelName.ToLower().Contains(pword.ToLower()))
                                {
                                    matchScore++;
                                }
                            }

                            if (matchScore > 0)
                            {
                                hotelResultList.Add(hotelEntity);
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        hotelResultList.AddRange(basicHotelList);
                    }
                }
            }

            #region 设置酒店的照片

            try
            {
                for (int i = 0; i < hotelResultList.Count; i++)
                {
                    var hotelEntity = hotelResultList[i];

                    HotelPhotosEntity phe = hotelService.GetHotelPhotos(hotelEntity.HotelId);
                    if (phe != null && phe.HPList != null && phe.HPList.Count > 0)
                    {
                        if (phe.HPList.Exists(ph => ph.IsCover))
                        {
                            hotelEntity.HotelCoverPicUrl = string.Format("http://whphoto.b0.upaiyun.com/{0}_{1}", phe.HPList.Find(ph => ph.IsCover).SURL, "theme");
                        }
                        else
                        {
                            hotelEntity.HotelCoverPicUrl = string.Format("http://whphoto.b0.upaiyun.com/{0}_{1}", phe.HPList.First().SURL, "theme");
                        }

                    }
                    else
                    {
                        hotelEntity.HotelCoverPicUrl = "";
                    }
                }
            }
            catch (Exception ex)
            {

            }

            #endregion

            #region 筛选指定的条数

            if (hotelResultList.Count > hotelLimitCount)
            {
                hotelResultList = hotelResultList.Take(hotelLimitCount).ToList();
            }

            //自定义排序
            if (hotelResultList.Count > 0)
            {
                hotelResultList = hotelResultList.OrderByDescending(h => h.ReviewScore).ToList();
            }

            #endregion

            searchList.Add(new SearchResult { Type = SearchType.Hotel, HotelList = hotelResultList });

            #endregion

            #region 目的地结果生成

            try
            {
                if (districtKey != null)
                {
                    //取得酒店目的地+酒店数目的集合
                    var districtHotelsList = HotelDistrictCache.GetData<List<DistrictRelationInfoEntity>>("DistrictHotelsList", () => { return HotelEngine.GetDistrictHotelsList(); });

                    //如果当前搜索的目的地下有酒店，则推荐一条类似“查看该目的地下酒店”的项
                    if (districtHotelsList.Exists(dh => dh.DistrictId == districtKey.DistrictId))
                    {
                        searchList.Add(new SearchResult
                        {
                            Type = SearchType.District,
                            DistrictList = new List<DistrictSearchResult> { new DistrictSearchResult
                            {
                                DistrictId = districtKey.DistrictId,
                                Name = districtKey.Name,
                                Ename = districtKey.EName
                            }}
                        });

                        #region 目的地+主题结果生成

                        try
                        {
                            if (themeKey != null)
                            {
                                //取得酒店目的地+酒店主题+酒店数目的集合
                                var districtInterestList = HotelDistrictCache.GetData<List<DistrictRelationInfoEntity>>("DistrictInterestList", () => { return HotelEngine.GetDistrictInterestList(); });

                                //如果当前搜索的目的地+主题下有酒店，则推荐一条类似“查看该目的地的该主题下酒店”的项
                                if (districtInterestList.Exists(di => di.DistrictId == districtKey.DistrictId && di.InterestId == themeKey.Id))
                                {
                                    searchList.Add(new SearchResult
                                    {
                                        Type = SearchType.Theme,
                                        ThemeList = new List<ThemeSearchResult> { new ThemeSearchResult
                                        {
                                            ThemeId = themeKey.Id,
                                            DistrictId = districtKey.DistrictId,
                                            DistrictName = districtKey.Name,
                                            Name = themeKey.Name,
                                            Ename = ""
                                        }}
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {

            }

            #endregion

            return searchList;
        }

        public static string[] HotelTypeWordsA = new string[] { 
        "酒店式公寓","酒店公寓",
        "青年旅舍","青年旅馆","青年旅社","家庭旅馆",
        "酒店","宾馆","公寓","会所","度假村","饭店","山庄","客栈","招待所","俱乐部",
        "驿站","公馆","农家乐","渔家乐","旅馆","旅社","旅舍","旅店","民宿"};

        /// <summary>
        /// 对指定值过滤指定关键字
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public string FilterKeywords(string keywords)
        {
            foreach (var w in HotelTypeWordsA)
            {
                keywords = keywords.Replace(w, "");
            }

            return keywords;
        }

        /// <summary>
        /// 在目的地集合中找出匹配的目的地分词
        /// </summary>
        /// <param name="wordList"></param>
        /// <param name="districtList"></param>
        /// <returns></returns>
        public DistrictInfoEntity GetDistrictKey(List<string> wordList, List<DistrictInfoEntity> districtList)
        {
            DistrictInfoEntity districtKey = null;

            foreach (var word in wordList)
            {
                if (districtList.Exists(d => d.Name.ToLower().Trim() == word.ToLower().Trim()))
                {
                    districtKey = districtList.Find(d => d.Name.ToLower().Trim() == word.ToLower().Trim());
                    districtKey.Name = word.Trim();
                    break;
                }
            }

            return districtKey;
        }

        /// <summary>
        /// 在品牌集合中找出匹配的品牌分词
        /// </summary>
        /// <param name="wordList"></param>
        /// <param name="brandList"></param>
        /// <returns></returns>
        public BrandEntity GetBrandKey(List<string> wordList, List<BrandEntity> brandList)
        {
            BrandEntity brandKey = null;

            foreach (var word in wordList)
            {
                if (brandList.Exists(d => d.BrandName.ToLower().Trim() == word.ToLower().Trim()))
                {
                    brandKey = brandList.Find(d => d.BrandName.ToLower().Trim() == word.ToLower().Trim());
                    brandKey.BrandName = word.Trim();
                    break;
                }
            }

            return brandKey;
        }

        /// <summary>
        /// 在酒店主题集合中找出匹配的酒店主题分词
        /// </summary>
        /// <param name="wordList"></param>
        /// <param name="interestList"></param>
        /// <returns></returns>
        public InterestInfoEntity GetInterestKey(List<string> wordList, List<InterestInfoEntity> interestList)
        {
            InterestInfoEntity interestKey = null;

            foreach (var word in wordList)
            {
                if (interestList.Exists(d => d.Name.ToLower().Trim() == word.ToLower().Trim()))
                {
                    interestKey = interestList.Find(d => d.Name.ToLower().Trim() == word.ToLower().Trim());
                    interestKey.Name = word.Trim();
                    break;
                }
            }

            return interestKey;
        }

        #endregion

        #region Qa 搜索相关

        /// <summary>
        /// 搜索酒店相关信息
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="limitCount"></param>
        /// <returns></returns>
        public QaSearchResult QaSearchHotel(string keywords, int limitCount, string checkIn = "", string checkOut = "", int minPrice = -1, int maxPrice = -1, string newWords = "", string userWordOptions = "")
        {
            var result = new QaSearchResult();

            result.CheckIn = checkIn;
            result.CheckOut = checkOut;
            result.MinPrice = minPrice;
            result.MaxPrice = maxPrice;

            try
            {
                result.Keyword = keywords;

                //问题的分词
                result.QuestionWords = GenQuestionWords(keywords, newWords, userWordOptions); 

                //创建搜索引擎对象
                var searchEngine = QaSearchEngine.GetInstance(SearchType.QaHotel);

                //搜索开始时间
                searchEngine.SearchStartTime = DateTime.Now;

                //执行搜索
                try
                {
                    result.HotelIds = searchEngine.QaSearch(result.QuestionWords, limitCount, SearcherType.Single);
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(string.Format(@"D:\Log\AccessService\AccessLog_{0}.txt", DateTime.Now.ToString("MM_dd")), "\r\n*****" + ex.Message);
                }

                //根据酒店ID获得酒店实体对象
                result.Hotels = HotelEngine.GetHotelSearchResult(result.HotelIds);

                result.Hotels = FillHotelsInfo(result.Hotels, checkIn, checkOut, minPrice, maxPrice);
       

            
            }
            catch (Exception ex)
            {
               LogHelper.WriteLog( ex.Message + ex.StackTrace);
            }

                #region 计算得出所有酒店包含的Filter项

                //得到所有酒店包含的Filter
                var filterList = new List<FilterSearchResult>();
                result.Hotels.ForEach(_ =>
                {
                    foreach (var item in _.FilterList)
                    {
                        if (!filterList.Exists(f => f.Name == item.Name && f.Type == item.Type))
                        {
                            filterList.Add(item);
                        }
                    }
                });

                result.Filters = filterList;

                #endregion

            return result;
        }


        public List<HotelSearchResult> FillHotelsInfo(List<HotelSearchResult> Hotels ,  string checkIn = "", string checkOut = "", int minPrice = -1, int maxPrice = -1)
        {
          
            try
            {
                #region 填充每一个酒店的主题、设施等信息集
                for (int i = 0; i < Hotels.Count; i++)
                    {
                        var h = Hotels[i];

                        h.FilterList = new List<FilterSearchResult>();

                        //酒店主题
                        var instersestData = HotelDistrictCache.GetData<List<FilterSearchResult>>("GetInterestResultByHotelIds_" + h.HotelId, () => { return HotelEngine.GetInterestResultByHotelIds(new List<BaseSearchResult> { new BaseSearchResult { Id = h.HotelId.ToString() } }); });
                        h.FilterList.AddRange(instersestData);

                        //酒店标签
                        var featuredData = HotelDistrictCache.GetData<List<FilterSearchResult>>("GetFeaturedResultByHotelIds_" + h.HotelId, () => { return HotelEngine.GetFeaturedResultByHotelIds(new List<BaseSearchResult> { new BaseSearchResult { Id = h.HotelId.ToString() } }); });
                        h.FilterList.AddRange(featuredData);

                        //酒店类型
                        var classData = HotelDistrictCache.GetData<List<FilterSearchResult>>("GetClassResultByHotelIds_" + h.HotelId, () => { return HotelEngine.GetClassResultByHotelIds(new List<BaseSearchResult> { new BaseSearchResult { Id = h.HotelId.ToString() } }); });
                        h.FilterList.AddRange(classData);

                        //酒店设施
                        var facilityData = HotelDistrictCache.GetData<List<FilterSearchResult>>("GetFacilityResultByHotelIds_" + h.HotelId, () => { return HotelEngine.GetFacilityResultByHotelIds(new List<BaseSearchResult> { new BaseSearchResult { Id = h.HotelId.ToString() } }); });
                        h.FilterList.AddRange(facilityData);
                    }

                #endregion

                #region 根据入住/离店时间 & 价格区间来筛选酒店
                
                //当min和max价格小于0，并且日期为空的时候，说明不需要追加查询HotelPriceSlot表
                if ((minPrice >= 0 && maxPrice >= 0) || (!string.IsNullOrEmpty(checkIn) && !string.IsNullOrEmpty(checkOut)))
                {
                    var checkInDate = DateTime.Now.AddDays(1).Date;
                    var checkOutDate = checkInDate.AddDays(1).Date;

                    //如果日期没有选择或转换错误，则将入住/离店日期默认为今天-30天
                    if (!string.IsNullOrEmpty(checkIn) && !string.IsNullOrEmpty(checkOut))
                    {
                        try
                        {
                            checkInDate = DateTime.Parse(checkIn);
                            checkOutDate = DateTime.Parse(checkOut);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    //如果价格没有设置，则默认0-不限
                    if (minPrice >= 0 && maxPrice >= 0)
                    {
                        if (minPrice >= maxPrice) maxPrice = 0;
                    }
                    else
                    {
                        minPrice = 0;
                        maxPrice = 0;
                    }

                    //价格段列名
                    var slotColName = string.Format("Slot_{0}_{1}", minPrice, maxPrice);

                    List<int> hotelIDList = Hotels.Select(h=> int.Parse( h.Id)).ToList();

                    if (slotColName != "Slot_0_0")
                    {
                        //获取所有酒店的HotelPriceSlot数据
                        var hotelPriceSlotList = HotelEngine.GenHotelPriceSlots(hotelIDList, checkInDate, checkOutDate, slotColName);

                        //将价格段数据分配至对应酒店
                        foreach (var hotelEntity in Hotels)
                        {
                            //获取当前酒店的价格段数据
                            var slotList = hotelPriceSlotList.Where(s => s.HotelId == hotelEntity.HotelId);
                            if (slotList != null)
                            {
                                hotelEntity.PirceSlotList = slotList.OrderBy(s => s.Night).ToList();
                            }
                        }

                        //在有日期和价格区间的条件情况下，将没有PirceSlotList的酒店筛选掉
                        Hotels = Hotels.Where(h => h.PirceSlotList != null && h.PirceSlotList.Count > 0).ToList();

                        //将没有价格的也返回，不过是放在列表的最下面
                       // result.Hotels.AddRange(hotels.Where(h => h.PirceSlotList == null || h.PirceSlotList.Count == 0).ToList());
                    }

                }

                #endregion

             
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message + ex.StackTrace);
            }

            return Hotels;
        }




        public List<QaParticipleEntity> GenQuestionWords(string keywords, string newWords = "", string userWordOptions = "")
        {
            var questionWords = new List<QaParticipleEntity>();

            #region UserWordOptions

            if (!string.IsNullOrEmpty(userWordOptions))
            {
                string[] list = userWordOptions.Split(",:".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                List<long> itemIDList = new List<long>();
                for (int i = 1; i < list.Length; i += 3)
                {
                    itemIDList.Add(long.Parse(list[i]));
                }

                List<MagiCallDialogItemsEntity> itemList = MagiCallEngine.GetDialogItemsByIDList(itemIDList);

                foreach (var wItem in itemList)
                {
                    UserWordOptionItem optionItem = Newtonsoft.Json.JsonConvert.DeserializeObject<UserWordOptionItem>(wItem.Params);
                    switch (optionItem.OptionType)
                    {
                        case MagiCallUserWordOptionType.Brand:
                        case MagiCallUserWordOptionType.City:
                        case MagiCallUserWordOptionType.CityAround:
                        case MagiCallUserWordOptionType.POI:
                        case MagiCallUserWordOptionType.Class:
                        case MagiCallUserWordOptionType.Facility:
                        case MagiCallUserWordOptionType.Featured:
                        case MagiCallUserWordOptionType.Other:
                        case MagiCallUserWordOptionType.Theme:
                            foreach (string item in optionItem.ActionParam.Split('|'))
                            {
                                string[] itemDetail = item.Split(",:=".ToArray());
                                string ID = itemDetail[1];
                                string word = itemDetail[3];//{"OptionType":0,"ActionParam":"ID:2,Name:上海","ItemID":0,"DialogID":0,"ItemType":2,"text":"","CreatTime":"2016-03-02T12:19:03.5107793+08:00"}
                                var qaParEntity = new QaParticipleEntity();
                                qaParEntity.Type = (QaWordType)Convert.ToInt32(optionItem.OptionType + 1);
                                qaParEntity.Word = word;
                                qaParEntity.Speech = "";
                                qaParEntity.Id = ID;

                                questionWords.Add(qaParEntity);
                            }
                            break;
                    }
                }
            }

            #endregion

            if ( !string.IsNullOrEmpty(newWords )  || !string.IsNullOrEmpty( keywords))
            {
                #region 自定义分词指定

                if (!string.IsNullOrEmpty(newWords))
                {
                    var wList = newWords.Split(',');
                    foreach (var wItem in wList)
                    {
                        try
                        {
                            if (wItem.Contains(":"))
                            {
                                var wArray = wItem.Split(':');
                                var wType = wArray[0];
                                foreach (string wVal in ParseEngine.SplitWords(wArray[1]))
                                {
                                    QaWordType type = (QaWordType)Convert.ToInt32(wType);
                                    string name = wVal;
                                    if (type == QaWordType.DistrictName && ParseEngine.IsAroundCity(name))
                                    {
                                        type = QaWordType.CityAround;
                                        name = ParseEngine.RemoveAroundWord(name);

                                    }
                                    var qa = GenQaParams(type, name);
                                    if (qa.Id != "")
                                    {
                                        questionWords.Add(qa);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("GenQuestionWords:" + ex.Message);
                        }
                    }
                }

                #endregion

                #region 标准分词分类（在没有自定义分词值时）

                if (keywords.Length > 0)
                {
                    //首先得到基础分词
                    var wList = QaSearchEngine.GetWords(keywords).ToList();

                    //排除一个字的分词
                    wList = wList.Where(p => p.Length > 1).ToList();

                    //查找每一个词的标准库关联词（类似于同义词概念）
                    wList = ExQuestionWords(wList);

                    //根据不同的词库，归类分词数据
                    for (int wNum = 0; wNum < wList.Count; wNum++)
                    {
                        var word = wList[wNum];

                        #region 判断分词的类型归属

                        //var hotelNameWord = "";
                        var haveFilterKey = false;

                        foreach (QaWordType typeItem in Enum.GetValues(typeof(QaWordType)))         
                        {
                            if (typeItem == QaWordType.CityAround) continue;

                            QaParticipleEntity qa =  GenQaParams(typeItem, word);
                            if(qa.Id != "")
                            {
                                if(typeItem == QaWordType.DistrictName)
                                {
                                    if( IsCityAround(wList,wNum) )
                                    {
                                        qa.Type = QaWordType.CityAround;
                                    }
                                }

                                questionWords.Add(qa);
                                haveFilterKey = true;
                            }
                        }

                        //需要追加至酒店名称的搜索
                        //if (!string.IsNullOrEmpty(hotelNameWord))
                        //{
                        //    questionWords.Add(new QaParticipleEntity { Id = "0", Word = hotelNameWord, Type = QaWordType.HotelName });
                        //}
                        //else if (!haveFilterKey)
                        //{
                        //    questionWords.Add(new QaParticipleEntity { Id = "0", Word = word, Type = QaWordType.HotelName });
                        //}

                        #endregion
                    }

                    var dicPOI = ParseEngine.ParsePOIInfo(keywords);
                   if(dicPOI.Count > 0)
                   {
                       foreach(string key in dicPOI.Keys)
                       {
                           questionWords.Add(new QaParticipleEntity { Type = QaWordType.POI, Id = key, Word = dicPOI[key] });
                       }
                   }


                }
            }

                #endregion

            return questionWords;
        }


        /// <summary>
        /// 检查是否为城市周边
        /// 通过判断当前字符后面的第一个或第二个词是不是周边词来确定 
        /// </summary>
        /// <param name="wList"></param>
        /// <param name="wNum"></param>
        /// <returns></returns>
        private bool IsCityAround(List<string> wList, int wNum)
        {
            bool bIsCityAround = false; 

             if( wNum +1 < wList.Count)
             {
                 if(ParseEngine.IsAroundCity(wList[wNum+1]))
                 {
                     bIsCityAround = true;
                 }
             }
             else if (wNum + 2 < wList.Count)
             {
                 if (ParseEngine.IsAroundCity(wList[wNum + 2]))
                 {
                     bIsCityAround = true;
                 }
             }

             return bIsCityAround;
        }

        private QaParticipleEntity GenQaParams(QaWordType type, string wVal)
        {
            var qaParEntity = new QaParticipleEntity();
            qaParEntity.Type = type;
            qaParEntity.Word = wVal;
            qaParEntity.Speech = "";

            if (ParseEngine.DicFilterData.ContainsKey(type))
            {
                //  LogHelper.WriteLog(string.Format("GenQaParams:{0}  {1}", type.ToString(), wVal));
                var dl = ParseEngine.DicFilterData[type].Where(d => d.Name == wVal);
                if (dl.Count() > 0)
                {
                    qaParEntity.Id = dl.First().Id;
                }
            }
            return qaParEntity;
        }

        /// <summary>
        /// 扩展分词的关联词数据
        /// </summary>
        /// <param name="wList"></param>
        /// <returns></returns>
        private List<string> ExQuestionWords(List<string> wList)
        {
            var newList = new List<string>();
            newList.AddRange(wList);

            //获取Qa关联词库
            var qaRelationWords = HotelDistrictCache.GetData<List<QaRelationWordEntity>>("QaRelationWords", () => { return HotelEngine.GetQaRelationWords(); });
            if (qaRelationWords != null && qaRelationWords.Count > 0)
            {
                foreach (var w in wList)
                {
                    var relationList = qaRelationWords.Where(qw => qw.RelWord.ToLower().Trim() == w.ToLower().Trim());
                    if (relationList != null && relationList.Count() > 0)
                    {
                        foreach (var relWord in relationList.ToList())
                        {
                            if (!newList.Contains(relWord.OriWord.Trim()))
                            {
                                newList.Add(relWord.OriWord.Trim());
                            }
                        }
                    }
                }
            }
            else
            {
                System.IO.File.AppendAllText(string.Format(@"D:\Log\AccessService\AccessLog_{0}.txt", DateTime.Now.ToString("MM_dd")), "\r\n=====GetQaRelationWords is null or count = 0");
            }

            return newList;
        }

        /// <summary>
        /// 过滤酒店类型分词
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string FilterHotelClassWord(string word)
        {
            word = word.ToLower().Trim();

            if (word.Contains("大酒店"))
            {
                word = word.Replace("大", "");
            }

            return word;
        }

        #endregion

        #region 搜索处理

        /// <summary>
        /// 提交搜索反馈
        /// </summary>
        /// <param name="fb"></param>
        /// <returns></returns>
        public int SubFeedback(QaFeedback fb)
        {
            return HotelEngine.AddQaFeedback(fb);
        }

        /// <summary>
        /// 记录搜索行为
        /// </summary>
        /// <param name="qsb"></param>
        /// <returns></returns>
        public int RecordSearchBehavior(QaSearchBehavior qsb)
        {
            return HotelEngine.RecordSearchBehavior(qsb);
        }

        #endregion

        #region HotelPriceSlot

        static IModel hps_Channel;
        static IConnection hps_connection;
        static int hps_initChannel = 0;
        static string hps_queueKey = "HotelPriceSlotQueue";

        public IModel GetHpsChannel
        {
            get
            {
                if (hps_initChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = Config.RabbitmqHostName;
                    factory.UserName = Config.RabbitmqUserName;
                    factory.Password = Config.RabbitmqPassword;

                    hps_connection = factory.CreateConnection();
                    hps_Channel = hps_connection.CreateModel();
                    hps_Channel.QueueDeclare(hps_queueKey, false, false, false, null);

                    hps_initChannel = 1;
                }
                return hps_Channel;
            }
        }

        public void AddPriceSlot(int hotelid)
        {
            try
            {
                GetHpsChannel.BasicPublish("", hps_queueKey, null, Encoding.UTF8.GetBytes(hotelid.ToString()));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 读取队列中的行为记录（TEST）

        public void ReaderBehaviorQueue()
        {
            var factory = new ConnectionFactory();
            factory.HostName = Config.RabbitmqHostName;
            factory.UserName = Config.RabbitmqUserName;
            factory.Password = Config.RabbitmqPassword;

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueKey, false, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueKey, true, consumer);

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    while (ea.Body.Length > 0)
                    {
                        try
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);

                        }
                        catch (Exception err)
                        {

                        }

                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    }
                }
            }

        }

        #endregion

        #region 注释代码（记录行为）

        ///// <summary>
        ///// 记录行为
        ///// </summary>
        ///// <param name="behaviorParams"></param>
        //public void RecordBehaviorIndexSearchHotel(BehaviorIndexSearchHotel behaviorParams)
        //{
        //    var behaviorEntity = BehaviorHelper.ConvertBehaviorIndexSearchHotel(behaviorParams);
        //    var behaviorString = BehaviorHelper.FormatBehaviorString(behaviorEntity);
        //    GetChannel.BasicPublish("", queueKey, null, Encoding.UTF8.GetBytes(behaviorString));
        //}

        //public void RecordBehaviorZoneSearchHotel(BehaviorZoneSearchHotel behaviorParams)
        //{
        //    var behaviorEntity = BehaviorHelper.ConvertBehaviorZoneSearchHotel(behaviorParams);
        //    var behaviorString = BehaviorHelper.FormatBehaviorString(behaviorEntity);
        //    GetChannel.BasicPublish("", queueKey, null, Encoding.UTF8.GetBytes(behaviorString));
        //}

        //public void RecordBehaviorHotelSearchHotel(BehaviorHotelSearchHotel behaviorParams)
        //{
        //    var behaviorEntity = BehaviorHelper.ConvertBehaviorHotelSearchHotel(behaviorParams);
        //    var behaviorString = BehaviorHelper.FormatBehaviorString(behaviorEntity);
        //    GetChannel.BasicPublish("", queueKey, null, Encoding.UTF8.GetBytes(behaviorString));
        //}

        //public void RecordBehaviorPackageSearchHotel(BehaviorPackageSearchHotel behaviorParams)
        //{
        //    var behaviorEntity = BehaviorHelper.ConvertBehaviorPackageSearchHotel(behaviorParams);
        //    var behaviorString = BehaviorHelper.FormatBehaviorString(behaviorEntity);
        //    GetChannel.BasicPublish("", queueKey, null, Encoding.UTF8.GetBytes(behaviorString));
        //}

        //public void RecordBehaviorBookSearchHotel(BehaviorBookSearchHotel behaviorParams)
        //{
        //    var behaviorEntity = BehaviorHelper.ConvertBehaviorBookSearchHotel(behaviorParams);
        //    var behaviorString = BehaviorHelper.FormatBehaviorString(behaviorEntity);
        //    GetChannel.BasicPublish("", queueKey, null, Encoding.UTF8.GetBytes(behaviorString));
        //}

        #endregion
    }
}
