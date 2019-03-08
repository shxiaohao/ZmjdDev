using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.Search.CommonLibrary.Config;
using HJD.Search.CommonLibrary.Helper;
using HJD.Search.CommonLibrary.Model;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Engine
{
    public class IndexEngine
    {
        #region 单例对象

        private static IndexEngine m_Instance = null;
        internal static IndexEngine GetInstance(SearchType t)
        {
            if (m_Instance == null)
                m_Instance = new IndexEngine(t);
            return m_Instance;
        }

        //对象函数
        public IndexEngine(SearchType t)
        {
            InitIndexParams(t);
        }

        #endregion

        #region 成员

        private static IndexWriter writer;

        private static Lucene.Net.Analysis.Analyzer analyzer;

        private static SearchType type;

        private static string indexPath;

        #endregion

        #region 索引参数配置

        /// <summary>
        /// 初始索引
        /// </summary>
        private void InitIndexParams(SearchType t)
        {
            type = t;

            //根据不同的索引类型，来获取相关参数（索引库地址）
            indexPath = IndexConfig.GetIndexPathByType(t);

            Init(indexPath);
        }

        private void Init(string indexPath)
        {
            analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(indexPath);

            var exsit = !Lucene.Net.Index.IndexReader.IndexExists(FSDirectory.Open(info));

            writer = new IndexWriter(FSDirectory.Open(info), analyzer, exsit, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        #endregion

        #region 索引操作

        /// <summary>
        /// 写入索引
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public IEnumerable<int> WriteIndex(IEnumerable<DocumentData> dataList)
        {
            int index = 0;

            foreach (var item in dataList)
            {
                AddDocument(item);

                yield return index++;
            }

            CloseWrite();
        }

        private void AddDocument(DocumentData t)
        {
            Document document = new Document();

            foreach (var item in t.FieldDataList)
            {
                AbstractField field = null;
                if (item.DataType == DataTypes.Int)
                {
                    //field = new NumericField(item.Column, Field.Store.YES, true).SetIntValue(int.Parse(item.Value));
                    field = new Field(item.Column, item.Value, Field.Store.YES, Field.Index.ANALYZED);
                }
                else
                {
                    field = new Field(item.Column, item.Value, item.IsStore ? Field.Store.YES : Field.Store.NO, Field.Index.ANALYZED);
                }

                field.Boost = item.Weight;

                document.Add(field);

                if (item.IsSort)
                {
                    document.Add(new Field(item.Column + "_sort", item.Value, Field.Store.NO, Field.Index.NOT_ANALYZED));
                }
            }

            document.Boost = t.Boost;

            writer.AddDocument(document);
        }

        private void CloseWrite()
        {
            writer.Optimize();
            writer.Dispose();
        }

        //索引的删除/修改操作(根据索引任务)
        public void ProcessIndexJob(IndexJob indexJob)
        {
            writer.DeleteDocuments(new Term("Id", indexJob.Id.ToString()));

            //添加
            if (indexJob.JobType == IndexJobType.Add)
            {
                var docs = GetIndexDocsById(indexJob.IndexType, indexJob.Id);
                if (docs != null)
                {
                    foreach (var doc in docs)
                    {
                        AddDocument(doc);
                    }
                }
            }

            writer.Commit();
            CloseWrite();
        }

        #endregion

        #region Document 相关操作

        /// <summary>
        /// 根据索引类型及相关id，获取索引Doc对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<DocumentData> GetIndexDocsById(SearchType type, string id)
        {
            var docList = new List<DocumentData>();

            switch (type)
            {
                case SearchType.Hotel:
                    #region
                    {
                        var hotelIndexResult = new HotelIndexResult();

                        //id 为空/null 或者 0 的时候，获取所有酒店数据
                        if (string.IsNullOrEmpty(id) || id == "0")
                        {
                            hotelIndexResult = HotelEngine.GenHotelIndexResult();
                        }
                        else
                        {
                            hotelIndexResult = HotelEngine.GenHotelIndexResult(id);
                        }

                        //将酒店对象转换为索引对象
                        foreach (var hotelEntity in hotelIndexResult.HotelEntityList)
                        {
                            var docData = new DocumentData();
                            var objs = new List<FieldData>();
                            objs.Add(new FieldData { Column = "Id", Value = hotelEntity.Id.ToString(), DataType = DataTypes.Int, IsSort = false, IsStore = true, Weight = 1F });
                            objs.Add(new FieldData { Column = "HotelName", Value = hotelEntity.HotelName, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });
                            objs.Add(new FieldData { Column = "HotelName2", Value = hotelEntity.HotelName, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });
                            objs.Add(new FieldData { Column = "PinyinName", Value = hotelEntity.PinyinName, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 3F });
                            objs.Add(new FieldData { Column = "Ename", Value = hotelEntity.Ename, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1F });
                            objs.Add(new FieldData { Column = "Address", Value = hotelEntity.Address, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 0.5F });
                            objs.Add(new FieldData { Column = "DistrictName", Value = hotelEntity.DistrictName, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });
                            objs.Add(new FieldData { Column = "ReviewScore", Value = hotelEntity.ReviewScore.ToString(), DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1F });
                            objs.Add(new FieldData { Column = "Star", Value = hotelEntity.Star, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1F });

                            objs.Add(new FieldData { Column = "Themes", Value = hotelEntity.Themes, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1.5F });
                            objs.Add(new FieldData { Column = "Featured", Value = hotelEntity.Featured, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1.5F });

                            //objs.Add(new FieldData { Column = "HotelDesc", Value = hotelEntity.HotelDesc, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 0.8F });
                            docData.FieldDataList = objs;

                            //控制权重
                            ChangeBoost(type, docData, hotelEntity);

                            docList.Add(docData);
                        }

                        break;
                    }
                    #endregion
                case SearchType.QaHotel:
                    #region
                    {
                        var hotelIndexResult = new HotelIndexResult();

                        //id 为空/null 或者 0 的时候，获取所有酒店数据
                        if (string.IsNullOrEmpty(id) || id == "0")
                        {
                            hotelIndexResult = HotelEngine.GenHotelIndexResult();
                        }
                        else
                        {
                            hotelIndexResult = HotelEngine.GenHotelIndexResult(id);
                        }

                        LogHelper.WriteConsole("BeginBuild");
                        //将酒店对象转换为索引对象
                        foreach (var hotelEntity in hotelIndexResult.HotelEntityList)
                        {
                            var docData = new DocumentData();
                            var objs = new List<FieldData>();
                            objs.Add(new FieldData { Column = "Id", Value = hotelEntity.Id.ToString(), DataType = DataTypes.Int, IsSort = false, IsStore = true, Weight = 1F });
                            
                            //酒店名称
                            objs.Add(new FieldData { Column = "HotelName", Value = hotelEntity.HotelName, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });

                            //酒店名称
                            objs.Add(new FieldData { Column = "EnName", Value = hotelEntity.Ename, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 10F });
                            
                            //详细地址
                            objs.Add(new FieldData { Column = "Address", Value = hotelEntity.Address, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1F });
                            
                            //目的地
                            objs.Add(new FieldData { Column = "DistrictName", Value = hotelEntity.DistrictName, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 100F });
                            
                            //点评得分
                            objs.Add(new FieldData { Column = "ReviewScore", Value = hotelEntity.ReviewScore.ToString(), DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1F });

                            //星级
                            objs.Add(new FieldData { Column = "Star", Value = hotelEntity.Star, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 1F });

                            //主题
                            objs.Add(new FieldData { Column = "Themes", Value = hotelEntity.Themes, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });

                            //标签
                            objs.Add(new FieldData { Column = "Featured", Value = hotelEntity.Featured, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });

                            //设施
                            objs.Add(new FieldData { Column = "Facility", Value = hotelEntity.Facility, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });

                            //酒店类型
                            objs.Add(new FieldData { Column = "Class", Value = hotelEntity.Class, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });

                            //酒店区域
                            objs.Add(new FieldData { Column = "DistrictZone", Value = hotelEntity.DistrictZone, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });

                            //酒店POI
                            objs.Add(new FieldData { Column = "POI", Value = hotelEntity.POI, DataType = DataTypes.String, IsSort = false, IsStore = false, Weight = 20F });
                          
                            docData.FieldDataList = objs;

                            docList.Add(docData);
                        }
                        LogHelper.WriteConsole("EndBuild");
                        break;
                    }
                    #endregion
                case SearchType.City:
                    break;
                case SearchType.Spot:
                    break;
                default:
                    break;
            }


            return docList;
        }

        /// <summary>
        /// 控制权重
        /// </summary>
        /// <param name="docData"></param>
        /// <param name="hotelEntity"></param>
        private static void ChangeBoost(SearchType type, DocumentData docData, HotelIndexResultEntity hotelEntity)
        {
            switch (type)
            {
                case SearchType.Hotel:
                    {
                        #region 点评评分影响权重

                        if (hotelEntity.ReviewScore > 3.0)
                        {
                            docData.Boost += (float)(hotelEntity.ReviewScore - 3);
                        }

                        #endregion

                        #region 酒店类型影响权重（1直采酒店 2精选 other）

                        //直采酒店
                        if (hotelEntity.HotelPkHid > 0)
                        {
                            docData.Boost += 2;
                        }

                        //精选酒店
                        if (hotelEntity.HotelState == 2)
                        {
                            docData.Boost += 1;
                        }

                        #endregion

                        //if (hotelEntity.HotelName.Contains("西安秦风唐韵酒店"))
                        //{
                        //    docData.Boost += 2;
                        //}

                        break; 
                    }
                case SearchType.City:
                    break;
                case SearchType.Spot:
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
