using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Hotel;
using HJD.Search.CommonLibrary.Helper;
using HJD.Search.CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Engine
{
    public class HotelEngine
    {
        /// <summary>
        /// 根据酒店ID获取酒店相关信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<HotelSearchResult> GetHotelSearchResult(List<BaseSearchResult> baseList)
        {
            var list = new List<HotelSearchResult>();

            //所有的酒店ID集合
            var ids = baseList.Select(l => l.Id).ToList();
            if (ids.Count == 0) return list;

            var sql = @"
select h1.HotelId,h1.HotelName,h1.Ename,h1.Star,h2.HotelDesc,h2.Address,hs.ReviewScore
from hotel h1 (nolock) 
left join hotel2 h2  (nolock) on h1.HotelID = h2.HotelID
left join HotelContacts hc  (nolock) on hc.HotelID = h1.HotelId
left join HotelStat hs (nolock)  on hs.HotelID = h1.HotelId
where h1.hotelid in ({0}) ";

            sql = string.Format(sql, string.Join(",", ids));

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    var hotel = new HotelSearchResult()
                    {
                        HotelId = Convert.ToInt32(reader["HotelId"].ToString()),
                        HotelName = reader["HotelName"].ToString(),
                        Ename = reader["Ename"].ToString(),
                        Address = reader["Address"].ToString(),
                        HotelDesc = reader["HotelDesc"].ToString(),
                        Star = reader["Star"].ToString(),
                        Themes = GenHotelInterest(Convert.ToInt32(reader["HotelId"].ToString())),
                        ReviewScore = reader["ReviewScore"] != null && !string.IsNullOrEmpty(reader["ReviewScore"].ToString()) ? Convert.ToDouble(reader["ReviewScore"].ToString()) : 0.0
                    };

                    var baseResult = baseList.Find(d => d.Id == hotel.HotelId.ToString());
                    if (baseResult != null) hotel.LoadBase(baseResult);

                    list.Add(hotel);
                }
            }

            return list.OrderByDescending(l => l.Boost).ToList();
        }

        /// <summary>
        /// 根据酒店ID列表获取主题数据
        /// </summary>
        /// <param name="baseList"></param>
        /// <returns></returns>
        public static List<FilterSearchResult> GetInterestResultByHotelIds(List<BaseSearchResult> baseList)
        {
            var list = new List<FilterSearchResult>();

            //所有的酒店ID集合
            var ids = baseList.Select(l => l.Id).ToList();
            if (ids.Count == 0) return list;

            var sql = @"
select distinct i.ID,i.Name from interesthotelrel ihr  (nolock) 
inner join interestplace ip  (nolock) on ip.id = ihr.ipid 
inner join interest i (nolock)  on i.id = ip.iid 
where ihr.state > 10 and i.released = 1 and i.type = 1
and ihr.HID in ({0}) ";

            sql = string.Format(sql, string.Join(",", ids));

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    var hotel = new FilterSearchResult()
                    {
                        Id = reader["ID"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.Themes
                    };

                    list.Add(hotel);
                }
            }

            return list;
        }

        public static List<FilterSearchResult> GetFeaturedResultByHotelIds(List<BaseSearchResult> baseList)
        {
            var list = new List<FilterSearchResult>();

            //所有的酒店ID集合
            var ids = baseList.Select(l => l.Id).ToList();
            if (ids.Count == 0) return list;

            var sql = @"
select distinct ft.id,ft.[Name] from HotelFilter hf (nolock)
inner join dbo.fn_split( '{0}',',') i on i.item = hf.hotelid 
inner JOIN FeaturedTree ft (NOLOCK) ON  230000000000 + ft.ID = hf.FilterCol ";

            sql = string.Format(sql, string.Join(",", ids));

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    var hotel = new FilterSearchResult()
                    {
                        Id = reader["ID"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.Featured
                    };

                    list.Add(hotel);
                }
            }

            return list;
        }

        /// <summary>
        /// 根据酒店ID列表获取酒店类型数据
        /// </summary>
        /// <param name="baseList"></param>
        /// <returns></returns>
        public static List<FilterSearchResult> GetClassResultByHotelIds(List<BaseSearchResult> baseList)
        {
            var list = new List<FilterSearchResult>();

            //所有的酒店ID集合
            var ids = baseList.Select(l => l.Id).ToList();
            if (ids.Count == 0) return list;

            var sql = @"
select distinct ClassType,Name from [HotelBizDB].[dbo].[HotelClass2Define]  (nolock) where ClassType in
(select top 1000 (FilterCol - 10000000000 * 21) from HotelFilter  (nolock) where HotelID in ({0}) and FilterCol like '21000%')";

            sql = string.Format(sql, string.Join(",", ids));

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    var hotel = new FilterSearchResult()
                    {
                        Id = reader["ClassType"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.Class
                    };

                    list.Add(hotel);
                }
            }

            return list;
        }

        /// <summary>
        /// 根据酒店ID列表获取酒店设施数据
        /// </summary>
        /// <param name="baseList"></param>
        /// <returns></returns>
        public static List<FilterSearchResult> GetFacilityResultByHotelIds(List<BaseSearchResult> baseList)
        {
            var list = new List<FilterSearchResult>();

            //所有的酒店ID集合
            var ids = baseList.Select(l => l.Id).ToList();
            if (ids.Count == 0) return list;

            var sql = @"
select distinct ID,Name from FeaturedTree where Id in
(select top 1000 (FilterCol - 10000000000 * 19) from HotelFilter (nolock)  where HotelID in ({0}) and FilterCol like '19000%')";

            sql = string.Format(sql, string.Join(",", ids));

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    var hotel = new FilterSearchResult()
                    {
                        Id = reader["ID"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.Facility
                    };

                    list.Add(hotel);
                }
            }

            return list;
        }

        public static List<FilterSearchResult> GetHotelDistrictList()
        {
            return GetDistrictList().Select(d => new FilterSearchResult { Id = d.DistrictId.ToString(), Name = d.Name, Type = AccessService.Contract.QaWordType.DistrictName }).ToList();
        }
        /// <summary>
        /// 获取目的地集合
        /// </summary>
        /// <returns></returns>
        public static List<DistrictInfoEntity> GetDistrictList()
        {
            var list = new List<DistrictInfoEntity>();

            var sql = @"select distinct DistrictID,Name,EName,isnull(DistrictType,0) DistrictType from DestDB.dbo.DistrictInfo (nolock)  where Released = 1";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.DestDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new DistrictInfoEntity 
                    {
                        DistrictId = Convert.ToInt32(reader["DistrictID"].ToString()),
                        Name = reader["Name"].ToString().Trim(),
                        EName = reader["EName"].ToString(),
                        DistrictType = Convert.ToInt32(reader["DistrictType"].ToString())
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 获取品牌集合
        /// </summary>
        /// <returns></returns>
        public static List<BrandEntity> GetBrandList()
        {
            var list = new List<BrandEntity>();

            var sql = @"
select distinct Brand,BrandName,BrandEname,[Group],BrandType from
(
select distinct Brand,BrandName,BrandEname,[Group],BrandType from Hoteldb.dbo.Brand (nolock) 
union
select distinct 0 Brand,[Group] BrandName,GroupEname BrandEname,[Group],'' BrandType from Hoteldb.dbo.Brand (nolock) 
)
brands
where
BrandName <> ''";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new BrandEntity 
                    {
                        Brand = Convert.ToInt32(reader["Brand"].ToString()),
                        BrandName = reader["BrandName"].ToString(),
                        BrandEname = reader["BrandEname"].ToString(),
                        Group = reader["Group"].ToString(),
                        BrandType = reader["BrandType"].ToString(),
                    });
                }
            }

            return list;
        }

        public static List<FilterSearchResult> GetHotelInterestList()
        {
            return GetInterestList().Select(i => new FilterSearchResult { Name = i.Name, Id = i.Id.ToString(), Type = AccessService.Contract.QaWordType.Themes }).ToList();
        }

        /// <summary>
        /// 获取酒店主题集合
        /// </summary>
        /// <returns></returns>
        public static List<InterestInfoEntity> GetInterestList()
        {
            var list = new List<InterestInfoEntity>();

            var sql = @"select ID,Name,Type,released from HotelDB.dbo.Interest i  (nolock) where i.Type = 1 and i.released = 1 ";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new InterestInfoEntity 
                    {
                        Id = Convert.ToInt32(reader["ID"].ToString()),
                        Name = reader["Name"].ToString().Trim(),
                        Type = Convert.ToInt32(reader["Type"].ToString()),
                        Released = Convert.ToBoolean(reader["released"].ToString())
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 目的地+对应酒店数量的集合
        /// </summary>
        /// <returns></returns>
        public static List<DistrictRelationInfoEntity> GetDistrictHotelsList()
        {
            var list = new List<DistrictRelationInfoEntity>();

            var sql = @"
select di.districtid,COUNT(1) HotelCount from 
DestDB.dbo.DistrictInfo di  (nolock) 
inner join Hoteldb.dbo.Hotel h (nolock)  on di.DistrictID = h.DistrictID
inner join hoteldb.dbo.HotelState hs (nolock)  on h.HotelID = hs.HotelID and hs.State = 1
where h.Enabled = 'T' and di.released = 1 
group by di.districtid
";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new DistrictRelationInfoEntity
                    {
                        DistrictId = Convert.ToInt32(reader["districtid"].ToString()),
                        HotelCount = Convert.ToInt32(reader["HotelCount"].ToString())
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 目的地+酒店主题的集合
        /// </summary>
        /// <returns></returns>
        public static List<DistrictRelationInfoEntity> GetDistrictInterestList()
        {
            var list = new List<DistrictRelationInfoEntity>();

            var sql = @"
select ip.districtid,i.ID from HotelDB.dbo.Interest i  (nolock) 
inner join Hoteldb.dbo.InterestPlace ip (nolock)  on i.ID = ip.IID 
inner join Hoteldb.dbo.InterestHotelRel ihr (nolock)  on ihr.IPID = ip.ID 
inner join hoteldb.dbo.HotelState hs (nolock)  on hs.HotelID = ihr.HID and hs.State = 2
where i.Type = 1 and i.released = 1 
group by ip.districtid,i.ID
";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new DistrictRelationInfoEntity
                    {
                        DistrictId = Convert.ToInt32(reader["districtid"].ToString()),
                        InterestId = Convert.ToInt32(reader["ID"].ToString())
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 获取酒店标签集合
        /// </summary>
        /// <returns></returns>
        public static List<FilterSearchResult> GetFeaturedList()
        {
            var list = new List<FilterSearchResult>();
            //要去掉设施标签
            var sql = @"
select distinct ft.id,ft.name 
FROM HotelDB.dbo.FeaturedTree ft  (nolock) 
left join ( select distinct FeaturedID from  HotelDB.dbo.FeaturedCategoryRel (nolock) where  CategoryID = 152) fcr on fcr.FeaturedID = ft.ID 
where  fcr.FeaturedID is null  ";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new FilterSearchResult
                    {
                        Id = reader["id"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.Featured
                    });
                }
            }

            return list;
        }


        public static List<FilterSearchResult> GetPOIList()
        {
            var list = new List<FilterSearchResult>();

            var sql = @"
select  distinct p.ID, p.Name
from DestDB.dbo.POIForNearby p (nolock) 
where detailCategoryID in (3,4,5,7,8,9,10)";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new FilterSearchResult
                    {
                        Id = reader["id"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.POI
                    });
                }
            }

            return list;
        }


        /// <summary>
        /// 获取酒店设施集合
        /// </summary>
        /// <returns></returns>
        public static List<FilterSearchResult> GetFacilityList()
        {
            var list = new List<FilterSearchResult>();

            var sql = @" select ID, Name 
 from HotelDB.dbo.FeaturedTree ft  (nolock)
 inner join HotelDB.dbo.FeaturedCategoryRel fcr (nolock) on fcr.FeaturedID = ft.ID 
 where fcr.CategoryID = 152 ";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new FilterSearchResult
                    {
                        Id = reader["ID"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.Facility
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 获取酒店类型集合
        /// </summary>
        /// <returns></returns>
        public static List<FilterSearchResult> GetClassList()
        {
            var list = new List<FilterSearchResult>();

            var sql = @"select ClassType,Name from [HotelBizDB].[dbo].[HotelClass2Define] (nolock) ";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new FilterSearchResult
                    {
                        Id = reader["ClassType"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.Class
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 获取酒店区域集合
        /// </summary>
        /// <returns></returns>
        public static List<QaRelationWordEntity> GetQaRelationWords()
        {
            var list = new List<QaRelationWordEntity>();

            var sql = @"select Id,OriWord,RelWord,Type from CommDB.dbo.QaRelationWords (nolock)  order by OriWord";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new QaRelationWordEntity
                    {
                        Id = Convert.ToInt64(reader["Id"].ToString()),
                        OriWord = reader["OriWord"].ToString(),
                        RelWord = reader["RelWord"].ToString(),
                        Type = Convert.ToInt32(reader["Type"].ToString())
                    });
                }
            }

            return list;
        }        
        
        /// <summary>
        /// 获取酒店区域集合
        /// </summary>
        /// <returns></returns>
        public static List<FilterSearchResult> GetDistrictZoneList()
        {
            var list = new List<FilterSearchResult>();

            var sql = @"select distinct ID,Name,* from DestDB..DistrictZone (nolock) where TYPE>1 and State=1 ";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new FilterSearchResult
                    {
                        Id = reader["ID"].ToString(),
                        Name = reader["Name"].ToString().Trim(),
                        Type = AccessService.Contract.QaWordType.DistrictZone
                    });
                }
            }

            return list;
        }  
        
        /// <summary>
        /// 获取品牌集合
        /// </summary>
        /// <returns></returns>
        public static List<FilterSearchResult> GetHotelBrandList()
        {
            var list = new List<FilterSearchResult>();

            var sql = @"select distinct Brand,BrandName  from Hoteldb.dbo.BrandList (nolock)  ";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new FilterSearchResult
                    {
                        Id = reader["Brand"].ToString(),
                        Name = reader["BrandName"].ToString(),
                        Type = AccessService.Contract.QaWordType.Brand
                    });
                }
            }

            return list;
        }

        public static List<HotelPriceSlot> GenHotelPriceSlots(List<int> hotelIDList, DateTime checkIn, DateTime checkOut, string slotColName)
        {
            var list = new List<HotelPriceSlot>();

            if (hotelIDList.Count == 0) return list;

            var sql = @"
select Id,HotelId,Night,MinPrice,MaxPrice,ChannelId,Prices,SellState 
from HotelPriceSlot  (nolock) 
where HotelId in ({0}) and Night >= '{1}' and Night <= '{2}' and {3} = 1
order by Night
";

            sql = string.Format(sql, string.Join(",", hotelIDList), checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"), slotColName);

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var hotelSlot = new HotelPriceSlot()
                    {
                        Id = Convert.ToInt64(reader["Id"].ToString()),
                        HotelId = Convert.ToInt32(reader["HotelId"].ToString()),
                        Night = DateTime.Parse(reader["Night"].ToString()),
                        MinPrice = Convert.ToInt32(reader["MinPrice"].ToString()),
                        MaxPrice = Convert.ToInt32(reader["MaxPrice"].ToString()),
                        ChannelId = Convert.ToInt32(reader["ChannelId"].ToString()),
                        Prices = reader["Prices"].ToString(),
                        SellState = Convert.ToInt32(reader["SellState"].ToString())
                    };

                    list.Add(hotelSlot);
                }
            }

            return list;
        }

        #region 搜索反馈相关

        /// <summary>
        /// 获取所有的搜索反馈数据
        /// </summary>
        /// <returns></returns>
        public static List<QaFeedback> GetQaFeedbacks()
        {
            var list = new List<QaFeedback>();

            var sql = @"select ID,Question,QuestionUrl,AnswerCount,Feedback,Sure,CreateTime,CreateBy from CommDB.dbo.QaFeedback order by CreateTime desc";

            using (var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(new QaFeedback
                    {
                        Id = Convert.ToInt32(reader["ID"].ToString()),
                        Question = reader["Question"].ToString(),
                        QuestionUrl = reader["QuestionUrl"].ToString(),
                        AnswerCount = Convert.ToInt32(reader["AnswerCount"].ToString()),
                        Feedback = reader["Feedback"].ToString(),
                        Sure = Convert.ToInt32(reader["Sure"].ToString()),
                        CreateTime = DateTime.Parse(reader["CreateTime"].ToString()),
                        CreateBy = reader["CreateBy"].ToString()
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 添加搜索反馈
        /// </summary>
        /// <param name="fb"></param>
        /// <returns></returns>
        public static int AddQaFeedback(QaFeedback fb)
        {
            var add = 0;

            var sql = @"
insert into CommDB.dbo.QaFeedback (Question,QuestionUrl,AnswerCount,Feedback,Sure,CreateTime,CreateBy) 
values
('{0}','{1}','{2}','{3}','{4}','{5}','{6}')
";
            sql = string.Format(sql, fb.Question,fb.QuestionUrl,fb.AnswerCount,fb.Feedback,fb.Sure,fb.CreateTime,fb.CreateBy);

            add = SqlHelper.ExecuteNonQuery(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);

            return add;
        }

        public static int RecordSearchBehavior(QaSearchBehavior qsb)
        {
            var add = 0;

            var sql = @"
insert into CommDB.dbo.QaSearchBehavior (ParentQuestion,Question,QuestionUrl,AnswerCount,CreateTime,CreateBy) 
values
('{0}','{1}','{2}','{3}','{4}','{5}')
";
            sql = string.Format(sql, qsb.ParentQuestion, qsb.Question, qsb.QuestionUrl, qsb.AnswerCount, qsb.CreateTime, qsb.CreateBy);

            add = SqlHelper.ExecuteNonQuery(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);

            return add;
        }

        #endregion

        #region 索引库数据准备

        /// <summary>
        /// 从数据库读取酒店索引库的源数据
        /// </summary>
        /// <returns></returns>
        public static HotelIndexResult GenHotelIndexResult()
        {
            return GenHotelIndexResult(null);
        }
        public static HotelIndexResult GenHotelIndexResult(string id)
        {
            var hotelIndexResult = new HotelIndexResult();

            var sql = string.Format(@"
select h1.HotelId id,h1.HotelName,h1.Ename,h1.Star,h2.Address,(dif.Name+' '+dif.EName) DistrictName,hs.ReviewScore
from 
(select distinct HotelId as hid 
from HotelState  (nolock) 
where State = 1
) i1
left join hotel h1 (nolock)  on h1.HotelId = i1.hid
left join hotel2 h2  (nolock) on h1.HotelID = h2.HotelID
left join HotelContacts hc (nolock)  on hc.HotelID = h1.HotelId
left join DestDB.dbo.DistrictInfo dif (nolock)  on dif.DistrictID = h1.DistrictID
inner join destdb.dbo.DistrictDirectory dd (nolock)  on dd.DistrictID = h1.DistrictID
left join HotelStat hs (nolock)  on hs.HotelID = h1.HotelId
where h1.Enabled = 'T' 
");
            sql = @"
select h1.HotelId id,h1.HotelName,h1.Ename,h1.Star,h2.Address,(dif.Name+' '+dif.EName) DistrictName,hs.ReviewScore, isnull(hst.State,0) HotelState, ISNULL(pk.HotelID,0) pkhid
from hotel h1 (nolock)
left join hotel2 h2  (nolock) on h1.HotelID = h2.HotelID
left join HotelContacts hc (nolock)  on hc.HotelID = h1.HotelId
left join DestDB.dbo.DistrictInfo dif (nolock)  on dif.DistrictID = h1.DistrictID
inner join destdb.dbo.DistrictDirectory dd (nolock)  on dd.DistrictID = h1.DistrictID
left join HotelStat hs (nolock) on hs.HotelID = h1.HotelId
left join HotelState hst (nolock) on hst.State = 2 and hst.HotelID = h1.HotelId
left join (select distinct HotelID from Package (nolock) where isValid = 1) pk on pk.HotelID = h1.HotelId
where h1.Enabled = 'T' 
";

            if (!string.IsNullOrEmpty(id))
            {
                sql += string.Format(@" and h1.HotelId in ({0})", id);
            }
            else
            {
                sql += string.Format(@" and dd.InChina = 1", id);
            }

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);


            using (reader)
            {
             
                while (reader.Read())
                {
                    if (reader["id"] == null || string.IsNullOrEmpty(reader["id"].ToString())) continue;

                    var hotelEntity = new HotelIndexResultEntity
                    {
                        Id = Convert.ToInt32(reader["id"].ToString()),
                        HotelName = reader["HotelName"].ToString(),
                        Ename = reader["Ename"].ToString(),
                        Address = reader["Address"].ToString(),
                        DistrictName = reader["DistrictName"].ToString(),
                        Star = reader["Star"].ToString(),
                        ReviewScore = reader["ReviewScore"] != null && !string.IsNullOrEmpty(reader["ReviewScore"].ToString()) ? Convert.ToDouble(reader["ReviewScore"].ToString()) : 0.0,
                        HotelState = Convert.ToInt32(reader["HotelState"].ToString()),
                        HotelPkHid = Convert.ToInt32(reader["pkhid"].ToString())
                    };

                    LogHelper.WriteConsole("GetHotelInfo:" + hotelEntity.Id.ToString());

                    //酒店名称转换拼音名称
                    hotelEntity.PinyinName = SearchHelper.ToPinYin(hotelEntity.HotelName);

                    //获得星级
                    hotelEntity.Star = FormatHotelStar(hotelEntity.Star);

                    //获得主题
                    hotelEntity.Themes = GenHotelInterest(hotelEntity.Id);

                    //获得标签
                    hotelEntity.Featured = GenHotelFeatured(hotelEntity.Id);

                    //获得设施
                    hotelEntity.Facility = GenHotelFacility(hotelEntity.Id);

                    //获得酒店类型
                    hotelEntity.Class = GenHotelClass(hotelEntity.Id);

                    //获得酒店的区域
                    hotelEntity.DistrictZone = GenHotelDistrictZone(hotelEntity.Id);

                    //获得酒店的相关POI
                    hotelEntity.POI = GenHotelPOI(hotelEntity.Id);

                    hotelIndexResult.HotelEntityList.Add(hotelEntity);
                }
            }

            return hotelIndexResult;
        }

        /// <summary>
        /// 1	长途汽车站
//2	地铁轻轨站
//3	飞机场
//4	港口、码头
//5	火车站
//6	其他地点
//7	高尔夫球场
//8	高铁
//9	主题乐园
//10	景点
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        private static string GenHotelPOI(int hotelId)
        {
            var districtZone = "";
            //
            var sql = string.Format(@"SELECT pn.[name] 
FROM DestDB.dbo.POIForNearby pn (NOLOCK)
INNER JOIN NearbyHotels nh (NOLOCK) ON nh.POIID = pn.id 
WHERE pn.DetailCategoryID >2 AND  nh.HotelID = {0} 
", hotelId);

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(districtZone)) districtZone += " ";
                    districtZone += reader[0].ToString();
                }
            }

            return districtZone;
        }

        /// <summary>
        /// 获取酒店主题信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static string GenHotelInterest(int hotelId)
        {
            var interest = "";

            var sql = string.Format(@"
select i.name  
from interesthotelrel ihr  (nolock) 
inner join interestplace ip  (nolock) on ip.id = ihr.ipid 
inner join interest i  (nolock) on i.id = ip.iid 
where ihr.state > 10 and i.released = 1 and i.type = 1
and ihr.HID = {0}", hotelId);

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(interest)) interest += " ";
                    interest += reader[0].ToString();
                }
            }

            return interest;
        }

        /// <summary>
        /// 获取酒店标签信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static string GenHotelFeatured(int hotelId)
        {
            var featured = "";

            var sql = string.Format(@"
select fc.hotelid,ft.name 
from [dbo].[FeaturedComment] fc (nolock) 
inner join FeaturedTree ft  (nolock) on fc.featuredid = ft.id 
where released = 1 and hotelid =  {0}
union 
select wbtr.hotelid,ft.name 
from  FeaturedTree ft (nolock) 
INNER JOIN FeaturedTreeSubRel ftr (NOLOCK) ON ftr.ID= ft.ID
INNER JOIN FeaturedBaseTagRel fbtr (nolock)  ON ftr.SubID = fbtr.FeaturedID
INNER JOIN CommentDB.dbo.WHHotelBaseTagRel wbtr  (nolock) ON wbtr.BaseTagIDX = fbtr.BaseTagID AND wbtr.[State] = 1
where wbtr.hotelid =  {0} 
union 
select wbtr.hotelid,ft.name 
from  FeaturedTree ft (nolock) 
INNER JOIN FeaturedTreeSubRel ftr (NOLOCK) ON ftr.ID= ft.ID
INNER JOIN FeaturedBaseTagRel fbtr (nolock)  ON ftr.SubID = fbtr.FeaturedID
INNER JOIN CommentDB.dbo.WHHotelBaseTagRel wbtr (nolock)  ON wbtr.BaseTagIDX = fbtr.BaseTagID AND wbtr.[State] = 3
where wbtr.hotelid =  {0} 
", hotelId);

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(featured)) featured += " ";
                    featured += reader["name"].ToString();
                }
            }

            return featured;
        }

        /// <summary>
        /// 获取酒店设施信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static string GenHotelFacility(int hotelId)
        {
            var facility = "";

            var sql = string.Format(@"
select Name from FeaturedTree f (nolock) 
INNER JOIN (
    select top 1000 hfID=(FilterCol - 10000000000 * 19) 
    from HotelFilter (nolock) 
    where HotelID = '{0}' and FilterCol like '19000%') a
ON f.ID = a.hfID
", hotelId);

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(facility)) facility += " ";
                    facility += reader[0].ToString();
                }
            }

            return facility;
        }

        /// <summary>
        /// 获取酒店类型信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static string GenHotelClass(int hotelId)
        {
            var classVal = "";

            var sql = string.Format(@"
select Name from [HotelBizDB].[dbo].[HotelClass2Define] hc (nolock) 
INNER JOIN (select top 1000 hfClassType= (FilterCol - 10000000000 * 21)
from HotelFilter (nolock) 
where HotelID = '{0}' and FilterCol like '21000%') a
ON hc.ClassType = a.hfClassType
", hotelId);

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(classVal)) classVal += " ";
                    classVal += reader[0].ToString();
                }
            }

            return classVal;
        }

        /// <summary>
        /// 获取酒店区域信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static string GenHotelDistrictZone(int hotelId)
        {
            var districtZone = "";

            var sql = string.Format(@"select Name from DestDB..DistrictZone  DZ (nolock) 
INNER JOIN (select top 1000 ZID=(FilterCol - 10000000000 * 18) 
from HotelFilter  (nolock) where HotelID = {0} and FilterCol like '18000%') a 
ON a.ZID = DZ.ID
", hotelId);

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(districtZone)) districtZone += " ";
                    districtZone += reader[0].ToString();
                }
            }

            return districtZone;
        }

        /// <summary>
        /// 根据星级数字返回星级全称
        /// </summary>
        /// <param name="star"></param>
        /// <returns></returns>
        public static string FormatHotelStar(string star)
        {
            switch (star)
            {
                case "1":
                    {
                        return "一星级 1星级";
                        break;
                    }
                case "2":
                    {
                        return "二星级 2星级";
                        break;
                    }
                case "3":
                    {
                        return "三星级 3星级";
                        break;
                    }
                case "4":
                    {
                        return "四星级 4星级";
                        break;
                    }
                case "5":
                    {
                        return "五星级 5星级";
                        break;
                    }
            }

            return "";
        }

        /// <summary>
        /// 获取有更新的Hotel数据
        /// </summary>
        /// <returns></returns>
        public static List<HotelEntity> GetUpdatesHotels(string lastTimestamp)
        {
            var hotels = new List<HotelEntity>();

            var sql = string.Format(@"
select distinct h.HotelId,HotelName,Enabled,TimeStampCol,1 State --isnull(hs.State,0) State
from Hotel h
--left join HotelState hs on hs.HotelId = h.HotelId and hs.State = 1
where timeStampCol > {0} 
order by timeStampCol
", lastTimestamp);
            //0x000000000A775A11

            var reader = SqlHelper.ExecuteReader(SqlHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    if (reader["HotelID"] == null || string.IsNullOrEmpty(reader["HotelID"].ToString())) continue;

                    var state = reader["State"].ToString();
                    var enabled = reader["Enabled"].ToString();
                    if (state != "1")
                    {
                        enabled = "F";
                    }

                    hotels.Add(new HotelEntity 
                    {
                        HotelId = Convert.ToInt32(reader["HotelID"].ToString()),
                        HotelName = reader["HotelName"].ToString(),
                        Enabled = enabled,
                        TimeStampCol = ConvertTimestampToString(((byte[])(reader["TimeStampCol"])))
                    });
                }
            }

            return hotels;
        }

        /// <summary>
        /// 将Timestamp类型(C#取出来是byte[])的字段，转换成字符串
        /// </summary>
        /// <param name="timestamps"></param>
        /// <returns></returns>
        private static string ConvertTimestampToString(byte[] timestamps)
        {
            string t = BitConverter.ToString(timestamps);
            t = t.Replace("-", "");
            return "0x" + t;
        }

        #endregion
    }
}
