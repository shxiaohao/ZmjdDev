var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var activeId = $("#activeId").val();
var openid = $("#openid").val();
var activeDrawId = $("#activeDrawId").val();
var nickName = $("#nickName").val();
var drawPhone = $("#drawPhone").val();
var ruleExId = $("#ruleExId").val();
var urlfrommine = parseInt($("#urlfrommine").val());
var isurlfrommine = urlfrommine > 0;

//头像图片ID
var headImgUploadId = $("#headImgUploadId").val();

var _Config = new Config();

var activeData;

$(function () {

    //init data
    var init = function () {

        //初始订单详情
        activeData = new Vue({
            el: "#active-obj",
            data: {
                "activeInfo": {},
                "thisVoteResult": {},
                "voteInfo": {
                    "thisVoteInfo": null,
                    "voteResultList": [],
                }
            }
        })

        ////检查是否需要上传微信头像
        //if (headImgUploadId && parseInt(headImgUploadId)) {

        //    //获取活动信息
        //    var _imgdic = { "activeId": activeId, "openid": openid, "imgId": headImgUploadId };
        //    $.get("/Active/UploadDrawHeadimg", _imgdic, function (_imgdata) {

        //        console.log(_imgdata);
        //        if (_imgdata) {

        //            if (_imgdata.ImageUrl) {
        //                $("#user-head-img").attr("src", _imgdata.ImageUrl);
        //            }

        //            Vue.nextTick(function () {


        //            });
        //        }

        //    });
        //}

    }
    init();

    var loadActiveInfo = function () {

        //获取活动信息
        var _dic = { "activeId": activeId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetOneWeixinActive", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                activeData.activeInfo = _data;

                Vue.nextTick(function () {


                });
            }
            
        });
    }
    loadActiveInfo();

    //获取活动参与者（酒店方）投票结果
    var loadVoteResultList = function () {

        var _dic = { "activeId": activeId, "orderType": 2 };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRuleExsForVoteByActiveId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //过滤投票者
                for (var _num = 0; _num < _data.length; _num++) {

                    var _item = _data[_num];
                    if (_item.ID == ruleExId) {

                        //该酒店的头图（海报要求正方形）
                        _item.PicUrl = _item.PicUrl.replace("_jupiter", "_640x640");

                        //该酒店的海报文案
                        _item.PosterDesc = "我是{0}，我正在参加【{1}】亲子大使打榜，快来pick！助我登顶排行~".format(nickName, _item.Title);

                        activeData.thisVoteResult = _item;
                        break;
                    }
                }

                Vue.nextTick(function () {

                    //获取海报（去拉票）
                    $(".get-poster-btn").each(function () {

                        $(this).click(function () {

                            var _qrcodeHtml = $("#mine-qrcode-info").html();

                            _Modal.show({
                                title: "",
                                content: _qrcodeHtml,
                                textAlign: "center",
                                showClose: true,
                                confirmText: "",
                                confirm: function () {
                                    _Modal.hide();
                                }
                            });

                            $("._modal-section").css("top", "15%");
                        });
                    });

                    ////获取海报（去拉票） haoy 暂时这里不直接弹海报了
                    //$(".get-poster-btn").each(function () {

                    //    $(this).click(function () {

                    //        var _id = $(this).data("id");
                    //        var _bannerurl = $(this).data("bannerurl").replace('_jupiter', '_640x920').replace('_640x640', '_640x920').replace('_640x360', '_640x920').replace('_350X350', '_640x920');
                    //        var _headimgurl = $(this).data("headimgurl").replace('_jupiter', '_350X350');
                    //        var _username = $(this).data("username");
                    //        var _posterdesc = $(this).data("posterdesc");
                    //        var _tipimgurl = $(this).data("tipimgurl");
                    //        var _producturl = $(this).data("producturl");    //"http://192.168.1.188:8081/wx/active/supervoteitem/786/85/0/0";

                    //        showPoster(_id, _bannerurl, _headimgurl, _username, _posterdesc, _tipimgurl, _producturl);
                    //    });
                    //});
                });
            }
        });
    }
    loadVoteResultList();

    //注册报名事件
    $(".reg-btn").click(function () {

        var _phone = $("#reg-phone").val();
        if (!_phone || _phone.length != 11) {

            alert("请填写真实有效的手机号码");
            return;
        }

        _Loading.show();

        setTimeout(function () {

            //更新报名手机号
            var _updatePhoneDic = {
                "activeId": activeId,
                "openid": openid,
                "phone": _phone
            };
            $.get(_Config.APIUrl + "/api/weixinapi/UpdateActiveWeixinDrawPhone", _updatePhoneDic, function (_data) {

                console.log(_data);
                if (_data) {


                }

            });

            //代言信息
            var _spokesman = {
                "ID": 0,
                "ActiveDrawId": activeDrawId,  //当前用户的报名记录
                "RuleExId": ruleExId,          //谁的代言
                "ActiveId": activeId,          //主活动ID
                "CreateTime": "2018-10-23"
            }

            console.log(_spokesman)

            //新增代言记录
            $.post(_Config.APIUrl + "/api/weixinapi/AddActiveRuleSpokesman", _spokesman, function (_data) {

                _Loading.hide();

                console.log(_data);

                //显示报名成功
                $("#reg-section").hide();
                $("#reged-section").fadeIn();

                document.title = "报名成功";

            });

        }, 1000);
    });

    //生成个人主页专属二维码
    var genUserQrcode = function () {

        //qrscene_ACTIVESUPERVOTE_ACTIVEID
        var _sceneStr = "qrscene_ACTIVESUPERVOTE_{0}_{1}".format(activeId, ruleExId);

        var _dic = {
            weixinAcount: 7, //如 周末酒店服务号 浩颐
            expires: 2592000,
            actionName: "QR_STR_SCENE",
            sceneId: 0,
            sceneStr: _sceneStr
        };

        console.log(_dic);

        //$.get(_Config.APIUrl + '/api/WeixinApi/CreateAccountQrcode', _dic, function (_data) {
        $.get('http://api.zmjiudian.com/api/WeixinApi/CreateAccountQrcode', _dic, function (_data) {

            console.log(_data);

            if (_data && _data.indexOf("ticket") >= 0) {

                var _dataObj = JSON.parse(_data);
                console.log(_dataObj);

                var _src = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}".format(_dataObj.ticket);
                console.log(_src);

                $("#userQrcodeImg").attr("src", _src);

            }
        });
    }
    genUserQrcode();

    //加载并显示海报
    var productImgLoaded = false;
    var qrcodeImgLoaded = false;
    var headTipImgLoaded = true;
    var headImgLoaded = true;
    var loadPoster = function () {

        if (productImgLoaded && qrcodeImgLoaded && headTipImgLoaded && headImgLoaded) {

            console.log(123)

            $("#showPosterSection").css("top", $('body').scrollTop());
            $("#showPosterSection").show();
            $("#showPosterSection .poster-result").hide();
            $("#showPosterSection .poster-loadding").show();
            $(".poster-bg").show();

            setTimeout(function () {

                var _posterOriginElm = $(".mine-poster")[0];

                html2canvas(_posterOriginElm, { useCORS: true }).then(function (canvas) {

                    //console.log(canvas.toDataURL());

                    $("#showImg").attr("src", canvas.toDataURL());
                    $("#showImg").unbind("load");
                    $("#showImg").load(function () {

                        setTimeout(function () {

                            isloaded = true;

                            $("#showPosterSection .poster-result").slideDown();
                            $("#showPosterSection .poster-loadding").hide();

                            $("#showPosterSection .poster-result .poster-close").unbind("click");
                            $("#showPosterSection .poster-result .poster-close").click(function () {

                                $("#showPosterSection").fadeOut(200);
                                $(".poster-bg").fadeOut(100);
                                $(".poster-tip-loading").hide();
                            });

                            $(".poster-bg").unbind("click");
                            $(".poster-bg").click(function () {

                                $("#showPosterSection").fadeOut(200);
                                $(".poster-bg").fadeOut(100);
                                $(".poster-tip-loading").hide();
                            });

                        }, 200);
                    });

                });

            }, 100);
        }
    }

    //弹出海报事件
    var isloaded = false;
    var showPoster = function (_id, _bannerurl, _headimgurl, _username, _posterdesc, _tipimgurl, _producturl) {

        if (!isloaded) {

            $("#showPosterSection .poster-loadding .mine-poster").show();
            //$("#showPosterSection .poster-result .share-info .share-desc").html(_posterdesc);
            $("#poster-desc").html(_posterdesc);

            //生成分享二维码
            $("#productQrcode").html("");
            new QRCode(document.getElementById('productQrcode'), _producturl);
            setTimeout(function () {

                var _productQrcodeSrc = $("#productQrcode img").attr("src");
                console.log(_productQrcodeSrc)

                var _loadQrcodeState_load = true;
                var _loadQrcodeState_auto = true;
                $("#goget-qrcode-img").load(function () {
                    _loadQrcodeState_auto = false;
                    if (_loadQrcodeState_load) {
                        console.log("qrcode load")
                        qrcodeImgLoaded = true;
                        loadPoster();
                    }
                });
                $("#goget-qrcode-img").attr("src", _productQrcodeSrc);

                //300毫秒后自动加载海报（有些环境下qrcode的图片load事件不会触发 2018.07.12 haoy）
                setTimeout(function () {
                    _loadQrcodeState_load = false;
                    if (_loadQrcodeState_auto) {
                        console.log("qrcode auto")
                        qrcodeImgLoaded = true;
                        loadPoster();
                    }
                }, 300);

            }, 600);

            //加载banner图和头像
            setTimeout(function () {

                var timestamp = Date.parse(new Date());

                //产品图片（ios微信环境下，如果图片之间加载过有缓存，不会执行load事件，所以这里统一加上时间戳，MMP 20180109 haoy）
                var _bannerImgId = $("#item-banner-img");
                _bannerImgId.attr("src", _bannerurl + "?v=" + timestamp);
                _bannerImgId.unbind("load");
                _bannerImgId.load(function () {

                    console.log("banner好了");
                    productImgLoaded = true;
                    loadPoster()
                });

                //加载头像


            }, 300);
        }
        else {

            $("#showPosterSection").css("top", $('body').scrollTop());
            $("#showPosterSection").show();
            $("#showPosterSection .poster-result").show();
            $("#showPosterSection .poster-loadding").hide();
            $(".poster-bg").show();
        }
    }
});
