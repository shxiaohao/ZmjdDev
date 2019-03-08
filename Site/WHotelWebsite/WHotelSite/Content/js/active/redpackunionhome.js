var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var isGetThisRedpack = $("#isGetThisRedpack").val() == "1";
var activeid = parseInt($("#activeid").val());
var showKeyword = $("#showKeyword").val();
var ficName = $("#ficName").val();
var partnerid = parseInt($("#partnerid").val());
var thisRedpackCost = parseInt($("#thisRedpackCost").val());
var gemCount = parseInt($("#gemCount").val());
var headimgurl = $("#headimgurl").val();
var shareLink = $("#shareLink").val();
var goget = $("#goget").val() == "1";

var _Config = new Config();

//是否已生成海报
var isLoadPoster = false;

$(function () {

    var checkState = function () {

        if (partnerid > 0 && partnerid != 7) {

            //如果没有领取过当前红包，判断当前宝石是否足够
            if (!isGetThisRedpack && gemCount < thisRedpackCost) {

                _Modal.show({
                    title: ficName + '不够哦～',
                    content: "兑换此红包需要" + thisRedpackCost + "颗" + ficName + "，你当前的" + ficName + "还不够哦，请前往活动主页获取更多" + ficName + "哦～",
                    confirmText: '前往',
                    confirm: function () {

                        _Modal.hide();

                        //跳转至活动主页
                        gourl("/wx/Active/RedpackUnionHome/7/" + activeid + "?goget=1");
                    },
                    showCancel: false,
                    showClose: true
                });
                return;
            }
        }
        else {


        }
        
    }
    checkState();
    
    if (partnerid > 0 && partnerid != 7) {

        //单个合作伙伴的获取更多红包
        $(".get-moregem-btn").click(function () {

            //跳转至活动主页
            gourl("/wx/Active/RedpackUnionHome/7/" + activeid + "?goget=1");

            //var _msg = "长按识别下方二维码，关注“周末酒店服务号”，回复“红包”去获得更多红包";
            //_msg += "<br/ ><br/ ><center><img src='http://whfront.b0.upaiyun.com/app/img/qrcode-wx-zmjdservice-240x240.jpg' style='width:50%;' alt='二维码'></center>";

            //_Modal.show({
            //    title: '',
            //    content: _msg,
            //    confirmText: '知道了',
            //    confirm: function () {
            //        _Modal.hide();
            //    },
            //    showCancel: false,
            //    showClose: true
            //});

            //$("._modal-section").css("top", "22%");
        });
    }
    else {

        //滚动到领取红包区域
        var scrollGemSection = function () {

            $("html,body").animate({ scrollTop: $(".gem-section").offset().top - 45 }, 300);
        }

        //参数控制自动滚动到领取区域
        if (goget) {
            scrollGemSection();
        }

        //立即领取 待领取红包
        $(".go-btn").click(function () {

            scrollGemSection();
        });

        //获取更多宝石
        $(".gem-share-btn").click(function () {

            showSharePoster();
        });

        //合作伙伴的领取功能
        $(".get-partner-btn").each(function () {

            var _thisObj = $(this);
            _thisObj.click(function () {

                var _pname = _thisObj.data("pname");
                var _headurl = _thisObj.data("headurl");
                var _pqrcode = _thisObj.data("pqrcode");
                var _cost = parseInt(_thisObj.data("cost"));
                if (gemCount >= _cost) {

                    var _msg = "<div class='alert-p-head'><img src='" + _headurl + "' alt='' /></div>";
                    _msg += "<div class='alert-p-tit'>" + _pname + "</div>";
                    _msg += "<div class='alert-p-desc'>长按识别下方二维码关注“" + _pname + "”公众号，回复“" + showKeyword + "”即可获得红包</div>";
                    _msg += "<div><center><img src='" + _pqrcode + "' style='width:50%;' alt='二维码加载失败'></center></div>";

                    _Modal.show({
                        title: '',
                        content: _msg,
                        confirmText: '知道了',
                        confirm: function () {
                            _Modal.hide();
                        },
                        showCancel: false,
                        showClose: false
                    });

                    $("._modal-section").css("top", "16%");
                }
                else {

                    _Modal.show({
                        title: ficName + '不够哦～',
                        content: "兑换此红包需要" + _cost + "颗" + ficName + "，你当前的" + ficName + "还不够哦，试试获取更多" + ficName + "吧",
                        confirmText: '获取更多' + ficName,
                        confirm: function () {

                            _Modal.hide();

                            //弹出海报
                            showSharePoster();
                        },
                        showCancel: false,
                        showClose: true
                    });
                }

            });
        });

        var _bannerImgLoad = true;
        var _qrcodeBgImgLoad = true;
        var _footLogoImgLoad = true;
        $(".mine-poster .p-banner img").load(function () {

            _bannerImgLoad = true;
        });
        $(".mine-poster .p-qrcode-section .bg img").load(function () {

            _qrcodeBgImgLoad = true;
        });
        $(".mine-poster .foot img").load(function () {

            _footLogoImgLoad = true;
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

        ////加载头像
        ////将微信头像上传并生成zmjd下的图片地址
        //$.get(_Config.APIUrl + '/api/WeixinApi/UploadWeixinQrcodeImg', { oriQrcodeImg: headimgurl }, function (_qrcodeData) {

        //    console.log(_qrcodeData);

        //    //产品图片（ios微信环境下，如果图片之间加载过有缓存，不会执行load事件，所以这里统一加上时间戳，MMP 20180109 haoy）
        //    var timestamp = Date.parse(new Date());
        //    $(".mine-poster .head-img img").attr("src", _qrcodeData);
        //    $(".mine-poster .head-img img").load(function () {

        //    });
        //});

        var showSharePoster = function () {

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

            var genPoster = function () {

                if (_bannerImgLoad && _qrcodeBgImgLoad && _footLogoImgLoad) {

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

                                        isLoadPoster = true;

                                        _Modal.show({
                                            title: "分享海报赢" + ficName,
                                            content: "长按保存下方海报并分享给好友，每有一位好友扫码参与，你就能获得一颗" + ficName + "，多劳多得，快去行动吧～",
                                            confirmText: '知道了',
                                            confirm: function () {

                                                _Modal.hide();

                                                $(".poster-tip").fadeIn(500);
                                            },
                                            showCancel: false,
                                            showClose: false
                                        });

                                    }, 200);
                                });

                            });

                        }, 200);
                    }
                }
            }
            genPoster();
        }

        //android下宝石位置调优
        if (B.v.android) {

            $(".gem-icon-big").css("top", "0.05em");

            $(".gem-icon-sml").each(function () {
                $(this).css("top", "0.1em");
            });
        }
    }
});
