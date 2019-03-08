var pubuserid = parseInt($("#userid").val());
var isInWeixin = $("#isInWeixin").val() == "1";
var isVip = $("#isVip").val() == "1";
var _name = $("#_name").val();

var _Config = new Config();

function validateDates(dict, silent) {
	var d1 = Calendar.parse(dict.checkIn),
		d2 = Calendar.parse(dict.checkOut);
	if (!d1 || !d2) {
		silent || alert('输入日期格式有误');
		return false;
	}
	if (d1 >= d2) {
		silent || alert('入住时间应早于离店时间');
		return false;
	}
	return true;
}

//查看可售日
function lookSaleDate() {

    var _sellState = $(this).data("sellstate");
    var _calendarOptions = $(this).data('calendarOptions');

    //显示可售日日历
    var _showCalendar = function () {

        //隐藏套餐展示section
        $(".single-package .close-ctrl").click();

        showSpinner(true);
        $.getJSON(_calendarOptions, function (dict) {
            var calendar = new Calendar(function (cIn, cOut) {
                location.search = '?' + $.param({
                    checkin: Calendar.format(cIn),
                    checkout: Calendar.format(cOut)
                });
            }, dict);
            calendar.show();
        }).fail(function () {
            alert('网络请求失败，请稍后重试');
        }).always(function () {
            showSpinner(false);
        });
    }

    //_sellState == 3 不可售，显示预订，点击弹出日历选择日期
    if (_sellState == 3) {

        _Modal.show({
            title: '请重新选择入住日期',
            content: "当前选择日期不可预订",
            confirmText: '选择日期',
            confirm: function () {

                _showCalendar();
                _Modal.hide();
            },
            showCancel: true,
            cancel: function () {

                _Modal.hide();
            }
        });
    }
    else {

        _showCalendar();
    }
}

window.isMobile && $(function () {
	// main calendar
	var checkIn = Calendar.parse($('#checkIn').val());
	var checkOut = Calendar.parse($('#checkOut').val());
	var onSelect = function (newCheckIn, newCheckOut) {
		if (newCheckIn - checkIn || newCheckOut - checkOut) {
			location.search = '?' + $.param({
				checkIn: Calendar.format(newCheckIn),
				checkOut: Calendar.format(newCheckOut) 
			});
			$('<div class="spinner dark"><a></a></div>').appendTo(document.body).find('a').css('opacity', 0).animate({opacity: 1}, 400);
		}
	};
	var calendar = null;
	$('#dates').on('click', function () {
		calendar || (calendar = new Calendar(onSelect, window.calendarOptions));
		calendar.show();
		calendar.selectRange(checkIn, checkOut);
	});
	$('#m-dates').on('click', function () {
	    calendar || (calendar = new Calendar(onSelect, window.calendarOptions));
	    calendar.show();
	    calendar.selectRange(checkIn, checkOut);
	});

	// package calendar
	$('.btn.unavailable').click(lookSaleDate);
});

window.isMobile || $(function () {

	// main calendar
	Calendar.ctripTwins('#checkIn', '#checkOut', window.calendarOptions);

	$('#confirmChange').on('click', function () {
		var dict = {
			checkIn: $('#checkIn').val(),
			checkOut: $('#checkOut').val() 
		};
		if (validateDates(dict)) {
			showSpinner(true);
			location.search = '?' + $.param(dict);
		}
	});

	window.calendarOptions && !function () {
		var remarks = Calendar.getRemarks(calendarOptions);
		if (remarks.length) {
			remarks.unshift('');
			$('#remarks').text(remarks.join(' * '));
		}
	}();

	$('#dates').on('click', function () {

	    if (isInWeixin) {
	        alert("请在默认浏览器打开后进行支付");
	        return;
	    }
	});

    // package calendar（pc端的查看可售日弹出）
	lookSaleDate = function () {

        var _sellState = $(this).data("sellstate");
        var _calendarOptions = $(this).data('calendarOptions');

        //显示可售日日历
        var _showCalendar = function () {

            //隐藏套餐展示section
            $(".single-package .close-ctrl").click();

            showSpinner(true);
            var request = $.getJSON(_calendarOptions).done(function (data) {
                showSpinner(false);
                showDialog();
                initCalendar(data);
                $('#packageCheckIn').focus();
            }).fail(function () {
                alert('网络请求失败，请稍后重试');
            });
            var showDialog = $.proxy(bootbox, 'dialog', {
                title: "查看可售日",
                message: [
                    '<form class="form-inline" role="form">',
                    '<div class="form-group" style="margin-right:15px">',
                    '<label>入住时间</label>',
                    '<input id="packageCheckIn" type="text" class="form-control">',
                    '</div>',
                    '<div class="form-group" style="margin-right:15px">',
                    '<label>离店时间</label>',
                    '<input id="packageCheckOut" type="text" class="form-control">',
                    '</div>',
                    '</form>'
                ].join(''),
                buttons: {
                    main: {
                        label: "确定",
                        className: "btn-primary",
                        callback: function () {
                            var dict = {
                                checkIn: $('#packageCheckIn').val(),
                                checkOut: $('#packageCheckOut').val()
                            };
                            if (validateDates(dict)) {
                                showSpinner(true);
                                location.search = '?' + $.param(dict);
                            } else {
                                return false;
                            }
                        }
                    }
                },
                animate: false
            });
            var initCalendar = function (data) {
                Calendar.ctripTwins('#packageCheckIn', '#packageCheckOut', data);
            };
        }

        //_sellState == 3 不可售，显示预订，点击弹出日历选择日期
        if (_sellState == 3) {

            _Modal.show({
                title: '请重新选择入住日期',
                content: "当前选择日期不可预订",
                confirmText: '选择日期',
                confirm: function () {

                    _showCalendar();
                    _Modal.hide();
                },
                showCancel: true,
                cancel: function () {

                    _Modal.hide();
                }
            });
        }
        else {

            _showCalendar();
        }
	}
	$('.btn.unavailable').click(lookSaleDate);
});

showSpinner.prefetch();

//old套餐详情的折叠展开功能
$(".show-package-bar").each(function () {
    $(this).click(function () {
        var thisObj = $(this);
        var topHead = thisObj.parent().find(".panel-heading");
        setTimeout(function () {
            $("html,body").animate({ scrollTop: topHead.offset().top - 130 }, 500);
        }, 0);

        var topOne = thisObj.parent().find(".panel-body");
        var op = thisObj.data("op");
        if (op == "1") {
            thisObj.addClass("show-package-bar-bottom").removeClass("show-package-bar-top").data("op", "0");
            topOne.slideUp(200);
        }
        else {

            //set img'src
            var roomPics = topOne.find(".room-pics img");
            if (roomPics) {
                roomPics.each(function () {
                    setImgOriSrc($(this));
                });
            }

            thisObj.addClass("show-package-bar-top").removeClass("show-package-bar-bottom").data("op", "1");
            topOne.slideDown(200);
        }
        //setTimeout(function () {
        //    $("html,body").animate({ scrollTop: topHead.offset().top - 130 }, 300);
        //}, 200);
    });
});

//show more ota packages
var scrollMorePackageTimer = null;
$(".ota-morepackage").click(function () {
    var _thisObj = $(this);
    _thisObj.unbind("click");
    _thisObj.addClass("ota-morepackage-die").removeClass("ota-morepackage");
    $(".ota-packages").fadeIn(200);
    clearTimeout(scrollMorePackageTimer);
    scrollMorePackageTimer = setTimeout(function () {
        $("html,body").animate({ scrollTop: _thisObj.offset().top - 145 }, 500);
    }, 250);
});


//new 组内的套餐折叠展开功能
var packageGroupTimer = null;
$(".group-head").each(function () {

    var _thisObj = $(this);
    var _parentObj = _thisObj.parent();

    $(this).click(function () {

        var op = _thisObj.data("op");
        if (op == "1") {
            //_thisObj.addClass("show-morepackage-bar-bottom").removeClass("show-morepackage-bar-top").data("op", "0");
            _thisObj.data("op", "0");
            _thisObj.find(".arrow-icon-down").show();
            _thisObj.find(".arrow-icon-up").hide();
            _parentObj.find(".group-items").fadeOut(100);
        }
        else {
            //_thisObj.addClass("show-morepackage-bar-top").removeClass("show-morepackage-bar-bottom").data("op", "1");
            _thisObj.data("op", "1");
            _thisObj.find(".arrow-icon-up").show();
            _thisObj.find(".arrow-icon-down").hide();
            _parentObj.find(".group-items").fadeIn(300);
            
            clearTimeout(packageGroupTimer);
            packageGroupTimer = setTimeout(function () {

                var _setTop = _thisObj.offset().top - 40;
                if (_parentObj.find(".group-tip") && _parentObj.find(".group-tip").html()) {
                    _setTop = _thisObj.offset().top - 67;
                }

                $("html,body").animate({ scrollTop: _setTop }, 200);
            }, 310);
        }
    });

    //var _aupObj = _parentObj.find(".arrow-icon-up");
    //_aupObj.click(function () {

    //    //_thisObj.addClass("show-morepackage-bar-bottom").removeClass("show-morepackage-bar-top").data("op", "0");
    //    _thisObj.data("op", "0");
    //    _thisObj.find(".arrow-icon-down").show();
    //    _thisObj.find(".arrow-icon-up").hide();
    //    _parentObj.find(".group-items").fadeOut(100);

    //});
});

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        imgObj.attr("src", orisrc);
        imgObj.data("orisrc", "");
    }
};

$(function () {

    //如果有邀请好友得现金券tip，3秒后隐藏
    if ($(".shareTip")) {
        setTimeout(function () { $(".shareTip").slideUp(300); }, 3000);
    }

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //检测登录并自动登录
    if (pubuserid == 0) {
        _loginModular.verify.autoLogin(loginCheckFun);
    }

    //验证并直接跳转订单确认页面
    var checkGoBook = function (_pid, _gobuyurl) {

        //验证
        var _checkDic = { userid: pubuserid, pid: _pid };
        $.get('/hotel/CheckSubmitOrderBefore', _checkDic, function (_checkResult) {

            //console.log(_checkResult)

            if (_checkResult.ResultCode === 0) {
                location.href = _gobuyurl;
            }
            else if (_checkResult.ResponseResult) {
                if (_checkResult.ResponseResult.Text && _checkResult.ResponseResult.ActionUrl) {
                    _Modal.show({
                        title: '',
                        content: _checkResult.ResponseResult.Description,
                        confirmText: _checkResult.ResponseResult.Text,
                        confirm: function () {
                            goBuyVip();
                            _Modal.hide();
                        },
                        showCancel: true,
                        cancel: function () {
                            _Modal.hide();
                        }
                    });
                }
                else {
                    _Modal.show({
                        title: '',
                        content: _checkResult.ResponseResult.Description,
                        confirm: function () {
                            _Modal.hide();
                        }
                    });
                }

            }

        });
    }

    //预订
    var goBuy = function () {

        //隐藏套餐展示section
        $(".single-package .close-ctrl").click();

        var _this = $(this);
        var _gobuyurl = _this.data("bookurl");
        var _pid = _this.data("pid");
        var _price = _this.data("price");
        var _vipprice = _this.data("vipprice");

        if (pubuserid <= 0) {

            //酒店套餐页的预订操作，必须登录后才能去购买（去掉了 直接购买 功能） 2017.05.15 haoy
            _loginModular.show();

            //bootbox.confirm({
            //    message: "<div class='alert-rulesmsg'>登录后购买享受更多优惠，是否去登录？</div>",
            //    buttons: {
            //        confirm: { label: '马上登录', },
            //        cancel: { label: '直接购买', }
            //    },
            //    callback: function (result) {
            //        if (result) {
            //            _loginModular.show();
            //        } else {
            //            location.href = _gobuyurl;
            //        }
            //    },
            //    closeButton: true,
            //    onEscape: function () { }
            //});
        }
        else {

            if (!isVip) {

                var pcDic = { "orderTotalPrice": _price, "orderVipTotalPrice": _vipprice };
                $.get(_Config.APIUrl + "/api/coupon/BecomeVIPDiscountDescription", pcDic, function (_data) {

                    console.log(_data);
                    if (_data && _data.ActionUrl && _data.Description) {

                        _data.Description = _data.Description.replace(/1em/g, "1.5rem");
                        //alert(_data.Description);

                        _Modal.show({
                            title: '<b style="font-size:1.5rem;">此为VIP专享价哦～还不是VIP会员？</b>',
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
                                checkGoBook(_pid, _gobuyurl);
                            },
                            close: function () {
                                _Modal.hide();
                            }
                        });

                        $("._modal-section").css("top", "25%");
                    }
                    else {

                        checkGoBook(_pid, _gobuyurl);
                    }

                });
            }
            else {

                checkGoBook(_pid, _gobuyurl);
            }
        }
    }

    //绑定预订事件
    $(".btn-gobuy").each(function () {
        $(this).click(goBuy);
    });

    //询价
    var goAsk = function () {

        //隐藏套餐展示section
        $(".single-package .close-ctrl").click();

        var _this = $(this);
        var _gourl = _this.data("gourl");

        if (pubuserid <= 0) {

            bootbox.confirm({
                message: "<div class='alert-rulesmsg'>请您登录后进行询价哦</div>",
                buttons: {
                    confirm: { label: '马上登录', },
                    cancel: { label: '暂不询价', }
                },
                callback: function (result) {
                    if (result) {
                        _loginModular.show();
                    }
                },
                closeButton: false,
                onEscape: function () { }
            });
        }
        else {
            location.href = _gourl;
        }
    }

    //绑定询价事件
    $(".btn-goask").each(function () {
        $(this).click(goAsk);
    });

    //组内套餐click
    var _singleP = $(".single-package");
    var _singlePBG = $(".single-package-bg");
    var _singleClose = _singleP.find(".close-ctrl");
    $(".group-package-item").each(function () {

        var _thisObj = $(this);
        var _prow = _thisObj.find("._prow");
        _prow.click(function () {

            var _panelBody = _thisObj.find(".panel-body");
            var _pContent = _singleP.find(".p-content");

            var _pbHtml = _panelBody.html();
            _pContent.html(_pbHtml);

            //set img'src
            var roomPics = _pContent.find(".room-pics img");
            if (roomPics) {
                roomPics.each(function () {
                    setImgOriSrc($(this));
                });
            }

            /* build event */

            // package calendar
            _pContent.find('.unavailable').click(lookSaleDate);

            //go buy
            _pContent.find('.btn-gobuy').click(goBuy);

            //go ask price
            _pContent.find('.btn-goask').click(goAsk);

            /* set postion */
            var _pSection = _pContent.find(".p-section");
            var _buyCtrl = _pContent.find(".buy-ctrl");
            var _wwidth = $(window).width();
            var _wheight = $(window).height();
            if (_wwidth >= 600) {
                var _singleWidth = 600;
                var _left = (_wwidth - _singleWidth) / 2;
                _singleP.css("left", _left);

                _pSection.css("height", "97%");
                //_buyCtrl.hide();

                _singleP.fadeIn(300);
                _singlePBG.fadeIn(350);
            }
            else {
                _singleP.css("left", "4%");

                _wheight = _wheight / 100 * 94;
                _wheight = _wheight - 41;
                _pSection.css("height", _wheight);
                _buyCtrl.show();

                _singleP.show();
                _singlePBG.fadeIn(350);
            }
        });

    });

    //single-package close
    _singleClose.click(function () {
        _singlePBG.hide();
        _singleP.hide();
    });
    _singlePBG.click(function () {
        _singlePBG.hide();
        _singleP.hide();
    });

    //页面滚动事件
    $(window).scroll(function () {

        //头部的日期显示
        var _dateSeactionHeight = $(".mbdate").height();

        var m_st = Math.max(document.body.scrollTop || document.documentElement.scrollTop);
        if (m_st > _dateSeactionHeight - 40) {
            $(".m-fixed-date").fadeIn(200);
        } else {
            $(".m-fixed-date").hide();
            //$(".m-flow-date").hide();
        }
    });

});

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': _name, 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?userid=" + pubuserid + "&_isoneoff=1&_newpage=1";
}