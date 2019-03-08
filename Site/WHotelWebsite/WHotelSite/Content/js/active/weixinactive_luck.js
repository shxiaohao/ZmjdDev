var needpaysign = $("#needpaysign").val();
var payprice = $("#payprice").val();
var returnprice = $("#returnprice").val();
var shareLink = $("#shareLink").val();
var activeType = $("#type").val();
var activeId = $("#activeId").val();
var openid = $("#openid").val();
var showThreePosterCord = $("#showThreePosterCord").val() == "1";
var posterCordAlertKey = $("#posterCordAlertKey").val();
var weixinAcountId = 7;

var _Config = new Config();

$(function () {

    //是否已生成海报
    var isLoadPoster = false;

    //验证码操作
    $(".vCodeBtn").click(function () {
        var userPhone = $("#regtell").val();
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

    window.httpsWebUrl = "/";
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

    function regXmas()
    {
        var activeid = $("#activeid").val();
        var partnerid = $("#partnerid").val();
        var openid = $("#openid").val();
        var unionid = $("#unid").val();
        var username = $("#username").val();

        //如果是活动类型3（免费住(需姓名)）的，则要验证regName
        if (activeType == "3") {
            username = $("#regName").val();

            if (username == "") {
                alert("请输入您的姓名");
                return;
            }
        }

        var phone = $("#regtell").val();
        if (phone == "") {
            alert("请输入您的手机号");
            return;
        }

        if (phone.length != 11) {
            alert("手机号码有误");
            return;
        }

        var dic = {};
        dic["openid"] = openid;
        dic["unionid"] = unionid;
        dic["username"] = username;
        dic["phone"] = phone;
        dic["activeid"] = activeid;
        dic["partnerid"] = partnerid;
        dic["needpaysign"] = needpaysign;
        dic["payprice"] = payprice;
        dic["returnprice"] = returnprice;

        $.get('/Active/SignUpWeixinLuckActive', dic, function (dic) {
            if (dic.state == "-1")
            {
                //强制关注
                $(".mustfollow-md").show();
                $(".mustfollow-panel").show();
            }
            else if (dic.state == "1")
            {
                ////现在全部直接报名成功（20180517微信宣布禁止获取用户分享状态，于20180705生效。 2018.07.06 haoy）
                //location.href = "/Active/Weixin_LuckActive_ShareDone/" + activeid + "?shared=1&swfund=0&openid=" + openid;

                //if (partnerid == 17 || partnerid == 18 || partnerid == 45 || partnerid == 47 || partnerid == 49 || partnerid == 50) {
                if (partnerid != 7 && partnerid != 50 && partnerid != 51) {

                    //尚旅入口报名，则还是跳到强制分享页面
                    location.href = "/Active/Weixin_LuckActive_RegDone/" + activeid + "?openid=" + openid;
                }
                else {

                    //其他报名成功，直接跳转至分享完成页面
                    location.href = "/Active/Weixin_LuckActive_ShareDone/" + activeid + "?shared=1&swfund=0&openid=" + openid;
                }
            }
            else
            {
                //失败
                alert("报名失败，请重试！");
            }
        });
    }
    $(".submit").click(regXmas);

    //立即支付
    function subpay()
    {
        var activeid = $("#activeid").val();
        var partnerid = $("#partnerid").val();
        var openid = $("#openid").val();
        var unionid = $("#unid").val();
        var regName = $("#regName").val();
        var phone = $("#regtell").val();
        var vCode = $("#vCode").val();

        //if (regName == "") {
        //    alert("请输入您的姓名");
        //    return;
        //}

        if (phone == "") {
            alert("请输入您的手机号");
            return;
        }

        if (phone.length != 11) {
            alert("手机号码有误");
            return;
        }

        if (vCode == "") {
            alert("请输入短信验证码");
            return;
        }

        $.post(verifyNewUserUrl, {
            action: 'check',
            number: phone,
            code: vCode,
            CID: $("#hidCurUserCID").val()
        }, 'json').then(function (r) {
            if (r.ok == "1") {
                
                var dic = {};
                dic["openid"] = openid;
                dic["unionid"] = unionid;
                dic["username"] = regName;
                dic["phone"] = phone;
                dic["activeid"] = activeid;
                dic["partnerid"] = partnerid;
                dic["needpaysign"] = needpaysign;
                dic["payprice"] = payprice;
                dic["returnprice"] = returnprice;

                $.get('/Active/SignUpWeixinLuckActive', dic, function (dic) {
                    //强制关注
                    if (dic.state == "-1") {

                        $(".mustfollow-md").show();
                        $(".mustfollow-panel").show();
                    }
                        //成功
                    else if (dic.state == "1") {
                        location.href = "/Active/Weixin_LuckActive_RegDone/" + activeid + "?openid=" + openid;
                    }
                        //去支付
                    else if (dic.state == "2") {
                        location.href = dic.payurl;
                    }
                    else {
                        //失败
                        alert("报名失败，请重试！");
                    }
                });

            }
            else {
                alert("短信验证码输入有误");
            }
        });
    }
    $(".subpay").click(subpay);

    //分享给好友
    $("#sharefd-btn").click(function ()
    {
        $(".sharetofriend-panel").fadeIn(300);
        $(".sharetofriend-md").fadeIn(300);
    });
    $(".sharetofriend-md").click(function ()
    {
        $(".sharetofriend-md").fadeOut(300);
        $(".sharetofriend-panel").fadeOut(300);
    });
    $(".sharetofriend-panel").click(function () {
        $(".sharetofriend-md").fadeOut(300);
        $(".sharetofriend-panel").fadeOut(300);
    });
    $("#close-fundalter").click(function () {
        $(".fundalter-md").fadeOut(300);
        $(".fundalter-panel").fadeOut(300);
    });

    var _bannerImgLoad = true;
    var _qrcodeBgImgLoad = true;
    var _footLogoImgLoad = true;
    $(".mine-poster .banner img").load(function () {

        _bannerImgLoad = true;
    });
    $(".mine-poster .bg-img img").load(function () {

        _bannerImgLoad = true;
    });
    $(".mine-poster .stamp img").load(function () {

        _qrcodeBgImgLoad = true;
    });

    //加载分享二维码
    var qrcodeContent = shareLink; //("http://192.168.1.25:8081/wx/Active/RedpackUnionHome/50/498");
    var qrcode = new QRCode('poster-qrcode', {
        text: qrcodeContent,
        width: 115,
        height: 115,
        colorDark: '#000000',
        colorLight: '#ffffff',
        correctLevel: QRCode.CorrectLevel.L
    });

    //显示分享海报
    var showSharePoster = function () {

        var genPoster = function () {

            if (_bannerImgLoad && _qrcodeBgImgLoad && _footLogoImgLoad) {

                $(".poster-tip-loading").show();
                $(".poster-bg").show();
                $(".mine-poster").show();

                $(".poster-bg").click(function () {

                    $("#showPosterSection").hide();
                    $(".poster-tip").hide();
                    $(".poster-tip-loading").hide();
                    $(".poster-bg").hide();
                    $(".mine-poster").hide();
                });

                $(".poster-tip").click(function () {

                    $("#showPosterSection").hide();
                    $(".poster-tip").hide();
                    $(".poster-tip-loading").hide();
                    $(".poster-bg").hide();
                    $(".mine-poster").hide();
                });

                if (isLoadPoster) {

                    //$("#showPosterSection").fadeIn(500);
                    $("#showPosterSection").slideDown();
                    $(".poster-tip").fadeIn(500);
                    $(".poster-tip-loading").hide();
                    $(".mine-poster").hide();
                }
                else {

                    setTimeout(function () {

                        html2canvas($(".mine-poster")[0], { useCORS: true }).then(function (canvas) {

                            //console.log(canvas.toDataURL());

                            $("#showImg").attr("src", canvas.toDataURL());
                            $("#showImg").load(function () {

                                setTimeout(function () {

                                    $("#showPosterSection").slideDown();    //$("#showPosterSection").fadeIn(500);
                                    $(".poster-tip-loading").hide();
                                    $(".mine-poster").hide();
                                    $(".poster-tip").fadeIn(500);

                                    isLoadPoster = true;

                                    //_Modal.show({
                                    //    title: "分享海报赢" + ficName,
                                    //    content: "长按保存下方海报并分享给好友，每有一位好友扫码参与，你就能获得一颗" + ficName + "，多劳多得，快去行动吧～",
                                    //    confirmText: '知道了',
                                    //    confirm: function () {

                                    //        _Modal.hide();

                                    //        $(".poster-tip").fadeIn(500);
                                    //    },
                                    //    showCancel: false,
                                    //    showClose: false
                                    //});

                                }, 200);
                            });

                        });

                    }, 200);
                }
            }
            else {
                alert("准备中，请稍后重试");
            }
        }
        genPoster();
    }
    $(".share-poster-float").click(showSharePoster);

    /************** 翻倍卡 start **************/

    if (showThreePosterCord) {

        //弹出翻倍卡
        var openLuckCord = function () {

            $(".luck-cord .cord-close").hide();
            $('.luck-cord').show();
            $(".luck-cord .main-cord").animate({ top: '10%' }, 800)
            setTimeout(function () { $(".luck-cord .cord-close").show(); }, 1200);
        }

        //关闭翻倍卡
        var closeLuckCord = function () {

            $('.luck-cord').fadeOut();
        }

        //翻倍卡的翻牌事件
        $('.luck-cord .main-cord').click(function () {

            $('.luck-cord .main-cord').removeClass("m-cord-1");
            $('.luck-cord .main-cord').addClass("m-cord-0");

            setTimeout(function () {

                $('.luck-cord .main-cord').hide();
                $('.luck-cord .poster-cord').show();

                $('.luck-cord .poster-cord').addClass("p-cord-1");
                $('.luck-cord .poster-cord').removeClass("p-cord-0");

                setTimeout(function () {

                    //生成海报卡片
                    if (luckCordQrcodeImgLoaded) {
                        loadPoster();
                    }
                    else {
                        setTimeout(function () { loadPoster(); }, 1000);
                    }

                }, 1000);

            }, 200);
        });

        //加载并显示海报
        var luckCordQrcodeImgLoaded = false;
        var luckCordPosterIsloaded = false;
        var loadPoster = function () {

            if (luckCordQrcodeImgLoaded) {

                console.log(123)

                $(".luck-cord .poster-cord .poster-cord-result").hide();
                //$(".luck-cord .poster-cord .poster-cord-loadding").show();
                //$(".luck-cord .poster-cord .poster-cord-init").show();

                setTimeout(function () {

                    var _posterOriginElm = $(".luck-cord .poster-cord .poster-cord-init")[0];

                    html2canvas(_posterOriginElm, { useCORS: true }).then(function (canvas) {

                        setTimeout(function () {

                            console.log("海报生成ok")
                            //console.log(canvas.toDataURL());

                            $("#cordShowImg").attr("src", canvas.toDataURL());
                            $("#cordShowImg").unbind("load");
                            $("#cordShowImg").load(function () {

                                setTimeout(function () {

                                    console.log("显示海报")

                                    luckCordPosterIsloaded = true;

                                    $(".luck-cord .poster-cord .poster-cord-result").show();
                                    $(".luck-cord .poster-cord .poster-cord-loadding").hide();
                                    $(".luck-cord .poster-cord .poster-cord-init").hide();

                                    //显示分享提示
                                    var _winHeight = $(window).height();
                                    var _posterTop = $('.luck-cord .poster-cord').offset().top;
                                    var _posterHeight = $('.luck-cord .poster-cord').height();
                                    $(".luck-cord .cord-share-tip").animate({ bottom: (_winHeight - _posterTop - _posterHeight - 30) }, 300)

                                    //记录已经弹出过
                                    Store.Set(posterCordAlertKey, "1");

                                }, 0);
                            });

                        }, 500);

                    });

                }, 100);
            }
        }

        //生成个人投票专属二维码
        var genMineVoteQrcode = function () {

            //获取当前发起助力的专属二维码
            //GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID
            var _sceneStr = "qrscene_ACTIVELUCKCORD_{0}~{1}~{2}".format(activeId, 3, openid);

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
                    console.log(_src);

                    //转换成zmjd域名下的地址
                    _src = "http://api.zmjiudian.com/api/photo/GetZmjdImgByUrl?url={0}".format(_src);

                    $("#poster-cord-qrcode-img").attr("src", _src);
                    $("#poster-cord-qrcode-img").load(function () {

                        console.log("二维码 ok")

                        luckCordQrcodeImgLoaded = true;

                        //显示海报放在卡片翻过来的时候，不然没法生成图片
                        //loadPoster();
                    });

                }
                else {

                    console.log("专属二维码生成失败");
                }

            });
        }

        //初始翻倍卡及生成卡片
        var initLuckCord = function () {

            //检查是否已经谈过了
            //是否弹出过二维码
            var _posterCordAlertStore = Store.Get(posterCordAlertKey);
            //如果来自自己，点击去拉票直接生成海报
            if (!_posterCordAlertStore) {

                //关闭事件
                $(".luck-cord .cord-close").click(closeLuckCord);

                //加载个人专属二维码和卡片
                genMineVoteQrcode();

                //弹出翻倍卡
                openLuckCord();
            }
        }
        initLuckCord();
    }

    /************** 翻倍卡 end **************/
});