function shareWXTimeLine(title,content,photoUrl,shareLink){
    wx.onMenuShareTimeline({
        title: title,
        link: shareLink,
        imgUrl: photoUrl,
        success: function () {
            // 用户确认分享后执行的回调函数
            //调用更新现金红包接口
            //UpdateCash();
            var reg = new RegExp("(^|&)" + "key" + "=([^&]*)(&|$)", "i");

            var start = shareLink.indexOf("key");

            var searchStr = shareLink.substr(start);
            var r = searchStr.match(reg);
            if (r != null) {
                window.location.href = "/Coupon/ShareResult?key=" + unescape(r[2]);
            }
            else {
                location.reload(true);
            }
        },
        cancel: function () {
            // 用户取消分享后执行的回调函数
            alert("您已取消分享到朋友圈");
        }
    });
}

function GetConfig() {
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
            if (typeof(result.Success) != undefined && result.Success == 1) {
                alert("获得注册参数失败");
                return false;
            }
            else {
                //得到微信config的值
                if(result.jsApiList){
                    var array = [];
                    var jsArray = result.jsApiList.split(',');
                    for(var key in jsArray){
                        array.push("" + jsArray[key]);
                    }
                    wx.config({
                        debug: result.debug,
                        appId: result.appId,
                        timestamp: "" + result.timestamp,
                        nonceStr: result.nonceStr,
                        signature: result.signature,
                        jsApiList: array
                    });
                }
            }
        }
    });
}

//function UpdateCash() {
//    var guid = getQueryString("key");//获得guid参数值
//    $.ajax({
//        url: '/Coupon/UpdateCashCoupon',
//        type: 'POST',
//        data: { key: guid },
//        dataType: 'json',
//        error: function () {
//            alert("网络故障，请重试！");
//        },
//        success: function (result) {
//            if (result.Success === 0) {
//                alert("您已成功领取返现");
//            }
//            else {
//                alert("领取返现失败");
//            }
//        }
//    });
//}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

function grabCoupon2() {
    var phoneNumReg2 = /^((100)|(13[0-9])|(15[0-9])|(19[0-9])|(18[0-9])|(17[0-9])|(16[0-9])|(14[0-9]))\d{8}$/;
    var guid = $("#coupon1").attr("data-guid");
    var url = "/Coupon/Result?key=" + guid;
    var phoneNum = $.trim($("#phoneNum").val());
    if (!phoneNum) {
        alert("请先填写手机号");
        return false;
    }
    else if (!phoneNumReg2.test(phoneNum)) {
        alert("手机号格式错误");
        return false;
    }
    $.post(url, { phoneNum: phoneNum }, function (res) {
        if(res.url){
            window.location.href = res.url;
        }
        else {
            alert(res.Message);
            return false;
        }
    },"json");//post方式访问结果页
}

function getCoupon2() {
    window.location.href = "/Coupon/Download";
    //$.get(url, { phoneNum: phoneNum });//post方式访问结果页
}

function requestRebate(obj,id) {
    var that = obj;
    var url = "/Coupon/Rebate";
    $.post(url, { orderid: id }, function (res) {
        if (res.Success == 0) {
            //alert("申请返现成功！");
            //var userId = getCookie("userId");
            //location.reload(true);
            $("#requestTip").show();
            $("#waitRequestTip").hide();
            $("#requestTipLink").show();
            $("#waitRequestTipLink").hide();
            document.body.id = 'msgBody';
        }
        else {
            alert(res.Message);
            return false;
        }
    }, "json");//post方式访问
}

function CanNotRequest() {
    $("#requestTip").hide();
    $("#waitRequestTip").show();
    $("#requestTipLink").hide();
    $("#waitRequestTipLink").show();
    document.body.id = 'msgBody';
}