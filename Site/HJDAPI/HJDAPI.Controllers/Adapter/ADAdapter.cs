using HJD.ADServices.Contract;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class ADAdapter
    {
        static IADService ADService = ServiceProxyFactory.Create<IADService>("IADService");
        public static ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForADV");

        public static int AddUserNotices_ActiveRabate(long userid, int ActiveRebateAmount)
        {
            return ADService.AddUserNotices_ActiveRabate( userid,  ActiveRebateAmount);
        }

        public static  List<ActiveUserCodeEntity> GetActiveCode(long userID, int activeID)
        {
            return ADService.GetActiveCode(userID, activeID);
        }

        public static  bool AddActiveUserCodeRel(ActiveUserCodeRelEntity activeusercoderel)
        {
            ADService.AddActiveUserCodeRel(activeusercoderel);

            return true;
        }

        public static ADTypeEntity GetAdTypeByID(int id)
        {
            return ADService.GetADTypeByID(id);
        }

        public static List<ADTypeEntity> GetAllADType()
        {
            return ADService.GetAllADType();
        }

        /// <summary>
        /// 获取所有有效的banner 包括推荐度假地区 常规广告
        /// </summary>
        /// <returns></returns>
        public static List<AdvBaseInfoEntity> GetOnlineAdvBaseInfo()
        {
           return  ADService.GetOnlineAdvBaseInfo();
        }

        /// <summary>
        /// 4.0及之前App版本获取广告栏内容
        /// </summary>
        /// <param name="districtID"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="glat"></param>
        /// <param name="glon"></param>
        /// <returns></returns>
        public static Advertise GetOnlineAdv(int districtID, int geoScopeType, float glat, float glon, bool isShowMagicall = false)
        {
            if (geoScopeType != (int)HJDAPI.Common.Helpers.Enums.GEOScopeType.ArooundUser)
            {
                return LocalCache.GetData<Advertise>(string.Format("OnlineAdv:{0}:{1}:{2}", districtID, geoScopeType, isShowMagicall ? 1 : 0), () =>
                {
                    return GenOnlineAdv(districtID, geoScopeType, glat, glon, isShowMagicall);
                });
            }
            else
            {
                //兼顾老版和新版 以4.0版本为界 之前key加个0 之后加个1
                return LocalCache.GetData<Advertise>(string.Format("OnlineAdv:{0}:{1}:{2}:{3}", Math.Round(glat),
                    Math.Round(glon), districtID > 0 ? 1 : 0, isShowMagicall ? 1 : 0), () =>
                {
                    return GenOnlineAdv(districtID, geoScopeType, glat, glon, isShowMagicall);
                });
            }
        }

        /// <summary>
        /// 4.0版本精选页面（此处说的首页指显示某地区及周边的精选酒店 而指定具体地区的精选酒店不是首页）广告出现规则:1.全部地区 不是仅首页显示 -> 全部地区及首页显示（全站）; 2.某个地区 + 仅首页显示 -> 某个城市(地区)及周边(首页)显示（包括其上级区域显示在首页的广告）; 3.某个地区 + 非仅首页显示 -> 选择某个具体地区后显示（不包括首页 且显示上级地区的广告）;4. 全部地区 + 仅首页显示 -> 全部地区及周边显示（所有用户的首页显示 选定具体城市不显示）
        /// </summary>
        /// <param name="districtID"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="glat"></param>
        /// <param name="glon"></param>
        /// <returns></returns>
        public static Advertise GenOnlineAdv(int districtID, int geoScopeType, float glat, float glon, bool isShowMagicall = false)
        {
            Advertise adv = new Advertise()
            {
                Ratio = 0.547,  // 广告高宽比
                ADList = new List<ADItem>()
            };

            List<AdvBaseInfoEntity> al = GetOnlineAdvBaseInfo().FindAll(_ => _.ADType == 0);
            List<AdvBaseInfoEntity> AL = new List<AdvBaseInfoEntity>();
            //同一时期一个城市上线一家酒店。当区域范围为“城市及周边”的时候，区域范围相关城市最多显示两家酒店，按后台设置的优先度显示。
            //后台可设置单家酒店上线广告的优先度。
            if (geoScopeType == (int)HJDAPI.Common.Helpers.Enums.GEOScopeType.District)
            {
                //2015-09-10 wwb 限制地区级别 不出仅限首页出现的广告  !a.IsOnlyHomeShow
                AL = al.Where(a => (a.DistrictID == 0 || a.DistrictID == districtID || judgeAdvRelDistrict(a.DistrictIdStr, districtID)) && true).OrderByDescending(o => o.DistrictID).ThenByDescending(o => o.Priority).ToList();
            }
            else
            {
                foreach (AdvBaseInfoEntity a in al.OrderBy(o => (o.DistrictID>0?1:0)).ThenByDescending(o => o.Priority))
                {
                    int finalDistrictId = 0;
                    if(districtID == 0 && (glon != 0 || glat != 0)){
                        var relDistricts = HotelAdapter.CalculateNearDistrictByDistance(0, glat, glon).OrderBy(_ => _.Distance).FirstOrDefault();
                        finalDistrictId = relDistricts != null ? relDistricts.DistrictID : 0;
                    }

                    if (a.DistrictID == 0 || ((finalDistrictId == a.DistrictID || judgeAdvRelDistrict(a.DistrictIdStr, finalDistrictId)) && a.IsOnlyHomeShow))
                    {
                        AL.Add(a);
                    }
                }
            }

            AL = isShowMagicall ? AL.FindAll(_ => _.ID != 164) : AL.FindAll(_ => _.ID != 101);//不显示则排除101

            foreach(AdvBaseInfoEntity a in AL)
            {
                adv.ADList.Add(new ADItem { ADShowType = a.ADShowType,  ADURL = (a.ADShowType==0?a.BgImgUrl:a.AdUrl), ActionURL = FormatActionURL(a.ContentUrl) });
            }

           return adv;

        }

        /// <summary>
        /// 4.1版本 精选页面(此处app加了一个单独的首页栏目 此方法不再涉及首页显示 仅显示多个地区或指定地区显示的广告) 广告出现规则:
        /// 如果显示某个地区及周边 则要计算出附近300公里所有地区的关联广告 按距离和优先级排序；如果显示某个具体城市的广告 则只需显示该地区关联的广告（包括在其行政上级地区显示的广告）
        /// Last But Not Least, 如果没有设置banner 那么从DistrictZone(TYPE = 1)地区找一个相同目的地的数据 如果没有数据则使用默认的目的地图片
        /// 注意：如果没有城市酒店攻略 则ActionURL为空字符串 则无挑战仅为显示图片
        /// 115NMBa0Ks   city-banner默认图片
        /// </summary>
        /// <param name="districtID"></param>
        /// <param name="geoScopeType"></param>
        /// <param name="glat"></param>
        /// <param name="glon"></param>
        /// <returns></returns>
        public static Advertise GenOnlineStrategy(int districtID, int geoScopeType, float glat, float glon, string districtName)
        {
            Advertise adv = new Advertise()
            {
                Ratio = 0.625,// 广告高宽比
                ADList = new List<ADItem>()
            };

            List<AdvBaseInfoEntity> AL = new List<AdvBaseInfoEntity>();
            
            if (geoScopeType == (int)HJDAPI.Common.Helpers.Enums.GEOScopeType.District)
            {
                //2015-11-11 只要一个攻略记录没有不显示
                //AdvBaseInfoEntity tempInfo = al.Where(a => !a.IsOnlyHomeShow && (a.DistrictID == districtID || judgeAdvRelDistrict(a.DistrictIdStr,districtID))).FirstOrDefault();

                HJD.DestServices.Contract.DistrictZoneEntity strategyData = ResourceAdapter.GetManyDistrictZoneEntity(new List<int> { districtID }, 1, -1).FirstOrDefault();

                if (strategyData != null)
                {
                    string strategyUrl = string.Format("http://www.zmjiudian.com/strategy{0}", districtID);
                    Regex reg = new Regex("_.+$", RegexOptions.None);
                    AdvBaseInfoEntity tempInfo = new AdvBaseInfoEntity()
                    {
                        ContentUrl = strategyData.State == 1 ? FormatActionURL(strategyUrl) : "",
                        ADShowType = 0,
                        AdUrl = strategyUrl,
                        BgImgUrl = string.IsNullOrWhiteSpace(strategyData.PicUrl) ? PhotoAdapter.GenHotelPicUrl("115NMBa0Ks", HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter) : reg.Replace(PhotoAdapter.GenHotelPicUrl(strategyData.PicUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter), "_jupiter"),
                        DistrictID = districtID
                    };
                    AL.Add(tempInfo);
                }
                else
                {
                    //先找districtzone的目的地照片 再用默认城市照片替换 actionUrl设为""
                    AL.Add(genDefaultStrategyBaseInfo(districtID, 0, "115NMBa0Ks", ""));
                }
            }
            else //显示附近300km所有地区的广告 显示最近的一张
            {
                StrategyADResult aroundAd = GetStrategyADList(districtID, glon, glat);
                if(aroundAd != null && aroundAd.ADList != null && aroundAd.ADList.Count != 0){
                    //默认的城市banner数据 无跳转 115NMBa0Ks
                    //int targetDistrictID = aroundAd.ADList[0].DistrictID;
                    AL.Add(genDefaultStrategyBaseInfo(districtID, 0, "115NMBa0Ks", string.Format("whotelapp://www.zmjiudian.com/strategylist?districtID={0}&lon={1}&lat={2}&districtName={3}", districtID, glon, glat, districtName)));
                }
                else
                {
                    //默认的城市banner数据 无跳转 115NMBa0Ks
                    AL.Add(genDefaultStrategyBaseInfo(districtID, 0, "115NMBa0Ks", ""));
                }
            }

            foreach (AdvBaseInfoEntity a in AL)
            {
                adv.ADList.Add(new ADItem { ADShowType = a.ADShowType, ADURL = (a.ADShowType == 0 ? a.BgImgUrl : a.AdUrl), ActionURL = a.ContentUrl });
            }

            return adv;
        }

        private static AdvBaseInfoEntity genDefaultStrategyBaseInfo(int districtId, int showType, string adSUrl, string contentUrl)
        {
            //先找可以找到的攻略目的地
            //if(districtId > 0){
            //    string adUrl = HotelAdapter.HotelService.GetDistrictZonePicSUrl(districtId);
            //    if(!string.IsNullOrWhiteSpace(adUrl)){
            //        return new AdvBaseInfoEntity()
            //        {
            //            ADShowType = showType,
            //            AdUrl = PhotoAdapter.GenHotelPicUrl(adUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter),
            //            ContentUrl = contentUrl
            //        };
            //    }
            //}
            //加缺省的城市照片
            return new AdvBaseInfoEntity()
            {
                ADShowType = showType,
                AdUrl = PhotoAdapter.GenHotelPicUrl(adSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter),
                BgImgUrl = PhotoAdapter.GenHotelPicUrl(adSUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter),
                ContentUrl = contentUrl,
                DistrictID = districtId
            };
        }

        private static bool judgeAdvRelDistrict(string relDistrictIds, int districtId)
        {
            return string.IsNullOrWhiteSpace(relDistrictIds) ? false : new Regex("\b,?" + districtId.ToString() + ",?\b", RegexOptions.ECMAScript).IsMatch(relDistrictIds, 0);
        }

        /// <summary>
        /// 4.1版本首页栏目显示的广告内容 要求广告设置为全部地区 暨DistrictID = 0
        /// </summary>
        /// <returns></returns>
        public static Advertise GenHomeOnlineAdv(long curUserID = 0)
        {
            Advertise adv = new Advertise()
            {
                Ratio = 0.7,  // 广告高宽比
                ADList = new List<ADItem>()
            };

            List<AdvBaseInfoEntity> al = GetOnlineAdvBaseInfo().FindAll(_ => _.ADType == 0);

            bool isMagiCallUser = curUserID == 0 ? false : AccountAdapter.HasUserPriviledge(curUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.MagiCallUser);
            bool isVIPUser = curUserID == 0 ? false : AccountAdapter.HasUserRole(curUserID, AccountAdapter.UserRoleEnum.VIP);
            bool isCanReserveVipUser = curUserID == 0 ? false : new CouponController().IsVipNoPayReserveUser(curUserID.ToString()) > 0;

            if (!isCanReserveVipUser)
            {
                al = al.FindAll(a => a.DistrictID == 0 && a.IsOnlyHomeShow && a.ID != 164).ToList();
            }
            if( !isMagiCallUser)
            {
                al = al.Where(a => a.DistrictID == 0 && a.IsOnlyHomeShow && a.ID != 101).ToList();
            }
                        

            foreach (AdvBaseInfoEntity a in al)
            {
                adv.ADList.Add(new ADItem { ADShowType = a.ADShowType, ADURL = (a.ADShowType == 0 ? a.BgImgUrl : a.AdUrl), ActionURL = FormatActionURL(a.ContentUrl), ADTitle = a.ADTitle });
            }

            return adv;
        }


        /// <summary>
        /// 获取指定type和user的广告列表
        /// </summary>
        /// <param name="_ContextBasicInfo"></param>
        /// <param name="adType"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public static Advertise GenHomeOnlineAdvByType(ContextBasicInfo _ContextBasicInfo, int adType = 0, long curUserID = 0)
        {
            ADTypeEntity admodel = GetAllADType().Where(_ => _.Type == adType).FirstOrDefault();
            Advertise adv = new Advertise()
            {
                Ratio = Convert.ToDouble(admodel.WidthRatio), //0.7,  // 广告高宽比
                ADList = new List<ADItem>()
            };

            //获取指定type下的所有列表
            List<AdvBaseInfoEntity> al = GetOnlineAdvBaseInfo().FindAll(_ => _.ADType == adType);

            ////获取当前用户的各种权限条件
            //bool isMagiCallUser = curUserID == 0 ? false : AccountAdapter.HasUserPriviledge(curUserID, HJD.AccountServices.Entity.PrivilegeEnums.UserPrivilege.MagiCallUser);
            //bool isCanReserveVipUser = curUserID == 0 ? false : new CouponController().IsVipNoPayReserveUser(curUserID.ToString()) > 0;
            //bool isVIPUser = curUserID == 0 ? false : AccountAdapter.HasUserRole(curUserID, AccountAdapter.UserRoleEnum.VIP);

            bool isVIPUser = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(curUserID));
            //if (!isCanReserveVipUser)
            //{
            //    al = al.FindAll(a => a.DistrictID == 0 && a.IsOnlyHomeShow && a.ID != 164).ToList();
            //}
            //if (!isMagiCallUser)
            //{
            //    al = al.Where(a => a.DistrictID == 0 && a.IsOnlyHomeShow && a.ID != 101).ToList();
            //}

            if (isVIPUser)
            {
                al = al.FindAll(_ => _.UserShowType == 0 || _.UserShowType == 2).ToList();
            }
            else
            {
                al = al.FindAll(_ => _.UserShowType == 0 || _.UserShowType == 1).ToList();
            }

            foreach (AdvBaseInfoEntity a in al)
            {
                adv.ADList.Add(new ADItem { ADShowType = a.ADShowType, ADURL = (a.ADShowType == 0 ? a.BgImgUrl : a.AdUrl), ActionURL = a.ContentUrl, ADTitle = a.ADTitle, ADUserShowType = a.UserShowType });
            }

            return adv;
        }

        /// <summary>
        /// 获取指定type、districtid 和user的广告列表
        /// </summary>
        /// <param name="_ContextBasicInfo"></param>
        /// <param name="adType"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public static Advertise GetHomeOnlineBannersByTypeAndDistrictId(ContextBasicInfo _ContextBasicInfo, int adType = 0, long curUserID = 0, int districtid = 0)
        {
            ADTypeEntity admodel = GetAllADType().Where(_ => _.Type == adType).FirstOrDefault();
            Advertise adv = new Advertise()
            {
                Ratio = admodel != null ? Convert.ToDouble(admodel.WidthRatio) : 0.7, //0.7,  // 广告高宽比
                ADList = new List<ADItem>()
            };
            List<AdvBaseInfoEntity> al = new List<AdvBaseInfoEntity>();
            //获取指定type下的所有列表
            List<AdvBaseInfoEntity> allist = GetOnlineAdvBaseInfo().FindAll(_ => _.ADType == adType);
            foreach (AdvBaseInfoEntity advBase in allist)
            {
                if (!string.IsNullOrWhiteSpace(advBase.DistrictIdStr))
                {
                    List<string> listdistrict = advBase.DistrictIdStr.Split(',').ToList();
                    if (listdistrict.Contains(districtid.ToString()))
                    {
                        al.Add(advBase);
                    }
                }
            }

            bool isVIPUser = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(curUserID));

            if (isVIPUser)
            {
                al = al.FindAll(_ => _.UserShowType == 0 || _.UserShowType == 2).ToList();
            }
            else
            {
                al = al.FindAll(_ => _.UserShowType == 0 || _.UserShowType == 1).ToList();
            }

            foreach (AdvBaseInfoEntity a in al)
            {
                adv.ADList.Add(new ADItem { ADShowType = a.ADShowType, ADURL = (a.ADShowType == 0 ? a.BgImgUrl : a.AdUrl), ActionURL = a.ContentUrl, ADTitle = a.ADTitle, ADUserShowType = a.UserShowType });
            }

            return adv;
        }
        public static List<InterestEntity> GenOnlineAdvOld()
        {

            List<InterestEntity> hl = new List<InterestEntity>();

            List<AdvBaseInfoEntity> al = GetOnlineAdvBaseInfo().FindAll(_ => _.ADType == 0);

           foreach (AdvBaseInfoEntity a in al)
           {
               hl.Add(new InterestEntity()
               {
                   Name = a.Name,
                   ID = a.ID,
                   ImageUrl = a.BgImgUrl,
                   Type = 5,
                   InterestPlaceIDs = a.ContentUrl,
                   HotelCount = 0,
                   DistrictID = 0,
                   GLat = 0,
                   GLon = 0
               });
           }

           return hl;

        }
        
        internal static string FormatActionURL(string contentURL)
        {
            if(string.IsNullOrWhiteSpace(contentURL)){
                return "";
            }
            else if (contentURL.ToLower().StartsWith("http://www.zmjiudian.com/hotel/"))
            {
                return contentURL.ToLower().Replace("http://", "whotelapp://");
            }
            else if (contentURL.ToLower().StartsWith("http://www.zmjiudian.com/coupon/shop/"))
            {
                return "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(contentURL + (contentURL.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&realuserid=1");
            }
            else if (contentURL.ToLower().StartsWith("http://www.zmjiudian.com/magicall/magicallclient"))
            {
                return "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(contentURL + (contentURL.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&realuserid=1");
            }
            else if (contentURL.ToLower().StartsWith("http://www.zmjiudian.com/fund"))
            {
                return "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(contentURL + (contentURL.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&realuserid=1");
            }
            else if (contentURL.ToLower().StartsWith("http://www.zmjiudian.com/custom/shop"))
            {
                return "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(contentURL + (contentURL.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&realuserid=1");
            }
            else if (contentURL.ToLower().StartsWith("http://www.zmjiudian.com/custom/reserve"))
            {
                return "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(contentURL + (contentURL.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&realuserid=1");
            }
            else if (contentURL.ToLower().StartsWith("whotelapp"))
            {
                return contentURL;
            }
            else
            {
                return "whotelapp://www.zmjiudian.com/gotopage?url=" + System.Web.HttpUtility.UrlEncode(contentURL + (contentURL.IndexOf("?") >= 0 ? "&" : "?") + "userid={userid}&userlat={userlat}&userlng={userlng}");
            }
        }

        /// <summary>
        /// 获取广告和地区是否存在（直接或间接的）关联关系
        /// </summary>
        /// <param name="advId"></param>
        /// <param name="districtId"></param>
        /// <returns></returns>
        private static bool IsDistrictCanSeeAd(int advId, int districtId)
        {
            return ADService.GetAdvRelSubDistrictCount(advId, districtId) > 0 ? true : false;
        }

        /// <summary>
        /// 获取广告关联的所有下级区域 如果广告设置为全部地区 则为空集合
        /// </summary>
        /// <param name="advId"></param>
        /// <returns></returns>
        private static List<AdvDistrictRelEntity> GetAdvRelSubDistrict(int advId)
        {
            return ADService.GetAdvRelSubDistrict(advId).ToList();
        }
        
        /// <summary>
        /// 根据周边地区和广告涉及的关联地区 是否有相交的来判断可否显示地区
        /// </summary>
        /// <param name="relDisrtrictIds"></param>
        /// <param name="subDistrictIds"></param>
        /// <returns></returns>
        private static bool CanShowThisAdv(IEnumerable<int> relDisrtrictIds, IEnumerable<int> subDistrictIds)
        {
            bool isOkay = false;
            foreach (var item in relDisrtrictIds)
            {
                if (subDistrictIds.Contains(item))
                {
                    isOkay = true;
                    break;
                }
            }
            return isOkay;
        }

        /// <summary>
        /// 获取某个地区周边的城市攻略列表
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="districtID"></param>
        /// <returns></returns>
        public static StrategyADResult GetStrategyADList(int districtID, float lon = 0, float lat = 0)
        {
            StrategyADResult adv = new StrategyADResult()
            {
                ADList = new List<StrategyADItem>()
            };
            
            List<ArounDistrictEntity> aroundDistrictList = HotelAdapter.CalculateNearDistrictByDistance(districtID, lat, lon).OrderBy(_=>_.Distance).ToList();//乱序 没有按距离排

            if(aroundDistrictList != null && aroundDistrictList.Count != 0){

                adv.AroundDistrictName = aroundDistrictList[0].DistrictName + "及周边—酒店攻略";//第一个是距离最近的地区
                                
                //批量获取攻略信息  查询附近所有发布状态的城市
                var aroundStrategyDistricts = ResourceAdapter.GetManyDistrictZoneEntity(aroundDistrictList.Select(_ => _.DistrictID), 1, 1);
                Regex reg = new Regex("_.+$", RegexOptions.Compiled);
                foreach (var item in aroundStrategyDistricts)
                {
                    ArounDistrictEntity aroundDistrict = aroundDistrictList.First(_ => _.DistrictID == item.DistrictID);
                    adv.ADList.Add(new StrategyADItem()
                        {
                            ADShowType = 0,
                            ADURL = string.IsNullOrWhiteSpace(item.PicUrl) ? PhotoAdapter.GenHotelPicUrl("115NMBa0Ks", HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter) : PhotoAdapter.GenHotelPicUrl(item.PicUrl, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter),// reg.Replace(item.PicUrl, "_jupiter")
                            ActionURL = FormatActionURL(string.Format("http://www.zmjiudian.com/strategy{0}", aroundDistrict.DistrictID)),
                            Distance = (int)Math.Round(aroundDistrict.Distance / 1000, 0),
                            DistrictName = aroundDistrict.DistrictName,
                            DistrictID = aroundDistrict.DistrictID
                        });
                }
            }
            return adv;
        }

        /// <summary>
        /// 由ID获取点评记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static AdvBaseInfoEntity GetAdvBaseInfoByID(int ID)
        {
            return ADService.GetAdvBaseInfoByID(ID);
        }

        /// <summary>
        /// 指定页需要的弹出框
        /// </summary>
        /// <param name="pageType"></param>
        /// <returns></returns>
        public static List<PopBoxConfigEntity> GetPopBoxConfigEntityList(int popBoxTargetNo)
        {
            var popList = ADService.GetAllPopBoxConfigList();
            return popList.FindAll(_ => !string.IsNullOrWhiteSpace(_.target) && _.target.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Contains(popBoxTargetNo.ToString()));
        }

        public static Advertise GenSelectedResort()
        {
            Advertise adv = new Advertise()
            {
                Ratio = 0.56,  // 广告高宽比
                ADList = new List<ADItem>()
            };

            List<AdvBaseInfoEntity> al = GetOnlineAdvBaseInfo().FindAll(_ => _.ADType == 1);//精选度假地

            foreach (AdvBaseInfoEntity a in al)
            {
                adv.ADList.Add(new ADItem { ADShowType = a.ADShowType, ADURL = (a.ADShowType == 0 ? a.BgImgUrl : a.AdUrl), ActionURL = FormatActionURL(a.ContentUrl), ADTitle = a.ADTitle });
            }

            return adv;
        }

    }
}