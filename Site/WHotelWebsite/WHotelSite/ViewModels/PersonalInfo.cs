using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJD.AccountServices.Entity;

namespace WHotelSite.ViewModels
{
    public class PersonalInfo
    {
        public long PhoneNumber { get; set; }
        public long UserID { get; set; }
        public string NickName { get; set; }
        public List<UserCommInfoEntity> CommonInfos { get; set; }
    }
}