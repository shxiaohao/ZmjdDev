$(function () {
	var amount = {
		init: function () {
			this._buttons = $('#amount .btn');
			this._input = $('#amount input');
			this._label = $('#amount-label');
			this._price = $('#price strong');
			this._buttons.click($.proxy(function (e) {
				var delta = this._buttons.index(e.target) == 0 ? -1 : 1;
				this.set(this.get() + delta);
			}, this));
		},
		get: function () {
			return parseInt(this._input.val(), 10);
		},
		set: function (value, fireEvent) {
			if (value >= 1) {
				this._input.val(value);
				this._label.text(value);
				this._price.text(window.packagePrice * value);
				contacts.setCount(value);
			}
		}
	};
	var contacts = {
		init: function () {
			var form = $('#form');
			var self = this;
			this._form = form;
			this._template = form.children('.contact:first');
			this._phone = form.children('.mobile').find('input');
			this._vrow = form.children('.verify');
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
			this._contacts().each(function () {
				ret.push($.trim(this.value));
			});
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
			return this._form.children('.contact');
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
	var verify = {
		send: function (number, lock, update) {
			var self = this;
			if (self._timer) {
				return;
			}
			self._startTimer(lock, update);
			$.post(window.verifyUrl, {
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
			return $.post(window.verifyUrl, {
				action: 'check',
				number: number,
				code: code
			}, 'json').then(function (r) {
				return r.ok ? contacts._store(number) : $.Deferred().reject(new Error('短信校验码有误'));
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

	amount.init();
	contacts.init();
	options.init();

	var paywindow;

	$('button[data-submit-url]').click(function (e) {	   
	    if (!window.isMobile)
	    {
	        paywindow = window.open();
	    }
	    var url = $(this).data('submitUrl');
	    submitAndPay(url);
	});

	$('a[data-choose-url]').click(function (e) {
	    if (!window.isMobile) {
	        paywindow = window.open();
	    }
	    var url = $(this).attr('data-choose-url');
	    gotoChoosePay(url);
	});

	function gotoChoosePay(url){
	    var params = {
	        roomCount: amount.get(),
	        contact: contacts.getContacts().join(','),
	        contactPhone: contacts.getPhone(),
	        note: options.get()
	    };
	    if (/,,/.test(',' + params.contact + ',')) {
	        return alertCheckMsg('请填写入住人姓名');
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

	    showSpinner(true);
	    dfd.then(function () {
	        return $.post(url, params);
	    }).then(function (r) {
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
	        return $.Deferred().reject(new Error(r.error || r));
	    }).fail(function (e) {
	        alertCheckMsg(e instanceof Error ? e.message : '网络请求失败');
	        showSpinner(false);
	    });
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
            note: options.get()
        };
        if (/,,/.test(',' + params.contact + ',')) {
            return alertCheckMsg('请填写入住人姓名');
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
       
        showSpinner(true);
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
});





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
