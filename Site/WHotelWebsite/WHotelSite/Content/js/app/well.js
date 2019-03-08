
var userid = $P["userid"];
var districtId = $P["districtId"];
var districtName = $P["districtName"];
var lat = $P["lat"];
var lng = $P["lng"];
var geoScopeType = $P["geoScopeType"];

var _HomeSliderItemWidth = 363;
var _HomeSliderItemMargin = 10;
var _HomeSliderCusAddValue = 50;
var _HomeSliderLiMarLeft = 25;

var _Config = new Config();

/*
 测试参数：
 ?userid=0&districtId=2&districtName=上海&lat=0&lng=0&geoScopeType=1
 * */

$(function () {

    var _this = this;

    if (userid == undefined) userid = 0;
    if (lat == undefined) lat = 0;
    if (lng == undefined) lng = 0;
    if (geoScopeType == "" || geoScopeType == undefined) geoScopeType = "3";
    if (districtId == "" || districtId == undefined) districtId = "2";
    if (districtName == "" || districtName == undefined) districtName = "上海";

    var inWeixin = $("#inWeixin").val() == "1";

    var wwidth = $(window).width();
    if (wwidth < 410) { _HomeSliderItemWidth = 328; _HomeSliderLiMarLeft = 23; }	//ip6
    if (wwidth == 360) { _HomeSliderItemWidth = 317; _HomeSliderLiMarLeft = 22; }
    if (wwidth < 350) { _HomeSliderItemWidth = 280; _HomeSliderLiMarLeft = 20; }	//ip4/ip5

    $('body').append("<style>.well-hlist-panel .item{ width: " + _HomeSliderItemWidth + "px !important; }</style>");

    //记录每个主题下的第一个酒店，用于去重所有主题列表的第一个酒店
    var repeatHotelList = [];
    var haveHotelInRepeatList = function (hid) {
        for (var i = 0; i < repeatHotelList.length; i++) {
            if (repeatHotelList[i] == hid) {
                return true;
            }
        }
        return false;
    }

    var start = 0;
    var count = 3;

    var genThemeSection = function () {

        var index = start;
        var _list = iconList;

        if (_list && _list.length > index && count > index) {

            var item = _list[index];
            var id = item.ID;
            var name = item.Name;
            var actionUrl = item.ActionUrl;

            //create template html
            var _themeHtml = '<div id="themeSection{0}" class="section-seat well-hide"><component-well-theme{0} :albums-info=AlbumsInfo></component-well-theme{0}></div>';
            if (index < 2) _themeHtml = _themeHtml + '<div id="themeSection-seat{0}" class="well-seat-slider"><img src="~/Content/images/seat/img-home-seat-slider1031.png" alt=""></div>';

            $("#well-panel").append(_themeHtml.format(id));

            //定义模板
            var ComponentWellTheme = Vue.extend({
                template: $("#template-well-albums").html(),
                props: ["AlbumsInfo"]
            });

            //reg Component
            Vue.component('component-well-theme' + id, ComponentWellTheme);

            /*if(item.HotelListResult){

				var _data = item.HotelListResult;*/

            var hotelListDic = {
                aroundCityId: (geoScopeType == 3 ? districtId : 0),
                districtid: districtId,
                start: 0,
                count: 6,
                lat: lat,
                lng: lng,
                geoScopeType: geoScopeType,
                interest: id,
                JustMinPricePlan: true,
                v: 3
            };
            $$.Post(_Config.APIUrl + "/api/hotel/SearchHotelList30Cache", hotelListDic, function (_data) {

                //对android做一个特殊处理，如果是android，则图片使用WebP格式
                if (_data && _data.Result20 && _data.Result20.length > 0) {

                    //对第一个酒店项做去重处理
                    if (_data.Result20.length > 1) {
                        var firstHotel = _data.Result20[0];
                        if (haveHotelInRepeatList(firstHotel.Id)) {
                            var secondHotel = _data.Result20[1];
                            _data.Result20[0] = secondHotel;
                            _data.Result20[1] = firstHotel;
                        }
                        repeatHotelList.push(_data.Result20[0].Id);
                    }

                    _data["HotelTotalCount"] = _data.Result20.length;

                    _data.Result20.map(function (item, index) {
                        if (item.PictureList && item.PictureList.length > 0) {
                            item["HotelPicUrl"] = item.PictureList[0];
                        }
                        else {
                            item["HotelPicUrl"] = "http://whfront.b0.upaiyun.com/app/img/home/home-load-3x2.png";
                        }

                        //评星
                        item.HotelScoreHtml = "";
                        if (item.HotelScore) {
                            item.HotelScoreHtml = getScoreHtml(item.HotelScore);
                        }
                    });

                    _data["albumTitle"] = name;
                    _data["albumId"] = id;
                    _data["actionUrl"] = "http://www.zmjiudian.com/city" + districtId + "/theme" + id;//actionUrl;
                    _data["userid"] = userid;

                    new Vue({
                        el: '#themeSection' + id,
                        data: { "AlbumsInfo": _data }
                    })

                    $("#themeSection" + id).show();

                    //Banners
                    $('#albnum-banner-' + id + ' .well-hlist-panel').swiper({
                        slidesPerView: 'auto',
                        offsetPxBefore: _HomeSliderLiMarLeft,
                        offsetPxAfter: _HomeSliderLiMarLeft,
                        onTouchEnd: function (slider) {
                            if (slider.activeIndex + 1 < slider.slides.length) {
                                var li = $(slider.slides[slider.activeIndex + 1]);
                                var imgObj = li.find("img");
                                setImgOriSrc(imgObj);
                            }
                        }
                    })
                }

                $("#themeSection" + id + " img").lazyload({
                    threshold: 10,
                    placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load-3x2.png",
                    effect: "show"
                });

                if ($("#themeSection-seat" + id)) $("#themeSection-seat" + id).hide();

                start++;

                genThemeSection();
            });
        }
        else {
            count += 3;
        }

    }

    //生成所有主题列表
    var iconList = {};
    var loadThemeList = function () {

        var themeDic = { "userLat": lat, "userLng": lng, "geoScopeType": geoScopeType, "districtid": districtId, "districtName": districtName, "needICONCount": 50, "loadHotelList": 0, "v": 5 };
        $$.Get(_Config.APIUrl + "/api/hotel/GetThemeInterestList", themeDic, function (_data) {

            if (_data.ICONList && _data.ICONList.length > 0) {

                if (_data.ICONList.length > 2) {
                    $(".scrollpageloading").show();
                }

                $(".well-seat-slider").hide();

                iconList = _data.ICONList;

                genThemeSection();
            }
            else {
                $(".scrollpageloading").hide();
            }
        });

    }

    //生成标题
    var genDistrictTitle = function () {

        //district show
        if (districtName && districtName != "") {

            var _tit = districtName + (geoScopeType == 3 ? "周边" : "") + "好去处";

            $(".well-title .val").html(_tit);

            document.title = _tit;
        }
    }

    var $win = $(window);

    if (inWeixin && false) {

        //微信API注册
        GetWeixinApiConfig();
        function GetWeixinApiConfig() {
            var url = location.href.split("#")[0];
            $.ajax({
                url: '/Coupon/GetWeixinConfigInfo',
                type: 'POST',
                data: { url: url },
                dataType: 'json',
                async: false,
                error: function () {
                    console.log("网络服务错误");
                },
                success: function (result) {
                    if (typeof (result.Success) != undefined && result.Success == 1) {
                        return false;
                    }
                    else {
                        //得到微信config的值
                        var array = [
                            'checkJsApi',
                            'openLocation',
                            'getLocation'
                        ];
                        if (result.jsApiList) {
                            var jsArray = result.jsApiList.split(',');
                            for (var key in jsArray) {
                                array.push("" + jsArray[key]);
                            }
                        }

                        wx.config({
                            debug: false,
                            appId: result.appId,
                            timestamp: "" + result.timestamp,
                            nonceStr: result.nonceStr,
                            signature: result.signature,
                            jsApiList: array
                        });
                    }
                }
            });
        }

        wx.ready(function () {
            wx.getLocation({
                type: 'wgs84', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
                success: function (res) {
                    console.log(1)
                    console.log(res)
                    var latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
                    var longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
                    var speed = res.speed; // 速度，以米/每秒计
                    var accuracy = res.accuracy; // 位置精度

                    if (latitude > 0 || longitude > 0) {
                        lat = latitude;
                        lng = longitude;
                    }

                    //load theme list
                    loadThemeList();

                    //load district name
                    var aroundDic = { "lat": lat, "lon": lng, "v": 1 };
                    $$.Get(_Config.APIUrl + "/api/dest/GetAroundCityInfo", aroundDic, function (_data) {
                        if (_data && _data.length > 0 && _data[0].Name) {
                            districtName = _data[0].Name;
                            genDistrictTitle();
                        }
                    });

                    $win.on('scroll', function () {

                        var tagTop = $(".well-foot").offset().top;
                        var winTop = $win.scrollTop();
                        var winHeight = $win.height();

                        if (winTop >= tagTop - winHeight - 100) {
                            genThemeSection();
                            if (iconList && count > iconList.length) {
                                $(".scrollpageloading").hide();
                            }
                        }
                    });
                },
                error: function (res) {
                    console.log(2)
                    console.log(res)
                }
            });
        });
    }
    else {
        loadThemeList();
        genDistrictTitle();

        $win.on('scroll', function () {

            var tagTop = $(".well-foot").offset().top;
            var winTop = $win.scrollTop();
            var winHeight = $win.height();

            if (winTop >= tagTop - winHeight - 100) {
                genThemeSection();
                if (iconList && count > iconList.length) {
                    $(".scrollpageloading").hide();
                }
            }
        });
    }
});

var gourl = function (_url) { location.href = _url; }

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        var defsrc = imgObj.attr("src");
        imgObj.attr("src", orisrc);
        imgObj.data("orisrc", "");
        imgObj.error(function () {
            imgObj.attr("src", defsrc);
        });
    }
};



//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    

}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}

_getAppData(userid, '', '', '');
