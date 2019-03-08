using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJD.CommentService.Contract;
using HJD.CouponService.Contracts.Entity;

namespace WHotelSite
{
    public class ViewReturnValueHelper
    {
        public static string CommentInfoConcat(int tagCatID, int blockCatID, CommentInfoEntity entity)
        {
            string basicTag = "";
            List<TagCategoryEntity> tagInfoList = (from n in entity.TagInfo where n.CategoryID == tagCatID select n).ToList<TagCategoryEntity>();
            if (tagInfoList != null && tagInfoList.Count != 0)
            {
                List<string> ss = new List<string>();
                tagInfoList[0].Tags.ForEach(i => ss.Add(i.TagName));
                basicTag = string.Join("、", ss);
                basicTag.TrimEnd('、');
            }

            string addInfo = "";
            int extraCategoryID = ReturnAddInfoExtraID(tagCatID);
            List<CommentAddInfoEntity> tagAddList = (from n in entity.AddInfos where n.CategoryID == tagCatID || n.CategoryID == extraCategoryID select n).ToList<CommentAddInfoEntity>();
            if (tagAddList != null && tagAddList.Count != 0)
            {
                addInfo = string.Join("、", tagAddList.Select(_ => _.Content));//tagAddList[0].Content;
            }

            if (string.IsNullOrEmpty(basicTag))
            {
                return addInfo;
            }
            else
            {
                return basicTag + "。" + addInfo;
            }
        }

        /// <summary>
        /// 由标签块所属大类 获得其其他标签对应的特殊类 如房型大类是3 其他房型标签特殊类型是-100
        /// </summary>
        /// <param name="tagCatID"></param>
        /// <returns></returns>
        private static int ReturnAddInfoExtraID(int tagCatID)
        {
            switch (tagCatID)
            {
                case 3://房型
                    return -100;
                case 12://特色标签
                    return -200;
                case 8://出游类型
                    return -300;
                default:
                    return tagCatID;
            }
        }
    }

    public class DateComparer : IComparer<AcquiredCoupon>
    {
        public int Compare(AcquiredCoupon x, AcquiredCoupon y)
        {
            DateTime xDate = x.AcquiredTime.HasValue ? (DateTime)x.AcquiredTime : DateTime.MinValue;
            DateTime yDate = y.AcquiredTime.HasValue ? (DateTime)y.AcquiredTime : DateTime.MinValue;
            return -(int)Math.Round((xDate - yDate).TotalSeconds,0);
        }
    }
}