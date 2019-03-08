using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.ViewModels
{
    public class CommentViewModel
    {
        public PackageOrderInfo20 OrderInfo20
        {
            get;
            set;
        }
        public CommentDefaultInfoModel CommentInfoModel
        {
            get;
            set;
        }
    }
}