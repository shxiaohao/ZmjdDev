var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var shareNativeLink = $("#shareNativeLink").val();
var toget = $("#toget").val() == "1";

var _shareTitle = $("#shareTitle").val();
var _shareDesc = $("#shareDesc").val();
var _shareLink = $("#shareLink").val();
var _shareImgUrl = $("#shareImgUrl").val();
var _isNewShare = $("#isNewShare").val() == "1";

var _albumids = "26,27";

//常规活动gift
var activeGiftData = null;

//未领取的券list
var getCouponList = [];
var getCouponIds = "";

//当前总红包金额
var sumCouponAmount = 0;

//未领取的金额
var getCouponAmount = 0.0;

//已领取的金额
var getedCouponAmount = 0.0;

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

    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();

    var _itemWidth = winWidth / 2;

    //绑定tab事件
    var bulidTabEvent = function () {

        var _num = 0;
        var _tabs = $("#tabs .t-item");
        _tabs.each(function () {

            var _tabObj = $(this);

            //关联专辑ID
            var _relId = _tabObj.data("relid");
            
            //是否默认选中
            var _seled = _tabObj.data("seled");

            //展示版式
            var _listType = _tabObj.data("listtype");

            //设置默认宽度
            _tabObj.css("width", _itemWidth);

            //click
            _tabObj.click(function () {

                selectMenu(_relId);

                loadAlbum(_relId, _listType, true);
            });

            //默认加载第一个
            if (_seled == 1) {
                selectMenu(_relId);
                loadAlbum(_relId, _listType, false);
            }

            _num++;
        })

    }

    //选中当前菜单
    var selectMenu = function (id) {

        $("#tabs .t-item").each(function () {

            var _mitem = $(this);
            var _relid = _mitem.data("relid");

            _mitem.removeClass("sel");
            if (_relid === id) {

                _mitem.addClass("sel");

            }

        });
    }

    var tabListArray = {};

    //加载指定套餐专辑ID的list
    //_listType:0默认酒店版式 1海报套餐版式
    var loadAlbum = function (_albumId, _listType, _scroll) {

        var _tabListItemKey = "tl-" + _albumId;
        var _tabListItemData = tabListArray[_tabListItemKey];
        if (_tabListItemData) {

            $(".tab-detail-list").hide();
            $("#t-list-" + _albumId).show();

            if (_scroll) {
                $("html,body").animate({ scrollTop: $("#tabs").offset().top }, 200);
            }
        }
        else {

            $(".tab-detail-list").hide();
            $("#t-list-" + _albumId).show();

            var _listDic = { albumId: _albumId, count: 50, start: 0, curUserID: userId };
            var _loadUrl = _Config.APIUrl + '/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID';
            
            $.get(_loadUrl, _listDic, function (data) {

                console.log(data);

                if (data) {

                    var _listId = "#t-list-" + _albumId;
                    new Vue({
                        el: _listId,
                        data: {
                            "tabDetail": data
                        }
                    })

                    Vue.nextTick(function () {

                        $("#scrollpageloading-" + _albumId).hide();

                        //记录已经加载
                        tabListArray[_tabListItemKey] = data;

                        if (_scroll) {
                            $("html,body").animate({ scrollTop: $("#tabs").offset().top }, 200);
                        }
                    });
                }

            });
        }
    }

    //初始加载
    var init = function () {

        bulidTabEvent();

        checkActiveCouponState(userId, 0);

        //延时加载活动说明
        setTimeout(function () {

            //分享提示图片动态加载
            var shareTipImg = $(".weixin-share-tip img");
            setImgOriSrc(shareTipImg);

            //右上角分享提示点击事件
            $(".weixin-share-tip").click(function () {
                $(this).hide();
            });

        }, 800);
    }
    init();
});

var gobuy = function (_type, _id) {

    var _url = "";

    //0酒店套餐 1酒店 2消费券产品 3消费券团购
    if (_type == 0) {

        if (1) {
            _url = "/coupon/product/{0}?_newpage=1&_newtitle=1".format(_id);
        }
        else {
            _url = "/coupon/product/{0}".format(_id);
        }
    }

    gourl(_url);
}

//app登录回调
var loginCallback = function (_userid) {
    gourl("/Active/ProductAlbumActive?userid={userid}");
}

//检查当前用户的活动券领取状态
var checkActiveCouponState = function (_userId, _isref) {

    var _getDic = { userid: _userId, albumids: _albumids };
    var _getCouponListApi = _Config.APIUrl + "/api/coupon/GetCouponUserCouponListBySKUAlbumIds";
    //var _getCouponListApi = "http://api.zmjd100.com/api/coupon/GetCouponUserCouponListBySKUAlbumIds";
    $.get(_getCouponListApi, _getDic, function (_data) {

        console.log(_data);

        if (_data) {

            //未领取的券list
            getCouponList = [];
            getCouponIds = "";

            //当前总红包金额
            sumCouponAmount = 0;

            //未领取的金额
            getCouponAmount = 0.0;

            //已领取的金额
            getedCouponAmount = 0.0;

            //遍历所有券，做日期格式&券领取状态的处理
            _data.map(function (item, index) {

                item.StartUseDate = (item.StartUseDate).split("T")[0];
                item.ValidUntilDate = (item.ValidUntilDate).split("T")[0];

                //console.log(_userId)
                if (_userId == 0) {
                    item.UsedCount = 0;
                }

                if (!item.UsedCount) {

                    getCouponAmount += item.DiscountAmount;

                    getCouponList.push(item);

                    if (getCouponIds) getCouponIds += ",";
                    getCouponIds += item.IDX;
                }
                else {
                    getedCouponAmount += item.DiscountAmount;
                }

                sumCouponAmount += item.DiscountAmount;
            });

            //build data
            var _buildData = {
                "list": _data,
                "getCouponList": getCouponList,
                "getCouponIds": getCouponIds,
                "sumCouponAmount": sumCouponAmount,
                "getCouponAmount": getCouponAmount,
                "getedCouponAmount": getedCouponAmount
            }

            //单个大红包的提示金额
            $("#span-coupon-amount").html("¥" + getCouponAmount);

            //vue build
            if (activeGiftData) {
                activeGiftData.AlbumsInfo = _buildData;
            }
            else {
                activeGiftData = new Vue({
                    el: '#gift-coupon-list',
                    data: {
                        "AlbumsInfo": _buildData
                    }
                })
            }

            Vue.nextTick(function () {

                //如果是直接领取红包（app分享回调），则直接去领取并弹出结果
                if (toget) {

                    toget = 0;

                    //领取并弹出
                    goGetCoupon(userId);

                    return;
                }

                //非刷新模式，初始绑定的一系列操作
                if (!_isref) {

                    _shareTitle = "冰点特价住Top亲子酒店，领￥{0}大礼包，还赚免费机票！".format(sumCouponAmount);

                    if (inWeixin) {

                        var _shareSucessFunc = function () {

                            if (userId > 0 && getCouponList && getCouponList.length) {

                                //隐藏分享提示
                                $(".weixin-share-tip").hide();

                                //领取并弹出
                                goGetCoupon(userId);
                                return;
                            }
                        }

                        //微信环境下，动态定义分享文案
                        loadWechat(_shareTitle, _shareDesc, _shareLink, _shareImgUrl, _shareSucessFunc);
                    }

                    //如果存在未领取的券，则默认弹出
                    if (getCouponList && getCouponList.length) {

                        //弹出活动红包
                        _showGiftCouponOne();

                    }
                    else {

                        //隐藏活动红包
                        _hideGiftCouponList();

                        //弹出活动浮窗
                        $(".gift-coupon-ball").fadeIn();
                    }
                }

                //领取欢迎礼相关事件
                $("#gift-coupon-ball").click(function () {

                    if (getCouponList && getCouponList.length) {
                        _showGiftCouponOne();
                    }
                    else {
                        _showGiftCouponList();
                    }

                    //隐藏活动浮窗
                    $(".gift-coupon-ball").hide();
                });
                $(".gift-coupon-one .close").click(function () {
                    _hideGiftCouponList();

                    //弹出活动浮窗
                    $(".gift-coupon-ball").fadeIn();
                });
                $(".gift-coupon-list .close").click(function () {
                    _hideGiftCouponList();

                    //弹出活动浮窗
                    $(".gift-coupon-ball").fadeIn();
                });
                $(".gift-coupon-list-model").click(function () {
                    _hideGiftCouponList();

                    //弹出活动浮窗
                    $(".gift-coupon-ball").fadeIn();
                });

                //去领券功能
                $(".gift-coupon-one .info2 img").click(function () {

                    console.log(getCouponList);
                    console.log(getCouponIds);

                    var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
                    if (userId) {

                        if (isapp) {

                            //app内默认5秒钟后自动领取
                            setTimeout(function () {

                                toget = 0;

                                //领取并弹出
                                goGetCoupon(userId);

                            }, 5000);

                            //app内弹出分享
                            gourl(shareNativeLink);
                        }
                        else {

                            //隐藏活动红包
                            _hideGiftCouponList();

                            //弹出活动浮窗
                            $(".gift-coupon-ball").fadeIn();

                            //提示微信分享
                            $(".weixin-share-tip").show();
                        }
                    }
                    else {

                        //app环境下，如果没有登录则弹出登录
                        if (isapp) {
                            location.href = loginapphref;
                            return;
                        }
                        else {
                            _loginModular.show();
                        }
                    }
                });

                //去使用功能
                $(".geted-gift-coupon").click(function () {

                    //隐藏活动红包
                    _hideGiftCouponList();

                    //弹出活动浮窗
                    $(".gift-coupon-ball").fadeIn();

                    $("html,body").animate({ scrollTop: $("#tabs").offset().top }, 300);
                });

            });
        }
    });
}

var _showGiftCouponOne = function () {
    $(".gift-coupon-one").fadeIn(200);
    $(".gift-coupon-list-model").show();
}
var _showGiftCouponList = function () {
    $(".gift-coupon-list").fadeIn(200);
    $(".gift-coupon-list-model").show();
}
var _hideGiftCouponList = function () {
    $(".gift-coupon-one").hide();
    $(".gift-coupon-list").hide();
    $(".gift-coupon-list-model").hide();
}

//领取券
var goGetCoupon = function (_userid) {

    $.get(_Config.APIUrl + "/api/coupon/ReceiveCouponDefine", { userid: _userid, couponDefineIds: getCouponIds }, function (_getData) {

        console.log(_getData);

        if (_getData) {

            checkActiveCouponState(_userid, 1);

            //隐藏活动浮窗
            $(".gift-coupon-ball").hide();

            _hideGiftCouponList();
            _showGiftCouponList();
        }
        else {
            
            alert("领取失败，请刷新后重试");
        }
    });
}