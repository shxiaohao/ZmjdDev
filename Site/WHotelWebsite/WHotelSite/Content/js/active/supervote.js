var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var openid = $("#openid").val();
var activeId = $("#activeId").val();
var activeDrawId = $("#activeDrawId").val();
var drawPhone = $("#drawPhone").val();
var luckdrawForVoteCount = parseInt($("#luckdrawForVoteCount").val());
var maxGoVoteCount = parseInt($("#maxGoVoteCount").val());
var urlfrommine = parseInt($("#urlfrommine").val());
var isurlfrommine = urlfrommine > 0;
var qrcodeAlertKey = $("#qrcodeAlertKey").val();

var year0 = $("#year0").val();
var month0 = $("#month0").val();
var day0 = $("#day0").val();
var hour0 = $("#hour0").val();
var minute0 = $("#minute0").val();
var second0 = $("#second0").val();

var _Config = new Config();

var activeData;

$(function () {

    _Loading.show();

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
                "thisUserInfo": {
                    "Headimgurl": "",
                    "NickName": "",
                    "RankNo": 0,
                    "RankIcon": "",
                    "totalVoteCount": 0,    //总得票
                    "trueVoteCount": 0,     //可用票数
                    "trueLuckCount": 0,     //可抽奖次数
                    "trueGoVoteCount": 0,   //可投票数
                    "todayVoteList": [],
                    "mineRuleExList": [],
                },
                "voteInfo": {
                    "voteItemList": [],
                    "voteResultList": []
                }
            }
        })
    }
    init();

    //去拉票方法（弹出个人二维码）
    var goGetVoteFunction = function (_ruleExId) {

        //是否弹出过二维码
        var _qrcodeAlertStore = Store.Get(qrcodeAlertKey);

        //如果来自自己，则可以直接跳转至自己主页
        if (isurlfrommine || _qrcodeAlertStore) {

            gourl("/wx/active/supervoteuser/{0}/{1}".format(activeId, _ruleExId));
        }
        else {

            //alert
            Store.Set(qrcodeAlertKey, "1");

            //gen qrcode
            genUserQrcode(_ruleExId);

            _Loading.show();
            setTimeout(function () {

                _Loading.hide();

                var _qrcodeHtml = $("#mine-qrcode-info").html();

                _Modal.show({
                    title: "",
                    content: _qrcodeHtml,
                    textAlign: "center",
                    confirmText: "好的",
                    confirm: function () {
                        _Modal.hide();
                    }
                });

                $("._modal-section").css("top", "15%");

            }, 1500);
        }
        
    }

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

    //【酒店票数列表】获取所有活动参与者投票结果
    var loadVoteResultList = function () {

        var _dic = { "activeId": activeId, "orderType": 2 };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRuleExsForVoteByActiveId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //过滤投票者
                for (var _num = 0; _num < _data.length; _num++) {

                    var _item = _data[_num];

                    _item.RankNo = (_num + 1);
                    _item.RankIcon = "";
                    switch (_item.RankNo) {
                        case 1: { _item.RankIcon = "http://whfront.b0.upaiyun.com/www/img/Active/supervote/icon-rank-item-1.png"; break; }
                        case 2: { _item.RankIcon = "http://whfront.b0.upaiyun.com/www/img/Active/supervote/icon-rank-item-2.png"; break; }
                        case 3: { _item.RankIcon = "http://whfront.b0.upaiyun.com/www/img/Active/supervote/icon-rank-item-3.png"; break; }
                    }

                    //头像图片格式更换
                    _item.PicUrl = _item.PicUrl.replace("_jupiter", "_350X350").replace("_appview", "_350X350").replace('_theme', '_350X350').replace('_small', '_350X350');

                    //编号格式化
                    _item.Number = (_item.HotelId < 10 ? "0" + _item.HotelId : _item.HotelId);

                    //排名
                    _item.Ranking = (_num + 1);

                    //当前item的投票状态
                    _item.TodayVoteState = false;
                    if (activeData.thisUserInfo.todayVoteList && activeData.thisUserInfo.todayVoteList.length) {
                        for (var _vnum = 0; _vnum < activeData.thisUserInfo.todayVoteList.length; _vnum++) {

                            var _voteItem = activeData.thisUserInfo.todayVoteList[_vnum];
                            if (_item.ID == _voteItem.SourceId) {
                                _item.TodayVoteState = true;
                                break;
                            }
                        }
                    }

                    //是否为该item的大使状态
                    _item.IsSpokesman = false;
                    if (activeData.thisUserInfo.mineRuleExList && activeData.thisUserInfo.mineRuleExList.length) {
                        for (var _snum = 0; _snum < activeData.thisUserInfo.mineRuleExList.length; _snum++) {

                            var _ruleexItem = activeData.thisUserInfo.mineRuleExList[_snum];
                            if (_item.ID == _ruleexItem.ID) {
                                _item.IsSpokesman = true;
                                break;
                            }
                        }
                    }
                }

                //所有投票记录
                activeData.voteInfo.voteResultList = _data;

                Vue.nextTick(function () {

                    _Loading.hide();

                    $(".reged-btn").each(function () {

                        var _ruleexid = $(this).data("ruleexid");

                        $(this).click(function () { goGetVoteFunction(_ruleexid) });
                    });

                    $(".rank-list-def img").lazyload({
                        threshold: 20,
                        placeholder: "/Content/images/default-avatar2.png",
                        effect: "fadeIn"
                    });

                });
            }
        });
    }

    //【奖品金额排行】获取所有活动参与的被投票对象
    var _loadVoteItemListOk = false;
    var loadVoteItemList = function () {

        if (!_loadVoteItemListOk) {

            _Loading.show();

            var _dic = { "activeId": activeId };
            $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRuleExsForVoteByActiveId", _dic, function (_data) {

                console.log(_data);
                if (_data) {

                    _loadVoteItemListOk = true;

                    //过滤投票者
                    for (var _num = 0; _num < _data.length; _num++) {

                        var _item = _data[_num];

                        //头像图片格式更换
                        _item.PicUrl = _item.PicUrl.replace("_jupiter", "_350X350").replace("_appview", "_350X350").replace('_theme', '_350X350').replace('_small', '_350X350');

                        //编号格式化
                        _item.Number = (_item.HotelId < 10 ? "0" + _item.HotelId : _item.HotelId);

                        //排名
                        _item.Ranking = (_num + 1);

                        //当前item的投票状态
                        _item.TodayVoteState = false;
                        if (activeData.thisUserInfo.todayVoteList && activeData.thisUserInfo.todayVoteList.length) {
                            for (var _vnum = 0; _vnum < activeData.thisUserInfo.todayVoteList.length; _vnum++) {

                                var _voteItem = activeData.thisUserInfo.todayVoteList[_vnum];
                                if (_item.ID == _voteItem.SourceId) {
                                    _item.TodayVoteState = true;
                                    break;
                                }
                            }
                        }

                        //是否为该item的大使状态
                        _item.IsSpokesman = false;
                        if (activeData.thisUserInfo.mineRuleExList && activeData.thisUserInfo.mineRuleExList.length) {
                            for (var _snum = 0; _snum < activeData.thisUserInfo.mineRuleExList.length; _snum++) {

                                var _ruleexItem = activeData.thisUserInfo.mineRuleExList[_snum];
                                if (_item.ID == _ruleexItem.ID) {
                                    _item.IsSpokesman = true;
                                    break;
                                }
                            }
                        }
                    }

                    //所有投票记录
                    activeData.voteInfo.voteItemList = _data;

                    Vue.nextTick(function () {

                        _Loading.hide();

                        $("html,body").animate({ scrollTop: $(".rank-info").offset().top - 30 }, 300);

                        $(".reged-btn").each(function () {

                            var _ruleexid = $(this).data("ruleexid");

                            $(this).click(function () { goGetVoteFunction(_ruleexid) });
                        });
                    });
                }
            });
        }
    }

    //检查当前微信用户的投票信息
    var checkUserVoteInfo = function () {

        var _dic = { "activeId": activeId, "weixinAccount": openid, "today": 1 };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveVoteRecordForType1ByWxAccount", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //计算今日还可投数量
                activeData.thisUserInfo.trueGoVoteCount = _data.length >= maxGoVoteCount ? 0 : maxGoVoteCount - _data.length;

                //今日投票记录
                activeData.thisUserInfo.todayVoteList = _data;

                //查询当前用户的所有已代言酒店
                var _exDic = { "activeId": activeId, "activedrawid": activeDrawId };
                $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRuleExsForVoteByDrawId", _exDic, function (_exData) {

                    console.log(_exData);
                    if (_exData) {

                        //今日投票记录
                        activeData.thisUserInfo.mineRuleExList = _exData;
                    }

                    //查询实时的酒店排行
                    loadVoteResultList();
                });
            }
        });
    }
    checkUserVoteInfo();

    //可用票数计算和今日可抽奖次数计算
    var trueVoteState = function () {

        //查询当前用户抽奖记录
        var _dic = { "activedrawid": activeDrawId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveLuckRecordAndPrizeByDrawId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //查询今天是否抽过奖（目前一天只能抽一次）
                for (var i = 0; i < _data.length; i++) {

                    var _item = _data[i];

                    _item.RecordTimeDate = _item.RecordTime.split('T')[0];
                    _item.RecordTime = _item.RecordTime.replace(/-/g, "/");

                    //今天已抽奖
                    var _recordTimeDate = new Date(_item.RecordTimeDate);
                    if (parseInt(year0) == _recordTimeDate.getFullYear() && (parseInt(month0) - 1) == _recordTimeDate.getMonth() && parseInt(day0) == _recordTimeDate.getDate()) {

                        activeData.thisUserInfo.todayIsLuckOk = false;
                        activeData.thisUserInfo.todayLuckOver = true;
                        activeData.thisUserInfo.trueLuckCount = 0;
                    }
                }

                //已抽奖次数
                var _luckedCount = _data.length;

                //计算出当前抽奖次数已使用掉的票数
                var _usedVoteCount = _luckedCount * luckdrawForVoteCount;

                //【可用票数】总票数-已用票数=当前可用票数
                var _trueVoteCount = activeData.thisUserInfo.totalVoteCount - _usedVoteCount;
                _trueVoteCount = _trueVoteCount >= 0 ? _trueVoteCount : 0;
                activeData.thisUserInfo.trueVoteCount = _trueVoteCount;

                //【还差几票】通过可用票数，计算今天还差几票
                activeData.thisUserInfo.diffVoteCount = luckdrawForVoteCount - _trueVoteCount;

                //抽奖状态提示
                if (activeData.thisUserInfo.todayIsLuckOk) {

                    //【今日未抽，但票数不够】如果今天没有抽奖，但票数还不够，则显示还差几票
                    if (activeData.thisUserInfo.diffVoteCount > 0) {
                        activeData.thisUserInfo.todayIsLuckOk = false;
                        activeData.thisUserInfo.todayLuckOver = false;
                        activeData.thisUserInfo.trueLuckCount = 0;
                    }
                    else {

                        //【今日可抽奖】
                        activeData.thisUserInfo.todayIsLuckOk = true;
                        activeData.thisUserInfo.todayLuckOver = false;
                        activeData.thisUserInfo.trueLuckCount = 1;
                    }
                }
            }

        });
    }

    //【总得票】获取当前用户的总得票信息
    var loadMineVoteInfo = function () {

        var _dic = { "activeId": activeId, "sourceId": activeDrawId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveVoteRecordForType2BySourceId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                activeData.thisUserInfo.totalVoteCount = _data.length;

                //获取总得票后，结合已抽奖次数计算可用票数
                trueVoteState();
            }

        });
    }
    loadMineVoteInfo();

    var init = function () {

        //酒店列表tab切换事件
        $(".tab-item-def").click(function () {

            $(".tab-item-def").addClass("seled");
            $(".tab-item-result").removeClass("seled");
            $(".rank-list-def").show();
            $(".rank-list-result").hide();
        });
        $(".tab-item-result").click(function () {

            $(".tab-item-result").addClass("seled");
            $(".tab-item-def").removeClass("seled");
            $(".rank-list-result").show();
            $(".rank-list-def").hide();

            //查询奖品金额的酒店排行
            loadVoteItemList();
        });

        //当前用户信息模块：成为亲子大使和去拉票按钮的事件（滑动至下面）
        $(".go-reg-btn").click(function () {
            $("html,body").animate({ scrollTop: $(".rank-info").offset().top }, 300);
        });

        //去拉票
        if ($(".go-getvote-btn")) $(".go-getvote-btn").click(function () { goGetVoteFunction(0) });

        //去抽奖
        if ($(".go-luckdraw-btn")) $(".go-luckdraw-btn").click(function () { goGetVoteFunction(0) });

        //报名成功的头像点击
        if ($("#drawUserHeadImg")) $("#drawUserHeadImg").click(function () { goGetVoteFunction(0) });
    }
    init();

    //生成个人主页专属二维码
    var genUserQrcode = function (ruleExId) {

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

                setTimeout(function () {

                    if (drawPhone && drawPhone.length == 11) {

                        var qrcodeAutoAlertRedKey = qrcodeAlertKey + "_red";

                        //是否弹出引导抽奖
                        var _qrcodeAutoAlertRedStore = Store.Get(qrcodeAutoAlertRedKey);
                        if (!_qrcodeAutoAlertRedStore) {
                            
                            Store.Set(qrcodeAutoAlertRedKey, "1");

                            _Modal.show({
                                title: "红包天天抽",
                                content: "记得每天还能抽奖哦，人人有奖！",
                                textAlign: "center",
                                showClose: true,
                                confirmText: "去抽奖",
                                confirm: function () {
                                    _Modal.hide();
                                    goGetVoteFunction(0);
                                }
                            });
                        }
                    }
                    else {

                        //是否弹出引导报名
                        var qrcodeAutoAlertKey = qrcodeAlertKey + "_auto";
                        var _qrcodeAutoAlertStore = Store.Get(qrcodeAutoAlertKey);
                        if (!_qrcodeAutoAlertStore) {

                            Store.Set(qrcodeAutoAlertKey, "1");

                            _Modal.show({
                                title: "赢酒店大奖",
                                content: "pick你心仪的酒店，成为亲子大使，总统套房、海景别墅等百万壕礼等你来！",
                                textAlign: "center",
                                showClose: true,
                                confirmText: "我要报名",
                                confirm: function () {
                                    _Modal.hide();

                                    $("html,body").animate({ scrollTop: $(".rank-info").offset().top }, 300);
                                }
                            });
                        }
                    }

                }, 1000);

            }
        });
    }
    genUserQrcode(0);
});
