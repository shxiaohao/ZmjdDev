using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using System.Web.Routing;
using HJD.HotelServices.Contracts;

namespace WHotelSite.Params.Hotel
{
    public class ListParam : ParamBase
    {
        public int City;
        public int ScType;
        public int Start;
        public int Count;
        public int Sort;

        // Filters
        public int Interest;
        public string Distance;
        public string Price;
        public int Star;

        public int MinPrice;
        public int MaxPrice;
        public int FromDistance;
        public int ToDistance;

        const int PageSize = 15;

        public ListParam(Controller controller)
            : base(controller)
        {
            Start = GetIntFromQuery("start", 0);
            Count = PageSize;
            Sort = GetIntFromQuery("sort", 0);
            Distance = GetStringFromQuery("distance", null);
            Price = GetStringFromQuery("price", null);
            Star = GetIntFromQuery("star", 0);

            SplitRange(ref Distance, ref FromDistance, ref ToDistance);
            SplitRange(ref Price, ref MinPrice, ref MaxPrice);
        }

        private void SplitRange(ref string str, ref int a, ref int b)
        {
            if (str != null)
            {
                Match m = Regex.Match(str, @"^(\d+),(\d+)$");
                try
                {
                    a = Convert.ToInt32(m.Groups[1].Value);
                    b = Convert.ToInt32(m.Groups[2].Value);
                }
                catch (Exception)
                {
                    a = 0;
                    b = 0;
                    str = null;
                }
            }
        }

    }

    public class DetailParam : ParamBase
    {
        public int HotelId;
        public string CheckIn;
        public string CheckOut;
        public int pid;

        public DateTime CheckInDate
        {
            get { return Utils.ParseDate(CheckIn, DateTime.Today); }
            set { CheckIn = Utils.FormatDate(value); }
        }

        public DateTime CheckOutDate
        {
            get { return Utils.ParseDate(CheckOut, CheckInDate.AddDays(1)); }
            set { CheckOut = Utils.FormatDate(value); }
        }

        public string CheckOutDateStr
        {
            get { return Utils.FormatDate(CheckOutDate); }
        }

        public DetailParam(Controller controller)
            : base(controller)
        {
            int tempId = GetInt("hotel", 0);
            HotelId = tempId == 0 ? GetIntFromQuery("hotel", 0) : tempId;
            CheckIn = GetStringFromQuery("checkIn", "0");
            CheckOut = GetStringFromQuery("checkOut", "0");
        }
    }

    public class PackagesParam : DetailParam
    {
        public PackagesParam(Controller controller)
            : base(controller)
        {
            int.TryParse(controller.Request.QueryString["pid"], out pid);
        }
    }

    public class PackageCalendarParam : ParamBase
    {
        public int HotelId;
        public int PackageId;
        public PackageCalendarParam(Controller controller)
            : base(controller)
        {
            int tempId = GetInt("hotel", 0);
            HotelId = tempId == 0 ? GetIntFromQuery("hotel", 0) : tempId;
            PackageId = GetIntFromQuery("package", 0);
        }
    }

    public class BookParam : DetailParam
    {
        public int PackageId;
        public int RoomCount;
        public string[] Contacts;
        public string ContactPhone;
        public string[] Notes;

        public BookParam(Controller controller)
            : base(controller)
        {
            PackageId = GetIntFromQuery("package", 0);
            RoomCount = GetIntFromQuery("roomCount", 1);
            Contacts = GetStringFromQuery("contact", "").Split(new char[] { ',' });
            ContactPhone = GetStringFromQuery("contactPhone", "");
            Notes = GetStringFromQuery("note", "").Split(new char[] { ' ' });

            if (RoomCount < 1 || RoomCount > 100)
            {
                throw new Exception("参数无效");
            }

            if (Contacts.Length != RoomCount)
            {
                List<string> list = new List<string>();
                for (int i = 0; i < RoomCount; ++i)
                {
                    list.Add(i < Notes.Length ? Contacts[i] : "");
                }
                Contacts = list.ToArray();
            }
        }
    }

    public class SubmitParam : BookParam
    {
        public string Contact;
        public string Note;
        public string Channel;
        public bool isChannelNull;
        public string TravelPersons;
        public string AirPersons;

        public SubmitParam(Controller controller)
            : base(controller)
        {
            String channelParam = (String)controller.Request.QueryString["channel"];
            if (string.IsNullOrEmpty(channelParam))
            {
                Channel = "alipay";
                isChannelNull = true;
            }
            else
            {
                isChannelNull = false;
                Channel = channelParam;
            }
            Contact = controller.Request.Form["contact"];
            ContactPhone = controller.Request.Form["contactPhone"];
            RoomCount = Int32.Parse(controller.Request.Form["roomCount"]);
            Note = controller.Request.Form["note"];
            TravelPersons = controller.Request.Form["travelPersons"];
            AirPersons = controller.Request.Form["airPersons"];
        }
    }

    public class ReviewsParam : ParamBase
    {
        public int HotelId;
        public int Interest;
        public int Start;
        public int Count;
        public RatingType Rating;
        public int Feature;
        public int TFTType;
        private const int PageSize = 15;

        public ReviewsParam(Controller controller)
            : base(controller)
        {
            int tempId = GetInt("hotel", 0);
            HotelId = tempId == 0 ? GetIntFromQuery("hotel", 0) : tempId;
            Interest = GetIntFromQuery("interest", 0);
            Start = GetIntFromQuery("start", 0);
            Count = PageSize;
            Rating = (RatingType)GetIntFromQuery("rating", (int)RatingType.All);
            if (Rating == 0)
            {
                Feature = GetIntFromQuery("feature", 0);
                TFTType = GetIntFromQuery("tftType", 0);
            }
        }

        public override RouteValueDictionary Change(string field, object value)
        {
            RouteValueDictionary dict = new RouteValueDictionary {
                {"hotel", HotelId},
                {"interest", Interest},
                {"rating", (int)Rating},
                {"feature", Feature},
                {"start", Start}
            };
            dict[field] = value;
            if (field == "feature")
            {
                dict.Remove("rating");
                if ((int)Rating > 0)
                {
                    dict.Remove("start");
                }
            }
            else if (field == "rating")
            {
                dict.Remove("interest");
                dict.Remove("feature");
                if (Feature > 0)
                {
                    dict.Remove("start");
                }
            }
            foreach (string key in new string[] { "start", "rating", "feature" })
            {
                if (dict.ContainsKey(key) && (int)dict[key] == 0)
                {
                    dict.Remove(key);
                }
            }
            return dict;
        }
    }
}