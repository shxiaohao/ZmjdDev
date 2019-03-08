using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Cache;
using ProductService.Contracts;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace HJDAPI.Controllers
{
    public class ProductController : BaseApiController
    {
        [HttpGet]
        public Boolean IsRetailerProduct(int SKUID)
        {
            return ProductAdapter.IsRetailerProduct(SKUID);
        }

         [HttpGet]
        public Boolean  RemoveStepGroupCahceWithSKUID(int SKUID)
        {
            ProductCache.RemoveStepGroupCahceWithSKUID(SKUID);
            return true;
        }

        /// <summary>
        /// 从字典获取产品标签列表
        /// </summary>
        /// <param name="typeId">字典类型</param>
        /// <param name="dicKey">字典key</param>
        /// <returns></returns>
        [HttpGet]
        public List<ProcudtCategoryEntity> GetProductCategoryList(int typeID = 600, int dicKey = 1, int districtID=0)
        {
            var productTagList = CommAdapter.GetProductTagList(typeID, dicKey, districtID);
            List<ProcudtCategoryEntity> list = new List<ProcudtCategoryEntity>();
            productTagList.ForEach(p => {
                ProcudtCategoryEntity model = new ProcudtCategoryEntity();
                model.ID = p.ID;
                model.Name = p.Name;
                model.ICON = p.ICON;
                model.Description = p.Description;
                list.Add(model);
            });
            return list;
        }
        [HttpGet]
        public List<ProcudtCategoryEntity> GetProductCategoryByID(int id)
        {
            return ProductAdapter.GetProductCategoryList(id);
        }

    }
}
