using HJD.AccountServices.Entity;
using HJD.CommentService.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Common
{
    public class DescriptionHelper
    {
        static int rep = 0;
        static readonly object _lock = new object();
        //115CNyD0oQ
        public static string defaultInspectorAvatar = PhotoAdapter.GenHotelPicUrl("115FSeu0pm", HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter);
        //115ZQpo0Dl 115KMvh0G8
        public static string defaultAvatar
        {
            get 
            {
                var picUrl = PhotoAdapter.GenHotelPicUrl("115CN1V08W", HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter);

                //Random _r = new Random();
                //var picNum = _r.Next(1, 10);
                //var picUrl = string.Format("http://whfront.b0.upaiyun.com/app/img/head/{0}-150x150.png", picNum);

                return picUrl;
            }
        }

        public static string defaultZMJDLogo = "116HOQY0XH";

        public static CommentShareModel GetShareModel(CommentInfoModel comment, long trackUserID = 0)
        {
            try
            {
                CommentShareModel m = new CommentShareModel();
                int hotelID = comment.commentInfo.Comment.HotelID;
                string hotelName = "";
                if (hotelID == 0)
                {
                    List<CommentAddHotelEntity> addHotellist = HotelAdapter.HotelService.GetUserAddHotelByComment(comment.commentInfo.Comment.ID);//获得用户添加酒店名称
                    if (addHotellist != null && addHotellist.Count != 0)
                    {
                        hotelName = addHotellist.First().HotelName;
                    }
                }
                else
                {
                    HotelItem hi = HotelAdapter.GetOneHotelInfo(hotelID);
                    hotelName = hi.Name;
                }

                var oldTitle = string.Format("推荐一篇对{0}的点评", hotelName);                
                string commentTitle = DescriptionHelper.CommentInfoConcat(9, comment.commentInfo);//4.0版本新加的一句话点评

                m.notHotelNameTitle = string.IsNullOrWhiteSpace(commentTitle) ? oldTitle : commentTitle;
                m.title = m.notHotelNameTitle;

                //有点评标题的点评 则详情不用显示 详细点评小标题
                if (!string.IsNullOrWhiteSpace(commentTitle))
                {
                    m.Content = DescriptionHelper.CommentInfoConcat(10, comment.commentInfo);//4.0版本新加的详细点评
                }
                else{
                    m.Content = HotelAdapter.GetOrGenWHHotelCommentDescription(comment.commentInfo);
                }

                if (!string.IsNullOrWhiteSpace(m.Content) && m.Content.Length > 20)
                {
                    string s1 = System.Text.RegularExpressions.Regex.Replace(m.Content, "<.*?>", "");
                    string s2 = s1.Replace("&nbsp;", "");
                    int length = s2.Length;
                    m.Content = s2.Substring(0, length >= 20 ? 20 : length);
                }

                if (comment.photoInfo != null && comment.photoInfo.Count > 0)
                {
                    m.photoUrl = comment.photoInfo.First();
                }
                else if (hotelID > 0)
                {
                    HotelPhotosEntity hps = HotelAdapter.GetHotelPhotos(comment.commentInfo.Comment.HotelID, 0);
                    if (hps.HPList.Count > 0)
                    {
                        m.photoUrl = PhotoAdapter.GenHotelPicUrl(hps.HPList.First().SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.small).Replace("_small", "_290x290s");
                    }
                }
                else
                {
                    m.photoUrl = "";
                }

                int commentId = comment.commentInfo.Comment.ID;
                string url = GenAppJumpUrl(HJDAPI.Common.Helpers.Enums.JumpUrlName.commentsharedetail, commentId);//4.2上线commentsharedetail

                long commentUserID = comment.commentInfo.Comment.UserID;
                string identityParamPre = url.IndexOf("?") > -1 ? "&CID=" : "?CID=";

                //追踪码生成 点评ID + 英文半角逗号 + 分享者UserID DES加密后传输 防止url篡改
                string desBase64Str = HJDAPI.Common.Security.DES.Encrypt(string.Format("{0},{1}", commentId, trackUserID != 0 && trackUserID != commentUserID ? trackUserID : commentUserID));
                string queryParamEnd = identityParamPre + desBase64Str;

                m.shareLink = string.Format("{0}{1}", url, queryParamEnd);
                var urlEncoded = System.Web.HttpUtility.UrlEncode(m.shareLink);
                //短链接处理
                m.shareLink = HJDAPI.Controllers.Adapter.AccessAdapter.GenShortUrl(0, urlEncoded);
                return m;
            }
            catch(Exception ex){
                Log.WriteLog("获取点评分享信息报错，内容是" + ex.Message + ex.StackTrace);
                throw;
            }
        }

        public static string CommentInfoConcat(int tagCatID, CommentInfoEntity entity)
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

        public static string GenCommentContent(CommentInfoEntity entity)
        {
            //顺序为 特点 不足 房间 玩点 美食的顺序拼接 自然换行
            string styleComment = CommentInfoConcat(1, entity);
            string badComment = CommentInfoConcat(7, entity);
            string roomComment = CommentInfoConcat(3, entity);
            string playComment = CommentInfoConcat(4, entity);
            string foodComment = CommentInfoConcat(5, entity);
            string serviceComment = CommentInfoConcat(6, entity);

            return (string.IsNullOrWhiteSpace(styleComment) ? "" : "特点：" + styleComment + "\r\n") +
                (string.IsNullOrWhiteSpace(badComment) ? "" : "不足：" + badComment + "\r\n") +
                (string.IsNullOrWhiteSpace(roomComment) ? "" : "房间：" + roomComment + "\r\n") +
                (string.IsNullOrWhiteSpace(playComment) ? "" : "玩点：" + playComment + "\r\n") +
                (string.IsNullOrWhiteSpace(foodComment) ? "" : "美食：" + foodComment + "\r\n") +
                (string.IsNullOrWhiteSpace(serviceComment) ? "" : "服务：" + serviceComment);
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
                case 2://房型
                    return -100;
                case 12://特色标签
                    return -200;
                case 8://出游类型
                    return -300;
                default:
                    return tagCatID;
            }
        }

        public static string GenAppJumpUrl(HJDAPI.Common.Helpers.Enums.JumpUrlName typeCode, int id) {
            string baseDomin = Configs.WWWURL;//http://www.zmjiudian.com/;
            string resultUrl = "";
            switch(typeCode){
                case HJDAPI.Common.Helpers.Enums.JumpUrlName.commentdetail:
                    resultUrl = baseDomin + "/personal/comments/" + id.ToString();
                    break;
                case HJDAPI.Common.Helpers.Enums.JumpUrlName.commentsharedetail:
                    resultUrl = baseDomin + "/Comment/ShareDetail?CommentId=" + id.ToString();
                    break;
                default:
                    resultUrl = baseDomin + "/personal/comments/" + id.ToString();
                    break;
            }
            return resultUrl;
        }

        public static string GenerateRandomStr()
        {
            Random rand = new Random();
            int count = rand.Next(8,30);
            return GenerateCheckCode(count);
        }

        /// <summary>
        /// 生成固定长度的随机字符串(数字加字母)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GenAssignLengthRandomStr(int count)
        {
            return GenerateCheckCode(count);
        }

        /// 生成随机字母字符串(数字字母混和)
        /// 待生成的位数
        /// 生成的字母字符串
        private static string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            lock(_lock){
                long num2 = DateTime.Now.Ticks + rep;
                rep++;
                Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
                for (int i = 0; i < codeCount; i++)
                {
                    char ch;
                    int num = random.Next();
                    if ((num % 2) == 0)
                    {
                        ch = (char)(0x30 + ((ushort)(num % 10)));
                    }
                    else
                    {
                        ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                    }
                    if( i==0 && ch =='0')//生成随机字母字符串首位不能为‘0’
                    {
                        ch = '1';
                    }
                    str = str + ch.ToString();
                }
            }  
            return str;
        }

        //private static string GenintCheckCode(int codeCount)
        //{
        //    string str = string.Empty;
        //    return str;
        //}

        /// <summary>
        /// 生成固定长度的随机字符串(数字)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GenintLengthRandomStr(int count)
        {
            return GenintCheckCode(count);
        }
        private static string GenintCheckCode(int Length)
        {
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0,n);
                if(i == 0 && rnd==0)  //生成随机字母字符串首字母不能为0
                {
                    rnd = 1;
                }
                result += Pattern[rnd];
            }
            return result;
        }

        public static string maskPhoneNum(string phoneNum)
        {
            if (!string.IsNullOrEmpty(phoneNum) && Regex.IsMatch(phoneNum, @"^((13[0-9])|(15[^4,\\d])|(18[0-9])|(17[0-8])|(147,145))\d{8}$"))
            {
                int length = phoneNum.Length;
                return phoneNum.Substring(0, 3) + "*****" + phoneNum.Substring(length-3,3);
            }
            else {
                return "匿名用户";
            }
        }

        public static int getSecondCountSince19700101()
        {
            DateTime oldDate = new DateTime(1970,1,1,0,0,0);
            DateTime currentDate = DateTime.Now;
            return (int)Math.Round((currentDate - oldDate).TotalSeconds, 0);
        }

        public static String CreateNoncestr(int length)
        {
            String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String res = "";
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                res += chars[rd.Next(chars.Length - 1)];
            }
            return res;
        }

        public static String CreateNoncestr()
        {
            String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String res = "";
            Random rd = new Random();
            for (int i = 0; i < 16; i++)
            {
                res += chars[rd.Next(chars.Length - 1)];
            }
            return res;
        }

        /// <summary>
        /// 根据最大价格 每次加价幅度 生成价格的标签块
        /// </summary>
        /// <param name="maxSize"></param>
        /// <param name="slideSize"></param>
        /// <returns></returns>
        public static List<FilterTag> GenFilterTags(int maxSize, int slideSize)
        {
            if (maxSize > 0 && slideSize > 0 && maxSize % slideSize == 0)
            {
                return new List<FilterTag>();
            }

            return new List<FilterTag>();
        }

        public static TrackCodeData GenTrackCodeResult4Share(GenTrackCodeParam param)
        {
            var dic = new Dictionary<string,string>();
            if(param.BizType != 0)
            {
                dic.Add("BizType", param.BizType.ToString());
            }
            if (param.CommentID != 0)
            {
                dic.Add("CommentID", param.CommentID.ToString());
            }
            if (param.CouponActivityID != 0)
            {
                dic.Add("CouponActivityID", param.CouponActivityID.ToString());
            }
            if (param.DistrictID != 0)
            {
                dic.Add("DistrictID", param.DistrictID.ToString());
            }
            if (param.HotelID != 0)
            {
                dic.Add("HotelID", param.HotelID.ToString());
            }
            if (param.Interest != 0)
            {
                dic.Add("Interest", param.Interest.ToString());
            }
            if (param.UserID != 0)
            {
                dic.Add("UserID", param.UserID.ToString());
            }

            var tobeEncrypt = Newtonsoft.Json.JsonConvert.SerializeObject(dic);//param

            //var originObj = Newtonsoft.Json.JsonConvert.DeserializeObject<GenTrackCodeParam>(tobeEncrypt);

            var desBase64Str = HJDAPI.Common.Security.DES.Encrypt(tobeEncrypt);

            var baseUrl = "http://www.zmjiudian.com";

            switch(param.BizType){
                case ZMJDShareTrackBizType.commentdetail:
                    baseUrl += string.Format("/comment/shareDetail?commentId={0}",param.CommentID);
                    break;
                case ZMJDShareTrackBizType.hoteldetail:
                    baseUrl += string.Format("/hotel/{0}", param.HotelID);
                    break;
                case ZMJDShareTrackBizType.roomcoupondetail:
                    baseUrl += string.Format("/coupon/shop/{0}?userid=0", param.CouponActivityID);
                    break;
                case ZMJDShareTrackBizType.districtinterestlist:
                    baseUrl += string.Format("/coupon/shop/group/{0}?userid=0", param.CouponActivityID);
                    break;
                case ZMJDShareTrackBizType.festivalroom:
                    baseUrl += "/active/holiday";
                    break;
                case ZMJDShareTrackBizType.recommendfriend:
                    baseUrl += "/fund/userrecommend";
                    break;
                default:
                    break;
            }

            var shareLink = baseUrl + (baseUrl.IndexOf("?") > -1 ? "&SID=" : "?SID=") + desBase64Str;
            return new TrackCodeData()
            {
                EncodeStr = desBase64Str,
                ShareLink = shareLink
            };
        }
    }
}