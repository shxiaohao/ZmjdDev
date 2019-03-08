var userId = $("#userid").val();
var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var shareNativeLink = $("#shareNativeLink").val();

var tempid = $("#tempdataid").val();
var _Config = new Config();

$(function () {

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    

    var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
    var redcashcouponactivity = new Vue({
        el: "#redcashcoupon",
        data: {
            received: false,
            template: {
                title: "",
                share: {},
                topbanner: {},
                cashcouponids: {},
                getbtn: {},
                gobtn: {},
                tippng: {},
                cashcouponlist: {},
                isexpire: false,
                isshare: false
            }
        },
        methods: {
            vueReceiveRedCashCoupon: function () {

                //检测登录并自动登录
                if (!isapp && userId == "0") {
                    _loginModular.verify.autoLogin(loginCheckFun);
                }

                //如果需要分享才能领取..
                if (redcashcouponactivity.template.isshare) {

                    if (isapp) {

                        //app分享配置
                        var param = { title: redcashcouponactivity.template.share.sharetitle, content: redcashcouponactivity.template.share.sharedesc, photoUrl: redcashcouponactivity.template.share.sharepic, shareLink: redcashcouponactivity.template.share.sharelink };
                        zmjd.setShareConfig(param);

                        //app内默认6秒钟后自动领取
                        setTimeout(function () {

                            //领取并弹出
                            receiveRedCashCoupon();

                        }, 6000);

                        //app内弹出分享
                        gourl(shareNativeLink);
                    }
                    else {

                        //显示右上角分享提示
                        $(".weixin-share-tip").show();

                        //加载微信分享配置
                        loadWechat(redcashcouponactivity.template.share.sharetitle, redcashcouponactivity.template.share.sharedesc, redcashcouponactivity.template.share.sharelink, redcashcouponactivity.template.share.sharepic, receiveRedCashCoupon)
                    }
                }
                else {

                    //直接领取
                    receiveRedCashCoupon();
                }
            },
            vueHideWXTip: function () {

                //隐藏分享提示
                $(".weixin-share-tip").hide()
            }
        }

    })

    //获取模板信息
    var pcDic = { "id": tempid };
    $.get(_Config.APIUrl + "/api/HotelTheme/GetTempSource", pcDic, function (_templateData) {

        Vue.nextTick(function () {

            if (userId == 0) {
                //app环境下，如果没有登录则弹出登录
                if (isapp) {
                    location.href = loginapphref;
                    return;
                }
                else {
                    _loginModular.show();
                }
            }

            for (var i = 0; i < _templateData.ContentList.length; i++) {
                switch (_templateData.ContentList[i].Code) {
                    case "title":
                        redcashcouponactivity.template.title = $.parseJSON(_templateData.ContentList[i].Content).title;
                        document.title = redcashcouponactivity.template.title;
                        redcashcouponactivity.template.share = $.parseJSON(_templateData.ContentList[i].Content).share;
                        break;
                    case "topbanner":
                        redcashcouponactivity.template.topbanner = $.parseJSON(_templateData.ContentList[i].Content);
                        break;
                    case "cashcouponids":
                        redcashcouponactivity.template.cashcouponids = _templateData.ContentList[i].Content;
                        break;
                    case "getbtn":
                        redcashcouponactivity.template.getbtn = $.parseJSON(_templateData.ContentList[i].Content);
                        break;
                    case "gobtn":
                        redcashcouponactivity.template.gobtn = $.parseJSON(_templateData.ContentList[i].Content);
                        //特殊处理
                        if (isapp && redcashcouponactivity.template.gobtn.appactionurl) {
                            redcashcouponactivity.template.gobtn.actionurl = redcashcouponactivity.template.gobtn.appactionurl;
                        }
                        break;
                    case "tippng":
                        redcashcouponactivity.template.tippng = $.parseJSON(_templateData.ContentList[i].Content);
                        break;
                    case "activitydate":
                        var expire = $.parseJSON(_templateData.ContentList[i].Content)
                        var currdate = new Date();
                        var expiredate = new Date(Date.parse(expire.expiretime));
                        if (currdate > expiredate) {
                            redcashcouponactivity.template.isexpire = true;
                        }
                        break;
                    case "isshare":
                        redcashcouponactivity.template.isshare = $.parseJSON(_templateData.ContentList[i].Content);
                        break;

                }
            }
            getRedCashCouponList();
            checkUserReceived();

            //默认初始化初始化分享
            if (inWeixin && redcashcouponactivity.template.isshare) {
                loadWechat(redcashcouponactivity.template.share.sharetitle, redcashcouponactivity.template.share.sharedesc, redcashcouponactivity.template.share.sharelink, redcashcouponactivity.template.share.sharepic, function () { });
            }

            //暂时app不支持这样使用
            //if (isapp)
            //{
            //    onAppShareCall(redcashcouponactivity.template.share.sharetitle, redcashcouponactivity.template.share.sharedesc, redcashcouponactivity.template.share.sharelink, redcashcouponactivity.template.share.sharepic);
            //}
        })

        console.log(redcashcouponactivity.template)
    })

    //已过期并且领取过的用户不弹窗
    var istanchuang = function () {
        if (redcashcouponactivity.template.isexpire == true && redcashcouponactivity.received == false) {
            _Modal.show({
                title: '抱歉，你来晚了',
                content: '当前活动已结束，请持续关注我们获取更多活动信息。',
                confirmText: '我知道了',
                confirm: function () {
                    _Modal.hide();
                },
                showCancel: false,
                cancelText: '',
                cancel: function () {
                    _Modal.hide();
                }
            })
        }
    }

    //获取红包列表
    var getRedCashCouponList = function () {
        $.ajax({
            type: "GET",
            data: { cashCouponIds: redcashcouponactivity.template.cashcouponids },
            url: _Config.APIUrl + "/api/coupon/GetRedCashCouponList",
            success: function (_data) {
                console.log(_data);
                _data.forEach(function (value, index, _data) {
                    _data[index].StartDate = value.StartUseDate.split("T")[0].replace(/-/g, '/');
                    _data[index].ExpiredDate = value.ValidUntilDate.split("T")[0].replace(/-/g, '/');
                })
                redcashcouponactivity.template.cashcouponlist = _data;
            }
        })
    }

    //检查是否已经领取过
    var checkUserReceived = function () {
        $.get(_Config.APIUrl + "/api/coupon/CheckUserCouponItemByUserIdAndCouponDefineIdlist", { userId: userId, cashCouponIds: redcashcouponactivity.template.cashcouponids }, function (_result) {
            redcashcouponactivity.received = _result;
            istanchuang();

        })
    }

    //领取红包
    var receiveRedCashCoupon = function () {
        $.get(_Config.APIUrl + "/api/coupon/SendCashCoupon", { userId: userId, cashCouponIds: redcashcouponactivity.template.cashcouponids }, function (_result) {
            redcashcouponactivity.received = _result;

            //隐藏分享提示
            $(".weixin-share-tip").hide()
        })
    }
})

