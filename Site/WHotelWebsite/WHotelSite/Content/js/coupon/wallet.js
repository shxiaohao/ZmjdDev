var _Config = new Config();
//_Config.APIUrl = "http://192.168.1.114:8000";

$(document).ready(function () {
    
    //app type
    var appType = $("#appType").val();
    var isThanVer5_0 = $("#isThanVer5_0").val() == "1"; //appType = "ios";isThanVer5_0 = true;
    var isThanVer5_2 = $("#isThanVer5_2").val() == "1";
    var isThanVer5_4 = $("#isThanVer5_4").val() == "1";

    var pageTag = $("#pageTag").val();
    var mode = $("#mode").val();
    var pubuserid = $("#userid").val();

    //我的钱包总菜单
    $(".walletTagPanel li").each(function () {
        var tag = $(this);
        var tagType = tag.data("tag");
        var url = tag.data("url");
        tag.click(function () {

            if (appType == "android" || isThanVer5_4) {
                location.href = url + "&_newpage=1";//location.pathname + "/" + tagType + location.search;
            }
            else {
                if (isThanVer5_0) {
                    location.href = "whotelapp://www.zmjiudian.com/gotopage?url=" + url;
                }
                else {
                    switch (tagType) {
                        case "cash":
                        case "coupon":
                        case "point":
                        case "roomcoupon":
                        case "airmiles":
                        case "productcoupon":
                            {
                                location.href = url;//location.pathname + "/" + tagType + location.search;
                                break;
                            }
                        default:
                            {
                                if (appType == "") {
                                    location.href = url;
                                }
                                else {
                                    location.href = "whotelapp://www.zmjiudian.com/gotopage?url=" + url;
                                }
                                break;
                            }
                    }
                }
            }
        });
    });

    $(".wallet-tap-ul li").each(function () {
        $(this).click(function () {
            //change css
            initTapLiState();
            $(this).addClass("li-select");

            //display tap
            var tapId = $(this).data("tap");
            if ($("#" + tapId)) {
                $("#" + tapId).fadeIn(0);
            }
        });
    });

    function initTapLiState()
    {
        $(".wallet-tap-ul li").each(function () {
            $(this).removeClass("li-select");
            var tapId = $(this).data("tap");
            if ($("#" + tapId)) {
                $("#" + tapId).hide();
            }
        });
    }

    /* 航空里程相关 */
    if (pageTag == "airmiles") {

        //删除常旅客
        $("#del-miles-link").click(function () {

            if (confirm("确定解绑积分卡？")) {
                var _cardId = $(this).data("cardid"); console.log("del id:" + _cardId);
                var _delDic = { "cardid": _cardId };
                $.get('/Coupon/DeleteEaCard', _delDic, function (result) {

                    var _msg = result.Message;
                    var _suc = result.Success; //suc = "0";

                    if (_suc == 0 || _suc == "0") {

                        console.log(_msg)

                        //刷新钱包页
                        Global.Monito.Publisher("coupon", "wallet2", 1);

                        location.reload();;
                    }
                    else {
                        alert(_msg);
                    }
                });
            }

        });

        if (mode != "detail") {

            //开启刷新机制（500毫秒检测一次）
            Global.Monito.Listener("coupon", "wallet", 1, 500);
        }
    }
    else if (pageTag == "roomcoupon") {

        //商户兑换
        $(".shopexchange").each(function () {

            var _exid = $(this).data("exid");
            $(this).click(function () {

                var _tipObj = $("#shopexchange_" + _exid);
                var _tipHtml = _tipObj.html();

                _Modal.show({
                    title: '兑换说明',
                    content: _tipHtml,
                    confirmText: '好的',
                    confirm: function () {
                        _Modal.hide();
                    },
                    showCancel: false
                });
            });
        });

        //500平日券兑换
        $(".exchangebtn500").click(function () {
            bootbox.confirm({
                message: "<div class='alert-rulesmsg'>兑换房券请联系客服<br />4000-021-702</div>",
                buttons: {
                    confirm: { label: '联系客服', },
                    cancel: { label: '取消', }
                },
                callback: function (result) {
                    if (result) {
                        location.href = "tel:4000021702";
                    }
                },
                closeButton: false,
                onEscape: function () { }
            });
        });

        //大团购支付尾款
        $(".pay-stepgroup").each(function () {

            $(this).click(function () {

                var _depositskuid = $(this).data("depositskuid");
                var _tailskuid = $(this).data("tailskuid");
                var _tailprice = $(this).data("tailprice");
                var _corderid = $(this).data("corderid");
                var _bookpostion = $(this).data("bookpostion");
                var _paynum = 1;

                goCouponBook(_bookpostion, _tailskuid, _paynum, pubuserid, 0, _corderid);

            });
        });

    }
    else if (pageTag == "productcoupon") {

        //消费券的使用相关操作
        var _productModelBg = $(".product-model-bg");
        _productModelBg.click(function () {
            $(this).hide();
            $(".use-section").each(function () {
                $(this).hide();
            });
            $("#qrcode-section").hide();
            $("#qrcode-section").html("");
        });

        $(".couponitem-ctrl .use-coupon").each(function () {

            var _thisCtrl = $(this);
            _thisCtrl.click(function () {

                var _parentObj = $(this).parent().parent();
                var _useSection = _parentObj.find(".use-section");
                _useSection.css("top", $('body').scrollTop() + 50);
                _useSection.show();
                _productModelBg.show();

                _useSection.click(function () {
                    //$("#qrcode-section").hide();
                });

            });

        });

        //退款功能
        $(".return-policy-btn").each(function () {
            var _thisObj = $(this);
            _thisObj.click(function () {

                if (confirm("这么优惠的套餐，你确认退款吗？")) {
                    var _exid = _thisObj.data("exid");

                    var dic = {};
                    dic["exid"] = _exid;
                    dic["userid"] = pubuserid;

                    $.get('/Coupon/ProductCouponReturnPolicy', dic, function (result) {

                        console.log(result)

                        if (result.Success == 0) {
                            alert("退款申请已提交");
                            location.reload();
                        }
                        else {
                            alert(result.Message);
                        }

                    });
                }
            });
        });

        //取消预约功能
        $(".reserve-cancel").each(function () {
            var _thisObj = $(this);
            _thisObj.click(function () {

                if (confirm("取消预约后，当日名额将不再保留")) {
                    var _ebookid = _thisObj.data("bookid");

                    var dic = {};
                    dic["id"] = _ebookid;

                    $.get(_Config.APIUrl + '/api/Coupon/CancelBookInfo', dic, function (result) {

                        console.log(result)

                        if (result.RetCode === "1") {
                            alert("你的预约已取消成功");
                            location.reload();
                        }
                        else {
                            alert(result.Message);
                        }

                    });
                }
            });
        });
        
        $(".photo-ctrl").each(function () {

            var _thisUpload = $(this).find(".photo-upload");
            var _photoImg = $(this).find("img");
            var _uploadingImg = $(this).find(".uploadlodding");

            //重新上传照片功能
            _thisUpload.click(function () {

                console.log(_photoImg);
                _loadPhotoObj = _photoImg;
                _uploadObj = _thisUpload;
                _uploadLoadingObj = _uploadingImg;
                _exid = _thisUpload.data("exid");

                $("#previewImg").click();

            });

            //照片预览
            _photoImg.click(function () {

                var _oriSrc = $(this).data("oriimg");
                $(".preview-img-bg").fadeIn(200);
                $(".preview-img-alert").fadeIn(100);
                $(".preview-img-alert img").attr("src", _oriSrc);

            });
        });

        $(".preview-img-bg").click(function () {
            $(".preview-img-bg").hide();
            $(".preview-img-alert").hide();
        });
        $(".preview-img-alert").click(function () {
            $(".preview-img-bg").hide();
            $(".preview-img-alert").hide();
        });

        //大团购支付尾款
        $(".pay-stepgroup").each(function () {

            $(this).click(function () {

                var _depositskuid = $(this).data("depositskuid");
                var _tailskuid = $(this).data("tailskuid");
                var _tailprice = $(this).data("tailprice");
                var _corderid = $(this).data("corderid");
                var _bookpostion = $(this).data("bookpostion");
                var _paynum = 1;

                goCouponBook(_bookpostion, _tailskuid, _paynum, pubuserid, 0, _corderid);

            });
        });
    }
    else if (!pageTag) {

        /* default pagetag */

        //开启刷新机制（500毫秒检测一次）
        Global.Monito.Listener("coupon", "wallet2", 1, 500);
    }
});

var gourl = function(url) {
    location.href = url;
}

//去提交购买(如大团购产品的尾款支付)
function goCouponBook(bookpostion, skuid, paynum, userid, fromwxuid, coid) {

    switch (parseInt(bookpostion)) {
        case 0: {
            //不需要预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&coid={4}&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid, coid));
            break;
        }
        case 1: {
            //前置预约
            gourl("/Coupon/CouponReserve?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&exid={4}&coid={5}&prereserve=1&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid, 0, coid));
            break;
        }
        case 2: {
            //后置预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&coid={4}&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid, coid));
            break;
        }

    }
}

var showQrcode = function (exno) {
    
    $("#qrcode-section").fadeIn(200);

    var qrcodeContent = ("http://www.zmjiudian.com/Shop/Coupon?exno=" + exno);
    var qrcode = new QRCode('qrcode-section', {
        text: qrcodeContent,
        width: 256,
        height: 256,
        colorDark: '#000000',
        colorLight: '#ffffff',
        correctLevel: QRCode.CorrectLevel.H
    });
}

var hideQrcodeSection = function () {
    
    $("#qrcode-section").hide();
    $("#qrcode-section").html("");

}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    //5.8.1之前的版本，钱包里才会显示 房券和消费券，该版本以后，房券和消费券移至 订单
    if (!IsThanVer5_8_1) {

        $(".wallet-roomcoupon-menu").show();
        $(".wallet-productcoupon-menu").show();
    }
}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //init data
    _InitApp(userid, apptype, appvercode, appverno);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}

//图片上传预览    IE是用了滤镜
var _uploadObj = null;
var _uploadLoadingObj = null;
var _loadPhotoObj = null;
var _exid = 0;
function previewImage(file) {
    //var MAXWIDTH = 290;
    //var MAXHEIGHT = 290;
    //var div = document.getElementById('preview');
    if (file.files && file.files[0]) {
        //div.innerHTML = '<img id=imghead onclick=$("#previewImg").click()>';
        //var img = document.getElementById('imghead');
        //img.onload = function () {
        //    var rect = clacImgZoomParam(MAXWIDTH, MAXHEIGHT, img.offsetWidth, img.offsetHeight);
        //    img.width = rect.width;
        //    img.height = rect.height;
        //    ////                 img.style.marginLeft = rect.left+'px';
        //    //img.style.marginTop = rect.top + 'px';
        //}
        //var reader = new FileReader();
        //reader.onload = function (evt) { img.src = evt.target.result; }
        //reader.readAsDataURL(file.files[0]);
        var file = file.files[0];
        var contentSecret = 'whhotels';//getcontentSecret(8) || 
        var options = {
            'notify_url': 'http://upyun.com',
            'content-secret': contentSecret
        };
        var config = {
            bucket: 'whphoto',
            expiration: parseInt((new Date().getTime() + 3600000) / 1000),
            form_api_secret: 'Mbu7g+t64a0dWPfPpkzEUEiKJHc='
        };
        var instance = new Sand(config);
        instance.setOptions(options);
        var fileName = file.name;
        //var ext = '.' + fileName.split('.').pop();
        var newPicName = getPicPath2(0, 0, getcontentSecret(2));
        var PhotoSURL = newPicName;// + ext;
        var picPath = '/' + PhotoSURL;
        //上传图片
        instance.upload(picPath, file);

        //$(".uploadlodding").show();
        var cusPhotoUrl = "http://p1.zmjiudian.com/" + PhotoSURL + "_640x640";
        var cusPhotoUrl_s = "http://p1.zmjiudian.com/" + PhotoSURL + "_290x290";

        //隐藏上传按钮
        _uploadObj.hide();
        _uploadLoadingObj.show();

        //睡眠5秒   等待上传头像完成
        var _canupload = true;
        setTimeout(function () {
            _loadPhotoObj.attr("src", cusPhotoUrl_s);
            _loadPhotoObj.data("oriimg", cusPhotoUrl);
            _loadPhotoObj.unbind("error");
            _loadPhotoObj.error(function () {

                alert("照片上传失败，请换一张重试哦");
                _uploadLoadingObj.hide();
                _uploadObj.show();

                //恢复默认图
                var _oriimg2 = _loadPhotoObj.data("oriimg2");
                _loadPhotoObj.attr("src", _oriimg2.replace("_640x640", "_290x290"));
                _loadPhotoObj.data("oriimg", _oriimg2);
                _canupload = false;
            });
            _loadPhotoObj.unbind("load");
            _loadPhotoObj.load(function () {

                if (_canupload) {
                    _uploadLoadingObj.hide();

                    //更新照片
                    var _uploadPhotoDic = { exid: _exid, photoUrl: cusPhotoUrl };
                    console.log(_uploadPhotoDic);
                    $.get('/coupon/UpdateCoupnPhoto', _uploadPhotoDic, function (_update) {

                        console.log(_update);
                        if (_update.IsSuccess) {
                            alert("照片已上传");
                        }
                        else {
                            alert("照片上传失败，请重试");
                            _uploadObj.show();
                        }

                    });
                }
            });
            //setTimeout(function () { _uploadLoadingObj.hide(); }, 200);
            
        }, 5000);

        console.log("上传图片");
        console.log(cusPhotoUrl);
        console.log(cusPhotoUrl_s);
    }
}
function clacImgZoomParam(maxWidth, maxHeight, width, height) {
    var param = { top: 0, left: 0, width: width, height: height };
    if (width > maxWidth || height > maxHeight) {
        rateWidth = width / maxWidth;
        rateHeight = height / maxHeight;

        if (rateWidth > rateHeight) {
            param.width = maxWidth;
            param.height = Math.round(height / rateWidth);
        } else {
            param.width = Math.round(width / rateHeight);
            param.height = maxHeight;
        }
    }
    param.left = Math.round((maxWidth - param.width) / 2);
    param.top = Math.round((maxHeight - param.height) / 2);
    return param;
}
function getPicPath2(CommentID, picFlowNum, catID) {
    var datetime = new Date();
    var date = datetime.getDate();
    var hour = datetime.getHours();
    var minute = datetime.getMinutes();
    var second = datetime.getSeconds();
    //var imgLength = $("#fileList").find("img").length;
    //var picFlowNum = imgLength;
    return (CommentID == 0 ? "" : CommentID) + getTimeCount2Char(datetime.getYear()) + getTimeCount2Char(date) + getTimeCount2Char(hour) + getTimeCount2Char(minute) + getTimeCount2Char(second) + picFlowNum + catID;
}
function getTimeCount2Char(time) {
    if (time >= 60) {
        return time;
    }
    var dic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    return dic.charAt(time);
}
function getcontentSecret(num) {
    var dic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var contentSecret = [];
    for (var i = 1; i <= num; i++) {
        contentSecret.push(dic.charAt(Math.floor(Math.random() * 61)));
    }
    return contentSecret.join('');
}
