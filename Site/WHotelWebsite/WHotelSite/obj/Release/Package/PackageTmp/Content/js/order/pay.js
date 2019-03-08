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

$('a[data-pay-href]').click(function (e) {
    var url = $(this).attr('data-pay-href');
    PayOrder(url);
});

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

function WeixinPayJS(code,order) {
    if (code && order) {
        var data2 = { code: code, order: order };
        $.ajax({
            type: 'POST',
            url: '/Order/WeixinPay',
            data: data2,
            datatype: 'json',
            async: true,
            success: function (result) {
                if (result.package) {
                    wx.chooseWXPay({
                        timeStamp: result.timeStamp,
                        nonceStr: result.nonceStr,
                        package: result.package,
                        signType: result.signType,
                        paySign: result.paySign,
                        success: function (res) {
                            alert("恭喜您支付成功！");
                        }
                    });
                }
                else {
                    alert(result.Message);
                }
            },
            error: function () {
                alert('网络异常，请重试！');
            }
        });
    }
}