var _Config = new Config();
var oid = $("#oid").val();
var userid = $("#userid").val();
var isApp = $("#isApp").val() == "1";

var year0 = $("#year0").val();
var month0 = $("#month0").val();
var day0 = $("#day0").val();
var hour0 = $("#hour0").val();
var minute0 = $("#minute0").val();
var second0 = $("#second0").val();

var orderDetailData;

//_Config.APIUrl = "http://192.168.1.114:8000";

$(function () {

    //init data
    var init = function () {

        //初始订单详情
        orderDetailData = new Vue({
            el: "#orderDetail",
            data: {
                "detailInfo": {}
            }
        })
    }
    init();

    //加载订单信息
    var loadOrderDetail = function () {

        var _detailDic = { id: oid };
        $.get(_Config.APIUrl + '/api/coupon/GetExchangeCouponOrderDetail', _detailDic, function (data) {

            console.log(data);

            if (data) {

                data.UserID = userid;

                //now time
                var _nowTime = new Date(parseInt(year0), parseInt(month0) - 1, parseInt(day0), parseInt(hour0), parseInt(minute0), parseInt(second0));

                //有效期 开始日期
                var dtArr = (data.StartTime).split("-");
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
                data.StartTime = data.StartTime.split('T')[0];

                //有效期 结束日期
                dtArr = (data.ExpireTime).split("-");
                dayArr = dtArr[2].split("T");
                timeArr = dayArr[1].split(":");
                var _y3 = dtArr[0];
                var _mo3 = parseInt(dtArr[1]) - 1;
                var _d3 = dayArr[0];
                var _h3 = timeArr[0];
                var _mi3 = timeArr[1];
                var _s3 = timeArr[2];
                var _endTime = new Date(parseInt(_y3), parseInt(_mo3), parseInt(_d3), parseInt(_h3), parseInt(_mi3), parseInt(_s3));

                //有效期 结束日期格式化
                data.ExpireTime = data.ExpireTime.split('T')[0];

                //有效期模块显示（OthersTip可能会显示 大团购产品的付尾款时间 提示）
                data.OthersTip = "有效期至" + data.ExpireTime;

                ////是否在有效期内
                //data.IsStartAndExpireWithIn = false;
                //if (data.ExpireTime >= new Date() && data.StartTime < new Date()) {
                //    data.IsStartAndExpireWithIn = true;
                //}

                //电话/地址显示
                if (data.DicProperties) {
                    data.telObj = [];
                    data.otherObjList = [];
                    for (var _key in data.DicProperties) {
                        var _val = data.DicProperties[_key];
                        var _lab = _key.replace(":", "").replace("：", "");
                        if (_lab.indexOf("电话") >= 0) {
                            var _tel = _val;
                            var _telex = "";
                            if (_val.indexOf("转") >= 0) {
                                _tel = _val.split('转')[0];
                                _telex = "转" + _val.split('转')[1];
                            }

                            data.otherObjList.push({
                                "lab": _lab,
                                "tel": _tel,
                                "telex": _telex,
                                "istel": 1,
                            });
                        }
                        else {
                            data.otherObjList.push({
                                "lab": _lab,
                                "val": _val,
                                "telex": "",
                                "istel": 0
                            })
                        }
                    }
                }

                //对使用说明做过滤规则相关处理
                if (data.Notice && data.Notice.length) {
                    for (var _noticeNum = 0; _noticeNum < data.Notice.length; _noticeNum++) {

                        var regexp = /(http:\/\/|https:\/\/)((\w|=|\?|\.|\/|&|-)+)/g; //"/(http://|https://)((w|=|?|.|/|&|-)+)/g";

                        var _notice = data.Notice[_noticeNum];
                        data.Notice[_noticeNum] = _notice.replace(regexp, function ($url) {
                            return "<a href='" + $url + "' target='_blank'>" + $url + "</a>";
                        });
                    }
                }

                //显示联系客服
                data.ShowServiceTell = true;

                //需要支付
                data.ShowPay = false;

                //需要支付尾款
                data.ShowFinalPay = false;
                data.ShowFinalPay0 = false;
                data.ShowFinalTxt = "支付尾款";
                data.FinalNeedCouponOrderId = "";

                //需要兑换（房券）
                data.ShowExchange = false;

                //需要预约（消费券）
                data.ShowReserve = false;

                //需要取消预约
                data.ShowCanelReserve = false;

                //已预约信息
                data.FirstReserveInfo = null;

                //预约配置
                data.ReserveSKUID = 0;
                data.ReserveExId = 0;

                //可退款
                data.ShowRefund = false;

                //退款状态
                data.RefundState = 0;

                //退款政策（）
                data.ReturnPolicy = 0;

                //显示 获取海报（目前只有发起助力的产品才会有）
                data.ShowGetGroupPoster = false;
                data.GetGroupPosterUrl = "";

                //当前券ID（排除定金）
                data.ThisExchangeId = 0;

                //当前的房券专辑id
                data.RelPackageAlbumsID = 0;

                //券类型
                data.ActivityType = 0;

                //是否助力团购产品
                data.IsLikeGroup = data.CategoryId == 21;

                //券码
                data.CouponNo = "";

                //券状态
                data.StateName = "";

                //券状态追加样式
                data.StateAddCss = "";

                //是否显示券码模块
                data.ShowCouponSection = true;

                //头Tip
                data.TopTip = "";

                //是否显示券二维码功能
                data.ShowQrcode = false;

                //当前券的核销类型（1仅APP兑换 2仅后台兑换 3后台APP都可兑换 4商户兑换 5至商家扫码使用）
                data.ExchangeMethod = 2;

                //券来源类型（0系统自动生成 1商家提供券码 2在第三方手动下单 3通过API第三方下单）
                data.ExchangeNoType = 0;

                //关于券码的扩展维护内容（一般是第三方提供券码的券，会业务人员自己维护券码信息，也可能是一句说明的话术）
                data.ExchangeTipsName = "";

                //当前券包含产品的优惠状态（IsPromotion 如果为true，表示为壳产品的子产品）
                data.IsPromotion = false;

                //exchangelist处理
                if (data.ExchangeCouponList) {
                    
                    for (var _enum = 0; _enum < data.ExchangeCouponList.length; _enum++) {
                        var _eitem = data.ExchangeCouponList[_enum];

                        //exchange id
                        data.ThisExchangeId = _eitem.ID;

                        //RelPackageAlbumsID id
                        data.RelPackageAlbumsID = _eitem.RelPackageAlbumsID;

                        //核销类型
                        data.ExchangeMethod = _eitem.ExchangeMethod;

                        //券来源类型
                        data.ExchangeNoType = _eitem.ExchangeNoType;

                        //券Tips
                        data.ExchangeTipsName = _eitem.ExchangeTipsName;

                        //是否壳产品的子产品
                        data.IsPromotion = _eitem.IsPromotion;

                        //获取券码
                        if (_eitem.ExchangeNo) {
                            data.CouponNo = _eitem.ExchangeNo;
                        }

                        //获取非定金的券码（如果是大团购获取尾款的券码，非大团购则肯定都是非定金）
                        if (!_eitem.IsDepositOrder) {

                            //需要预约
                            if (_eitem.IsBook) {

                                //预约配置
                                data.ReserveSKUID = _eitem.SKUID;
                                data.ReserveExId = _eitem.ID;

                                //已预约信息
                                if (_eitem.BookUserDateList && _eitem.BookUserDateList.length) {
                                    data.FirstReserveInfo = _eitem.BookUserDateList[0];

                                    //时间格式化
                                    data.FirstReserveInfo.BookDay = data.FirstReserveInfo.BookDay.split('T')[0];

                                    //显示取消预约(只有已支付的订单，才能操作取消预约)
                                    if (data.State == 2) {
                                        data.ShowCanelReserve = true;
                                    }

                                    //显示券二维码功能
                                    data.ShowQrcode = true;
                                }
                            }
                        }

                        //取最后一条的退款状态、退款政策等
                        if (_enum + 1 == data.ExchangeCouponList.length) {

                            //券类型
                            data.ActivityType = _eitem.ActivityType;

                            //退款状态与政策
                            data.RefundState = _eitem.RefundState;
                            data.ReturnPolicy = _eitem.ReturnPolicy;
                        }

                        //下单时间格式化
                        _eitem.CreateTime = _eitem.CreateTime.replace('T', ' ');// .split('T')[0];
                        _eitem.CreateTime = _eitem.CreateTime.split('.')[0]
                        //_eitem.CreateTime = formatDate(_eitem.CreateTime, "yyyy-MM-dd HH:mm")

                        //大团购支付尾款时需要使用的CouponOrderId
                        data.FinalNeedCouponOrderId = _eitem.CouponOrderId;
                    }
                }

                //小团购逻辑判断
                if (data.GroupPurchase) {

                    //小团购产品默认设置链接（支付过带groupid）
                    data.productUrl = "/coupon/group/product/{0}/{1}?_newpage=1&_newtitle=1&_dorpdown=1&userid={2}".format(data.SkuID, data.GroupPurchase.ID, userid);

                    if (data.State == 1) {

                        //小团购产品链接（未支付不带groupid）
                        data.productUrl = "/coupon/group/product/{0}/{1}?_newpage=1&_newtitle=1&_dorpdown=1&userid={2}".format(data.SkuID, 0, userid);

                        switch (data.GroupPurchase.State) {
                            case 0:
                            case 3: {
                                data.StateName = "待支付";
                                data.StateAddCss = "state-1";
                                data.CouponNo = "支付后发起拼团";
                                data.TopTip = "已提交，10分钟内未支付将自动取消";
                                data.ShowPay = true;
                                break;
                            }
                            case 1: {
                                data.StateName = "待支付";
                                data.StateAddCss = "state-1";
                                data.CouponNo = "支付后显示";
                                data.TopTip = "已提交，10分钟内未支付将自动取消";
                                data.ShowPay = true;
                                break;
                            }
                            case 2: {
                                data.CouponNo = "拼团失败";
                                break;
                            }
                            case 4: {
                                data.CouponNo = "拼团取消";
                                break;
                            }

                        }
                    }
                    else if (data.State == 3) {
                        if (data.CategoryParentId == 20) {
                            data.StateName = "已出票";
                        }
                        else {
                            data.StateName = "已使用";
                        }
                    }
                    else if (data.State == 5) {
                        data.CouponNo = "订单已退款";
                        data.StateName = "已退款";
                    }
                    else if (data.State == 7) {
                        data.CouponNo = "订单待退款";
                        data.StateName = "待退款";
                    }
                    else if (data.State == 8) {
                        data.CouponNo = "订单已过期";
                        data.StateName = "已过期";
                    }
                    else {

                        //助力团购获取海报的链接
                        data.GetGroupPosterUrl = "http://www.zmjiudian.com/coupon/group/tree/{0}/{1}?_newpage=1&userid={2}".format(data.SkuID, data.GroupPurchase.ID, userid);

                        switch (data.GroupPurchase.State) {
                            case 0: {
                                data.StateName = "拼团中";
                                data.CouponNo = "拼团成功后，生成券码";

                                //助力团购，显示获取海报的按钮
                                if (data.IsLikeGroup) {
                                    data.ShowGetGroupPoster = true;
                                }
                                break;
                            }
                            case 3: {
                                data.StateName = "拼团取消";
                                data.CouponNo = "拼团取消";
                                break;
                            }
                            case 1: {

                                data.StateName = "拼团成功";
                                data.StateAddCss = "state-1";

                                //显示券二维码功能
                                data.ShowQrcode = true;

                                //显示兑换（房券200 专辑房券500）
                                if (data.ActivityType == 200 || data.ActivityType == 500) {

                                    //非 后台兑换、商户兑换 才会显示兑换功能  兑换日期必须在券码有效期内
                                    if (data.ExchangeMethod != 2 && data.ExchangeMethod != 4 && data.ExchangeMethod != 5 && _endTime >= _nowTime && _startTime <= _nowTime) {
                                        data.ShowExchange = true;
                                    }
                                }

                                //显示预约（消费券）
                                if (data.ReserveExId && !data.FirstReserveInfo) {
                                    data.ShowReserve = true;
                                }

                                //助力团购，显示获取海报的按钮
                                if (data.IsLikeGroup) {
                                    data.ShowGetGroupPoster = true;
                                }
                                break;
                            }
                            case 2: {
                                data.StateName = "拼团失败";
                                data.CouponNo = "拼团失败";
                                break;
                            }
                            case 4: {
                                data.StateName = "拼团取消";
                                data.CouponNo = "拼团取消";
                                break;
                            }

                        }
                    }
                }
                else {

                    //大团购逻辑判断
                    if (data.CouponOrderStepGroup) {

                        //大团购产品链接
                        data.productUrl = "/coupon/stepgroup/product/{0}?_newpage=1&_newtitle=1&_dorpdown=1&userid={1}".format(data.SkuID, userid);

                        //尾款支付时间格式化
                        data.CouponOrderStepGroup.TailMoneyStartTime = data.CouponOrderStepGroup.TailMoneyStartTime.replace('T', ' ');// .split('T')[0];
                        data.CouponOrderStepGroup.TailMoneyEndTime = data.CouponOrderStepGroup.TailMoneyEndTime.replace('T', ' ');// .split('T')[0];

                        if (data.State == 1) {

                            //团失败
                            if (data.CouponOrderStepGroup.StepGroupState == 2) {

                                data.StateName = "拼团失败";
                                data.CouponNo = "拼团失败";

                                //不显示券码模块
                                data.ShowCouponSection = false;
                            }
                            else {

                                //如果是未失败、未支付的大团购，肯定是定金
                                data.StateName = "待支付定金";
                                data.StateAddCss = "state-1";
                                data.CouponNo = "待支付定金";
                                data.TopTip = "已提交，10分钟内未支付将自动取消";

                                //显示支付
                                data.ShowPay = true;

                                //不显示券码模块
                                data.ShowCouponSection = false;
                            }
                        }
                        else if (data.State == 3) {
                            if (data.CategoryParentId == 20) {
                                data.StateName = "已出票";
                            }
                            else {
                                data.StateName = "已使用";
                            }
                        }
                        else if (data.State == 5) {
                            data.CouponNo = "订单已退款";
                            data.StateName = "已退款";

                            //默认尾款时间灰色显示
                            data.TopTip = "请在{0}－{1}支付尾款".format(formatDate(data.CouponOrderStepGroup.TailMoneyStartTime, "MM月dd日 HH:mm"), formatDate(data.CouponOrderStepGroup.TailMoneyEndTime, "MM月dd日 HH:mm"));
                        }
                        else if (data.State == 7) {
                            data.CouponNo = "订单待退款";
                            data.StateName = "待退款";

                            //默认尾款时间灰色显示
                            data.TopTip = "请在{0}－{1}支付尾款".format(formatDate(data.CouponOrderStepGroup.TailMoneyStartTime, "MM月dd日 HH:mm"), formatDate(data.CouponOrderStepGroup.TailMoneyEndTime, "MM月dd日 HH:mm"));
                        }
                        else if (data.State == 8) {
                            data.CouponNo = "订单已过期";
                            data.StateName = "已过期";
                        }
                        else {

                            //尾款时间高亮显示
                            data.TopTip = "请在<span class='h'>{0}－{1}</span>支付尾款".format(formatDate(data.CouponOrderStepGroup.TailMoneyStartTime, "MM月dd日 HH:mm"), formatDate(data.CouponOrderStepGroup.TailMoneyEndTime, "MM月dd日 HH:mm"));

                            //定金已支付，但没有完成的：要么失败了 or 待支付尾款
                            if (!data.CouponOrderStepGroup.IsPayFinish) {

                                //团失败
                                if (data.CouponOrderStepGroup.StepGroupState == 2) {

                                    data.StateName = "团购失败";
                                    data.CouponNo = "团购失败";

                                    //不显示券码模块
                                    data.ShowCouponSection = false;

                                    //默认尾款时间灰色显示
                                    data.TopTip = "请在{0}－{1}支付尾款".format(formatDate(data.CouponOrderStepGroup.TailMoneyStartTime, "MM月dd日 HH:mm"), formatDate(data.CouponOrderStepGroup.TailMoneyEndTime, "MM月dd日 HH:mm"));
                                }
                                else if (data.CouponOrderStepGroup.StepGroupState == 0) {

                                    //团购中
                                    data.StateName = "团购中";
                                    data.CouponNo = "团购中，等待成团";
                                }
                                else {

                                    //显示支付尾款(需到支付尾款时间)
                                    if (_nowTime >= new Date(data.CouponOrderStepGroup.TailMoneyStartTime.replace(/-/g, "/")) 
                                        && _nowTime <= new Date(data.CouponOrderStepGroup.TailMoneyEndTime.replace(/-/g, "/"))) {

                                        //待支付尾款
                                        data.StateName = "待支付尾款";
                                        data.StateAddCss = "state-1";
                                        data.CouponNo = "尾款支付后显示";

                                        data.ShowFinalPay = true;
                                        data.ShowFinalTxt = "支付尾款 ¥" + data.CouponOrderStepGroup.Price;
                                    }
                                    else if (_nowTime <= new Date(data.CouponOrderStepGroup.TailMoneyStartTime.replace(/-/g, "/"))) {

                                        //团购中
                                        data.StateName = "团购中";
                                        data.CouponNo = "定金已付，等待支付尾款";

                                        data.ShowFinalPay0 = true;
                                        data.ShowFinalTxt = "支付尾款 ¥" + data.CouponOrderStepGroup.Price;
                                    }
                                    else {

                                        //尾款逾期未付
                                        data.StateName = "逾期未付";
                                        data.CouponNo = "尾款逾期未付";

                                        //默认尾款时间灰色显示
                                        data.TopTip = "请在{0}－{1}支付尾款".format(formatDate(data.CouponOrderStepGroup.TailMoneyStartTime, "MM月dd日 HH:mm"), formatDate(data.CouponOrderStepGroup.TailMoneyEndTime, "MM月dd日 HH:mm"));
                                    }
                                }
                            }
                            else {

                                /* 尾款已付，订单已完成 */

                                //显示券二维码功能
                                data.ShowQrcode = true;

                                //显示兑换（房券）
                                if (data.ActivityType == 200 || data.ActivityType == 500) {

                                    //非 后台兑换、商户兑换 才会显示兑换功能
                                    if (data.ExchangeMethod != 2 && data.ExchangeMethod != 4 && data.ExchangeMethod != 5 && _endTime >= _nowTime && _startTime <= _nowTime) {
                                        data.ShowExchange = true;
                                    }
                                }

                                //显示预约（消费券）
                                if (data.ReserveExId && !data.FirstReserveInfo) {
                                    data.ShowReserve = true;
                                }
                            }
                        }

                    }
                    else {

                        //常规产品链接
                        data.productUrl = "/coupon/product/{0}?_newpage=1&_newtitle=1&_dorpdown=1&userid={1}".format(data.SkuID, userid);

                        //常规券订单状态判断
                        switch (data.State) {

                            //已提交待支付
                            case 1: {

                                data.StateName = "待支付";
                                data.StateAddCss = "state-1";
                                data.CouponNo = "支付成功后生成券码";
                                data.TopTip = "已提交，10分钟内未支付将自动取消";

                                //显示支付
                                data.ShowPay = true;
                                break;
                            }
                                //已支付
                            case 2: {

                                //是否退款中
                                if (data.RefundState > 0) {

                                    data.StateName = "退款中";
                                }
                                else {

                                    data.StateName = "已支付";

                                    //显示券二维码功能
                                    data.ShowQrcode = true;

                                    //显示兑换（房券）
                                    if (data.ActivityType == 200 || data.ActivityType == 500) {

                                        //非 后台兑换、商户兑换 才会显示兑换功能
                                        if (data.ExchangeMethod != 2 && data.ExchangeMethod != 4 && data.ExchangeMethod != 5 && _endTime >= _nowTime && _startTime <= _nowTime) {
                                            data.ShowExchange = true;
                                        }
                                    }

                                    //显示预约（消费券）
                                    if (data.ReserveExId && !data.FirstReserveInfo) {
                                        data.ShowReserve = true;
                                    }

                                    //是否可以退款
                                    if (data.ReturnPolicy == 2) {
                                        data.ShowRefund = true;
                                    }
                                }

                                break;
                            }
                                //已消费(消费后什么都不能再操作了)
                            case 3: {

                                if (data.CategoryParentId == 20) {
                                    data.StateName = "已出票";
                                }
                                else {
                                    data.StateName = "已使用";
                                }
                                break;
                            }
                                //已取消
                            case 4:
                            case 50: {

                                data.CouponNo = "订单已取消";

                                data.StateName = "已取消";
                                break;
                            }
                                //已退款
                            case 5: {

                                data.CouponNo = "订单已退款";

                                data.StateName = "已退款";
                                break;
                            }
                                //待退款
                            case 7: {

                                data.CouponNo = "订单待退款";

                                data.StateName = "待退款";
                                break;
                            }
                                //已过期
                            case 8: {

                                data.CouponNo = "订单已过期";

                                data.StateName = "已过期";
                                break;
                            }
                        }
                    }

                }

                //【壳产品】 不显示券码模块
                if (data.IsPackage) {
                    data.ShowCouponSection = false;
                }
                else {

                    /* 【壳产品】，都显示券码模块 */

                    //仅后台核销类型，券码自定义
                    if (data.ExchangeMethod == 2) {
                        
                        //自定义券码字段显示
                        data.CouponNo = "请联系客服使用";

                        //不显示二维码功能
                        data.ShowQrcode = false;
                    }

                    //至商家扫码核销类型的券，券码自定义
                    if (data.ExchangeMethod == 5) {

                        //自定义券码字段显示
                        data.CouponNo = "请在使用当日现场微信扫码入场";

                        //不显示二维码功能
                        data.ShowQrcode = false;
                    }

                    //券如果是 在第三方手动下单、通过API第三方下单，券码显示为业务人员维护的一句话
                    if (data.ExchangeNoType == 2 || data.ExchangeNoType == 3) {

                        //自定义券码字段显示
                        data.CouponNo = data.ExchangeTipsName;

                        //不显示二维码功能
                        data.ShowQrcode = false;
                    }
                }

                //bind data
                orderDetailData.detailInfo = data;

                Vue.nextTick(function () {

                    //生成二维码
                    if (data.ShowQrcode) {

                        var loadCouponNoQrcode = function () {

                            var qrcodeContent = ("http://www.zmjiudian.com/Shop/Coupon?exno=" + data.CouponNo);
                            var qrcode = new QRCode('qrcode-section', {
                                text: qrcodeContent,
                                width: 256,
                                height: 256,
                                colorDark: '#000000',
                                colorLight: '#ffffff',
                                correctLevel: QRCode.CorrectLevel.H
                            });

                        }
                        loadCouponNoQrcode();
                    }


                })
            }
        });
    }
    loadOrderDetail();

    //设置页面刷新监听
    Global.Monito.Listener("coupon", "couponorderdetail", 1, 500);
});

//消费券去支付方法
var productcouponGoPay = function(_couponOrderId) {
    
    $.get('/Coupon/CheckCanPay', { orderid: _couponOrderId }, function (result) {
        var success = result.Success;
        if (success == "0")
        {
            location.href = result.Url;
        }
        else
        {
            alert("订单已过期或已取消");
            location.reload();
        }
    });
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
        bootbox.confirm({
            message: "<div class='alert-rulesmsg'>兑换房券请联系客服<br />4000-021-702</div>",
            buttons: {
                confirm: { label: '联系客服', },
                cancel: { label: '取消', }
            },
            callback: function (result) {
                if (result) {
                    location.href = "tel:4000021702";
                }
            },
            closeButton: false,
            onEscape: function () { }
        });
    }
}

//退款方法
var refund = function (_exid) {

    if (confirm("这么优惠的套餐，你确认退款吗？")) {

        var dic = {};
        dic["exid"] = _exid;
        dic["userid"] = userid;

        $.get('/Coupon/ProductCouponReturnPolicy', dic, function (result) {

            console.log(result)

            if (result.Success == 0) {
                alert("退款申请已提交");
                location.reload();
            }
            else {
                alert(result.Message);
            }

        });
    }
}

//取消预约
var cancelReserve = function (_ebookid) {

    if (confirm("取消预约后，当日名额将不再保留")) {
        $.get(_Config.APIUrl + '/api/Coupon/CancelBookInfo', { id: _ebookid }, function (result) {

            console.log(result)

            if (result.RetCode === "1") {
                alert("你的预约已取消成功");
                location.reload();
            }
            else {
                alert(result.Message);
            }

        });
    }
}



//弹出券二维码
var showQrcode = function (exno) {

    var _qrcodeHtml = $("#qrcode-section").html();

    _Modal.show({
        title: "",
        content: _qrcodeHtml,
        textAlign: "center",
        confirmText: "关闭",
        confirm: function () {
            _Modal.hide();
        }
    });

    $("._modal-section").css("top", "15%");
}
var hideQrcodeSection = function () {

    $("#qrcode-section").hide();
    $("#qrcode-section").html("");

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
