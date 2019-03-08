//window.httpsWebUrl = "/";

//初始手机验证登录模块
var _loginModular = {

    setpwdBackTag: "reg",   //reg标识设置密码页面要返回到注册页 find标识设置密码页面要返回到找回密码页面
    _ckFun: function () { },
    _cancelFun: function () { },
    _hideCancel: false,
    _followUserId: 0,
    _followNickName: '',
    _invitationCode: '',
    _onlyPhoneLogin: false, //只显示验证码登录
    _useGeetest: true,      //是否使用极验安全验证
    _onlyUseForget: false,  //是否只使用忘记密码模块
    _forgetCallback: function () { },
    init: function (ckFun, cancelFun, hideCancel) {

        var self = this;

        //create ele
        self.createLoginElement();

        self._ckFun = ckFun;
        self._cancelFun = cancelFun;
        self._hideCancel = hideCancel;

        //行为验证操作对象
        var _geetest;
        if (self._useGeetest) {

            $.ajax({
                // 获取id，challenge，success（是否启用failback）
                url: "/account/GetCaptcha?t=" + (new Date()).getTime(), // 加随机数防止缓存
                type: "get",
                dataType: "json",
                success: function (data) {

                    // 使用initGeetest接口
                    // 参数1：配置参数
                    // 参数2：回调，回调的第一个参数验证码对象，之后可以使用它做appendTo之类的事件
                    initGeetest({
                        gt: data.gt,
                        challenge: data.challenge,
                        product: "bind", // 产品形式，包括：float，embed，popup。注意只对PC版验证码有效
                        offline: !data.success, // 表示用户后台检测极验服务器是否宕机，一般不需要关注
                        width: '100%'
                        // 更多配置参数请参见：http://www.geetest.com/install/sections/idx-client-sdk.html#config
                    }, function (captchaObj) {
                        _geetest = captchaObj;
                    });
                }
            })
        }

        this._loginModel = $(".login-model");
        this._loginSection = $(".login-section");

        this._login_block = $("._login_block");
        this._input_login_phone = this._loginSection.find("._input_login_phone");
        this._input_login_pwd = this._loginSection.find("._input_login_pwd");
        this._btn_login = this._loginSection.find("._btn_login");
        this._btn_gophonelogin = this._loginSection.find("._btn_gophonelogin");
        this._login_goreg = this._loginSection.find("._login_goreg");
        this._login_findpwd = this._loginSection.find("._login_findpwd");

        this._login2_block = $("._login2_block");
        this._input_login2_phone = this._loginSection.find("._input_login2_phone");
        this._input_login2_code = this._loginSection.find("._input_login2_code");
        this._login2_getcode = this._loginSection.find("._login2_getcode");
        this._btn_login2 = this._loginSection.find("._btn_login2");
        this._btn_gologin = this._loginSection.find("._btn_gologin");

        this._reg_block = $("._reg_block");
        this._input_reg_phone = this._loginSection.find("._input_reg_phone");
        this._input_reg_code = this._loginSection.find("._input_reg_code");
        this._reg_getcode = this._loginSection.find("._reg_getcode");
        this._btn_reg_next = this._loginSection.find("._btn_reg_next");

        this._setpwd_block = $("._setpwd_block");
        this._input_set_pwd1 = this._loginSection.find("._input_set_pwd1");
        this._input_set_pwd2 = this._loginSection.find("._input_set_pwd2");
        this._btn_set_ok = this._loginSection.find("._btn_set_ok");

        this._findpwd_block = $("._findpwd_block");
        this._input_find_phone = this._loginSection.find("._input_find_phone");
        this._input_find_code = this._loginSection.find("._input_find_code");
        this._find_getcode = this._loginSection.find("._find_getcode");
        this._btn_find_next = this._loginSection.find("._btn_find_next");

        this._login_close = this._loginSection.find("._login_close");
        this._login2_close = this._loginSection.find("._login2_close");
        this._reg_back = this._loginSection.find("._reg_back");
        this._set_back = this._loginSection.find("._set_back");
        this._find_back = this._loginSection.find("._find_back");
        this._reg_nopwd_back = this._loginSection.find("._reg_nopwd_back");

        this._reg_nopwd_block = $("._reg_nopwd_block");
        this._input_reg_nopwd_phone = this._loginSection.find("._input_reg_nopwd_phone");
        this._input_reg_nopwd_code = this._loginSection.find("._input_reg_nopwd_code");
        this._input_reg_nopwd_invitation = this._loginSection.find("._input_reg_nopwd_invitation");
        this._reg_nopwd_getcode = this._loginSection.find("._reg_nopwd_getcode");
        this._reg_nopwd_ck = this._loginSection.find("._ck")[0];
        this._btn_reg_nopwd_next = this._loginSection.find("._btn_reg_nopwd_next");

        this._reg_nopwd_complate_block = $("._reg_nopwd_complate_block");

        //login 执行登录
        this._btn_login.click(function () {

            var _phone = self._input_login_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            var _pwd = self._input_login_pwd.val();
            if (_pwd.length < 6) { alert('请输入正确的密码'); return; }

            //login
            $.post(self.verify.verifyCkLoginUrl, {
                number: _phone,
                password: _pwd,
                isSaveCookie: false
            }, 'json').then(function (r) {
                if (r.Message == '') {

                    var token = r.token;
                    var isv = r.isv;
                    var nn = r.nn;
                    var uid = r.uid;

                    var _loginDic = { token: token, isv: isv, nn: nn, uid: uid };

                    //cache login config
                    self.verify.setLoginConfigCache(_loginDic);

                    //go login
                    $.post(self.verify.verifyGoLoginUrl, _loginDic, 'json')
                        .then(function (data) {
                            if (data.Message == '') {
                                ckFun(uid);
                                //location.reload(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
                            }
                            else {
                                alert("登录失败");
                            }
                        }).fail(function () {
                            alert('网络请求失败');
                        })

                }
                else if (r.Message == '0') {
                    alert('该手机号还没有注册');
                }
                else if (r.Message == '1') {
                    alert('您的手机号或密码错误');
                }
                else {
                    alert('登录失败');
                }
            }).fail(function (e) {
                //console.log(e)
                alert('网络请求失败');
            })
        });

        //login 登录页的密码获焦
        this._input_login_pwd.click(function () {
            var _thisObj = $(this);
            self.reviewDivScrollTop(_thisObj);
        });

        //phonelogin 获取验证码
        this._login2_getcode.click(function () {

            var _phone = self._input_login2_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            if (self._useGeetest) {

                _geetest.verify();
                _geetest.onSuccess(function () {
                    var result = _geetest.getValidate();
                    var geetest_challenge = result.geetest_challenge;
                    var geetest_seccode = result.geetest_seccode;
                    var geetest_validate = result.geetest_validate;
                    self.verify.sendAndGeetest(geetest_challenge, geetest_seccode, geetest_validate, _phone, function (lock) {
                        self._login2_getcode.toggleClass('disabled', lock).prop('disabled', lock);
                        if (!lock) {
                            self._login2_getcode.text('重发验证码');
                        }
                    }, function (seconds) {
                        self._login2_getcode.text(seconds + '秒后可重发');
                    });
                })
            }
            else {

                self.verify.send(_phone, function (lock) {
                    self._login2_getcode.toggleClass('disabled', lock).prop('disabled', lock);
                    if (!lock) { self._login2_getcode.text('重发验证码'); }
                }, function (seconds) {
                    self._login2_getcode.text(seconds + '秒后可重发');
                });
            }

        });

        //phonelogin 执行登录
        this._btn_login2.click(function () {

            var _phone = self._input_login2_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            var _code = self._input_login2_code.val();
            if (_code.length == 0) { alert('请输入短信验证码'); return; }

            var _pwd1 = "";
            var _pwd2 = "";

            //follow status
            var _isFollow = false;

            //follow userid
            var _followUserId = 0;

            //邀请码
            var _invitationCode = "";

            var CID = $("#hidCurUserCID").val();

            var unionid = $("#hidWxUnionid").val();

            //reg nopwd
            $.post(self.verify.verifyRegisterForInvitationUrl, { number: _phone, password: _pwd2, code: _code, invCode: _invitationCode, isFollow: _isFollow, followUserId: _followUserId, CID: CID, Unionid: unionid }, 'json').then(function (result) {
                if (result) {

                    if (result.Success) {

                        self.verify.loginByUid(result.UserID, function () {

                            //self._login2_block.hide();
                            ckFun(result.UserID);

                        });
                    }
                    else {
                        alert(result.Message);
                    }
                }
                else { alert("请求错误，请重试或联系我们"); }
            });

        });

        //reg 注册获取验证码 包含验证
        this._reg_getcode.click(function () {

            var _phone = self._input_reg_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            if (self._useGeetest) {

                _geetest.verify();
                _geetest.onSuccess(function () {
                    var result = _geetest.getValidate();
                    var geetest_challenge = result.geetest_challenge;
                    var geetest_seccode = result.geetest_seccode;
                    var geetest_validate = result.geetest_validate;
                    $.post(self.verify.verifyExistsMobileGeetestUrl, {
                        geetest_challenge: geetest_challenge,
                        geetest_seccode: geetest_seccode,
                        geetest_validate: geetest_validate,
                        number: _phone
                    }, 'json').then(function (r) {
                        if (r.isExists) {
                            alert("该手机号已注册");
                        }
                        else {
                            self.verify.send(_phone, function (lock) {
                                self._reg_getcode.toggleClass('disabled', lock).prop('disabled', lock);
                                if (!lock) {
                                    self._reg_getcode.text('重发验证码');
                                }
                            }, function (seconds) {
                                self._reg_getcode.text(seconds + '秒后可重发');
                            });
                        }
                    });
                })
            }
            else {

                $.post(self.verify.verifyExistsMobileUrl, { number: _phone }, 'json').then(function (r) {
                    if (r.isExists) {
                        alert("该手机号已注册");
                    }
                    else {
                        self.verify.send(_phone, function (lock) {
                            self._reg_getcode.toggleClass('disabled', lock).prop('disabled', lock);
                            if (!lock) { self._reg_getcode.text('重发验证码'); }
                        }, function (seconds) {
                            self._reg_getcode.text(seconds + '秒后可重发');
                        });
                    }
                });
            }

        });

        //reg 注册页的验证码获焦
        this._input_reg_code.click(function () {
            var _thisObj = $(this);
            self.reviewDivScrollTop(_thisObj);
        });

        //reg 检查验证码【下一步】
        this._btn_reg_next.click(function () {

            var _phone = self._input_reg_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            var _code = self._input_reg_code.val();
            if (_code.length == 0) { alert('请输入短信验证码'); return; }

            $.post(self.verify.verifyNewUserUrl, {
                action: 'check',
                number: _phone,
                code: _code,
                CID: $("#hidCurUserCID").val(),
                Unionid: $("#hidWxUnionid").val()
            }, 'json').then(function (r) {
                if (r.ok == "1") {

                    //标识从注册页面跳转
                    self.setpwdBackTag = "reg";

                    //跳转到设置密码
                    self._setpwd_block.fadeIn();
                    self._input_set_pwd1.val("");
                    self._input_set_pwd2.val("");
                    self._reg_block.hide();
                }
                else { alert("短信验证码输入有误"); }
            });

        });

        //find 获取验证码 包含验证
        this._find_getcode.click(function () {

            var _phone = self._input_find_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            if (self._useGeetest) {

                _geetest.verify();
                _geetest.onSuccess(function () {
                    var result = _geetest.getValidate();
                    var geetest_challenge = result.geetest_challenge;
                    var geetest_seccode = result.geetest_seccode;
                    var geetest_validate = result.geetest_validate;

                    $.post(self.verify.verifyExistsMobileGeetestUrl, {
                        geetest_challenge: geetest_challenge,
                        geetest_seccode: geetest_seccode,
                        geetest_validate: geetest_validate,
                        number: _phone
                    }, 'json').then(function (r) {
                        if (!r.isExists) {
                            alert("该手机号还未注册");
                        }
                        else {
                            self.verify.send(_phone, function (lock) {
                                self._find_getcode.toggleClass('disabled', lock).prop('disabled', lock);
                                if (!lock) { self._find_getcode.text('重发验证码'); }
                            }, function (seconds) {
                                self._find_getcode.text(seconds + '秒后可重发');
                            });
                        }
                    });
                })
            }
            else {

                $.post(self.verify.verifyExistsMobileUrl, { number: _phone }, 'json').then(function (r) {
                    if (!r.isExists) {
                        alert("该手机号还未注册");
                    }
                    else {
                        self.verify.send(_phone, function (lock) {
                            self._find_getcode.toggleClass('disabled', lock).prop('disabled', lock);
                            if (!lock) { self._find_getcode.text('重发验证码'); }
                        }, function (seconds) {
                            self._find_getcode.text(seconds + '秒后可重发');
                        });
                    }
                });
            }

        });

        //find 忘记密码页的验证码获焦
        this._input_find_code.click(function () {
            var _thisObj = $(this);
            self.reviewDivScrollTop(_thisObj);
        });

        //find 检查验证码【下一步】
        this._btn_find_next.click(function () {

            var _phone = self._input_find_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            var _code = self._input_find_code.val();
            if (_code.length == 0) { alert('请输入短信验证码'); return; }

            $.post(self.verify.verifyNewUserUrl, {
                action: 'check',
                number: _phone,
                code: _code,
                CID: $("#hidCurUserCID").val(),
                Unionid: $("#hidWxUnionid").val()
            }, 'json').then(function (r) {
                if (r.ok == "1") {

                    //标识从注册页面跳转
                    self.setpwdBackTag = "find";

                    //跳转到设置密码
                    self._setpwd_block.fadeIn();
                    self._input_set_pwd1.val("");
                    self._input_set_pwd2.val("");
                    self._findpwd_block.hide();
                }
                else { alert("短信验证码输入有误"); }
            });

        });

        //set pwd 设置密码
        this._btn_set_ok.click(function () {

            var _phone = (self.setpwdBackTag == "reg" ? self._input_reg_phone.val() : (self.setpwdBackTag == "find" ? self._input_find_phone.val() : ""));
            if (!self.isMobile(_phone)) { alert('手机号码有误'); return; }

            var _code = (self.setpwdBackTag == "reg" ? self._input_reg_code.val() : (self.setpwdBackTag == "find" ? self._input_find_code.val() : ""));
            if (_code.length == 0) { alert('短信验证码有误'); return; }

            var _pwd1 = self._input_set_pwd1.val();
            if (_pwd1.length < 6) { alert('请输入有效的密码'); return; }

            var _pwd2 = self._input_set_pwd2.val();
            if (_pwd2.length < 6) { alert('请输入确认密码'); return; }

            if (_pwd1 != _pwd2) { alert('确认密码有误'); return; }

            //只使用忘记密码模块
            if (self._onlyUseForget) {

                //reset pwd
                $.post(self.verify.verifyResetPwdUrl, { password: _pwd2, code: _code, number: _phone }, 'json').then(function (result) {
                    if (result) {
                        alert('您的密码已经设置成功！马上去登录～');
                        self._input_login_phone.val(_phone);
                        self._input_login_pwd.val("");
                        self._input_reg_phone.val("");
                        self._input_reg_code.val("");
                        self._input_set_pwd1.val("");
                        self._input_set_pwd1.val("");
                        self.hide();
                        self._findpwd_block.show();
                        self._setpwd_block.hide();

                        try {
                            self._forgetCallback(_phone);
                        } catch (e) {

                        }
                    }
                    else { alert("抱歉，密码设置失败，请稍后重试"); }
                });
            }
            else {

                if (self.setpwdBackTag == "reg") {

                    var CID = $("#hidCurUserCID").val();

                    var unionid = $("#hidWxUnionid").val();

                    //reg
                    $.post(self.verify.verifyRegisterUrl, { number: _phone, password: _pwd2, code: _code, CID: CID, Unionid: unionid }, 'json').then(function (result) {
                        if (result == "") {
                            alert('恭喜您已注册成为周末酒店会员！马上去登录～');
                            self._input_login_phone.val(_phone);
                            self._input_login_pwd.val("");
                            self._input_reg_phone.val("");
                            self._input_reg_code.val("");
                            self._input_set_pwd1.val("");
                            self._input_set_pwd1.val("");
                            self._login_block.fadeIn();
                            self._setpwd_block.hide();
                        }
                        else { alert(result); }
                    });
                }
                else if (self.setpwdBackTag == "find") {

                    //reset pwd
                    $.post(self.verify.verifyResetPwdUrl, { password: _pwd2, code: _code, number: _phone }, 'json').then(function (result) {
                        if (result) {
                            alert('您的密码已经设置成功！马上去登录～');
                            self._input_login_phone.val(_phone);
                            self._input_login_pwd.val("");
                            self._input_reg_phone.val("");
                            self._input_reg_code.val("");
                            self._input_set_pwd1.val("");
                            self._input_set_pwd1.val("");
                            self._login_block.fadeIn();
                            self._setpwd_block.hide();
                        }
                        else { alert("抱歉，密码设置失败，请稍后重试"); }
                    });
                }
                else {
                    alert("非法操作");
                }
            }

        });

        //reg nopwd 手机号onchange事件
        this._input_reg_nopwd_phone.change(function () {
            var _phone = self._input_reg_nopwd_phone.val();
            if (_phone) {
                if (!self.isMobile(_phone)) { return; }
            }
            self.regnopwdRefInputState();
        });

        //reg nopwd 验证码onchange事件
        this._input_reg_nopwd_code.change(function () {
            var _code = self._input_reg_nopwd_code.val();
            if (_code) {
                if (_code.length == 0) { return; }
            }
            self.regnopwdRefInputState();
        });

        //reg nopwd 验证码获焦
        this._input_reg_nopwd_code.focus(function () {
            var _thisObj = $(this);
            self.reviewDivScrollTop(_thisObj);
        });

        //reg nopwd 邀请码获焦
        this._input_reg_nopwd_invitation.focus(function () {
            var _thisObj = $(this);
            self.reviewDivScrollTop(_thisObj);;
        });

        //reg nopwd 获取验证码
        this._reg_nopwd_getcode.click(function () {

            var _phone = self._input_reg_nopwd_phone.val();
            if (_phone.length != 11) { alert('您输入的' + _phone + '有误，请输入有效的11位手机号'); return; }
            if (!self.isMobile(_phone)) { alert('请输入有效的手机号'); return; }

            $.post(self.verify.verifyExistsMobileUrl, { number: _phone }, 'json').then(function (r) {
                if (r.isExists) {
                    alert("该手机号已注册");
                }
                else {
                    self.verify.send(_phone, function (lock) {
                        self._reg_nopwd_getcode.toggleClass('disabled', lock).prop('disabled', lock);
                        if (!lock) { self._reg_nopwd_getcode.text('重发验证码'); }
                    }, function (seconds) {
                        self._reg_nopwd_getcode.text(seconds + '秒后可重发');
                    });
                }
            });

        });

        //reg nopwd 注册
        this._btn_reg_nopwd_next.click(function () {

            var _phone = self._input_reg_nopwd_phone.val();
            if (!self.isMobile(_phone)) { alert('手机号码有误'); return; }

            var _code = self._input_reg_nopwd_code.val();
            if (_code.length == 0) { alert('短信验证码有误'); return; }

            var _invitationCode = self._input_reg_nopwd_invitation.val();
            console.log(_invitationCode);

            var _pwd1 = "";
            var _pwd2 = "";

            //follow status
            var _isFollow = self._reg_nopwd_ck.checked;

            //follow userid
            var _followUserId = self._followUserId;

            var CID = $("#hidCurUserCID").val();

            var unionid = $("#hidWxUnionid").val();

            //reg nopwd
            $.post(self.verify.verifyRegisterForInvitationUrl, { number: _phone, password: _pwd2, code: _code, invCode: _invitationCode, isFollow: _isFollow, followUserId: _followUserId, CID: CID, Unionid: unionid }, 'json').then(function (result) {
                if (result) {

                    if (result.Success) {

                        alert('登录APP所需的临时密码已随短信发送至你的手机，请妥善保管');
                        //self._input_reg_nopwd_phone.val(_phone);
                        //self._input_reg_nopwd_code.val("");

                        self._reg_nopwd_block.hide();
                        self._reg_nopwd_complate_block.show();
                    }
                    else {
                        alert(result.Message);
                    }
                }
                else { alert("请求错误，请重试或联系我们"); }
            });
        });

        //如果有设置关注好友的ID等信息，则默认显示邀请注册页面
        if (self._followUserId > 0) {
            this._login_block.hide();
            this._login2_block.hide();
            this._reg_nopwd_block.show();
        }

        //如果邀请码是前面传过来的，那么这里不允许清空&修改邀请码
        if (self._invitationCode && self._invitationCode.length > 0) {
            self._input_reg_nopwd_invitation.attr('disabled', true);
        }

        //是否显示关闭按钮
        if (self._hideCancel) {
            this._login_close.hide();
        }

        //关闭登录模块
        this._login_close.click(function () {

            if (cancelFun()) {
                self.hide();
            }

        });

        //login 登录页的手机快捷登录
        this._btn_gophonelogin.click(function () {
            self._login2_block.fadeIn();
            self._login_block.hide();
        });

        //phonelogin 手机登录页的注册账号登录
        this._btn_gologin.click(function () {
            self._login_block.fadeIn();
            self._login2_block.hide();
        });

        //jump to reg block
        this._login_goreg.click(function () {
            self._reg_block.fadeIn();
            self._input_reg_phone.val("");
            self._input_reg_code.val("");
            self._login_block.hide();
        });

        //jump to findpwd block
        this._login_findpwd.click(function () {

            var _login_phone = self._input_login_phone.val();
            self._input_find_phone.val(_login_phone);

            self._findpwd_block.fadeIn();
            self._login_block.hide();
        });

        //back to login block [reg]
        this._reg_back.click(function () {
            self._login_block.fadeIn();
            self._reg_block.hide();
        });

        //back to login block [find]
        this._find_back.click(function () {
            self._login_block.fadeIn();
            self._findpwd_block.hide();
        });

        //back to reg/find block
        this._set_back.click(function () {
            switch (self.setpwdBackTag) {
                case "reg": {
                    self._reg_block.fadeIn();
                    self._setpwd_block.hide();
                    break;
                }
                case "find": {
                    self._findpwd_block.fadeIn();
                    self._setpwd_block.hide();
                    break;
                }
                default: {
                    self._login_block.fadeIn();
                    self._setpwd_block.hide();
                    break;
                }
            }
        });
    },
    createLoginElement: function () {

        var self = this;

        var _loginHtml = "<div class='login-model' style='display:none;'></div>";
        _loginHtml += "<div class='login-section' style='display:none;'>";
        _loginHtml += "<div class='_login_block login-hide'>";
        _loginHtml += "<div class='titbar'><button class='_login_close'></button><div class='_login_goreg'><a href='javascript:;' class='_login_goreg'>还没有账号?</a></div></div>";
        _loginHtml += "<div class='_logo'><img src='http://whfront.b0.upaiyun.com/app/img/aboutapp-logo.png' alt='' /></div>";
        _loginHtml += "<div class='_row'><input type='tel' class='_input_def _input_login_phone' placeholder='输入手机号' /></div>";
        _loginHtml += "<div class='_row'><input type='password' class='_input_def _input_login_pwd' placeholder='输入密码' /></div>";
        _loginHtml += "<div class='_btns'><button class='_login_btn _btn_login'>登录</button><button class='_defult_btn _btn_gophonelogin'>验证码登录</button></div>";
        _loginHtml += "<div class='_foot'><a href='javascript:;' class='_blue_link2 _login_findpwd'>忘记密码?</a></div>";
        _loginHtml += "</div>";
        _loginHtml += "<div class='_login2_block'>";
        _loginHtml += "<div class='titbar'><button class='_login_close'></button></div>";

        if (self._onlyPhoneLogin) {
            _loginHtml += "<div class='_tit'>验证手机</div>";
        }
        else {
            _loginHtml += "<div class='_tit'>验证码登录</div>";
        }

        _loginHtml += "<div class='_row'><input type='tel' class='_input_def _input_login2_phone' placeholder='输入手机号' /></div>";
        _loginHtml += "<div class='_row'><input type='number' class='_input_code _input_login2_code' placeholder='输入验证码' /><a href='javascript:;' class='_getcode _login2_getcode'>发送验证码</a></div>";

        if (self._onlyPhoneLogin) {
            _loginHtml += "<div class='_btns'><button class='_login_btn _btn_login2'>确定</button></div>";
        }
        else {
            _loginHtml += "<div class='_btns'><button class='_login_btn _btn_login2'>登录</button><button class='_defult_btn _btn_gologin'>密码登录</button></div>";
        }

        _loginHtml += "</div>";
        _loginHtml += "<div class='_reg_block login-hide'>";
        _loginHtml += "<div class='titbar'><button class='_login_back _reg_back'></button></div>";
        _loginHtml += "<div class='_tit'>使用手机号注册</div>";
        _loginHtml += "<div class='_row'><input type='tel' class='_input_def _input_reg_phone' placeholder='输入手机号' /></div>";
        _loginHtml += "<div class='_row'><input type='number' class='_input_code _input_reg_code' placeholder='输入验证码' /><a href='javascript:;' class='_getcode _reg_getcode'>发送验证码</a></div>";
        _loginHtml += "<div class='_btns'><button class='_login_btn _btn_reg_next'>下一步</button></div>";
        _loginHtml += "<div class='_foot'><a href='http://www.zmjiudian.com/Account/ContractDescription' target='_blank' class='_blue_link1'>周末酒店用户协议</a></div>";
        _loginHtml += "</div>";
        _loginHtml += "<div class='_setpwd_block login-hide'>";
        _loginHtml += "<div class='titbar'><button class='_login_back _set_back'></button></div>";
        _loginHtml += "<div class='_tit'>设置密码</div>";
        _loginHtml += "<div class='_row'><input type='password' class='_input_def _input_set_pwd1' placeholder='设置6-13位数字和字母密码' /></div>";
        _loginHtml += "<div class='_row'><input type='password' class='_input_def _input_set_pwd2' placeholder='请再输入一遍' /></div>";
        _loginHtml += "<div class='_btns'><button class='_login_btn _btn_set_ok'>完成</button></div>";
        _loginHtml += "</div>";
        _loginHtml += "<div class='_findpwd_block login-hide'>";
        _loginHtml += "<div class='titbar'><button class='_login_back _find_back'></button></div>";
        _loginHtml += "<div class='_tit'>忘记密码</div>";
        _loginHtml += "<div class='_row'><input type='tel' class='_input_def _input_find_phone' placeholder='输入手机号' /></div>";
        _loginHtml += "<div class='_row'><input type='number' class='_input_code _input_find_code' placeholder='输入验证码' /><a href='javascript:;' class='_getcode _find_getcode'>发送验证码</a></div>";
        _loginHtml += "<div class='_btns'><button class='_login_btn _btn_find_next'>下一步</button></div>";
        _loginHtml += "</div>";
        _loginHtml += "<div class='_reg_nopwd_block login-hide'>";
        _loginHtml += "<div class='titbar' style='display:none;'><button class='_login_back _reg_nopwd_back'></button></div>";
        _loginHtml += "<div class='_tit2'>现在注册</div>";
        _loginHtml += "<div class='_row'><input type='tel' class='_input_def _input_reg_nopwd_phone' placeholder='输入手机号' /></div>";
        _loginHtml += "<div class='_row'><input type='number' class='_input_code _input_reg_nopwd_code' placeholder='输入验证码' /><a href='javascript:;' class='_getcode _reg_nopwd_getcode'>发送验证码</a></div>";
        _loginHtml += "<div class='_row'><input type='text' class='_input_def _input_def2 _input_reg_nopwd_invitation' placeholder='输入邀请码（选填）' value='" + self._invitationCode + "' /></div>";
        //_loginHtml += "<div class='_row'><span class='_text'>使用邀请码，你和你的朋友都将获得50积分</span></div>";
        _loginHtml += "<div class='_row'><span class='_text'></span></div>";
        _loginHtml += "<div class='_ctrl'><input type='checkbox' class='_ck' checked='checked' name='_ck_follow' /><label for='_ck_follow'>关注" + self._followNickName + "动态</label></div>";
        _loginHtml += "<div class='_btns _btns2'><button class='_login_btn _btn_reg_nopwd_next' disabled='true'>注册</button></div>";
        _loginHtml += "<div class='_foot'><a href='http://www.zmjiudian.com/Account/ContractDescription' target='_blank' class='_blue_link1'>周末酒店用户协议</a></div>";
        _loginHtml += "</div>";
        _loginHtml += "<div class='_reg_nopwd_complate_block login-hide'>";
        _loginHtml += "<div class='_row'><img class='_top_img' src='http://whfront.b0.upaiyun.com/app/img/fund/invitation-reg-complate-topbg.png' alt='' /></div>";
        _loginHtml += "<div class='_row'>";
        _loginHtml += "<div class='_alert-t1'>注册成功</div>";
        _loginHtml += "<div class='_alert-t2'>请到周末酒店APP->我的页面<br />查看已获得的积分</div>";
        _loginHtml += "</div>";
        _loginHtml += "<div class='_btns _btns2'>";
        _loginHtml += "<a class='_downapp_btn' href='http://app.zmjiudian.com' target='_blank'>下载周末酒店APP</a>";
        _loginHtml += "</div>";
        _loginHtml += "<div class='_btns _btns2'>";
        _loginHtml += "<a class='_look_btn' href='/App/MorePackageList?albumId=10' target='_blank'>查看优惠酒店</a>";
        _loginHtml += "</div>";
        _loginHtml += "</div>";
        _loginHtml += "</div>";

        $('body').append(_loginHtml);
    },

    //显示 登录
    show: function () {

        var self = this;

        //显示登录模块
        self._loginModel.show();
        //self._loginSection.css("top", $('body').scrollTop());
        self._loginSection.show();
    },

    //显示 忘记密码
    showForget: function (cb) {

        var self = this;

        //隐藏登录模块
        self._login_block.hide();
        self._login2_block.hide();

        //显示忘记密码模块
        self._findpwd_block.show();

        //忘记密码成功后的call back（回调时会返回当前重置密码的手机号供后续使用）
        if (cb) { self._forgetCallback = cb; }

        //重置返回事件
        self._find_back.unbind("click");
        self._find_back.click(function () { self.hide(); });

        //标记只显示忘记密码
        self._onlyUseForget = true;

        //显示登录模块
        self._loginModel.show();
        //self._loginSection.css("top", $('body').scrollTop());
        self._loginSection.show();
    },

    hide: function () {
        this._loginModel.hide();
        this._loginSection.hide();
    },

    isMobile: function (val) {
        return phoneNumReg.test(val);
    },

    //邀请注册的基本输入检验
    regnopwdRefInputState: function () {
        var self = this;
        var _phone = self._input_reg_nopwd_phone.val();
        var _code = self._input_reg_nopwd_code.val();
        if (self.isMobile(_phone) && _code.length > 0) {
            self._btn_reg_nopwd_next.attr('disabled', false);
        }
        else {
            self._btn_reg_nopwd_next.attr('disabled', true);
        }
    },

    _reviewDivScrollTopTimer: null,
    reviewDivScrollTop: function (_thisObj) {
        var self = this;
        clearTimeout(self._reviewDivScrollTopTimer);
        self._reviewDivScrollTopTimer = setTimeout(function () {
            self._loginSection.animate({ scrollTop: _thisObj.offset().top - 100 }, 300);
        }, 200);
    },

    verify: {
        verifyUrl: window.httpsWebUrl + 'Account/Verify',
        verifyGeetestUrl: window.httpsWebUrl + 'Account/VerifyGeetest',
        verifyNewUserUrl: window.httpsWebUrl + 'Account/VerifyNewUser',
        verifyCkLoginGeetestUrl: window.httpsWebUrl + 'Account/VerifyLoginGeetest',
        verifyCkLoginUrl: window.httpsWebUrl + 'Account/VerifyLogin',
        verifyCkLoginByUidUrl: window.httpsWebUrl + 'Account/VerifyLoginByUid',
        verifyGoLoginUrl: '/Account/Login',
        verifyRegisterUrl: window.httpsWebUrl + 'Account/Register',
        verifyRegisterForInvitationUrl: window.httpsWebUrl + 'Account/RegisterForInvitation',
        verifyResetPwdUrl: window.httpsWebUrl + 'Account/ResetPasswordWithPhone',
        verifyExistsMobileUrl: window.httpsWebUrl + 'Account/ExistsMobileAccount',
        verifyExistsMobileGeetestUrl: window.httpsWebUrl + 'Account/ExistsMobileAccountGeetest',
        _loginConfigCacheKey: "zmjd_login_cache",
        send: function (number, lock, update) {
            var self = this;
            if (self._timer) {
                return;
            }
            self._startTimer(lock, update);
            $.post(self.verifyUrl, {
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
        sendAndGeetest: function (geetest_challenge, geetest_seccode, geetest_validate, number, lock, update) {
            var self = this;
            if (self._timer) {
                return;
            }
            self._startTimer(lock, update);
            $.post(self.verifyGeetestUrl, {
                action: 'sendAndGeetest',
                number: number,
                geetest_challenge: geetest_challenge,
                geetest_seccode: geetest_seccode,
                geetest_validate: geetest_validate
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
                CID: $("#hidCurUserCID").val(),
                Unionid: $("#hidWxUnionid").val()
            }, 'json').then(function (r) {
                return r.ok || true ? contacts._store(number) : $.Deferred().reject(new Error('短信校验码有误'));
            });
        },
        autoLogin: function (callBack) {

            var self = this;

            var _loginConfigCache = self.getLoginConfigCache();
            if (_loginConfigCache && _loginConfigCache.token) {

                //go login
                $.post(self.verifyGoLoginUrl, _loginConfigCache, 'json')
                    .then(function (data) {
                        if (data.Message == '') {
                            callBack(_loginConfigCache.uid);
                            //location.reload(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
                        }
                        else {
                            alert("自动登录失败");
                        }
                    }).fail(function () {
                        console.log('网络请求失败');
                        //alert('网络请求失败');
                    })

                return true;
            }

            return false;
        },
        loginByUid: function (uid, callBack) {

            var self = this;

            //login
            $.post(self.verifyCkLoginByUidUrl, { uid: uid }, 'json').then(function (r) {
                if (r.Message == '') {

                    var token = r.token;
                    var isv = r.isv;
                    var nn = r.nn;
                    var uid = r.uid;

                    var _loginDic = { token: token, isv: isv, nn: nn, uid: uid };

                    //cache login config
                    self.setLoginConfigCache(_loginDic);

                    //go login
                    $.post(self.verifyGoLoginUrl, _loginDic, 'json')
                        .then(function (data) {
                            if (data.Message == '') {
                                try {
                                    callBack(uid);
                                    //location.reload(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
                                } catch (e) {

                                }
                            }
                            else {
                                console.log("登录失败");
                            }
                        }).fail(function () {
                            console.log('网络请求失败');
                        })

                }
                else {
                    console.log('登录失败');
                }
            }).fail(function (e) {
                //console.log(e)
                console.log('网络请求失败');
            })

        },
        setLoginConfigCache: function (data) {
            Store.Set(this._loginConfigCacheKey, data);
        },
        getLoginConfigCache: function () {
            return Store.Get(this._loginConfigCacheKey);
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
    }
}