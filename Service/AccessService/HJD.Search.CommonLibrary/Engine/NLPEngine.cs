using HJD.Search.CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using com.hankcs.hanlp;
using java.util;
using com.hankcs.hanlp.suggest;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.tokenizer;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;
using System.Text.RegularExpressions;
using PanGu;
using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;

namespace HJD.Search.CommonLibrary.Engine
{
    public class NLPEngine
    {
        public static PanGuSegment Segment = null;

        /// <summary>
        /// 解释搜索值， 并根据搜索类目返回搜索参数集合
        /// </summary>
        /// <param name="searchTypes">搜索类目</param>
        /// <param name="keywords">搜索值</param>
        /// <returns></returns>
        public static List<SearchData> ExplainKeywords(List<SearchType> searchTypes, string keywords)
        {
            var searchDataList = new List<SearchData>();

            ////首先将搜索内容分词
            //IEnumerable<string> splitKeyWords = SearchEngine.GetWords(keywords);

            //Hanlp分词
            //var keyDic = KeyTokenizerHanlp(keywords);

            //盘古分词
            //...
            var keyDic = KeyTokenizerPangu(keywords);

            //其他分词
            //...

            //根据搜索类目的区别，去给不同的搜索列设置不同的搜索值
            foreach (var searchType in searchTypes)
            {
                switch (searchType)
                {
                        //酒店搜索类目
                    case SearchType.Hotel:
                        #region
                        {

                            /*
                             * HotelName 机构名词 地名 其他专名 未知
                             * Ename 机构名词 其他专名 未知
                             * HotelDesc 机构名词 地名 其他专名 未知
                             * Address 地名
                             * TelePhone 量词 数词
                             * OpenDate 量词 数词
                             * Themes 其他专名 未知
                             */

                            //搜索参数
                            searchDataList.Add(new SearchData { Column = "HotelName", Logic = Logic.or, OperatorType = Operator.Like, Value = (string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nr,nt,nz,nit,nis,unk")) + " " + keywords) });
                            //searchDataList.Add(new SearchData { Column = "Ename", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nt,nz,nit,nis")) });
                            //searchDataList.Add(new SearchData { Column = "PinyinName", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nt,nz,nit,nis")) });
                            searchDataList.Add(new SearchData { Column = "Address", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "ns")) });
                            searchDataList.Add(new SearchData { Column = "Themes", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,nt,nz,unk")) });
                            searchDataList.Add(new SearchData { Column = "Featured", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,nt,nz,unk")) });
                            searchDataList.Add(new SearchData { Column = "DistrictName", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nr,nz")) + " " + keywords });   //^3    

                            //searchDataList.Add(new SearchData { Column = "*", OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nt,nz,nit,nis")) });
                            //searchDataList.Add(new SearchData { Column = "Star", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "m,q")) });        
                            //searchDataList.Add(new SearchData { Column = "HotelDesc", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nt,nz,nit,nis,unk,nr,t,v")) });

                            break;
                        }
                        #endregion
                    case SearchType.QaHotel:
                        #region
                        {

                            /*
                             * HotelName 机构名词 地名 其他专名 未知
                             * Ename 机构名词 其他专名 未知
                             * HotelDesc 机构名词 地名 其他专名 未知
                             * Address 地名
                             * TelePhone 量词 数词
                             * OpenDate 量词 数词
                             * Themes 其他专名 未知
                             */

                            //搜索参数
                            searchDataList.Add(new SearchData { Column = "HotelName", Logic = Logic.or, OperatorType = Operator.Like, Value = (string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nr,nt,nz,nit,nis,unk")) + " " + keywords) });
                            searchDataList.Add(new SearchData { Column = "Address", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "ns")) });
                            searchDataList.Add(new SearchData { Column = "Themes", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,nt,nz,unk")) });
                            searchDataList.Add(new SearchData { Column = "Featured", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,nt,nz,unk")) });
                            searchDataList.Add(new SearchData { Column = "DistrictName", Logic = Logic.or, OperatorType = Operator.Like, Value = string.Join(" ", GetWordListByKeyType(keyDic, "n,ns,nr,nz")) + " " + keywords });

                            break;
                        }
                        #endregion
                    case SearchType.Spot:
                        break;
                    case SearchType.City:
                        break;
                    default:
                        break;
                }

                //暂时处理第一个搜索类目
                break;
            }

            return searchDataList;
        }

        /// <summary>
        /// 根据指定词性参数，返回这些词性下的所有分词总集合
        /// </summary>
        /// <param name="keyDic"></param>
        /// <param name="keytypes"></param>
        /// <returns></returns>
        public static List<string> GetWordListByKeyType(Dictionary<string, List<string>> keyDic, string keytypes)
        {
            var wordList = new List<string>();

            var keyTypeList = Regex.Split(keytypes.ToLower(), ",");
            foreach (var keytype in keyTypeList)
            {
                if (keyDic.ContainsKey(keytype))
                {
                    wordList.AddRange(keyDic[keytype]);
                }
            }

            return wordList;
        }

        #region Qa 分词

        /// <summary>
        /// 解释搜索值， 并根据搜索类目返回搜索参数集合
        /// </summary>
        /// <param name="searchTypes">搜索类目</param>
        /// <param name="keywords">搜索值</param>
        /// <returns></returns>
        public static List<SearchData> QaExplainKeywords(List<SearchType> searchTypes, List<QaParticipleEntity> questionWords)
        {
            var searchDataList = new List<SearchData>();

            //根据搜索类目的区别，去给不同的搜索列设置不同的搜索值
            foreach (var searchType in searchTypes)
            {
                switch (searchType)
                {
                    //酒店搜索类目
                    case SearchType.Hotel:
                        break;
                    case SearchType.QaHotel:
                        #region
                        {
                            var hotelName = "";
                            var districtName = "";
                            var districtZone = "";
                            var facility = "";
                            var featured = "";
                            var poi = "";
                            var themes = "";
                            var classVal = "";
                            var other = "";

                            foreach (var word in questionWords)
                            {
                                switch (word.Type)
                                {
                                    case QaWordType.HotelName:
                                        if (!string.IsNullOrEmpty(hotelName)) hotelName += " "; hotelName += word.Word; break;
                                    case QaWordType.DistrictName:
                                        if (!string.IsNullOrEmpty(districtName)) districtName += " "; districtName += word.Word; break;
                                    case QaWordType.DistrictZone:
                                        if (!string.IsNullOrEmpty(districtZone)) districtZone += " "; districtZone += word.Word; break;
                                    case QaWordType.Facility:
                                        if (!string.IsNullOrEmpty(facility)) facility += " "; facility += word.Word; break;
                                    case QaWordType.Featured:
                                        if (!string.IsNullOrEmpty(featured)) featured += " "; featured += word.Word; break;
                                    case QaWordType.POI:
                                        if (!string.IsNullOrEmpty(poi)) poi += " "; poi += word.Word; break;
                                    case QaWordType.Themes:
                                        if (!string.IsNullOrEmpty(themes)) themes += " "; themes += word.Word; break;
                                    case QaWordType.Class:
                                        if (!string.IsNullOrEmpty(classVal)) classVal += " "; classVal += word.Word; break;
                                    case QaWordType.Other:
                                        if (!string.IsNullOrEmpty(other)) other += " "; other += word.Word; break;
                                    default:
                                        break;
                                }
                            } 

                            //搜索参数
                            if (!string.IsNullOrEmpty(hotelName)) searchDataList.Add(new SearchData { Column = "HotelName", Logic = Logic.or, OperatorType = Operator.Like, Value = hotelName });
                            if (!string.IsNullOrEmpty(districtName)) searchDataList.Add(new SearchData { Column = "Address", Logic = Logic.or, OperatorType = Operator.Like, Value = other });
                            if (!string.IsNullOrEmpty(districtName)) searchDataList.Add(new SearchData { Column = "DistrictName", Logic = Logic.or, OperatorType = Operator.Like, Value = districtName });
                            if (!string.IsNullOrEmpty(districtZone)) searchDataList.Add(new SearchData { Column = "DistrictZone", Logic = Logic.or, OperatorType = Operator.Like, Value = districtZone });

                            if (!string.IsNullOrEmpty(themes)) searchDataList.Add(new SearchData { Column = "Themes", Logic = Logic.or, OperatorType = Operator.Like, Value = themes });
                            if (!string.IsNullOrEmpty(featured)) searchDataList.Add(new SearchData { Column = "Featured", Logic = Logic.or, OperatorType = Operator.Like, Value = featured });
                           // if (!string.IsNullOrEmpty(poi)) searchDataList.Add(new SearchData { Column = "poi", Logic = Logic.or, OperatorType = Operator.Like, Value = poi });
                            if (!string.IsNullOrEmpty(facility)) searchDataList.Add(new SearchData { Column = "Facility", Logic = Logic.or, OperatorType = Operator.Like, Value = facility });
                            if (!string.IsNullOrEmpty(classVal)) searchDataList.Add(new SearchData { Column = "Class", Logic = Logic.or, OperatorType = Operator.Like, Value = classVal });

                            break;
                        }
                        #endregion
                    case SearchType.Spot:
                        break;
                    case SearchType.City:
                        break;
                    default:
                        break;
                }

                //暂时处理第一个搜索类目
                break;
            }

            return searchDataList;
        }

        #endregion

        #region 分词器大全

        /*
            n  名词 取英语名词 noun的第1个字母。
            nr 人名 名词代码 n和“人(ren)”的声母并在一起。
            ns 地名 名词代码 n和处所词代码s并在一起。
            nt 机构团体 “团”的声母为 t，名词代码n和t并在一起。
            nz 其他专名 “专”的声母的第 1个字母为z，名词代码n和z并在一起。
            t  时间词 取英语 time的第1个字母。
            y  语气词 取汉字“语”的声母。
            m  数词 取英语 numeral的第3个字母，n，u已有他用。
            q  量词 取英语 quantity的第1个字母。
            u  助词
            v  动词
        */

        /// <summary>
        /// 【HanLP】将内容分词，返回以词性为Kery的分词集合（该方法是使用HanLP分词，后面可能会根据实际效果切换不同的分词器）
        /// </summary>
        public static Dictionary<string, List<string>> KeyTokenizerHanlp(string keywords)
        {
            var keyDic = new Dictionary<string, List<string>>();
            
            List list = StandardTokenizer.segment(keywords);
            for (int i = 0; i < list.size(); i++)
            {
                var term = (Term)list.get(i);
                var keyStr = term.nature.toString().Replace(" ","").ToLower();
                var keyArr = Regex.Split(keyStr, ",");
                foreach (var key in keyArr)
                {
                    if (!keyDic.ContainsKey(key))
                    {
                        keyDic[key] = new List<string> { term.word };
                    }
                    else
                    {
                        keyDic[key].Add(term.word);
                    }   
                }
            }

            return keyDic;
        }

        /// <summary>
        /// 【PanGu】将内容分词，返回以词性为Kery的分词集合（该方法是使用PanGu盘古分词）
        /// </summary>
        public static Dictionary<string, List<string>> KeyTokenizerPangu(string keywords)
        {
            var keyDic = new Dictionary<string, List<string>>();

            if (Segment == null) Segment = new PanGuSegment();

            var wic = Segment.GetSegment(keywords).ToList();
            for (int i = 0; i < wic.Count; i++)
            {
                var wordInfo = wic[i];
                var keyStr = wordInfo.Pos.ToString().Replace(" ", "").ToLower();
                var keyArr = Regex.Split(keyStr, ",");
                foreach (var key in keyArr)
                {
                    if (!keyDic.ContainsKey(key))
                    {
                        keyDic[key] = new List<string> { wordInfo.Word };
                    }
                    else
                    {
                        keyDic[key].Add(wordInfo.Word);
                    }
                }
            }

            return keyDic;
        }

        #endregion

        #region Test Demo

        public static void HanlpDemo(string keywords)
        {
            // 动态增加
            //CustomDictionary.add("凯宾斯基","ntch 1");

            //凯宾斯基度假酒店 nt 1

            List termList2 = StandardTokenizer.segment(keywords);

            List termList3 = StandardTokenizer.segment("万豪酒店周末有房吗");

            //var a = ((com.hankcs.hanlp.seg.common.Term)((termList2 as System.Collections.IEnumerable).Items[3])).word;
        }

        #endregion
    }
}
