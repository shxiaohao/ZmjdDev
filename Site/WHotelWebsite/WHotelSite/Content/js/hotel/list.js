var city = $P["city"];
var districtName = $P["districtName"];
var lat = $P["userlat"];
var lng = $P["userlng"];
var geoScopeType = $P["sctype"];
var interest = $P["interest"];
var checkin = $P["checkin"];
var checkout = $P["checkout"];

var isApp = $("#isApp").val() == "1";
var inWeixin = $("#inWeixin").val() == "1";
var isMobile = $("#isMobile").val() == "1";
var routeCityId = $("#cityId").val();
var routeCityName = $("#cityName").val();
var routeInterest = $("#interest").val();

var _Config = new Config();

//主题
var themesData = null;
var loadThemesFunction = function () { };
var _themeListScroll = {};

//酒店列表
var hotelListData = null;
var loadHotelListFunction = function () { };
var iniLoadHotelList = function () { };

var hotelsStart = 0;
var hotelsCount = 10;
var hotelsLoadLock = false;



$(function () {

    if (!isMobile && !isApp) {

        var $page = $('.pager1');
        srollpage($page, function (content) {
            $page.before(content);
        });

        $('.text-center').on('click', '.more', function () {
            var $el = $(this);
            $el.text('收起').removeClass('more').addClass('less').blur();
            var $dl = $el.parent().prev();
            $dl.css('height', 'auto');
        });

        $('.text-center').on('click', '.less', function () {
            var $el = $(this);
            $el.text('更多主题').removeClass('less').addClass('more').blur();
            var $dl = $el.parent().prev();
            $dl.css('height', '210px');
        });
    }
    else {

        /********** 移动或者App环境 **********/
        
        if (city == "0" || city == "" || city == undefined) {
            if (routeCityId) {
                city = routeCityId;
            }
        }
        
        //坐标默认值根据是否有默认城市ID确定
        if (city == "0" || city == "" || city == undefined) {
            city = "2";
            if (lat == "" || lat == undefined) lat = 31.236237;
            if (lng == "" || lng == undefined) lng = 121.389139;
            if (geoScopeType == "" || geoScopeType == undefined) geoScopeType = "3";
        }
        else {
            if (lat == "" || lat == undefined) lat = 0;
            if (lng == "" || lng == undefined) lng = 0;
            if (geoScopeType == "" || geoScopeType == undefined) geoScopeType = "1";
        }
        
        if (districtName == "" || districtName == undefined) districtName = ""; //districtName = "上海";
        if (interest == "" || interest == undefined) {
            if (routeInterest) {
                interest = routeInterest;
            }
            else {
                interest = 0;
            }
        }
        if (checkin == "" || checkin == undefined) checkin = "";
        if (checkout == "" || checkout == undefined) checkout = "";

        //加载主题
        loadThemesFunction = function () {

            var _themeDic = { "userLat": lat, "userLng": lng, "geoScopeType": geoScopeType, "districtid": city, "districtName": districtName, "needICONCount": 50, "loadHotelList": 0, "v": 5 };
            $$.Get(_Config.APIUrl + "/api/hotel/GetThemeInterestList", _themeDic, function (_data) {

                //console.log(_data)

                if (_data.ICONList && _data.ICONList.length) {

                    $("#theme-scroll-list-seat").hide();
                    $("#theme-scroll-list").fadeIn(200);

                    _data["selInterest"] = parseInt(interest);

                    if (themesData) {
                        themesData.themeData = _data;
                    }
                    else {
                        themesData = new Vue({
                            el: '#theme-obj',
                            data: { "themeData": _data }
                        })
                    }

                    Vue.nextTick(function () {

                        //热门主题支持横向滑动
                        try {
                            _themeListScroll = new IScroll('#theme-scroll-list', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
                        } catch (e) {

                        }

                        var _selItem = $("#theme-scroll-list ._scroller").find(".sel");
                        var _selNum = parseInt(_selItem.data("num"));
                        if (_selNum > 1) {
                            //如果选择的项靠后，则自动滚动到指定区域
                            _themeListScroll.scrollToElement('loop:nth-child(' + (_selNum + 1) + ')', 500);
                        }

                        //主题的点击事件
                        $("#theme-obj .t-item").each(function () {

                            var _item = $(this);
                            _item.click(function () {

                                if (!hotelsLoadLock || true) {

                                    var _inid = _item.data("inid");
                                    var _op = _item.data("op");

                                    //change style
                                    $("#theme-obj .t-item").each(function () {
                                        $(this).removeClass("sel");
                                        $(this).data("op", "0");
                                    });

                                    //选中
                                    if (_op == "0") {

                                        _item.addClass("sel");
                                        _item.data("op", "1");

                                        _selItem = $("#theme-scroll-list ._scroller").find(".sel");
                                        _selNum = parseInt(_selItem.data("num"));
                                        if (_selNum > 1) {
                                            //如果选择的项靠后，则自动滚动到指定区域
                                            _themeListScroll.scrollToElement('loop:nth-child(' + (_selNum + 1) + ')', 500);
                                        }

                                        //ref list
                                        interest = parseInt(_inid);
                                        iniLoadHotelList();
                                    }
                                    else {

                                        //取消选中
                                        _item.data("op", "0");

                                        //ref list
                                        interest = 0;
                                        iniLoadHotelList();
                                    }
                                }

                            });
                        });
                    });
                }
                else {
                    
                    if (themesData) {
                        themesData.themeData = _data;
                    }
                    else {
                        themesData = new Vue({
                            el: '#theme-obj',
                            data: { "themeData": _data }
                        })
                    }

                    $("#theme-obj").hide();
                }
            });
        }
        loadThemesFunction();

        //加载当前地址的精选酒店
        loadHotelListFunction = function (isfirst) {

            if (!hotelsLoadLock) {
                hotelsLoadLock = true;

                if (hotelListData && hotelListData.hotelsData && hotelListData.hotelsData.Result20 && !hotelListData.hotelsData.Result20.length) {
                    return;
                }

                //默认先隐藏空提示
                $(".hotels-null").hide();

                //默认显示scroll loading
                $(".scrollpageloading").show();

                if (isfirst) {
                    $(".scrollpageloading").html('<img class="img-first" src="http://whfront.b0.upaiyun.com/app/img/loading-changes.gif" alt="" />');
                }
                else {
                    $(".scrollpageloading").find("img").removeClass("img-first");
                    hotelsStart += hotelsCount;
                }

                var _hotelListDic = {
                    districtid: city,
                    districtName: districtName.replace("及周边", ""),
                    start: hotelsStart,
                    count: hotelsCount,
                    lat: lat,
                    lng: lng,
                    geoScopeType: geoScopeType,
                    interest: interest,
                    JustMinPricePlan: true,
                    checkin: checkin,
                    checkout: checkout,
                };
                console.log(_hotelListDic);
                $.post(_Config.APIUrl + "/api/hotel/SearchHotelList30", _hotelListDic, function (_data) {

                    console.log(_data);
                    if (_data && _data.Result20 && _data.Result20.length) {

                        if (_data.Result20.length >= hotelsCount) {
                            if (isfirst) {
                                $(".scrollpageloading").show();
                            }
                            else {
                                $(".scrollpageloading").hide();
                            }

                            //如果可能有下一页的话，则解锁加载
                            hotelsLoadLock = false;
                        }
                        else {
                            //如果是第一页，则不显示“没有更多了”，不然很奇怪
                            if (!isfirst) {
                                $(".scrollpageloading").hide();
                                //$(".scrollpageloading").html("<div>没有更多了</div>");
                            }
                            else {
                                $(".scrollpageloading").hide();
                            }
                        }

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

                        if (hotelListData) {
                            if (isfirst) {
                                hotelListData.hotelsData = _data;
                            }
                            else {
                                var _oldListData = hotelListData.hotelsData;
                                for (var _num = 0; _num < _data.Result20.length; _num++) {
                                    var _newData = _data.Result20[_num];
                                    _oldListData.Result20.push(_newData);
                                }

                                hotelListData.hotelsData = _oldListData;
                            }
                        }
                        else {
                            hotelListData = new Vue({
                                el: '#hotels-obj',
                                data: { "hotelsData": _data }
                            })
                        }

                        Vue.nextTick(function () {

                            //图片懒加载
                            $("#hotels-obj .lazyload-img").lazyload({
                                threshold: 50,
                                placeholder: "/content/images/seat/home-hotel-load-16x9.png",
                                effect: "show"
                            });
                        });
                    }
                    else {

                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        if (isfirst) {

                            if (hotelListData) {
                                hotelListData.hotelsData = _data;
                            }
                            else {
                                hotelListData = new Vue({
                                    el: '#hotels-obj',
                                    data: { "hotelsData": _data }
                                })
                            }

                            //隐藏scroll loading
                            $(".scrollpageloading").hide();

                            //显示空提示
                            $(".hotels-null").fadeIn(100);
                        }
                        else {

                            $(".scrollpageloading").hide();
                            //$(".scrollpageloading").html("<div>没有更多了</div>");

                            if (!hotelListData.hotelsData.Result20.length) {

                                //显示空提示
                                $(".hotels-null").fadeIn(100);
                            }
                        }
                    }
                });
            }
        }

        //初始加载hotel list
        iniLoadHotelList = function () {
            hotelsLoadLock = false;
            hotelsStart = 0;
            if (hotelListData) hotelListData.hotelsData.Result20 = null;
            loadHotelListFunction(true);
        }
        iniLoadHotelList();

        //如果默认没有拿到地址，则通过坐标获取
        if (!routeCityName) {

            if (lat && lng) {

                var _cityDic = { "lat": lat, "lon": lng };
                $.get(_Config.APIUrl + "/api/dest/GetAroundCityInfo", _cityDic, function (_data) {

                    console.log(_data)

                    if (_data && _data.length && _data[0].Name) {
                        routeCityName = _data[0].Name;
                        districtName = _data[0].Name;
                        if (geoScopeType == "3") {
                            districtName = _data[0].Name + "及周边";
                        }
                        document.title = districtName;
                    }
                    else {
                        document.title = "请尝试输入“上海”";
                    }

                });
            }
            else {
                document.title = "请尝试输入“上海”";
            }
            
        }

        var $win = $(window);
        var isload = true;

        $win.on('scroll', function () {
            var hotelsFootTop = $(".hotels-section-foot").offset().top;
            var winTop = $win.scrollTop();
            var winHeight = $win.height();

            if (winTop >= hotelsFootTop - winHeight - 100) {
                loadHotelListFunction(false);
            }
        });

        ////app分享配置
        var param = shareConfig(lat, lng, city, geoScopeType, interest, districtName)
        //zmjd.setShareConfig(param);

        if (inWeixin) {

            //微信分享
            loadWechat(param.title, param.content, param.shareLink, param.photoUrl, function () { });
        }
    }
});

$(function () {

    if (isMobile || isApp) {

        var selectDateDom = $('#date-picker');
        var showDateDom = selectDateDom;

        //init calendar
        var calendar = null;

        // main calendar
        var onSelect = function (newCheckIn, newCheckOut) {

            console.log("onselect");

            $("#checkIn").val(Calendar.format(newCheckIn));
            $("#checkOut").val(Calendar.format(newCheckOut));

            var cin = formatDate(newCheckIn, "yyyy-MM-dd");
            var cout = formatDate(newCheckOut, "yyyy-MM-dd");
        };

        var onselectRange = function (newCheckIn, newCheckOut) {

            console.log("onselectRange");

            $("#checkIn").val(Calendar.format(newCheckIn));
            $("#checkOut").val(Calendar.format(newCheckOut));

            var cin = formatDate(newCheckIn, "yyyy-MM-dd");
            var cout = formatDate(newCheckOut, "yyyy-MM-dd");

            $("#checkIn").val(cin);
            $("#checkOut").val(cout);
        };

        //show calendar
        calendar || (calendar = new Calendar(onSelect, window.calendarOptions, onselectRange, function () { }, $(".date-ctrl")));

        //select calendar
        var showCalendarFunc = function () {

            console.log(calendar);

            calendar.show();

            //def calendar selectRange
            var cin = $("#checkIn").val();
            var cout = $("#checkOut").val();
            calendar.selectRange(new Date(cin), new Date(cout));
        }
        selectDateDom.click(showCalendarFunc);

        showSpinner.prefetch();

        function formatDate(date, format) {
            if (!date) return;
            if (!format) format = "yyyy-MM-dd";
            switch (typeof date) {
                case "string":
                    date = new Date(date.replace(/-/, "/"));
                    break;
                case "number":
                    date = new Date(date);
                    break;
            }
            if (!date instanceof Date) return;
            var dict = {
                "yyyy": date.getFullYear(),
                "M": date.getMonth() + 1,
                "d": date.getDate(),
                "H": date.getHours(),
                "m": date.getMinutes(),
                "s": date.getSeconds(),
                "MM": ("" + (date.getMonth() + 101)).substr(1),
                "dd": ("" + (date.getDate() + 100)).substr(1),
                "HH": ("" + (date.getHours() + 100)).substr(1),
                "mm": ("" + (date.getMinutes() + 100)).substr(1),
                "ss": ("" + (date.getSeconds() + 100)).substr(1)
            };
            return format.replace(/(yyyy|MM?|dd?|HH?|ss?|mm?)/g, function () {
                return dict[arguments[0]];
            });
        }

        function formatToDays(sm) {
            return sm / 1000 / 60 / 60 / 24;
        }

        //确定选择日期
        $(".date-ok-btn").click(function () {

            var selectedCells = calendar.selectedCells();
            if (selectedCells.length < 2) {
                alert("请选择离店日期");
                return;
            }

            calendar.hide();

            //ref list
            checkin = $("#checkIn").val();
            checkout = $("#checkOut").val();
            iniLoadHotelList();

            //change filter
            $("#date-picker").html("{0}至{1}".format(checkin, checkout));
            $("#date-picker").addClass("sel");
        });

        //重置日期
        $(".date-cancel-btn").click(function () {

            calendar.hide();

            //ref list
            checkin = "";
            checkout = "";
            iniLoadHotelList();

            //change filter
            $("#date-picker").html("日期");
            $("#date-picker").removeClass("sel");
        });

        return;
    }

    //// 初始化时间
    //var now = new Date();
    //var nowYear = now.getFullYear();
    //var nowMonth = now.getMonth() + 1;
    //var nowDate = now.getDate();
    //showDateDom.attr('data-year', nowYear);
    //showDateDom.attr('data-month', nowMonth);
    ////showDateDom.attr('data-date', nowDate);

    //// 数据初始化
    //function formatYear(nowYear) {
    //    var arr = [];
    //    for (var i = nowYear - 5; i <= nowYear + 5; i++) {
    //        arr.push({
    //            id: i + '',
    //            value: i + '年'
    //        });
    //    }
    //    return arr;
    //}
    //function formatMonth() {
    //    var arr = [];
    //    for (var i = 1; i <= 12; i++) {
    //        arr.push({
    //            id: i + '',
    //            value: i + '月'
    //        });
    //    }
    //    return arr;
    //}

    //var yearData = function (callback) {
    //    console.log("year");
    //    callback(formatYear(nowYear))
    //}
    //var monthData = function (year, callback) {
    //    console.log("month");
    //    callback(formatMonth());
    //};

    //selectDateDom.bind('click', function () {
    //    var oneLevelId = showDateDom.attr('data-year');
    //    var twoLevelId = showDateDom.attr('data-month');
    //    var iosSelect = new IosSelect(2,
    //        [yearData, monthData],
    //        {
    //            title: '日期',
    //            itemHeight: 45,
    //            oneLevelId: oneLevelId,
    //            twoLevelId: twoLevelId,
    //            showLoading: true,
    //            callback: function (selectOneObj, selectTwoObj) {
    //                console.log("change");
    //                showDateDom.attr('data-year', selectOneObj.id);
    //                showDateDom.attr('data-month', selectTwoObj.id);
    //                showDateDom.html(selectOneObj.value + '' + selectTwoObj.value);
    //            }
    //        });
    //});

});

//打开酒店
var openHotel = function (_hotelId) {

    var _gourl = 'http://www.zmjiudian.com/hotel/' + _hotelId;
    if (isApp) {
        _gourl = 'whotelapp://www.zmjiudian.com/hotel/' + _hotelId;
    }

    if (checkin && checkin) {
        _gourl = _gourl + "?checkin={0}&checkout={1}".format(checkin, checkout);
    }

    gourl(_gourl);
}

//目的地的点击处理事件
var districtClick = function (_districtId, _districtName, _lat, _lng, _geoScopeType) {

    city = _districtId;
    districtName = _districtName;
    lat = _lat;
    lng = _lng;
    geoScopeType = _geoScopeType;
    interest = 0;

    //ref interest
    loadThemesFunction();

    //ref list
    iniLoadHotelList();

    //set title
    document.title = _districtName;


    //app分享配置
    var param = shareConfig(lat, lng, city, geoScopeType, interest, districtName)

    zmjd.setShareConfig(param);
    
    if (inWeixin) {
        //微信分享
        loadWechat(param.title, param.content, param.shareLink, param.photoUrl, function () { });
    }
};

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    if (lat == "" || lat == undefined) lat = 31.236237;
    if (lng == "" || lng == undefined) lng = 121.389139;
    //app分享配置
    var param = shareConfig(lat, lng, city, geoScopeType, interest, districtName);
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
var shareConfig = function (_lat, _lng, _districtId, _sctype, _interest, _districtName) {
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
    shareLink = "http://www.zmjiudian.com/hotel/list?userlat=" + _lat + "&userlng=" + _lng + "&city=" + _districtId + "&sctype=" + _sctype + "&interest=" + interest + "&districtName=" + _districtName + "&_newpage=0"
    var param = { title: title, content: content, photoUrl: photoUrl, shareLink: shareLink };
    return param;
}

