var wwidth = $(window).width();
var isapp = $("#isapp").val() == "1";
var isvip = $("#isvip").val() == "1";
var customerType = $("#customerType").val();
var stype = $("#stype").val();
var pubskuid = $("#skuid").val();
var pubuserid = $("#userid").val();
var skuPrice = $("#skuPrice").val();
var skuVipPrice = $("#skuVipPrice").val();

var _name = $("#_name").val();

//SKU加载次数（每切换一次SKU累加一次）
var loadNum = 0;
var loadNumList = [0];

//是否显示更多套餐
var showMoreSku = false;

$(document).ready(function () {
    
    var _Config = new Config();

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //检测登录并自动登录
    if (!isapp && pub_userid == "0") {
        _loginModular.verify.autoLogin(loginCheckFun);
    }

    var productDetailData = new Vue({
        el: "#product-detail",
        data: {
            "productDetail": {
                "priceInfo": {}
            }
        }
    })

    ////头图滑动
    ////动态设置首页smallbanner的具体宽度
    //if ($('#product-img-list2')) {
    //    $(".product-img-item").css("width", wwidth);
    //    $('#product-img-list2').swiper({
    //        slidesPerView: 'auto',
    //        pagination: '.pagination2',
    //        paginationHide: false,
    //        //loop: true,
    //        offsetPxBefore: 0,
    //        offsetPxAfter: 0
    //    })
    //}

    //加载产品详情
    var loadProductDetail = function (skuid) { 
        var _detailDic = { skuid: skuid };
        $.get(_Config.APIUrl + '/api/coupon/GetSKUCouponActivityDetail', _detailDic, function (data) {

            if (data) {

                //console.log(data)

                data.SKUID = skuid;
                data.UserId = pubuserid;
                data.CustomerType = customerType;
                data.SType = stype;
                data.IsVip = isvip;
                data.IsApp = isapp;
                data.priceInfo = {};
                data.productDefImg = "http://whfront.b0.upaiyun.com/app/img/pic-def-16x9.png";
                data.loadNum = loadNum;
                data.loadNumList = loadNumList;

                data.activity.OriMinBuyNum = data.activity.MinBuyNum;

                //电话/地址显示
                if (data.activity.DicProperties) {
                    data.activity.telObj = {};
                    data.activity.otherObjList = [];
                    for (var _key in data.activity.DicProperties) {
                        var _val = data.activity.DicProperties[_key];
                        var _lab = _key.replace(":", "").replace("：", "");
                        if (_lab.indexOf("电话") >= 0) {
                            var _tel = _val;
                            var _telex = "";
                            if (_val.indexOf("转")>=0) {
                                _tel = _val.split('转')[0];
                                _telex = "转" + _val.split('转')[1];
                            }

                            data.activity.telObj = {
                                "lab": _lab,
                                "tel": _tel,
                                "telex": _telex
                            }
                        }
                        else {
                            data.activity.otherObjList.push({
                                "lab": _lab,
                                "val": _val
                            })
                        }
                    }
                }

                //当前选择项置顶
                var _thisSelectedSku = {};
                if (data.SKUInfo && data.SKUInfo.SKUList && data.SKUInfo.SKUList.length > 0) {
                    for (var _itemNum = 0; _itemNum < data.SKUInfo.SKUList.length; _itemNum++) {
                        var _itemSku = data.SKUInfo.SKUList[_itemNum];
                        if (_itemSku.ID == data.SKUInfo.SKU.ID) {
                            //console.log(_itemSku)
                            _thisSelectedSku = _itemSku;
                            _thisSelectedSku._index = _itemNum;
                            break;
                        }
                    }

                    //是否显示更多套餐
                    data.showMoreSku = true;
                    data.showMoreBtn = false;
                    if (loadNum == 0 && data.SKUInfo.SKUList.length > 2 && _thisSelectedSku._index <= 2) {
                        data.showMoreSku = false;
                        data.showMoreBtn = true;
                    }
                }
                data.selectedSKU = _thisSelectedSku;

                //console.log("load detail")
                //console.log(data);

                //初始价格
                if (loadNum > 0 && productDetailData.productDetail && productDetailData.productDetail.priceInfo) {
                    data.priceInfo = productDetailData.productDetail.priceInfo;
                }

                //初始VIP专享状态（兼容老VIP和新VIP专享）
                if (!data.activity.IsVipExclusive && data.SKUInfo && data.SKUInfo.SKU && data.SKUInfo.SKU.TagsList && data.SKUInfo.SKU.TagsList.length > 0) {
                    for (var _tagNum = 0; _tagNum < data.SKUInfo.SKU.TagsList.length; _tagNum++) {
                        var _tagItem = data.SKUInfo.SKU.TagsList[_tagNum];
                        if (_tagItem.TagID == 1) {
                            data.activity.IsVipExclusive = true;
                            break;
                        }
                    }
                }

                productDetailData.productDetail = data;

                setTimeout(function () {

                    //绑定事件
                    bindEvent();

                    //页面初始进来时计算价格信息
                    setxiaoji();

                }, 300);
            }

        });

    }

    //绑定事件
    var bindEvent = function () {

        //动态加载头图
        if (productDetailData.productDetail.activity.PicList) {

            if (productDetailData.productDetail.activity.PicList.length > 1) {

                //头图滑动
                //动态设置首页smallbanner的具体宽度
                if ($('#product-img-list-' + loadNum)) {

                    //console.log("有图")

                    $(".product-img-item").css("width", wwidth);
                    $('#product-img-list-' + loadNum).swiper({
                        slidesPerView: 'auto',
                        pagination: '.pagination-' + loadNum,
                        paginationHide: false,
                        loop: true,
                        offsetPxBefore: 0,
                        offsetPxAfter: 0
                    })
                }

                //显示产品图
                if ($(".def-photo")) $(".def-photo").hide();
                if ($(".product-photo")) $(".product-photo").fadeIn(500);

                var _productImgNum = 0;
                $(".product-img").each(function () {

                    var _thisImg = $(this);

                    setImgOriSrc(_thisImg);

                    //第一张图加载好以后显示
                    if (_productImgNum == 0) {
                        _thisImg.load(function () {

                            
                        });
                    }

                    //设置产品图的点击预览功能（app内）
                    if (IsThanVer5_1) {
                        _thisImg.click(previewImage);
                    }

                    _productImgNum++;
                });
            }
            else {

                var _singleImg = $(".product-single-img");
                //setImgOriSrc(_singleImg);
                //_singleImg.load(function () {

                //    //显示产品图
                //    if ($(".def-photo")) $(".def-photo").hide();
                //    if ($(".product-photo")) $(".product-photo").fadeIn(500);

                //});
                //显示产品图
                if ($(".def-photo")) $(".def-photo").hide();
                if ($(".product-photo")) $(".product-photo").fadeIn(500);
            }
        }

        //手动判断是否显示购买须知
        if (!productDetailData.productDetail.activity.NoticeList || productDetailData.productDetail.activity.NoticeList.length <= 0) {
            $(".shopread").hide();
        }
        else {
            $(".shopread").show();
        }

        //查看更多套餐
        $(".show-more").unbind("click");
        $(".show-more").click(function () {
            productDetailData.productDetail.showMoreSku = true;
            productDetailData.productDetail.showMoreBtn = false;
            
            setTimeout(changeSku, 200);
        });

        $(".submit").unbind("click");
        $(".submit").click(submitFun);
    }

    loadProductDetail(pubskuid);

    //减 数量
    $(".btn0").click(function () {

        var cansell = $("#cansell").val() == "1";
        if (!cansell) return;

        var num = productDetailData.productDetail.activity.MinBuyNum;
        if (num == "" || isNaN(num)) {
            num = 1;
        }
        num = parseInt(num);
        if (num > 1) num--;
        productDetailData.productDetail.activity.MinBuyNum = num;

        //得出小计
        setxiaoji();

        //验证购买数量
        checkBuyNum();
    });

    //加 数量
    $(".btn1").click(function () {

        var cansell = $("#cansell").val() == "1";
        if (!cansell) return;

        var num = productDetailData.productDetail.activity.MinBuyNum;
        if (num == "" || isNaN(num)) {
            num = 0;
        }
        num = parseInt(num);
        num++;
        productDetailData.productDetail.activity.MinBuyNum = num;

        //得出小计
        setxiaoji();

        //验证购买数量
        checkBuyNum();
    });

    //数量变更
    $(".sellnum").change(function () {

        var cansell = $("#cansell").val() == "1";
        if (!cansell) return;

        var num = $(this).val();
        if (num == "" || isNaN(num) || parseInt(num) < 1) {
            $(this).val(1);
        }

        //得出小计
        setxiaoji();

        //验证购买数量
        checkBuyNum();
    });

    //得出小计
    function setxiaoji()
    {
        var sellPrice = parseInt($("#sellPrice").val());
        var num = parseInt(productDetailData.productDetail.activity.MinBuyNum);
        var sum = sellPrice * num;

        var dic = {};
        dic["skuid"] = productDetailData.productDetail.SKUID;
        dic["buynum"] = num;
        dic["userid"] = pubuserid;
        dic["stype"] = stype;

        $.get('/Coupon/CheckProductPromotionForCoupon', dic, function (result) {

            result["IsVip"] = isvip;
            result["CustomerType"] = customerType;
            result["PromotionRuleEntity"] = null;
            result["FirstVipPrice"] = 0;
            result["FirstPrice"] = 0;
            result["FirstPromotionVipPrice"] = 0;
            result["FirstPromotionPrice"] = 0;
            result["SumPrice"] = 0;
            result["SumVipPrice"] = 0;

            if (result && result.SellPriceItemList && result.SellPriceItemList.length > 0 && result.SellVIPPriceItemList && result.SellVIPPriceItemList.length > 0) {

                //默认价格
                result.FirstVipPrice = parseInt($("#skuVipPrice").val());
                result.FirstPrice = parseInt($("#skuPrice").val());
                result.FirstPromotionVipPrice = 0;
                result.FirstPromotionPrice = 0;
                result.SumVipPrice = result.SellVipPrice;
                result.SumPrice = result.SellPrice;

                //如果包含优惠价格
                if (result.PromotionRuleList && result.PromotionRuleList.length > 0 && result.PromotionRuleList[0]) {

                    var _promotionRuleEntity = result.PromotionRuleList[0];

                    //享有优惠
                    if (_promotionRuleEntity.Valid) {

                        result.SumVipPrice = result.PromotionVipPrice;
                        result.SumPrice = result.PromotionPrice;

                        result.FirstPromotionVipPrice = result.PromotionVIPPriceItemList[0].Price;
                        result.FirstPromotionPrice = result.PromotionPriceItemList[0].Price;

                        result.PromotionRuleEntity = _promotionRuleEntity;

                    } else {

                        //不享有优惠，但排除一些指定情况（如新VIP爆款，普通会员也要看到优惠信息）
                        if (_promotionRuleEntity.PromotionUseState == 7 && _promotionRuleEntity.PrivID.indexOf("2010") >= 0 && !isvip) {
                            
                            result.FirstPromotionVipPrice = result.PromotionVIPPriceItemList[0].Price;
                            result.FirstPromotionPrice = result.PromotionPriceItemList[0].Price;

                            result.PromotionRuleEntity = _promotionRuleEntity;

                            //顶部价格banner
                            if (isvip) {
                                $(".promotion-price-banner .p .val").html(result.FirstPromotionVipPrice);
                            }
                            else {
                                $(".promotion-price-banner .p .val").html(result.FirstPromotionPrice);
                            }
                            $(".promotion-price-banner .tag-txt").html(_promotionRuleEntity.TagName);
                            $(".promotion-price-banner").click(goBuyVip);
                            $(".promotion-price-banner").show();
                        }

                    }
                }

                //console.log(result)

                //得出最后结算价
                var _sumPrice = isvip ? result.SumVipPrice : result.SumPrice;

                //购买按钮等结算价格显示
                $(".submit").html("￥" + _sumPrice + "&nbsp;&nbsp;立即购买");
                $(".xiaoji").data("sum", _sumPrice);
                $(".xiaoji .right .price").text(_sumPrice);
                
                //顶部价格区域显示
                productDetailData.productDetail.priceInfo = result;

                //display
                $("#price-detail").show();
            }
            else {
                $(".submit").html("￥" + sum + "&nbsp;&nbsp;立即购买");
                $(".xiaoji").data("sum", sum);
                $(".xiaoji .right .price").text(sum);
            }
        });
    }

    //验证购买数量
    function checkBuyNum() {
        
        var dic = {};
        dic["id"] = $("#aid").val();
        var num = productDetailData.productDetail.activity.MinBuyNum; if (num == "" || isNaN(num) || parseInt(num) < 1) num = 1;
        dic["buynum"] = num;
        dic["userid"] = $("#userid").val();

        //首先验证最小购买数量
        var minbuy = productDetailData.productDetail.activity.OriMinBuyNum;
        var minbuyMsg = "至少" + minbuy + "份起售";
        //console.log(num)
        //console.log(minbuy)
        if (parseInt(num) < minbuy) {
            console.log(minbuyMsg);
            showTip(minbuyMsg);
            productDetailData.productDetail.activity.MinBuyNum = minbuy;
            setxiaoji();
            return;
        }
    }

    function showTip(mes)
    {
        //console.log(mes)
        $(".pubAlertTip .tipinfo").html(mes);
        $(".pubAlertTip").fadeIn(500);
        setTimeout(function () {
            $(".pubAlertTip").fadeOut(300);
        }, 3000);
    }

    var submitFun = function () {

        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
        if (pub_userid == "0") {
            //app环境下，如果没有登录则弹出登录
            if (isapp) {
                location.href = loginapphref;
                return;
            }
            else {
                _loginModular.show();
            }
        }
        else {

            gosubmit($("#userid").val());
        }

    }

    //购买
    $(".submit").unbind("click");
    $(".submit").click(submitFun);

    //让日期选择列表支持横向滑动
    var dateListScroll = new IScroll('#dateList', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });

});

//submit
var gosubmit = function (userid) {

    var subdic = {};
    subdic["aid"] = $("#aid").val();
    subdic["atype"] = $("#atype").val();
    subdic["skuid"] = $("#thisskuid").val();
    var sellnum = $(".sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
    subdic["paynum"] = sellnum;
    subdic["userid"] = userid;
    subdic["stype"] = stype;

    $.get('/Coupon/SubmitConponForProduct', subdic, function (content) {
        var msg = content.Message;
        var suc = content.Success;
        var url = content.Url;

        switch (suc) {
            case "0":
                {
                    location = url;
                    break;
                }
            case "1":
                {
                    alert(msg);
                    break;
                }
            case "2":
                {
                    _Modal.show({
                        title: '',
                        content: msg,
                        confirmText: '成为VIP',
                        confirm: function () {
                            goBuyVip();
                            _Modal.hide();
                        },
                        showCancel: true,
                        cancel: function () {
                            _Modal.hide();
                        }
                    });
                    break;
                }
        }
    });
};

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

function gourl(url) {
    location.href = url;
}

function loginCallback(userid) {
    gosubmit(userid);
    //location.reload();
    //location.replace(location.pathname + "?userid=" + userid);
}

var timerTags = $(".timer-tag");
var timeDic = [];
if (timerTags) {
    for (var i = 0; i < timerTags.length; i++) {

        timeDic[i] = {
            timerEntity: null,
            nowTime: null,
            endDate: null,
            closeDate: null,
            endTimerState: true,
            closeTimerState: false,
            initNowtime: function () {
                this.nowTime = new Date(
                parseInt(this.timerEntity.data("year0"))
                , parseInt(this.timerEntity.data("month0"))
                , parseInt(this.timerEntity.data("day0"))
                , parseInt(this.timerEntity.data("hour0"))
                , parseInt(this.timerEntity.data("minute0"))
                , parseInt(this.timerEntity.data("second0"))
                    ).getTime();
            },
            initEndtime: function () {
                this.endDate = new Date(
                parseInt(this.timerEntity.data("year1"))
                , parseInt(this.timerEntity.data("month1"))
                , parseInt(this.timerEntity.data("day1"))
                , parseInt(this.timerEntity.data("hour1"))
                , parseInt(this.timerEntity.data("minute1"))
                , parseInt(this.timerEntity.data("second1"))
                    ).getTime();
            },
            initClosetime: function () {
                this.closeDate = new Date(
                parseInt(this.timerEntity.data("year2"))
                , parseInt(this.timerEntity.data("month2"))
                , parseInt(this.timerEntity.data("day2"))
                , parseInt(this.timerEntity.data("hour2"))
                , parseInt(this.timerEntity.data("minute2"))
                , parseInt(this.timerEntity.data("second2"))
                    ).getTime();
            },
            init: function () {
                this.initNowtime();
                this.initEndtime();
                this.initClosetime();
            },
            timerAction: function () {
                if (this.endTimerState) {
                    var t = this.endDate - this.nowTime;
                    var d = Math.floor(t / (1000 * 60 * 60 * 24));
                    var h = Math.floor(t / 1000 / 60 / 60 % 24);// + (d * 24);
                    var m = Math.floor(t / 1000 / 60 % 60);
                    var s = Math.floor(t / 1000 % 60);

                    var timehtml =
                        d <= 0 ? (
                        h <= 0
                        ? "距开抢 <time>00</time>:<time>" + (m < 10 ? "0" + m : m) + "</time>:<time>" + (s < 10 ? "0" + s : s) + "</time>"
                        : "距开抢 <time>" + (h < 10 ? "0" + h : h) + "</time>:<time>" + (m < 10 ? "0" + m : m) + "</time>:<time>" + (s < 10 ? "0" + s : s) + "</time>"
                        ) : "距开抢" + d + "天";


                    this.timerEntity.html(timehtml);
                    //$("#timer-tag-0").html(timehtml);

                    try {

                        if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                            this.stopEndAction();
                        }

                    } catch (e) { }

                    this.nowTime = this.nowTime + 1000;
                }
            },
            timerCloseAction: function () {
                if (this.closeTimerState) {
                    var t = this.closeDate - this.nowTime;
                    var d = Math.floor(t / (1000 * 60 * 60 * 24));
                    var h = Math.floor(t / 1000 / 60 / 60 % 24);// + (d * 24);
                    var m = Math.floor(t / 1000 / 60 % 60);
                    var s = Math.floor(t / 1000 % 60);

                    var timehtml = d <= 0 ?
                        (h <= 0 ? "剩余<time>" + (m < 10 ? "0" + m : m) + "</time>分钟<time>" + (s < 10 ? "0" + s : s) + "</time>秒"
                        : "剩余<time>" + (h < 10 ? "0" + h : h) + "</time>小时<time>" + (m < 10 ? "0" + m : m) + "</time>分钟")
                        : "剩余<time>" + (d < 10 ? "0" + d : d) + "</time>天<time>" + (h < 10 ? "0" + h : h) + "</time>小时<time>" + (m < 10 ? "0" + m : m) + "</time>分钟";

                    this.timerEntity.html(timehtml);
                    //$("#timer-tag-0").html(timehtml);

                    try {

                        if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                            this.stopCloseAction();
                        }

                    } catch (e) { }

                    this.nowTime = this.nowTime + 1000;
                }
            },
            stopEndAction: function () {
                this.endTimerState = false;
                this.closeTimerState = true;
                this.timerEntity.html("进行中");

                $("#cansell").val("1");
                try {
                    $(".submit").css("display", "block");
                    $(".submit0").css("display", "none");
                } catch (e) {

                }
            },
            stopCloseAction: function () {
                this.closeTimerState = false;
                this.timerEntity.html("已结束");

                $("#cansell").val("0");
                try {
                    $(".submit").css("display", "none");
                    $(".submit0").css("display", "block");
                } catch (e) {

                }
            }
        };

        //build
        timeDic[i].timerEntity = $(timerTags[i]);

        //init
        timeDic[i].init();

        //start
        timeDic[i].timerAction();
        setInterval("gotime(timeDic[" + i + "])", 1000);
    }
}

function gotime(timeObj) {
    timeObj.timerAction();
    timeObj.timerCloseAction();
}

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': $("#_name").val(), 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?userid=" + pubuserid + "&_isoneoff=1&_newpage=1";
}

//照片预览功能
var previewImage = function () {

    var _thisImg = $(this);
    var _param = { index: 0, urls: [] };
    _param.index = _thisImg.data("num");

    //get urls
    var _allImgs = $(".product-img");
    _allImgs.each(function () {
        var _oriSrc = $(this).data("showsrc").replace("_640x426", "_jupiter");
        _param.urls.push(_oriSrc);
    });

    zmjd.previewImage(_param);
}
var previewImageSuccess = function () {
    console.log("预览成功")
}
var previewImageFail = function () {
    console.log("预览失败")
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    

}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //alert(apptype)

    //init data
    _InitApp(userid, apptype, appvercode, appverno);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}