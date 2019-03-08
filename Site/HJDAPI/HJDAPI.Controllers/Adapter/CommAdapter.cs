using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WH.DataAccess.CommDB.Item;

namespace HJDAPI.Controllers.Adapter
{
    public class CommAdapter
    {
        public static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService"); 

         
        #region Tag
        public static List<ObjRelItem> GetObjRelList(ObjRelTypeEnum type, long ObjID, ObjIDTypeEnum idType)
        {
            return commService.GetObjRelList(type, ObjID, idType);
        }

        /// <summary>
        /// 获取标签对象
        /// </summary>
        /// <param name="IDList"></param>
        /// <returns></returns>
        public static List<TagItem> GetTagItemList(List<int> IDList)
        {
            return CacheAdapter.LocalCache30Min.GetMultiDataEx<int,TagItem>(IDList,"CommTag",
                  noDataList =>commService.GetTagItemList(noDataList), 
                  tag=>tag.ID
                 );
        }

        #endregion


        public static int AddBizOpLog(BizOpLogEntity bizoplog)
        {
            return commService.AddBizOpLog(bizoplog);
        }
         

        public static List<string>  GetNewVIPGiftCouponList()
        {
            return GetCommDicKeyValueWidthCache(10008, 7);//VIP欢迎礼券  
        }
        public static List<string>  GetCommDicKeyValueWidthCache(int typeid, int key)
        {
            return CacheAdapter.LocalCache10Min.GetData<List<string>>(GenCommDicKeyValueCacheKey( typeid,  key),
                () =>
                {
                    return   commService.GetCommDict(typeid, key).Descript.Split("\r\n".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries ).ToList();
                });
        }

        public static string GenCommDicKeyValueCacheKey(int typeid, int key)
        {
            return string.Format("GetCommDicKeyValueWidthCache:{0}|{1}", typeid, key);
        }

        public static void ClearCommDicKeyValueCache(int typeid, int key)
        {
            CacheAdapter.LocalCache10Min.Remove(GenCommDicKeyValueCacheKey(typeid, key));
        }

        public static List<KeyValueEntity> GetUploadFileByOrderID(long orderId)
        {
            List<KeyValueEntity> fileUrlList = new List<KeyValueEntity>();
            List<UploadDataFileEntity> fileList = commService.GetUploadDataFileByOrderId(orderId);
            foreach (UploadDataFileEntity item in fileList)
            {
                KeyValueEntity model = new KeyValueEntity();
                model.Key = "下载" + item.OriFileName.Split('.')[0];
                model.Value = Configs.UploadFileHttpPath + item.FileRelativePath;
                //string fileUrl = "" + item.FileRelativePath;
                fileUrlList.Add(model);
            }
            return fileUrlList;
        }
        public static List<ProductTag> GetProductTagList(int typeId, int dicKey, int districtID)
        {
            CommDictEntity commDictEntity = commService.GetCommDict(600, 1);
            var productTagModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductTagModel>(commDictEntity.Descript);
            if (productTagModel != null)
            {
                var tagIdList = ProductAdapter.GetSPUTagID(districtID);
                return productTagModel.ProductTagList.Where(p=>tagIdList.Contains(p.ID)).OrderBy(p=>p.Sort).ToList();
            }
            else
            {
                return new List<ProductTag>();
            }
            
            //return LocalCache10Min.GetData<List<ProductTag>>(GenCommDicKeyValueCacheKey(typeId, dicKey), () =>
            //{
            //    if (typeId > 0)
            //    {
            //        CommDictEntity commDictEntity = commService.GetCommDict(600, 1);
            //        var productTagModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductTagModel>(commDictEntity.Descript);
            //        if (productTagModel != null)
            //        {
            //            return productTagModel.ProductTagList;
            //        }
            //        else
            //        {
            //            return new List<ProductTag>();
            //        }
            //    }
            //    else
            //    {
            //        return new List<ProductTag>();
            //    }

            //});
        }

        /// <summary>
        /// 返回所有活动海报列表
        /// </summary>
        /// <returns></returns>
        public static List<ChannelPosterActiveEntity> GetAllChannelActive()
        {
            return commService.GetChannelActiveList();   
        }

        /// <summary>
        /// 根据ID查询
        /// </summary>
        /// <returns></returns>
        public static ChannelPosterActiveEntity GetChannelActiveByID(int id) {
           
            return commService.GetChannelActiveByID(id);
        

        }

         
    }
}
