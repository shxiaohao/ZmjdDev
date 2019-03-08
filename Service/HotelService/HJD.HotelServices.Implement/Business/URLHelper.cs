using HJD.Framework.Interface;
using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Business
{
    public class URLHelper
    {
        private static IMemcacheProvider memCacheHotelURL = MemcacheManagerFactory.Create("HotelURLCache");


        private static string GetBookingUrl(int hotelID)
        {
            return memCacheHotelURL.GetData<string>("bookingUrl:" + hotelID.ToString(), () =>
            {
                return HotelDAL.GetBookingUrlByHotel(hotelID).Replace("\t", "");
            });
        }
        private static string GetAgodaUrl(int hotelID)
        {
            return memCacheHotelURL.GetData<string>("agodaUrl:" + hotelID.ToString(), () =>
            {
                return HotelDAL.GetAgodaUrlByHotel(hotelID).Replace("\t", "");
            });
        }

        private static OTACodeEntity GetOTAHotelCode(int channelid, int otahotelid)
        {
            return memCacheHotelURL.GetData<OTACodeEntity>(String.Format("OTAHotelCode:{0}:{1}", channelid, otahotelid), () =>
            {
                return HotelDAL.GetOTAHotelCode(channelid, new List<int> { otahotelid }).FirstOrDefault();
            });
        }

        private static string GetQunarHotelInfo(int hotelID)
        {
            return memCacheHotelURL.GetData<string>("QunerHotel:" + hotelID.ToString(), () =>
            {
                return "";// TODO HotelService.GetQunarHotelInfo(hotelID);
            });

        }

        private static string AddZeroFront(int otaHotelID)
        {
            List<int> ugly = new List<int> { 1101002, 1102002, 1104002, 1105002, 1801002, 1915003, 1915004, 1915005, 2301002 };
            return ugly.Contains(otaHotelID) ? otaHotelID.ToString() : otaHotelID.ToString().PadLeft(8, '0');
        }

        public static string GetOtaHotelUrl(string channel, int otaHotelID, string sourcePageType, string sType)
        {
            string bYear = "{bYear}";
            string bMonth = "{bMonth}";
            string bDay = "{bDay}";

            string eYear = "{eYear}";
            string eMonth = "{eMonth}";
            string eDay = "{eDay}";
            string url = "";
            switch (channel.ToUpper())
            {
                case "STARWAY":
                    #region
                    {
                        switch (sourcePageType.ToLower())
                        {
                            case ""://空的情况
                                sourcePageType = "jdfx";
                                break;
                            case "list":
                                sourcePageType = "jdfx";
                                break;
                            case "detail":
                                sourcePageType = "jdfx";
                                break;
                            case "link":
                                sourcePageType = "zjlj";
                                break;
                            default:
                                sourcePageType = "jdfx";
                                break;
                        }
                        return "http://www.xingcheng.com/online/exchannel/smartlink.aspx?spread=08&channel=zmjiudian&linktype=" + sourcePageType + "&returnurl=online%2fShowHotelInfo.aspx%3fhotel%3d" + otaHotelID + "%26startDate%3d" + "{bYear}-{bMonth}-{bDay}" + "%26enddate%3d" + "{eYear}-{eMonth}-{eDay}" + "";
                    }
                    #endregion

                case "QUNAR":
                    #region
                    {
                        OTACodeEntity CE = GetOTAHotelCode(26, otaHotelID);

                        return string.Format("http://h.qunar.com/preDetail.jsp?seq={0}&days={{day}}&checkin={{bYear}}{{bMonth}}{{bDay}}&city={1}&mobileRoomStatus=2&v=2&from=zmjiudian&bd_source=",
                            CE.OTAHotelCode, CE.OTACity);
                    }
                    #endregion

                case "HOLSUN":
                    #region
                    {
                        switch (sourcePageType.ToLower())
                        {
                            case ""://空的情况
                                sourcePageType = "zmjiudian2";
                                break;
                            case "list":
                                sourcePageType = "zmjiudian2";
                                break;
                            case "detail":
                                sourcePageType = "zmjiudian2";
                                break;
                            case "link":
                                sourcePageType = "zmjiudian3";
                                break;
                            default:
                                sourcePageType = "zmjiudian1";
                                break;
                        }
                        return "http://hotel.holsun.com/" + sourcePageType + "/hotel_" + otaHotelID + ".html?checkin=" + "{bYear}-{bMonth}-{bDay}" + "&checkout=" + "{eYear}-{eMonth}-{eDay}" + "";
                    }
                    #endregion

                case "SEVENDAYS":
                    #region
                    {
                        switch (sourcePageType.ToLower())
                        {
                            case ""://空的情况
                                sourcePageType = "ad1";
                                break;
                            case "list":
                                sourcePageType = "ad1";
                                break;
                            case "detail":
                                sourcePageType = "ad3";
                                break;
                            case "link":
                                sourcePageType = "ad2";
                                break;
                            default:
                                sourcePageType = "ad1";
                                break;
                        }
                        return "http://e.7daysinn.cn/marketact/active/outbking/innpage_bk.php?id=" + otaHotelID + "&sid=112068&rid=" + sourcePageType + "&dtstart=" + "{bYear}-{bMonth}-{bDay}" + "&dtend=" + "{eYear}-{eMonth}-{eDay}" + "";
                    }
                    #endregion

                case "ZHUNA":
                    #region
                    {
                        switch (sourcePageType.ToLower())
                        {
                            case ""://空的情况
                                sourcePageType = "";
                                break;
                            case "list":
                                sourcePageType = "";
                                break;
                            case "detail":
                                sourcePageType = "";
                                break;
                            case "link":
                                sourcePageType = "45";
                                break;
                            default:
                                sourcePageType = "43";
                                break;
                        }
                        return "http://www.zhuna.cn/hotel-" + otaHotelID + ".html?agent_id=191&zn_qudao=" + sourcePageType + "&tm1=" + "{bYear}-{bMonth}-{bDay}" + "&tm2=" + "{eYear}-{eMonth}-{eDay}" + "";
                    }
                    #endregion

                #region PODINNS

                //case "PODINNS":

                //    switch (sourcePageType.ToLower())
                //    {
                //        case ""://空的情况
                //            sourcePageType = "6354";
                //            break;
                //        case "list":
                //            sourcePageType = "6353";
                //            break;
                //        case "detail":
                //            sourcePageType = "6354";
                //            break;
                //        case "detailreview"://酒店详细页列表
                //            sourcePageType = "6512";
                //            break;
                //        case "link":
                //            sourcePageType = "6356";
                //            break;
                //        default:
                //            sourcePageType = "6355";
                //            break;
                //    }
                //    if (sourcePageType == "6356")
                //        return "http://www.podinns.com/Hotel/HotelFirstLessMoney?from=6356";
                //    else
                //    {
                //        string realHotelid = zmjiudian.Business.DestinationBusiness.Business.Hotel.GetBookingUrlByHotel(otaHotelID.ToString(), "PODINNS");
                //        return "http://www.podinns.com/Hotel/HotelDetailzmjiudian/" + realHotelid + ".html?from=" + sourcePageType + "&InDate=" + "{bYear}-{bMonth}-{bDay}" + "&InDate2=" + "{eYear}-{eMonth}-{eDay}" + "";
                //    }

                #endregion

                case "HRS":
                    #region
                    {
                        if (sourcePageType == "0")
                            return "http://www.hrs.com/web3/hotelData.do?client=zh_CN&activity=offer&hotelnumber=" + otaHotelID +
                       "&DCMP=KNC-CN0910zmjiudian&customerID=860000487&singleRooms=1&startDateDay=" + "{bDay}" +
                       "&startDateMonth={bMonth}" + "{bMonth}" +
                       "&startDateYear=" + "{bYear}" +
                       "&endDateDay=" + "{eDay}" +
                       "&endDateMonth=" + "{eMonth}" +
                       "&endDateYear=" + "{eYear}" + "&availability=check&hrsReferer=hrs.cn";
                        else
                            return "http://www.hrs.com/web3/hotelData.do?client=zh_CN&activity=offer&hotelnumber=" + otaHotelID +
                            "&DCMP=KNC-CN0910zmjiudian&customerID=860000487&doubleRooms=1&adults=2&startDateDay=" + "{bDay}" +
                            "&startDateMonth=" + "{bMonth}" +
                            "&startDateYear=" + "{bYear}" +
                            "&endDateDay=" + "{eDay}" +
                            "&endDateMonth=" + "{eMonth}" +
                            "&endDateYear=" + "{eYear}" + "&availability=check&hrsReferer=hrs.cn";
                    }
                    #endregion

                case "ELONG":
                    #region
                    {
                        if (sType.ToLower() == "www")
                        {
                            return "http://hotel.elong.com/search/" + AddZeroFront(otaHotelID) + "-hotel/?indate={eYear}-{eMonth}-{bDay}&outdate={eYear}-{eMonth}-{eDay}&banid=zmjiudian";

                        }
                        else
                        {
                            return "http://m.elong.com/hotel/city/" + AddZeroFront(otaHotelID) + "/?checkindate={bYear}-{bMonth}-{bDay}&checkoutdate={eYear}-{eMonth}-{eDay}&ref=mzmjiudian";
                        }
                        break;
                    }
                    #endregion

                case "JINJIANG":
                    #region
                    {
                        string HotelID = otaHotelID.ToString();
                        switch (4 - HotelID.Length)
                        {
                            case 1:
                                HotelID = "0" + HotelID.ToString();
                                break;
                            case 2:
                                HotelID = "00" + HotelID.ToString();
                                break;
                            case 3:
                                HotelID = "000" + HotelID.ToString();
                                break;
                            default:
                                break;
                        }
                        switch (sourcePageType.ToLower())
                        {
                            case ""://空的情况
                                sourcePageType = "searching";
                                break;
                            case "list":
                                sourcePageType = "searching";
                                break;
                            case "detail":
                                sourcePageType = "booking";
                                break;
                            case "detailreview":
                                sourcePageType = "booking";
                                break;
                            case "link":
                                sourcePageType = "word";
                                break;
                            default:
                                sourcePageType = "price";
                                break;
                        }
                        return "http://www.jinjianginns.com/ResvUrl.aspx?sid=715&UnitID=" + HotelID + "&Tn=" + sourcePageType + "&gotoUrl=resv1&DateCheckIn=" + "{bYear}-{bMonth}-{bDay}" + "&DateCheckOut=" + "{eYear}-{eMonth}-{eDay}" + "";
                    }
                    #endregion

                case "TONGCHENG":
                    #region
                    {
                        return "http://touch.17u.com/hotel/jiudian_" + otaHotelID + ".html?comedate=" + "{bYear}-{bMonth}-{bDay}" + "&leavedate=" + "{eYear}-{eMonth}-{eDay}" + "";

                        //if (sourcePageType == "link")
                        //    return "http://www.17u.cn/hotelinfo_news.aspx?id=" + otaHotelID + "&refid=8713057&comedate=" + "{bYear}-{bMonth}-{bDay}" + "&leavedate=" + "{eYear}-{eMonth}-{eDay}" + "";
                        //else
                        //    return "http://www.17u.cn/hotelinfo_news.aspx?id=" + otaHotelID + "&refid=7884926&comedate=" + "{bYear}-{bMonth}-{bDay}" + "&leavedate=" + "{eYear}-{eMonth}-{eDay}" + "";
                    }
                    #endregion

                case "CTRIP":
                    #region
                    {
                     int allianceid = 731899;
                     int sid = 1273259;
                        //switch (sType.ToLower())
                        //{
                        //    case "attios":
                        //        sid = 383041;//	 景区酒店iOS	
                        //        break;
                        //    case "attandroid":
                        //        sid = 383056;//	 景区酒店android	
                        //        break;
                        //    case "m":
                        //        sid = 324381;//	 周末酒店wap	
                        //        break;
                        //    case "whios":
                        //        sid = 383057;//	 周末酒店ios	
                        //        break;
                        //    case "whandroid":
                        //        sid = 383058;	// 周末酒店android
                        //        break;
                        //    default:
                        //        sid = 324378;
                        //        break;
                        //}

                        if (sType.ToLower() == "www")
                        {
                            return "http://u.ctrip.com/union/CtripRedirect.aspx?TypeID=272&Allianceid=731899&sid=1273259&ouid=" + otaHotelID + "&CheckInDate={bYear}-{bMonth}-{bDay}&CheckOutDate={eYear}-{eMonth}-{eDay}&HotelID=" + otaHotelID;
                        }
                        else
                        {
                            return "http://m.ctrip.com/html5/Hotel/HotelDetail/" + otaHotelID + ".html?atime={bYear}{bMonth}{bDay}&day={day}&allianceid=" + allianceid.ToString() + "&sid=" + sid.ToString() + "&ouID=" + otaHotelID;
                        }
                        //  return "http://hotels.ctrip.com/Domestic/ShowHotelInformation.aspx?hotel=" + otaHotelID + "&StartDate=" + "{bYear}-{bMonth}-{bDay}" + "&DepDate=" + "{eYear}-{eMonth}-{eDay}" + "&allianceid=7676&sid="+sid.ToString()+"&ouid=" + otaHotelID;    
                    }
                    #endregion

                case "HOTEL":
                    #region
                    {
                        string deeplink = "http://cn.hotels.com/PPCHotelDetails?hotelid=" + otaHotelID + "&arrivalDate=" + Convert.ToDateTime("{bYear}-{bMonth}-{bDay}").ToString("dd/MM/yyyy") + "&departureDate=" + Convert.ToDateTime("{eYear}-{eMonth}-{eDay}").ToString("dd/MM/yyyy") + "&pos=HCOM_CN&locale=zh_CN ";
                        return "http://ad.doubleclick.net/clk;242815819;65715575;u?" + deeplink + "&rffrid=mdp.hcom.cn.173.000.02.01";
                    }
                    #endregion

                case "BOOKING":
                    #region
                    {
                        // &checkin_monthday=15&checkin_year_month=2011-9
                        //&checkout_monthday=18&checkout_year_month=2011-9
                        //&do_availability_check=1
                        if (sourcePageType == "" || sourcePageType.ToLower() == "detail")//为空表示是右上角过来的 text-cr
                            sourcePageType = "text-cr";
                        else
                            sourcePageType = "text-cs";

                        int aid = 847369;
                        if (sType.ToLower() == "app")
                        {
                            aid = 863410;
                        }
                        else if (sType.ToLower() == "m")
                        {
                            aid = 863412;
                        }

                        string bookingUrl = GetBookingUrl(otaHotelID);//.Replace("www.booking.com", "m.booking.com");
                        switch (sType.ToLower())
                        {
                            case "www": break;
                            default: bookingUrl = bookingUrl.Replace("www.booking.com", "m.booking.com"); break;
                        }

                        bookingUrl += string.Format("?aid={0}&checkin_monthday={{bDay}}&checkin_year_month={{bYear}}-{{bMonth}}&checkout_monthday={{eDay}}&checkout_year_month={{eYear}}-{{eMonth}}&do_availability_check=1&utm_content=text-cs", aid);

                        return bookingUrl;
                    }
                    #endregion

                case "AGODA":
                    #region
                    {
                        var agodaUrl = GetAgodaUrl(otaHotelID);
                        var cid = 1739698;
                        var tag = "hid" + otaHotelID;
                        var los = 1;

                        agodaUrl += string.Format("?checkin={{bYear}}-{{bMonth}}-{{eDay}}&los={1}&cid={0}&tag={2}", cid, los, tag);
                        return agodaUrl;
                    }
                    #endregion

                case "8848U":
                    #region
                    {
                        return string.Format("http://www.8848u.com/hotel/HotelInfo-{0}.html?refId=11421454&comeDate={1}&leaveDate={2}",
                        otaHotelID,
                        "{bYear}-{bMonth}-{bDay}",
                        "{eYear}-{eMonth}-{eDay}");
                    }
                    #endregion

                case "128UU":
                    #region
                    {
                        string tmpStr = string.Format("http://www.hotelhotel.cn/hotel/hotel_info.asp?hotelid={0}&intime={1}&outtime={2}&aid=164004",
                        otaHotelID,
                        "{bYear}-{bMonth}-{bDay}",
                        "{eYear}-{eMonth}-{eDay}");
                        return tmpStr;
                        //return "http://union.128uu.com/user/adsstatic.aspx?url=" + HttpUtility.UrlEncode(tmpStr);
                    }
                    #endregion

                case "LAIDINGBA":
                    #region
                    {
                        string laiyuan = "zmjiudian";
                        if ("link".Equals(sourcePageType, StringComparison.OrdinalIgnoreCase))
                            laiyuan = "zmjiudianad";

                        return string.Format("http://www.laidingba.com/hotel_{0}.html?sdate={1}&edate={2}&laiyuan={3}",
                            otaHotelID,
                            "{bYear}-{bMonth}-{bDay}",
                            "{eYear}-{eMonth}-{eDay}",
                            laiyuan);
                    }
                    #endregion

                case "SUNNYCHINA":
                    #region
                    {
                        string canid = "zmjiudian";
                        if ("link".Equals(sourcePageType, StringComparison.OrdinalIgnoreCase))
                            canid = "zmjiudianAd";

                        return string.Format("http://www.sunnychina.com/hotel/hotel_info.asp?hotelid={0}&startdate={1}&endDate={2}&canid={3}",
                            otaHotelID,
                            "{bYear}-{bMonth}-{bDay}",
                            "{eYear}-{eMonth}-{eDay}",
                            canid);
                    }
                    #endregion

                default:
                    return "http://hotels.ctrip.com/Domestic/ShowHotelInfo.aspx?hotel=" + otaHotelID + "&StartDate=" + "{bYear}-{bMonth}-{bDay}" + "&DepDate=" + "{eYear}-{eMonth}-{eDay}" + "&allianceid=7676&sid=174373&ouid=" + otaHotelID;
            }
            //return url;

        }

        public static List<HotelOTAEntity> GetOtaListByHotelID(int HotelID)
        {
            return HotelDAL.GetOtaListByHotelID(HotelID);
        }
        public static Dictionary<string, string> GetOtaUrlListByHotelID(int localHotelID, string sourcePageType, string sType)
        {
            string bYear = "{bYear}";
            string bMonth = "{bMonth}";
            string bDay = "{bDay}";

            string eYear = "{eYear}";
            string eMonth = "{eMonth}";
            string eDay = "{eDay}";
            string url = "";
            Dictionary<string, string> dicreturn = new Dictionary<string, string>();
            List<HotelOTAEntity> listota = HotelDAL.GetOtaListByHotelID(localHotelID);
            string otaCode = string.Empty;
            string otaUrl = string.Empty;
            foreach (var item in listota)
            {
                switch (item.ChannelCode.ToUpper())
                {
                    case "STARWAY":
                        #region
                        {
                            switch (sourcePageType.ToLower())
                            {
                                case ""://空的情况
                                    sourcePageType = "jdfx";
                                    break;
                                case "list":
                                    sourcePageType = "jdfx";
                                    break;
                                case "detail":
                                    sourcePageType = "jdfx";
                                    break;
                                case "link":
                                    sourcePageType = "zjlj";
                                    break;
                                default:
                                    sourcePageType = "jdfx";
                                    break;
                            }
                            otaUrl = " http://www.xingcheng.com/online/exchannel/smartlink.aspx?spread=08&channel=zmjiudian&linktype=" + sourcePageType + "&returnurl=online%2fShowHotelInfo.aspx%3fhotel%3d" + item.HotelOriID + "%26startDate%3d" + "{bYear}-{bMonth}-{bDay}" + "%26enddate%3d" + "{eYear}-{eMonth}-{eDay}" + "";
                            break;
                        }
                        #endregion

                    case "QUNAR":
                        #region
                        {
                            OTACodeEntity CE = GetOTAHotelCode(26, item.HotelOriID);

                            otaUrl = string.Format("http://h.qunar.com/preDetail.jsp?seq={0}&days={{day}}&checkin={{bYear}}{{bMonth}}{{bDay}}&city={1}&mobileRoomStatus=2&v=2&from=zmjiudian&bd_source=",
                                CE.OTAHotelCode, CE.OTACity);
                            break;
                        }
                        #endregion

                    case "HOLSUN":
                        #region
                        {
                            switch (sourcePageType.ToLower())
                            {
                                case ""://空的情况
                                    sourcePageType = "zmjiudian2";
                                    break;
                                case "list":
                                    sourcePageType = "zmjiudian2";
                                    break;
                                case "detail":
                                    sourcePageType = "zmjiudian2";
                                    break;
                                case "link":
                                    sourcePageType = "zmjiudian3";
                                    break;
                                default:
                                    sourcePageType = "zmjiudian1";
                                    break;
                            }
                            otaUrl = "http://hotel.holsun.com/" + sourcePageType + "/hotel_" + item.HotelOriID + ".html?checkin=" + "{bYear}-{bMonth}-{bDay}" + "&checkout=" + "{eYear}-{eMonth}-{eDay}" + "";
                            break;
                        }
                        #endregion

                    case "SEVENDAYS":
                        #region
                        {
                            switch (sourcePageType.ToLower())
                            {
                                case ""://空的情况
                                    sourcePageType = "ad1";
                                    break;
                                case "list":
                                    sourcePageType = "ad1";
                                    break;
                                case "detail":
                                    sourcePageType = "ad3";
                                    break;
                                case "link":
                                    sourcePageType = "ad2";
                                    break;
                                default:
                                    sourcePageType = "ad1";
                                    break;
                            }
                            otaUrl = "http://e.7daysinn.cn/marketact/active/outbking/innpage_bk.php?id=" + item.HotelOriID + "&sid=112068&rid=" + sourcePageType + "&dtstart=" + "{bYear}-{bMonth}-{bDay}" + "&dtend=" + "{eYear}-{eMonth}-{eDay}" + "";
                            break;
                        }
                        #endregion

                    case "ZHUNA":
                        #region
                        {
                            switch (sourcePageType.ToLower())
                            {
                                case ""://空的情况
                                    sourcePageType = "";
                                    break;
                                case "list":
                                    sourcePageType = "";
                                    break;
                                case "detail":
                                    sourcePageType = "";
                                    break;
                                case "link":
                                    sourcePageType = "45";
                                    break;
                                default:
                                    sourcePageType = "43";
                                    break;
                            }
                            otaUrl = "http://www.zhuna.cn/hotel-" + item.HotelOriID + ".html?agent_id=191&zn_qudao=" + sourcePageType + "&tm1=" + "{bYear}-{bMonth}-{bDay}" + "&tm2=" + "{eYear}-{eMonth}-{eDay}" + "";
                            break;
                        }
                        #endregion

                    #region PODINNS

                    //case "PODINNS":

                    //    switch (sourcePageType.ToLower())
                    //    {
                    //        case ""://空的情况
                    //            sourcePageType = "6354";
                    //            break;
                    //        case "list":
                    //            sourcePageType = "6353";
                    //            break;
                    //        case "detail":
                    //            sourcePageType = "6354";
                    //            break;
                    //        case "detailreview"://酒店详细页列表
                    //            sourcePageType = "6512";
                    //            break;
                    //        case "link":
                    //            sourcePageType = "6356";
                    //            break;
                    //        default:
                    //            sourcePageType = "6355";
                    //            break;
                    //    }
                    //    if (sourcePageType == "6356")
                    //        return "http://www.podinns.com/Hotel/HotelFirstLessMoney?from=6356";
                    //    else
                    //    {
                    //        string realHotelid = zmjiudian.Business.DestinationBusiness.Business.Hotel.GetBookingUrlByHotel(otaHotelID.ToString(), "PODINNS");
                    //        return "http://www.podinns.com/Hotel/HotelDetailzmjiudian/" + realHotelid + ".html?from=" + sourcePageType + "&InDate=" + "{bYear}-{bMonth}-{bDay}" + "&InDate2=" + "{eYear}-{eMonth}-{eDay}" + "";
                    //    }

                    #endregion

                    case "HRS":
                        #region
                        {
                            if (sourcePageType == "0")
                                otaUrl = "http://www.hrs.com/web3/hotelData.do?client=zh_CN&activity=offer&hotelnumber=" + item.HotelOriID +
                           "&DCMP=KNC-CN0910zmjiudian&customerID=860000487&singleRooms=1&startDateDay=" + "{bDay}" +
                           "&startDateMonth={bMonth}" + "{bMonth}" +
                           "&startDateYear=" + "{bYear}" +
                           "&endDateDay=" + "{eDay}" +
                           "&endDateMonth=" + "{eMonth}" +
                           "&endDateYear=" + "{eYear}" + "&availability=check&hrsReferer=hrs.cn";
                            else
                                otaUrl = "http://www.hrs.com/web3/hotelData.do?client=zh_CN&activity=offer&hotelnumber=" + item.HotelOriID +
                                "&DCMP=KNC-CN0910zmjiudian&customerID=860000487&doubleRooms=1&adults=2&startDateDay=" + "{bDay}" +
                                "&startDateMonth=" + "{bMonth}" +
                                "&startDateYear=" + "{bYear}" +
                                "&endDateDay=" + "{eDay}" +
                                "&endDateMonth=" + "{eMonth}" +
                                "&endDateYear=" + "{eYear}" + "&availability=check&hrsReferer=hrs.cn";
                            break;
                        }
                        #endregion

                    case "ELONG":
                        #region
                        {
                            if (sType.ToLower() == "www")
                            {
                                //otaUrl = "http://hotel.elong.com/search/" + AddZeroFront(item.HotelOriID) + "-hotel/?indate={eYear}-{eMonth}-{bDay}&outdate={eYear}-{eMonth}-{eDay}&banid=zmjiudian";
                                otaUrl = "http://hotel.elong.com/search/" + AddZeroFront(item.HotelOriID) + "-hotel/?indate={bYear}-{bMonth}-{bDay}&outdate={eYear}-{eMonth}-{eDay}&banid=zmjiudian";
                            }
                            else
                            {
                                otaUrl = "http://m.elong.com/hotel/city/" + AddZeroFront(item.HotelOriID) + "/?checkindate={bYear}-{bMonth}-{bDay}&checkoutdate={eYear}-{eMonth}-{eDay}&ref=mzmjiudian";
                            }
                            break;
                        }
                        #endregion

                    case "JINJIANG":
                        #region
                        {
                            string HotelID = item.HotelOriID.ToString();
                            switch (4 - HotelID.Length)
                            {
                                case 1:
                                    HotelID = "0" + HotelID.ToString();
                                    break;
                                case 2:
                                    HotelID = "00" + HotelID.ToString();
                                    break;
                                case 3:
                                    HotelID = "000" + HotelID.ToString();
                                    break;
                                default:
                                    break;
                            }
                            switch (sourcePageType.ToLower())
                            {
                                case ""://空的情况
                                    sourcePageType = "searching";
                                    break;
                                case "list":
                                    sourcePageType = "searching";
                                    break;
                                case "detail":
                                    sourcePageType = "booking";
                                    break;
                                case "detailreview":
                                    sourcePageType = "booking";
                                    break;
                                case "link":
                                    sourcePageType = "word";
                                    break;
                                default:
                                    sourcePageType = "price";
                                    break;
                            }
                            otaUrl = "http://www.jinjianginns.com/ResvUrl.aspx?sid=715&UnitID=" + HotelID + "&Tn=" + sourcePageType + "&gotoUrl=resv1&DateCheckIn=" + "{bYear}-{bMonth}-{bDay}" + "&DateCheckOut=" + "{eYear}-{eMonth}-{eDay}" + "";
                            break;
                        }
                        #endregion

                    case "TONGCHENG":
                        #region
                        {
                            otaUrl = "http://touch.17u.com/hotel/jiudian_" + item.HotelOriID + ".html?comedate=" + "{bYear}-{bMonth}-{bDay}" + "&leavedate=" + "{eYear}-{eMonth}-{eDay}" + "";
                            break;
                            //if (sourcePageType == "link")
                            //    return "http://www.17u.cn/hotelinfo_news.aspx?id=" + otaHotelID + "&refid=8713057&comedate=" + "{bYear}-{bMonth}-{bDay}" + "&leavedate=" + "{eYear}-{eMonth}-{eDay}" + "";
                            //else
                            //    return "http://www.17u.cn/hotelinfo_news.aspx?id=" + otaHotelID + "&refid=7884926&comedate=" + "{bYear}-{bMonth}-{bDay}" + "&leavedate=" + "{eYear}-{eMonth}-{eDay}" + "";
                        }
                        #endregion

                    case "CTRIP":
                        #region
                        {
                        int allianceid = 731899;
                        int sid = 1273259;
                            //switch (sType.ToLower())
                            //{
                            //    case "attios":
                            //        sid = 383041;//	 景区酒店iOS	
                            //        break;
                            //    case "attandroid":
                            //        sid = 383056;//	 景区酒店android	
                            //        break;
                            //    case "m":
                            //        sid = 324381;//	 周末酒店wap	
                            //        break;
                            //    case "whios":
                            //        sid = 383057;//	 周末酒店ios	
                            //        break;
                            //    case "whandroid":
                            //        sid = 383058;	// 周末酒店android
                            //        break;
                            //    default:
                            //        sid = 324378;
                            //        break;
                            //}

                            if (sType.ToLower() == "www")
                            {
                                otaUrl = "http://u.ctrip.com/union/CtripRedirect.aspx?TypeID=272&Allianceid=731899&sid=1273259&ouid=" + item.HotelOriID + "&CheckInDate={bYear}-{bMonth}-{bDay}&CheckOutDate={eYear}-{eMonth}-{eDay}&HotelID=" + item.HotelOriID;

                            }
                            else
                            {
                                otaUrl = "http://m.ctrip.com/html5/Hotel/HotelDetail/" + item.HotelOriID + ".html?atime={bYear}{bMonth}{bDay}&day={day}&allianceid=" + allianceid.ToString() + "&sid=" + sid.ToString() + "&ouID=" + item.HotelOriID;
                            }
                            //  return "http://hotels.ctrip.com/Domestic/ShowHotelInformation.aspx?hotel=" + otaHotelID + "&StartDate=" + "{bYear}-{bMonth}-{bDay}" + "&DepDate=" + "{eYear}-{eMonth}-{eDay}" + "&allianceid=7676&sid="+sid.ToString()+"&ouid=" + otaHotelID;    
                            break;
                        }
                        #endregion

                    case "HOTEL":
                        #region
                        {
                            string deeplink = "http://cn.hotels.com/PPCHotelDetails?hotelid=" + item.HotelOriID + "&arrivalDate=" + Convert.ToDateTime("{bYear}-{bMonth}-{bDay}").ToString("dd/MM/yyyy") + "&departureDate=" + Convert.ToDateTime("{eYear}-{eMonth}-{eDay}").ToString("dd/MM/yyyy") + "&pos=HCOM_CN&locale=zh_CN ";
                            otaUrl = "http://ad.doubleclick.net/clk;242815819;65715575;u?" + deeplink + "&rffrid=mdp.hcom.cn.173.000.02.01";
                            break;
                        }
                        #endregion

                    case "BOOKING":
                        #region
                        {
                            // &checkin_monthday=15&checkin_year_month=2011-9
                            //&checkout_monthday=18&checkout_year_month=2011-9
                            //&do_availability_check=1
                            if (sourcePageType == "" || sourcePageType.ToLower() == "detail")//为空表示是右上角过来的 text-cr
                                sourcePageType = "text-cr";
                            else
                                sourcePageType = "text-cs";

                            int aid = 847369;
                            if (sType.ToLower() == "app")
                            {
                                aid = 863410;
                            }
                            else if (sType.ToLower() == "m")
                            {
                                aid = 863412;
                            }

                            string bookingUrl = GetBookingUrl(item.HotelOriID);//.Replace("www.booking.com", "m.booking.com");
                            switch (sType.ToLower())
                            {
                                case "www": break;
                                default: bookingUrl = bookingUrl.Replace("www.booking.com", "m.booking.com"); break;
                            }

                            bookingUrl += string.Format("?aid={0}&checkin_monthday={{bDay}}&checkin_year_month={{bYear}}-{{bMonth}}&checkout_monthday={{eDay}}&checkout_year_month={{eYear}}-{{eMonth}}&do_availability_check=1&utm_content=text-cs", aid);

                            otaUrl = bookingUrl;
                            break;
                        }
                        #endregion

                    case "AGODA":
                        #region
                        {
                            var agodaUrl = GetAgodaUrl(item.HotelOriID);
                            var cid = 1739698;
                            var tag = "hid" + item.HotelOriID;
                            var los = 1;

                            //agodaUrl += string.Format("?checkin={{bYear}}-{{bMonth}}-{{eDay}}&los={1}&cid={0}&tag={2}", cid, los, tag);
                            agodaUrl += string.Format("?checkin={{bYear}-{bMonth}-{bDay}}&los={1}&cid={0}&tag={2}", cid, los, tag);
                            otaUrl = agodaUrl;
                            break;
                        }
                        #endregion

                    case "8848U":
                        #region
                        {
                            otaUrl = string.Format("http://www.8848u.com/hotel/HotelInfo-{0}.html?refId=11421454&comeDate={1}&leaveDate={2}",
                            item.HotelOriID,
                            "{bYear}-{bMonth}-{bDay}",
                            "{eYear}-{eMonth}-{eDay}");
                            break;
                        }
                        #endregion

                    case "128UU":
                        #region
                        {
                            string tmpStr = string.Format("http://www.hotelhotel.cn/hotel/hotel_info.asp?hotelid={0}&intime={1}&outtime={2}&aid=164004",
                            item.HotelOriID,
                            "{bYear}-{bMonth}-{bDay}",
                            "{eYear}-{eMonth}-{eDay}");
                            otaUrl = tmpStr;
                            //return "http://union.128uu.com/user/adsstatic.aspx?url=" + HttpUtility.UrlEncode(tmpStr);
                            break;
                        }
                        #endregion

                    case "LAIDINGBA":
                        #region
                        {
                            string laiyuan = "zmjiudian";
                            if ("link".Equals(sourcePageType, StringComparison.OrdinalIgnoreCase))
                                laiyuan = "zmjiudianad";

                            otaUrl = string.Format("http://www.laidingba.com/hotel_{0}.html?sdate={1}&edate={2}&laiyuan={3}",
                                item.HotelOriID,
                                "{bYear}-{bMonth}-{bDay}",
                                "{eYear}-{eMonth}-{eDay}",
                                laiyuan);
                            break;
                        }
                        #endregion

                    case "SUNNYCHINA":
                        #region
                        {
                            string canid = "zmjiudian";
                            if ("link".Equals(sourcePageType, StringComparison.OrdinalIgnoreCase))
                                canid = "zmjiudianAd";

                            otaUrl = string.Format("http://www.sunnychina.com/hotel/hotel_info.asp?hotelid={0}&startdate={1}&endDate={2}&canid={3}",
                                item.HotelOriID,
                                "{bYear}-{bMonth}-{bDay}",
                                "{eYear}-{eMonth}-{eDay}",
                                canid);
                            break;
                        }
                        #endregion

                    default:
                        otaUrl = "http://hotels.ctrip.com/Domestic/ShowHotelInfo.aspx?hotel=" + item.HotelOriID + "&StartDate=" + "{bYear}-{bMonth}-{bDay}" + "&DepDate=" + "{eYear}-{eMonth}-{eDay}" + "&allianceid=7676&sid=174373&ouid=" + item.HotelOriID;
                        break;
                }
                otaCode = item.ChannelName;
                if (!dicreturn.Keys.Contains(otaCode))
                {
                    dicreturn.Add(otaCode, otaUrl);
                }
            }
            return dicreturn;
            //return url;

        }
    }
}
