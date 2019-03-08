$(function () {
    
    var userid = $("#userid").val();
    var latitude = $("#userlat").val();
    var longitude = $("#userlng").val();

    $("img").lazyload({
        threshold: 30,
        placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load2-16x9.png",
        effect: "fadeIn"
    });
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
    
};