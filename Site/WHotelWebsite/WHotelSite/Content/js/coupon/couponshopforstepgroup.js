var wwidth = $(window).width();
var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var isvip = $("#isvip").val() == "1";
var showShop = $("#showShop").val() == "1";
var customerType = $("#customerType").val();
var fromwxuid = $("#fromwxuid").val();
var stype = $("#stype").val();
var pubskuid = $("#skuid").val();
var pubuserid = $("#userid").val();
var skuPrice = $("#skuPrice").val();
var skuVipPrice = $("#skuVipPrice").val();
var shareNativeLink = $("#shareNativeLink").val();
var pcid = parseInt($("#pcid").val());

var year0 = $("#year0").val();
var month0 = $("#month0").val();
var day0 = $("#day0").val();
var hour0 = $("#hour0").val();
var minute0 = $("#minute0").val();
var second0 = $("#second0").val();

var _shareTitle = $("#shareTitle").val();
var _shareDesc = $("#shareDesc").val();
var _shareLink = $("#shareLink").val();
var _shareImgUrl = $("#shareImgUrl").val();
var _isNewShare = $("#isNewShare").val() == "1";

var _name = $("#_name").val();

//SKU加载次数（每切换一次SKU累加一次）
var loadNum = 0;
var loadNumList = [0];

//是否显示更多套餐
var showMoreSku = false;

//当前选择SKU产品的图片list
var productPicList = [];

//产品detail model
var productDetailData = {};

var dingjinSku = {};
var groupSku = {};

var _Config = new Config();

$(document).ready(function () {

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    //如果是分销产品，并显示了店铺信息，则定义只显示验证手机登录(这种情况下，尽量不突出zmjd账号登录的概念)
    if (showShop) {
        _loginModular._onlyPhoneLogin = true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //检测登录并自动登录
    if (!isapp && pubuserid == "0") {
        _loginModular.verify.autoLogin(loginCheckFun);
    }

    productDetailData = new Vue({
        el: "#product-detail",
        data: {
            "productDetail": {
                "priceInfo": {},
                "hotelPackageInfo": {}
            }
        }
    })

    //_Config.APIUrl = "http://192.168.1.114:8000";
    //_Config.APIUrl = "http://api.zmjd100.com";

    //加载产品详情
    var loadProductDetail = function (skuid) {

        var _detailDic = { skuid: skuid, userid: pubuserid };
        $.get(_Config.APIUrl + '/api/coupon/GetSKUCouponActivityDetail', _detailDic, function (data) {

            clearInterval(timerIntObj);

            if (data) {

                console.log(data)

                data.SKUID = skuid;
                data.UserId = pubuserid;
                data.PCID = pcid;
                data.CustomerType = customerType;
                data.SType = stype;
                data.IsVip = isvip;
                data.IsApp = isapp;
                data.priceInfo = {};
                data.hotelPackageInfo = {};
                data.productDefImg = "http://whfront.b0.upaiyun.com/app/img/pic-def-16x9.png";
                data.loadNum = loadNum;
                data.loadNumList = loadNumList;

                //当前产品的支付类型（0费用支付 1积分支付）
                data.PayType = data.SKUInfo.Category.PayType;

                data.activity.OriMinBuyNum = data.activity.MinBuyNum;

                //电话/地址显示
                if (data.activity.DicProperties) {
                    data.activity.telObj = [];
                    data.activity.otherObjList = [];
                    for (var _key in data.activity.DicProperties) {
                        var _val = data.activity.DicProperties[_key];
                        var _lab = _key.replace(":", "").replace("：", "");
                        if (_lab.indexOf("电话") >= 0) {
                            var _tel = _val;
                            var _telex = "";
                            if (_val.indexOf("转") >= 0) {
                                _tel = _val.split('转')[0];
                                _telex = "转" + _val.split('转')[1];
                            }

                            data.activity.otherObjList.push({
                                "lab": _lab,
                                "tel": _tel,
                                "telex": _telex,
                                "istel": 1,
                            });
                        }
                        else {
                            data.activity.otherObjList.push({
                                "lab": _lab,
                                "val": _val,
                                "telex": "",
                                "istel": 0
                            })
                        }
                    }
                }

                //阶梯团逻辑预处理
                if (data.SKUInfo.StepGroup && data.SKUInfo.StepGroup.ID) {

                    if (data.SKUInfo.StepGroup.GradientPriceList) {

                        //初始第一个阶梯0
                        var _newList = [];
                        _newList.push({
                            "GroupCount": 0,
                            "Price": -1,
                            "IsHigh": true,
                            "IsThis": false
                        });

                        for (var _gnum = 0; _gnum < data.SKUInfo.StepGroup.GradientPriceList.length; _gnum++) {
                            var _groupItem = data.SKUInfo.StepGroup.GradientPriceList[_gnum];
                            _groupItem["IsHigh"] = false;
                            _groupItem["IsThis"] = false;

                            //是否到达或超过当前阶段
                            if (data.activity.SellNum >= _groupItem.GroupCount) {
                                _groupItem.IsHigh = true;
                            }

                            //是否为当前阶段
                            if (data.SKUInfo.StepGroup.CurrentPrice > 0 && data.SKUInfo.StepGroup.CurrentPrice == _groupItem.Price) {
                                _groupItem.IsThis = true;
                            }

                            _newList.push(_groupItem);
                        }
                        data.SKUInfo.StepGroup.GradientPriceList = _newList;

                        //不同阶梯数，格式不同
                        data.SKUInfo.StepGroup["StepItemClass"] = ("_s_item_" + data.SKUInfo.StepGroup.GradientPriceList.length)
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
                    if (loadNum == 0 && data.SKUInfo.SKUList.length > 3 && _thisSelectedSku._index < 3) {
                        data.showMoreSku = false;
                        data.showMoreBtn = true;
                    }
                }
                data.selectedSKU = _thisSelectedSku;

                //取出当前产品的定金SKU和团SKU
                data.DingjinSKU = { Price: 0, VIPPrice: 0 };
                data.GroupSKU = { Price: 0, VIPPrice: 0 };
                if (data.SKUInfo && data.SKUInfo.SKUList && data.SKUInfo.SKUList.length > 1) {
                    for (var _skunum = 0; _skunum < data.SKUInfo.SKUList.length; _skunum++) {
                        var _skuobj = data.SKUInfo.SKUList[_skunum];
                        if (_skuobj.IsDepositSKU) {
                            data.DingjinSKU = _skuobj;
                        }
                        else {
                            data.GroupSKU = _skuobj;
                        }
                    }
                }

                dingjinSku = data.DingjinSKU;
                groupSku = data.GroupSKU;

                //console.log("load detail")
                //console.log(data);

                //初始价格
                if (loadNum > 0 && productDetailData.productDetail && productDetailData.productDetail.priceInfo) {
                    data.priceInfo = productDetailData.productDetail.priceInfo;
                }

                //初始VIP专享状态（兼容老VIP和新VIP专享）
                data.activity.IsOffLine = false;
                if (data.SKUInfo && data.SKUInfo.SKU && data.SKUInfo.SKU.TagsList && data.SKUInfo.SKU.TagsList.length > 0) {
                    for (var _tagNum = 0; _tagNum < data.SKUInfo.SKU.TagsList.length; _tagNum++) {
                        var _tagItem = data.SKUInfo.SKU.TagsList[_tagNum];
                        switch (_tagItem.TagID) {
                            case 1:
                            case 5: { data.activity.IsVipExclusive = true; break; }
                            case 2: { data.activity.IsOffLine = true; break; }
                        }
                    }
                }

                //now time
                _nowTime = new Date(parseInt(year0), parseInt(month0) - 1, parseInt(day0), parseInt(hour0), parseInt(minute0), parseInt(second0));
                data.activity["y0"] = year0;
                data.activity["mo0"] = month0 - 1;
                data.activity["d0"] = day0;
                data.activity["h0"] = hour0;
                data.activity["mi0"] = minute0;
                data.activity["s0"] = second0;

                //团时间初始显示
                data.activity["sTit"] = "加载中..";
                data.activity["sD"] = "00";
                data.activity["sH"] = "00";
                data.activity["sM"] = "00";
                data.activity["sS"] = "00";

                //活动时间控制
                data.activity["IsOver"] = false;
                if (data.activity.SaleEndDate) {

                    var dtArr = (data.activity.SaleEndDate).split("-");
                    var dayArr = dtArr[2].split("T");
                    var timeArr = dayArr[1].split(":");
                    data.activity["y2"] = dtArr[0];
                    data.activity["mo2"] = parseInt(dtArr[1]) - 1;
                    data.activity["d2"] = dayArr[0];
                    data.activity["h2"] = timeArr[0];
                    data.activity["mi2"] = timeArr[1];
                    data.activity["s2"] = timeArr[2];

                    //是否结束
                    var _gEndTime = new Date(parseInt(data.activity["y2"]), parseInt(data.activity["mo2"]), parseInt(data.activity["d2"]), parseInt(data.activity["h2"]), parseInt(data.activity["mi2"]), parseInt(data.activity["s2"]));
                    if (_gEndTime < _nowTime) { data.activity.IsOver = true; }

                }

                //活动开始时间
                data.activity["IsStart"] = false;
                //console.log(data.activity.EffectiveTime);
                if (data.activity.EffectiveTime) {
                    var dtArr1 = (data.activity.EffectiveTime).split("-");
                    var dayArr1 = dtArr1[2].split("T");
                    var timeArr1 = dayArr1[1].split(":");
                    data.activity["y1"] = dtArr1[0];
                    data.activity["mo1"] = parseInt(dtArr1[1]) - 1;
                    data.activity["d1"] = dayArr1[0];
                    data.activity["h1"] = timeArr1[0];
                    data.activity["mi1"] = timeArr1[1];
                    data.activity["s1"] = timeArr1[2];
                }

                //如果是大团购产品，检查大团购产品的售卖时间（其实也是单买的可售时间）
                if (data.SKUInfo.StepGroup && data.SKUInfo.StepGroup.ID && data.SKUInfo.StepGroup.TailMoneyEndTime) {

                    data.SKUInfo.StepGroup["SingleBuyTimeIsOk"] = false;
                    var dtArr = (data.SKUInfo.StepGroup.TailMoneyEndTime).split("-");
                    var dayArr = dtArr[2].split("T");
                    var timeArr = dayArr[1].split(":");

                    //是否结束
                    var _gEndTime = new Date(parseInt(dtArr[0]), (parseInt(dtArr[1]) - 1), parseInt(dayArr[0]), parseInt(timeArr[0]), parseInt(timeArr[1]), parseInt(timeArr[2]));
                    if (_gEndTime > _nowTime) { data.SKUInfo.StepGroup.SingleBuyTimeIsOk = true; }
                }

                //当前产品是否分销产品
                data.activity["IsRetailer"] = false;
                if (data.activity.MerchantCode && data.activity.MerchantCode.indexOf("retailer") >= 0) {
                    data.activity.IsRetailer = true;
                }

                //推荐理由换行处理
                data.activity.Description = data.activity.Description.replace(/\r\n/g, "<br />")
                data.activity.Description = data.activity.Description.replace(/\n/g, "<br />");

                //对购买须知做过滤规则相关处理
                if (data.activity.NoticeList && data.activity.NoticeList.length) {
                    for (var _noticeNum = 0; _noticeNum < data.activity.NoticeList.length; _noticeNum++) {

                        var regexp = /(http:\/\/|https:\/\/)((\w|=|\?|\.|\/|&|-)+)/g; //"/(http://|https://)((w|=|?|.|/|&|-)+)/g";

                        var _notice = data.activity.NoticeList[_noticeNum];
                        data.activity.NoticeList[_noticeNum] = _notice.replace(regexp, function ($url) {
                            return "<a href='" + $url + "' target='_blank'>" + $url + "</a>";
                        });
                    }
                }

                //是否参与活动信息的解析（如送东航里程）
                if (data.ActiviyInfo) {
                    var activeJsonObj = JSON.parse(data.ActiviyInfo);
                    //console.log(activeJsonObj)
                    if (activeJsonObj) {
                        data["ActiviyInfoObj"] = activeJsonObj;
                    }
                }

                //可领红包解析
                if (data.CouponInfo && data.CouponInfo.CouponDefineList && data.CouponInfo.CouponDefineList.length) {

                    //当前总的券金额
                    var _sumCouponAmount = 0;

                    //遍历当前产品包含的现金券
                    data.CouponInfo.CouponDefineList.map(function (item, index) {

                        item.StartUseDate = (item.StartUseDate).split("T")[0];
                        item.ValidUntilDate = (item.ValidUntilDate).split("T")[0];

                        _sumCouponAmount += item.DiscountAmount;
                    });

                    if (inWeixin) {

                        _shareTitle = "{0} 领￥{1}大礼包".format(data.activity.PageTitle, _sumCouponAmount);
                        _shareDesc = data.activity.Tags ? data.activity.Tags : _shareDesc;
                        //console.log(_shareTitle);

                        var _shareSucessFunc = function () {

                            if (_getCouponIdx) {

                                //隐藏分享提示
                                $(".weixin-share-tip").hide();

                                //领取并弹出
                                goGetCoupon(parseInt(pubuserid));

                                _hideGiftCouponList();
                                _Modal.show({
                                    title: '',
                                    content: "<center>红包已领取！</center>",
                                    confirmText: '确定',
                                    confirm: function () {

                                        _showGiftCouponList();
                                        _Modal.hide();
                                    },
                                    showCancel: false
                                });

                                return;
                            }
                        }

                        //微信环境下，动态定义分享文案
                        loadWechat(_shareTitle, _shareDesc, _shareLink, _shareImgUrl, _shareSucessFunc);
                    }
                }

                //console.log(data)

                productDetailData.productDetail = data;

                Vue.nextTick(function () {

                    //绑定事件
                    bindEvent();

                    //如果是房券，则读取出当前房券对应的套餐的信息（主要是要读取酒店信息）
                    if (data.activity.Type === 200) {
                        loadHotelPackage(data.activity.SourceID);
                    }

                    //加载图文详情
                    loadSourcePage();
                })
            }

        });

    }

    //绑定事件
    var bindEvent = function () {

        //动态加载头图
        if (productDetailData.productDetail.activity.PicList) {

            //将每次的图片list缓存至对象
            productPicList = productDetailData.productDetail.activity.PicList;

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

        //绑定常规购买事件
        if ($(".submit")) {
            $(".submit").unbind("click");
            $(".submit").click(submitFun);
        }
        
        //绑定常规购买事件
        if ($("#single-buy")) {
            $("#single-buy").unbind("click");
            $("#single-buy").click(singlebuyFun);
        }

        //获取当前服务器时间
        $.get("/Coupon/GetNowtime", { timerFormat: "yyyy-MM-dd" }, function (_timeData) {

            //console.log(_timeData)
            year0 = _timeData.Year;
            month0 = _timeData.Month;
            day0 = _timeData.Day;
            hour0 = _timeData.Hour;
            minute0 = _timeData.Minute;
            second0 = _timeData.Second;

            //开始倒计时
            runTimer();

        });
    }

    loadProductDetail(pubskuid);

    var submitFun = function () {

        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
        if (pubuserid == "0") {

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

            //跳转到book页面继续购买
            var sellnum = $(".sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;

            //非VIP购买，弹出提示（增加false，暂时都不要显示弹出提示吧 2018-01-25 haoy）
            if (false && !productDetailData.productDetail.activity.IsRetailer && !isvip) {

                var _price = productDetailData.productDetail.SKUInfo.SKU.Price * productDetailData.productDetail.activity.MinBuyNum;
                var _vipprice = productDetailData.productDetail.SKUInfo.SKU.VIPPrice * productDetailData.productDetail.activity.MinBuyNum;

                var pcDic = { "orderTotalPrice": _price, "orderVipTotalPrice": _vipprice };
                $.get(_Config.APIUrl + "/api/coupon/BecomeVIPDiscountDescription", pcDic, function (_data) {

                    //console.log(_data);
                    if (_data && _data.ActionUrl && _data.Description) {

                        //_data.Description = _data.Description.replace(/1em/g, "1.5rem");

                        _Modal.show({
                            title: '<b style="font-size:1rem;">此为VIP专享价哦～还不是VIP会员？</b>',
                            content: _data.Description,
                            confirmText: '成为VIP会员',
                            confirm: function () {
                                _Modal.hide();
                                goBuyVip();
                            },
                            showCancel: true,
                            showClose: true,
                            cancelText: '继续购买',
                            cancel: function () {
                                _Modal.hide();
                                goCouponBook($("#thisskuid").val(), sellnum, $("#userid").val(), fromwxuid);
                                //gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&_isoneoff=1&_newpage=1".format($("#thisskuid").val(), sellnum, $("#userid").val(), fromwxuid));
                            },
                            close: function () {
                                _Modal.hide();
                            }
                        });

                        $("._modal-section").css("top", "25%");
                    }
                    else {

                        goCouponBook($("#thisskuid").val(), sellnum, $("#userid").val(), fromwxuid);
                        //gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&_isoneoff=1&_newpage=1".format($("#thisskuid").val(), sellnum, $("#userid").val(), fromwxuid));
                    }

                });
            }
            else {

                goCouponBook($("#thisskuid").val(), sellnum, $("#userid").val(), fromwxuid);
                //gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&_isoneoff=1&_newpage=1".format($("#thisskuid").val(), sellnum, $("#userid").val(), fromwxuid));
            }
            return;

            //如果是积分产品，首先弹出确认兑换的提示
            if (productDetailData.productDetail.SKUInfo.Category.PayType === 1 && productDetailData.productDetail.SKUInfo.SKU.Points > 0) {

                _Modal.show({
                    title: '',
                    content: '你正在兑换' + productDetailData.productDetail.SKUInfo.SKU.Name + '，本次兑换将消耗' + productDetailData.productDetail.SKUInfo.SKU.Points + '积分',
                    confirmText: '确认兑换',
                    confirm: function () {
                        gosubmit($("#userid").val(), false);
                        _Modal.hide();
                    },
                    showCancel: true,
                    cancelText: '取消',
                    cancel: function () {
                        _Modal.hide();
                    }
                });

            }
            else {
                gosubmit($("#userid").val(), false);
            }
        }

    }

    //绑定常规购买事件
    if ($(".submit")) {
        $(".submit").unbind("click");
        $(".submit").click(submitFun);
    }

    var singlebuyFun = function () {
        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallbackFroSingleBuy('{userid}')&realuserid=1";
        if (pubuserid == "0") {
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
            gosingleBuy(pubuserid);
        }
    }

    //绑定常规购买事件
    if ($("#single-buy")) {
        $("#single-buy").unbind("click");
        $("#single-buy").click(singlebuyFun);
    }

    //加载酒店套餐
    var loadHotelPackage = function (pid) {

        var _hotelPackageDic = { pid: pid, userid: pubuserid, startdate: '2017-07-01', enddate: '2017-07-02' };
        $.get(_Config.APIUrl + '/api/hotel/GetPackageDetailResult', _hotelPackageDic, function (data) {

            if (data) {
                //console.log(data)

                productDetailData.productDetail.hotelPackageInfo = data.packageItem;
            }

        });

    }

    function showTip(mes) {
        //console.log(mes)
        $(".pubAlertTip .tipinfo").html(mes);
        $(".pubAlertTip").fadeIn(500);
        setTimeout(function () {
            $(".pubAlertTip").fadeOut(300);
        }, 3000);
    }

    //红包操作
    var initActiveCouponEvent = function () {

        //当前产品可领券处理
        if (productDetailData.productDetail.CouponInfo && productDetailData.productDetail.CouponInfo.CouponDefineList && productDetailData.productDetail.CouponInfo.CouponDefineList.length) {

            $("#active-coupon-obj").click(function () {

                _showGiftCouponList();
            });

            $(".gift-coupon-list .close").click(function () {
                _hideGiftCouponList();

                //弹出活动浮窗
                $(".gift-coupon-ball").fadeIn();
            });

            $(".gift-coupon-list-model").click(function () {

                _hideGiftCouponList();

                //弹出活动浮窗
                $(".gift-coupon-ball").fadeIn();
            });

            //领取功能
            $("#gift-coupon-list .toget").each(function () {

                $(this).click(function () {

                    var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
                    if (parseInt(pubuserid)) {

                        _getCouponIdx = parseInt($(this).data("couponidx"));

                        if (isapp) {

                            //app内默认5秒钟后自动领取
                            setTimeout(function () {

                                //领取并弹出
                                goGetCoupon(parseInt(pubuserid));

                            }, 5000);

                            //app内弹出分享
                            gourl(shareNativeLink);
                        }
                        else {

                            //隐藏活动红包
                            _hideGiftCouponList();

                            //弹出活动浮窗
                            $(".gift-coupon-ball").fadeIn();

                            //提示微信分享
                            $(".weixin-share-tip").show();
                        }
                    }
                    else {

                        //app环境下，如果没有登录则弹出登录
                        if (isapp) {
                            location.href = loginapphref;
                            return;
                        }
                        else {
                            _loginModular.show();
                        }
                    }

                });
            });

            //如果有未领取的红包，则默认弹出领取红包窗口
            var _firtCouponDefine = productDetailData.productDetail.CouponInfo.CouponDefineList[0];
            //console.log(_firtCouponDefine)
            if (!_firtCouponDefine.UsedCount) {

                _hideGiftCouponList();
                _showGiftCouponList();

            }
        }
    }

    var runTimer = function () {
        var timerTags = $(".activity-timer");
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
                        //this.nowTime = new Date(
                        //parseInt(this.timerEntity.data("year0"))
                        //, parseInt(this.timerEntity.data("month0"))
                        //, parseInt(this.timerEntity.data("day0"))
                        //, parseInt(this.timerEntity.data("hour0"))
                        //, parseInt(this.timerEntity.data("minute0"))
                        //, parseInt(this.timerEntity.data("second0"))
                        //    ).getTime();
                        this.nowTime = new Date(
                        parseInt(year0)
                        , (parseInt(month0) - 1)
                        , parseInt(day0)
                        , parseInt(hour0)
                        , parseInt(minute0)
                        , parseInt(second0)
                            ).getTime();
                    },
                    initEndtime: function () {

                        //this.endDate = new Date(
                        //parseInt(this.timerEntity.data("year1"))
                        //, parseInt(this.timerEntity.data("month1"))
                        //, parseInt(this.timerEntity.data("day1"))
                        //, parseInt(this.timerEntity.data("hour1"))
                        //, parseInt(this.timerEntity.data("minute1"))
                        //, parseInt(this.timerEntity.data("second1"))
                        //    ).getTime();

                        this.endDate = new Date(
                        parseInt(productDetailData.productDetail.activity.y1)
                        , parseInt(productDetailData.productDetail.activity.mo1)
                        , parseInt(productDetailData.productDetail.activity.d1)
                        , parseInt(productDetailData.productDetail.activity.h1)
                        , parseInt(productDetailData.productDetail.activity.mi1)
                        , parseInt(productDetailData.productDetail.activity.s1)
                            ).getTime();
                    },
                    initClosetime: function () {

                        //this.closeDate = new Date(
                        //parseInt(this.timerEntity.data("year2"))
                        //, parseInt(this.timerEntity.data("month2"))
                        //, parseInt(this.timerEntity.data("day2"))
                        //, parseInt(this.timerEntity.data("hour2"))
                        //, parseInt(this.timerEntity.data("minute2"))
                        //, parseInt(this.timerEntity.data("second2"))
                        //    ).getTime();

                        this.closeDate = new Date(
                        parseInt(productDetailData.productDetail.activity.y2)
                        , parseInt(productDetailData.productDetail.activity.mo2)
                        , parseInt(productDetailData.productDetail.activity.d2)
                        , parseInt(productDetailData.productDetail.activity.h2)
                        , parseInt(productDetailData.productDetail.activity.mi2)
                        , parseInt(productDetailData.productDetail.activity.s2)
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
                            var h = Math.floor(t / 1000 / 60 / 60 % 24) + (d * 24);
                            var m = Math.floor(t / 1000 / 60 % 60);
                            var s = Math.floor(t / 1000 % 60);

                            productDetailData.productDetail.activity["sTit"] = "尚未开始";
                            productDetailData.productDetail.activity["sD"] = d;
                            productDetailData.productDetail.activity["sH"] = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                            productDetailData.productDetail.activity["sM"] = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                            productDetailData.productDetail.activity["sS"] = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);

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
                            var d = Math.floor(t / (1000 * 60 * 60 * 24));//alert(d)
                            var h = Math.floor(t / 1000 / 60 / 60 % 24) + (d * 24);
                            var m = Math.floor(t / 1000 / 60 % 60);
                            var s = Math.floor(t / 1000 % 60);

                            productDetailData.productDetail.activity["sTit"] = "距离结束仅剩";
                            productDetailData.productDetail.activity["sD"] = d;
                            productDetailData.productDetail.activity["sH"] = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                            productDetailData.productDetail.activity["sM"] = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                            productDetailData.productDetail.activity["sS"] = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);

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

                        productDetailData.productDetail.activity["sTit"] = "距团购结束仅剩";
                        productDetailData.productDetail.activity.IsStart = true;

                        Vue.nextTick(function () {

                            //绑定常规购买事件
                            if ($(".submit")) {
                                $(".submit").unbind("click");
                                $(".submit").click(submitFun);
                            }

                            //绑定常规购买事件
                            if ($("#single-buy")) {
                                $("#single-buy").unbind("click");
                                $("#single-buy").click(singlebuyFun);
                            }
                        })
                    },
                    stopCloseAction: function () {
                        this.closeTimerState = false;

                        //location.reload();

                        productDetailData.productDetail.activity["sTit"] = "活动结束";
                        productDetailData.productDetail.activity["sD"] = "00";
                        productDetailData.productDetail.activity["sH"] = "00";
                        productDetailData.productDetail.activity["sM"] = "00";
                        productDetailData.productDetail.activity["sS"] = "00";
                        productDetailData.productDetail.activity.IsOver = true;
                    }
                };

                //console.log(timerTags[i]);

                //build
                timeDic[i].timerEntity = $(timerTags[i]);

                //init
                timeDic[i].init();

                //start
                timeDic[i].timerAction();

                clearInterval(timerIntObj);
                timerIntObj = setInterval("gotime(timeDic[" + i + "])", 1000);
            }
        }
    }

    //延时加载活动说明
    setTimeout(function () {

        //分享提示图片动态加载
        var shareTipImg = $(".weixin-share-tip img");
        setImgOriSrc(shareTipImg);

        //右上角分享提示点击事件
        $(".weixin-share-tip").click(function () {
            $(this).hide();
        });

    }, 800);


    //加载酒店图文详情
    var loadSource = false;
    var loadSourcePage = function () {

        if (loadSource) return;
        if (productDetailData.productDetail.activity.MoreDetailUrl) {

            loadSource = true;

            //alert("b～");
            console.log("load source");

            //_hotelSourceUrl = "/active/activepage?midx=5886";
            productDetailData.productDetail.activity.MoreDetailUrl = productDetailData.productDetail.activity.MoreDetailUrl.replace("http://www.zmjiudian.com", "");

            $("#hotel-source-body").load(productDetailData.productDetail.activity.MoreDetailUrl, function (response, status, xhr) {

                if (status === "success") {
                    $(".source-more-btn").show();
                    $(".source-more-btn").click(function () {

                        $(".source-more-btn").hide();
                        $("#hotel-source-body").css("max-height", "100%");
                    });
                }
                else {
                    $(".hotel-source").hide();
                }
            });
        }
        else {
            $(".hotel-source").hide();
        }
    }
});

//单独购买原生登录回调
function loginCallbackFroSingleBuy(userid) {
    gosingleBuy(userid);
}

//单独购买(成团后的单独购买的是第二个团SKU)
var gosingleBuy = function (userid) {

    var sellnum = 1;

    //判断是否有前置预约
    console.log(groupSku)
    switch (groupSku.BookPosition) {
        case 0: {
            //不需要预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&_isoneoff=1&_newpage=1".format(groupSku.ID, sellnum, userid));
            break;
        }
        case 1: {
            //前置预约
            gourl("/Coupon/CouponReserve?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&exid={4}&prereserve=1&_isoneoff=1&_newpage=1".format(groupSku.ID, sellnum, userid, 0, 0));
            break;
        }
        case 2: {
            //后置预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&_isoneoff=1&_newpage=1".format(groupSku.ID, sellnum, userid));
            break;
        }

    }
};

//app登录回调
var loginCallback = function (_userid) {
    gourl("/coupon/product/" + productDetailData.productDetail.SKUInfo.SKU.ID + "?userid={userid}");
}

//领取券
var _getCouponIdx = 0;
var goGetCoupon = function (_userid) {

    if (_getCouponIdx) {
        $.get(_Config.APIUrl + "/api/coupon/ReceiveCouponDefine", { userid: _userid, couponDefineIds: _getCouponIdx }, function (_getData) {

            console.log(_getData);

            if (_getData) {

                refActiveCouponState(_userid);

                _getCouponIdx = 0;
            }
            else {

                alert("领取失败，请刷新后重试");
            }
        });
    }
}

//刷新券list
var refActiveCouponState = function (_userid) {

    var _couponDefineList = productDetailData.productDetail.CouponInfo.CouponDefineList;
    if (_couponDefineList) {

        _couponDefineList.map(function (item, index) {

            console.log(item)
            if (item.IDX == _getCouponIdx) {
                item.UsedCount = 1;
            }
        });
    }
}

var _showGiftCouponList = function () {
    //$(".welcome-list").css("top", $(window).scrollTop() + 15);
    $(".gift-coupon-list").fadeIn(200);
    $(".gift-coupon-list-model").show();
}
var _hideGiftCouponList = function () {
    $(".gift-coupon-list").hide();
    $(".gift-coupon-list-model").hide();
}

//submit
var _submitLock = false;
var gosubmit = function (userid, iscallback) {

    if (!_submitLock) {
        _submitLock = true;

        var subdic = {};
        subdic["aid"] = $("#aid").val();
        subdic["atype"] = $("#atype").val();
        subdic["skuid"] = $("#thisskuid").val();
        var sellnum = $(".sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
        subdic["paynum"] = sellnum;
        subdic["userid"] = userid;
        subdic["stype"] = stype;

        $.get('/Coupon/SubmitConponForProduct', subdic, function (content) {

            _submitLock = false;

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

                        _Modal.show({
                            title: '',
                            content: msg,
                            confirmText: '确定',
                            confirm: function () {

                                if (iscallback) {
                                    try {
                                        //如果是app环境，在VIP购买成功后向app标记用户信息产品了变更
                                        zmjd.userinfoChanged();
                                    } catch (e) {

                                    }
                                }

                                _Modal.hide();
                            },
                            showCancel: false
                        });

                        break;
                    }
                case "2":
                    {
                        _Modal.show({
                            title: '',
                            content: msg,
                            confirmText: '成为VIP',
                            confirm: function () {

                                if (iscallback) {
                                    try {
                                        //如果是app环境，在VIP购买成功后向app标记用户信息产品了变更
                                        zmjd.userinfoChanged();
                                    } catch (e) {

                                    }
                                }

                                goBuyVip();
                                _Modal.hide();
                            },
                            showCancel: true,
                            cancel: function () {

                                if (iscallback) {
                                    try {
                                        //如果是app环境，在VIP购买成功后向app标记用户信息产品了变更
                                        zmjd.userinfoChanged();
                                    } catch (e) {

                                    }
                                }

                                _Modal.hide();
                            }
                        });

                        break;
                    }
            }
        });
    }
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

function goto(param) {
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (!isapp) {
        url = "http://www.zmjiudian.com/" + param + "?_newpage=1";;
    }

    this.location = url;
}

function gourl(url) {
    location.href = url;
}

function loginCallback(userid) {

    //跳转到book页面继续购买
    var sellnum = $(".sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
    goCouponBook($("#thisskuid").val(), sellnum, userid, fromwxuid);
    //gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&_isoneoff=1&_newpage=1".format($("#thisskuid").val(), sellnum, userid, fromwxuid));

    //gosubmit(userid, true);
    ////location.reload();
    ////location.replace(location.pathname + "?userid=" + userid);
}

//去提交购买
function goCouponBook(skuid, paynum, userid, fromwxuid) {

    switch (productDetailData.productDetail.SKUInfo.SKU.BookPosition) {
        case 0: {
            //不需要预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid));
            break;
        }
        case 1: {
            //前置预约
            gourl("/Coupon/CouponReserve?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&exid={4}&prereserve=1&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid, 0));
            break;
        }
        case 2: {
            //后置预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid));
            break;
        }

    }
}

//倒计时相关
var timeDic = [];
var timerIntObj = {};

function gotime(timeObj) {
    timeObj.timerAction();
    timeObj.timerCloseAction();
}

$(".wxsignalert .iknow").click(function () {
    $(".wxsignalert").fadeOut();
});

$(".goyuyue").click(function () {
    $(".wxsignalert").fadeIn();
});

$(".wxSign").click(function () {
    $(".wxsignalert").fadeIn();
    var _apptype = $("#apptype").val();
    if (_apptype != "android") window.open("weixin://dl/");
});

if ($(".wxSignOk")) {
    setInterval(function () {
        var _obj = $(".wxSignOk .goshop-timer");
        var _count = parseInt(_obj.data("count"));
        if (_count == 1) {
            $(".wxSignOk").fadeOut();
        }
        else {
            _count--;
            _obj.data("count", _count)
            _obj.html(_count + "s后将自动跳转");
        }
    }, 1000);
    $(".wxSignOk .goshop").click(function () {
        $(".wxSignOk").fadeOut();
    });
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

    ////get urls
    //var _allImgs = $(".product-img");
    //_allImgs.each(function () {
    //    var _oriSrc = $(this).data("showsrc").replace("_640x426", "_jupiter");
    //    _param.urls.push(_oriSrc);
    //});
    _param.urls = productPicList;

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