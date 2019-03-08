var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var activeId = $("#activeId").val();
var id = $("#id").val();
var openid = $("#openid").val();
var unionid = $("#unionid").val();
var wxuid = $("#wxuid").val();
var activeDrawId = $("#activeDrawId").val();
var sourceDrawid = $("#sourceDrawid").val();
var userDrawId = activeDrawId;
var drawPhone = $("#drawPhone").val();
var nickName = $("#nickName").val();
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

    //【封禁】提醒投票用户账号异常=========
    if (sourceDrawid == 994159
        || sourceDrawid == 995854
        || sourceDrawid == 996833
        || sourceDrawid == 998315
        || sourceDrawid == 1001638
        || sourceDrawid == 1003456
        || sourceDrawid == 1011537
        || sourceDrawid == 1012667
        || sourceDrawid == 1013755
        || sourceDrawid == 1028893
        || sourceDrawid == 1049030
        || sourceDrawid == 1068987
        || sourceDrawid == 1076503

        || sourceDrawid == 1014365
        || sourceDrawid == 1028893  //存在没有微信用户信息的投票
        || sourceDrawid == 1029085  //存在没有微信用户信息的投票
        || sourceDrawid == 1000372  //存在没有微信用户信息的投票
        || sourceDrawid == 995458   //存在没有微信用户信息的投票
        || sourceDrawid == 1060130  //存在没有微信用户信息的投票
        || sourceDrawid == 1147688  //存在没有微信用户信息的投票
        || sourceDrawid == 1052477  //存在没有微信用户信息的投票
        || sourceDrawid == 1100796  //存在没有微信用户信息的投票
        || sourceDrawid == 1251247  //存在没有微信用户信息的投票
        || sourceDrawid == 1050896  //存在没有微信用户信息的投票

        || sourceDrawid == 1081215  //夜里凌晨2、3、4点还在大量刷票
        || sourceDrawid == 1074545  //夜里凌晨2、3、4点还在大量刷票
        || sourceDrawid == 1001240  //夜里凌晨2、3、4点还在大量刷票
        || sourceDrawid == 1063840  //连续3天同样的3个时间段大量刷票
        || sourceDrawid == 1091182  //最后两天夜里凌晨2、3、4点还在大量刷票
        || sourceDrawid == 1060972  //最后两天大量毫秒级刷票
        || sourceDrawid == 1048303  //最后一天夜里凌晨2、3、4点还在大量刷票
        || sourceDrawid == 1127012  //最后一天夜里凌晨2、3、4点还在大量刷票
        || sourceDrawid == 1032800  //最后一天夜里凌晨2、3、4点还在大量刷票
        || sourceDrawid == 1113556  //最后一天夜里凌晨2、3、4点还在大量刷票

        || sourceDrawid == 1091182  //最后两天超过800票
        || sourceDrawid == 1154429  //最后两天超过800票
        || sourceDrawid == 1154799  //最后两天超过800票
        || sourceDrawid == 1258607  //最后两天超过800票
        || sourceDrawid == 1060972  //最后两天超过800票
        || sourceDrawid == 1127012  //最后两天超过800票
        //|| sourceDrawid == 1141132  //最后两天超过800票    阿杰
        || sourceDrawid == 1142466  //最后两天超过800票
        || sourceDrawid == 1142670  //最后两天超过800票
        || sourceDrawid == 1091173  //最后两天超过800票
        || sourceDrawid == 1083580  //最后两天超过800票
        || sourceDrawid == 1048303  //最后两天超过800票
        || sourceDrawid == 1097885  //最后两天超过800票
        || sourceDrawid == 995338   //最后两天超过800票
        || sourceDrawid == 1113556  //最后两天超过800票 (之前有来投诉过，提供了凭证)
        || sourceDrawid == 1192825  //最后两天超过800票
        || sourceDrawid == 1042191  //最后两天超过800票
        || sourceDrawid == 1140966  //最后两天超过800票
        || sourceDrawid == 1050127  //最后两天超过800票
        || sourceDrawid == 1032800  //最后两天超过800票
        || sourceDrawid == 1220980  //最后两天超过800票

        || sourceDrawid == 1031271) {

        alert("你要投的账号异常。如存在违规行为（包括但不限于作弊、刷票），将取消投票资格，并收回奖励。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    //【提醒】提醒账号异常=========
    if (activeDrawId == 994159
        || activeDrawId == 995854
        || activeDrawId == 996833
        || activeDrawId == 998315
        || activeDrawId == 1001638
        || activeDrawId == 1003456
        || activeDrawId == 1011537
        || activeDrawId == 1012667
        || activeDrawId == 1013755
        || activeDrawId == 1028893
        || activeDrawId == 1049030
        || activeDrawId == 1068987
        || activeDrawId == 1076503

        || activeDrawId == 1014365
        || activeDrawId == 1028893
        || activeDrawId == 1029085
        || activeDrawId == 1000372
        || activeDrawId == 995458
        || activeDrawId == 1060130
        || activeDrawId == 1147688
        || activeDrawId == 1052477
        || activeDrawId == 1100796
        || activeDrawId == 1251247
        || activeDrawId == 1050896

        || activeDrawId == 1081215  //夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1074545  //夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1001240  //夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1063840  //连续3天同样的3个时间段大量刷票
        || activeDrawId == 1091182  //最后两天夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1060972  //最后两天大量毫秒级刷票1091173
        || activeDrawId == 1048303  //最后一天夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1127012  //最后一天夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1032800  //最后一天夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1113556  //最后一天夜里凌晨2、3、4点还在大量刷票

        || activeDrawId == 1091182  //最后两天超过800票
        || activeDrawId == 1154429  //最后两天超过800票
        || activeDrawId == 1154799  //最后两天超过800票
        || activeDrawId == 1258607  //最后两天超过800票
        || activeDrawId == 1060972  //最后两天超过800票
        || activeDrawId == 1127012  //最后两天超过800票
        //|| activeDrawId == 1141132  //最后两天超过800票 阿杰
        || activeDrawId == 1142466  //最后两天超过800票
        || activeDrawId == 1142670  //最后两天超过800票
        || activeDrawId == 1091173  //最后两天超过800票
        || activeDrawId == 1083580  //最后两天超过800票
        || activeDrawId == 1048303  //最后两天超过800票
        || activeDrawId == 1097885  //最后两天超过800票
        || activeDrawId == 995338   //最后两天超过800票
        || activeDrawId == 1113556  //最后两天超过800票 (之前有来投诉过，提供了凭证)
        || activeDrawId == 1192825  //最后两天超过800票
        || activeDrawId == 1042191  //最后两天超过800票
        || activeDrawId == 1140966  //最后两天超过800票
        || activeDrawId == 1050127  //最后两天超过800票
        || activeDrawId == 1032800  //最后两天超过800票
        || activeDrawId == 1220980  //最后两天超过800票

        || activeDrawId == 1031271) {

        alert("检测到您的投票数据异常，我们会进一步核实，期间您可以继续给好友投票；如存在违规行为（包括但不限于作弊、刷票），将取消投票资格，并收回奖励。")
        //location.href = "/wx/active/supervote/786/0";
        //return;
    }

    //【封禁】过于频繁===========
    //2018.11.15 9:30
    if (sourceDrawid == 1028893) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    //2018.11.15 13:20
    if (sourceDrawid == 1014692) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    //2018.11.16 18:50
    if (sourceDrawid == 1031690) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    //2018.11.19 10:00
    if (sourceDrawid == 1063112) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    //2018.11.19 10:27
    if (sourceDrawid == 995637) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    //2018.11.20 11:21
    if (sourceDrawid == 1060055) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    ////2018.11.22 11:09 (2018.11.27 中午解封)
    //if (sourceDrawid == 1113556) {

    //    alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
    //    location.href = "/wx/active/supervote/786/0";
    //    return;
    //}

    //2018.11.22 11:38
    if (sourceDrawid == 1003393) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    //2018.11.26 13:12
    if (sourceDrawid == 997988) {

        alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
        location.href = "/wx/active/supervote/786/0";
        return;
    }

    ////2018.11.22 12:01
    //if (sourceDrawid == 1076541) {

    //    alert("系统安全检测，您要投的用户操作过于频繁，暂时无法投票。")
    //    location.href = "/wx/active/supervote/786/0";
    //    return;
    //}

    //【提醒】提醒数据存在异常===========
    if (activeDrawId == 996452
        || activeDrawId == 1113556  //11.22号有大量毫秒级投票，中奖需要核实
        || activeDrawId == 1063112
        || activeDrawId == 1003393
        || activeDrawId == 1031690  //早期刷票者 11.16号左右
        || activeDrawId == 1106564  //早期刷票者 11.17号左右
        || activeDrawId == 1063840  //连续3天同样的3个时间段大量刷票
        || activeDrawId == 999343   //早期11.13号有大量毫秒级刷票，考虑最后取消资格
        || activeDrawId == 1029334  //11.19号有大量毫秒级刷票，考虑最后取消资格
        //|| activeDrawId == 1076541  //每天很规律的大量刷票
        || activeDrawId == 1012961  //早期11.13号有大量刷票，考虑最后取消资格
        || activeDrawId == 1091182  //最后两天夜里凌晨2、3、4点还在大量刷票
        || activeDrawId == 1060972  //最后两天大量毫秒级刷票

    ) {

        alert("安全提醒：检测到部分数据异常，我们会进一步核实，期间不影响您继续参与该活动，如存在违规行为（包括但不限于作弊、刷票），将取消投票资格，并收回奖励。")
        //location.href = "/wx/active/supervote/786/0";
        //return;
    }

    //now time
    var _nowTime = new Date(parseInt(year0), parseInt(month0) - 1, parseInt(day0), parseInt(hour0), parseInt(minute0), parseInt(second0));
    var _nowTimeDate = new Date(parseInt(year0), parseInt(month0) - 1, parseInt(day0), parseInt(0), parseInt(0), parseInt(0));

    //展示用户信息的报名ID
    if (sourceDrawid && sourceDrawid != "0") {
        userDrawId = sourceDrawid;
    }

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
                    "MineVoteCount": 0,     //在该酒店的得票
                    "totalVoteCount": 0,    //总得票
                    "trueVoteCount": 0,     //可用票数
                    "trueLuckCount": 0,     //可抽奖次数
                    "trueGoVoteCount": 0,   //可投票数
                },
                "thisVoteResult": {},
                "thisPrizeInfo": [],
                "showManList": false,
                "manResultList": [],
                "config": {
                    "showRegBtn": true,
                    "showRegedBtn": false,
                    "showVoteBtn": true,
                    "showVotedBtn": false,
                }
            }
        })
    }
    init();

    //当前活动信息
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

    //获取活动参与者（酒店方）投票结果
    var loadVoteResultList = function () {

        var _dic = { "activeId": activeId, "orderType": 2 };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRuleExsForVoteByActiveId", _dic, function (_data) {

            console.log("loadVoteResultList"+_data);
            if (_data) {

                //过滤投票者
                for (var _num = 0; _num < _data.length; _num++) {

                    var _item = _data[_num];
                    if (_item.ID == id) {

                        _item.PicUrl = _item.PicUrl.replace("_jupiter", "_640x360").replace("_appview", "_640x360");

                        //排名
                        _item.Ranking = (_num + 1);

                        //该酒店的海报文案
                        _item.PosterDesc = "我是{0}，我正在参加【{1}】亲子大使打榜，快来pick！助我登顶排行~".format(nickName, _item.Title);

                        activeData.thisVoteResult = _item;
                        break;
                    }
                }
            }

            Vue.nextTick(function () {

                if (inWeixin) {

                    //自定义当前分享文案
                    var _shareTitle = "{0}正在《2018中国最受欢迎亲子酒店》打榜，快来帮我投一票~".format(activeData.thisVoteResult.Title);
                    var _shareDesc = "一起瓜分百万大奖，参与人人有奖";
                    var _shareLink = "http://www.zmjiudian.com/wx/active/supervoteitem/{0}/{1}/0/0".format(activeId, id);
                    var _shareImgUrl = activeData.thisVoteResult.PicUrl.replace("_jupiter", "_350X350").replace("_640x360", "_350X350");
                    loadWechat(_shareTitle, _shareDesc, _shareLink, _shareImgUrl, function () { });
                }

                //获取酒店详情
                var _hoteldic = { "id": activeData.thisVoteResult.HotelId, "checkIn": "", "checkOut": "", "sType": "www", "interestid": 0};
                $.get(_Config.APIUrl + "/api/hotel/GetHotelDetail", _hoteldic, function (_data) {
                    //activeData.thisVoteResult.Description = _data.IntroNew.Description;
                    if (_data.IntroNew.ActionUrl) {
                        Vue.set(activeData.thisVoteResult, 'Description', _data.IntroNew.Description)
                        Vue.set(activeData.thisVoteResult, 'ActionUrl', _data.IntroNew.ActionUrl)
                        console.log("酒店" + activeData.thisVoteResult.ActionUrl);
                        Vue.nextTick(function () {
                            //加载图文详情
                            loadSourcePage()
                        })
                    }
                })
            });

        });
    }
    loadVoteResultList();

    //加载酒店图文详情
    var loadSource = false;
    var loadSourcePage = function () {

        if (loadSource) return;
        if (activeData.thisVoteResult.ActionUrl) {

            loadSource = true;

            //alert("b～");
            console.log("load source");
            
            activeData.thisVoteResult.ActionUrl = activeData.thisVoteResult.ActionUrl.replace("http://www.zmjiudian.com", "");
            $("#hotel-source-body").load(activeData.thisVoteResult.ActionUrl, function (response, status, xhr) {

                if (status === "success") {
                    $(".source-more-btn").show();
                    $(".source-more-btn").click(function () {

                        $(".source-more-btn").hide();
                        $("#hotel-source-body").css("max-height", "100%");
                    });
                }
                else {
                    $(".hotel-source").hide();
                }
            });
        }
        else {
            $(".hotel-source").hide();
        }
    }

    //加载活动参与者（酒店方）的奖品信息
    var loadPrizeInfo = function () {

        //_Loading.show();

        var _dic = { "activeId": activeId, "sourceId": id };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRulePrizeBySourceId", _dic, function (_data) {

            //_Loading.hide();

            console.log(_data);
            if (_data) {

                for (var _num = 0; _num < _data.length; _num++) {
                    var _item = _data[_num];

                    if (!_item.Picture) {
                        _item.Picture = "";
                    }
                    else {
                        _item.Picture = _item.Picture.replace("_jupiter", "_350X350");
                    }

                    //奖品描述
                    if (!_item.Description) {
                        _item.Description = "价值{0}元的{1}".format(_item.Price, _item.Name);
                    }
                }

                activeData.thisPrizeInfo = _data;

                Vue.nextTick(function () {


                });
            }

        });
    }
    loadPrizeInfo();

    //加载活动参与者（酒店方）的所有大使
    var _loadManInfoOk = false;
    var loadManInfo = function () {

        if (!_loadManInfoOk) {

            _Loading.show();

            var _dic = { "activeId": activeId, "exid": id };
            $.get(_Config.APIUrl + "/api/weixinapi/GetActiveSpokesmanInfoByActiveIdAndExId", _dic, function (_data) {

                console.log(_data);
                if (_data) {

                    _loadManInfoOk = true;

                    //遍历所有大使
                    for (var _num = 0; _num < _data.length; _num++) {
                        var _item = _data[_num];

                        _item.RankNo = (_num + 1);
                        _item.RankIcon = "";
                        switch (_item.RankNo) {
                            case 1: { _item.RankIcon = "http://whfront.b0.upaiyun.com/www/img/Active/supervote/icon-rank-man-1.png"; break; }
                            case 2: { _item.RankIcon = "http://whfront.b0.upaiyun.com/www/img/Active/supervote/icon-rank-man-2.png"; break; }
                            case 3: { _item.RankIcon = "http://whfront.b0.upaiyun.com/www/img/Active/supervote/icon-rank-man-3.png"; break; }
                        }

                        //当前大使的最后得票时间
                        _item.LastCreateTime = _item.LastCreateTime.replace("T", " ");

                        //如果包含当前微信用户..
                        if (_item.ActiveDrawId == activeDrawId) {

                            //显示去拉票，隐藏成为大使
                            activeData.config.showRegBtn = false;
                            activeData.config.showRegedBtn = true;
                        }

                        //包含当前显示用户
                        if (_item.ActiveDrawId == userDrawId) {

                            //activeData.thisUserInfo.NickName = _item.NickName;
                            //activeData.thisUserInfo.Headimgurl = _item.Headimgurl;
                            activeData.thisUserInfo.RankNo = _item.RankNo;
                            activeData.thisUserInfo.RankIcon = _item.RankIcon;

                            //在该酒店的得票
                            activeData.thisUserInfo.MineVoteCount = _item.MineVoteCount;
                        }
                    }

                    activeData.manResultList = _data;

                    Vue.nextTick(function () {

                        _Loading.hide();

                        //去个人主页抽红包
                        var goMineLuck = function () {

                            //是否弹出过二维码
                            var _qrcodeAlertStore = Store.Get(qrcodeAlertKey);
                            //如果来自自己，点击去拉票直接生成海报
                            if (isurlfrommine || _qrcodeAlertStore) {

                                gourl("/wx/active/supervoteuser/{0}/{1}".format(activeId, 0));
                            }
                            else {

                                $(".reged-btn")[0].click();
                            }
                        }

                        //去拉票方法
                        var getVoteFunction = function () {

                            //是否弹出过二维码
                            var _qrcodeAlertStore = Store.Get(qrcodeAlertKey);

                            //如果来自自己，点击去拉票直接生成海报
                            if (isurlfrommine || _qrcodeAlertStore) {

                                var _id = $(this).data("id");
                                var _bannerurl = $(this).data("bannerurl").replace('_jupiter', '_640x920').replace('_640x640', '_640x920').replace('_640x360', '_640x920').replace('_350X350', '_640x920');
                                var _headimgurl = $(this).data("headimgurl").replace('_jupiter', '_350X350');
                                var _username = $(this).data("username");
                                var _posterdesc = $(this).data("posterdesc");
                                var _tipimgurl = $(this).data("tipimgurl");
                                var _producturl = $(this).data("producturl");    //"http://192.168.1.188:8081/wx/active/supervoteitem/786/85/0/0";

                                showPoster(_id, _bannerurl, _headimgurl, _username, _posterdesc, _tipimgurl, _producturl);
                            }
                            else {

                                //alert
                                Store.Set(qrcodeAlertKey, "1");

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
                            }
                        }

                        //底部去拉票
                        $(".reged-btn").click(getVoteFunction);

                        //头像点击去拉票
                        $("#mineHeadImg").click(function () {

                            //如果来自自己，点击头像回到个人主页
                            if (isurlfrommine) {

                                gourl("/wx/active/supervoteuser/{0}/0".format(activeId));
                            }
                            else {

                                $(".reged-btn")[0].click();
                            }
                        });

                        if (parseInt(sourceDrawid)) {

                            setTimeout(function () {

                                $("html,body").animate({ scrollTop: $(".vote-item-name").offset().top - 45 }, 200);

                            }, 500);
                        }
                        else {

                            setTimeout(function () {

                                if (activeData.config.showRegedBtn) {

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
                                                goMineLuck();
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
                                            content: "我们为你准备了丰厚奖品！<br />快报名成为{0}的亲子大使！".format(activeData.thisVoteResult.Title),
                                            textAlign: "center",
                                            showClose: true,
                                            showCancel: true,
                                            confirmText: "我要报名",
                                            confirm: function () {
                                                _Modal.hide();

                                                gourl('/wx/active/supervoteuserreg/{0}/{1}/{2}'.format(activeId, id, urlfrommine))
                                            },
                                            cancelText: "查看奖品",
                                            cancel: function () {
                                                _Modal.hide();

                                                $("html,body").animate({ scrollTop: $(".vote-item-prize-info").offset().top - 75 }, 300);
                                            }
                                        });
                                    }
                                }

                            }, 1000);
                        }

                        ////弹幕
                        //var item = {
                        //    img: '', //图片 
                        //    info: '弹幕文字信息', //文字 
                        //    href: '', //链接 
                        //    close: false, //显示关闭按钮 
                        //    speed: 20, //延迟,单位秒,默认6 
                        //    bottom: 300, //距离底部高度,单位px,默认随机 
                        //    color: '#fff', //颜色,默认白色 
                        //    old_ie_color: '#000000', //ie低版兼容色,不能与网页背景相同,默认黑色 
                        //}
                        //$('body').barrager(item);

                    });
                }

            });
        }
    }
    loadManInfo();

    //可用票数计算和今日可抽奖次数计算
    var trueVoteState = function () {

        //查询当前用户抽奖记录
        var _dic = { "activedrawid": userDrawId };
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

        var _dic = { "activeId": activeId, "sourceId": userDrawId };
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

    //查询source user的用户信息
    if (sourceDrawid && sourceDrawid != "0") {
        
        var loadSourceDrawUserInfo = function () {

            var _dic = { "activeId": activeId, "id": sourceDrawid  };
            $.get(_Config.APIUrl + "/api/weixinapi/GetActiveWeixinDrawById", _dic, function (_data) {

                console.log(_data);
                if (_data) {

                    activeData.thisUserInfo.NickName = _data.UserName;
                    activeData.thisUserInfo.Headimgurl = _data.HeadImgUrl;

                    Vue.nextTick(function () {


                    });
                }

            });
        }
        loadSourceDrawUserInfo();
    }

    //生成个人主页专属二维码
    var genUserQrcode = function () {

        //qrscene_ACTIVESUPERVOTE_ACTIVEID
        var _sceneStr = "qrscene_ACTIVESUPERVOTE_{0}_{1}".format(activeId, id);

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

    //生成个人投票专属二维码
    var genMineVoteQrcode = function () {

        //qrscene_ACTIVESUPERVOTE_ACTIVEID
        var _sceneStr = "qrscene_ACTIVESUPERVOTE_{0}_{1}_{2}".format(activeId, id, sourceDrawid);

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

                $("#userGoVoteQrcodeImg").attr("src", _src);

            }
        });
    }
    genMineVoteQrcode();

    //检查当前微信用户的投票信息
    var checkUserVoteInfo = function () {

        var _dic = { "activeId": activeId, "weixinAccount": openid, "today": 1 };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveVoteRecordForType1ByWxAccount", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //计算今日还可投数量
                activeData.thisUserInfo.trueGoVoteCount = _data.length >= maxGoVoteCount ? 0 : maxGoVoteCount - _data.length;

                //根据投票记录判断投票状态
                if (_data.length) {

                    for (var i = 0; i < _data.length; i++) {
                        var _item = _data[i];

                        //如果今天已经给当前item（酒店）投票了，则不显示投票按钮
                        if (_item.SourceId == id) {

                            //并检测今天是否通过当前drawid（包括自己）投过该酒店
                            var _checkDrawDic = { "activeId": activeId, "sourceId": userDrawId, "reltionId": id, "weixinAccount": openid, "today": 1 };
                            $.get(_Config.APIUrl + "/api/weixinapi/GetActiveVoteRecordForType2BySourceIdAndReltionId", _checkDrawDic, function (_checkDrawData) {

                                console.log(_checkDrawData);
                                if (_checkDrawData && _checkDrawData.length) {

                                    activeData.config.showVoteBtn = false;
                                    activeData.config.showVotedBtn = true;
                                }
                                else {

                                    activeData.config.showVoteBtn = true;
                                    activeData.config.showVotedBtn = false;
                                }
                            });
                            
                            break;
                        }
                    }
                }
                else {
                    activeData.config.showVoteBtn = true;
                    activeData.config.showVotedBtn = false;
                }

                Vue.nextTick(function () {

                    
                });
            }
        });
    }
    checkUserVoteInfo();

    //初始事件绑定
    var init = function () {

        //tab切换事件
        $(".tab-item-def").click(function () {

            $(".tab-item-def").addClass("seled");
            $(".tab-item-result").removeClass("seled");
            $(".tab-info-one").show();
            $(".tab-info-two").hide();
        });
        $(".tab-item-result").click(function () {

            $(".tab-item-result").addClass("seled");
            $(".tab-item-def").removeClass("seled");
            $(".tab-info-two").show();
            $(".tab-info-one").hide();

            //加载大使
            _Loading.show();
            setTimeout(function () { _Loading.hide(); }, 1500);
            activeData.showManList = true;

            Vue.nextTick(function () {

                $(".tab-info-two img").lazyload({
                    threshold: 20,
                    placeholder: "/Content/images/default-avatar2.png",
                    effect: "fadeIn"
                });
            });

        });

    }
    init();

    //加载并显示海报
    var productImgLoaded = false;
    var qrcodeImgLoaded = false;
    var headTipImgLoaded = true;
    var headImgLoaded = true;
    var loadPoster = function () {

        if (productImgLoaded && qrcodeImgLoaded && headTipImgLoaded && headImgLoaded) {

            console.log(123)

            $("#showPosterSection").css("top", $("body").scrollTop() ? $("body").scrollTop() : $("html,body").scrollTop());
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

            $("#showPosterSection").css("top", $("body").scrollTop() ? $("body").scrollTop() : $("html,body").scrollTop());
            $("#showPosterSection").show();
            $("#showPosterSection .poster-result").show();
            $("#showPosterSection .poster-loadding").hide();
            $(".poster-bg").show();
        }
    }
});

var _geetest;
var handlerEmbed = function (captchaObj) {

    console.log(888888)

    _geetest = captchaObj;
}

var loadGeetestConfig = function () {

    _Loading.show();

    //行为验证码
    $.ajax({
        // 获取id，challenge，success（是否启用failback）
        url: "/Active/GetCaptcha?t=" + (new Date()).getTime(), // 加随机数防止缓存
        type: "get",
        dataType: "json",
        success: function (data) {

            console.log(data)

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
            }, handlerEmbed)
        }
    })

}
loadGeetestConfig();

var gotogotogotoVoteCheck = function (t) {
    
    var _c = $(t).data("sss");
    var _cs = Store.Get(_c);
    if (_c != _cs) {
        alert("操作太快了，请重试~")
        return;
    }

    if ((activeData && activeData.thisUserInfo && activeData.thisUserInfo.MineVoteCount >= 10) ||
        sourceDrawid == 1001465 ||
        sourceDrawid == 1028893 ||
        sourceDrawid == 999343 ||
        sourceDrawid == 1012961 ||
        sourceDrawid == 1031271 ||
        sourceDrawid == 1014692 ||
        sourceDrawid == 1028417 ||
        sourceDrawid == 1032043 ||
        sourceDrawid == 1002989 ||
        sourceDrawid == 996872 ||
        sourceDrawid == 996833 ||
        sourceDrawid == 998315 ||

        sourceDrawid == 1000181 ||
        sourceDrawid == 1007149 ||
        sourceDrawid == 997924 ||
        sourceDrawid == 1027613 ||
        sourceDrawid == 1030419 ||
        sourceDrawid == 995338 ||
        sourceDrawid == 994152 ||
        sourceDrawid == 1000181 ||
        sourceDrawid == 1036417 ||
        sourceDrawid == 996076 ||
        sourceDrawid == 994685 ||
        sourceDrawid == 997207 ||

        sourceDrawid == 1028430 ||
        sourceDrawid == 995637 ||
        sourceDrawid == 1031690 ||
        sourceDrawid == 998689
    ) {

        _geetest.verify();
        _geetest.onSuccess(function () {
            var result = _geetest.getValidate();
            console.log(result);

            gogogoVote(t)
        });
    }
    else {

        gogogoVote(t);
    }
}

//投票事件
var gogogoVote = function (_t) {

    var _c = $(_t).data("sss");
    var _cs = Store.Get(_c);
    if (_c != _cs) {
        alert("操作太快了，请重试哦~")
        return;
    }

    //大使投票信息
    var _newVote = {
        "activeId": activeId,
        "weixinAccount": openid,
        "sourceId": id,
        "sourceType": 1,
        "reltionId": userDrawId,
        "sourceDrawid": sourceDrawid,
        "checkkey": _cs
    }

    //新增酒店投票记录
    $.post("/Active/GogogoVoteRecord", _newVote, function (_data2) {

        if (_data2) {

            if (_data2.State == 1) {

                console.log("给大使投票成功");
                console.log(_data2);

                //默认给自己投
                var _voteSourceId = activeDrawId;

                //如果是通过海报来的，那么给酒店投票成功后，还需要给来源大使增加一票
                if (parseInt(sourceDrawid)) {
                    _voteSourceId = sourceDrawid;
                }

                //大使投票信息
                _newVote = {
                    "activeId": activeId,
                    "weixinAccount": openid,
                    "sourceId": _voteSourceId,
                    "sourceType": 2,
                    "reltionId": id,
                }

                //新增大使投票记录
                $.post(_Config.APIUrl + "/api/weixinapi/AddActiveVoteRecord", _newVote, function (_data3) {

                    console.log("给大使投票成功");
                    console.log(_data3);

                    if (activeData.config.showRegBtn) {

                        var _alertTxt = "你已为酒店投票成功！你也可以成为酒店亲子大使，瓜分大奖~";
                        if (sourceDrawid && sourceDrawid != "0") {
                            _alertTxt = "已为朋友投票成功！你也可以成为酒店亲子大使，一起瓜分大奖~";
                        }
                        _Modal.show({
                            title: "投票成功",
                            content: _alertTxt,
                            textAlign: "center",
                            confirmText: "成为亲子大使",
                            confirm: function () {
                                _Modal.hide();
                                gourl("/wx/active/supervoteuserreg/{0}/{1}/{2}".format(activeId, id, urlfrommine));
                            },
                            showCancel: true,
                            showClose: true,
                            cancelText: '去活动主场',
                            cancel: function () {
                                _Modal.hide();
                                gourl("/wx/active/supervote/{0}/{1}".format(activeId, urlfrommine));
                            },
                            close: function () {
                                _Modal.hide();
                                location.reload();
                            }
                        });
                    }
                    else {

                        var _alertTxt = "你又离酒店大奖近了一步哦~";
                        if (sourceDrawid && sourceDrawid != "0") {
                            _alertTxt = "你已为朋友投票成功！";
                        }

                        _Modal.show({
                            title: "投票成功",
                            content: _alertTxt,
                            textAlign: "center",
                            confirmText: "好的",
                            confirm: function () {
                                _Modal.hide();
                                location.reload();
                            }
                        });
                    }
                });

            }
            else {

                if (_data2.State == 3) {

                    var _qrcodeHtml = $("#mine-govote-qrcode-info").html();

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
                }
                else {

                    if (_data2.Message) {
                        alert(_data2.Message)
                        location.reload();
                    }
                    else {

                        alert("投票失败")
                    }
                }
            }
        }
        else {

            alert("投票失败")
            return;
        }
    });
}