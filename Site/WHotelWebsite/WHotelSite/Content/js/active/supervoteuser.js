var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var openid = $("#openid").val();
var activeId = $("#activeId").val();
var activeDrawId = $("#activeDrawId").val();
var nickName = $("#nickName").val();
var luckdrawForVoteCount = parseInt($("#luckdrawForVoteCount").val());
var maxGoVoteCount = parseInt($("#maxGoVoteCount").val());
var ruleExId = parseInt($("#ruleExId").val());

var year0 = $("#year0").val();
var month0 = $("#month0").val();
var day0 = $("#day0").val();
var hour0 = $("#hour0").val();
var minute0 = $("#minute0").val();
var second0 = $("#second0").val();

var _Config = new Config();

var activeData;

$(function () {

    //now time
    var _nowTime = new Date(parseInt(year0), parseInt(month0) - 1, parseInt(day0), parseInt(hour0), parseInt(minute0), parseInt(second0));
    var _nowTimeDate = new Date(parseInt(year0), parseInt(month0) - 1, parseInt(day0), parseInt(0), parseInt(0), parseInt(0));

    //init data
    var init = function () {

        //初始订单详情
        activeData = new Vue({
            el: "#active-obj",
            data: {
                "activeInfo": {},
                "mineRuleExList": [],
                "mineVoteList": [],
                "mineTodayVoteList": [],
                "myTodayGoVoteList": [],
                "luckRecordList": [],
                "luckDrawResult": {},
                "voteInfo": {
                    "totalVoteCount": 0,    //总票数
                    "todayVoteCount": 0,    //今日票数
                    "trueVoteCount": 0,     //可用票数
                    "trueLuckCount": 0,     //今日可抽奖次数
                    "trueGoVoteCount": 0,   //今日可投票次数
                    "todayIsLuckOk": false, //今天可否抽奖
                    "diffVoteCount": 0,     //还差？票可以抽奖
                    "todayLuckOver": true,  //今天已抽奖
                    "maxRankNo": 0          //当前最高排名

                }
            }
        })

        //tab切换事件
        $(".tab-item-daiyan").click(function () {

            $(".tab-item-daiyan").addClass("seled");
            $(".tab-item-luckdraw").removeClass("seled");
            $("#daiyan-table").show();
            $("#luckdraw-table").hide();
        });
        $(".tab-item-luckdraw").click(function () {

            $(".tab-item-luckdraw").addClass("seled");
            $(".tab-item-daiyan").removeClass("seled");
            $("#luckdraw-table").show();
            $("#daiyan-table").hide();

            $(".vote-status").hide();
            $(".vote-status-space").show();
        });
    }
    init();

    //获取活动信息
    var loadActiveInfo = function () {

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

    //【今日得票】获取当前用户的今天得票信息
    var loadMineTodayVoteInfo = function () {

        var _dic = { "activeId": activeId, "sourceId": activeDrawId, "today": 1 };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveVoteRecordForType2BySourceId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                activeData.mineTodayVoteList = _data;

                activeData.voteInfo.todayVoteCount = _data.length;

                Vue.nextTick(function () {


                });
            }

        });
    }
    loadMineTodayVoteInfo();

    //可用票数计算和今日可抽奖次数计算
    var trueVoteState = function () {

        //查询当前用户抽奖记录
        var _dic = { "activedrawid": activeDrawId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveLuckRecordAndPrizeByDrawId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //默认今日可抽
                activeData.voteInfo.todayIsLuckOk = true;
                activeData.voteInfo.todayLuckOver = false;

                //查询今天是否抽过奖（目前一天只能抽一次）
                for (var i = 0; i < _data.length; i++) {

                    var _item = _data[i];

                    _item.RecordTimeDate = _item.RecordTime.split('T')[0];
                    _item.RecordTime = _item.RecordTime.replace(/-/g, "/");

                    //今天已抽奖
                    var _recordTimeDate = new Date(_item.RecordTimeDate);
                    if (parseInt(year0) == _recordTimeDate.getFullYear() && (parseInt(month0) - 1) == _recordTimeDate.getMonth() && parseInt(day0) == _recordTimeDate.getDate()) {

                        //【今日抽奖状态】
                        activeData.voteInfo.todayIsLuckOk = false;
                        activeData.voteInfo.todayLuckOver = true;
                        activeData.voteInfo.trueLuckCount = 0;
                    }
                }

                //已抽奖次数
                var _luckedCount = _data.length;

                //计算出当前抽奖次数已使用掉的票数
                var _usedVoteCount = _luckedCount * luckdrawForVoteCount;

                //【可用票数】总票数-已用票数=当前可用票数
                var _trueVoteCount = activeData.voteInfo.totalVoteCount - _usedVoteCount;
                _trueVoteCount = _trueVoteCount >= 0 ? _trueVoteCount : 0;
                activeData.voteInfo.trueVoteCount = _trueVoteCount;

                //【还差几票】通过可用票数，计算今天还差几票
                activeData.voteInfo.diffVoteCount = luckdrawForVoteCount - _trueVoteCount;

                //抽奖状态提示
                if (activeData.voteInfo.todayIsLuckOk) {

                    //【今日未抽，但票数不够】如果今天没有抽奖，但票数还不够，则显示还差几票
                    if (activeData.voteInfo.diffVoteCount > 0) {
                        activeData.voteInfo.todayIsLuckOk = false;
                        activeData.voteInfo.todayLuckOver = false;
                        activeData.voteInfo.trueLuckCount = 0;
                    }
                    else {

                        //【今日可抽奖】
                        activeData.voteInfo.todayIsLuckOk = true;
                        activeData.voteInfo.todayLuckOver = false;
                        activeData.voteInfo.trueLuckCount = 1;
                    }
                }

                //抽奖记录
                activeData.luckRecordList = _data;

                Vue.nextTick(function () {

                    //打开抽奖
                    $("#open-luckdraw-banner").click(function () {

                        //可以抽奖
                        if (activeData.voteInfo.todayIsLuckOk && !activeData.voteInfo.todayLuckOver && activeData.voteInfo.diffVoteCount <= 0) {

                            $(".luckdraw-bg").show();
                            $(".luckdraw-section").show();
                        }
                        else {

                            //今天已经抽奖
                            if (activeData.voteInfo.todayLuckOver && !activeData.voteInfo.todayIsLuckOk) {
                                alert("今日已抽奖，明天再来哦～");
                                return;
                            }

                            //今天未抽奖，但票数还不够
                            if (activeData.voteInfo.diffVoteCount > 0 && !activeData.voteInfo.todayLuckOver && !activeData.voteInfo.todayIsLuckOk) {
                                alert("还差{0}票就能抽奖了哦～".format(activeData.voteInfo.diffVoteCount));
                                return;
                            }
                        }
                    });

                    //抽奖弹窗的关闭事件
                    var closeLuckdrawFunction = function () {

                        $(".luckdraw-bg").hide();
                        $(".luckdraw-section").hide();
                    }
                    $(".luckdraw-section .close").click(closeLuckdrawFunction);

                    //去抽奖
                    var goLuckdrawFunction = function () {

                        //alert(112)
                        //return;
                        var _luckDrawDic = { "activeId": activeId, "activedrawid": activeDrawId, "openid": openid };
                        $.get("/Active/SuperVoteLuckDraw", _luckDrawDic, function (_luckDrawData) {

                            console.log(_luckDrawData)
                            if (_luckDrawData) {

                                activeData.luckDrawResult = _luckDrawData;

                                //抽中无效
                                if (_luckDrawData.State == 0) {

                                    alert(_luckDrawData.Message);
                                }
                                else {

                                    //有效抽奖~（但可能是0元，代表未抽中）
                                    if (parseFloat(_luckDrawData.Price) > 0) {

                                        $(".fail-prize").hide();
                                        $(".open-section").hide();
                                        $(".opened-section").fadeIn();
                                    }
                                    else {

                                        //未抽中
                                        $(".fail-prize").fadeIn();
                                        $(".open-section").hide();
                                        $(".opened-section").hide();
                                    }
                                }

                                //抽奖以后，关闭事件要刷新页面
                                $(".luckdraw-section .close").click(function () {

                                    closeLuckdrawFunction();
                                    location.reload();
                                });

                            }
                            else {
                                alert("抽奖失败，请重试");
                            }
                        })
                    }
                    $(".go-luckdraw-btn").click(goLuckdrawFunction);

                    //抽中奖后的“去看看”
                    $(".look-luck-btn").click(function () {
                        gourl("/wx/active/supervoteluckrecord/{0}".format(activeId));
                    });
                });
            }

        });
    }

    //【总得票】获取当前用户的总得票信息
    var loadMineVoteInfo = function () {

        var _dic = { "activeId": activeId, "sourceId": activeDrawId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveVoteRecordForType2BySourceId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                activeData.mineVoteList = _data;

                activeData.voteInfo.totalVoteCount = _data.length;

                //获取总得票后，结合已抽奖次数计算可用票数
                trueVoteState();

                Vue.nextTick(function () {


                });
            }

        });
    }
    loadMineVoteInfo();

    //获取当前用户所代言的所有酒店
    var _posterItemsDic = [];
    var loadSpokeItems = function () {

        var _dic = { "activeId": activeId, "activedrawid": activeDrawId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRuleExsForVoteByDrawId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                for (var i = 0; i < _data.length; i++) {

                    var _item = _data[i];

                    _item.PicUrl = _item.PicUrl.replace("_jupiter", "_350X350");

                    //该酒店的海报二维码url
                    _item.ProductUrl = "http://www.zmjiudian.com/wx/active/supervoteitem/{0}/{1}/{2}/0".format(activeId, _item.ID, activeDrawId);;

                    //该酒店的海报文案
                    _item.PosterDesc = "我是{0}，我正在参加【{1}】亲子大使打榜，快来pick！助我登顶排行~".format(nickName, _item.Title);

                    //该酒店该用户的今日投票状态
                    _item.TodayVoteState = false;
                    for (var j = 0; j < activeData.myTodayGoVoteList.length; j++) {

                        var _todayVote = activeData.myTodayGoVoteList[j];
                        if (_item.ID == _todayVote.SourceId) {
                            _item.TodayVoteState = true
                            break;
                        }
                    }

                    //记录最高排名
                    if (i == 0) {
                        if (_item.VoteCount) {
                            activeData.voteInfo.maxRankNo = _item.RankNo;
                        }
                        else {
                            activeData.voteInfo.maxRankNo = "无";
                        }
                    }

                    //缓存海报图片地址
                    _posterItemsDic[_item.ID] = "";
                }

                activeData.mineRuleExList = _data;

                Vue.nextTick(function () {

                    //去拉票
                    var _initLoadPosterItem = null;
                    $(".reged-btn").each(function () {

                        var _id = $(this).data("id");
                        var _bannerurl = $(this).data("bannerurl").replace('_jupiter', '_640x920').replace('_640x640', '_640x920').replace('_640x360', '_640x920').replace('_350X350', '_640x920');
                        var _headimgurl = $(this).data("headimgurl").replace('_jupiter', '_350X350');
                        var _username = $(this).data("username");
                        var _posterdesc = $(this).data("posterdesc");
                        var _tipimgurl = $(this).data("tipimgurl");
                        var _producturl = $(this).data("producturl");    //"http://192.168.1.188:8081/wx/active/supervoteitem/786/85/0/0";

                        $(this).click(function () {
                            
                            showPoster(_id, _bannerurl, _headimgurl, _username, _posterdesc, _tipimgurl, _producturl);
                        });

                        //是否指定
                        console.log(ruleExId)
                        if (ruleExId == _id) {
                            _initLoadPosterItem = $(this);
                        }
                    });

                    //判断是否指定了直接生成某家酒店的海报
                    if (_initLoadPosterItem) {

                        setTimeout(function () {

                            $("html,body").animate({ scrollTop: _initLoadPosterItem.offset().top - 130 }, 300);

                            _Loading.show();
                            setTimeout(function () {

                                _Loading.hide();

                                console.log("指定了")
                                console.log(_initLoadPosterItem)
                                _initLoadPosterItem.click();

                            }, 500);

                        }, 500);

                        
                    }
                });
            }

        });
    }

    //【今日投票】获取当前用户的今天投票记录
    var loadMyTodayGoVoteInfo = function () {

        var _dic = { "activeId": activeId, "weixinAccount": openid, "today": 1 };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveVoteRecordForType1ByWxAccount", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //计算今日还可投数量
                activeData.voteInfo.trueGoVoteCount = _data.length >= maxGoVoteCount ? 0 : maxGoVoteCount - _data.length;

                //今日投票数据
                activeData.myTodayGoVoteList = _data;

                //查询今天投票数据后，再加载代言信息（需要判断代言酒店的投票情况）
                loadSpokeItems();

                Vue.nextTick(function () {


                });
            }

        });
    }
    loadMyTodayGoVoteInfo();

    //加载并显示海报
    var productImgLoaded = false;
    var qrcodeImgLoaded = false;
    var headTipImgLoaded = true;
    var headImgLoaded = true;
    var loadPoster = function (_id) {

        if (productImgLoaded && qrcodeImgLoaded && headTipImgLoaded && headImgLoaded) {

            $("#showPosterSection").css("top", $("body").scrollTop() ? $("body").scrollTop() : $("html,body").scrollTop());
            $("#showPosterSection").show();
            $("#showPosterSection .poster-result").hide();
            $("#showPosterSection .poster-loadding").show();
            $(".poster-bg").show();

            setTimeout(function () {

                var _posterOriginElm = $(".mine-poster")[0];

                html2canvas(_posterOriginElm, { useCORS: true }).then(function (canvas) {

                    //console.log(canvas.toDataURL());

                    var _imgsrcdata = canvas.toDataURL()
                    _posterItemsDic[_id] = _imgsrcdata;

                    $("#showImg").attr("src", _imgsrcdata);
                    $("#showImg").unbind("load");
                    $("#showImg").load(function () {

                        setTimeout(function () {

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
    var showPoster = function (_id, _bannerurl, _headimgurl, _username, _posterdesc, _tipimgurl, _producturl) {

        //如果已经生成，直接弹出
        if (_posterItemsDic[_id]) {

            console.log(_posterItemsDic)

            $("#showPosterSection").css("top", $("body").scrollTop() ? $("body").scrollTop() : $("html,body").scrollTop());
            $("#showPosterSection").show();
            $("#showPosterSection .poster-result").hide();
            $("#showPosterSection .poster-loadding").show();
            $(".poster-bg").show();

            $("#showImg").attr("src", _posterItemsDic[_id]);

            setTimeout(function () {

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

            }, 500);

            return;
        }

        $("#showPosterSection .poster-loadding .mine-poster").show();
        //$("#showPosterSection .poster-result .share-info .share-desc").html(_posterdesc);
        $("#poster-desc").html(_posterdesc);

        //生成分享二维码
        $("#productQrcode").html("");
        new QRCode(document.getElementById('productQrcode'), {
            text: _producturl,//二维码内容
            correctLevel: QRCode.CorrectLevel.L//容错级别
        });
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
                    loadPoster(_id);
                }
            });
            $("#goget-qrcode-img").attr("src", _productQrcodeSrc);

            //300毫秒后自动加载海报（有些环境下qrcode的图片load事件不会触发 2018.07.12 haoy）
            setTimeout(function () {
                _loadQrcodeState_load = false;
                if (_loadQrcodeState_auto) {
                    console.log("qrcode auto")
                    qrcodeImgLoaded = true;
                    loadPoster(_id);
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
                loadPoster(_id)
            });

            //加载头像


        }, 300);
    }
});
