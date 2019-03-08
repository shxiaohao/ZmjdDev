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
var _sourcekey = $P["_sourcekey"];

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

var _requestCID = $("#requestCID").val();
var _wxuid = $("#wxuid").val();


//SKU加载次数（每切换一次SKU累加一次）
var loadNum = 0;
var loadNumList = [0];

//是否显示更多套餐
var showMoreSku = false;

//当前选择SKU产品的图片list
var productPicList = [];

//产品detail model
var productDetailData = {};

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
    if (!isapp && pub_userid == "0") {
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
                            if (_val.indexOf("转")>=0) {
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

                //活动时间控制
                data.activity["IsOver"] = false;
                if (data.activity.SaleEndDate) {

                    var dtArr = (data.activity.SaleEndDate).split("-");
                    var dayArr = dtArr[2].split("T");
                    var timeArr = dayArr[1].split(":");
                    data.activity["y2"] = dtArr[0];
                    data.activity["mo2"] = parseInt(dtArr[1])-1;
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

                    //if (inWeixin) {

                    //    //_shareTitle = "{0} 领￥{1}大礼包".format(data.activity.PageTitle, _sumCouponAmount);
                    //    _shareTitle = "{0}".format(data.activity.PageTitle);
                    //    _shareDesc = data.activity.Tags ? data.activity.Tags : _shareDesc;
                    //    //console.log(_shareTitle);

                    //    var _shareSucessFunc = function () {

                    //        if (_getCouponIdx) {

                    //            //隐藏分享提示
                    //            $(".weixin-share-tip").hide();

                    //            //领取并弹出
                    //            goGetCoupon(parseInt(pubuserid));

                    //            _hideGiftCouponList();
                    //            _Modal.show({
                    //                title: '',
                    //                content: "<center>红包已领取！</center>",
                    //                confirmText: '确定',
                    //                confirm: function () {

                    //                    _showGiftCouponList();
                    //                    _Modal.hide();
                    //                },
                    //                showCancel: false
                    //            });

                    //            return;
                    //        }
                    //    }

                    //    //微信环境下，动态定义分享文案
                    //    loadWechat(_shareTitle, _shareDesc, _shareLink, _shareImgUrl, _shareSucessFunc);
                    //}
                }

                if (inWeixin) {

                    //_shareTitle = "{0} 领￥{1}大礼包".format(data.activity.PageTitle, _sumCouponAmount);
                    _shareTitle = "{0}".format(data.activity.PageTitle);
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

                    var _slink = "http://www.zmjiudian.com/coupon/product/" + skuid + "?CID=" + _requestCID + "&fromwxuid=" + _wxuid

                    //微信环境下，动态定义分享文案
                    loadWechat(_shareTitle, _shareDesc, _slink, _shareImgUrl, _shareSucessFunc);
                }
                
                //动态变更页面title
                document.title = data.SKUInfo.SKU.Name.replace("·", " ");

                //console.log(data)

                productDetailData.productDetail = data;

                Vue.nextTick(function () {

                    if (!_isRunNextTick) {

                        _nextTickFunctions();
                        _isRunNextTick = true;
                    }
                })

                var _isRunNextTick = false;
                var _nextTickFunctions = function () {

                    //绑定事件
                    bindEvent();

                    //页面初始进来时计算价格信息
                    setxiaoji();

                    //如果是房券，则读取出当前房券对应的套餐的信息（主要是要读取酒店信息）
                    if (data.activity.Type === 200) {
                        loadHotelPackage(data.activity.SourceID);
                    }

                    //加载图文详情
                    loadSourcePage();


                    //var $win = $(window);
                    //var hotelSourceTop = productDetailData.productDetail.activity.MoreDetailUrl ? $(".hotel-source").offset().top : 0;

                    //$(window).scroll(function () {
                    //    var winTop = $win.scrollTop();
                    //    var winHeight = $win.height();
                    //    if (winTop > 0 && winTop > (hotelSourceTop - winHeight)) {
                    //        loadHotelSourcePage();
                    //    }
                    //});
                }
                
                ////5秒后做一次补救加载事件
                //setTimeout(function () {

                //    if (!_isRunNextTick) {

                //        _nextTickFunctions();
                //        _isRunNextTick = true;
                //    }

                //}, 5000);
            }

        });

    }

    //绑定事件
    var _sectionTabTop = 0;
    var bindEvent = function () {
        
        //动态加载头图
        if (productDetailData.productDetail.activity.PicList) {

            //将每次的图片list缓存至对象
            productPicList = productDetailData.productDetail.activity.PicList;

            if (productDetailData.productDetail.activity.PicList.length > 1) {

                //头图滑动
                //动态设置首页smallbanner的具体宽度
                if ($('#product-img-list-' + loadNum)) {

                    console.log($('#product-img-list-' + loadNum))

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

        //切换套餐
        changeSku();

        //查看更多套餐
        $(".show-more").unbind("click");
        $(".show-more").click(function () {
            productDetailData.productDetail.showMoreSku = true;
            productDetailData.productDetail.showMoreBtn = false;
            
            setTimeout(changeSku, 200);
        });

        //可见“全员分销”tip
        if ($(".share-redpack-tip")) {
            $(".share-redpack-tip").show();
        }

        $(".submit").unbind("click");
        $(".submit").click(submitFun);

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

            //显示倒计时 IsShowCountDown
            if (productDetailData.productDetail.activity.IsShowCountDown) {
                $(".timer-tag").slideDown();
            }

        });

        //初始tab切换
        initTabSeaction();

        //页面滚动事件
        
        $(window).scroll(function () {

            if (!_sectionTabTop) {
                _sectionTabTop = $(".section-tab").offset().top;
            }

            var m_st = Math.max(document.body.scrollTop || document.documentElement.scrollTop);
            if (m_st > _sectionTabTop + 15) {
                $(".section-tab").addClass("section-tab-fixed");
            } else {
                $(".section-tab").removeClass("section-tab-fixed");
            }
        });
    }

    //初始记录访问记录
    try {

        //【数据统计】统计券访问记录
        var _category = "券详情页";
        var _action = "访问";
        var _label = (_sourcekey ? _sourcekey + "+" : "") + pubskuid;
        var _value = 1;
        var _nodeid = "";
        _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

        _statistic.push("券详情页", "访问", pubskuid, _sourcekey, "");

    } catch (e) {

    }

    loadProductDetail(pubskuid);

    //切换套餐
    var changeSku = function () {

        $(".product-item-link").each(function () {
            $(this).unbind("click");
            $(this).click(function () {
                var _skuid = $(this).data("skuid");
                //console.log("切换到SKU：")
                //console.log(_skuid)

                loadNum++;
                loadNumList.push(loadNum);

                if ($(".def-photo")) $(".def-photo").show();
                if ($(".product-photo")) $(".product-photo").hide();

                //隐藏倒计时
                $(".timer-tag").hide();

                //记录SKU的切换
                try {

                    //【数据统计】统计立即购买行为
                    var _category = "券详情页";
                    var _action = "券切换";
                    var _label = (_sourcekey ? _sourcekey + "+" : "") + _skuid;
                    var _value = 1;
                    var _nodeid = "";
                    _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

                    _statistic.push("券详情页", "券切换", _skuid, _sourcekey, "");

                } catch (e) {

                }

                //load detail
                loadProductDetail(_skuid);
            });
        });

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

    //加载店铺信息
    var loadProductShopInfo = function () {

    }

    //减 数量
    $(".btn0").click(function () {

        var cansell = $("#cansell").val() == "1";
        if (!cansell) return;

        var num = productDetailData.productDetail.activity.MinBuyNum;
        if (num == "" || isNaN(num)) {
            num = 1;
        }
        num = parseInt(num);
        
        var _buyCountStep = 1;
        try { _buyCountStep = parseInt(productDetailData.productDetail.SKUInfo.SKU.BuyCountStep); } catch (e) { }
        if (num > 1 & num >= _buyCountStep) num = num - _buyCountStep;

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
        
        var _buyCountStep = 1;
        try { _buyCountStep = parseInt(productDetailData.productDetail.SKUInfo.SKU.BuyCountStep); } catch (e) { }
        num = num + _buyCountStep;

        //最大购买验证
        var _maxBuyCount = productDetailData.productDetail.SKUInfo.SKU.MaxBuyCount;
        if (num > _maxBuyCount) {

            showTip("每人最多购买{0}份哦~".format(_maxBuyCount))
            return;
        }

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
        //积分产品暂时不通过该计算方式
        if (productDetailData.productDetail.PayType === 1) {

            //display
            $("#price-detail").show();

        }
        else {

            var sellPrice = parseFloat($("#sellPrice").val());
            var num = parseFloat(productDetailData.productDetail.activity.MinBuyNum);
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
                result["FirstVipPrice"] = 0.0;
                result["FirstPrice"] = 0.0;
                result["FirstPromotionVipPrice"] = 0.0;
                result["FirstPromotionPrice"] = 0.0;
                result["SumPrice"] = 0.0;
                result["SumVipPrice"] = 0.0;

                //console.log(result);

                if (result && result.SellPriceItemList && result.SellPriceItemList.length > 0 && result.SellVIPPriceItemList && result.SellVIPPriceItemList.length > 0) {

                    //默认价格
                    result.FirstVipPrice = parseFloat($("#skuVipPrice").val());
                    result.FirstPrice = parseFloat($("#skuPrice").val());
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
                    //$(".submit").html("￥" + _sumPrice + "&nbsp;&nbsp;立即购买");
                    $(".submit").html("立即购买");
                    $(".xiaoji").data("sum", _sumPrice);
                    $(".xiaoji .right .price").text(_sumPrice);

                    //顶部价格区域显示
                    productDetailData.productDetail.priceInfo = result;

                    //display
                    $("#price-detail").show();
                }
                else {
                    //$(".submit").html("￥" + sum + "&nbsp;&nbsp;立即购买");
                    $(".submit").html("立即购买");
                    $(".xiaoji").data("sum", sum);
                    $(".xiaoji .right .price").text(sum);
                }

                Vue.nextTick(function () {

                    //加载红包相关操作
                    initActiveCouponEvent();
                });
            });

        }

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
            //console.log(minbuyMsg);
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

    //红包操作
    var initActiveCouponEvent = function () {

        //当前产品可领券处理
        if (productDetailData.productDetail.CouponInfo && productDetailData.productDetail.CouponInfo.CouponDefineList && productDetailData.productDetail.CouponInfo.CouponDefineList.length) {

            //特殊处理，指定的几个产品前端不显示领取红包操作 2018.08.15 haoy
            //10052 立秀宝（防止黄牛刷的一个临时方案）
            if (pubskuid == 10052) {
                productDetailData.productDetail.CouponInfo.CouponDefineList = [];
                return;
            }

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

    //验证码操作
    $(".vCodeBtn").click(function () {
        var userPhone = $("#userPhone").val();
        if (userPhone.length != 11) {
            alert('请输入有效的手机号');
            return;
        }

        verify.send(userPhone, function (lock) {
            $(".vCodeBtn").toggleClass('disabled', lock).prop('disabled', lock);
            if (!lock) {
                $(".vCodeBtn").text('重发验证码');
            }
        }, function (seconds) {
            $(".vCodeBtn").text(seconds + '秒后可重发');
        });
    });

    //window.httpsWebUrl = "/";
    var verifyUrl = window.httpsWebUrl + 'Account/Verify';
    var verifyNewUserUrl = window.httpsWebUrl + 'Account/VerifyNewUser';
    var verify = {
        send: function (number, lock, update) {
            var self = this;
            if (self._timer) {
                return;
            }
            self._startTimer(lock, update);
            $.post(verifyUrl, {
                action: 'send',
                number: number
            }, 'json').then(function (r) {
                if (!r.ok) {
                    self._stopTimer(lock);
                    alert('系统故障，请稍后重试或联系技术支持');
                }
            }).fail(function () {
                self._stopTimer(lock);
                alert('网络请求失败');
            })
        },
        check: function (number, code) {
            return $.post(verifyNewUserUrl, {
                action: 'check',
                number: number,
                code: code,
                CID: $("#hidCurUserCID").val()
            }, 'json').then(function (r) {
                return r.ok || true ? contacts._store(number) : $.Deferred().reject(new Error('短信校验码有误'));
            });
        },
        _startTimer: function (lock, update) {
            var self = this;
            self._dest = new Date().getTime() + 60 * 1000;
            self._timer = setInterval(function () {
                var seconds = Math.max(0, ((self._dest - new Date) / 1000) | 0);
                seconds > 0 ? update(seconds) : self._stopTimer(lock);
            }, 1000);
            lock(true);
        },
        _stopTimer: function (lock) {
            clearInterval(this._timer);
            this._timer = null;
            lock(false);
        }
    };

    var isMobile = function (val) {
        return phoneNumReg.test(val);
    };

    var localStoreContact = function (contactPhone) {
        if (window.localStorage) {
            localStorage.contactPhone = contactPhone;
        } else {
            var now = new Date();
            var expires = new Date(now.setYear(now.getFullYear() + 1));
            document.cookie = 'contactPhone=' + contactPhone + '; expires=' + expires.toGMTString();
        }
    };
    var getStorePhone = function () {
        if (window.localStorage) {
            return localStorage.contactPhone;
        }
        return (document.cookie.match(/(^|; )contactPhone=(\d+)/) || ['', ''])[1];
    };

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

    //购买
    $(".submit").unbind("click");
    $(".submit").click(submitFun);

    var runTimer = function () {
        var timerTags = $(".timer-tag");
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
                        , (parseInt(month0)-1)
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
                            var h = Math.floor(t / 1000 / 60 / 60 % 24);// + (d * 24);
                            var h2 = Math.floor(t / 1000 / 60 / 60 % 24) + (d * 24);
                            var m = Math.floor(t / 1000 / 60 % 60);
                            var s = Math.floor(t / 1000 % 60);

                            var _timeHtml = "";
                            var _dVal = "", _hVal = "", _mVal = "", _sVal = "";
                            if (d > 0) {
                                _dVal = d < 0 ? "00" : (d < 10 ? "0" + d : "" + d);
                                _hVal = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                                _mVal = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                _sVal = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                _timeHtml = "距开始还有 " + _dVal + "天" + _hVal + "小时" + _mVal + "分钟";
                            }
                            else {
                                _hVal = h2 < 0 ? "00" : (h2 < 10 ? "0" + h2 : "" + h2);
                                _mVal = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                _sVal = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                _timeHtml = "距开始还有 " + _hVal + ":" + _mVal + ":" + _sVal;
                            }


                            this.timerEntity.html(_timeHtml);

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
                            var h2 = Math.floor(t / 1000 / 60 / 60 % 24) + (d * 24);
                            var m = Math.floor(t / 1000 / 60 % 60);
                            var s = Math.floor(t / 1000 % 60);

                            var _timeHtml = "";
                            var _dVal = "", _hVal = "", _mVal = "", _sVal = "";
                            if (d > 0) {
                                _dVal = d < 0 ? "00" : (d < 10 ? "0" + d : "" + d);
                                _hVal = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                                _mVal = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                _sVal = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                _timeHtml = "距结束还有 " + _dVal + "天" + _hVal + "小时" + _mVal + "分钟";
                            }
                            else {
                                _hVal = h2 < 0 ? "00" : (h2 < 10 ? "0" + h2 : "" + h2);
                                _mVal = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                _sVal = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                _timeHtml = "距结束还有 " + _hVal + ":" + _mVal + ":" + _sVal;
                            }

                            this.timerEntity.html(_timeHtml);

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

                        productDetailData.productDetail.activity.IsStart = true;

                        Vue.nextTick(function () {
                            $(".submit").unbind("click");
                            $(".submit").click(submitFun);
                        })

                        //reloadPage(true);

                        //$("#cansell").val("1");
                        //try {
                        //    $(".submit").css("display", "block");
                        //    $(".submit0").css("display", "none");
                        //} catch (e) {

                        //}
                    },
                    stopCloseAction: function () {
                        this.closeTimerState = false;
                        this.timerEntity.html("已结束");

                        productDetailData.productDetail.activity.IsOver = true;

                        //reloadPage(true);

                        //$("#cansell").val("0");
                        //try {
                        //    $(".submit").css("display", "none");
                        //    $(".submit0").css("display", "block");
                        //} catch (e) {

                        //}
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
            //productDetailData.productDetail.activity.MoreDetailUrl
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

    //初始tab切换功能
    var initTabSeaction = function () {

        $(".section-tab .tab-item").each(function () {
            
            $(this).click(function () {

                var _thisTab = $(this);
                console.log(_thisTab.data("sel"))

                //当前tab的打开状态
                var _relTabInfoOpened = _thisTab.data("sel") == 1;
                if (!_relTabInfoOpened) {

                    //去除其它tab选中
                    $(".section-tab .tab-item").each(function () {
                        $(this).removeClass("tab-item-sel");
                        $(this).data("sel", "0");
                    });

                    //当前tab选中
                    _thisTab.addClass("tab-item-sel");
                    _thisTab.data("sel", "1");

                    //隐藏其它的详细
                    $(".tab-info").each(function () {
                        $(this).hide();
                    });

                    //当前tab关联的详细
                    var _relTabInfoClass = _thisTab.data("openinfo");
                    var _tabInfoObj = $(".{0}".format(_relTabInfoClass))

                    //显示当前详细
                    _tabInfoObj.fadeIn(200);
                }

            });
        });
    }
});

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

    try {

        Store.Set("sku_sourcekey", "");

        //缓存当前sku的来源，该sku购买完成时通过该来源缓存统计sku的购买完成虚拟pv
        if (_sourcekey) {
            Store.Set("sku_sourcekey", _sourcekey);
        }

        //【数据统计】统计立即购买行为
        var _category = "券详情页";
        var _action = "券购买";
        var _label = (_sourcekey ? _sourcekey + "+" : "") + skuid;
        var _value = paynum;
        var _nodeid = "";
        _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

        _statistic.push("券详情页", "券购买", skuid, _sourcekey, "");

    } catch (e) {

    }

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