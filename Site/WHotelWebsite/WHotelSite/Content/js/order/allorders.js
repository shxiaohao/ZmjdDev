var _Config = new Config();
var userid = $("#userid").val();
var isApp = $("#isApp").val() == "1";
var selectedtype = $("#selectedtype").val();

var year0 = $("#year0").val();
var month0 = $("#month0").val();
var day0 = $("#day0").val();
var hour0 = $("#hour0").val();
var minute0 = $("#minute0").val();
var second0 = $("#second0").val();

var loadItem = {};

$(function () {

    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();
    var tabsHeight = $("#tabs").height();
    var tabsActiveTop = 0;

    //now time
    var _nowTime = new Date(parseInt(year0), parseInt(month0) - 1, parseInt(day0), parseInt(hour0), parseInt(minute0), parseInt(second0));

    //头tabs
    var tabDetailList = [];
    var initTabs = function () {

        var _tabDetailItem = { id: -1, type: "order", name: "全部", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        _tabDetailItem = { id: 28, type: "order", name: "遛娃卡", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        _tabDetailItem = { id: 0, type: "order", name: "酒店", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        _tabDetailItem = { id: 1, type: "order", name: "机酒", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        _tabDetailItem = { id: 2, type: "order", name: "邮轮", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        _tabDetailItem = { id: 15, type: "order", name: "房券", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        _tabDetailItem = { id: 20, type: "order", name: "玩乐", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        _tabDetailItem = { id: 14, type: "order", name: "美食", start: 0, count: 15, stop: false, orderList: [], AdData: {} }
        tabDetailList.push(_tabDetailItem);

        //初始绑定tabs
        var productTabData = new Vue({
            el: "#tabs",
            data: {
                "tabDetailList": tabDetailList
            }
        })

        ////如果只有一个tab，则隐藏..
        //if (tabDetailList.length < 2) {
        //    $("#tabs").hide();
        //    tabsHeight = 0;
        //}

        //默认第一个
        loadItem = tabDetailList[0];

        //找出指定选中的菜单类型
        for (var _tnum = 0; _tnum < tabDetailList.length; _tnum++) {

            var _thisTab = tabDetailList[_tnum];
            if (_thisTab.id == selectedtype) {
                loadItem = _thisTab;
                break;
            }
        }
    }
    initTabs();

    //初始绑定list
    var orderListData = new Vue({
        el: "#more-vue-obj",
        data: {
            "tabDetailList": tabDetailList
        }
    })

    //存储各个tab的下拉对象
    var miniRefreshArr = [];
    Vue.nextTick(function () {

        //下拉刷新事件处理
        $(".scroll-div").each(function () {

            var _scrollObj = $(this);
            //_scrollObj.css("height", winHeight - tabsHeight);

            var _relid = _scrollObj.data("relid");
            var _reltype = _scrollObj.data("reltype");
            
            //旧的上拉加载处理
            //_scrollObj.scroll(function () {

            //    var _divTop = _scrollObj[0].scrollTop;
            //    var _divHeight = _scrollObj.height() + 10;  //多减去10，很奇怪，android上计算有问题... haoy 20170808
            //    var _divScrollHeight = _scrollObj[0].scrollHeight;

            //    if (_divTop >= (_divScrollHeight - _divHeight)) {

            //        //console.log("div到底了")
            //        loadMoreOrderList(_relid, _reltype, false, false);
            //    }
            //});

            //H5下拉实现。引入任何一个主题后，都会有一个MiniRefresh全局变量
            miniRefreshArr[_relid] = new MiniRefresh({
                container: '#minirefresh_' + _relid,
                down: {
                    bounceTime: 400,
                    dampRateBegin: 0.6,
                    callback: function () {

                        /*下拉事件*/

                        //setTimeout(function () { location.reload(); }, 400);
                        setTimeout(function () {
                            loadItem.stop = false;
                            loadItem.start = 0;
                            loadItem.orderList = [];
                            selectMenu(loadItem.id, true);
                            loadMoreOrderList(loadItem.id, loadItem.type, false, true);

                            ////停止下拉刷新
                            //miniRefreshArr[_relid].endDownLoading(true);

                        }, 200);
                    }
                },
                up: {
                    isAuto: true,
                    callback: function () {

                        /*上拉事件*/

                        //alert(101)
                        //console.log(101);

                        //console.log("div到底了")
                        loadMoreOrderList(_relid, _reltype, false, false);

                        //注意，由于默认情况是开启满屏自动加载的，所以请求失败时，请务必endUpLoading(true)，防止无限请求
                        miniRefreshArr[_relid].endUpLoading(false);
                    }
                }
            });
        });

    });

    //var start = 0;
    //var count = 6;

    //加载list
    var loadMoreOrderList = function (id, type, isfirst, goScrollTop) {

        if (isfirst) {
            isload = true;
        }

        if (isload) {

            isload = false;

            //获取start，count等
            var _tabDetailObj = {};
            for (var _tnum = 0; _tnum < orderListData.tabDetailList.length; _tnum++) {
                var _tabDetailItem = orderListData.tabDetailList[_tnum];
                if (_tabDetailItem.id === id) {
                    _tabDetailObj = _tabDetailItem;
                    break;
                }
            }

            //如果是第一页，则初始start
            if (isfirst) {
                _tabDetailObj.stop = false;
                _tabDetailObj.start = 0;
                _tabDetailObj.orderList = [];
            }

            //默认显示loadding
            $("#scrollpageloading-" + id).show();

            //第一次绑定完后，做一些初始操作
            if (isfirst) {
                $(".scrollpageloading").html('<img class="img-first" src="http://whfront.b0.upaiyun.com/app/img/loading-changes.gif" alt="" />');
            }
            else {
                $(".scrollpageloading").find("img").removeClass("img-first");
            }

            //加载指定类型的订单list
            var loadOrderListCallback = function (data) {

                console.log(data);

                isload = true;

                if (data) {

                    if (data.length) {

                        //隐藏空数据占位
                        $("#null-list_" + id).hide();

                        //第一次绑定完后，做一些初始操作
                        if (data.length >= _tabDetailObj.count) {
                            if (isfirst) {
                                $("#scrollpageloading-" + id).show();
                            }
                            else {
                                $("#scrollpageloading-" + id).hide();
                            }
                        }
                        else {
                            //如果是第一页，则不显示“没有更多了”，不然很奇怪
                            if (!isfirst) {
                                $("#scrollpageloading-" + id).hide();
                                //$(".scrollpageloading").html("<div>没有更多了</div>");
                            }
                            else {
                                $("#scrollpageloading-" + id).hide();
                            }
                        }

                        for (var _lnum = 0; _lnum < data.length; _lnum++) {
                            var _litem = data[_lnum];

                            _litem.UserID = userid;

                            //订单日期格式化
                            _litem.SubmitOrderDate = _litem.SubmitOrderDate.split('T')[0];

                            //_litem.SubmitOrderDate = "";

                            //有效期 开始日期
                            var dtArr = (_litem.StartDate).split("-");
                            var dayArr = dtArr[2].split("T");
                            var timeArr = dayArr[1].split(":");
                            var _y2 = dtArr[0];
                            var _mo2 = parseInt(dtArr[1]) - 1;
                            var _d2 = dayArr[0];
                            var _h2 = timeArr[0];
                            var _mi2 = timeArr[1];
                            var _s2 = timeArr[2];
                            var _startTime = new Date(parseInt(_y2), parseInt(_mo2), parseInt(_d2), parseInt(_h2), parseInt(_mi2), parseInt(_s2));

                            //有效期 开始日期格式化
                            _litem.StartDate = _litem.StartDate.split('T')[0];

                            //有效期 结束日期
                            dtArr = (_litem.EndDate).split("-");
                            dayArr = dtArr[2].split("T");
                            timeArr = dayArr[1].split(":");
                            var _y3 = dtArr[0];
                            var _mo3 = parseInt(dtArr[1]) - 1;
                            var _d3 = dayArr[0];
                            var _h3 = timeArr[0];
                            var _mi3 = timeArr[1];
                            var _s3 = timeArr[2];
                            var _endTime = new Date(parseInt(_y3), parseInt(_mo3), parseInt(_d3), parseInt(_h3), parseInt(_mi3), parseInt(_s3));
                            
                            //有效期格式化
                            _litem.EndDate = _litem.EndDate.split('T')[0];

                            //控制订单是否显示其他信息（包含数量或者有效期）
                            _litem.ShowOthers = true;

                            //订单的其他信息（包含数量或者有效期）
                            _litem.Others = "数量：{0}".format(_litem.Count);

                            //是否显示支付按钮
                            _litem.ShowGoPay = false;

                            //订单状态高亮状态css
                            _litem.StateAddCss = "";

                            //显示子订单list
                            _litem.ShowSubList = false;

                            //需要支付尾款
                            _litem.ShowFinalPay = false;
                            _litem.ShowFinalTxt = "支付尾款";
                            _litem.FinalNeedCouponOrderId = "";
                            _litem.FinalPayExchange = null;
                            
                            //是否显示房券兑换按钮
                            _litem.ShowExchange = false;

                            //是否显示预约按钮
                            _litem.ShowReserve = false;

                            //已预约信息
                            _litem.FirstReserveInfo = null;

                            //预约配置
                            _litem.ReserveExId = 0;

                            //当前item的详情url
                            _litem.url = "";

                            //显示子订单记录
                            if (_litem.Count > 1 && _litem.DetailOrderList) {
                                
                                //_litem.ShowDetailList = true;
                                console.log(_litem.Count)

                                //【子订单数据格式化相关处理】（字段DetailOrderList）
                                for (var _subnum = 0; _subnum < _litem.DetailOrderList.length; _subnum++) {

                                    var _subOrder = _litem.DetailOrderList[_subnum];
                                    _subOrder.UserID = userid;

                                    //订单状态高亮状态css
                                    _subOrder.StateAddCss = "";

                                    //是否显示房券兑换按钮
                                    _subOrder.ShowExchange = false;

                                    //是否显示预约按钮
                                    _subOrder.ShowReserve = false;

                                    //已预约信息
                                    _subOrder.FirstReserveInfo = null;

                                    //预约配置
                                    _subOrder.ReserveExId = 0;

                                    //有效期 开始日期
                                    dtArr = (_subOrder.StartDate).split("-");
                                    dayArr = dtArr[2].split("T");
                                    timeArr = dayArr[1].split(":");
                                    _y2 = dtArr[0];
                                    _mo2 = parseInt(dtArr[1]) - 1;
                                    _d2 = dayArr[0];
                                    _h2 = timeArr[0];
                                    _mi2 = timeArr[1];
                                    _s2 = timeArr[2];
                                    var _subStartTime = new Date(parseInt(_y2), parseInt(_mo2), parseInt(_d2), parseInt(_h2), parseInt(_mi2), parseInt(_s2));

                                    //有效期 开始日期格式化
                                    _subOrder.StartDate = _subOrder.StartDate.split('T')[0];

                                    //有效期 结束日期
                                    dtArr = (_subOrder.EndDate).split("-");
                                    dayArr = dtArr[2].split("T");
                                    timeArr = dayArr[1].split(":");
                                    _y3 = dtArr[0];
                                    _mo3 = parseInt(dtArr[1]) - 1;
                                    _d3 = dayArr[0];
                                    _h3 = timeArr[0];
                                    _mi3 = timeArr[1];
                                    _s3 = timeArr[2];
                                    var _subEndTime = new Date(parseInt(_y3), parseInt(_mo3), parseInt(_d3), parseInt(_h3), parseInt(_mi3), parseInt(_s3));

                                    //有效期格式化
                                    _subOrder.EndDate = _subOrder.EndDate.split('T')[0];

                                    //需要预约
                                    if (_subOrder.IsBook) {

                                        //预约配置
                                        _subOrder.ReserveExId = _subOrder.OrderId;

                                        //已预约信息
                                        if (_subOrder.BookUserDateList && _subOrder.BookUserDateList.length) {
                                            _subOrder.FirstReserveInfo = _subOrder.BookUserDateList[0];

                                            //时间格式化
                                            _subOrder.FirstReserveInfo.BookDay = _subOrder.FirstReserveInfo.BookDay.split('T')[0];
                                        }
                                    }

                                    //不同环境下订单详情跳转url
                                    if (isApp) {
                                        _subOrder.url = "/Order/CouponOrderDetail?oid=" + _subOrder.OrderId + "&userid={userid}&_newpage=1&_dorpdown=1";
                                    }
                                    else {
                                        _subOrder.url = "/Order/CouponOrderDetail?oid=" + _subOrder.OrderId + "&userid=" + userid + "&_newpage=1&_dorpdown=1";
                                    }

                                    //是否显示有效期
                                    _subOrder.ShowEndDate = true;

                                    //子订单的右上角的图标（如 定金等）(SubSkuType==1代表为定金订单)
                                    if (_subOrder.SubSkuType == 1) {

                                        _subOrder.TopRightTag = "http://whfront.b0.upaiyun.com/app/img/order/dingjin-label-icon.png";

                                        //“定金”子订单状态显示为已支付
                                        _subOrder.OrderStateName = "已支付";

                                        //“定金”不显示有效期
                                        _subOrder.ShowEndDate = false;

                                        //如果是定金产品的子产品，则不需要跳转
                                        _subOrder.url = "";
                                    }
                                    else {

                                        //不是定金但又是大团购订单，则肯定是尾款exchange对象
                                        if (_litem.StepGroup) {
                                            _litem.FinalPayExchange = _subOrder;
                                        }

                                        //已支付状态的子订单
                                        if (_subOrder.OrderState == 2) {

                                            //房券订单显示兑换（房券200 专辑房券500）
                                            if (_subOrder.ActivityType == 200 || _subOrder.ActivityType == 500) {

                                                //非 后台兑换、商户兑换 才会显示兑换功能  兑换日期必须在券码有效期内
                                                if (_subOrder.ExchangeMethod != 2 && _subOrder.ExchangeMethod != 4 && _subOrder.ExchangeMethod != 5 && _subEndTime >= _nowTime && _subStartTime <= _nowTime) {
                                                    _subOrder.ShowExchange = true;
                                                }
                                            }

                                            //显示预约（消费券）
                                            if (_subOrder.ReserveExId) {

                                                if (_subOrder.FirstReserveInfo) {

                                                    //已预约
                                                    _subOrder.OrderStateName = "已预约";
                                                    _subOrder.StateAddCss = "state-1";
                                                }
                                                else {

                                                    //需要预约
                                                    _subOrder.ShowReserve = true;
                                                }
                                            }
                                        }
                                    }

                                    //壳产品相关处理
                                    if (_litem.IsPackage) {

                                        //壳产品DetailOrderList里的壳产品本身，不显示有效期
                                        if (_subOrder.IsPackage) {
                                            _subOrder.ShowEndDate = false;
                                        }
                                    }
                                }

                            }

                            /* 酒店订单和券订单各自判断 */

                            /* 酒店订单 */
                            if (_litem.OrderType == 0 || _litem.OrderType == 1 || _litem.OrderType == 2) {

                                //暂时酒店订单不显示数量
                                _litem.ShowOthers = false;

                                switch (_litem.OrderState) {

                                    //已提交未支付
                                    case 1: {

                                        //状态显示为 待支付
                                        _litem.OrderStateName = "待支付";

                                        //高亮状态
                                        _litem.StateAddCss = "state-1";

                                        //显示支付
                                        _litem.ShowGoPay = true;

                                        //去支付 按钮的专属class
                                        _litem.GoPayClass = "HotelOrderGoPay";
                                        break;
                                    }
                                    //已取消
                                    case 5: {
                                        break;
                                    }
                                    //已支付待确认
                                    case 10: {
                                        break;
                                    }
                                    //已确认
                                    case 10: {
                                        break;
                                    }
                                    //已完成
                                    case 2: {
                                        break;
                                    }
                                }

                                //如果有补汇款链接，则显示补汇款
                                if (_litem.OrderAddPayURL) {

                                    //状态显示为 待补汇款
                                    _litem.OrderStateName = "待补汇款";

                                    //高亮状态
                                    _litem.StateAddCss = "state-1";

                                    //隐藏支付
                                    _litem.ShowGoPay = false;

                                    //显示补汇款
                                    _litem.ShowRepairPay = true;

                                    //补汇款 按钮的专属class
                                    _litem.RepairPayClass = "HotelOrderRepairPay";
                                }

                                //不同环境下订单详情跳转url
                                if (isApp) {
                                    _litem.url = "whotelapp://www.zmjiudian.com/personal/order/" + _litem.OrderId;
                                }
                                else {
                                    _litem.url = "/personal/order/" + _litem.OrderId
                                }
                            }
                            else {

                                /* 券订单 */

                                //需要预约
                                if (_litem.IsBook) {

                                    //预约配置
                                    _litem.ReserveExId = _litem.OrderId;

                                    //已预约信息
                                    if (_litem.BookUserDateList && _litem.BookUserDateList.length) {
                                        _litem.FirstReserveInfo = _litem.BookUserDateList[0];

                                        //时间格式化
                                        _litem.FirstReserveInfo.BookDay = _litem.FirstReserveInfo.BookDay.split('T')[0];
                                    }
                                }

                                //券订单状态判断
                                switch (_litem.OrderState) {

                                        //已提交待支付
                                    case 1: {

                                        //状态显示为 待支付
                                        _litem.OrderStateName = "待支付";

                                        //高亮状态
                                        _litem.StateAddCss = "state-1";

                                        //显示支付
                                        _litem.ShowGoPay = true;

                                        //去支付 按钮的专属class
                                        _litem.GoPayClass = "CouponOrderGoPay";

                                        ////显示子订单记录
                                        //if (_litem.Count > 1) {
                                        //    _litem.ShowDetailList = true;
                                        //}

                                        break;
                                    }
                                        //已支付
                                    case 2: {

                                        //显示子订单记录
                                        if (_litem.Count > 1) {
                                            _litem.ShowDetailList = true;
                                            _litem.OrderStateName = "";
                                        }

                                        break;
                                    }
                                        //已出票/已使用
                                    case 3: {

                                        //显示子订单记录
                                        if (_litem.Count > 1) {
                                            _litem.ShowDetailList = true;
                                            _litem.OrderStateName = "";
                                        }
                                        else {

                                            //没有多个子订单时，显示订单状态
                                            _litem.OrderStateName = "已使用";

                                            //玩乐券显示为 已出票
                                            if (_litem.CategoryParentId == 20) {
                                                _litem.OrderStateName = "已出票";
                                            }
                                        }

                                        break;
                                    }
                                        //已取消
                                    case 4:
                                    case 50: {

                                        break;
                                    }
                                        //已退款
                                    case 5: {

                                        //显示子订单记录
                                        if (_litem.Count > 1) {
                                            _litem.ShowDetailList = true;
                                            _litem.OrderStateName = "";
                                        }

                                        break;
                                    }
                                        //待退款
                                    case 7: {

                                        //显示子订单记录
                                        if (_litem.Count > 1) {
                                            _litem.ShowDetailList = true;
                                            _litem.OrderStateName = "";
                                        }

                                        break;
                                    }
                                        //已过期
                                    case 8: {

                                        //显示子订单记录
                                        if (_litem.Count > 1) {
                                            _litem.ShowDetailList = true;
                                            _litem.OrderStateName = "";
                                        }

                                        break;
                                    }

                                }

                                //当付款后，显示券的有效期（不等于【待支付】【已取消】【已退款】）
                                if (_litem.OrderState != 1 && _litem.OrderState != 4 && _litem.OrderState != 5) {

                                    //普通券产品需要显示有效期
                                    if (_litem.Count == 1) {
                                        _litem.Others = "有效期至{0}".format(_litem.EndDate);
                                    }

                                    //【小团购产品】
                                    if (_litem.GroupState > -1) {

                                        //不等于【已使用】【待退款】【已过期】，则显示为小团状态
                                        if (_litem.OrderState != 3 && _litem.OrderState != 7 && _litem.OrderState != 8) {
                                            _litem.OrderStateName = _litem.GroupStateName;
                                        }

                                        //已支付并且拼团成功（GroupState == 1）
                                        if (!_litem.ShowDetailList && _litem.OrderState == 2 && _litem.GroupState == 1) {

                                            //房券订单显示兑换（房券200 专辑房券500）
                                            if (_litem.ActivityType == 200 || _litem.ActivityType == 500) {

                                                //非 后台兑换、商户兑换 才会显示兑换功能  兑换日期必须在券码有效期内
                                                if (_litem.ExchangeMethod != 2 && _litem.ExchangeMethod != 4 && _litem.ExchangeMethod != 5 && _endTime >= _nowTime && _startTime <= _nowTime) {
                                                    _litem.ShowExchange = true;
                                                }
                                            }

                                            //显示预约（消费券）
                                            if (_litem.ReserveExId) {

                                                if (_litem.FirstReserveInfo) {

                                                    //已预约
                                                    _litem.OrderStateName = "已预约";
                                                    _litem.StateAddCss = "state-1";
                                                }
                                                else {

                                                    //需要预约
                                                    _litem.ShowReserve = true;
                                                }

                                            }
                                        }
                                    }
                                    //【大团购产品】
                                    else if (_litem.StepGroup) {

                                        //尾款支付时间格式化
                                        _litem.StepGroup.TailMoneyStartTime = _litem.StepGroup.TailMoneyStartTime.replace('T', ' ');// .split('T')[0];
                                        _litem.StepGroup.TailMoneyEndTime = _litem.StepGroup.TailMoneyEndTime.replace('T', ' ');// .split('T')[0];

                                        //已支付后显示尾款支付时间
                                        //_litem.Others = "请在规定时间内支付尾款";
                                        _litem.Others = "尾款支付时间：<span class='h'>{0}－{1}</span>".format(formatDate(_litem.StepGroup.TailMoneyStartTime, "MM月dd日 HH:mm"), formatDate(_litem.StepGroup.TailMoneyEndTime, "MM月dd日 HH:mm"));

                                        //团失败
                                        if (_litem.StepGroup.StepGroupState == 2) {

                                            //团购失败
                                            _litem.OrderStateName = "团购失败";

                                            //失败需要普通灰色字体显示
                                            _litem.Others = "尾款支付时间：{0}－{1}".format(formatDate(_litem.StepGroup.TailMoneyStartTime, "MM月dd日 HH:mm"), formatDate(_litem.StepGroup.TailMoneyEndTime, "MM月dd日 HH:mm"));

                                            _litem.ShowDetailList = false;
                                        }
                                        else if (_litem.StepGroup.StepGroupState == 0) {

                                            //团购中
                                            _litem.OrderStateName = "团购中";

                                            _litem.ShowDetailList = false;
                                        }
                                        else {

                                            //【完成】有尾款并已支付，则大团购已经算买到了
                                            if (_litem.FinalPayExchange && _litem.FinalPayExchange.OrderState == 2) {

                                                _litem.ShowDetailList = true;

                                                _litem.Others = "";

                                                _litem.OrderStateName = "团购成功";
                                                
                                            }
                                            else if (_litem.FinalPayExchange && _litem.FinalPayExchange.OrderState == 3) {

                                                _litem.ShowDetailList = true;

                                                _litem.Others = "";

                                                _litem.OrderStateName = "已使用";
                                                _litem.FinalPayExchange.OrderStateName = "已使用";

                                                //玩乐券显示为 已出票
                                                if (_litem.CategoryParentId == 20) {
                                                    _litem.OrderStateName = "已出票";
                                                    _litem.FinalPayExchange.OrderStateName = "已出票";
                                                }
                                                
                                            }
                                            else if (_litem.FinalPayExchange && _litem.FinalPayExchange.OrderState == 5) {

                                                _litem.ShowDetailList = true;

                                                _litem.Others = "";

                                                _litem.OrderStateName = "已退款";
                                                _litem.FinalPayExchange.OrderStateName = "已退款";
                                            }
                                            else if (_litem.FinalPayExchange && _litem.FinalPayExchange.OrderState == 7) {

                                                _litem.ShowDetailList = true;

                                                _litem.Others = "";

                                                _litem.OrderStateName = "待退款";
                                                _litem.FinalPayExchange.OrderStateName = "待退款";
                                            }
                                            else if (_litem.FinalPayExchange && _litem.FinalPayExchange.OrderState == 8) {

                                                _litem.ShowDetailList = true;

                                                _litem.Others = "";

                                                _litem.OrderStateName = "已过期";
                                                _litem.FinalPayExchange.OrderStateName = "已过期";
                                            }
                                            else {

                                                _litem.ShowDetailList = false;

                                                //显示支付尾款(需到支付尾款时间)
                                                if (_nowTime >= new Date(_litem.StepGroup.TailMoneyStartTime.replace(/-/g, "/"))
                                                    && _nowTime <= new Date(_litem.StepGroup.TailMoneyEndTime.replace(/-/g, "/"))) {

                                                    //待支付尾款
                                                    _litem.OrderStateName = "待支付尾款";

                                                    //高亮状态
                                                    _litem.StateAddCss = "state-1";

                                                    //_litem.ShowFinalPay = true;
                                                    _litem.ShowFinalTxt = "支付尾款 ¥" + _litem.StepGroup.Price;
                                                }
                                                else if (_nowTime <= new Date(_litem.StepGroup.TailMoneyStartTime.replace(/-/g, "/"))) {

                                                    //团购中
                                                    _litem.OrderStateName = "团购中";
                                                }
                                                else {

                                                    //尾款逾期未付
                                                    _litem.OrderStateName = "逾期未付";
                                                }
                                            }
                                        }
                                    }
                                    //【常规产品】
                                    else {

                                        //已支付
                                        if (!_litem.ShowDetailList && _litem.OrderState == 2) {

                                            //房券订单显示兑换（房券200 专辑房券500）
                                            if (_litem.ActivityType == 200 || _litem.ActivityType == 500) {

                                                //非 后台兑换、商户兑换 才会显示兑换功能  兑换日期必须在券码有效期内
                                                if (_litem.ExchangeMethod != 2 && _litem.ExchangeMethod != 4 && _litem.ExchangeMethod != 5 && _endTime >= _nowTime && _startTime <= _nowTime) {
                                                    _litem.ShowExchange = true;
                                                }
                                            }

                                            //显示预约（消费券）
                                            if (_litem.ReserveExId) {

                                                if (_litem.FirstReserveInfo) {

                                                    //已预约
                                                    _litem.OrderStateName = "已预约";
                                                    _litem.StateAddCss = "state-1";
                                                }
                                                else {

                                                    //需要预约
                                                    _litem.ShowReserve = true;
                                                }

                                            }
                                        }
                                    }
                                }

                                //当没有更多展示的时候，设置当前item的详情url
                                if (!_litem.ShowDetailList) {
                                    
                                    //不同环境下订单详情跳转url
                                    if (isApp) {
                                        _litem.url = "/Order/CouponOrderDetail?oid=" + _litem.OrderId + "&userid={userid}&_newpage=1&_dorpdown=1";
                                    }
                                    else {
                                        _litem.url = "/Order/CouponOrderDetail?oid=" + _litem.OrderId + "&userid=" + userid + "&_newpage=1&_dorpdown=1";
                                    }
                                }
                            }

                            //分页追加
                            for (var _tnum = 0; _tnum < orderListData.tabDetailList.length; _tnum++) {
                                var _tabDetailItem = orderListData.tabDetailList[_tnum];
                                if (_tabDetailItem.id === id) {
                                    _tabDetailItem.orderList.push(_litem);
                                    break;
                                }
                            }
                        }

                        //更新页码
                        for (var _tnum = 0; _tnum < orderListData.tabDetailList.length; _tnum++) {
                            var _tabDetailItem = orderListData.tabDetailList[_tnum];
                            if (_tabDetailItem.id === id) {
                                _tabDetailItem.start = (_tabDetailObj.start + _tabDetailObj.count)
                                break;
                            }
                        }

                        //list渲染完执行
                        Vue.nextTick(function () {

                            //显示加载状态模块
                            $(".status-default").show();

                            //停止下拉刷新
                            miniRefreshArr[id].endDownLoading();

                            //展开子订单功能操作
                            $(".more-detail-btn").each(function () {

                                $(this).unbind("click");
                                $(this).click(function () {

                                    var _open = $(this).data("open");

                                    var _orderitemid = $(this).data("orderitemid");
                                    var _orderitemObj = $("#" + _orderitemid);

                                    var _moreid = $(this).data("moreid");
                                    var _moreObj = $("#" + _moreid);

                                    //需要展开
                                    if (_open == 0) {
                                        $(this).html("折叠&#xe64f;");
                                        $(this).data("open", 1);
                                        _moreObj.slideDown(150);
                                    }
                                    else {

                                        //需要合并
                                        $(this).html("展开&#xe650;");
                                        $(this).data("open", 0);
                                        _moreObj.slideUp(150);
                                        
                                        try {
                                           
                                            //合并后，自动滑动到当前订单项位置
                                            setTimeout(function () {
                                                var _thisScrollTop = $("#minirefresh_" + id).scrollTop();
                                                var _backTop = parseFloat(_orderitemObj.offset().top);
                                                if (_backTop < 0) {
                                                    $("#minirefresh_" + id).animate({ scrollTop: _thisScrollTop + _backTop - 105 }, 150);
                                                }
                                            }, 180);

                                        } catch (e) {

                                        }
                                        
                                    }
                                });
                            });

                            //券订单去支付功能操作
                            $(".CouponOrderGoPay").each(function () {

                                $(this).unbind("click");
                                $(this).click(function () {

                                    var _payId = $(this).data("payid");
                                    productcouponGoPay(_payId);
                                });
                            });

                            //酒店订单去支付功能操作
                            $(".HotelOrderGoPay").each(function () {

                                $(this).unbind("click");
                                $(this).click(function () {

                                    var _orderid = $(this).data("orderid");
                                    hotelGoPay(_orderid);
                                });
                            });
                        })
                    }
                    else {

                        //停止下拉刷新
                        miniRefreshArr[id].endDownLoading();

                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        if (!isfirst) {
                            $("#scrollpageloading-" + id).hide();
                        }

                        if (_tabDetailObj.start == 0) {
                            $("#null-list_" + id).show();
                            $("#scrollpageloading-" + id).hide();
                        }

                        //更新状态
                        //_tabDetailObj.stop = true;

                        for (var _tnum = 0; _tnum < orderListData.tabDetailList.length; _tnum++) {
                            var _tabDetailItem = orderListData.tabDetailList[_tnum];
                            if (_tabDetailItem.id === id) {
                                _tabDetailItem.stop = true;
                                break;
                            }
                        }

                        //注意，由于默认情况是开启满屏自动加载的，所以请求失败时，请务必endUpLoading(true)，防止无限请求
                        miniRefreshArr[id].endUpLoading(true);
                    }
                }
                else {

                    //停止下拉刷新
                    miniRefreshArr[id].endDownLoading();

                    //如果是第一页，则不显示“没有更多了”，不然很奇怪
                    if (!isfirst) {
                        $("#scrollpageloading-" + id).hide();
                    }
                    
                    if (_tabDetailObj.start == 0) {
                        $("#null-list_" + id).show();
                        $("#scrollpageloading-" + id).hide();
                    }

                    //更新状态
                    //_tabDetailObj.stop = true;

                    for (var _tnum = 0; _tnum < orderListData.tabDetailList.length; _tnum++) {
                        var _tabDetailItem = orderListData.tabDetailList[_tnum];
                        if (_tabDetailItem.id === id) {
                            _tabDetailItem.stop = true;
                            break;
                        }
                    }

                    //注意，由于默认情况是开启满屏自动加载的，所以请求失败时，请务必endUpLoading(true)，防止无限请求
                    miniRefreshArr[id].endUpLoading(true);
                }
            }

            console.log(_tabDetailObj);
            console.log(_tabDetailObj.stop);

            //get request
            if (!_tabDetailObj.stop) {
                var _listDic = { userid: userid, oType: id, start: _tabDetailObj.start, count: _tabDetailObj.count };
                $.get(_Config.APIUrl + '/api/Order/GetOrderListByUserId', _listDic, function (data) {
                    loadOrderListCallback(data);
                });
            }
            else {

                //注意，由于默认情况是开启满屏自动加载的，所以请求失败时，请务必endUpLoading(true)，防止无限请求
                miniRefreshArr[id].endUpLoading(true);

                isload = true;
                $("#scrollpageloading-" + id).hide();
            }
        }

    }

    //页面初始加载
    setTimeout(function () {
        selectMenu(loadItem.id, true);
        loadMoreOrderList(loadItem.id, loadItem.type, true, false);
    }, 0);

    var tabsScroll = {};  //头部滚动菜单对象
    var bindEvent = function () {

        $("#tabs .t-item").each(function () {

            var _mitem = $(this);

            //设置宽度
            if (tabDetailList.length > 1) {

                var _chuNum = (tabDetailList.length > 4 ? 4.5 : tabDetailList.length);
                var _itemWidth = winWidth / _chuNum;
                //console.log(_itemWidth)
                _mitem.css("width", _itemWidth);

                //让头菜单支持横向滑动
                tabsScroll = new IScroll('#tabs', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
            }

            //event
            _mitem.click(function () {

                var _relid = _mitem.data("relid");

                //设置选中
                selectMenu(_relid);

                var _tabDetailObj = {};
                for (var _tnum = 0; _tnum < orderListData.tabDetailList.length; _tnum++) {
                    var _tabDetailItem = orderListData.tabDetailList[_tnum];
                    if (_tabDetailItem.id === _relid) {
                        _tabDetailObj = _tabDetailItem;
                        loadItem = _tabDetailItem;
                        break;
                    }
                }

                //console.log(_tabDetailObj);

                if (!_tabDetailObj.orderList || !_tabDetailObj.orderList.length) {
                    loadMoreOrderList(_tabDetailObj.id, _tabDetailObj.type, false, true);
                }
                else {

                    //显示加载状态模块
                    $(".status-default").show();
                }
            });

        });
    }
    bindEvent();

    //选中当前菜单
    var selectMenu = function (id, isfirst) {

        $("#tabs .t-item").each(function () {

            var _mitem = $(this);
            var _relid = _mitem.data("relid");

            _mitem.removeClass("sel");
            if (_relid === id) {

                _mitem.addClass("sel");

                $(".minirefresh-wrap").hide();
                $("#minirefresh_" + id).show();

                //隐藏加载状态模块
                $(".status-default").hide();

                //$(".scroll-div").hide();
                //$("#scroll_" + id).show();

                var _num = parseInt(_mitem.data("num"));
                if (_num > 0) {

                    //如果选择的项靠后，则自动滚动到指定区域
                    tabsScroll.scrollToElement('.t-item:nth-child(' + (_num - 1) + ')', 500);
                }
            }

        });
    }
});

//消费券去支付方法
var productcouponGoPay = function (_couponOrderId) {

    $.get('/Coupon/CheckCanPay', { orderid: _couponOrderId }, function (result) {
        var success = result.Success;
        if (success == "0") {
            location.href = result.Url;
        }
        else {
            alert("订单已过期或已取消");
            location.reload();
        }
    });
}

//酒店去支付方法
function hotelGoPay(orderId) {

    var _goPayUrl = "/Order/Pay?order=" + orderId + "&payChannels=tenpay,alipay,chinapay&_newpage=1&_isoneoff=1";

    //app环境下的跳转链接不同，很奇怪，下面if的写法在ios上有问题？要原生配合查一下，这里暂时只对Android app做处理
    if (isApp && B.v.android) {

        var _completeUrl = "whotelapp://www.zmjiudian.com/personal/order/{0}".format(orderId);
        _goPayUrl = "whotelapp://orderPay?orderid={0}&finishurl={1}&paytype={2}".format(orderId, _completeUrl, "all");
    }

    window.location.href = _goPayUrl;
}

//去提交购买(如大团购产品的尾款支付)
function goCouponBook(bookpostion, skuid, paynum, fromwxuid, coid) {

    switch (parseInt(bookpostion)) {
        case 0: {
            //不需要预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&coid={4}&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid, coid));
            break;
        }
        case 1: {
            //前置预约
            gourl("/Coupon/CouponReserve?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&exid={4}&coid={5}&prereserve=1&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid, 0, coid));
            break;
        }
        case 2: {
            //后置预约
            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&coid={4}&_isoneoff=1&_newpage=1".format(skuid, paynum, userid, fromwxuid, coid));
            break;
        }

    }
}

//专辑房券兑换
function goCouponExchangeActivityType500(userid, exchangeid, relPackageAlbumsID) {
    if (relPackageAlbumsID > 0) {
        gourl("/exchange/packages/{0}?userid={1}&exchangeid={2}&userlat=0&userlng=0&_newpage=1".format(relPackageAlbumsID, userid, exchangeid));
    }
    else {
        alert("兑换房券请联系客服 4000021702");
    }
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {



}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //init data
    _InitApp(userid, apptype, appvercode, appverno);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}