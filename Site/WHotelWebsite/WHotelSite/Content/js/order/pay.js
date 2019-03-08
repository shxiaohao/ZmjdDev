function PayOrder(url) {
    //showSpinner(true);
    //orderid = url.split('/')[4];
    if (window.isMobile) {
        window.location.href = url;
    }
    else {
        var paywindow2 = window.open();
        paywindow2.location = url;
        //$('#popPay').show();
    }
    return;
}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) { 
    //init data
    _InitApp(userid, apptype, appvercode, appverno);    
    //目前不用
    //zmjd.whotelGetAndroidSupportedPayMode();
}

function getAndroidSupportedPayModeSuccess(data) {
    $("#ChinaPayAndroidPay").val(data);
}

$('a[data-pay-href]').click(function (e) {

    var TerminalType = $("#TerminalType").val();
    var orderid = $("#orderid").val();
    var payType = $(this).attr('pay-type');
    var subPayType = $(this).attr('sub-pay-type');
    var url = $(this).attr('data-pay-href');
    var finishurl  = $(this).attr('data-finishurl');
    var successidentifier = $(this).attr('data-successidentifier');
    var failedidentifier = $(this).attr('data-failedidentifier');

    if (IsApp)
    {
        var paydata = {
            "paytype": payType,//支付方式：alipay|upay|tenpay|chinaflashpay
            "subpaytype": subPayType, //chinaflashpay 下需要细分支付方式
                "orderid": orderid,//
                "finishurl": finishurl ,//支付完成页面
                "webpayurl": addUrlParam(url , "_modalpage=1"),// h5支付页面（如银联）
                "successidentifier": successidentifier,//支付成功标识（拦截用）
                "failedidentifier": failedidentifier,//支付失败标识（拦截用）
        }
        zmjd.whotelAppPay(paydata);
    }
    else {
        PayOrder(url);
    } 
});

function addUrlParam(url, param)
{
    if(url.indexOf("?") > 0)
    {
        return url + "&" + param;
    }
    else {
        return url + "?" + param;
    }
}


$('a[data-submitorder-href]').click(function (e) {
    var url = $(this).attr('data-submitorder-href');
    submitOrderAndGotoPay(url);
});

function submitOrderAndGotoPay(url) {
    var orderFormUrl = $.trim($("#orderFormUrl").val());
    var paramArray = orderFormUrl.split(',');
    if (paramArray.length < 4) {
        alert("订单参数错误，请重新预订");
        return false;
    }
    var params = {
        roomCount: paramArray[0],
        contact: paramArray[1],
        contactPhone: paramArray[2],
        note: paramArray[3]
    };
    if (/,,/.test(',' + params.contact + ',')) {
        return alertCheckMsg('请填写入住人姓名');
    }
    if (!params.contactPhone) {
        return alertCheckMsg('请填写手机号码');
    }
    if (!phoneNumReg.test(params.contactPhone)) {
        return alertCheckMsg('请填写有效的手机号码');
    }

    showSpinner(true);
    $.ajax({
        url: url,
        type: 'POST',
        data: params,
        dataType: 'json',
        error: function () {
            alert("网络服务错误,请重试");
            showSpinner(false);
            return false;
        },
        success: function (r) {
            if (r.url) {
                var id = r.url.split('/')[4];
                $("#submitOrderValue").val(id);
                if (window.isMobile) {
                    window.location.href = r.url;
                }
                else {
                    showSpinner(false);
                    var paywindow = window.open();
                    paywindow.location = r.url;
                    //showSpinner(true);
                    //$('#pop').show();
                }
                return;
            }
            else if (r.error) {
                alert(r.error);
                showSpinner(false);
                return false;
            }
        }
    });
}

$(document).ready(function () {
    showSpinner.prefetch();
});

//进入页面绑定子元素的单击事件
$(document).ready(function () {
    $("ul").delegate('input', 'click', function () {
        //$(this).parents("ul").find("input").each(function () {
        //    $(this).removeAttr("checked");
        //});
        //$(this).attr("checked", true);

        $("#gotoPay").attr("data-href", $(this).val());
    });
});

$("#gotoPay").click(function () {
    var href = $(this).attr("data-href");
    if (href) {
        $(this).parents("#myModal").find(".close").click();//隐藏弹出层
        //ToDo 如果layout不是_Layout.cshtml 那么window.isMobile不存在了
        if (!window.isMobile) {
            var paywindow = window.open();
            paywindow.location.href = href;
        }
        else {
            window.location.href = href;//付款页面或选择付款方式页面
        }
    }
});

function closepopPay() {
    showSpinner(false);
    $("#popPay").hide();
}

function checkCanPay(orderId) {
    var isOk = false;
    var data2 = { order: orderId };
    
    $.ajax({
        type: 'POST',
        url: '/Order/CheckCanPay',
        data: data2,
        datatype: 'json',
        async: false,
        success: function (result) {
            if (result.Success == 1) {
                alert(result.Message);
            }
            else if (result.bCanPay) {
                isOk = true;
            }
            else {
                alert(result.Message);
            }
        },
        error: function () {
            alert('网络出现异常！');
        }
    });
    
    //$.get('/Order/CheckCanPay', data, function (result) {
    //    if (result.Success == 1) {
    //        alert(result.Message);
    //        isOk = false;
    //    }
    //    else if (result.bCanPay) {
    //        isOk = true;
    //    }
    //    else {
    //        alert(result.Message);
    //        isOk = false;
    //    }
    //});
    return isOk;
}



function GetWeixinApiConfig() {

    var url = location.href.split("#")[0];
    $.ajax({
        url: '/Active/GetWeixinServiceConfigInfo',
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
                  'onMenuShareTimeline',
                  'onMenuShareAppMessage',
                  'hideOptionMenu'
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


function WeixinPayJS(order) {
    if (order) {
        var data2 = { order: order };
        $.ajax({
            type: 'POST',
            url: '/Order/WeixinPay',
            data: data2,
            datatype: 'json',
            async: true,
            success: function (result) {
                if (result.package != null && result.package != "" && result.package != undefined) {
                    try {
                        wx.chooseWXPay({
                            timestamp: result.timeStamp,
                            nonceStr: result.nonceStr,
                            package: result.package,
                            signType: result.signType,
                            paySign: result.paySign,
                            success: function (res) {
                                //debugger;
                                alert("恭喜您支付成功！");
                            }
                        });

                    } catch (e) {
                        alert("支付异常：" + e.message);
                    }
                }
                else {
                    alert("return_msg:" + result.return_msg);
                }
            },
            error: function () {
                alert('网络异常，请重试！');
            }
        });
    }
}