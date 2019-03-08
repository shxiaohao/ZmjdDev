
var userid = $P["userid"];
var districtId = $P["districtId"];
var districtName = $P["districtName"];
var lat = $P["lat"];
var lng = $P["lng"];
var geoScopeType = $P["geoScopeType"];

var isApp = $("#isApp").val() == "1";
var inWeixin = $("#inWeixin").val() == "1";

var _HomeSliderItemWidth = 363;
var _HomeSliderItemMargin = 10;
var _HomeSliderCusAddValue = 50;
var _HomeSliderLiMarLeft = 25;

var _Config = new Config();
//lat, lng, city, geoScopeType, interest, districtName
/*
 测试参数：
 ?userid=0&districtId=2&districtName=上海&lat=0&lng=0&geoScopeType=1
 * */

$(function () {

    var _this = this;

    if (userid == undefined) userid = 0;

    if (geoScopeType == "" || geoScopeType == undefined) {
        geoScopeType = "3";
        if (lat == undefined) lat = 31.236237;
        if (lng == undefined) lng = 121.389139;
    }
    else {
        if (lat == undefined) lat = 0;
        if (lng == undefined) lng = 0;
    }
    
    if (districtId == "" || districtId == undefined) districtId = "2";
    if (districtName == "" || districtName == undefined) districtName = "上海";

    //如果默认没有拿到地址，则通过坐标获取
    if (!parseInt(districtId)) {

        if (lat && lng) {

            var _cityDic = { "lat": lat, "lon": lng };
            $.get(_Config.APIUrl + "/api/dest/GetAroundCityInfo", _cityDic, function (_data) {

                console.log(_data)

                if (_data && _data.length && _data[0].DistrictId) {
                    districtId = _data[0].DistrictId;
                    districtName = _data[0].Name;

                    //美食
                    productCouponLoadFunc();

                    //玩乐
                    playProductCouponLoadFunc();
                }
                else {
                    districtId = "2";
                }

            });
        }
        else {
            districtId = "2";
        }
    }

    var wwidth = $(window).width();
    if (wwidth >= 430) { _SmallBannerAddWidth = 20; }							//meizu mx3
    if (wwidth < 410) { _HomeSliderItemWidth = 328; _HomeSliderLiMarLeft = 23; }	//ip6
    if (wwidth == 360) { _HomeSliderItemWidth = 317; _HomeSliderLiMarLeft = 22; }
    if (wwidth < 350) { _HomeSliderItemWidth = 280; _HomeSliderLiMarLeft = 20; }	//ip4/ip5

    $('body').append("<style>.hlist-panel{ width: " + wwidth + "px !important; } .hlist-panel .item{ width: " + _HomeSliderItemWidth + "px !important; }</style>");
    $('body').append("<style>.hlist-panel .sml-item{ width: " + ((_HomeSliderItemWidth / 2) - 5) + "px !important; }</style>");

    //美食
    productCouponLoadFunc();

    //玩乐
    playProductCouponLoadFunc();

    //周末好去处
    weekGoodLoadFunc(lat, lng);

    var _pagetit = "周边";
    if (districtName) {
        if (geoScopeType === "3") {
            _pagetit = "{0}及周边".format(districtName.replace("及周边", ""));
        }
        else {
            _pagetit = districtName;
        }
    }
    document.title = _pagetit;


    ////app分享配置
    var param = shareConfig(lat, lng, districtId, geoScopeType, districtName)
    //zmjd.setShareConfig(param);
    
    if (inWeixin) {
        //微信分享
        loadWechat(param.title, param.content, param.shareLink, param.photoUrl, function () { });
    }
});

var hotelNull = false;
var foodNull = false;
var playNull = false;
var showNull = function () {
    if (hotelNull && foodNull && playNull) {
        $(".null-info").fadeIn(200);
    }
}

var hideNull = function () {
    hotelNull = false;
    foodNull = false;
    playNull = false;
    $(".null-info").hide();
}

//周边好去处加载 
var weekGoodData = null;
var weekGoodLoadLock = true;
var weekGoodLoadFunc = function (lat, lng) {

    if (weekGoodLoadLock) {
        weekGoodLoadLock = false;

        setTimeout(function () {

            var _cityName = districtName.replace("及周边", "")

            var _hotelListDic = {
                districtid: parseInt(geoScopeType) === 3 ? "0" : districtId,
                districtName: _cityName,
                start: 0,
                count: 6,
                lat: lat,
                lng: lng,
                geoScopeType: geoScopeType,
                interest: 0,
                JustMinPricePlan: true
            };
            $.post(_Config.APIUrl + "/api/hotel/SearchHotelList30Cache", _hotelListDic, function (_data) {

                console.log(_data);
                if (_data && _data.Result20 && _data.Result20.length) {

                    _data.Result20.map(function (item, index) {
                        if (item.PictureList && item.PictureList.length > 0) {
                            item["HotelPicUrl"] = item.PictureList[0].replace('theme', '640x360');

                            //if (IsAndroid) {
                            //    item["HotelPicUrl"] = getImgWebpUrl(item["HotelPicUrl"]);
                            //}
                        }
                        else {
                            item["HotelPicUrl"] = "/content/images/seat/home-hotel-load-16x9.png";
                        }

                        //评星
                        item.HotelScoreHtml = "";
                        if (item.HotelScore) {
                            item.HotelScoreHtml = getScoreHtml(item.HotelScore);
                        }
                    });

                    var _tit = "{0}精选酒店";
                    if (parseInt(geoScopeType) === 3) {
                        _tit = "{0}及周边精选酒店";
                    }

                    if (_cityName) {
                        _tit = _tit.format(_cityName);
                    }
                    else {
                        _tit = "周边精选酒店";
                    }

                    _data["albumTitle"] = _tit;
                    _data["albumId"] = 0;
                    _data["userid"] = userid;
                    _data["lat"] = lat;
                    _data["lng"] = lng;

                    var _moreLink = "/hotel/list?userid=" + userid + "&city=" + (parseInt(geoScopeType) === 3 ? "0" : districtId) + "&districtName=" + districtName + "&userlat=" + lat + "&userlng=" + lng + "&sctype=" + geoScopeType + "&_newpage=1&_headSearch=1&_searchType=1";
                    _data["moreLink"] = _moreLink;

                    //是否需要横滑逐个加载图片
                    _data["stepLoadImg"] = true;

                    if (weekGoodData) {
                        _data.stepLoadImg = false;
                        weekGoodData.AlbumsInfo = _data;
                    }
                    else {
                        weekGoodData = new Vue({
                            el: '#weekGoodSection',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    $("#weekGoodSection").show();

                    //Banners
                    $('#weekGoodSection .hlist-panel').swiper({
                        slidesPerView: 'auto',
                        offsetPxBefore: _HomeSliderLiMarLeft,
                        offsetPxAfter: _HomeSliderLiMarLeft,
                        onTouchEnd: function (slider) {
                            if (slider.activeIndex + 1 < slider.slides.length) {
                                var li = $(slider.slides[slider.activeIndex + 1]);
                                var imgObj = li.find(".show-img");
                                setImgOriSrc(imgObj);
                            }
                        }
                    })

                }
                else {

                    $("#weekGoodSection").hide();

                    hotelNull = true;
                    showNull();
                }

                $("#weekGoodSection img").lazyload({
                    threshold: 10,
                    placeholder: "./content/images/seat/home-hotel-load-3x2.png",
                    effect: "show"
                });

                $("#weekGoodSection-seat").hide();

                weekGoodLoadLock = true;
            });
        },
		0);
    }

}

//美食
var vModel_productCouponSection = null;
var vModel_productCouponSectionLock = true;
var productCouponLoadFunc = function () {

    if (vModel_productCouponSectionLock) {
        vModel_productCouponSectionLock = false;

        setTimeout(function () {

            //if (userid == 4514792) {
            //    alert("food")
            //    alert(districtId)
            //    alert(districtName)
            //    alert(lat)
            //    alert(lng)
            //    alert(geoScopeType)
            //}

            var pcDic = { category: 14, districtId: districtId, lat: 0, lng: 0, geoScopeType: 1, count: 6, start: 0, userid: userid, sort: 0 };
            $.get(_Config.APIUrl + "/api/coupon/GetProductSKUCouponList", pcDic, function (_data) {
                //console.log("加载 美食")
                //console.log(_data)
                if (_data && _data.SKUCouponList && _data.SKUCouponList.length > 0) {
                    _data.SKUCouponList.map(function (item, index) {
                        if (!item.PicList || item.PicList.length <= 0) {
                            item.PicUrl = "/content/images/seat/home-hotel-load-1x1.png";
                        }
                        else {
                            item.PicUrl = item.PicList[0];

                            //对android做一个特殊处理，如果是android，则图片使用WebP格式
                            if (IsAndroid) {
                                item.PicUrl = getImgWebpUrl(item.PicList[0]);
                            }
                        }
                    });

                    var _tit = "{0}美食推荐";
                    //if (parseInt(geoScopeType) === 3) {
                    //    _tit = "{0}及周边美食推荐";
                    //}

                    if (districtName) {
                        _tit = _tit.format(districtName.replace("及周边", ""));
                    }
                    else {
                        _tit = "周边美食推荐";
                    }

                    _data["albumTitle"] = _tit;
                    _data["albumId"] = 0;
                    _data["userid"] = userid;
                    _data["category"] = 14;
                    _data["districtId"] = districtId;
                    _data["districtName"] = districtName.replace("及周边", "");
                    _data["lat"] = lat;
                    _data["lng"] = lng;
                    _data["geoScopeType"] = 1;//geoScopeType;

                    if (vModel_productCouponSection) {
                        vModel_productCouponSection.AlbumsInfo = _data;
                    }
                    else {
                        vModel_productCouponSection = new Vue({
                            el: '#productCouponSection',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    $("#productCouponSection").show();
                }
                else {

                    $("#productCouponSection").hide();

                    foodNull = true;
                    showNull();
                }

                $("#productCouponSection-seat").hide();

                vModel_productCouponSectionLock = true;
            });
        },
		0);

    }

}

//玩乐
var vModel_playProductCouponSection = null;
var vModel_playProductCouponSectionLock = true;
var playProductCouponLoadFunc = function () {

    if (vModel_playProductCouponSectionLock) {
        vModel_playProductCouponSectionLock = false;

        setTimeout(function () {
            var pcDic = { category: 20, districtId: districtId, lat: 0, lng: 0, geoScopeType: 1, count: 6, start: 0, userid: userid, sort: 0 };
            $$.Get(_Config.APIUrl + "/api/coupon/GetProductSKUCouponList", pcDic, function (_data) {

                //console.log(_data)
                if (_data && _data.SKUCouponList && _data.SKUCouponList.length > 0) {
                    _data.SKUCouponList.map(function (item, index) {
                        if (!item.PicList || item.PicList.length <= 0) {
                            item.PicUrl = "/content/images/seat/home-hotel-load-1x1.png";
                        }
                        else {
                            item.PicUrl = item.PicList[0];

                            ////对android做一个特殊处理，如果是android，则图片使用WebP格式
                            //if(IsAndroid) {
                            //	item.PicUrl = getImgWebpUrl(item.PicList[0]);
                            //}
                        }
                    });

                    var _tit = "{0}玩乐推荐";
                    //if (parseInt(geoScopeType) === 3) {
                    //    _tit = "{0}及周边玩乐推荐";
                    //}

                    if (districtName) {
                        _tit = _tit.format(districtName.replace("及周边", ""));
                    }
                    else {
                        _tit = "周边玩乐推荐";
                    }

                    _data["albumTitle"] = _tit;
                    _data["albumId"] = 0;
                    _data["userid"] = userid;
                    _data["category"] = 20;
                    _data["districtId"] = districtId;
                    _data["districtName"] = districtName.replace("及周边", "");
                    _data["lat"] = lat;
                    _data["lng"] = lng;
                    _data["geoScopeType"] = 1;//geoScopeType;

                    if (vModel_playProductCouponSection) {
                        vModel_playProductCouponSection.AlbumsInfo = _data;
                    }
                    else {
                        vModel_playProductCouponSection = new Vue({
                            el: '#playProductCouponSection',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    $("#playProductCouponSection").show();
                }
                else {

                    $("#playProductCouponSection").hide();

                    playNull = true;
                    showNull();
                }
                $("#playProductCouponSection-seat").hide();

                vModel_playProductCouponSectionLock = true;
            });
        },
		0);

    }

}

//打开酒店
var openHotel = function (_hotelId) {
    
    if (isApp) {
        gourl('whotelapp://www.zmjiudian.com/hotel/' + _hotelId);
    }
    else {
        gourl('http://www.zmjiudian.com/hotel/' + _hotelId);
    }
}

//目的地的点击处理事件
var districtClick = function (_districtId, _districtName, _lat, _lng, _geoScopeType) {

    hideNull();

    districtId = _districtId;
    districtName = _districtName;
    lat = _lat;
    lng = _lng;
    geoScopeType = _geoScopeType;

    //美食
    productCouponLoadFunc();

    //玩乐
    playProductCouponLoadFunc();

    //周末好去处
    weekGoodLoadFunc(lat, lng);

    //var _pagetit = "周边";
    //if (districtName) {
    //    if (geoScopeType === "3") {
    //        _pagetit = "{0}及周边".format(districtName.replace("及周边", ""));
    //    }
    //    else {
    //        _pagetit = districtName;
    //    }
    //}
    //document.title = _pagetit;

    //set title
    document.title = _districtName;


    //app分享配置
    var param = shareConfig(lat, lng, districtId, geoScopeType, districtName)
    zmjd.setShareConfig(param);
    
    if (inWeixin) {
        //微信分享
        loadWechat(param.title, param.content, param.shareLink, param.photoUrl, function () { });
    }
};

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {
    
    //app分享配置
    var param = shareConfig(lat, lng, districtId, geoScopeType, districtName);
    zmjd.setShareConfig(param);
}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //init data
    _InitApp(userid, apptype, appvercode, appverno);
    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}

var setShareConfigSuccess = function () {
    //alert("分享成功");
}

var setShareConfigFail = function () {
    //alert("分享失败");
}


if ($P["w"] == "1") {
    _getAppData(userid, '', '', '');
}

//分享配置
var shareConfig = function (_lat, _lng, _districtId, _sctype, _districtName) {
    var title = _districtName;
    var content = "周末酒店-旅行休闲，又好又划算";
    var photoUrl = "http://whfront.b0.upaiyun.com/app/img/home/zmjd-logo-256x256.png";
    var shareLink = "";
    //if (_sctype == 3) {
    //    title = _districtName + "及周边";
    //}
    //else {
    //    title = _districtName;
    //}
    shareLink = "http://www.zmjiudian.com/App/Around?lat=" + _lat + "&lng=" + _lng + "&districtId=" + _districtId + "&geoScopeType=" + _sctype + "&districtName=" + _districtName + "&_newpage=0"
    var param = { title: title, content: content, photoUrl: photoUrl, shareLink: shareLink };
    return param;
}