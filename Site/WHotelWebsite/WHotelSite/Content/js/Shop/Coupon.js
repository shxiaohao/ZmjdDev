var operatorID = $("#operatorID").val();
var _Config = new Config();

function startTime()
{
    var dtpicker = new mui.DtPicker({
        type: "date",//设置日历初始视图模式 
        beginDate: new Date(2000, 01, 01),//设置开始日期 
        endDate: new Date(2100, 12, 31),//设置结束日期 
        labels: ['年', '月', '日'],//设置默认标签区域提示语 
    })
    dtpicker.show(function (e) {
        $("#startTime").text(e);
    })
}
function endTime() {
    var dtpicker = new mui.DtPicker({
        type: "date",//设置日历初始视图模式 
        beginDate: new Date(2000, 01, 01),//设置开始日期 
        endDate: new Date(2100, 12, 31),//设置结束日期 
        labels: ['年', '月', '日'],//设置默认标签区域提示语 
    })
    dtpicker.show(function (e) {
        $("#endTime").text(e);
    })
    dtPicker.dispose()
}
var first = 1;
function UpdateCouponState(exchangeNo) {
    var OperationState =$("#OperationState").val();
    if (OperationState == 4)
    {
        alert("该券码未预约,不能核销");
        return false;
    }
    //var isbooked = $("#isbooked").val();
    //if (isbooked=="false") {
    //    alert("该券码未预约,不能核销");
    //    return false;
    //}
    
    if (first != 1)//防止多次触发事件
    {
        return false;
    }
    first = 2;
    var exchangeNo = $("#CheckExchangeNo").html();
    $.ajax({
        type: "POST",
        url: "/Shop/UpdateCouponState",
        data: { exchangeNo: exchangeNo },
        success: function (data) {
            first = 1;
            if (data == 1) {
                window.location.reload();
            }
            else if(data==0){
                //alert("兑换失败，联系管理员");
                window.location.href = "/Shop/ShopLogin";
            }
            else if (data == -1)
            {
                alert("核销失败！")
            }
            else if (data == -2)
            {
                alert("核销失败,已核销！")
            }
            else if (data == -3) {
                alert("核销失败，需要先预约！")
            }
        },
        error: function (XMLHttpRequest, textStatus) {
            alert("请联系管理员");
        }
    })
}

function LogOut() {
    //Store.Remove("OperatorName");
    //Store.Remove("PWD");
    Store.Remove("SignIn")
    window.location.href="/Shop/ShopLogin"
}
function signOut() {
    Store.Remove("SignIn")
    window.location.href = "/Shop/ShopLogin?autoLogin=false";
}

function CouponList(type) {
    window.location.href = "/Shop/Coupon?type=" + type

}
function SearchCoupon(type) {
    var startTime = "";
    var endTime = "";
    startTime = $("#startTime").text();
    endTime = $("#endTime").text();
    window.location.href = "/Shop/Coupon?startTime=" + startTime + "&endTime=" + endTime + "&type=" + type
}

function checkExchangeNo(exchangeno) {

    var exchangeNo = exchangeno;
    if (exchangeNo == "" || exchangeNo == null)
    {
        exchangeNo = $("#exchangeNo").val();
    }
    $.ajax({
        type: "POST",
        url: "/Shop/checkExchangeNo",
        data: { exchangeNo: exchangeNo },
        success: function (data) {

            console.log(data)

            //var msg = "消费码　  |  " + data.ExchangeNo + "\r\n" + "产品名称  |  " + data.SPUName + "\r\n" + "产品类型  |  " + data.SKUName
            var ss = ($(document).scrollTop()+20) + "px";
            $(".coupon-alert-content").css("top", ss);
            var packageInfoHtml = "";
            if (data.PackageInfoList != null)
            {
                packageInfoHtml = "<ul>";
                for (var i = 0; i < data.PackageInfoList.length; i++) {
                    packageInfoHtml += "<li>";
                    packageInfoHtml += data.PackageInfoList[i];
                    packageInfoHtml += "</li>";
                }
                packageInfoHtml += "</ul>";
            }
            if (data.IsShowPrice == false)
            {
                $("#ShowPrice").hide();
            }
            if (data.State == 0)
            {
                window.location.href = "/Shop/ShopLogin";
            }
            else if (data.State == 2 && data.RefundState==0) {
                //var exchagenoInfo = "";
                if (data.CouponNote != null && data.CouponNote.length > 0) {
                    $("#couponNote").html("(" + data.CouponNote + ")");
                }
                //else {
                //    exchagenoInfo = data.ExchangeNo;
                //}
                //var exchagenoInfo = data.ExchangeNo + "(" + data.CouponNote + ")";
                $("#CheckExchangeNo").html(data.ExchangeNo);
                $("#spshopName").html(data.SupplierName);
                $("#spskuName").html(data.SKUName);
                $("#spprice").html("¥" + data.Price);

                $("#productInfo").html(packageInfoHtml);

                $("#exchangeState").html("已支付");

                $("#setUserId").val(data.UserID);
                $("#exchangeid").val(data.Id);
                $("#OperationState").val(data.OperationState);

                $("#TradingTime").hide();
                $("#sptime").hide();
                $("#UseCoupon").show();
                //$("#UsedCoupon").hide();
                $("#divAlertCouponMsgBg").show();
                $("#divAlertCouponMsg").show();

                var expiretime = new Date(parseInt(data.ExpireTime.slice(6)) + 24 * 60 * 60 * 1000);
                
                if (expiretime < new Date())
                {
                    $("#exchangeState").html("已过期");
                    $("#UseCoupon").hide();
                }
                //$("#divAlertCouponMsg").attr("style", "width:100%;height:100%;background-color:#000000;position: absolute;z-index:9999;display:block;background: rgba(0,0,0, 0.6);");
            }
            else if (data.State == 3) {
                //var exchagenoInfo = "";
                if (data.CouponNote != null && data.CouponNote.length > 0) {
                    $("#couponNote").html("(" + data.CouponNote + ")");
                }
                //else {
                //    exchagenoInfo = data.ExchangeNo;
                //}
                $("#CheckExchangeNo").html(data.ExchangeNo);//data.ExchangeNo
                $("#spshopName").html(data.SupplierName);
                $("#spskuName").html(data.SKUName);
                $("#spprice").html("¥" + data.Price);
                $("#exchangeState").html("已核销");

                $("#sptime").html(ChangeDateFormat(data.ConsumeTime));

                $("#productInfo").html(packageInfoHtml);

                $("#exchangeid").val(data.Id);
                $("#OperationState").val(data.OperationState);

                $("#UseCoupon").hide();
                //$("#UsedCoupon").show();
                $("#divAlertCouponMsgBg").show();
                $("#divAlertCouponMsg").show();
                //$("#divAlertCouponMsg").attr("style", "width:100%;height:100%;background-color:#000000;position: absolute;z-index:9999;display:block;background: rgba(0,0,0, 0.6);");
            }
            else if (data.State == 4 || data.State == 5 || data.State == 6 || data.State == 7 || data.State == 8)
            {
                if (data.CouponNote != null && data.CouponNote.length>0) {
                    $("#couponNote").html("(" + data.CouponNote + ")");
                }
                $("#CheckExchangeNo").html(data.ExchangeNo);
                $("#spshopName").html(data.SupplierName);
                $("#spskuName").html(data.SKUName);
                $("#spprice").html("¥" + data.Price);
                //if (data.State == 2 && data.RefundState > 0)
                //{
                //    $("#exchangeState").html("退款中");
                //} else
                    if (data.State == 4) {
                    $("#exchangeState").html("已取消");
                }
                else if (data.State == 5) {

                    $("#exchangeState").html("已退款");
                }
                else if (data.State == 6) {
                    $("#exchangeState").html("超时支付");
                }
                else if (data.State == 7) {
                    $("#exchangeState").html("待退款");
                }
                else if (data.State == 8) {
                    $("#exchangeState").html("已过期");
                }

                $("#TradingTime").hide();
                $("#productInfo").html(packageInfoHtml);

                $("#exchangeid").val(data.Id);
                $("#OperationState").val(data.OperationState);

                $("#UseCoupon").hide();
                $("#divAlertCouponMsgBg").show();
                $("#divAlertCouponMsg").show();
            }
            else {
                alert("消费券不可用");
            }

            if (data.NeedPhoto && data.PhotoUrl.length > 0) {
                $("#lookAvatar").show();
                $("#avatarShow").attr("src", data.PhotoUrl);

                if (data.State == 2) {
                    $("#reUploadPhoto").show();
                }
                else {
                    $("#reUploadPhoto").hide();
                }
            }
            else {
                $("#lookAvatar").hide();
                $("#reUploadPhoto").hide();
            }
        }
    })
}

function closeDiv() {
    //$("#divAlertCouponMsg").attr("style", "display:none");
    $("#divAlertCouponMsgBg").hide();
    $("#divAlertCouponMsg").hide();
}
function ChangeDateFormat(cellval) {
    var date = new Date(parseInt(cellval.replace("/Date(", "").replace(")/", ""), 10));
    var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
    var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
    return date.getFullYear() + "-" + month + "-" + currentDate;
}

function Export() {

    startTime = $("#startTime").text();
    endTime = $("#endTime").text();
    window.location = "/shop/ExceptUseCoupon?startTime=" + startTime + "&endTime=" + endTime;
}

function lookAvatar() {

    var ss = ($(document).scrollTop() + 20) + "px ";
    //$(".coupon-Avatar").css("top", ss);
    //$(".Avatar-alert").css("top", ss);
    $("#divAlertAvatarBg").show();
    $("#divAlertAvatar").show();
}

function uploadPhotoState() {

    //更新重新上传的状态
    var _userid = $("#setUserId").val();
    var _exid = $("#exchangeid").val();
    var _uploadStateDic = { userid: _userid, CouponID: _exid, operatorUserID: operatorID };
    console.log(_uploadStateDic);
    $.get(_Config.APIUrl + '/api/coupon/SetUserCanReUpdatePhotoListByUserID', _uploadStateDic, function (_data) {

        console.log(_data);
        alert("照片重新上传入口已开启，请提醒用户在“我的订单”-“消费券”中重新上传照片。");

    });
}

function closeDivAvatar() {
    //$("#divAlertCouponMsg").attr("style", "display:none");
    $("#divAlertAvatarBg").hide();
    $("#divAlertAvatar").hide();
}


function formatDate(dt) {
    var year = dt.getFullYear();
    var month = dt.getMonth() + 1;
    var date = dt.getDate();
    var hour = dt.getHours();
    var minute = dt.getMinutes();
    var second = dt.getSeconds();
    return year + "-" + month + "-" + date + " " + hour + ":" + minute + ":" + second;
}
