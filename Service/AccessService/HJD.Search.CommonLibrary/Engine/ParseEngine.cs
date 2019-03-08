using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Dialog;
using HJD.Framework.Interface;
using HJD.Search.CommonLibrary.Algorithms;
using HJD.Search.CommonLibrary.Parses;
using PanGu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Engine
{
    public class ParseEngine
    {

        private static IMemcacheProvider ParseDataCache = MemcacheManagerFactory.Create("HotelDistrictCache");

        private static string[] AroundKeyWord = "周边,及周边,附近,周围".Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        private static char[] splitTag = ",/:，／：、".ToCharArray();

        public static String []  SplitWords(string words)
        {
            return words.Split(splitTag,  StringSplitOptions.RemoveEmptyEntries);
        }


        private static Dictionary<QaWordType, List<FilterSearchResult>> mDicFilterData = new Dictionary<QaWordType, List<FilterSearchResult>>();

        public static  Dictionary<QaWordType, List<FilterSearchResult>> DicFilterData
        {
            get
            {
                if( mDicFilterData.Count == 0)
                {

                    #region 获取标准类别词库

                    //获取所有的目的地名称
                     mDicFilterData.Add(QaWordType.DistrictName, HotelEngine.GetHotelDistrictList() );
                  //  mDicFilterData.Add(QaWordType.CityAround, mDicFilterData[QaWordType.DistrictName]);

                    //获取所有酒店品牌数据
                     mDicFilterData.Add(QaWordType.Brand, HotelEngine.GetHotelBrandList());
                 
                    //获取所有酒店主题数据
                     mDicFilterData.Add(QaWordType.Themes, HotelEngine.GetHotelInterestList());
                 
                    //获取所有酒店的标签数据
                     mDicFilterData.Add(QaWordType.Featured, HotelEngine.GetFeaturedList());
                  
                    //获取所有酒店的POI数据  
                  //   mDicFilterData.Add(QaWordType.POI, HotelEngine.GetPOIList());
               
                    //获取所有酒店的设施数据
                     mDicFilterData.Add(QaWordType.Facility, HotelEngine.GetFacilityList());
                 
                    //获取所有酒店的类型数据
                     mDicFilterData.Add(QaWordType.Class, HotelEngine.GetClassList());
                 
                    //获取所有的区域名称
                     mDicFilterData.Add(QaWordType.DistrictZone, HotelEngine.GetDistrictZoneList());                   

                    #endregion
                }

                return mDicFilterData;
            }
        }




        private static Dictionary<string, int> mDicDistrict;


        //获取所有的目的地名称
        public static Dictionary<string,int> dicDistrict
        {
            get
            {
                if (mDicDistrict == null)
                {
                    mDicDistrict  = new Dictionary<string, int>();
                    HotelEngine.GetDistrictList().ForEach(d =>
                    {
                        if (!mDicDistrict.ContainsKey(d.Name))
                        {
                            mDicDistrict.Add(d.Name, d.DistrictId);
                        }
                    });
                }

                return mDicDistrict;
                //return ParseDataCache.GetData<Dictionary<string, int>>("Dic_HotelDistrictList", () =>
                //{
                //    Dictionary<string, int> dic = new Dictionary<string, int>();
                //    HotelEngine.GetDistrictList().ForEach(d =>
                //    {
                //        if (!dic.ContainsKey(d.Name))
                //        {
                //            dic.Add(d.Name, d.DistrictId);
                //        }
                //    });
                //    return dic;
                //});
            }
        }

        private static IKeywordsMatchAlgorithms mAlgDistrict;
        public static IKeywordsMatchAlgorithms algDistrict
        {
            get
            {
                if (mAlgDistrict == null)
                {
                    mAlgDistrict = new ACKeywordsMatchAlgorithms();
                    mAlgDistrict.Keywords = dicDistrict.Keys.ToArray();

                }

                return mAlgDistrict;

                //return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelDistrictList", () =>
                //{
                //    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                //    alg.Keywords = dicDistrict.Keys.ToArray();
                //    return alg;
                //});
            }
        }

        private static int  aroundDistance = 300 * 1000;

        private static IKeywordsMatchAlgorithms mAlgAroundDistrictKeyWords;
        public static IKeywordsMatchAlgorithms algAroundDistrict
        {
            get
            {
                if (mAlgAroundDistrictKeyWords == null)
                {
                    mAlgAroundDistrictKeyWords = new ACKeywordsMatchAlgorithms();
                    mAlgAroundDistrictKeyWords.Keywords = AroundKeyWord;

                }

                return mAlgAroundDistrictKeyWords;

                //return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelDistrictList", () =>
                //{
                //    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                //    alg.Keywords = dicDistrict.Keys.ToArray();
                //    return alg;
                //});
            }
        }

        //获取所有酒店品牌数据
        private static Dictionary<string, int> mDicBrand;
        public static Dictionary<string, int> dicBrand
        {
            get
            {
                if (mDicBrand == null)
                {
                    mDicBrand = new Dictionary<string, int>();
                    HotelEngine.GetBrandList().ForEach(d =>
                    {
                        if (!mDicBrand.ContainsKey(d.BrandName))
                        {
                            mDicBrand.Add(d.BrandName, d.Brand);
                        }
                    });
                }

                return mDicBrand;
                //return ParseDataCache.GetData<Dictionary<string, int>>("Dic_HotelBrandList", () =>
                //{
                //    Dictionary<string, int> dic = new Dictionary<string, int>();
                //    HotelEngine.GetBrandList().ForEach(d =>
                //    {
                //        if (!dic.ContainsKey(d.BrandName))
                //        {
                //            dic.Add(d.BrandName, d.Brand);
                //        }
                //    });
                //    return dic;
                //});
            }
        }

        public static IKeywordsMatchAlgorithms mAlgBrand;
        public static IKeywordsMatchAlgorithms algBrand
        {
            get
            {
                if (mAlgBrand == null)
                {
                    mAlgBrand = new ACKeywordsMatchAlgorithms();
                    mAlgBrand.Keywords = dicBrand.Keys.ToArray();
                }

                return mAlgBrand;
                //return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelBrandList", () =>
                //{
                //    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                //    alg.Keywords = dicBrand.Keys.ToArray();
                //    return alg;
                //});
            }
        }



        //获取所有酒店主题数据

        public static Dictionary<string, int> dicTheme
        {
            get
            {

                return ParseDataCache.GetData<Dictionary<string, int>>("Dic_HotelThemeList", () =>
                {
                    Dictionary<string, int> dic = new Dictionary<string, int>();
                    HotelEngine.GetInterestList().ForEach(d =>
                    {
                        if (!dic.ContainsKey(d.Name))
                        {
                            dic.Add(d.Name, d.Id);
                        }
                    });
                    return dic;
                });
            }
        }

        public static IKeywordsMatchAlgorithms algTheme
        {
            get
            {
                return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelThemeList", () =>
                {
                    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                    alg.Keywords = dicTheme.Keys.ToArray();
                    return alg;
                });
            }
        }


        //获取所有酒店的标签数据
        private static Dictionary<string, string> mDicFeatured;
        public static Dictionary<string, string> dicFeatured
        {
            get
            {
                if (mDicFeatured == null)
                {
                    mDicFeatured = new Dictionary<string, string>();
                    HotelEngine.GetFeaturedList().ForEach(d =>
                    {
                        if (!mDicFeatured.ContainsKey(d.Name))
                        {
                            mDicFeatured.Add(d.Name, d.Id);
                        }
                    });
                }
                return mDicFeatured;
                //return ParseDataCache.GetData<Dictionary<string, string>>("Dic_HotelFeaturedList", () =>
                //{
                //    Dictionary<string, string> dic = new Dictionary<string, string>();
                //    HotelEngine.GetFeaturedList().ForEach(d =>
                //    {
                //        if (!dic.ContainsKey(d.Name))
                //        {
                //            dic.Add(d.Name, d.Id);
                //        }
                //    });
                //    return dic;
                //});
            }
        }

        private static IKeywordsMatchAlgorithms mAlgFeatured;
         public static IKeywordsMatchAlgorithms algFeatured
        {
            get
            {
                if (mAlgFeatured == null)
                {
                    mAlgFeatured = new ACKeywordsMatchAlgorithms();
                    mAlgFeatured.Keywords = dicFeatured.Keys.ToArray();
                }

                return mAlgFeatured;
                //return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelFeaturedList", () =>
                //{
                //    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                //    alg.Keywords = dicFeatured.Keys.ToArray();
                //    return alg;
                //});
            }
        }

 //获取所有酒店的POI数据
        private static Dictionary<string, string> mDicPOI;
        public static Dictionary<string, string> dicPOI
        {
            get
            {
                if (mDicPOI == null)
                {
                    mDicPOI = new Dictionary<string, string>();
                    HotelEngine.GetPOIList().ForEach(d =>
                    {
                        if (!mDicPOI.ContainsKey(d.Name))
                        {
                            mDicPOI.Add(d.Name, d.Id);
                        }
                    });
                }
                return mDicPOI;
                //return ParseDataCache.GetData<Dictionary<string, string>>("Dic_HotelPOIList", () =>
                //{
                //    Dictionary<string, string> dic = new Dictionary<string, string>();
                //    HotelEngine.GetPOIList().ForEach(d =>
                //    {
                //        if (!dic.ContainsKey(d.Name))
                //        {
                //            dic.Add(d.Name, d.Id);
                //        }
                //    });
                //    return dic;
                //});
            }
        }

        private static IKeywordsMatchAlgorithms mAlgPOI;
         public static IKeywordsMatchAlgorithms algPOI
        {
            get
            {
                if (mAlgPOI == null)
                {
                    mAlgPOI = new ACKeywordsMatchAlgorithms();
                    mAlgPOI.Keywords = dicPOI.Keys.ToArray();
                }

                return mAlgPOI;
                //return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelPOIList", () =>
                //{
                //    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                //    alg.Keywords = dicPOI.Keys.ToArray();
                //    return alg;
                //});
            }
        }

        //获取所有酒店的标签数据
         public static Dictionary<string, string> mDicFacility;
        public static Dictionary<string, string> dicFacility
        {
            get
            {
                if (mDicFacility == null)
                {
                    mDicFacility = new Dictionary<string, string>();
                    HotelEngine.GetFacilityList().ForEach(d =>
                    {
                        if (!mDicFacility.ContainsKey(d.Name))
                        {
                            mDicFacility.Add(d.Name, d.Id);
                        }
                    });
                }

                return mDicFacility;
                //return ParseDataCache.GetData<Dictionary<string, string>>("Dic_HotelFacilityList", () =>
                //{
                //    Dictionary<string, string> dic = new Dictionary<string, string>();
                //    HotelEngine.GetFacilityList().ForEach(d =>
                //    {
                //        if (!dic.ContainsKey(d.Name))
                //        {
                //            dic.Add(d.Name, d.Id);
                //        }
                //    });
                //    return dic;
                //});
            }
        }

        private static IKeywordsMatchAlgorithms mAlgFacility;
        public static IKeywordsMatchAlgorithms algFacility
        {
            get
            {
                if (mAlgFacility == null )
                {
                    mAlgFacility = new ACKeywordsMatchAlgorithms();
                    mAlgFacility.Keywords = dicFacility.Keys.ToArray();
                }
                return mAlgFacility;
                //return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelFacilityList", () =>
                //{
                //    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                //    alg.Keywords = dicFacility.Keys.ToArray();
                //    return alg;
                //});
            }
        }

        //获取所有酒店的标签数据
        public static List<FilterSearchResult> classList
        {
            get
            {
                //获取所有酒店的类型数据
                return ParseDataCache.GetData<List<FilterSearchResult>>("HotelClassList", () => { return HotelEngine.GetClassList(); });
            }
        }

        //获取所有的区域名称
        private static Dictionary<string, string> mDicZone;
            public static Dictionary<string, string> dicZone
        {
            get
            {
                if (mDicZone == null)
                {
                    mDicZone = new Dictionary<string, string>();
                    HotelEngine.GetDistrictZoneList().ForEach(z =>
                    {
                        if (!mDicZone.ContainsKey(z.Name))
                        {
                            mDicZone.Add(z.Name, z.Id);
                        }
                    });
                }

                return mDicZone;
                //获取所有的区域名称
                //return ParseDataCache.GetData<Dictionary<string, string>>("Dic_HotelDistrictZoneList", () => {
                //    Dictionary<string, string> dic = new Dictionary<string, string>();
                //    HotelEngine.GetDistrictZoneList().ForEach(z =>
                //    {
                //        if (!dic.ContainsKey(z.Name))
                //        {
                //            dic.Add(z.Name, z.Id);
                //        }
                //    });

                //    return dic; });
            }
        }

        public static IKeywordsMatchAlgorithms algZone
        {
            get
            {
                return ParseDataCache.GetData<IKeywordsMatchAlgorithms>("Alg_HotelDistrictZoneList", () =>
                {
                    IKeywordsMatchAlgorithms alg = new ACKeywordsMatchAlgorithms();
                    alg.Keywords = dicZone.Keys.ToArray();
                    return alg;
                });
            }
        }


        public static bool IsAroundCity(string cityName)
        {
            return algAroundDistrict.FindAll(cityName).Length > 0;
        }

        public static List<UserWordOptionItem> ParseUserWord(UserWordItem userWord)
        {
            List<UserWordOptionItem> uwl = new List<UserWordOptionItem>();



            if (userWord.Text.Length > 0)
            {
                //首先得到基础分词
                ICollection<WordInfo> wordInfoList = QaSearchEngine.GetSegment(userWord.Text);


                MoneyParse.GenMoneyOption(uwl, wordInfoList.ToList());

                DateParse.GenDateOption(uwl, wordInfoList);

                UserNumParse.GenUserNumOption(uwl, wordInfoList);


                List<string> wList = wordInfoList.Select(x => x.Word).ToList();

                //查找每一个词的标准库关联词（类似于同义词概念）
                wList = ExWords(wList);

                string segmentedWord = string.Join(" ", wList);
                KeywordsMatchResult[] rlist = null;

                //是否是目的地
                List<UserWordOptionItem> cityOptionList = new List<UserWordOptionItem>();
                rlist = algDistrict.FindAll(segmentedWord);

                KeywordsMatchResult[] aroundList = algAroundDistrict.FindAll(segmentedWord);
                
                if (rlist.Length > 0)
                {
                    foreach (var r in rlist)
                    {
                        if (aroundList.Length == 0)
                        {
                            UserWordOptionItem option = GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.City, r.Keyword,
                                string.Format("ID:{0},Name:{1}", dicDistrict[r.Keyword], r.Keyword).Split(',').ToList());

                            cityOptionList.Add(option);
                        }
                        else
                        {
                            UserWordOptionItem option = GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.CityAround, r.Keyword + aroundList[0].Keyword ,
                               string.Format("ID:{0},Name:{1}", dicDistrict[r.Keyword], r.Keyword).Split(',').ToList());

                            //if (aroundList.Length > 0)
                            //{
                            //    option.Text =  r.Keyword + aroundList[0].Keyword;
                            //    CityParse.GenCityAroundOptionParam(option, dicDistrict[r.Keyword], aroundDistance);
                            //}

                            cityOptionList.Add(option);
                        }
                    }
                }
                
                if(cityOptionList.Count > 0)
                    uwl.AddRange(cityOptionList);

                //是否是主题
                rlist = algTheme.FindAll(segmentedWord);
                if (rlist.Length > 0)
                {
                    foreach (var r in rlist)
                    {
                        uwl.Add(GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.Theme, r.Keyword, string.Format("ID:{0},Name:{1}", dicTheme[r.Keyword], r.Keyword).Split(',').ToList()));

                    }
                }

                //是否是标签
                rlist = algFeatured.FindAll(segmentedWord);
                if (rlist.Length > 0)
                {
                    foreach (var r in rlist)
                    {
                        uwl.Add(GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.Featured, r.Keyword, string.Format("ID:{0},Name:{1}", dicFeatured[r.Keyword], r.Keyword).Split(',').ToList()));

                    }
                }

                //是否是POI
                rlist = algPOI.FindAll(userWord.Text);
                if (rlist.Length > 0)
                {
                    foreach (var r in rlist)
                    {
                        uwl.Add(GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.POI, r.Keyword, string.Format("ID:{0},Name:{1}", dicPOI[r.Keyword], r.Keyword).Split(',').ToList()));

                    }
                }


                //是否是设施
                rlist = algFacility.FindAll(segmentedWord);
                if (rlist.Length > 0)
                {
                    foreach (var r in rlist)
                    {
                        uwl.Add(GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.Facility, r.Keyword, string.Format("ID:{0},Name:{1}", dicFacility[r.Keyword], r.Keyword).Split(',').ToList()));

                    }
                }


                //是否是酒店类型

                //是否是酒店区域
                rlist = algZone.FindAll(segmentedWord);
                if (rlist.Length > 0)
                {
                    foreach (var r in rlist)
                    {
                        uwl.Add(GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.Zone, r.Keyword, string.Format("ID:{0},Name:{1}", dicZone[r.Keyword], r.Keyword).Split(',').ToList()));

                    }
                }

                //是否是酒店品牌
                rlist = algBrand.FindAll(segmentedWord);
                if (rlist.Length > 0)
                {
                    foreach (var r in rlist)
                    {
                        uwl.Add(GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.Brand, r.Keyword, string.Format("ID:{0},Name:{1}", dicBrand[r.Keyword], r.Keyword).Split(',').ToList()));

                    }
                }




                //根据不同的词库，归类分词数据
                for (int wNum = 0; wNum < wList.Count; wNum++)
                {
                    var word = wList[wNum];

                    #region 判断分词的类型归属

                    //是否是酒店类型
                    if (classList.Exists(c => c.Name.ToLower().Trim() == FilterHotelClassWord(word)))
                    {
                        var cObj = classList.Find(c => c.Name.ToLower().Trim() == FilterHotelClassWord(word));
                        uwl.Add(GenOptinItem(userWord.ItemID, MagiCallUserWordOptionType.Class, cObj.Name, string.Format("ID:{0},Name:{1}", cObj.Id, cObj.Name).Split(',').ToList()));
                    }

                    #endregion
                }

            }


            return uwl;

        }

        public static Dictionary<string, string> ParsePOIInfo(string words)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
           var   rlist = algPOI.FindAll(words);
            if (rlist.Length > 0)
            {
                foreach (var r in rlist)
                {
                    dic.Add( dicPOI[r.Keyword], r.Keyword);

                }
            }

            return dic;
        }

        private static UserWordOptionItem GenOptinItem(long realItemID, MagiCallUserWordOptionType optinType, string Text, List<string> parms)
        {
            return new UserWordOptionItem { RelItemID = realItemID, OptionType = optinType, Text=Text, ActionParam = parms==null?"": string.Join(",", parms) };
        }

        /// <summary>
        /// 扩展分词的关联词数据
        /// </summary>
        /// <param name="wList"></param>
        /// <returns></returns>
        private static List<string> ExWords(List<string> wList)
        {
            var newList = new List<string>();
            newList.AddRange(wList);

            //获取Qa关联词库
            var qaRelationWords = ParseDataCache.GetData<List<QaRelationWordEntity>>("QaRelationWords", () => { return HotelEngine.GetQaRelationWords(); });
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
        private static string FilterHotelClassWord(string word)
        {
            word = word.ToLower().Trim();

            if (word.Contains("大酒店"))
            {
                word = word.Replace("大", "");
            }

            return word;
        }

        public static string RemoveAroundWord(string wVal)
        {
            foreach(string word in AroundKeyWord)
            {
                wVal = wVal.Replace(word, "");
            }
            return wVal;
        }
    }
}
