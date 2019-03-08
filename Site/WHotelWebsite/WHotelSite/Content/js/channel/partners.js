var isMobile = $("#isMobile").val() == "1";
var isInWeixin = $("#isInWeixin").val() == "1";
var isApp = $("#isApp").val() == "1";
var isVip = $("#isVip").val() == "1";
var userid = parseInt($("#userId").val());
var cid = parseInt($("#cid").val());
var appShareLink = $("#appShareLink").val();
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

    //加载度假伙伴相关规则
    var partnerRulesData = null;
    var loadPartnerRules = function () {

        var _paramData = { "type": 9, "curuserid": userid };
        $.get(_Config.APIUrl + "/api/hotel/GetHomeOnlineBannersByType", _paramData, function (_data) {

            console.log(_data);

            if (partnerRulesData) {
                partnerRulesData.ListData = _data;
            }
            else {
                partnerRulesData = new Vue({
                    el: '#partner-rules',
                    data: { "ListData": _data }
                })
            }

        });
    }
    loadPartnerRules();

    //申请
    $(".sub-btn").click(function () {

        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
        if (!userid) {

            //app环境下，如果没有登录则弹出登录
            if (isApp) {
                location.href = loginapphref;
                return;
            }
            else {
                _loginModular.show();
            }
        }
        else {
            subFunction(userid);
        }
    });

    //右上角分享提示点击事件
    $(".wx-share-tip").click(function () {
        $(this).hide();
    });

    //邀请朋友
    $(".sub-share").click(function () {

        if (isInWeixin) {
            $(".wx-share-tip").show();
        }
        else if (isApp) {
            gourl(appShareLink);
        }
        else {
            alert("请到微信或周末酒店APP中进行分享哦～")
        }
    });

    //延时加载活动说明
    setTimeout(function () {

        //分享提示图片动态加载
        var shareTipImg = $(".wx-share-tip img");
        setImgOriSrc(shareTipImg);

    }, 500);
});

//原生登录回调
function loginCallback(userid) {

    location.href = location.href.replace("userid=0", "userid={userid}");
    return;

    //验证当前用户是否VIP
    var isVipDic = { uid: userid };
    $.post(_Config.APIUrl + "/api/Accounts/IsVIPCustomer", isVipDic, function (_data) {
        isVip = _data;
        subFunction(userid);
    });
    
}

//提交申请
var subFunction = function (_uid) {

    if (isVip || cid > 0) {

        gourl("/Channel/ApplyPartner?userid={0}&CID={1}&_newpage=0".format(_uid, cid));
    }
    else {

        _Modal.show({
            title: '',
            content: '只有VIP会员才能申请成为度假伙伴哦~',
            confirmText: '现在成为VIP',
            confirm: function () {

                goBuyVip();
                _Modal.hide();
            },
            showClose: true,
            showCancel: false
        });
    }
}

$(".goshop").click(function () {

    var showContent = "";
    if (isApp) {
        showContent += "<div style='font-size:0.9em;'>添加微信公众号“尚旅周末”（微信号：hellozmjiudian），进入“分销伙伴”—“我的店铺”中查看并管理你的店铺</div>";
        showContent += "<br /><center><img style='width:8em;' src='http://whfront.b0.upaiyun.com/app/img/channel/hellozmjiudian-qrcode-img.jpg' alt='微信号：shanglv006'></center>";
        showContent += "<div class='wx-no-span'><b>hellozmjiudian</b></div>";

        _Modal.show({
            title: '店铺管理',
            content: showContent,
            confirmText: '复制微信号',
            confirm: function () {
                _Modal.hide();

                //调用app复制功能
                zmjd.copyTxt("hellozmjiudian");

                alert("已复制");
            },
            showCancel: false,
            showClose: true
        });
    }
    else {
        showContent += "<div style='font-size:0.9em;'>添加微信公众号“尚旅周末”（微信号：hellozmjiudian），进入“分销伙伴”—“我的店铺”中查看并管理你的店铺</div>";
        showContent += "<br /><center><img style='width:8em;' src='http://whfront.b0.upaiyun.com/app/img/channel/hellozmjiudian-qrcode-img.jpg' alt='微信号：shanglv006'></center>";
        showContent += "<div class='wx-no-span'><b>hellozmjiudian</b></div>";
        showContent += "<div class='wx-no-tip'>长按复制微信号</div>";

        _Modal.show({
            title: '店铺管理',
            content: showContent,
            confirmText: '知道了',
            confirm: function () {
                _Modal.hide();
            },
            showCancel: false,
            showClose: true
        });
    }

    $("._modal-section").css("top", "12%");
});

//灰色开拓我的团队
$(".sub-share-0").click(function () {
    
    $("html,body").animate({ scrollTop: $(".partner-sub-section").offset().top - 12 }, 300);
});

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': $("#_name").val(), 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?_isoneoff=1&_newpage=1";
}