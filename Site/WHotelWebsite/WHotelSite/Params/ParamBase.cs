using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Routing;

namespace WHotelSite.Params
{

    public class ParamBase
    {
        protected Controller controller;

        public virtual RouteValueDictionary Change(string field, object value)
        {
            return new RouteValueDictionary();
        }

        public int GetInt(String name, int defaultValue)
        {
            int result;
            return Int32.TryParse((String)controller.RouteData.Values[name], out result) ? result : defaultValue;
        }

        public long GetLong(String name, long defaultValue)
        {
            long result;
            return long.TryParse((String)controller.RouteData.Values[name], out result) ? result : defaultValue;
        }

        public long GetLongFromQuery(String name, long defaultValue)
        {
            long result;
            return long.TryParse((String)controller.Request.QueryString[name], out result) ? result : defaultValue;
        }

        public int GetIntFromQuery(String name, int defaultValue)
        {
            int result;
            return Int32.TryParse(controller.Request.QueryString[name], out result) ? result : defaultValue;
        }

        public string GetString(String name, string defaultValue)
        {
            String ret = (String)controller.RouteData.Values[name];
            return ret == null ? defaultValue : ret;
        }
        public string GetStringFromQuery(String name, string defaultValue)
        {
            String ret = (String)controller.Request.QueryString[name];
            return ret == null ? defaultValue : ret;
        }
        public float GetFloatFromQuery(String name, float defaultValue)
        {
            float result;
            return float.TryParse((String)controller.Request.QueryString[name], out result) ? result : defaultValue;
        }

        public string GetStringFromCookie(String name, string defaultValue)
        {
            String ret = controller.Request.Cookies[name] == null ? null : (String)controller.Request.Cookies[name].Value;
            return ret == null ? defaultValue : ret;
        }
        public float GetFloatFromCookie(String name, float defaultValue)
        {
            float result;
            // XXX cookie.Values vs cookie.Value
            return float.TryParse(controller.Request.Cookies[name] == null ? null : (String)controller.Request.Cookies[name].Value, out result) ? result : defaultValue;
        }
        public string GetStringFromHeader(String name, string defaultValue)
        {
            String ret = controller.Request.Headers[name];
            return String.IsNullOrEmpty(ret) ? defaultValue : ret;
        }

        const float DefaultLat = 31.236705f;
        const float DefaultLon = 121.501f;
        public float GetLat()
        {
            return GetFloatFromCookie("lat", DefaultLat);
        }
        public float GetLon()
        {
            return GetFloatFromCookie("lon", DefaultLon);
        }
        public bool IsDefaultLatLon(float lat, float lon)
        {
            return lat == DefaultLat && lon == DefaultLon;
        }


        public ParamBase(Controller aController)
        {
            controller = aController;
        }
    }

}