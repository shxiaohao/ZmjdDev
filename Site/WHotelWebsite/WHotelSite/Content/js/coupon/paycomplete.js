var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userid = $("#userid").val();
var orderId = $("#orderId").val();
var categoryId = $("#categoryId").val();
var groupId = parseInt($("#groupId").val());
var thisSkuid = $("#thisSkuid").val()
var totalPrice = parseFloat($("#totalPrice").val());
var canShareRedCoupon = $("#canShareRedCoupon").val() == "1";
var appShareLink = $("#appShareLink").val();
var shareCallBack = $("#shareCallBack").val();
var isRetail = $("#isRetail").val() == "1";
var retailAmount = $("#retailAmount").val();

var isLikeGroup = $("#isLikeGroup").val() == "1";
var groupProductImg = $("#groupProductImg").val();
var groupSkuId = $("#groupSkuId").val();
var groupAid = $("#groupAid").val();
var groupCount = $("#groupCount").val();
var weixinAcountId = $("#weixinAcountId").val();

var _Config = new Config();

$(document).ready(function () {
    
    try {

        //【数据统计】统计券购买完成记录
        var _category = "券购买完成";
        var _action = "访问";

        //查询购买该产品时的缓存来源
        var _sku_sourcekey = Store.Get("sku_sourcekey");
        var _label = (_sku_sourcekey ? _sku_sourcekey + "+" : "") + orderId + "+" + thisSkuid;

        var _value = 1;
        var _nodeid = "";
        _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

        _statistic.push("券购买完成", "访问", orderId + "+" + thisSkuid, _sku_sourcekey, "");

        //var _pvUrl = "/coupon/paycomplete?orderid={0}&skuid={1}&_sku_sourcekey={2}".format(orderId, thisSkuid, _sku_sourcekey);
        ////console.log(_pvUrl)
        //_czc.push(﻿["_trackPageview", _pvUrl]);

    } catch (e) {

    }

    //延时加载活动说明
    setTimeout(function () {

        //分享提示图片动态加载
        var shareTipImg = $(".group-share-tip img");
        setImgOriSrc(shareTipImg);

    }, 100);

    //邀请好友
    $(".group-share").click(function () {

        if (inWeixin) {
            $(".group-share-tip").show();
        }
        else if (isapp) {
            gourl(appShareLink);
        }

    });

    //右上角分享提示点击事件
    $(".group-share-tip").click(function () {
        $(this).hide();
    });

    //稍后预约事件
    $(".after-exchange").click(function () {

        $(this).hide();
        $(".after-exchange-panel").show();
        $(".foot-section").hide();
    });

    //双11代金券购买完成
    if (categoryId == "25") {

        if (inWeixin || isapp) {
            _Modal.show({
                title: '邀请奖励',
                content: "每成功邀请一位朋友购买代金券<br />你将额外获得10元代金券奖励<br />可用于平台所有产品抵现，多邀多得",
                confirmText: '邀请好友',
                confirm: function () {

                    if (inWeixin) {
                        $(".group-share-tip").show();
                    }
                    else if (isapp) {
                        gourl(appShareLink);
                    }

                    _Modal.hide();
                },
                showCancel: false
            });
        }
    }
    else {

        if (inWeixin) {

            //非团购 & 非积分 & 非免费领取 产品，购买完成后弹出的红包分享
            if (canShareRedCoupon) {

                //红包分享事件
                var shareRedCoupon = function () {

                    if (inWeixin) {

                        //if (shareCallBack) {
                        //    $.get(shareCallBack, {}, function (_data) {

                        //        alert("OK");
                        //    });
                        //}

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
            //全员分销产品，购买成功提示分享
            else if (isRetail) {

                var _html = "<center><img style='width:4em;' src='http://whfront.b0.upaiyun.com/app/img/coupon/product/share-redpack-icon.png' alt=''></center>";
                _html += "<center style='font-size:1.1rem;margin:0.3em 0 0.3em 0;'><b>分享有礼</b></center>";
                _html += "<center>好友购买成功即可<br />获得<b style='margin:0 0.1em 0 0.1em;'>¥" + retailAmount + "</b>微信红包</center>";

                _Modal.show({
                    title: '',
                    content: _html,
                    showClose: true,
                    confirmText: '分享给好友',
                    confirm: function () {
                        _Modal.hide();

                        $(".group-share-tip").show();
                    },
                    showCancel: false
                });

                $("._modal-section").css("top", "22%");
            }

        }
    }

    //拼团助力活动相关操作
    if (isLikeGroup) {

        //获取当前发起助力的专属二维码
        //GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID
        var _sceneStr = "GROUPTREE_{0}_{1}_{2}_{3}".format(groupSkuId, groupAid, groupId, userid);

        var _dic = {
            weixinAcount: weixinAcountId, //周末酒店服务号 浩颐
            expires: 2592000,
            actionName: "QR_STR_SCENE",
            sceneId: 0,
            sceneStr: _sceneStr
        };

        console.log(_dic);

        $.get(_Config.APIUrl + '/api/WeixinApi/CreateAccountQrcode', _dic, function (_data) {

            console.log(_data);

            if (_data && _data.indexOf("ticket") >= 0) {

                var _dataObj = JSON.parse(_data);
                console.log(_dataObj);

                var _src = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}".format(_dataObj.ticket);

                //转换成zmjd域名下的地址
                _src = "http://api.zmjiudian.com/api/photo/GetZmjdImgByUrl?url={0}".format(_src);
                //console.log(_src);

                setTimeout(function () {

                    //产品图片（ios微信环境下，如果图片之间加载过有缓存，不会执行load事件，所以这里统一加上时间戳，MMP 20180109 haoy）
                    var timestamp = Date.parse(new Date());
                    $(".mine-poster .p-img img").attr("src", groupProductImg + "?v=" + timestamp);
                    $(".mine-poster .p-img img").load(function () {

                        productImgLoaded = true;
                        loadPoster()
                    });

                    //二维码图片
                    var _loadQrcodeState_load = true;
                    var _loadQrcodeState_auto = true;
                    //$(".mine-poster .p-info .qrcode-section img").attr("src", "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-159x159.png");
                    $(".mine-poster .p-info .qrcode-section img").attr("src", _src);
                    $(".mine-poster .p-info .qrcode-section img").load(function () {

                        _loadQrcodeState_auto = false;
                        if (_loadQrcodeState_load) {
                            console.log("qrcode load")
                            qrcodeImgLoaded = true;
                            loadPoster()
                        }
                    });

                    //300毫秒后自动加载海报（有些环境下qrcode的图片load事件不会触发 2018.07.12 haoy）
                    setTimeout(function () {
                        _loadQrcodeState_load = false;
                        if (_loadQrcodeState_auto) {
                            console.log("qrcode auto")
                            qrcodeImgLoaded = true;
                            loadPoster();
                        }
                    }, 3000);

                }, 0);

            }
            else {

                console.log("专属二维码生成失败");
            }

        });

        //加载并显示海报
        var productImgLoaded = false;
        var qrcodeImgLoaded = false;
        var footLogoImgLoaded = true;
        var loadPoster = function () {

            if (productImgLoaded && qrcodeImgLoaded && footLogoImgLoaded) {

                $(".poster-bg").show();
                $(".mine-poster").show();

                setTimeout(function () {

                    html2canvas($(".mine-poster")[0], { useCORS: true }).then(function (canvas) {

                        //console.log(canvas.toDataURL());

                        $("#showImg").attr("src", canvas.toDataURL());
                        $("#showImg").load(function () {

                            setTimeout(function () {

                                //$("#showPosterSection").fadeIn(500);
                                $("#showPosterSection").slideDown();
                                $(".poster-tip").fadeIn(500);
                                $(".mine-poster").hide();

                                //$(".poster-bg").click(function () {

                                //    $("#showPosterSection").fadeOut(200);
                                //    $(".poster-tip").hide();
                                //    $(".poster-bg").fadeOut(100);
                                //});

                                var fllowWxAcountTip = "";
                                switch (weixinAcountId) {
                                    case "7": {
                                        fllowWxAcountTip = "<br /><br />*关注周末酒店服务号查看助力进度";
                                        break;
                                    }
                                    case "8": {
                                        fllowWxAcountTip = "<br /><br />*关注遛娃指南苏州服务号查看助力进度";
                                        break;
                                    }
                                    case "11": {
                                        fllowWxAcountTip = "<br /><br />*关注遛娃指南服务号（微信号：liuwa616）查看助力进度";
                                        break;
                                    }
                                    case "13": {
                                        fllowWxAcountTip = "<br /><br />*关注遛娃指南南京服务号查看助力进度";
                                        break;
                                    }
                                    case "14": {
                                        fllowWxAcountTip = "<br /><br />*关注遛娃指南无锡服务号查看助力进度";
                                        break;
                                    }
                                    case "15": {
                                        fllowWxAcountTip = "<br /><br />*关注遛娃指南广州服务号查看助力进度";
                                        break;
                                    }

                                }

                                //弹出提示
                                _Modal.show({
                                    title: '最后一步',
                                    content: "长按保存海报，并邀请{0}位好友帮忙助力！{1}".format((parseInt(groupCount) - 1), fllowWxAcountTip),
                                    showClose: true,
                                    confirmText: '我知道了',
                                    confirm: function () {

                                        _Modal.hide();
                                    },
                                    showCancel: false
                                });

                            }, 200);
                        });

                    });

                }, 100);
            }
        }
    }
});

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        var defsrc = imgObj.attr("src"); //console.log(orisrc)
        imgObj.attr("src", orisrc);
        //imgObj.data("orisrc", "");
        imgObj.error(function () {
            imgObj.attr("src", defsrc);
        });
    }
};

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    //如果是app环境，在VIP购买成功后向app标记用户信息产品了变更
    zmjd.userinfoChanged();

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