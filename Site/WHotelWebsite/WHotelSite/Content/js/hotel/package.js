var pubuserid = $("#userid").val();
var isVip = $("#isVip").val() == "1";
var _name = $("#_name").val();

var isapp = $("#isapp").val() == "1";
var isThanVer46 = $("#isThanVer46").val() == "1";
var pubhotelid = $("#hotelid").val();
var pubhotelname = $("#hotelname").val();
var pubpid = $("#pid").val();
var pubalbumid = $("#albumid").val();
var showcld = $("#showcld").val() == "1";
var hotelSourceUrl = $("#hotelSourceUrl").val();
var realTimePrice = parseInt($("#totalPrice").val());
var realTimeVipPrice = parseInt($("#totalVipPrice").val());
var isSingleSelectDate = $("#isSingleSelectDate").val() == "1";
var dateSelectType = parseInt($("#dateSelectType").val());
var activeJson = $("#activeJson").val();

var _Config = new Config();

//_Config.APIUrl = "http://api.zmjd100.com";

$(document).ready(function () {

    ////初始mobile login
    //var loginCheckFun = function () {
    //    reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    //}

    //var loginCancelFun = function () {
    //    return true;
    //}

    //_loginModular.init(loginCheckFun, loginCancelFun);

    ////检测登录并自动登录
    //if (!isapp && pubuserid == "0") {
    //    _loginModular.verify.autoLogin(loginCheckFun);
    //}

    //init calendar
    var calendar = null;

    // main calendar
    var onSelect = function (newCheckIn, newCheckOut) {

        $("#checkIn").val(Calendar.format(newCheckIn));
        $("#checkOut").val(Calendar.format(newCheckOut));

        var cin = formatDate(newCheckIn, "yyyy-MM-dd");
        var cout = formatDate(newCheckOut, "yyyy-MM-dd");

    };

    var onselectRange = function (newCheckIn, newCheckOut) {

        updatePriceInfo(newCheckIn, newCheckOut);
    };

    var onselectSingle = function (newCheckIn, newCheckOut) {

        updatePriceInfo(newCheckIn, newCheckOut);
    };

    var updatePriceInfo = function (newCheckIn, newCheckOut) {

        $("#checkIn").val(Calendar.format(newCheckIn));
        $("#checkOut").val(Calendar.format(newCheckOut));

        var cin = formatDate(newCheckIn, "yyyy-MM-dd");
        var cout = formatDate(newCheckOut, "yyyy-MM-dd");

        $("#checkIn").val(cin);
        $("#checkOut").val(cout);

        var dic = {};
        dic["hotelid"] = pubhotelid;
        dic["pid"] = pubpid;
        dic["checkIn"] = cin;
        dic["checkOut"] = cout;
        dic["userid"] = pubuserid;
        dic["dateSelectType"] = dateSelectType;

        $.get('/Hotel/GetPackageCalendarPrice', dic, function (content) {

            //console.log(content);

            var price = content.price;
            var normalPrice = content.normalPrice;
            var vipPrice = content.vipPrice;
            var tip = content.tip;
            var days = content.days;

            realTimePrice = price;
            realTimeVipPrice = vipPrice;

            $(".price-line .pval").html(vipPrice);
            $(".price-line .pval2").html(normalPrice);
            $(".tip-line").html(tip);

            $("#nightCount").val(days);
        });
    }

    //show calendar
    calendar || (calendar = new Calendar(onSelect, window.calendarOptions, onselectRange, onselectSingle, $(".sub-go"), true));

    //日期单选
    calendar.isSingleSelect = isSingleSelectDate;

    //select calendar
    var showCalendarFunc = function () {

        calendar.show();

        //根据不同的
        if (isSingleSelectDate) {
            $(".calendar .ctrls .tip").html("*灰色日期为不可订");
        }

        //def calendar selectRange
        var cin = $("#checkIn").val();
        var cout = $("#checkOut").val();
        if (isSingleSelectDate) {
            calendar.select(new Date(cin));
        }
        else {
            calendar.selectRange(new Date(cin), new Date(cout));
        }
    }
    $(".submit").click(showCalendarFunc);

    //默认显示日历
    if (showcld) {
        showCalendarFunc();
    }

    showSpinner.prefetch();

    function zhengchu(x, y) {
        var z = parseInt(x / y);
        if (z * y == x) {
            return true;
        }
        return false;
    }

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

    var subGoBuy = function () {

        if (pub_userid == "0") {
            if (!isapp) {
                _loginModular.show();
                return;
            }
        }

        var dayLimitMin = parseInt($("#dayLimitMin").val());
        var nightCount = parseInt($("#nightCount").val());
        if (dayLimitMin > 0 && nightCount < dayLimitMin) {
            alert("只能选择" + dayLimitMin + "晚连住");
            return;
        }

        var checkIn = $("#checkIn").val();
        var checkOut = $("#checkOut").val();

        var url = "/hotel/" + pubhotelid + "/book?package=" + pubpid + "&checkIn=" + checkIn + "&checkOut=" + checkOut;
        if (isapp) {

            calendar.hide();

            if (isThanVer46) {
                url = "whotelapp://www.zmjiudian.com/hotelBook?hotelID=" + pubhotelid + "&hotelName=" + pubhotelname + "&checkIn=" + checkIn + "&checkOut=" + checkOut + "&packageID=" + pubpid;
                //alert("这是4.6版本哦~" + url);
            }
            else {
                var addUserParam = "{userid}";
                if (pubuserid.length > 1 && pubuserid != "0") {
                    addUserParam = pubuserid;
                }

                //whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com
                url = "/hotel/" + pubhotelid + "/book?package=" + pubpid + "&checkIn=" + checkIn + "&checkOut=" + checkOut + "&userid=" + addUserParam + "&realuserid=1";
            }
        }

        location.href = url;
    }

    $("#sub-go").click(function () {

        var selectedCells = calendar.selectedCells();
        if (isSingleSelectDate) {
            if (selectedCells.length < 1) {
                alert("请选择一个日期");
                return;
            }
        }
        else {
            if (selectedCells.length < 2) {
                alert("请选择离店日期");
                return;
            }
        }

        //vip discount 检查
        if (!isVip && pubuserid != "0") {

            var pcDic = { "orderTotalPrice": realTimePrice, "orderVipTotalPrice": realTimeVipPrice };
            $.get(_Config.APIUrl + "/api/coupon/BecomeVIPDiscountDescription", pcDic, function (_data) {

                console.log(_data);
                if (_data && _data.ActionUrl && _data.Description) {

                    //alert(_data.Description);

                    _Modal.show({
                        title: '还不是VIP会员？',
                        content: _data.Description,
                        confirmText: '成为VIP会员',
                        confirm: function () {
                            _Modal.hide();
                            goBuyVip();
                        },
                        showCancel: true,
                        showClose: true,
                        cancelText: '继续购买',
                        cancel: function () {
                            _Modal.hide();
                            subGoBuy();
                        },
                        close: function () {
                            _Modal.hide();
                        }
                    });

                    $("._modal-section").css("top", "25%");
                }
                else {

                    subGoBuy();
                }

            });
        }
        else {

            subGoBuy();
        }
    });

    var showRoomInfo = function (showType) {
        $("#room-tit-def").data("op", "1");
        $("#room-tit-sel").addClass("hide");
        $("#room-tit-def").removeClass("hide");
        if (showType == 1) $("#room-info-def").slideDown(200); else $("#room-info-def").show();
    };
    var hideRoomInfo = function (showType) {
        $("#room-tit-def").data("op", "0");
        $("#room-tit-sel").removeClass("hide");
        $("#room-tit-def").addClass("hide");
        if (showType == 1) $("#room-info-def").slideUp(200); else $("#room-info-def").hide();
    };
    var ctrlRoomSel = function () {
        var op = $("#room-tit-def").data("op");
        if (op == "1") {
            hideRoomInfo(1);
        }
        else {
            showRoomInfo(1);
        }
    };
    $("#room-tit-sel").click(ctrlRoomSel);
    $("#room-tit-def").click(ctrlRoomSel);

    var loadPackageContent = function (_pid, _userid, _albumid) {
        _Loading.show();
        $.get('/Hotel/PackageContent', { pid: _pid, userid: _userid, albumid: _albumid }, function (html) {
            if (html) {
                $("#package-content").hide().html(html).fadeIn(200);
                showRoomInfo(0);
                setTimeout(function () {
                    $("html,body").animate({ scrollTop: $(".activity .tit").offset().top - 20 }, 300);
                }, 500);
            }

            _Loading.hide();
        });
    };

    $(".room-item").each(function () {
        $(this).click(function () {
            var _pid = $(this).data("pid");
            loadPackageContent(_pid, pubuserid, pubalbumid);
            //location.href = "/Hotel/Package/" + _pid + "?userid=" + pubuserid;
        });
    });
    $(".p-item").each(function () {
        $(this).click(function () {
            var _pid = $(this).data("pid");
            loadPackageContent(_pid, pubuserid, pubalbumid);
            //location.href = "/Hotel/Package/" + _pid + "?userid=" + pubuserid;
        });
    });

    //init comment count
    try {


        //$.get('/Hotel/GetCommentCount', { hotelid: pubhotelid }, function (back) {
        //    if (back && back.count && back.count != "0") {
        //        $("#hotel-comment-tit").html(back.count);
        //        $("#hotel-comment-div").fadeIn();
        //    }
        //    else {
        //        $("#hotel-comment-div").hide();
        //    }
        //});
        $.get(_Config.APIUrl + '/api/Hotel/GetHotelReviewCount', { hotelid: pubhotelid }, function (back) {
            if (back != 0) {
                $("#hotel-comment-tit").html(back);
                $("#hotel-comment-div").fadeIn();
            }
            else {
                $("#hotel-comment-div").hide();
            }
        });
    } catch (e) {

    }

    //加载酒店图文详情
    var loadHotelSource = false;
    var loadHotelSourcePage = function () {

        if (loadHotelSource) return;
        if (hotelSourceUrl) {

            loadHotelSource = true;

            //alert("b～");
            console.log("load source");

            //hotelSourceUrl = "/active/activepage?midx=5886";
            hotelSourceUrl = hotelSourceUrl.replace("http://www.zmjiudian.com", "");

            $("#hotel-source-body").load(hotelSourceUrl, function (response, status, xhr) {

                if (status === "success") {
                    $(".source-more-btn").show();
                    $(".source-more-btn").click(function () {

                        $(".source-more-btn").hide();
                        $("#hotel-source-body").css("max-height", "100%");
                    });
                }
                else {
                    $(".hotel-source").hide();
                }
            });
        }
        else {
            $(".hotel-source").hide();
        }
    }

    //购买须知
    var showShopRead = function () {
        $(".shopread-btn").hide();
        $(".shopread").fadeIn();
    }
    $(".shopread-btn").click(showShopRead);

    //打开地图
    $(".address").click(function () {

        var _hotelid = $(this).data("hid");
        var _lat = $(this).data("lat");
        var _lon = $(this).data("lon");

        if (isapp) {
            var _param = { title: pubhotelname, latitude: _lat, longitude: _lon };
            zmjd.openLocation(_param);
        }
        else {
            gourl("/hotel/" + _hotelid + "/map");
        }

    });

    var $win = $(window);
    var hotelSourceTop = $(".hotel-source").offset().top;
    var isload = true;

    var _scrollEvent = function () {
        var winTop = $win.scrollTop();
        var winHeight = $win.height();

        //console.log(winTop)

        if (winTop > 0 && winTop > (hotelSourceTop - winHeight)) {
            loadHotelSourcePage();
        }
    }
    _scrollEvent();

    //页面滚动事件
    $win.on('scroll', _scrollEvent);

    //套餐活动显示
    //console.log(activeJson);
    if (activeJson) {
        var activeJsonObj = JSON.parse(activeJson);
        //console.log(activeJsonObj);
        if (activeJsonObj) {

            //1 暑期送东航航空里程活动 2018.5.24~2018.8.31
            if (activeJsonObj.activeID == 1) {

                new Vue({
                    el: "#active-obj",
                    data: {
                        "activeData": activeJsonObj
                    }
                })
            }
        }
    }
});

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': _name, 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?userid=" + pubuserid + "&_isoneoff=1&_newpage=1";
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {



}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //alert(apptype)

    //init data
    _InitApp(userid, apptype, appvercode, appverno);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}

//function onloadSelected(pid, cid) {
//    location.href = "http://localhost:2001/Hotel/Package/" + pid + "?CID=" + cid
//}