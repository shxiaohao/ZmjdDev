var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";

//欢迎礼data
var newVipGiftData = null;

//常规活动gift
var activeGiftData = null;

var aid = 100929;
var userid = $("#userid").val();
var phone = $("#phone").val();
var albumId = $("#albumId").val();
var t = $("#t").val();
var userlat = $("#userlat").val();
var userlng = $("#userlng").val();
var districtId = 0;
var isApp = $("#isApp").val() == "1";
var isInWeixin = $("#isInWeixin").val() == "1";
var isVip = $("#isVip").val() == "1";
var isOldVip = $("#isOldVip").val() == "1";
var couponGiftActive = $("#couponGiftActive").val() == "1";
var magicallUrl = $("#magicallUrl").val();
var cashCouponUrl = $("#cashCouponUrl").val();
var shareNativeLink = $("#shareNativeLink").val();

//会员切换
var ctrlBottom = $(".ctrl-bottom");
var option199 = $(".option-199");
var table199 = $(".t-199");
var option599 = $(".option-599");
var table599 = $(".t-599");
var optionBg = $(".op-bg");
var goBuy199 = $(".go-buy-199");
var goBuy599 = $(".go-buy-599");
var upgrade599 = $(".upgrade599-buy");

var _Config = new Config();

//_Config.APIUrl = "http://api.zmjd100.com";

$(function () {

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //如果没有登录
    if (!parseInt(userid)) {

        //app环境通过logincallback的方式获取登录信息(android不做该处理。。。)
        if (isApp && !B.v.android) {
            var loginapphref2 = "whotelapp://loadJS?url=javascript:loginCallbackForRef('{userid}')";
            location.href = loginapphref2;
        }
        //非app环境，检测登录并自动登录
        else if (!isApp) {
            _loginModular.verify.autoLogin(loginCheckFun);
        }

    }

    var wwidth = $(window).width();

    //获取VIPbanner
    //type：0 普通广告，1 度假广告，2 首页头广告，3 VIP专区广告
    var _randombannersDic = {"type":3, "curUserID":userid};
    $.get(_Config.APIUrl + "/api/hotel/GetHomeOnlineBannersByType", _randombannersDic, function (_data) {

        if (_data.AD && _data.AD.ADList && _data.AD.ADList.length) {

            //console.log(_data.AD.ADList[0]);

            var _imgSrc = _data.AD.ADList[0].ADURL;
            $(".vip-banner-img").attr("src", _imgSrc);

            var _actionUrl = _data.AD.ADList[0].ActionURL;
            if (_actionUrl && _actionUrl.indexOf("http") >= 0) {
                $(".vip-banner-img").click(function () {
                    gourl(_actionUrl);
                });
            }
        }
    });

    //头部banner滚起来
    //动态设置top banner的具体宽度
    $(".vip-top-banner .vip-banner-item").css("width", wwidth);
    $(".vip-top-banner").fadeIn(200);

    //Banners
    //$('.vip-top-banner').swiper({
    //    slidesPerView: 'auto',
    //    loop: true,
    //    pagination: '.pagination',
    //    paginationHide: false,
    //    offsetPxBefore: 0,
    //    offsetPxAfter: 0,
    //    onTouchEnd: function (slider) {
    //        if (slider.activeIndex < slider.slides.length) {
    //            var li = $(slider.slides[slider.activeIndex]);
    //            var imgObj = li.find("img");
    //            //setImgOriSrc(imgObj);
    //        }
    //    }
    //})

    option199.click(function () {
        var _sel = option199.data("sel");
        if (_sel == "0") {
            option199.addClass("sel");
            option199.data("sel", "1");
            option599.removeClass("sel");
            option599.data("sel", "0");
            optionBg.animate({ left: "-2px" }, 0);

            //table199.fadeIn(500);
            //table599.hide();

            goBuy199.show();
            goBuy599.hide();

            aid = $("#aid199").val();
        }
    });
    option599.click(function () {
        var _sel = option599.data("sel");
        if (_sel == "0") {
            option599.addClass("sel");
            option599.data("sel", "1");
            option199.removeClass("sel");
            option199.data("sel", "0");
            optionBg.animate({ left: "50%" }, 0);

            //table599.fadeIn(500);
            //table199.hide();

            goBuy599.show();
            goBuy199.hide();

            aid = $("#aid599").val();
        }
    });

    goBuy199.click(function () {

        if (userid == "0") {
            
            if (isApp) {
                location.href = loginapphref;
                return;
            }
        }

        showModal();
        //var _aid = $("#aid199").val();
        //goBuy(_aid);
    });
    goBuy599.click(function () {

        if (userid == "0") {

            if (isApp) {
                location.href = loginapphref;
                return;
            }
        }

        showModal();
        //var _aid = $("#aid599").val();
        //goBuy(_aid);
    });
    upgrade599.click(function () {

        aid = $("#aid599").val();

        if (userid == "0") {

            if (isApp) {
                location.href = loginapphref;
                return;
            }
        }

        showModal();
        //var _aid = $("#aid599").val();
        //goBuy(_aid);
    });

    //手机验证窗口中的购买
    _modalBuy.click(function () {

        _hideModalErr();

        var realname = $("#userName").val().replace(" ", "");
        if (realname.length == 0) {
            _showModalErr("请如实填写您的姓名");
            //alert("请如实填写您的姓名");
            return;
        }

        if (userid == "0") {

            //手机号码输入验证
            var userPhone = "";
            var vCode = "";
            if (userid == "0") {
                userPhone = $("#userPhone").val();
                if (!isMobile(userPhone)) {
                    _showModalErr("请输入有效的手机号");
                    //alert("请输入有效的手机号");
                    $("#userPhone").focus();
                    return;
                }

                vCode = $("#vCode").val();
                if (vCode == "") {
                    _showModalErr("请输入短信验证码");
                    //alert("请输入短信验证码");
                    $("#vCode").focus();
                    return;
                }
            }

            $.post(verifyNewUserUrl, {
                action: 'check',
                number: userPhone,
                code: vCode,
                CID: $("#hidCurUserCID").val()
            }, 'json').then(function (r) {
                if (r.ok == "1") {
                    goBuy(r.userid, realname);
                }
                else {
                    _showModalErr("短信验证码有误");
                    //alert("短信验证码有误");
                }
            });
        }
        else {
            goBuy(userid, realname);
        }

    });

    //续费功能
    var _renewVipBtn = $(".renew-vip-btn");
    _renewVipBtn.click(function () {

        var _aid = $(this).data("aid");
        aid = _aid;
        goBuy(userid, "");
    });

    //验证码操作
    $(".vCodeBtn").click(function () {

        _hideModalErr();

        var userPhone = $("#userPhone").val();
        if (!isMobile(userPhone)) {
            _showModalErr("请输入有效的手机号");
            //alert('请输入有效的手机号');
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
                    _showModalErr("系统错误");
                    //alert('系统错误');
                }
            }).fail(function () {
                self._stopTimer(lock);
                _showModalErr("网络请求失败");
                //alert('网络请求失败');
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

    //头部权益的点击跳转
    //免费福利
    $(".vip-top-info-free").click(function () {
        $("html,body").animate({ scrollTop: $("#vip-zx-free").offset().top + 6 }, 300);
    });

    //专享优惠
    $(".vip-top-info-zhuanxiang").click(function () {
        $("html,body").animate({ scrollTop: $("#vip-zx-youhui").offset().top + 6 }, 300);
    });

    //积分专区
    $(".vip-top-info-point").click(function () {
        $("html,body").animate({ scrollTop: $("#vip-zx-point").offset().top + 6 }, 300);
    });

    ////专享服务
    //$(".vip-top-info-call").click(function () {
    //    $("html,body").animate({ scrollTop: $("#vip-zx-fuwu").offset().top + 6 }, 300);
    //});

    var _showWelcomeList = function () {
        //$(".welcome-list").css("top", $(window).scrollTop() + 15);
        $(".welcome-list").fadeIn(200);
        $(".welcome-list-model").show();
    }
    var _hideWelcomeList = function () {
        $(".welcome-list").hide();
        $(".welcome-list-model").hide();
    }

    //弹出大红包
    var _showBigRedpack = function () {

        $(".big-redpack").fadeIn();
        $(".big-redpack-model").show();

        $(".big-redpack .pack").click(function () {
            _showWelcomeList();
            _hideBigRedpack();
        });

        $(".big-redpack .close").click(function () {
            _hideBigRedpack();
        });

        $(".big-redpack-model").click(function () {
            _hideBigRedpack();
        });
    }

    var _hideBigRedpack = function () {

        $(".big-redpack").hide();
        $(".big-redpack-model").hide();
    }

    //加载 新VIP欢迎礼
    var loadNewVipGift = function () {

        var getCouponListApi = _Config.APIUrl + "/api/coupon/GetNewVIPGiftUserCouponList";
        $.get(getCouponListApi, {}, function (_data) {

            if (_data) {

                if (newVipGiftData) {
                    newVipGiftData.AlbumsInfo = _data;
                }
                else {
                    newVipGiftData = new Vue({
                        el: '#newvip-p-list',
                        data: { "AlbumsInfo": _data }
                    })
                }

                //动态计算总的现金券价值
                var _sumCouponDiscount = 0;
                _data.map(function (item, index) {
                    if (item.DiscountAmount) {
                        _sumCouponDiscount += item.DiscountAmount;
                    }
                });
                
                if (_sumCouponDiscount) {
                    $("#gift-coupon-discount").html("价值¥" + _sumCouponDiscount);
                }

                //刷新现金券状态
                refCashGenState();

                $(".newvip-list2").fadeIn(200);

                //让头菜单支持横向滑动
                try {
                    var newvipProductListScroll = new IScroll('#newvip-p-list', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
                } catch (e) {

                }

                //领取欢迎礼相关事件
                $("#welcome-banner").click(function () {
                    _showWelcomeList();
                });

                $(".welcome-list .close").click(function () {
                    _hideWelcomeList();
                });
                $(".welcome-list-model").click(function () {
                    _hideWelcomeList();
                });

                Vue.nextTick(function () {

                    if (isVip) {

                        //显示已领取，隐藏领取
                        if ($(".geted-welcome-coupon")) $(".geted-welcome-coupon").show();
                        if ($(".get-welcome-coupon")) $(".get-welcome-coupon").hide();

                        if ($(".redpack-banner-ok")) $(".redpack-banner-ok").show(); //显示已领取去查看
                        if ($(".redpack-banner")) $(".redpack-banner").hide();
                    }
                    else {

                        //隐藏已领取，显示领取
                        if ($(".geted-welcome-coupon")) $(".geted-welcome-coupon").hide();
                        if ($(".get-welcome-coupon")) $(".get-welcome-coupon").show();

                        if ($(".redpack-banner-ok")) $(".redpack-banner-ok").hide();
                        if ($(".redpack-banner")) $(".redpack-banner").show(); //显示成为VIP去领取

                        ////跳出大红包
                        //_showBigRedpack();
                    }

                    //去领券功能
                    $(".get-welcome-coupon").click(function () {

                        _hideWelcomeList();

                        if (userid == "0") {

                            if (isApp) {
                                location.href = loginapphref;
                                return;
                            }
                        }

                        showModal(true);
                    });

                    //续费领券功能
                    $(".renew-welcome-coupon").click(function () {

                        _hideWelcomeList();

                        if (userid == "0") {

                            if (isApp) {
                                location.href = loginapphref;
                                return;
                            }
                        }

                        var _aid = $(this).data("aid");
                        aid = _aid;
                        goBuy(userid, "");
                    });

                    //$(".get-welcome-coupon").each(function () {

                    //    $(this).click(function () {

                    //        _hideWelcomeList();

                    //        if (userid == "0") {

                    //            if (isApp) {
                    //                location.href = loginapphref;
                    //                return;
                    //            }
                    //        }

                    //        showModal();
                    //    });
                    //});
                });
            }
        });
    }
    loadNewVipGift();

    //展开四大权益
    $(".open-vip-quanyi").click(function () {

        $(".project .items").fadeIn();
        $(".project .tit .open-icon").addClass("short-line");
        $(".project .tit .short-line").removeClass("open-icon");
        $(".project .tit .short-line").html("");
    });

    //加载 常规节日活动礼
    var loadHolidayGift = function () {

        var getCouponListApi = _Config.APIUrl + "/api/coupon/GetVIPCouponGiftUserCouponList";
        $.get(getCouponListApi, {}, function (_data) {

            console.log(_data);

            if (_data) {

                if (activeGiftData) {
                    activeGiftData.AlbumsInfo = _data;
                }
                else {
                    activeGiftData = new Vue({
                        el: '#gift-coupon-list',
                        data: { "AlbumsInfo": _data }
                    })
                }

                //领取欢迎礼相关事件
                $("#gift-coupon-ball").click(function () {
                    _showGiftCouponList();

                    //隐藏活动浮窗
                    $(".gift-coupon-ball").hide();
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

                Vue.nextTick(function () {

                    //检查当前用户是否领取活动券（之前是只有VIP才会验证，现在是放开所有用户都可以领取了 2018.07.16 haoy）
                    var _checkUserGetGiftState = function () {
                        $.get(_Config.APIUrl + "/api/coupon/IsUserHasGetNewVIPGift", { userid: userid }, function (_checkData) {

                            if (_checkData) {

                                //隐藏活动红包
                                _hideGiftCouponList();

                                //弹出活动浮窗
                                $(".gift-coupon-ball").fadeIn();

                                //显示已领取，隐藏领取
                                $(".go-use-coupon").show();
                                $(".holiday-gift-geted").show();
                                $(".get-gift-coupon").hide();
                            }
                            else {

                                //弹出活动红包
                                _showGiftCouponList();

                                //隐藏已领取，显示领取
                                $(".go-use-coupon").hide();
                                $(".holiday-gift-geted").hide();
                                $(".get-gift-coupon").show();
                            }
                        });
                    }
                    _checkUserGetGiftState();

                    //分享领券事件
                    var _getGiftCouponEvent = function () {

                        //需登录
                        var loginapphref_holidaygift = "whotelapp://loadJS?url=javascript:loginCallbackForHolidayGift('{userid}')&realuserid=1";
                        if (!parseInt(userid)) {

                            if (isApp) {
                                location.href = loginapphref_holidaygift;
                                return;
                            }
                            else {
                                _loginModular.show();
                            }
                            return;
                        }

                        goShareAndGetGift(userid);
                    }
                    $(".get-gift-coupon").click(_getGiftCouponEvent);
                });
            }
        });
    }

    if (couponGiftActive) {
        loadHolidayGift();
    }

    //加载 会员免费福利
    var vipFreeProductData = null;
    var vipFreeProductLock = true;
    var loadVipFreeProduct = function () {

        if (vipFreeProductData) return;

        if (vipFreeProductLock) {
            vipFreeProductLock = false;

            var pcDic = { "albumid": 9, "userid": userid, "start": 0, "count": 5, "v": 1 };
            $.get(_Config.APIUrl + "/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID", pcDic, function (_data) {

                //console.log("加载 会员免费福利")
                //console.log(_data)

                var _totalMarketPrice = 0;

                if (_data && _data.SKUCouponList && _data.SKUCouponList.length > 0) {
                    _data.SKUCouponList.map(function (item, index) {
                        if (!item.PicList || item.PicList.length <= 0) {
                            item.PicUrl = "/content/images/seat/img-viparea-item-3x2.png";
                        }
                        else {
                            item.PicUrl = item.PicList[0];
                        }

                        _totalMarketPrice += item.SKUMarketPrice;

                    });

                    _data["albumTitle"] = "会员免费福利";
                    _data["albumId"] = 9;
                    _data["userid"] = userid;
                    _data["totalCount"] = _data.SKUCouponList.length;
                    _data["totalMarketPrice"] = _totalMarketPrice;

                    //$("#freeMarketPrice").text(_totalMarketPrice);

                    //是否需要横滑逐个加载图片
                    _data["stepLoadImg"] = true;

                    if (vipFreeProductData) {
                        _data.stepLoadImg = false;
                        vipFreeProductData.AlbumsInfo = _data;
                    }
                    else {
                        vipFreeProductData = new Vue({
                            el: '#vip-free-section',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    //更新汇总信息
                    refVipFreeProductPrice(_data["albumId"]);

                    $("#vip-free-list").fadeIn(200);
                }
                $("#vip-free-seat").hide();

                vipFreeProductLock = true;
            });
        }

    }
    loadVipFreeProduct();

    //更新专辑的汇总提示（X项福利价值XXX元）
    var refVipFreeProductPrice = function (_albumId) {

        var _dic = { albumId: _albumId };
        $.get(_Config.APIUrl + "/api/coupon/GetProductAlbumSummary", _dic, function (_result) {
            if (_result && _result.SumMarketPrice && _result.SumCount) {
                vipFreeProductData.AlbumsInfo.totalCount = _result.SumCount;
                vipFreeProductData.AlbumsInfo.totalMarketPrice = _result.SumMarketPrice;
                $("#freeMarketPrice").text(_result.SumMarketPrice);
            }
        });

    }

    //加载 积分商城
    var pointProductData = null;
    var pointProductLock = true;
    var loadPointProduct = function () {

        if (pointProductData) return;

        if (pointProductLock) {
            pointProductLock = false;

            var pcDic = { "albumid": 22, "userid": userid, "start": 0, "count": 5, "v": 1 };
            $.get(_Config.APIUrl + "/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID", pcDic, function (_data) {

                //console.log("加载 会员积分商城")
                //console.log(_data)

                var _totalMarketPrice = 0;

                if (_data && _data.SKUCouponList && _data.SKUCouponList.length > 0) {
                    _data.SKUCouponList.map(function (item, index) {
                        if (!item.PicList || item.PicList.length <= 0) {
                            item.PicUrl = "/content/images/seat/img-viparea-item-3x2.png";
                        }
                        else {
                            item.PicUrl = item.PicList[0];
                        }

                        _totalMarketPrice += item.SKUMarketPrice;

                    });

                    _data["albumTitle"] = "会员积分商城";
                    _data["albumId"] = 9;
                    _data["userid"] = userid;
                    _data["totalCount"] = _data.SKUCouponList.length;
                    _data["totalMarketPrice"] = _totalMarketPrice;

                    //是否需要横滑逐个加载图片
                    _data["stepLoadImg"] = true;

                    if (pointProductData) {
                        _data.stepLoadImg = false;
                        pointProductData.AlbumsInfo = _data;
                    }
                    else {
                        pointProductData = new Vue({
                            el: '#vip-point-section',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    $("#vip-point-list").fadeIn(200);
                }
                $("#vip-point-seat").hide();

                pointProductLock = true;
            });
        }

    }
    //loadPointProduct();

    //加载 会员专享服务【酒店】
    var vipHotelProductData = null;
    var vipHotelProductLock = true;
    var loadVipHotelProduct = function () {

        if (vipHotelProductData) return;

        if (vipHotelProductLock) {
            vipHotelProductLock = false;

            var pcDic = { "albumId": 10, "start": 0, "count": 6, "curUserID": userid, "ckvip": 0, "v": 1 };
            $.get(_Config.APIUrl + "/api/hotel/GetRecommendHotelResultByAlbumId", pcDic, function (_data) {

                //console.log("加载 酒店")
                //console.log(_data)

                if (_data && _data.HotelList && _data.HotelList.length > 0) {
                    
                    if (_data && _data.HotelList) {
                        _data.HotelList.map(function (item, index) {
                            if (!item.HotelPicUrl) {
                                item.HotelPicUrl = "./content/images/seat/home-hotel-load-3x2.png";
                            }
                        });
                    }

                    _data["albumTitle"] = "酒店";
                    _data["albumId"] = 10;
                    _data["userid"] = userid;

                    //是否需要横滑逐个加载图片
                    _data["stepLoadImg"] = true;

                    if (vipHotelProductData) {
                        _data.stepLoadImg = false;
                        vipHotelProductData.AlbumsInfo = _data;
                    }
                    else {
                        vipHotelProductData = new Vue({
                            el: '#vip-hotel-section',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    $("#vip-hotel-list").fadeIn(200);
                }
                $("#vip-hotel-seat").hide();

                vipHotelProductLock = true;
            });
        }
    }
    //loadVipHotelProduct();

    //加载 会员专享服务【玩乐】
    var vipPlayProductData = null;
    var vipPlayProductLock = true;
    var loadVipPlayProduct = function () {

        if (vipPlayProductData) return;

        if (vipPlayProductLock) {
            vipPlayProductLock = false;

            var pcDic = { "albumid": 3, "userid": userid, "start": 0, "count": 5, "v": 1 };
            $.get(_Config.APIUrl + "/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID", pcDic, function (_data) {

                //console.log("加载 玩乐")
                //console.log(_data)

                if (_data && _data.SKUCouponList && _data.SKUCouponList.length > 0) {
                    _data.SKUCouponList.map(function (item, index) {
                        if (!item.PicList || item.PicList.length <= 0) {
                            item.PicUrl = "/content/images/seat/img-viparea-item-3x2.png";
                        }
                        else {
                            item.PicUrl = item.PicList[0];
                        }
                    });

                    _data["albumTitle"] = "玩乐";
                    _data["albumId"] = 3;
                    _data["userid"] = userid;

                    //是否需要横滑逐个加载图片
                    _data["stepLoadImg"] = true;

                    if (vipPlayProductData) {
                        _data.stepLoadImg = false;
                        vipPlayProductData.AlbumsInfo = _data;
                    }
                    else {
                        vipPlayProductData = new Vue({
                            el: '#vip-play-section',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    $("#vip-play-list").fadeIn(200);
                }
                $("#vip-play-seat").hide();

                vipPlayProductLock = true;
            });
        }
    }
    //loadVipPlayProduct();

    //加载 会员专享服务【美食】
    var vipFoodProductData = null;
    var vipFoodProductLock = true;
    var loadVipFoodProduct = function () {

        if (vipFoodProductData) return;

        if (vipFoodProductLock) {
            vipFoodProductLock = false;

            var pcDic = { "albumid": 2, "userid": userid, "start": 0, "count": 5, "v": 1 };
            $.get(_Config.APIUrl + "/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID", pcDic, function (_data) {

                //console.log("加载 美食")
                //console.log(_data)

                if (_data && _data.SKUCouponList && _data.SKUCouponList.length > 0) {
                    _data.SKUCouponList.map(function (item, index) {
                        if (!item.PicList || item.PicList.length <= 0) {
                            item.PicUrl = "/content/images/seat/img-viparea-item-3x2.png";
                        }
                        else {
                            item.PicUrl = item.PicList[0];
                        }
                    });

                    _data["albumTitle"] = "美食";
                    _data["albumId"] = 2;
                    _data["userid"] = userid;

                    //是否需要横滑逐个加载图片
                    _data["stepLoadImg"] = true;

                    if (vipFoodProductData) {
                        _data.stepLoadImg = false;
                        vipFoodProductData.AlbumsInfo = _data;
                    }
                    else {
                        vipFoodProductData = new Vue({
                            el: '#vip-food-section',
                            data: { "AlbumsInfo": _data }
                        })
                    }

                    $("#vip-food-list").fadeIn(200);
                }
                $("#vip-food-seat").hide();

                vipFoodProductLock = true;
            });
        }
    }
    //loadVipFoodProduct();

    //会员专享服务 尾单弹出窗
    $(".vip-top-info-usergroup").click(function () {

        var showContent = "";
        if (isApp) {
            showContent += "复制微信号，打开微信添加“<b>周大玉</b>”为好友，回复“<b>尾单群</b>”，周大玉会加你入群，第一时间获取超值尾单福利。";
            showContent += "<br /><br /><center><b style='-webkit-user-select: text;user-select: text;'>shanglv018</b></center>";

            _Modal.show({
                title: '加入福利群',
                content: showContent,
                confirmText: '复制微信号',
                confirm: function () {
                    _Modal.hide();

                    //调用app复制功能
                    zmjd.copyTxt("shanglv018");

                    alert("已复制");
                },
                showCancel: false,
                showClose: true
            });
        }
        else {
            showContent += "长按下方二维码，添加“<b>周大玉</b>”为好友，回复“<b>尾单群</b>”，周大玉会加你入群，第一时间获取超值尾单福利。";
            showContent += "<br /><br /><center><img style='width:8em;' src='http://whfront.b0.upaiyun.com/app/img/coupon/vipshopinfo/zhouxiaomei-qrcode-img.jpg' alt='微信号：shanglv018'></center>";

            _Modal.show({
                title: '加入福利群',
                content: showContent,
                confirmText: '知道了',
                confirm: function () {
                    _Modal.hide();
                },
                showCancel: false,
                showClose: true
            });
        }

        $("._modal-section").css("top", "16%");
    });

    var $win = $(window);
    var isload = true;

    var _scrollEvent = function () {
        var winTop = $win.scrollTop();
        var winHeight = $win.height();

        //会员免费福利
        var vipFreeTop = $("#vip-free-section").offset().top;

        //专享 酒店
        var vipHotelTop = $("#vip-hotel-section").offset().top;

        //专享 玩乐
        var vipPlayTop = $("#vip-play-section").offset().top;

        //专享 美食
        var vipFoodTop = $("#vip-food-section").offset().top;

        if (winTop > 0 && winTop > 100) {
            loadPointProduct();
            loadVipHotelProduct();
            loadVipFoodProduct();
            loadVipPlayProduct();
        }

        //if (winTop >= vipFoodTop - winHeight - 150) {
        //    loadVipFoodProduct();
        //}
        //else if (winTop >= vipPlayTop - winHeight - 150) {
        //    loadVipPlayProduct();
        //}
        //else if (winTop >= vipHotelTop - winHeight - 150) {
        //    loadVipHotelProduct();
        //}
    }
    _scrollEvent();

    //页面滚动事件
    $win.on('scroll', _scrollEvent);

    //点我了解“铂金VIP”
    $(".b-open-tit").click(function () {
        $(".b-open-tit").hide();
        $(".b-open-group").slideDown(200);
        setTimeout(function () {
            $("html,body").animate({ scrollTop: $(".b-open-group").offset().top + 8 }, 300);
        }, 200);
    });

    //初始加载
    var init = function () {

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
    }
    init();
});

var _showGiftCouponList = function () {
    //$(".welcome-list").css("top", $(window).scrollTop() + 15);
    $(".gift-coupon-list").fadeIn(200);
    $(".gift-coupon-list-model").show();
}
var _hideGiftCouponList = function () {
    $(".gift-coupon-list").hide();
    $(".gift-coupon-list-model").hide();
}

//领取节日活动券 登录回调
function loginCallbackForHolidayGift(_userid) {
    goShareAndGetGift(_userid);
}

//1 去分享并领取节日活动券
var goShareAndGetGift = function (_userid) {

    if (isApp) {

        //app内默认6秒钟后自动领取
        setTimeout(function () {

            //领取并弹出
            genHolidayGift(_userid);

        }, 6000);

        //app内弹出分享
        gourl(shareNativeLink);
    }
    else if (1) {

        //微信内默认6秒钟后自动领取
        setTimeout(function () {

            //领取并弹出
            genHolidayGift(_userid);

        }, 6000);

        //弹出微信分享提示
        $(".weixin-share-tip").show();

        ////隐藏gift券弹窗
        //_hideGiftCouponList();

        ////弹出活动浮窗
        //$(".gift-coupon-ball").fadeIn();
    }
    else {
        alert("请至周末酒店APP或微信中领取哦~");
        return;
    }

    /************* 现在是放开所有用户都可以领取了 2018.07.16 haoy ************/
    ////VIP可领取
    //if (isVip) {

    //    //去领取的代码....
    //}
    //else {

    //    _hideGiftCouponList();

    //    //弹出活动浮窗
    //    $(".gift-coupon-ball").fadeIn();

    //    if (userid == "0") {

    //        if (isApp) {
    //            location.href = loginapphref;
    //            return;
    //        }
    //    }

    //    showModal(true);
    //}
    /**************************************************************************/
}

//2 领取节日活动券
var genHolidayGift = function (_userid) {

    $.get(_Config.APIUrl + "/api/coupon/SendVIPCouponActitity", { userid: _userid }, function (_getData) {

        if (_getData) {

            if (isApp) {
                alert("恭喜你已成功领取现金券福利，礼券已发放至你的钱包，稍后可以到“钱包->现金券”中查看");
            }
            else {
                alert("恭喜你已成功领取现金券福利，礼券已发放至你的钱包，稍后可以到“我的->我的券->现金券”中查看");
            }

            //显示已领取，隐藏领取
            $(".go-use-coupon").show();
            $(".holiday-gift-geted").show();
            $(".get-gift-coupon").hide();
        }
        else {
            //隐藏已领取，显示领取
            $(".go-use-coupon").hide();
            $(".holiday-gift-geted").hide();
            $(".get-gift-coupon").show();
        }
    });
}

function loginCallbackForRef(_userid) {
    if (parseInt(_userid)) {
        location.href = "/Coupon/VipAreaInfo?userid=" + _userid;
    }
}

//app环境领取现金券的登录回调
function loginCallbackForCash(_userid) {

    if (parseInt(_userid)) {
        location.href = "/Coupon/VipAreaInfo?userid=" + _userid;
        return;
    }

    //如果是老vip，则刷新页面
    if (isVip && isOldVip) {
        location.reload();
        return;
    }

    try {
        //如果是app环境，在VIP购买成功后向app标记用户信息产品了变更
        zmjd.userinfoChanged();
    } catch (e) {

    }

    genCashCoupon(_userid);
}

//更新欢迎礼-300现金券的领取状态
var refCashGenState = function () {

    var _dic = { userid: userid, type: 17 };
    $.get(_Config.APIUrl + "/api/activity/GetOrgCouponCount", _dic, function (_result) {
        if (_result && parseInt(_result)) {
            newVipGiftData.AlbumsInfo.CashGenCount = parseInt(_result);
        }
    });

}

//领取欢迎礼现金券
var genCashCoupon = function (_userid) {

    //alert(_userid);

    var _dic = { userid: _userid, phone: phone };
    $.get('/coupon/GiftCashCoupon', _dic, function (_result) {

        if (_result) {

            //刷新现金券状态
            refCashGenState();

            if (_result.Success === 0) {

                _Modal.show({
                    title: '领取成功',
                    content: '你已成功领取¥300元现金券，可用于抵扣酒店度假产品的消费，请到周末酒店app“我的”->“钱包”->“现金券”内查看详情',
                    confirmText: '立即查看',
                    confirm: function () {
                        gourl(cashCouponUrl);
                        _Modal.hide();
                    },
                    showCancel: true,
                    cancelText: '确定',
                    cancel: function () {
                        _Modal.hide();
                    }
                });

            }
            else {
                _Modal.show({
                    title: '',
                    content: _result.Message,
                    confirmText: '确定',
                    confirm: function () {
                        _Modal.hide();
                    },
                    showCancel: false
                });
            }

        }
        else {
            _Modal.show({
                title: '',
                content: '抱歉，领取异常',
                confirmText: '确定',
                confirm: function () {
                    _Modal.hide();
                },
                showCancel: false
            });
        }

    });

}

var openMagiCall = function () {
    
    if (isApp) {
        gourl("whotelapp://www.zmjiudian.com/MagiCall");
    }
    else {
        gourl("http://www.zmjiudian.com/Account/WxMenuTransfer?menu=4");
    }

}

var goVipHotelList = function () {
    $("html,body").animate({ scrollTop: $(".more-packages-tit").offset().top }, 300);
}

//弹出手机号/真实姓名验证窗口
var _modalBg = $("._modal-bg");
var _modalPanel = $("._modal-panel");
var _modalPanelClose = $("._modal-panel ._close");
var _modalErr = $("._modal-panel ._body ._err");
var _modalBuy = $("._modal-panel ._btns ._buy");
var _vipCouponTip = $(".vip-coupon-tip");

//show
var showModal = function (showTip) {

    console.log($(window).scrollTop());

    _modalPanel.css("top", $(window).scrollTop() + 100);

    ctrlBottom.hide();
    _modalBg.show();
    _modalPanel.show();

    if (showTip) {
        _vipCouponTip.show();
    }
    else {
        _vipCouponTip.hide();
    }
}

//hide
var hideModal = function () {
    ctrlBottom.show();
    _hideModalErr();
    _modalBg.hide();
    _modalPanel.hide();
}

//close modal
_modalPanelClose.click(hideModal);
_modalBg.click(hideModal);

var _showModalErr = function (msg) {
    _modalErr.html(msg);
    _modalErr.fadeIn();
}
var _hideModalErr = function () {
    _modalErr.html("");
    _modalErr.hide();
}

//购买支付
var goBuy = function (userid, realname) {

    var subdic = {};
    subdic["aid"] = aid;
    subdic["atype"] = $("#atype").val();
    subdic["pid"] = $("#pid").val();
    subdic["pricetype"] = $("#pricetype").val();
    var sellnum = $("#sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
    subdic["paynum"] = sellnum;
    subdic["userid"] = userid;
    subdic["realname"] = realname;

    if (!subdic["userid"] || subdic["userid"] == "0") {
        alert("非法请求");
        return;
    }

    $.get('/Coupon/SubmitVipConpon', subdic, function (content) {
        var msg = content.Message;
        var suc = content.Success;
        var url = content.Url;

        switch (suc) {
            case "0":
                {
                    hideModal();
                    location = url;
                    break;
                }
            default:
                {
                    alert(msg);
                    break;
                }
        }
    });
}

//app环境下购买VIP的登录回调
function loginCallback(_userid) {

    if (parseInt(_userid)) {
        location.href = "/Coupon/VipAreaInfo?userid=" + _userid;
        return;
    }

    //处理输入姓名和手机号的弹窗模式
    if (_userid > 0) {

        userid = _userid;

        $.get('/Coupon/GetUserPhone', { userid: _userid }, function (content) {
            if (content) {
                $(".vcode-line").hide();
                $(".phone-line").addClass("onlyone-line");
                $(".vcode-line").hide();

                $("#userPhone").val(content);
                $("#userPhone").addClass("userPhone2").removeClass("userPhone");
                $("#userPhone").attr("disabled", "disabled");

                showModal();
            }
            else {
                alert("登录错误，请退出该页面后重试")
            }
        });
    }

    //gosubmit(userid);
    //location.reload();
    //location.replace(location.pathname + "?userid=" + userid);
}

function goto(param) {
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (!isApp) {
        url = "http://www.zmjiudian.com/" + param;
    }

    this.location = url;
}

function gotopage(param) {
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    if (!isApp) {
        url = "http://www.zmjiudian.com/" + param;
    }
    this.location = url;
}

function gourl(url) {
    location.href = url;
}

var _showCity = false;
var showCity = function () {
    if (!_showCity) {
        showCityFun();
    }
    else {
        hideCityFun();
    }
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    if (!IsThanVer5_8) {

        var _content = "从即日开始，周末酒店APP仅在5.8及以上版本支持使用现金券抵扣消费功能。 <br />您当前使用的版本不支持该功能，快去更新吧~";
        if (IsIos) {
            _content += "<br /><br /><center>(请前往App Store进行更新)</center>";
        }
        else if (IsAndroid) {
            _content += "<br /><br /><center>(请前往各大应用市场进行更新)</center>";
        }

        _Modal.show({
            title: '重要提示',
            content: _content,
            confirmText: '知道了',
            confirm: function () {
                _Modal.hide();
            },
            showCancel: false
        });
        //alert("从即日开始，仅APP版本号5.8及以上支持使用现金券抵扣消费。 您当前使用的版本不支持改功能，快去更新吧~");
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