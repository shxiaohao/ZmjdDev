$(function () {
    
    var userid = $("#userid").val();
    var latitude = $("#userlat").val();
    var longitude = $("#userlng").val();

    //load 发现专题
    function loadFindTheme() {
        var userid = $("#userid").val();
        var districtId = $("#districtId").val();
        var districtName = $("#districtName").val();
        var userlat = $("#userlat").val();
        var userlng = $("#userlng").val();
        var geoScopeType = $("#geoScopeType").val();
        
        var dic = {};
        dic["userid"] = userid;
        dic["districtId"] = districtId;
        dic["districtName"] = districtName;
        dic["lat"] = userlat;
        dic["lng"] = userlng;
        dic["geoScopeType"] = geoScopeType;

        $.get('/App/FindTheme', dic, function (htmls) {
            if (htmls) {
                $("#find-theme-panel").html(htmls);
            }
        });
    }
    setTimeout(loadFindTheme, 50);

    //load 大家都说好
    function loadFindRecHotel() {
        var userid = $("#userid").val();
        var districtId = $("#districtId").val();
        var userlat = $("#userlat").val();
        var userlng = $("#userlng").val();

        $.get('/App/FindHotRecommendHotel', { userid: userid, districtId: districtId, lat: userlat, lng: userlng }, function (htmls) {
            if (htmls) {
                $("#find-rechotel-panel").html(htmls);
            }
        });
    }
    setTimeout(loadFindRecHotel, 300);

    var $win = $(window);
    var isload = true;
    $win.on('scroll', function () {
        var tagTop = $(".find-hot-cmts-foot").offset().top;
        var winTop = $win.scrollTop();
        var winHeight = $win.height();

        if (winTop >= tagTop - winHeight - 100) {
            loadComments(false);
        }
    });

    var start = 0;
    var count = 6;

    //load 正在点评
    function loadComments(isfirst) {
        if (isload) {

            isload = false;

            //下一页
            start += count;
            if (isfirst) { start = 0; }

            var districtId = $("#districtId").val();
            var userlat = $("#userlat").val();
            var userlng = $("#userlng").val();

            $.get('/App/FindNewComments', { s: start, c: count, districtId: districtId, lat: userlat, lng: userlng }, function (htmls) {
                if (htmls) {

                    if (htmls != "" && htmls.indexOf("div") >= 0) {
                        $(".find-hot-cmts").html($(".find-hot-cmts").html() + htmls);

                        if (isfirst) {
                            $(".scrollpageloading").show();
                            $("#cmts-tit").show();
                        }

                        $("img").lazyload({
                            threshold: 20,
                            placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load-1x1.png",
                            effect: "show"
                        });
                    }
                    else {
                        $(".scrollpageloading").html("<div>没有更多了</div>");
                    }

                    isload = true;
                }
            });
        }
    }
    setTimeout(function () { loadComments(true); }, 400);
});

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

//目的地的点击处理事件
var districtClick = function (districtId, districtName, lat, lng, geoScopeType) {
    alert("districtId:" + districtId);

    $("#districtId").val(districtId);
    $("#districtName").val(districtName);
    $("#userlat").val(lat);
    $("#userlng").val(lng);
    $("#geoScopeType").val(geoScopeType);

    //刷新发现专题
    loadFindTheme();
};

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        imgObj.attr("src", orisrc);
        imgObj.data("orisrc", "");
    }
};