using AutoMapper;
using HJDAPI.Models;
using HJDAPI.Models.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public class AutoMapHelper
    {
        public static void InitAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<OrderListItemEntity, DetailOrderListEntity>();
                cfg.CreateMap<WH.DataAccess.CommDB.Item.TagItem, CommTagEntity>(); 
            });
        }
    }
}
