var pubuserid = $("#userid").val();
var isVip = $("#isVip").val() == "1";
var pid = $("#pid").val();
var checkIn = $("#checkIn").val();
var checkOut = $("#checkOut").val();
var maxPersonNum = parseInt($("#maxPersonNum").val());
var minPersonNum = parseInt($("#minPersonNum").val());
var cartTypeListStr = $("#cartTypeList").val();
var cartTypeList = cartTypeListStr.split(',');
var travelPersonDesc = $("#travelPersonDesc").val();
var isInWeixin = $("#isInWeixin").val() == "1";
var canUseCoupon = $("#canUseCoupon").val() == "1";
var userFundAmount = parseInt($("#userFundAmount").val());
var packageInChina = $("#packageInChina").val() == "1";
var isNeedDocInfo = $("#isNeedDocInfo").val() == "1";

var vipTipVue = null;
var userFundVue = null;
var cashCouponVue = null;
var orderPayMoneyVue = null;

var _Config = new Config();

//_Config.APIUrl = "http://api.zmjd100.com";

var fundSelectEvent = function () { };

$(function () {

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

	var amount = {
		init: function () {
			this._buttons = $('#amount .btn');
			this._input = $('#amount input');
			this._label = $('#amount-label');
			this._price = $('#price strong');
			this._useCash = ($("#canUseCashCoupon").val() == "1");
			
			this._buttons.click($.proxy(function (e) {
				var delta = this._buttons.index(e.target) == 0 ? -1 : 1;
				this.set(this.get() + delta);
			}, this));
			this._input.focus(function () { $(this).blur(); });

			if (this._useCash) {
			    this._useCashItem = $('#useCashCoupon-item');
			    this._useCashVal = $('#useCashCounpon strong');
			    this._subPrice = $('#subPrice strong');

			    this._useCashItem.click($.proxy(function (e) {
			        var thisObj = this._useCashItem;
			        var _p1 = parseInt(this._price.text());
			        var _cp = parseInt(this._useCashVal.text());
			        var _sbp = _p1;

			        var sel = thisObj.data("sel");
			        if (sel == "1") {
			            thisObj.data("sel", "0");
			            thisObj.removeClass("useCashCoupon-item-sel");
			        }
			        else {
			            thisObj.data("sel", "1");
			            thisObj.addClass("useCashCoupon-item-sel");
			            _sbp = _p1 - _cp;
			        }

			        this._subPrice.text(_sbp);
			    }, this));
			}
		},
		get: function () {
			return parseInt(this._input.val(), 10);
		},
		set: function (value, fireEvent) {

		    if (value >= 1 && value <= window.canSellPackageCount) {
		        this._input.val(value);
		        this._label.text(value);
		        var _priceVal = window.packagePrice * value;
		        this._price.text(_priceVal);
		        if (this._useCash) {

		            //set cash
		            var _cash = window.canUseCashAmount * value;
		            if (_cash > window.userCashAmount) { _cash = window.userCashAmount; }
		            this._useCashVal.text(_cash);
		            var sel = this._useCashItem.data("sel");
		            if (sel == "1") {
		                this._subPrice.text(_priceVal - _cash);
		            }
		            else {
		                this._subPrice.text(_priceVal);
		            }
		        }
		        contacts.setCount(value);

                //刷新出行人模块
                this.refPersonSection(value);

		        //是否需要刷新现金券使用
		        if (canUseCoupon) {

		            var _couponIdx = (cashCouponVue && cashCouponVue.baseCashCouponInfo && cashCouponVue.baseCashCouponInfo.CashCouponInfo)
                        ? cashCouponVue.baseCashCouponInfo.CashCouponInfo.IDX : 0;

		            if (_couponIdx > 0) {

		                //console.log("ck:" + _couponIdx);

		                //刷新当前选择券的状态
		                refCashCouponInfo(_couponIdx);

		                //刷新券选择列表
		                loadCashCouponSection(_couponIdx);
		            }
		            else {
                        
		                if (_couponIdx > -1) {

		                    //get best
		                    GetBestCashCoupon();
		                }
		                else {

		                    //不能使用现金券时，直接刷新住基金
		                    refUserFund();

		                    //刷新券选择列表
		                    loadCashCouponSection(_couponIdx);
		                }
		            }
		        }
		        else {

		            //不能使用现金券时，直接刷新住基金
		            refUserFund();
		        }

		        //刷新VIP tip
		        refVipTipInfo();
		    }
		    else if (value > window.canSellPackageCount) {
		        alert("超出可售套餐数");
		    }
        },
        refPersonSection: function (value) {

            //出行人配置信息
            var _mxp = parseInt($("#maxPersonNum").val());
            var _mip = parseInt($("#minPersonNum").val());

            //change travelperson options
            try {

                maxPersonNum = _mxp * value;
                minPersonNum = _mip * value;

                refAddPersonOptions();

            } catch (e) {

            }

            //刷新机酒邮轮出行人模块
            if (isNeedDocInfo && maxPersonNum) {

                var _sumAirPersonCount = _mxp * value;
                console.log(_sumAirPersonCount);

                var _newAirPersons = [];
                for (var _personNum = 0; _personNum < _sumAirPersonCount; _personNum++) {

                    var _personItem = { "CName": "" };
                    if (airPersons.length > _personNum) {
                        _personItem = airPersons[_personNum];
                    }

                    _newAirPersons.push(_personItem);
                }

                airPersons = _newAirPersons;
                airPersonsVue.minPersonNum = minPersonNum;
                airPersonsVue.persons = _newAirPersons;
            }
        },
		reset: function () {

		    var value = this.get();
		    if (value >= 1 && value <= window.canSellPackageCount) {
		        this._input.val(value);
		        this._label.text(value);
		        this._price.text(window.packagePrice * value);
                contacts.setCount(value);

                //刷新出行人模块
                this.refPersonSection(value);
		    }
		    else if (value > window.canSellPackageCount) {
		        alert("超出可售套餐数");
		    }
		}
	};
	var contacts = {
		init: function () {
			var form = $('#form');
			var self = this;
			this._form = form;
			this._template = form.find('.contact:first');
			this._phone = form.find('.mobile').find('input');
			this._vrow = form.find('.verify');
			this._vcode = this._vrow.find('input');
			this._vbutton = this._vrow.find('button');
			this._vbutton.on('click', function () {
				var number = self._phone.val();
				if (!isMobile(number)) {
					alert('请输入有效的手机号');
					return;
				}
				verify.send(number, function (lock) {
					self._vbutton.toggleClass('disabled', lock).prop('disabled', lock);
					if (!lock) {
						self._vbutton.text('重发验证码');
					}
				}, function (seconds) {
					self._vbutton.text(seconds + '秒后可重发');
				});
			});
			this.getPhone() || this._phone.val(this._retrieve());
			var toggleVRow = function () {
				self._vrow.toggle(self.shouldVerify());
			};
			this._phone.on('keyup change blur', toggleVRow);
			toggleVRow();
		},
		shouldVerify: function () {
			var stored = this._retrieve();
			var ret = !stored || this.getPhone() != stored;
			return ret;
		},
		getContacts: function () {
            var ret = [];

            if (packageInChina) {
                this._contacts().each(function () {
                    ret.push($.trim(this.value));
                });
            }
            else {
                this._contactRows().each(function () {

                    var _xingVal = $.trim($(this).find("._contact_xing input").val());
                    var _mingVal = $.trim($(this).find("._contact_ming input").val());
                    if (_xingVal && _mingVal) {
                        var _name = _xingVal + " " + _mingVal;
                        ret.push(_name);
                    }
                    
                });
            }
			
			return ret;
		},
		getPhone: function () {
			return $.trim(this._phone.val());
		},
		getVCode: function () {
			return $.trim(this._vcode.val());
		},
		setContacts: function (array) {
			this.setCount(array.length);
			this._contacts().each(i, function () {
				this.value = array[i] || '';
			});
		},
		setCount: function (count) {
			var rows = this._contactRows();
			for (var i = rows.length, ref = rows.last(); i < count; ++i) {
				this._template.clone().insertAfter(ref).find('input').val('');
			}
			rows.slice(count).remove();
		},
		setPhone: function (text) {
			this._phone.val(text);
		},
		_contacts: function () {
			return this._contactRows().find('input');
		},
		_contactRows: function () {
			return this._form.find('.contact');
		},
		_store: function (contactPhone) {
			if (window.localStorage) {
				localStorage.contactPhone = contactPhone;
			} else {
				var now = new Date();
				var expires = new Date(now.setYear(now.getFullYear() + 1));
				document.cookie = 'contactPhone=' + contactPhone + '; expires=' + expires.toGMTString();
			}
			return contactPhone;
		},
		_retrieve: function (contactPhone) {
			if (window.localStorage) {
				return localStorage.contactPhone;
			}
			return (document.cookie.match(/(^|; )contactPhone=(\d+)/) || ['', ''])[1];
		}
	};
	var options = {
		init: function () {
			var form = $('#form');
			this._options = form.find('input:checkbox');
			this._text = form.find('textarea');
			this._options.on('click', $.proxy(function (e) {
				var el = $(e.target);
				this._options.not(el).filter('[name="' + el.attr('name') + '"]').prop('checked', false);
			}, this));
		},
		get: function () {
			var ret = [];
			this._options.filter(':checked').each(function () {
				ret.push(this.value);
			});
			var txt = $.trim(this._text.val()).replace(/\r\n?/g, ' ');
			txt && ret.push(txt);
			return $.trim(ret.join(' '));
		},
		set: function (text) {
			var texts = [];
			var inputs = this._options;
			$.each(text.split(' '), function () {
				var tmp = inputs.filter('[name="' + this + '"]');
				tmp.length ? tmp.prop('checked', true) : texts.push(this);
			});
			this._text.val(texts.join(' '));
		}
	};
	var verifyUrl = window.httpsWebUrl + 'Account/Verify';
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
		    return $.post(verifyUrl, {
				action: 'check',
				number: number,
				code: code
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

	var isApp = $("#isapp").val() == "1";

	amount.init();
	contacts.init();
	options.init();

	amount.reset();
	
    //【VIP Tip】非VIP用户，显示成为VIP的提示块
	var refVipTipInfo = function () {
	    if (!isVip) {

	        var _orderNoVipTotalPrice = window.packageNoVipPrice * parseInt(amount._input.val());
	        var _orderVipTotalPrice = window.packageVipPrice * parseInt(amount._input.val());

	        var pcDic = { "orderTotalPrice": _orderNoVipTotalPrice, "orderVipTotalPrice": _orderVipTotalPrice };
	        $.get(_Config.APIUrl + "/api/coupon/BecomeVIPTips", pcDic, function (_data) {

	            console.log(_data);
	            if (_data && _data.ActionUrl) {

	                if (vipTipVue) {
	                    vipTipVue.vipTipInfo = _data;
	                }
	                else {
	                    vipTipVue = new Vue({
	                        el: '#vip-tip-section',
	                        data: { "vipTipInfo": _data }
	                    })
	                }

	                $("#vip-tip-section").show();
	            }

	        });
	    }
	    else {
	        $("#vip-tip-section").hide();
	    }
	};
	refVipTipInfo();

    //【住基金】住基金的选择事件
	fundSelectEvent = function () {

	    console.log("fundSelectEvent");

	    //$("#fund-radio-btn").unbind("click");
	    //$("#fund-radio-btn").click(function () {

	        console.log(userFundVue.fundInfo)

	        if (userFundVue.fundInfo.sel === 1) {

	            console.log("sel 0");
	            userFundVue.fundInfo.sel = 0;
	        }
	        else {

	            console.log("sel 1");
	            userFundVue.fundInfo.sel = 1;
	        }

	        refUserFund();
	    //});
	}

    //【住基金】刷新住基金使用
	var refUserFund = function () {

        //order total price
	    var _orderTotalPrice = window.packagePrice * parseInt(amount._input.val());

	    //可用现金券
	    var _couponIdx = 0;
	    var _couponType = 0;
	    var _cashCouponAmount = 0;
	    if (canUseCoupon && cashCouponVue && cashCouponVue.baseCashCouponInfo && cashCouponVue.baseCashCouponInfo.CashCouponInfo && cashCouponVue.baseCashCouponInfo.CashCouponInfo.IDX > 0) {
	        _cashCouponAmount = cashCouponVue.baseCashCouponInfo.OrderCanDiscountAmount;
	        _couponIdx = cashCouponVue.baseCashCouponInfo.CashCouponInfo.IDX;
	        _couponType = cashCouponVue.baseCashCouponInfo.CashCouponInfo.UserCouponType;
	    }

	    //可用住基金
	    var _canUseFund = userFundAmount;
	    if (_orderTotalPrice - _cashCouponAmount < userFundAmount) {
	        _canUseFund = _orderTotalPrice - _cashCouponAmount;
	    }

	    var _data = {
	        userFundAmount: userFundAmount,
	        canUseFund: _canUseFund,
            sel: 1
	    };

	    if (userFundVue) {

	        console.log("reset " + userFundVue.fundInfo.sel);

	        _data.sel = userFundVue.fundInfo.sel;
	        userFundVue.fundInfo = _data;
	    }
	    else {

	        userFundVue = new Vue({
	            el: '#user-fund-section',
	            data: { "fundInfo": _data }
	        })
	    }

	    Vue.nextTick(function () {

	        console.log(666);

	        //绑定住基金事件
	        //fundSelectEvent();
	    });

	    //如果没有选择住基金，则不算入最后的计算
	    if (userFundVue.fundInfo.sel === 0) {
	        _canUseFund = 0;
	    }

	    //目前住基金是最后的优惠选择，所以在住基金刷新最后，刷新订单支付金额及优惠信息
	    var _discountTotal = _cashCouponAmount + _canUseFund;
	    var _orderPayPrice = _orderTotalPrice - _discountTotal;

	    var _orderPayData = {
	        payPrice: _orderPayPrice,
	        discount: _discountTotal,
	        couponIdx: _couponIdx,
	        couponType: _couponType,
	        cashCouponAmount: _cashCouponAmount,
	        canUseFund: _canUseFund
	    };

	    if (orderPayMoneyVue) {
	        orderPayMoneyVue.priceInfo = _orderPayData;
	    }
	    else {
	        orderPayMoneyVue = new Vue({
	            el: '#order-pay-money',
	            data: { "priceInfo": _orderPayData }
	        })
	    }


	}
	
    //【现金券】刷新/检测券
	var refCashCouponInfo = function (_couponIdx) {

	    var _orderTotalPrice = window.packagePrice * parseInt(amount._input.val());

	    var _couponParamDic = {
	        BuyCount: amount._input.val(),
	        TotalOrderPrice: _orderTotalPrice,
	        SelectedCashCouponID: _couponIdx,
	        OrderSourceID: pid,
	        OrderTypeID: 1,
	        SelectedDateFrom: checkIn,
	        SelectedDateTo: checkOut,
	        UserID: pubuserid,
	    };

	    //console.log(_couponParamDic);

	    $.post(_Config.APIUrl + "/api/coupon/CheckSelectedCashCouponInfoForOrder", _couponParamDic, function (_data) {

	        console.log(_data);
	        if (_data) {

	            if (cashCouponVue) {
	                cashCouponVue.baseCashCouponInfo = _data;
	            }
	            else {
	                cashCouponVue = new Vue({
	                    el: '#cash-couopon-section',
	                    data: { "baseCashCouponInfo": _data }
	                })
	            }
	        }

	        //刷新住基金
	        refUserFund();

	    });
	}

    //【现金券】加载现金券选择模块
	var loadCashCouponSection = function (_selectedCouponId) {

	    var _orderTotalPrice = window.packagePrice * parseInt(amount._input.val());

	    var _buyCount = amount._input.val();

	    $("#cash-coupon-section").load(
            "/Coupon/WalletCashCoupon?userid={0}&couponid={1}&buycount={2}&totalprice={3}&from={4}&to={5}&sourceid={6}&sourcetype={7}&select=1&issection=1"
            .format(pubuserid, _selectedCouponId, _buyCount, _orderTotalPrice, checkIn, checkOut, pid, 1));
	}

    //【现金券】Get Best
	var GetBestCashCoupon = function () {

	    var _orderTotalPrice = window.packagePrice * parseInt(amount._input.val());

	    //默认一个最优的现金券
	    var _couponParamDic = {
	        BuyCount: amount._input.val(),
	        TotalOrderPrice: _orderTotalPrice,
	        SelectedCashCouponID: 0,
	        OrderSourceID: pid,
	        OrderTypeID: 1,
	        SelectedDateFrom: checkIn,
	        SelectedDateTo: checkOut,
	        UserID: pubuserid,
	    };
	    $.post(_Config.APIUrl + "/api/coupon/GetTheBestCouponInfoForOrder", _couponParamDic, function (_data) {

	        console.log(_data);
	        if (_data) {

	            if (cashCouponVue) {
	                cashCouponVue.baseCashCouponInfo = _data;
	            }
	            else {
	                cashCouponVue = new Vue({
	                    el: '#cash-couopon-section',
	                    data: { "baseCashCouponInfo": _data }
	                })
	            }

	            //加载现金券选择模块
	            loadCashCouponSection(_data.CashCouponInfo.IDX);

	            //刷新住基金
	            refUserFund();

	            //$("#cash-couopon-section").show();
	        }

	    });
	};

    //【现金券】当前套餐可以使用现金券的话..
	if (canUseCoupon) {

	    //展开选择现金券模块
	    var openCashSection = function () {

	        //url状态标识
	        location.href = location.href + "#cash-coupon-section";
	    }
	    $(".sel-cash").click(openCashSection);

	    //get best
	    GetBestCashCoupon();

	    //选择券的处理事件
	    window.selectCashCouponFun = function (_couponIdx) {

	        if (_couponIdx >= 0) {

	            refCashCouponInfo(_couponIdx);

	        } else {

	            cashCouponVue.baseCashCouponInfo.CashCouponInfo.IDX = -1;

	            //刷新住基金
	            refUserFund();
	        }
	    }
	}
	else {

	    //不能使用现金券时，直接刷新住基金
	    refUserFund();
	}

	var paywindow;

	$('button[data-submit-url]').click(function (e) {
	    
	    if (isInWeixin) {
	        alert("请在默认浏览器打开后进行支付");
	        return;
	    }

	    if (!window.isMobile)
	    {
	        paywindow = window.open();
	    }
	    var url = $(this).data('submitUrl');
	    submitAndPay(url);
	});

	$('a[data-submit-url]').click(function (e) {
	    if (!window.isMobile) {

	        if (isInWeixin) {
	            alert("请在默认浏览器打开后进行支付");
	            return;
	        }

	        paywindow = window.open();
	    }
	    var url = $(this).attr('data-submit-url');
	    submitAndPay(url);
	});

	$('a[data-choose-url]').click(function (e) {
	    if (!window.isMobile) {
            
	        if (isInWeixin) {
	            alert("请在默认浏览器打开后进行支付");
	            return;
	        }

	        paywindow = window.open();
	    }
	    var url = $(this).attr('data-choose-url');
	    gotoChoosePay(url);
	});

    //m版去支付操作
	$("#m-gopay").click(function () {

	    var url = $(this).attr('data-choose-url');
        gotoChoosePay(url);
	});

	function gotoChoosePay(url) {
	    var params = {
	        roomCount: amount.get(),
	        contact: contacts.getContacts().join(','),
	        contactPhone: contacts.getPhone(),
	        note: options.get(),
            travelPersons: getTravelPersonsStr(),
            airPersons: getAirPersonsStr()
        };

	    //travel person
        if (minPersonNum > 0) {

            //机酒邮轮出行人验证
            if (isNeedDocInfo) {

                var _airPersonsCount = 0;
                for (var _personNum = 0; _personNum < airPersons.length; _personNum++) {
                    if ($.trim(airPersons[_personNum].CName)) {
                        _airPersonsCount++;
                    }
                }

                if (_airPersonsCount < minPersonNum) {
                    return alertCheckMsg('此套餐需至少填写' + minPersonNum + "名出行人");
                }
            }
            else {

                if (addPersonSelecteds.length < minPersonNum) {
                    return alertCheckMsg('此套餐需至少添加' + minPersonNum + "名出行人");
                }
            }

	        //if (addPersonSelecteds.length < maxPersonNum) {
	        //    if (confirm("此套餐可设置" + maxPersonNum + "名出行人，还有" + (maxPersonNum - addPersonSelecteds.length) + "位没有设置，是否返回设置？")) {
	        //        return;
	        //    }
	        //}
	    }

        if (/,,/.test(',' + params.contact + ',')) {
            return alertCheckMsg('入住人信息填写不完整');
        }

        if (contacts.getContacts().length < amount.get()) {
            return alertCheckMsg('至少需要填写{0}名入住人信息'.format(amount.get()));
        }

        if (params.note.indexOf("床") == -1) {
            return alertCheckMsg("请选择床型");
        }

	    if (!params.contactPhone) {
	        return alertCheckMsg('请填写手机号码');
	    }
	    if (!isMobile(params.contactPhone)) {
	        return alertCheckMsg('请填写有效的手机号码');
	    }

	    var dfd = $.Deferred().resolve();
	    if (contacts.shouldVerify()) {
	        var vcode = contacts.getVCode();
	        if (!vcode) {
	            return alertCheckMsg('请填写手机验证码');
	        }
	        dfd = verify.check(params.contactPhone, vcode);
        }

        //验证结束后的去支付操作
        var _goPay = function () {

            //cash choose
            if (amount._useCash) {
                url += ("&chooseCash=" + amount._useCashItem.data("sel"));
            }

            if (orderPayMoneyVue && orderPayMoneyVue.priceInfo) {

                //cash coupon
                if (orderPayMoneyVue.priceInfo.couponIdx && orderPayMoneyVue.priceInfo.cashCouponAmount) {
                    url += ("&cashCouponIdx=" + orderPayMoneyVue.priceInfo.couponIdx);
                    url += ("&cashCouponType=" + orderPayMoneyVue.priceInfo.couponType);
                    url += ("&cashCouponAmount=" + orderPayMoneyVue.priceInfo.cashCouponAmount);
                }

                //fund
                if (orderPayMoneyVue.priceInfo.canUseFund) {
                    url += ("&useFundAmount=" + orderPayMoneyVue.priceInfo.canUseFund);
                }
            }

            showSpinner(true);
            dfd.then(function () {
                return $.post(url, params);
            }).then(function (r) {

                showSpinner(false);

                if (r.url) {
                    if (window.isMobile) {
                        location.href = r.url;
                    }
                    else {
                        paywindow.location = r.url;
                        showSpinner(true);
                        $('#pop').show();
                    }
                    return;
                }

                if (r.code && r.code == "2") {

                    //如果没有登录，则弹出选择登录的提示窗
                    if (pubuserid == "0") {

                        showSpinner(false);

                        bootbox.alert({
                            message: "<div class='alert-rulesmsg'>" + r.error + "</div>",
                            buttons: {
                                ok: { label: '现在登录', }
                            },
                            callback: function (result) {
                                _loginModular.show();
                            },
                            closeButton: true
                        });

                        //bootbox.confirm({
                        //    message: "<div class='alert-rulesmsg'>" + r.error + "</div>",
                        //    buttons: {
                        //        confirm: { label: '现在登录', },
                        //        cancel: { label: '我知道了', }
                        //    },
                        //    callback: function (result) {
                        //        if (result) {
                        //            _loginModular.show();
                        //        }
                        //    },
                        //    closeButton: true,
                        //    onEscape: function () { }
                        //});
                    }
                    else {
                        return $.Deferred().reject(new Error(r.error || r));
                    }
                }
                else {
                    return $.Deferred().reject(new Error(r.error || r));
                }

            }).fail(function (e) {
                alertCheckMsg(e instanceof Error ? e.message : '网络请求失败');
                showSpinner(false);
            });

            if (isApp) {
                setTimeout(function () { showSpinner(false); }, 2000);
            }
        }

        //m端非VIP弹出VIP引导
        if (isMobile && !isVip) {

            //目前非VIP会始终弹出提示
            var _orderNoVipTotalPrice = window.packageNoVipPrice * parseInt(amount._input.val());
            var _orderVipTotalPrice = window.packageVipPrice * parseInt(amount._input.val());

            var pcDic = { "orderTotalPrice": _orderNoVipTotalPrice, "orderVipTotalPrice": _orderVipTotalPrice };
            $.get(_Config.APIUrl + "/api/coupon/VIPDiscountDescription", pcDic, function (_data) {

                console.log(_data);
                if (_data && _data.ActionUrl && _data.Description) {

                    _Modal.show({
                        title: '还不是VIP会员？',
                        content: _data.Description,
                        confirmText: '成为VIP会员',
                        confirm: function () {
                            _Modal.hide();
                            goBuyVip();
                        },
                        showCancel: true,
                        cancelText: '继续支付',
                        cancel: function () {
                            _Modal.hide();
                            _goPay(url);
                        }
                    });
                }

            });
        }
        else {

            _goPay();
        }

        
	}

	$('#payLink').click(function (e) {
	    var url = $(this).data('submitUrl');
	    submitAndPay(url);
	});

    function submitAndPay(url)
	{
        var params = {
            roomCount: amount.get(),
            contact: contacts.getContacts().join(','),
            contactPhone: contacts.getPhone(),
            note: options.get(),
            travelPersons: getTravelPersonsStr(),
            airPersons: getAirPersonsStr()
        };

        //travel person
        if (minPersonNum > 0) {

            //机酒邮轮出行人验证
            if (!packageInChina) {

                var _airPersonsCount = 0;
                for (var _personNum = 0; _personNum < airPersons.length; _personNum++) {
                    if ($.trim(airPersons[_personNum].CName)) {
                        _airPersonsCount++;
                    }
                }

                if (_airPersonsCount < minPersonNum) {
                    return alertCheckMsg('此套餐需至少填写' + minPersonNum + "名出行人");
                }
            }
            else {

                if (addPersonSelecteds.length < minPersonNum) {
                    return alertCheckMsg('此套餐需至少添加' + minPersonNum + "名出行人");
                }
            }

            //if (addPersonSelecteds.length < maxPersonNum) {
            //    if (confirm("此套餐可设置" + maxPersonNum + "名出行人，还有" + (maxPersonNum - addPersonSelecteds.length) + "位没有设置，是否返回设置？")) {
            //        return;
            //    }
            //}
        }

        if (/,,/.test(',' + params.contact + ',')) {
            return alertCheckMsg('请填写入住人姓名');
        }

        if (contacts.getContacts().length < amount.get()) {
            return alertCheckMsg('至少需要填写{0}名入住人信息'.format(amount.get()));
        }

        if (!params.contactPhone) {
            return alertCheckMsg('请填写手机号码');
        }
        if (!isMobile(params.contactPhone)) {
            return alertCheckMsg('请填写有效的手机号码');
        }

        var dfd = $.Deferred().resolve();
        if (contacts.shouldVerify()) {
            var vcode = contacts.getVCode();
            if (!vcode) {
                return alertCheckMsg('请填写手机验证码');
            }
            dfd = verify.check(params.contactPhone, vcode);
        }
       
        //cash choose
        if (amount._useCash) {
            url += ("&chooseCash=" + amount._useCashItem.data("sel"));
        }

        if (orderPayMoneyVue && orderPayMoneyVue.priceInfo) {

            //cash coupon
            if (orderPayMoneyVue.priceInfo.couponIdx && orderPayMoneyVue.priceInfo.cashCouponAmount) {
                url += ("&cashCouponIdx=" + orderPayMoneyVue.priceInfo.couponIdx);
                url += ("&cashCouponType=" + orderPayMoneyVue.priceInfo.couponType);
                url += ("&cashCouponAmount=" + orderPayMoneyVue.priceInfo.cashCouponAmount);
            }

            //fund
            if (orderPayMoneyVue.priceInfo.canUseFund) {
                url += ("&useFundAmount=" + orderPayMoneyVue.priceInfo.canUseFund);
            }
        }

        showSpinner(true);

        $.post(url, params).then(function (r) {
        if (r.url) {
            var id = r.url.split('/')[4];
            $("#submitOrderValue").val(id);
            if (window.isMobile) {
                location.href = r.url;
            }
            else {                   
                paywindow.location = r.url;
                showSpinner(true);
                $('#pop').show();
            }
            return;
        }

        return $.Deferred().reject(new Error(r.error || r));
        }).fail(function (e) {
            alertCheckMsg(e instanceof Error ? e.message : '网络请求失败');
            showSpinner(false);
        });
        return;

        dfd.then(function () {
            return $.post(url, params);
        }).then(function (r) {
            if (r.url) {
                var id = r.url.split('/')[4];
                $("#submitOrderValue").val(id);
                if (window.isMobile) {
                    location.href = r.url;
                }
                else {                   
                    paywindow.location = r.url;
                    showSpinner(true);
                    $('#pop').show();
                }
                return;
            }
            return $.Deferred().reject(new Error(r.error || r));
        }).fail(function (e) {
            alertCheckMsg(e instanceof Error ? e.message : '网络请求失败');
            showSpinner(false);
        });
    }

    function alertCheckMsg(msg)
    {
        if (!window.isMobile && paywindow)
        {
            paywindow.close();
        }
        return alert(msg);
    }

    $('.icheck-panel label').click(function () {
        $(this).prev('input').click();
        //$(this).prev('input').click().end().addClass('curr').siblings('label.curr').removeClass('curr');
    });
    $('.icheck-panel label').each(function () {
        $(this).prev('input').change(function () {
            $('.icheck-panel label[fn=' + $(this).attr("name") + ']').removeClass("curr");
            if ($(this)[0].checked) $(this).next('label').addClass("curr");
        });
    });
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
                $("#cash-coupon-base-bg").show();
                document.title = "现金券";
            }
        }
    }
    else {

        console.log("no #");

        //清除/隐藏所有tag元素
        $("#cash-coupon-section").hide();
        $("#cash-coupon-base-bg").hide();

        document.title = "确认订单";
    }
}
urlHashChange();
window.onhashchange = urlHashChange;

showSpinner.prefetch();

$('#pop .close').on('click', function () {
    showSpinner(false);
    $('#pop').hide();
});

$('#chooseOtherPay').on('click', function () {
    showSpinner(false);
    $('#pop').hide();
});

function showOrderInfo()
{
    var orderid = parseInt($("#submitOrderValue").val(),10);
    if (orderid) {
        $.ajax({
            type: "POST",
            url: "/order/IsOrderHasPaied",
            data: { orderid: orderid },
            success: function (data) {
                if ( data == "True") {
                    var url = "/payment/complete/paid/" + orderid;
                    window.location = url;
                }
                else {
                    alert("订单尚末完成支付，请先支付订单。");
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                alert(XMLHttpRequest.responseText);
            },
            cache: false
        });
    }
    else
    {
        alert("订单尚末完成支付，请先支付订单。");
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

var getTravelPersonsStr = function () {
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
    if (maxPersonNum) {

        //_loginModular.init(loginCheckFun, loginCancelFun);

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

        Vue.nextTick(function () {

            $("#person-panel").show();
        });
    }
}

//加载机酒/邮轮出行人
var airPersonsVue;
var airPersons = [];
var initAirPersonInfo = function () {

    if (maxPersonNum) {

        for (var personNum = 0; personNum < maxPersonNum; personNum++) {

            airPersons.push({ "CName": "" });
        }

        //init add person options
        airPersonsVue = new Vue({
            el: "#air-person-panel",
            data: {
                "minPersonNum": minPersonNum,
                "persons": airPersons
            }
        })

        Vue.nextTick(function () {

            $("#air-person-panel").show();
        });
    }
}

//获取机酒邮轮出行人字符串信息
var getAirPersonsStr = function () {
    var _str = "";
    if (airPersons) {
        airPersons.map(function (item, index) {
            if (_str != "") _str += ",";
            _str = _str + item.CName;
        });
    }
    return _str;
}

if (isNeedDocInfo) {

    //机酒邮轮出行人初始加载
    initAirPersonInfo();
}
else {

    //常规出行人信息初始加载
    initPersonInfo();
}

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': $("#_name").val(), 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?userid=" + pubuserid + "&_isoneoff=1&_newpage=1";
}