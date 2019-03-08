var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var canShareRedCoupon = $("#canShareRedCoupon").val() == "1";
var appShareLink = $("#appShareLink").val();

$(function () {

    //非团购 & 非积分 & 非免费领取 产品，购买完成后弹出的红包分享
    if (canShareRedCoupon) {

        //延时加载活动说明
        setTimeout(function () {

            //分享提示图片动态加载
            var shareTipImg = $(".group-share-tip img");
            setImgOriSrc(shareTipImg);

        }, 100);

        //右上角分享提示点击事件
        $(".group-share-tip").click(function () {
            $(this).hide();
        });

        //红包分享事件
        var shareRedCoupon = function () {

            if (inWeixin) {
                $(".group-share-tip").show();
            }
            else if (isapp) {
                gourl(appShareLink);
            }
        }

        //右上角发红包浮窗
        $(".send-red-coupon-float").click(shareRedCoupon);
        var showRedCouponFloat = function () {
            $(".send-red-coupon-float").fadeIn();
        }

        var _html = $("#send-red-coupon-template").html();

        _Modal.show({
            title: '',
            content: _html,
            showClose: true,
            confirmText: '分享领红包',
            confirm: function () {

                //分享
                shareRedCoupon();

                //显示浮窗
                showRedCouponFloat();

                _Modal.hide();
            },
            cancel: function () {

                //显示浮窗
                showRedCouponFloat();
            },
            showCancel: false
        });

        //$("._modal-section").css("top", "15%");
    }

});

$("button[data-return-url]").click(function () {
    var $that = $(this);
    window.location.href = $that.attr("data-return-url");
});

$("button[data-cancel-url]").click(function () {
    var $that = $(this);
    var urlStr = $that.attr("data-cancel-url");
    var index1 = urlStr.indexOf('?');
    var index2 = urlStr.indexOf('=');

    var url = urlStr.substring(0, index1);
    var paramName = urlStr.substring(index1 + 1, index2);
    var paramValue = parseInt(urlStr.substr(index2 + 1), 10);//链接后的订单ID
    var data = { order: paramValue };
    $.ajax({
        type: 'POST',
        url: url,
        data:data,
        datatype: 'json',
        async: true,
        success: function (r) {
            if(r.success === "True"){
                alert(r.message);
                window.location.href = r.url;
            }
            else {
                alert(r.message);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

$("button[data-delete-url]").click(function () {

    if (!confirm('您确认删除该订单吗？')) {
        return;
    }

    var $that = $(this);
    var urlStr = $that.attr("data-delete-url");
    var index1 = urlStr.indexOf('?');
    var index2 = urlStr.indexOf('=');

    var url = urlStr.substring(0, index1);
    var paramName = urlStr.substring(index1 + 1, index2);
    var paramValue = parseInt(urlStr.substr(index2 + 1), 10);//链接后的订单ID
    var data = { order: paramValue };
    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        datatype: 'json',
        async: true,
        success: function (r) {
            if (r.success === "true") {
                alert(r.message);
                window.location.href = r.url;
            }
            else {
                alert(r.message);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

$("button[data-comment-url]").click(function () {
    window.location.href = $(this).attr("data-comment-url");//直接跳到点评页面
});

$("button[data-pay-url]").click(function () {
    window.location.href = $(this).attr("data-pay-url");//直接跳到支付页面
});

//订单列表取消 订单
$("a[data-cancel-url]").click(function () {
    var $that = $(this);
    var urlStr = $that.attr("data-cancel-url");
    var index1 = urlStr.indexOf('?');
    var index2 = urlStr.indexOf('=');

    var url = urlStr.substring(0,index1);
    var paramName = urlStr.substring(index1 + 1, index2);
    var paramValue = parseInt(urlStr.substr(index2 + 1), 10);//链接后的订单ID
    var data = { order: paramValue };
    $.ajax({
        type: 'POST',
        url: url,
        data:data,
        datatype: 'json',
        async: true,
        success: function (r) {
            if (r.success === "True") {
                alert(r.message);
                window.location.href = r.url;
            }
            else {
                alert(r.message);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

function bindPayOrderId(event,orderId) {
    var isOK = checkCanPay(orderId);
    if (isOK) {
        $("#upayRadio").val("/payment/direct?channel=upay&order=" + orderId);
        $("#alipayRadio").val("/payment/direct?channel=alipay&order=" + orderId);
        $("#gotoPay").attr("data-href", "/payment/direct?channel=upay&order=" + orderId);//默认银行卡支付
    }
    else {
        //阻止其后所有默认动作
        event.stopImmediatePropagation();
        return false;
    }

    
    //$("#alipayLink").attr("href", "/payment/direct?channel=alipay&order=" + orderId);
    //$("#upayLink").attr("href", "/payment/direct?channel=upay&order=" + orderId);
}

function gotoPayUrl(event, orderId, payType) {
    var isOK = true;//checkCanPay(orderId);
    if (isOK) {
        if (!window.isMobile) {
            var newWindow = window.open();
            newWindow.location.href = "/Order/Pay?order=" + orderId + "&payType=" + payType;
        }
        else {
            window.location.href = "/Order/Pay?order=" + orderId + "&payType=" + payType;
        }
    }
    else {
        //阻止其后所有默认动作
        event.stopImmediatePropagation();
        return false;
    }
}

function gotoPayUrl2(orderId) {
    window.location.href = "/Order/Pay?order=" + orderId + "&payChannels=tenpay,alipay,chinapay" ;
}

function openInvoice(orderid, hotelid, pid, packagetype, userid) {
    window.location.href = "/Order/OpenInvoice?orderID=" + orderid + "&hotelID=" + hotelid + "&pid=" + pid + "&packageType=" + packagetype + "&userid=" + userid;
}