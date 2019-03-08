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

function gotoPayUrl(event,orderId) {
    var isOK = checkCanPay(orderId);
    if (isOK) {
        if (!window.isMobile) {
            var newWindow = window.open();
            newWindow.location.href = "/Order/Pay?order=" + orderId;
        }
        else {
            window.location.href = "/Order/Pay?order=" + orderId;
        }
    }
    else {
        //阻止其后所有默认动作
        event.stopImmediatePropagation();
        return false;
    }
}