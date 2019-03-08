var wwidth = $(window).width();
var isapp = $("#isapp").val() == "1";
var isvip = $("#isvip").val() == "1";
var showShop = $("#showShop").val() == "1";
var isgroup = $("#isgroup").val() == "1";
var customerType = $("#customerType").val();
var fromwxuid = $("#fromwxuid").val();
var stype = $("#stype").val();
var pubskuid = $("#skuid").val();
var paynum = $("#paynum").val();
var groupid = $("#groupid").val();
var openid = $("#openid").val();
var coid = $("#coid").val();
var pubuserid = $("#userid").val();
var skuPrice = $("#skuPrice").val();
var skuVipPrice = $("#skuVipPrice").val();
var userFundAmount = parseInt($("#userFundAmount").val());

var writeOtherPostion = parseInt($("#writeOtherPostion").val());
var maxPersonNum = parseInt($("#maxPersonNum").val());
var minPersonNum = parseInt($("#minPersonNum").val());
var cartTypeListStr = $("#cartTypeList").val();
var cartTypeList = cartTypeListStr.split(',');
var travelPersonDesc = $("#travelPersonDesc").val();
var dateSelectType = parseInt($("#dateSelectType").val());
var dateTypeName = $("#dateTypeName").val();
var userLabel = $("#userLabel").val();
var userPlaceholder = $("#userPlaceholder").val();

var year0 = $("#year0").val();
var month0 = $("#month0").val();
var day0 = $("#day0").val();
var hour0 = $("#hour0").val();
var minute0 = $("#minute0").val();
var second0 = $("#second0").val();

var _name = $("#_name").val();

//SKU加载次数（每切换一次SKU累加一次）
var loadNum = 0;
var loadNumList = [0];

//是否显示更多套餐
var showMoreSku = false;

var _Config = new Config();
//_Config.APIUrl = "http://192.168.1.114:8000";
//_Config.APIUrl = "http://api.zmjd100.com";

var productDetailData = {};
var fundBindSelectEvent = function () { };

$(document).ready(function () {

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    productDetailData = new Vue({
        el: "#product-content",
        data: {
            "productDetail": {
                "priceInfo": {},
                "hotelPackageInfo": {}
            }
        }
    })

    //加载产品详情
    var loadProductDetail = function (skuid) {
        
        var _detailDic = { skuid: skuid, userid: pubuserid, couponOrderId: coid };
        $.get(_Config.APIUrl + '/api/coupon/GetSKUCouponActivityDetail', _detailDic, function (data) {

            if (data) {

                console.log(data)

                data.SKUID = skuid;
                data.UserId = pubuserid;
                data.CustomerType = customerType;
                data.SType = stype;
                data.IsVip = isvip;
                data.IsApp = isapp;

                data.priceInfo = {};
                data.vipTipInfo = {};
                data.baseCashCouponInfo = {};
                data.baseVoucherInfo = { voucherCount: 0, voucherIdList: [], voucherInfo: null };
                data.fundInfo = { userFundAmount: userFundAmount, canUseFund: 0, sel: 1 };
                data.orderPayPriceInfo = {};
                data.hotelPackageInfo = {};
                data.formData = {};
                data.canUseCoupon = true;
                data.otherPhotoUrl = "";
                data.otherPhotoUrlS = "http://whfront.b0.upaiyun.com/app/img/coupon/product/photo-add-icon.png";

                data.productDefImg = "http://whfront.b0.upaiyun.com/app/img/pic-def-16x9.png";
                data.loadNum = loadNum;
                data.loadNumList = loadNumList;

                //当前产品的支付类型（0费用支付 1积分支付）
                data.PayType = data.SKUInfo.Category.PayType;

                data.activity.OriMinBuyNum = data.activity.MinBuyNum;

                //将参数传递的购买数量，覆盖MinBuyNum
                data.activity.MinBuyNum = paynum;

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
                            case 6: { data.canUseCoupon = false; break; }
                        }
                    }
                }

                //临时处理，暂时不开放现金券使用，等钱包新现金券和VIP专区送500券功能一块儿
                //data.canUseCoupon = false;

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

                //console.log(data)

                productDetailData.productDetail = data;

                Vue.nextTick(function () {

                    //绑定事件
                    bindEvent();

                    //页面初始进来时计算价格信息
                    setxiaoji(false);

                    //其他提交信息（联系人、电话、地址等）
                    loadFormSection();

                    //购买
                    $(".gopay").unbind("click");
                    $(".gopay").click(submitFun);
                });
            }

        });

    }

    //绑定事件
    var bindEvent = function () {

        //手动判断是否显示购买须知
        if (!productDetailData.productDetail.activity.NoticeList || productDetailData.productDetail.activity.NoticeList.length <= 0) {
            $(".shopread").hide();
        }
        else {
            $(".shopread").show();
        }

        //切换套餐
        changeSku();

        //初始现金券
        initCashCoupon();

        //提交支付事件
        $(".submit").unbind("click");
        $(".submit").click(submitFun);
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

                //load detail
                loadProductDetail(_skuid);
            });
        });

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
        setxiaoji(true);

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
        setxiaoji(true);

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
        setxiaoji(true);

        //验证购买数量
        checkBuyNum();
    });

    //得出小计
    function setxiaoji(needRefCoupon)
    {
        //积分产品暂时不通过该计算方式
        if (productDetailData.productDetail.PayType === 1) {

            //display
            $("#price-detail").show();

        }
        else {

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

                if (needRefCoupon) {

                    //是否需要刷新现金券使用
                    if (productDetailData.productDetail) {

                        var _couponIdx = (productDetailData.productDetail.baseCashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo)
                            ? productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.IDX : 0;
                        if (_couponIdx > 0) {

                            //刷新当前选择券的状态
                            refCashCouponInfo(_couponIdx);

                            //刷新现金券选择列表
                            loadCashCouponSection(_couponIdx);
                        }
                        else {

                            if (_couponIdx > -1) {

                                //get best
                                getBestCashCoupon();
                            }
                            else {

                                //重新初始代金券(包含刷新住基金)
                                initDefaultVoucherInfo();

                                //刷新券选择列表
                                loadCashCouponSection(_couponIdx);
                            }
                        }
                    }
                    else {

                        //重新初始代金券(包含刷新住基金)
                        initDefaultVoucherInfo();

                        ////不能使用现金券时，直接刷新住基金
                        //refUserFund();
                    }
                }

            });

        }

        //刷新VIP tip
        refVipTipInfo();
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
            setxiaoji(true);
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

    //submit
    var submitFun = function () {

        //travel person
        if (writeOtherPostion == "1") {
            if (cartTypeList && cartTypeList.length && minPersonNum > 0) {
                if (addPersonSelecteds.length < minPersonNum) {
                    _Modal.show({
                        title: "",
                        content: "需至少添加" + minPersonNum + "名出行人",
                        textAlign: "center",
                        confirm: function () {
                            _Modal.hide();
                        }
                    });
                    return;
                }
            }
        }

        //模板表单信息
        var bookTempData = productDetailData.productDetail.formData;

        console.log(bookTempData);

        //验证表单录入
        if (bookTempData && bookTempData.TemplateItemObjs) {
            for (var i = 0; i < bookTempData.TemplateItemObjs.length; i++) {
                var titem = bookTempData.TemplateItemObjs[i];
                if (titem.MustWrite === 2 && !titem.Content) {
                    _Modal.show({
                        title: "",
                        content: "请填写" + titem.Name,
                        textAlign: "center",
                        confirm: function () {
                            _Modal.hide();
                        }
                    });
                    return;
                }
            }
        }

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

            //如果是积分产品，首先弹出确认兑换的提示
            if (productDetailData.productDetail.SKUInfo.Category.PayType === 1 && productDetailData.productDetail.SKUInfo.SKU.Points > 0) {
                
                var sellnum = $(".sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;

                _Modal.show({
                    title: '',
                    content: '你正在兑换' + productDetailData.productDetail.SKUInfo.SKU.Name + '，本次兑换将消耗' + (productDetailData.productDetail.SKUInfo.SKU.Points * sellnum) + '积分',
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

                if (!isvip) {

                    var _orderNoVipTotalPrice = productDetailData.productDetail.SKUInfo.SKU.Price * productDetailData.productDetail.activity.MinBuyNum;
                    var _orderVipTotalPrice = productDetailData.productDetail.SKUInfo.SKU.VIPPrice * productDetailData.productDetail.activity.MinBuyNum;

                    var pcDic = { "orderTotalPrice": _orderNoVipTotalPrice, "orderVipTotalPrice": _orderVipTotalPrice };
                    $.get(_Config.APIUrl + "/api/coupon/VIPDiscountDescription", pcDic, function (_data) {

                        console.log(_data);
                        if (_data && _data.ActionUrl && _data.Description) {

                            //分销产品不弹出“成为VIP”提示
                            if (productDetailData.productDetail.activity.MerchantCode && productDetailData.productDetail.activity.MerchantCode == "retailer") {

                                gosubmit($("#userid").val(), false);
                            }
                            else {

                                gosubmit($("#userid").val(), false);
                            }
                            
                        }

                    });
                }
                else {
                    gosubmit($("#userid").val(), false);
                }
            }
        }

    }

    //【住基金】住基金的选择事件
    fundBindSelectEvent = function () {
        
        console.log(productDetailData.productDetail.fundInfo)

        if (productDetailData.productDetail.fundInfo.sel === 1) {

            console.log("sel 0");
            productDetailData.productDetail.fundInfo.sel = 0;
        }
        else {

            console.log("sel 1");
            productDetailData.productDetail.fundInfo.sel = 1;
        }

        refUserFund();

    }

    //【住基金】刷新住基金使用
    var refUserFund = function () {

        console.log("refUserFund");

        //order total price
        var _orderTotalPrice = (isvip ? productDetailData.productDetail.SKUInfo.SKU.VIPPrice : productDetailData.productDetail.SKUInfo.SKU.Price) * productDetailData.productDetail.activity.MinBuyNum;

        //可用现金券
        var _couponIdx = 0;
        var _couponType = 0;
        var _cashCouponAmount = 0;
        if (productDetailData.productDetail && productDetailData.productDetail.baseCashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.IDX > 0) {
            _cashCouponAmount = productDetailData.productDetail.baseCashCouponInfo.OrderCanDiscountAmount;
            _couponIdx = productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.IDX;
            _couponType = productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.UserCouponType;
        }

        //可用代金券
        var _voucherAmount = 0;
        var _voucherIdList = [];
        if (productDetailData.productDetail && productDetailData.productDetail.baseVoucherInfo && productDetailData.productDetail.baseVoucherInfo.voucherInfo && productDetailData.productDetail.baseVoucherInfo.voucherInfo.OrderCanDiscountAmount) {
            _voucherAmount = productDetailData.productDetail.baseVoucherInfo.voucherInfo.OrderCanDiscountAmount;

            if (productDetailData.productDetail.baseVoucherInfo.voucherIdList.length) {
                _voucherIdList = productDetailData.productDetail.baseVoucherInfo.voucherIdList;
            }
        }

        //可用住基金
        var _canUseFund = userFundAmount;
        if (_orderTotalPrice - _cashCouponAmount - _voucherAmount < userFundAmount) {
            _canUseFund = _orderTotalPrice - _cashCouponAmount - _voucherAmount;
        }

        productDetailData.productDetail.fundInfo.userFundAmount = userFundAmount;
        productDetailData.productDetail.fundInfo.canUseFund = returnFloat(_canUseFund);

        Vue.nextTick(function () {

            console.log(666);

            //bind event
            //fundBindSelectEvent();
        });

        //如果没有选择住基金，则不算入最后的计算
        if (productDetailData.productDetail.fundInfo.sel === 0) {
            _canUseFund = 0;
        }

        //目前住基金是最后的优惠选择，所以在住基金刷新最后，刷新订单支付金额及优惠信息
        var _discountTotal = _cashCouponAmount + _canUseFund + _voucherAmount;
        var _orderPayPrice = _orderTotalPrice - _discountTotal;

        var _orderPayData = {
            defOrderPrice: returnFloat(_orderTotalPrice),
            payPrice: returnFloat(_orderPayPrice),
            discount: returnFloat(_discountTotal),
            couponIdx: _couponIdx,
            couponType: _couponType,
            cashCouponAmount: _cashCouponAmount,
            voucherIdList: _voucherIdList,
            voucherAmount: _voucherAmount,
            canUseFund: _canUseFund
        };

        console.log(_orderPayData);

        productDetailData.productDetail.orderPayPriceInfo = _orderPayData;

        //hideLoading();
    }


    //【现金券】刷新/检测券
    var refCashCouponInfo = function (_couponIdx) {

        var _orderTotalPrice = (isvip ? productDetailData.productDetail.SKUInfo.SKU.VIPPrice : productDetailData.productDetail.SKUInfo.SKU.Price) * productDetailData.productDetail.activity.MinBuyNum;

        var _couponParamDic = {
            BuyCount: productDetailData.productDetail.activity.MinBuyNum,
            TotalOrderPrice: _orderTotalPrice,
            SelectedCashCouponID: _couponIdx,
            OrderSourceID: pubskuid,
            OrderTypeID: 2,
            SelectedDateFrom: "2017-10-01",
            SelectedDateTo: "2017-10-02",
            UserID: pubuserid,
        };

        //console.log(_couponParamDic);

        $.post(_Config.APIUrl + "/api/coupon/CheckSelectedCashCouponInfoForOrder", _couponParamDic, function (_data) {

            console.log(_data);
            if (_data) {

                productDetailData.productDetail.baseCashCouponInfo = _data;
            }

            Vue.nextTick(function () {

                //重新初始代金券(包含刷新住基金)
                initDefaultVoucherInfo();

                ////刷新住基金
                //refUserFund();
            });

        });
    }

    //【现金券】Get Best
    var getBestCashCoupon = function () {

        var _orderTotalPrice = (isvip ? productDetailData.productDetail.SKUInfo.SKU.VIPPrice : productDetailData.productDetail.SKUInfo.SKU.Price) * productDetailData.productDetail.activity.MinBuyNum;

        //默认一个最优的现金券
        var _couponParamDic = {
            BuyCount: productDetailData.productDetail.activity.MinBuyNum,
            TotalOrderPrice: _orderTotalPrice,
            SelectedCashCouponID: 0,
            OrderSourceID: pubskuid,
            OrderTypeID: 2,
            SelectedDateFrom: "2017-10-01",
            SelectedDateTo: "2017-10-02",
            UserID: pubuserid,
            CanNotUseDiscountOverPrice: (productDetailData.productDetail.canUseCoupon ? 0 : 1)
        };
        $.post(_Config.APIUrl + "/api/coupon/GetTheBestCouponInfoForOrder", _couponParamDic, function (_data) {

            console.log(_data);
            if (_data) {

                productDetailData.productDetail.baseCashCouponInfo = _data;

                //加载现金券选择模块
                loadCashCouponSection(_data.CashCouponInfo.IDX);
            }

            //重新初始代金券(包含刷新住基金)
            initDefaultVoucherInfo();

            ////刷新住基金
            //refUserFund();

        });
    };

    //【现金券】加载现金券选择模块
    var loadCashCouponSection = function (_selectedCouponId) {

        var _orderTotalPrice = (isvip ? productDetailData.productDetail.SKUInfo.SKU.VIPPrice : productDetailData.productDetail.SKUInfo.SKU.Price) * productDetailData.productDetail.activity.MinBuyNum;

        var _buyCount = productDetailData.productDetail.activity.MinBuyNum;
        $("#cash-coupon-section").load(
            "/Coupon/WalletCashCoupon?userid={0}&couponid={1}&buycount={2}&totalprice={3}&from={4}&to={5}&sourceid={6}&sourcetype={7}&select=1&issection=0&canNotUseCashcoupon={8}"
            .format(pubuserid, _selectedCouponId, _buyCount, _orderTotalPrice, "2017-10-01", "2017-10-02", pubskuid, 2, (productDetailData.productDetail.canUseCoupon ? 0 : 1)));
    }


    //【代金券】刷新代金券
    var refVoucherInfo = function (_selectCouponIds) {

        //总价
        var _orderTotalPrice = (isvip ? productDetailData.productDetail.SKUInfo.SKU.VIPPrice : productDetailData.productDetail.SKUInfo.SKU.Price) * productDetailData.productDetail.activity.MinBuyNum;

        //减去现金券的优惠金额
        var _cashCouponAmount = 0;
        if (productDetailData.productDetail && productDetailData.productDetail.baseCashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.IDX > 0) {
            _cashCouponAmount = productDetailData.productDetail.baseCashCouponInfo.OrderCanDiscountAmount;
        }
        _orderTotalPrice = _orderTotalPrice - _cashCouponAmount;

        //当前SKU的代金券限额
        var _skuTotalAmounct = productDetailData.productDetail.SKUInfo.SKU.CanUseVoucherPrice * productDetailData.productDetail.activity.MinBuyNum;

        //当前已选券ID
        var _couponid = "";
        if (_selectCouponIds && _selectCouponIds.length) {
            $(_selectCouponIds).each(function () {
                if (_couponid) {
                    _couponid += ",";
                }
                _couponid += String($(this)[0]);

            });
        }

        var _couponParamDic = {
            buyCount: productDetailData.productDetail.activity.MinBuyNum,
            totalOrderPrice: _orderTotalPrice,
            orderSourceID: pubskuid,
            orderTypeID: 2,
            userID: pubuserid,
            maxOrderCanUseVoucherAmount: _skuTotalAmounct,
            selectedVoucherIDs: _couponid,
            CanNotUseDiscountOverPrice: (productDetailData.productDetail.canUseCoupon ? 0 : 1)
        };
        console.log(_couponParamDic);

        $.get("/coupon/CheckSelectedVoucherInfoForOrder", _couponParamDic, function (_data) {

            console.log(_data);
            if (_data && _data.Success) {

                productDetailData.productDetail.baseVoucherInfo.voucherCount = _data.VoucherInfoList.length;
                productDetailData.productDetail.baseVoucherInfo.voucherIdList = _selectCouponIds;
                productDetailData.productDetail.baseVoucherInfo.voucherInfo = {
                    VoucherShowName: _data.CashCouponShowName,
                    OrderCanDiscountAmount: _data.OrderCanDiscountAmount
                };
            }
            else {

                alert("已选代金券已失效，请重新选择");

                //重新初始代金券(包含刷新住基金)
                initDefaultVoucherInfo();
            }

            Vue.nextTick(function () {

                //刷新住基金
                refUserFund();
            });

        });
    }

    //【代金券】Get Default Voucher
    var initDefaultVoucherInfo = function () {

        //showLoading();

        //总价
        var _orderTotalPrice = (isvip ? productDetailData.productDetail.SKUInfo.SKU.VIPPrice : productDetailData.productDetail.SKUInfo.SKU.Price) * productDetailData.productDetail.activity.MinBuyNum;

        //减去现金券的优惠金额
        var _cashCouponAmount = 0;
        if (productDetailData.productDetail && productDetailData.productDetail.baseCashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.IDX > 0) {
            _cashCouponAmount = productDetailData.productDetail.baseCashCouponInfo.OrderCanDiscountAmount;
        }
        _orderTotalPrice = _orderTotalPrice - _cashCouponAmount;

        //根据条件获取当前可用的现金券数量
        var _couponParamDic = {
            BuyCount: productDetailData.productDetail.activity.MinBuyNum,
            TotalOrderPrice: _orderTotalPrice,
            SelectedCashCouponID: 0,
            OrderSourceID: pubskuid,
            OrderTypeID: 2,
            SelectedDateFrom: "2017-10-01",
            SelectedDateTo: "2017-10-02",
            UserID: pubuserid,
            CanNotUseDiscountOverPrice: (productDetailData.productDetail.canUseCoupon ? 0 : 1)
        };
        $.post(_Config.APIUrl + "/api/coupon/GetCanUseVoucherInfoListForOrder", _couponParamDic, function (_data) {

            console.log(_data);
            if (_data) {
                productDetailData.productDetail.baseVoucherInfo.voucherCount = _data.length;
            }
            productDetailData.productDetail.baseVoucherInfo.voucherIdList = [];
            productDetailData.productDetail.baseVoucherInfo.voucherInfo = null;

            //刷新住基金
            refUserFund();

            //加载代金券选择模块
            loadVoucherSection([]);
        });
    }

    //【代金券】加载代金券选择模块
    var loadVoucherSection = function (_selectCouponIds) {

        var _orderTotalPrice = (isvip ? productDetailData.productDetail.SKUInfo.SKU.VIPPrice : productDetailData.productDetail.SKUInfo.SKU.Price) * productDetailData.productDetail.activity.MinBuyNum;

        //减去现金券的优惠金额
        var _cashCouponAmount = 0;
        if (productDetailData.productDetail && productDetailData.productDetail.baseCashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo && productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.IDX > 0) {
            _cashCouponAmount = productDetailData.productDetail.baseCashCouponInfo.OrderCanDiscountAmount;
        }
        _orderTotalPrice = _orderTotalPrice - _cashCouponAmount;

        //当前SKU的代金券限额
        var _skuTotalAmounct = productDetailData.productDetail.SKUInfo.SKU.CanUseVoucherPrice * productDetailData.productDetail.activity.MinBuyNum;

        //购买数量
        var _buyCount = productDetailData.productDetail.activity.MinBuyNum;

        //当前已选券ID
        var _couponid = "";
        if (_selectCouponIds && _selectCouponIds.length) {
            $(_selectCouponIds).each(function () {
                if (_couponid) {
                    _couponid += ",";
                }
                _couponid += String($(this)[0]);
                
            });
        }

        console.log("加载代金券选择。。");

        $("#voucher-section").load(
            "/Coupon/WalletVoucher?userid={0}&couponid={1}&buycount={2}&totalprice={3}&skuVoucherPrice={4}&from={5}&to={6}&sourceid={7}&sourcetype={8}&canNotUseCashcoupon={9}&select=1&issection=0"
                .format(pubuserid, _couponid, _buyCount, _orderTotalPrice, _skuTotalAmounct, "2017-10-01", "2017-10-02", pubskuid, 2, (productDetailData.productDetail.canUseCoupon ? 0 : 1)));
    }


    //【现金券】当前套餐可以使用现金券的话..load..
    var initCashCoupon = function () {
        if (productDetailData.productDetail) {

            //展开选择现金券模块
            var openCashSection = function () {

                //url状态标识
                location.href = location.href + "#cash-coupon-section";
            }
            $(".sel-cash").click(openCashSection);

            //展开选择代金券模块
            var openCashSection = function () {

                //url状态标识
                location.href = location.href + "#voucher-section";
            }
            $(".sel-voucher").click(openCashSection);

            //get best cashcoupon
            getBestCashCoupon();

            //选择现金券的处理事件
            window.selectCashCouponFun = function (_couponIdx) {

                if (_couponIdx >= 0) {

                    //刷新现金券
                    refCashCouponInfo(_couponIdx);

                } else {

                    productDetailData.productDetail.baseCashCouponInfo.CashCouponInfo.IDX = -1;

                    //重新初始代金券(包含刷新住基金)
                    initDefaultVoucherInfo();

                    ////刷新住基金
                    //refUserFund();
                }
            }

            //选择代金券的处理事件
            window.selectVoucherFun = function (_selectCouponIds) {

                if (_selectCouponIds && _selectCouponIds.length) {

                    console.log("当前选择的代金券：");
                    console.log(_selectCouponIds);

                    //刷新代金券(包含刷新住基金)
                    refVoucherInfo(_selectCouponIds);

                } else {

                    console.log("没有选择代金券");
                    //重新初始代金券(包含刷新住基金)
                    initDefaultVoucherInfo();
                }
            }
        }
        else {

            //重新初始代金券(包含刷新住基金)
            initDefaultVoucherInfo();

            ////不能使用现金券时，直接刷新住基金
            //refUserFund();
        }
    }

    //非VIP用户，显示成为VIP的提示块
    var refVipTipInfo = function () {
        if (!isvip) {

            var _orderNoVipTotalPrice = productDetailData.productDetail.SKUInfo.SKU.Price * productDetailData.productDetail.activity.MinBuyNum;
            var _orderVipTotalPrice = productDetailData.productDetail.SKUInfo.SKU.VIPPrice * productDetailData.productDetail.activity.MinBuyNum;

            var pcDic = { "orderTotalPrice": _orderNoVipTotalPrice, "orderVipTotalPrice": _orderVipTotalPrice };
            $.get(_Config.APIUrl + "/api/coupon/BecomeVIPTips", pcDic, function (_data) {

                console.log(_data);
                if (_data && _data.ActionUrl) {

                    productDetailData.productDetail.vipTipInfo = _data;

                    $("#vip-tip-section").show();
                }

            });
        }
    }

    //加载模板表单
    var loadFormSection = function () {

        //postionIndex：0不需要填写，1：购买前填写信息 2：预约是填写信息
        var _dic = { skuid: pubskuid, postionindex: 1 };

        $.get(_Config.APIUrl + "/api/Coupon/GetSKUTempSource", _dic, function (_data) {

            console.log(_data);

            if (_data.TemplateItem) {

                //将模板json字符串转换为对象
                var templateItemStr = _data.TemplateItem;
                var templateItemObjs = JSON.parse(templateItemStr);
                console.log(templateItemObjs);
                _data.TemplateItemObjs = templateItemObjs;

                //bind
                productDetailData.productDetail.formData = _data;

                console.log(productDetailData.productDetail)

                $("#form-section").show();
            }

        });

    }
});

var urlHashChange = function () {

    var _url = window.location.href;
    var _urls = _url.split("#");
    if (_urls.length > 1) {

        var _tag = _urls[1];

        console.log(_tag);

        if ($("#" + _tag)) {
            $("#" + _tag).show();

            if (_tag === "cash-coupon-section") {
                document.title = "现金券";
            }
        }
    }
    else {

        console.log("no #");

        //清除/隐藏所有tag元素
        $("#cash-coupon-section").hide();
        $("#voucher-section").hide();

        document.title = "确认订单";
    }
}
urlHashChange();
window.onhashchange = urlHashChange;

//app登录回调
function loginCallback(userid) {

    gosubmit(userid, true);
    //location.reload();
    //location.replace(location.pathname + "?userid=" + userid);
}

//show loading
var showLoading = function () {
    $("._zmjd_loading").show();
}

//hide loading
var hideLoading = function () {
    $("._zmjd_loading").hide();
}

var _submitLock = false;

//普通消费券购买
var gosubmit = function (userid, iscallback) {

    if (isgroup) {

        openBuySubmit(userid);

    }
    else {

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

            subdic["selectedVoucherIDs"] = "";
            subdic["voucherAmount"] = 0;

            //模板表单信息
            var bookTempData = productDetailData.productDetail.formData;
            subdic["bookTempDataString"] = JSON.stringify(bookTempData.TemplateItemObjs);
            subdic["bookTempId"] = bookTempData.TemplateID ? bookTempData.TemplateID : 0;
            subdic["bookTempDescription"] = bookTempData.Description;

            //出行人
            subdic["travelPersons"] = getTravelPersonsList()

            //照片信息
            if (productDetailData.productDetail.SKUInfo.SKU.NeedPhoto) {

                if (productDetailData.productDetail.otherPhotoUrl) {
                    subdic["otherPhotoUrl"] = productDetailData.productDetail.otherPhotoUrl;
                }
                else {

                    _Modal.show({
                        title: "",
                        content: "请上传照片 ",
                        textAlign: "center",
                        confirm: function () {
                            _Modal.hide();
                        }
                    });
                    _submitLock = false;
                    return;
                }

            }

            //console.log(subdic);
            //return;

            if (productDetailData && productDetailData.productDetail && productDetailData.productDetail.orderPayPriceInfo) {

                //cash coupon
                if (productDetailData.productDetail.orderPayPriceInfo.couponIdx && productDetailData.productDetail.orderPayPriceInfo.cashCouponAmount) {
                    subdic["cashCouponIdx"] = productDetailData.productDetail.orderPayPriceInfo.couponIdx;
                    subdic["cashCouponType"] = productDetailData.productDetail.orderPayPriceInfo.couponType;
                    subdic["cashCouponAmount"] = productDetailData.productDetail.orderPayPriceInfo.cashCouponAmount;
                }

                //voucher
                if (productDetailData.productDetail.orderPayPriceInfo.voucherIdList && productDetailData.productDetail.orderPayPriceInfo.voucherAmount) {

                    //当前已选券ID
                    var _selectCouponIds = productDetailData.productDetail.orderPayPriceInfo.voucherIdList;
                    var _couponid = "";
                    if (_selectCouponIds && _selectCouponIds.length) {
                        $(_selectCouponIds).each(function () {
                            if (_couponid) {
                                _couponid += ",";
                            }
                            _couponid += String($(this)[0]);

                        });
                    }

                    subdic["selectedVoucherIDs"] = _couponid;
                    subdic["voucherAmount"] = productDetailData.productDetail.orderPayPriceInfo.voucherAmount;
                }

                //fund
                if (productDetailData.productDetail.orderPayPriceInfo.canUseFund) {
                    subdic["useFundAmount"] = productDetailData.productDetail.orderPayPriceInfo.canUseFund;
                }
            }

            //来自哪个微信用户的分享
            subdic["fromwxuid"] = fromwxuid;

            //大团购补尾款的定金关联CouponOrderId
            subdic["coid"] = coid;

            console.log("提交券订单");
            console.log(subdic);
            //_submitLock = false;
            //return;

            $.get('/Coupon/SubmitConponForProduct', subdic, function (content) {

                _submitLock = false;

                var msg = content.Message;
                var suc = content.Success;
                var exids = content.Exids;
                var exidlist = exids?exids.split(","):[];
                var url = content.Url;

                switch (suc) {
                    case "0":
                        {
                            //提交成功，如果是提前预约的券，则需要先提交预约，再跳转支付页
                            if (productDetailData.productDetail.SKUInfo.SKU.BookPosition === 1) {

                                //预约缓存Key
                                var _reserveCacheKey = "CouponReserve_{0}_{1}".format($("#thisskuid").val(), userid);

                                //获取预约信息
                                var _reserveCacheData = Store.Get(_reserveCacheKey);
                                //console.log(_reserveCacheData);

                                //赋值exchangeid list
                                _reserveCacheData.ExchangCouponIds = exidlist;
                                //console.log(_reserveCacheData);
                                //return;

                                $.post(_Config.APIUrl + "/api/Coupon/SubmitBookInfo", _reserveCacheData, function (_result) {

                                    console.log(_result);

                                    if (_result.RetCode === "1") {

                                        //alert("预约成功")
                                        location = url;

                                    }
                                    else {
                                        _Modal.show({
                                            title: "",
                                            content: _result.Message,
                                            textAlign: "center",
                                            confirm: function () {
                                                _Modal.hide();
                                            }
                                        });
                                    }
                                });
                            }
                            else {

                                location = url;
                            }
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

    }


};

//发起/参与拼团
var openBuySubmit = function (userid) {

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
        subdic["groupId"] = groupid;
        subdic["openid"] = openid;

        subdic["selectedVoucherIDs"] = "";
        subdic["voucherAmount"] = 0;

        if (productDetailData && productDetailData.productDetail && productDetailData.productDetail.orderPayPriceInfo) {

            //cash coupon
            if (productDetailData.productDetail.orderPayPriceInfo.couponIdx && productDetailData.productDetail.orderPayPriceInfo.cashCouponAmount) {
                subdic["cashCouponIdx"] = productDetailData.productDetail.orderPayPriceInfo.couponIdx;
                subdic["cashCouponType"] = productDetailData.productDetail.orderPayPriceInfo.couponType;
                subdic["cashCouponAmount"] = productDetailData.productDetail.orderPayPriceInfo.cashCouponAmount;
            }

            //voucher
            if (productDetailData.productDetail.orderPayPriceInfo.voucherIdList && productDetailData.productDetail.orderPayPriceInfo.voucherAmount) {

                //当前已选券ID
                var _selectCouponIds = productDetailData.productDetail.orderPayPriceInfo.voucherIdList;
                var _couponid = "";
                if (_selectCouponIds && _selectCouponIds.length) {
                    $(_selectCouponIds).each(function () {
                        if (_couponid) {
                            _couponid += ",";
                        }
                        _couponid += String($(this)[0]);

                    });
                }

                subdic["selectedVoucherIDs"] = _couponid;
                subdic["voucherAmount"] = productDetailData.productDetail.orderPayPriceInfo.voucherAmount;
            }

            //fund
            if (productDetailData.productDetail.orderPayPriceInfo.canUseFund) {
                subdic["useFundAmount"] = productDetailData.productDetail.orderPayPriceInfo.canUseFund;
            }
        }

        //模板表单信息
        var bookTempData = productDetailData.productDetail.formData;
        subdic["bookTempDataString"] = JSON.stringify(bookTempData.TemplateItemObjs);
        subdic["bookTempId"] = bookTempData.TemplateID;
        subdic["bookTempDescription"] = bookTempData.Description;

        //出行人
        subdic["travelPersons"] = getTravelPersonsList()

        console.log(subdic);

        //console.log("提交拼团")
        //console.log(subdic)
        //return

        $.get('/Coupon/SubmitConponForGroupProduct', subdic, function (content) {

            _submitLock = false;

            var msg = content.Message;
            var suc = content.Success;
            var exids = content.Exids;
            var exidlist = exids ? exids.split(",") : [];
            var url = content.Url;

            switch (suc) {
                case "0":
                    {
                        //提交成功，如果是提前预约的券，则需要先提交预约，再跳转支付页
                        if (productDetailData.productDetail.SKUInfo.SKU.BookPosition == 1) {

                            //预约缓存Key
                            var _reserveCacheKey = "CouponReserve_{0}_{1}".format($("#thisskuid").val(), userid);

                            //获取预约信息
                            var _reserveCacheData = Store.Get(_reserveCacheKey);
                            //console.log(_reserveCacheData);

                            //赋值exchangeid list
                            _reserveCacheData.ExchangCouponIds = exidlist;
                            //console.log(_reserveCacheData);
                            //return;

                            $.post(_Config.APIUrl + "/api/Coupon/SubmitBookInfo", _reserveCacheData, function (_result) {

                                console.log(_result);

                                if (_result.RetCode === "1") {

                                    //alert("预约成功")
                                    location = url;

                                }
                                else {
                                    _Modal.show({
                                        title: "",
                                        content: _result.Message,
                                        textAlign: "center",
                                        confirm: function () {
                                            _Modal.hide();
                                        }
                                    });
                                }
                            });
                        }
                        else {

                            location = url;
                        }
                        break;
                    }
                case "1":
                    {
                        _Modal.show({
                            title: '',
                            content: msg,
                            confirmText: '确定',
                            confirm: function () {
                                _Modal.hide();
                            },
                            showCancel: false
                        });

                        //alert(msg);
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
                case "3":
                    {
                        _Modal.show({
                            title: '',
                            content: msg,
                            confirmText: '去发起拼团',
                            confirm: function () {
                                goto("/coupon/group/product/" + subdic["skuid"] + "/0?userid=" + subdic["userid"]);
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
    }
}

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

////照片预览功能
//var previewImage = function () {

//    var _thisImg = $(this);
//    var _param = { index: 0, urls: [] };
//    _param.index = _thisImg.data("num");

//    //get urls
//    var _allImgs = $(".product-img");
//    _allImgs.each(function () {
//        var _oriSrc = $(this).data("showsrc").replace("_640x426", "_jupiter");
//        _param.urls.push(_oriSrc);
//    });

//    zmjd.previewImage(_param);
//}
//var previewImageSuccess = function () {
//    console.log("预览成功")
//}
//var previewImageFail = function () {
//    console.log("预览失败")
//}

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

/*出行人 相关处理*/

//所有出行人信息
var allPersonList = {};

//初始加载当前用户的出行人信息
var allPersonList = [];
var allPersonVue = {};

//添加出行人初始list
var addPersonAdds = [];
var addPersonSelecteds = [];
var addPersonVue = {};

//编辑出行人Vue相关
//0 add     1 edit
var personEditType = 0;
var personEditVue = {};

var getPerson = function (num) {
    return allPersonList[num];
}

//show win
var showSelPerson = function () {

    //如果需要添加出行人，但没有userid，则需要首先验证手机号获取userid
    if (pubuserid == "0") {
        _loginModular.show();
        return;
    }

    $(".b-d-win-panel").show();
    $(".b-d-win-model").show();
}

//hide win
var hideSelPerson = function () {
    $(".b-d-win-panel").hide();
    $(".b-d-win-model").hide();
}

var getTravelPersonsList = function () {
    var _str = "";
    if (addPersonSelecteds) {
        addPersonSelecteds.map(function (item, index) {
            if (_str != "") _str += ",";
            _str = _str + item.ID;
        });
    }

    return _str;
}

//back selperson
$("#back-selperson").click(function () {

    //返回操作，统计当前选择的出行人信息，以及是否合法
    var cklist = $("#ad-person input:checkbox[name='person-ck']:checked");
    if (cklist.length > maxPersonNum) {
        alert("最多选择" + maxPersonNum + "位出行人");
        return;
    }

    var _repeatckstate = true;
    var _repeatcks = [];
    var _addsles = [];
    cklist.map(function (index, item) {
        var _num = parseInt($(item).data("psnum"));
        var _person = getPerson(_num);

        //check repeat
        var _ckkey = _person.IDType + _person.IDNumber;
        if ($.inArray(_ckkey, _repeatcks) > -1) {
            _repeatckstate = false;
            return;
        }
        _repeatcks.push(_ckkey);

        _addsles.push(_person);
    });

    if (!_repeatckstate) {
        alert("请勿选择证件号码重复的出行人，请核对");
        return;
    }

    addPersonSelecteds = _addsles;

    refAddPersonOptions();

    hideSelPerson();
});

//show edit
var showEditPerson = function (num) {
    $("#ad-person").hide();
    $("#edit-person").show();

    //add
    if (personEditType == 0) {

        personEditVue.person = {
            "ID": 0,
            "UserID": pubuserid,
            "TravelPersonName": "",
            "IDType": 1,
            "IDNumber": "",
            "Birthday": "1990-01-01"
        };
    }
        //edit
    else if (personEditType == 1) {

        var _person = getPerson(num);
        _person.Birthday = formatDate(parseInt(_person.Birthday.slice(6)), "yyyy-MM-dd");

        personEditVue.person = _person;
    }
}

var delSelPerson = function (index) {
    addPersonSelecteds.baoremove(index);
    bindPersonList();
}

$("#add-person-btn").click(function () {
    personEditType = 0;
    showEditPerson();
});

//save editperson
$("#save-editperson").click(function () {

    var _dic = {};
    _dic["id"] = personEditVue.person.ID;
    _dic["userid"] = pubuserid;
    _dic["travelPersonName"] = personEditVue.person.TravelPersonName;
    _dic["idType"] = personEditVue.person.IDType;
    _dic["idnumber"] = personEditVue.person.IDNumber;
    _dic["birthday"] = personEditVue.person.Birthday;

    //add
    if (personEditType == 0) {
        _dic["saveType"] = 0;
    }
    //edit
    else if (personEditType == 1) {
        _dic["saveType"] = 1;
    }

    if (_dic["travelPersonName"].AllTrim().length <= 0) {
        alert("请如实填写姓名");
        return;
    }
    else if (_dic["idnumber"].AllTrim().length <= 0) {
        alert("请如实填写证件号码");
        return;
    }
    else if (_dic["idType"] != 1 && _dic["birthday"].length <= 0) {
        alert("请如实填写出生日期");
        return;
    }

    $.get('/Hotel/SaveTravelPerson', _dic, function (back) {

        if (back.Success) {

            bindPersonList();

            $("#edit-person").hide();
            $("#ad-person").show();
        }
        else {
            alert("出新人信息保存错误");
        }
    });
});

//cancel edit person
$("#back-editperson").click(function () {
    $("#edit-person").hide();
    $("#ad-person").show();
});

var bindPersonList = function () {
    $.get('/Hotel/GetTravelPersonByUserId', { userid: pubuserid }, function (back) {

        //person list
        allPersonList = back;
        allPersonList.map(function (item, index) {

            //身份证加星
            item["IDNumber2"] = plusXing(item.IDNumber, 4, 4);

            //checkbox state
            item["ck"] = false;
            addPersonSelecteds.map(function (item2, index2) {
                if (item.ID == item2.ID) {
                    item["ck"] = true;
                    return;
                }
            });

            //select state
            item["select"] = true;
            if (("," + cartTypeListStr + ",").indexOf("," + item.IDType + ",") < 0) {
                item["select"] = false;
            }

        });
        allPersonVue.list = allPersonList;
    });

    //生成“添加出行人”项
    refAddPersonOptions();
};

var refAddPersonOptions = function () {

    addPersonAdds = [];

    for (var _i = 0; _i < maxPersonNum - addPersonSelecteds.length; _i++) {

        addPersonAdds.push({});
    }

    //add options
    addPersonVue.adds = addPersonAdds;

    //selected options
    addPersonVue.sels = addPersonSelecteds;
};

var loginCheckFun = function (userid) {
    //alert(userid);
    location.href = location.href + "&userid=" + userid;
}

var loginCancelFun = function () {

    alert(travelPersonDesc + "\r\n" + "添加出行人需先登录");
    return true;
}

var initPersonInfo = function () {

    //绑定全部出行人信息
    //allPersonList = [
    //    { "ID": 1, "TravelPersonName": "小豪", "IDNumber": "372922198902046219", "Birthday": "1999-01-01" },
    //    { "ID": 2, "TravelPersonName": "小强", "IDNumber": "388888199002028888", "Birthday": "1999-02-02" }
    //];

    //初始添加出行人
    if (maxPersonNum > 0) {

        _loginModular.init(loginCheckFun, loginCancelFun);

        //init add person options
        addPersonVue = new Vue({
            el: "#person-panel",
            data: {
                "adds": addPersonAdds,
                "sels": addPersonSelecteds
            }
        })

        //init person list
        allPersonVue = new Vue({
            el: '#ad-person',
            data: { "list": allPersonList }
        })
        bindPersonList();

        //init edit person
        var _newperson = {
            "ID": 0,
            "UserID": pubuserid,
            "TravelPersonName": "",
            "IDType": 1,
            "IDNumber": "",
            "Birthday": "1990-01-01"
        };
        personEditVue = new Vue({
            el: '#edit-person',
            data: {
                "person": _newperson
            }
        })
    }

    $("#person-panel").show();
}

if (writeOtherPostion == "1") {
    initPersonInfo();
}


//图片上传预览    IE是用了滤镜。
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

        //var img = new Image();
        //img.name = file.name;
        //if (window.URL) {
        //    //File API
        //    img.src = window.URL.createObjectURL(file); //创建一个object URL，并不是你的本地路径

        //    img.onload = function (e) {
        //        window.URL.revokeObjectURL(this.src); //图片加载后，释放object URL
        //        $("#preview").html(img);
        //    }
        //}
        //else if (window.FileReader) {
        //    //opera不支持createObjectURL/revokeObjectURL方法。我们用FileReader对象来处理
        //    var reader = new FileReader();
        //    reader.readAsDataURL(file);
        //    reader.onload = function (e) {
        //        img.src = this.result;
        //        $("#preview").html(img);
        //    }
        //}
        
        $(".uploadlodding").show();
        var cusPhotoUrl = "http://p1.zmjiudian.com/" + PhotoSURL + "_640x640";
        var cusPhotoUrl_s = "http://p1.zmjiudian.com/" + PhotoSURL + "_290x290";

        //睡眠5秒   等待上传头像完成
        setTimeout(function () { 
            productDetailData.productDetail.otherPhotoUrl = cusPhotoUrl;
            productDetailData.productDetail.otherPhotoUrlS = cusPhotoUrl_s;
            //productDetailData.productDetail.otherPhotoUrlS = "http://p1.zmjiudian.com/118fW5t0u8_290x290";
            setTimeout(function () { $(".uploadlodding").hide(); }, 300);

            $("#imghead").unbind("error");
            $("#imghead").error(function () {
                alert("照片上传失败，请换一张重试哦");
                productDetailData.productDetail.otherPhotoUrl = "";
                productDetailData.productDetail.otherPhotoUrlS = "http://whfront.b0.upaiyun.com/app/img/coupon/product/photo-add-icon.png";
            }); 

        }, 5000);

        console.log("上传图片");
        console.log(cusPhotoUrl);
        console.log(cusPhotoUrl_s);

        ////_Config.APIUrl = "http://api.dev.jiudian.corp";
        ////保存数据库
        //$.ajax({
        //    url: _Config.APIUrl + '/api/shop/UpdateAvatarUrl',
        //    type: "GET",
        //    data: { cid: 0, avatarUrl: PhotoSURL },
        //    success: function (data) {
        //        $(".uploadlodding").show()
        //        //睡眠5秒   等待上传头像完成
        //        setTimeout(function () { productDetailData.productDetail.otherPhotoUrl = "http://p1.zmjiudian.com/" + PhotoSURL + "_290x290"; $(".uploadlodding").hide(); }, 5000)
        //        //window.location = window.location;
        //        //productDetailData.productDetail.otherPhotoUrl = "http://p1.zmjiudian.com/" + PhotoSURL + "_290x290";
        //    },
        //    error: function (XMLHttpRequest, textStatus) {
        //        alert(XMLHttpRequest.responseText);
        //    }
        //});
    }
    //else //兼容IE
    //{
    //    var sFilter = 'filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale,src="';
    //    file.select();
    //    var src = document.selection.createRange().text;
    //    div.innerHTML = '<img id=imghead>';
    //    var img = document.getElementById('imghead');
    //    img.filters.item('DXImageTransform.Microsoft.AlphaImageLoader').src = src;
    //    var rect = clacImgZoomParam(MAXWIDTH, MAXHEIGHT, img.offsetWidth, img.offsetHeight);
    //    status = ('rect:' + rect.top + ',' + rect.left + ',' + rect.width + ',' + rect.height);
    //    div.innerHTML = "<div id=divhead style='width:" + rect.width + "px;height:" + rect.height + "px;margin-top:" + rect.top + "px;" + sFilter + src + "\"'></div>";
    //}
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