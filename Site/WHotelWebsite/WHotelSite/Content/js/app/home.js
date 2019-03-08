$(function () {

    var isThanVer46 = $("#isThanVer46").val() == "1";
    var isThanVer47 = $("#isThanVer47").val() == "1";
    var userid = $("#userid").val();
    var hasUser = (userid != "" && userid != "0");

    //VIP专享
    if (hasUser) {
        setTimeout(function () {
            $.get('/App/HomeAlbumPackage', { userid: userid, albumId : 10 }, function (html) {
                if (html && html.indexOf("ul") >= 0) {
                    $("#vipPackageSection").fadeIn(1000);
                    $("#vipPackageSection").html(html);
                }
            });
        },
        100);
    }
    else {
        $("#vipPackageSection").hide();
    }

    ////闪购
    //setTimeout(function () {
    //    $.get('/App/HomeFlashDeals', { userid: userid }, function (html) {
    //        if (html) {
    //            $("#flashDealsSection").html(html);
    //        }
    //    });
    //},
    //200);

    ////团购
    //setTimeout(function () {
    //    //$.get('/App/HomeGroupDeals', { userid: userid }, function (html) {
    //    //    if (html) {
    //    //        $("#groupDealsSection").html(html);
    //    //    }
    //    //});
    //    $.get('/App/HomeAlbumPackage', { userid: userid, albumId: 12 }, function (html) {
    //        if (html && html.indexOf("ul") >= 0) {
    //            $("#groupDealsSection").html(html);
    //        }
    //    });
    //},
    //300);

    //【4.7】
    if (isThanVer47 && hasUser) {

        //最近搜索的酒店
        setTimeout(function () {
            $.get('/App/HomeDistrictSearchHistory', { userid: userid }, function (html) {
                if (html && html.indexOf("ul") >= 0) {
                    $("#disSearchHisSection").fadeIn(1000);
                    $("#disSearchHisSection").html(html);
                }
            });
        },
        50);

        //最近浏览过的酒店
        setTimeout(function () {
            $.get('/App/HomeRecentSeeHotels', { userid: userid }, function (html) {
                if (html && html.indexOf("ul") >= 0) {
                    $("#recentSeeSection").fadeIn(1000);
                    $("#recentSeeSection").html(html);
                }
            });
        },
        400);

        //朋友推荐的酒店
        setTimeout(function () {
            $.get('/App/HomeFriendRecHotels', { userid: userid }, function (html) {
                if (html && html.indexOf("ul") >= 0) {
                    $("#friendRecSection").fadeIn(1000);
                    $("#friendRecSection").html(html);
                }
            });
        },
        500);

        //其它特惠套餐
        setTimeout(function () {
            $.get('/App/HomeAlbumPackage', { userid: userid, albumId: 1 }, function (html) {
                if (html && html.indexOf("ul") >= 0) {
                    $("#weekPackageSection").fadeIn(1000);
                    $("#weekPackageSection").html(html);
                }
            });
        },
        600);
    }
    else {

        //首页顶部
        setTimeout(function () {
            $.get('/App/HomeTop', { userid: userid }, function (html) {
                if (html) {
                    $("#homeTopSection").show();
                    $("#homeTopSection").html(html);
                }
            });
        },
        50);

        //其它特惠套餐
        setTimeout(function () {
            $.get('/App/HomeHotelPackageList', { userid: userid }, function (html) {
                if (html) {
                    $("#hotelPackageListSection").html(html);

                    $(".home-hlist-panel").show();
                    $("#weekPackageSection2").show();

                    $("img").lazyload({
                        threshold: 30,
                        placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load2-16x9.png",
                        effect: "fadeIn"
                    });
                }
            });
        },
        400);

    }

    //城市选择列表（4.6版后的app不需要加载）
    var latitude = $("#userlat").val();
    var longitude = $("#userlng").val();
    if (!isThanVer46) {
        setTimeout(function () {
            $.get('/App/CityMenu', { userlat: latitude, userlng: longitude }, function (html) {
                if (html) {
                    $("#cityMenuSection").html(html);

                    $("#c-s-cancel").click(function () {
                        if (checkCancel()) {
                            setTimeout(function () { $(".home-city-section").hide(); }, 210);
                            $(".home-city-section").animate({ left: "100%" }, 200);
                            $(".home-panel").show();
                        }
                    });
                    $("#c-s-back").click(function () {
                        checkCancel();
                        setTimeout(function () { $(".home-city-section").hide(); }, 210);
                        $(".home-city-section").animate({ left: "100%" }, 200);
                        $(".home-panel").show();
                    });
                }
            });
        },
        500);

        //show city list
        $(".home-search").click(function () {
            $(".home-city-section").show();
            $(".home-panel").hide();
            $(".home-city-section").animate({ left: "0" }, 150);
            setTimeout(function () {
                $("html,body").animate({ scrollTop: 0 }, 500);
            },
            160);
        });
    }
});

//目的地的点击处理事件
var districtClick = function (districtId, districtName, lat, lng, geoScopeType, url, cache) {
    if (cache) {
        setCitySearchHistory(districtId, districtName);
        try {
            initCitySearchHistory();
        } catch (e) {

        }
    }
    $("#c-s-back").click();
    location.href = url;
};

function goto(param) {
    var isApp = $("#isApp").val();
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (isApp == "0") {
        url = "http://www.zmjiudian.com/" + param;
    }

    this.location = url;
}

function gotopage(param) {
    var isApp = $("#isApp").val();
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    if (isApp == "0") {
        url = "http://www.zmjiudian.com/" + param;
    }
    this.location = url;
}

function gourl(url) {
    location.href = url;
}

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        imgObj.attr("src", orisrc);
        imgObj.data("orisrc", "");
    }
};

function appOnResume()
{
    try {
        
    } catch (e) {

    }
}