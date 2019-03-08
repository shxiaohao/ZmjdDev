var wwidth = $(window).width();
var isapp = $("#isapp").val() == "1";
var isvip = $("#isvip").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var customerType = $("#customerType").val();
var stype = $("#stype").val();
var pubskuid = $("#skuid").val();
var pubgroupid = $("#groupid").val();
var origroupid = $("#groupid").val();
var pubaid = $("#pubaid").val();
var pubuserid = $("#userid").val();
var pubopenid = $("#openid").val();
var skuPrice = $("#skuPrice").val();
var skuVipPrice = $("#skuVipPrice").val();
var appShareLink = $("#appShareLink").val();
var walletRoomCouponLink = $("#walletRoomCouponLink").val();
var walletProductCouponLink = $("#walletProductCouponLink").val();
var weixinCode = $("#weixinCode").val();
var weixinAcountId = $("#weixinAcountId").val();

var year0 = $("#year0").val();
var month0 = $("#month0").val();
var day0 = $("#day0").val();
var hour0 = $("#hour0").val();
var minute0 = $("#minute0").val();
var second0 = $("#second0").val();

var _name = $("#_name").val();

//SKU加载次数（每切换一次SKU累加一次）
var loadNum = 0;
var loadNumList = [0];

var singleSku = {};
var groupSku = {};

$(document).ready(function () {
    
    var _Config = new Config();

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //目前只有拿到微信授权code时，才自动登录
    if (weixinCode && weixinCode.length > 0) {

        //检测登录并自动登录
        if (!isapp && pub_userid == "0") {
            _loginModular.verify.autoLogin(loginCheckFun);
        }
    }

    //水壶动画
    var shuihuWaterFun = function () {

        //水壶
        var _shuihuObj = $(this);
        var _shuihuLeft = _shuihuObj.position().left;
        var _shuihuTop = _shuihuObj.position().top;

        //隐藏水壶tip
        $(".clickme-tip").hide();

        //获取树的位置等信息
        var _treeImg = $(".canvas .tree ._img");
        var _goWidth = _treeImg.width();
        var _goLeft = _treeImg.offset().left + (_goWidth / 2);
        var _goTop = _treeImg.offset().top - 70 - (16*8);

        console.log(_goTop);
        console.log(_goLeft);
        
        var _sVal = 500;

        //移动到树
        _shuihuObj.animate({ left: _shuihuLeft, top: _shuihuTop }, 0);
        _shuihuObj.animate({ left: _goLeft, top: _goTop }, _sVal);

        //旋转45度，模拟浇水动作
        setTimeout(function () {
            _shuihuObj.css("transform", "rotate(-45deg)");
        }, _sVal - 150);

        //控制水滴，模拟浇水
        var _oneWater = $(".one-water")
        _oneWater.animate({ left: _goLeft - 10, top: _goTop + 30 }, 0);
        setTimeout(function () {

            //显示水滴
            _oneWater.fadeIn();

            //移动水滴至树
            _oneWater.animate({ top: _goTop + 80 }, 500);

            //隐藏水滴
            setTimeout(function () { _oneWater.fadeOut(); }, 500);

        }, _sVal - 150);

        //浇水结束，归位
        setTimeout(function () {
            _shuihuObj.css("transform", "rotate(0deg)");
            _shuihuObj.animate({ left: _shuihuLeft, top: _shuihuTop }, _sVal);

            //浇水后，弹出关注二维码
            showQrcodeSection();

        }, _sVal * 3);


    }

    //刷新树的位置
    var refTreeImg = function () {

        var _grassImg = $(".canvas .grass img");
        var _grassTop = _grassImg.offset().top;

        var _tree = $(".canvas .tree");
        var _treeHeight = _tree.height();
        _tree.css("top", _grassTop - _treeHeight + 55 - (16*8));

        _tree.fadeIn();
    }

    //加载专属二维码
    var loadQrcode = function () {

        //GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID
        var _sceneStr = "GROUPTREE_{0}_{1}_{2}_{3}".format(pubskuid, pubaid, origroupid, pubuserid);

        var _dic = {
            weixinAcount: weixinAcountId, //周末酒店服务号 浩颐
            expires: 2592000,
            actionName: "QR_STR_SCENE",
            sceneId : 0,
            sceneStr: _sceneStr
        };

        console.log(_dic);

        $.get(_Config.APIUrl + '/api/WeixinApi/CreateAccountQrcode', _dic, function (_data) {

            console.log(_data);

            if (_data && _data.indexOf("ticket") >= 0) {

                var _dataObj = JSON.parse(_data);
                console.log(_dataObj);

                var _src = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}".format(_dataObj.ticket);
                $("#qrcode-img").attr("src", _src);

                console.log(_src);

                //$("#testImg").attr("src", _src);

                //$("#testImg").load(function () {

                //    html2canvas($("#testDiv")[0], { useCORS: true }).then(function (canvas) {
                //        //document.body.appendChild(canvas);
                //        $("#showImg").attr("src", canvas.toDataURL());
                //    });
                //});
            }

        });

    }
    //loadQrcode();

    //弹出关注二维码
    var showQrcodeSection = function () {

        $(".service-qrcode").fadeIn();
        $(".service-qrcode-bg").fadeIn();
    }

    //更新团信息
    var refGroupState = function () {

    }

    var productDetailData = new Vue({
        el: "#product-detail",
        data: {
            "productDetail": {
                "BuyStatus": {}
            }
        }
    })

    //加载产品详情
    var loadProductDetail = function (skuid, groupid) {

        var _detailDic = { skuid: skuid, groupid: groupid, userid: pubuserid, openid: pubopenid };
        $.get(_Config.APIUrl + '/api/coupon/GetGroupSKUCouponActivityModel', _detailDic, function (data) {
            //console.log(data);
            if (data) {

                data.SKUID = parseInt(skuid);
                data.GROUPID = parseInt(groupid);
                data.UserId = pubuserid;
                data.CustomerType = customerType;
                data.SType = stype;
                data.IsVip = isvip;
                data.IsApp = isapp;
                data.productDefImg = "http://whfront.b0.upaiyun.com/app/img/pic-def-16x9.png";
                data.activity.OriMinBuyNum = data.activity.MinBuyNum;
                data.BuyStatus = {};
                data.loadNum = loadNum;
                data.loadNumList = loadNumList;

                //取出当前产品的单买SKU和拼团SKU
                data.SingleSKU = { Price: 0, VIPPrice: 0 };
                data.GroupSKU = { Price: 0, VIPPrice: 0 };
                if (data.SKUInfo && data.SKUInfo.SKUList && data.SKUInfo.SKUList.length > 1) {
                    for (var _skunum = 0; _skunum < data.SKUInfo.SKUList.length; _skunum++) {
                        var _skuobj = data.SKUInfo.SKUList[_skunum];
                        if (_skuobj.IsGroupSKU) {
                            data.GroupSKU = _skuobj;
                        }
                        else {
                            data.SingleSKU = _skuobj;
                        }
                    }
                }

                singleSku = data.SingleSKU;
                groupSku = data.GroupSKU;

                ////电话/地址显示
                //if (data.activity.DicProperties) {
                //    data.activity.telObj = {};
                //    data.activity.otherObjList = [];
                //    for (var _key in data.activity.DicProperties) {
                //        var _val = data.activity.DicProperties[_key];
                //        var _lab = _key.replace(":", "").replace("：", "");
                //        if (_lab.indexOf("电话") >= 0) {
                //            var _tel = _val;
                //            var _telex = "";
                //            if (_val.indexOf("转")>=0) {
                //                _tel = _val.split('转')[0];
                //                _telex = "转" + _val.split('转')[1];
                //            }

                //            data.activity.telObj = {
                //                "lab": _lab,
                //                "tel": _tel,
                //                "telex": _telex
                //            }
                //        }
                //        else {
                //            data.activity.otherObjList.push({
                //                "lab": _lab,
                //                "val": _val
                //            })
                //        }
                //    }
                //}

                //初始VIP专享状态（兼容老VIP和新VIP专享）
                if (!data.activity.IsVipExclusive && data.SKUInfo && data.SKUInfo.SKU && data.SKUInfo.SKU.TagsList && data.SKUInfo.SKU.TagsList.length > 0) {
                    for (var _tagNum = 0; _tagNum < data.SKUInfo.SKU.TagsList.length; _tagNum++) {
                        var _tagItem = data.SKUInfo.SKU.TagsList[_tagNum];
                        if (_tagItem.TagID == 1) {
                            data.activity.IsVipExclusive = true;
                            break;
                        }
                    }
                }

                //now time
                _nowTime = new Date(parseInt(year0), parseInt(month0), parseInt(day0), parseInt(hour0), parseInt(minute0), parseInt(second0));

                //大团时间配置
                var dtArr = {};
                var dayArr = {};
                var timeArr = {};

                data.activity["isOver"] = false;
                if (data.activity.EffectiveTime && data.activity.SaleEndDate) {

                    data.activity["sTit"] = "拼团结束";
                    data.activity["sD"] = "00";
                    data.activity["sH"] = "00";
                    data.activity["sM"] = "00";
                    data.activity["sS"] = "00";

                    dtArr = (data.activity.EffectiveTime).split("-");
                    dayArr = dtArr[2].split("T");
                    timeArr = dayArr[1].split(":");
                    data.activity["y1"] = dtArr[0];
                    data.activity["mo1"] = dtArr[1];
                    data.activity["d1"] = dayArr[0];
                    data.activity["h1"] = timeArr[0];
                    data.activity["mi1"] = timeArr[1];
                    data.activity["s1"] = timeArr[2];

                    dtArr = (data.activity.SaleEndDate).split("-");
                    dayArr = dtArr[2].split("T");
                    timeArr = dayArr[1].split(":");
                    data.activity["y2"] = dtArr[0];
                    data.activity["mo2"] = dtArr[1];
                    data.activity["d2"] = dayArr[0];
                    data.activity["h2"] = timeArr[0];
                    data.activity["mi2"] = timeArr[1];
                    data.activity["s2"] = timeArr[2];

                    //小团是否结束
                    var _gEndTime = new Date(parseInt(data.activity["y2"]), parseInt(data.activity["mo2"]), parseInt(data.activity["d2"]), parseInt(data.activity["h2"]), parseInt(data.activity["mi2"]), parseInt(data.activity["s2"]));
                    if (_gEndTime < _nowTime) { data.activity.isOver = true; }

                }

                //小团配置处理
                if (data.GroupPurchase) {

                    //参与人时间格式化
                    if (data.GroupPurchase.GroupPeople) {
                        data.GroupPurchase.GroupPeople.map(function (item, index) {
                            var _joinTime = item.JoinTime;
                            dtArr = (item.JoinTime).split("-");
                            dayArr = dtArr[2].split("T");
                            timeArr = dayArr[1].split(":");
                            item.JoinTime = dtArr[1] + "-" + dayArr[0] + " " + timeArr[0] + ":" + timeArr[1];
                        });
                    }

                    //小团时间配置
                    data.GroupPurchase["isOver"] = false;
                    if (data.GroupPurchase.CreatTime && data.GroupPurchase.EndTime) {

                        data.GroupPurchase["sTit"] = "拼团结束";
                        data.GroupPurchase["sD"] = "00";
                        data.GroupPurchase["sH"] = "00";
                        data.GroupPurchase["sM"] = "00";
                        data.GroupPurchase["sS"] = "00";

                        dtArr = (data.GroupPurchase.CreatTime).split("-");
                        dayArr = dtArr[2].split("T");
                        timeArr = dayArr[1].split(":");
                        data.GroupPurchase["y1"] = dtArr[0];
                        data.GroupPurchase["mo1"] = dtArr[1];
                        data.GroupPurchase["d1"] = dayArr[0];
                        data.GroupPurchase["h1"] = timeArr[0];
                        data.GroupPurchase["mi1"] = timeArr[1];
                        data.GroupPurchase["s1"] = timeArr[2];

                        //data.GroupPurchase.EndTime = "2017-05-19T15:35:40.00";

                        dtArr = (data.GroupPurchase.EndTime).split("-");
                        dayArr = dtArr[2].split("T");
                        timeArr = dayArr[1].split(":");
                        data.GroupPurchase["y2"] = dtArr[0];
                        data.GroupPurchase["mo2"] = dtArr[1];
                        data.GroupPurchase["d2"] = dayArr[0];
                        data.GroupPurchase["h2"] = timeArr[0];
                        data.GroupPurchase["mi2"] = timeArr[1];
                        data.GroupPurchase["s2"] = timeArr[2];

                        //小团是否结束
                        var _gEndTime = new Date(parseInt(data.GroupPurchase["y2"]), parseInt(data.GroupPurchase["mo2"]), parseInt(data.GroupPurchase["d2"]), parseInt(data.GroupPurchase["h2"]), parseInt(data.GroupPurchase["mi2"]), parseInt(data.GroupPurchase["s2"]));
                        if (_gEndTime < _nowTime) { data.GroupPurchase.isOver = true; }
                    }

                    //是否显示更多参与人
                    data.showMoreSku = true;
                    data.showMoreBtn = false;
                    if (data.GroupPurchase.GroupPeople.length > 4) {
                        data.showMoreSku = false;
                        data.showMoreBtn = true;
                    }
                }

                data.activity.CreatePeople = { NickName: "" };
                data.activity.GroupPeopleAllList = [];

                if (data.GroupPurchase && data.GroupPurchase.GroupPeople && data.GroupPurchase.GroupPeople.length) {

                    var groupMaxCount = data.activity.GroupCount;
                    if (data.GroupPurchase.GroupPeople.length > groupMaxCount) {
                        groupMaxCount = data.GroupPurchase.GroupPeople.length;
                    }

                    for (var _gnum = 0; _gnum < groupMaxCount; _gnum++) {
                        var _perpleObj = {};
                        if (_gnum < data.GroupPurchase.GroupPeople.length) {
                            _perpleObj = data.GroupPurchase.GroupPeople[_gnum];
                        }

                        if (_perpleObj.IsSponsor) {
                            data.activity.CreatePeople = _perpleObj;
                        }

                        data.activity.GroupPeopleAllList.push(_perpleObj);
                    }
                }

                //获取当前发起助力的专属二维码
                //GROUPTREE_SKUID_ACTIVEID_GROUPID_USERID
                var _sceneStr = "GROUPTREE_{0}_{1}_{2}_{3}".format(pubskuid, pubaid, origroupid, pubuserid);

                var _dic = {
                    weixinAcount: weixinAcountId, //周末酒店服务号 浩颐
                    expires: 2592000,
                    actionName: "QR_STR_SCENE",
                    sceneId: 0,
                    sceneStr: _sceneStr
                };

                //console.log(_dic);

                $.get(_Config.APIUrl + '/api/WeixinApi/CreateAccountQrcode', _dic, function (_data) {
                    //$.get('http://api.zmjiudian.com/api/WeixinApi/CreateAccountQrcode', _dic, function (_data) {

                    //console.log(_data);

                    if (_data && _data.indexOf("ticket") >= 0) {

                        var _dataObj = JSON.parse(_data);
                        //console.log(_dataObj);

                        var _src = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}".format(_dataObj.ticket);
                        //console.log(_src);

                        //将微信专属二维码上传并生成zmjd下的图片地址
                        $.get(_Config.APIUrl + '/api/WeixinApi/UploadWeixinQrcodeImg', { oriQrcodeImg: _src }, function (_qrcodeData) {

                            setTimeout(function () {

                                console.log(_qrcodeData);

                                //产品图片（ios微信环境下，如果图片之间加载过有缓存，不会执行load事件，所以这里统一加上时间戳，MMP 20180109 haoy）
                                var timestamp = Date.parse(new Date());
                                data.activity["productImgUrl"] = data.activity.PicList[0].replace('_appdetail1', '_640x426') + "?v=" + timestamp;

                                //二维码图片
                                //data.activity["posterQrcodeUrl"] = "http://whfront.b0.upaiyun.com/app/img/qrcode-wx-159x159.png";
                                data.activity["posterQrcodeUrl"] = _qrcodeData;

                                //海报底部logo
                                switch (weixinAcountId) {

                                    //周末酒店服务号
                                    case "7": {
                                        data.activity["posterFootLogoUrl"] = "http://whfront.b0.upaiyun.com/app/img/coupon/groupproductfortree/foot-zmjd-service-logo.png";
                                        break;
                                    }
                                        //周末酒店苏州服务号
                                    case "8": {
                                        data.activity["posterFootLogoUrl"] = "http://whfront.b0.upaiyun.com/app/img/coupon/groupproductfortree/foot-zmjd-service-sz-logo.png";
                                        break;
                                    }

                                }

                                productDetailData.productDetail = data;

                                setTimeout(function () {

                                    ////检查当前券状态
                                    //checkBuyStatus(productDetailData.productDetail.activity.ID);

                                    ////显示树的位置
                                    //refTreeImg();

                                    ////绑定浇水事件
                                    //$(".shuihu").click(shuihuWaterFun);

                                    ////【tip】点我助力
                                    //var _clickmeObj = $(".clickme-tip");
                                    //setTimeout(function () { _clickmeObj.fadeOut(); }, 60000);

                                    ////【tip】小团助力成功/撒花
                                    //var _groupokObj = $(".groupok-tip");
                                    //setTimeout(function () { _groupokObj.fadeOut(); }, 30000);

                                    ////【tip】快去邀请更多好友助力
                                    //var _sendfriendObj = $(".sendfriend-tip");
                                    //setTimeout(function () { _sendfriendObj.fadeOut(); }, 30000);

                                    ////【tip】好棒，帮好友助力完成
                                    //var _likeokObj = $(".likeok-tip");
                                    //setTimeout(function () { _likeokObj.fadeOut(); }, 30000);

                                }, 0);

                                console.log(productDetailData.productDetail)

                                Vue.nextTick(function () {

                                    $(".mine-poster .p-img img").load(function () {

                                        productImgLoaded = true;
                                        loadPoster()
                                    });

                                    $(".mine-poster .p-info .qrcode-section img").load(function () {

                                        qrcodeImgLoaded = true;
                                        loadPoster()
                                    });

                                    //$(".mine-poster .foot img").load(function () {

                                    //    footLogoImgLoaded = true;
                                    //    loadPoster()
                                    //});
                                })
                            });
                        });

                    }
                    else {

                        console.log("专属二维码生成失败");
                    }

                });
            }

        });

    }
    loadProductDetail(pubskuid, pubgroupid);

    //加载并显示海报
    var productImgLoaded = false;
    var qrcodeImgLoaded = false;
    var footLogoImgLoaded = true;
    var loadPoster = function () {

        if (productImgLoaded && qrcodeImgLoaded && footLogoImgLoaded) {

            $(".poster-bg").show();
            $(".mine-poster").show();
            html2canvas($(".mine-poster")[0], { useCORS: true }).then(function (canvas) {

                //console.log(canvas.toDataURL());

                $("#showImg").attr("src", canvas.toDataURL());
                $("#showImg").load(function () {

                    setTimeout(function () {
                        $("#showPosterSection").slideDown();
                        $(".poster-tip").fadeIn(500);
                        $(".mine-poster").hide();
                    }, 200);
                });

            });
        }
    }

    //检查当前券状态
    var checkBuyStatus = function (aid) {

        console.log("aid");
        console.log(aid);

        var _checkDic = { activityID: aid, userid: pubuserid };
        $.get(_Config.APIUrl + '/api/coupon/IsExchangeCouponSoldOut', _checkDic, function (checkData) {

            if (checkData) {

                //如果大团已经到时间了，同样表示大团结束
                if (productDetailData.productDetail.activity.isOver) {
                    checkData.ActivityState = 2;
                    //console.log("单独设置 checkData.ActivityState = 2");
                }

                productDetailData.productDetail.BuyStatus = checkData;
            }

            Vue.nextTick(function () {
                //绑定事件
                bindEvent();
            })
        });

    }

    //绑定事件
    var bindEvent = function () {

        //当前有GroupId & 大团未结束，但小团结束了，则同样可以发起
        //console.log(productDetailData.productDetail.BuyStatus.ActivityState)
        if (parseInt(pubgroupid) > 0 && productDetailData.productDetail.BuyStatus.ActivityState == 1 && productDetailData.productDetail.GroupPurchase && (productDetailData.productDetail.GroupPurchase.State == 2 || productDetailData.productDetail.GroupPurchase.isOver)) {
            //console.log("清团");
            pubgroupid = 0;
        }

        //动态加载头图
        if (productDetailData.productDetail.activity.PicList && productDetailData.productDetail.activity.PicList.length > 0) {

            if (productDetailData.productDetail.activity.PicList.length > 1) {

                //头图滑动
                //动态设置首页smallbanner的具体宽度
                if ($('#product-img-list-' + loadNum)) {

                    //console.log("有图")

                    $(".product-img-item").css("width", wwidth);
                    $('#product-img-list-' + loadNum).swiper({
                        slidesPerView: 'auto',
                        pagination: '.pagination-' + loadNum,
                        paginationHide: false,
                        loop: true,
                        offsetPxBefore: 0,
                        offsetPxAfter: 0
                    })
                }

                //显示产品图
                if ($(".def-photo")) $(".def-photo").hide();
                if ($(".product-photo")) $(".product-photo").fadeIn(500);

                var _productImgNum = 0;
                $(".product-img").each(function () {

                    var _thisImg = $(this);

                    setImgOriSrc(_thisImg);

                    //第一张图加载好以后显示
                    if (_productImgNum == 0) {
                        _thisImg.load(function () {


                        });
                    }

                    //设置产品图的点击预览功能（app内）
                    if (IsThanVer5_1 && productDetailData.productDetail.activity.Type != 200) {
                        _thisImg.click(previewImage);
                    }

                    _productImgNum++;
                });
            }
            else {

                var _singleImg = $(".product-single-img");
                //setImgOriSrc(_singleImg);
                //_singleImg.load(function () {

                //    //显示产品图
                //    if ($(".def-photo")) $(".def-photo").hide();
                //    if ($(".product-photo")) $(".product-photo").fadeIn(500);

                //});
                //显示产品图
                if ($(".def-photo")) $(".def-photo").hide();
                if ($(".product-photo")) $(".product-photo").fadeIn(500);
            }
        }
        
        if (!productDetailData.productDetail.activity.isOver
            && (parseInt(pubgroupid) <= 0 || (productDetailData.productDetail.BuyStatus.ActivityState == 1 && productDetailData.productDetail.GroupPurchase && (productDetailData.productDetail.GroupPurchase.State == 2 || productDetailData.productDetail.GroupPurchase.isOver)))) {

            //大团倒计时开启
            startActicityTimer();
        }
        else if (parseInt(pubgroupid) > 0 ) {

            //小团倒计时开启
            if (!productDetailData.productDetail.GroupPurchase.isOver
                && productDetailData.productDetail.GroupPurchase
                && (productDetailData.productDetail.GroupPurchase.State == 0 || productDetailData.productDetail.GroupPurchase.State == 1)) {
                startGroupTimer();
            }
        }

        //如果是房券的产品，则点击图片进入酒店详情页
        if (productDetailData.productDetail.activity.Type == 200) {
            $(".photo").click(function () {
                goto('hotel/' + productDetailData.productDetail.activity.HotelID);
            });
            $(".open-hotel").click(function () {
                goto('hotel/' + productDetailData.productDetail.activity.HotelID);
            });
        }

        //发起拼团
        $(".open-buy").click(function () {

            var loginapphref = "whotelapp://loadJS?url=javascript:loginCallbackFroOpenBuy('{userid}')&realuserid=1";
            if (pub_userid == "0") {
                //app环境下，如果没有登录则弹出登录
                if (isapp) {
                    location.href = loginapphref;
                    return;
                }
                else {
                    _loginModular.show();
                }
            }
            else {

                openBuySubmit($("#userid").val());
            }

        });

        //单独购买
        $(".single-buy").click(function () {
            
            var loginapphref = "whotelapp://loadJS?url=javascript:loginCallbackFroSingleBuy('{userid}')&realuserid=1";
            if (pub_userid == "0") {
                //app环境下，如果没有登录则弹出登录
                if (isapp) {
                    location.href = loginapphref;
                    return;
                }
                else {
                    _loginModular.show();
                }
            }
            else {

                gosubmit($("#userid").val());
            }
        });

        //邀请好友
        $(".btn-share").click(function () {

            if (inWeixin) {
                $(".group-share-tip").show();
            }
            else if (isapp) {
                gourl(appShareLink);
            }
            else {
                alert("请到微信或周末酒店APP中进行分享哦～")
            }

        });

        //查看更多套餐
        $(".show-more").click(function () {
            productDetailData.productDetail.showMoreSku = true;
            productDetailData.productDetail.showMoreBtn = false;
        });
    }

    //延时加载活动说明
    setTimeout(function () {

        //细则图片动态加载
        var ruleImg = $(".group-rule img");
        setImgOriSrc(ruleImg);

        //分享提示图片动态加载
        var shareTipImg = $(".group-share-tip img");
        setImgOriSrc(shareTipImg);

    }, 800);

    //打开活动说明
    $(".rules").click(function () {
        $(".group-rule-close").show();
        $(".group-rule").show();
    });

    //关闭活动说明
    $(".group-rule-close").click(function () {
        $(".group-rule-close").hide();
        $(".group-rule").hide();
    });

    //右上角分享提示点击事件
    $(".group-share-tip").click(function () {
        $(this).hide();
    });

    //开启大团倒计时
    var startActicityTimer = function () {
        runTimer(1)
    }

    //开启小团倒计时
    var startGroupTimer = function () {
        runTimer(2)
    }

    //type:1大团 2小团
    var runTimer = function (type) {

        //run timer
        var timerTags = $(".timer-tag");

        switch (type) {
            case 1: { timerTags = $(".activity-timer"); break; }
            case 2: { timerTags = $(".group-timer"); break; }
        }

        //console.log("开始倒计时：" + type)
        //return;

        if (timerTags) {
            for (var i = 0; i < timerTags.length; i++) {

                timeDic[i] = {
                    timerEntity: null,
                    nowTime: null,
                    endDate: null,
                    closeDate: null,
                    endTimerState: true,
                    closeTimerState: false,
                    initNowtime: function () {
                        this.nowTime = new Date(
                        parseInt(year0)
                        , parseInt(month0)
                        , parseInt(day0)
                        , parseInt(hour0)
                        , parseInt(minute0)
                        , parseInt(second0)
                            ).getTime();
                    },
                    initEndtime: function () {
                        this.endDate = new Date(
                        parseInt(this.timerEntity.data("year1"))
                        , parseInt(this.timerEntity.data("month1"))
                        , parseInt(this.timerEntity.data("day1"))
                        , parseInt(this.timerEntity.data("hour1"))
                        , parseInt(this.timerEntity.data("minute1"))
                        , parseInt(this.timerEntity.data("second1"))
                            ).getTime();
                    },
                    initClosetime: function () {
                        this.closeDate = new Date(
                        parseInt(this.timerEntity.data("year2"))
                        , parseInt(this.timerEntity.data("month2"))
                        , parseInt(this.timerEntity.data("day2"))
                        , parseInt(this.timerEntity.data("hour2"))
                        , parseInt(this.timerEntity.data("minute2"))
                        , parseInt(this.timerEntity.data("second2"))
                            ).getTime();
                    },
                    init: function () {
                        this.initNowtime();
                        this.initEndtime();
                        this.initClosetime();
                    },
                    timerAction: function () {
                        if (this.endTimerState) {
                            var t = this.endDate - this.nowTime;
                            var d = Math.floor(t / (1000 * 60 * 60 * 24));
                            var h = Math.floor(t / 1000 / 60 / 60 % 24) + (d * 24);
                            var m = Math.floor(t / 1000 / 60 % 60);
                            var s = Math.floor(t / 1000 % 60);

                            switch (type) {
                                case 1: {
                                    productDetailData.productDetail.activity["sTit"] = "尚未开始";
                                    productDetailData.productDetail.activity["sD"] = d;
                                    productDetailData.productDetail.activity["sH"] = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                                    productDetailData.productDetail.activity["sM"] = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                    productDetailData.productDetail.activity["sS"] = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                    break;
                                }
                                case 2: {
                                    productDetailData.productDetail.GroupPurchase["sTit"] = "尚未开始";
                                    productDetailData.productDetail.GroupPurchase["sD"] = d;
                                    productDetailData.productDetail.GroupPurchase["sH"] = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                                    productDetailData.productDetail.GroupPurchase["sM"] = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                    productDetailData.productDetail.GroupPurchase["sS"] = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                    break;
                                }
                            }

                            //var timehtml = h <= 0
                            //    ? "距离开抢还有 00:" + (m < 10 ? "0" + m : m) + ":" + (s < 10 ? "0" + s : s)
                            //    : "距离开抢还有" + (h < 10 ? "0" + h : h) + ":" + (m < 10 ? "0" + m : m) + ":" + (s < 10 ? "0" + s : s);

                            //this.timerEntity.html(timehtml);

                            try {

                                if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                                    this.stopEndAction();
                                }

                            } catch (e) { }

                            this.nowTime = this.nowTime + 1000;
                        }
                    },
                    timerCloseAction: function () {
                        if (this.closeTimerState) {
                            var t = this.closeDate - this.nowTime;
                            var d = Math.floor(t / (1000 * 60 * 60 * 24));
                            var h = Math.floor(t / 1000 / 60 / 60 % 24) + (d * 24);
                            var m = Math.floor(t / 1000 / 60 % 60);
                            var s = Math.floor(t / 1000 % 60);

                            switch (type) {
                                case 1: {
                                    productDetailData.productDetail.activity["sTit"] = "开团时间仅剩";
                                    productDetailData.productDetail.activity["sD"] = d;
                                    productDetailData.productDetail.activity["sH"] = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                                    productDetailData.productDetail.activity["sM"] = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                    productDetailData.productDetail.activity["sS"] = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                    break;
                                }
                                case 2: {
                                    productDetailData.productDetail.GroupPurchase["sTit"] = "活动结束";
                                    productDetailData.productDetail.GroupPurchase["sD"] = d;
                                    productDetailData.productDetail.GroupPurchase["sH"] = h < 0 ? "00" : (h < 10 ? "0" + h : "" + h);
                                    productDetailData.productDetail.GroupPurchase["sM"] = m < 0 ? "00" : (m < 10 ? "0" + m : "" + m);
                                    productDetailData.productDetail.GroupPurchase["sS"] = s < 0 ? "00" : (s < 10 ? "0" + s : "" + s);
                                    break;
                                }
                            }

                            //var timehtml = d <= 0 ?
                            //    (h <= 0 ? "还有" + (m < 10 ? "0" + m : m) + "分钟结束"
                            //    : "还有" + (h < 10 ? "0" + h : h) + "小时结束")
                            //    : "还有" + (d < 10 ? "" + d : d) + "天结束";

                            //this.timerEntity.html(timehtml);

                            try {

                                if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                                    this.stopCloseAction();
                                }

                            } catch (e) { }

                            this.nowTime = this.nowTime + 1000;
                        }
                    },
                    stopEndAction: function () {
                        this.endTimerState = false;
                        this.closeTimerState = true;

                        switch (type) {
                            case 1: {
                                productDetailData.productDetail.activity["sTit"] = "开团时间仅剩";
                                break;
                            }
                            case 2: {
                                productDetailData.productDetail.GroupPurchase["sTit"] = "活动仅剩";
                                break;
                            }
                        }

                        //this.timerEntity.html("进行中");
                    },
                    stopCloseAction: function () {
                        this.closeTimerState = false;

                        switch (type) {
                            case 1: {
                                location.reload();

                                productDetailData.productDetail.activity["sTit"] = "活动结束";
                                productDetailData.productDetail.activity["sD"] = "00";
                                productDetailData.productDetail.activity["sH"] = "00";
                                productDetailData.productDetail.activity["sM"] = "00";
                                productDetailData.productDetail.activity["sS"] = "00";
                                break;
                            }
                            case 2: {

                                location.reload();

                                productDetailData.productDetail.GroupPurchase["sTit"] = "拼团结束";
                                productDetailData.productDetail.GroupPurchase["sD"] = "00";
                                productDetailData.productDetail.GroupPurchase["sH"] = "00";
                                productDetailData.productDetail.GroupPurchase["sM"] = "00";
                                productDetailData.productDetail.GroupPurchase["sS"] = "00";
                                break;
                            }
                        }

                        //this.timerEntity.html("已结束");
                    }
                };

                //build
                timeDic[i].timerEntity = $(timerTags[i]);

                //init
                timeDic[i].init();

                //start
                timeDic[i].timerAction();
                setInterval("gotime(timeDic[" + i + "])", 1000);
            }
        }

    }

    function showTip(mes) {
        //console.log(mes)
        $(".pubAlertTip .tipinfo").html(mes);
        $(".pubAlertTip").fadeIn(500);
        setTimeout(function () {
            $(".pubAlertTip").fadeOut(300);
        }, 3000);
    }
});

//单独购买原生登录回调
function loginCallbackFroSingleBuy(userid) {
    gosubmit(userid);
}

//单独购买
var gosubmit = function (userid) {

    var subdic = {};
    subdic["aid"] = $("#aid").val();
    subdic["atype"] = $("#atype").val();
    subdic["skuid"] = singleSku.ID;
    subdic["paynum"] = 1;
    subdic["userid"] = userid;
    subdic["stype"] = stype;

    console.log("single buy")
    console.log(subdic);
    //return;

    $.get('/Coupon/SubmitConponForProduct', subdic, function (content) {
        var msg = content.Message;
        var suc = content.Success;
        var url = content.Url;

        switch (suc) {
            case "0":
                {
                    location = url;
                    break;
                }
            case "1":
                {
                    alert(msg);
                    break;
                }
            case "2":
                {
                    _Modal.show({
                        title: '',
                        content: msg,
                        confirmText: '成为VIP',
                        confirm: function () {
                            goBuyVip();
                            _Modal.hide();
                        },
                        showCancel: true,
                        cancel: function () {
                            _Modal.hide();
                        }
                    });
                    break;
                }
        }
    });
};

//发起/参与拼团原生登录回调
function loginCallbackFroOpenBuy(userid) {
    openBuySubmit(userid);
}

//发起/参与拼团
var openBuySubmit = function (userid) {

    var subdic = {};
    subdic["aid"] = $("#aid").val();
    subdic["atype"] = $("#atype").val();
    subdic["skuid"] = groupSku.ID;
    subdic["paynum"] = 1;
    subdic["userid"] = userid;
    subdic["stype"] = stype;
    subdic["groupId"] = pubgroupid;

    //console.log("提交拼团")
    //console.log(subdic)
    //return

    $.get('/Coupon/SubmitConponForGroupProduct', subdic, function (content) {
        var msg = content.Message;
        var suc = content.Success;
        var url = content.Url;

        switch (suc) {
            case "0":
                {
                    location = url;
                    break;
                }
            case "1":
                {
                    alert(msg);
                    break;
                }
            case "2":
                {
                    _Modal.show({
                        title: '',
                        content: msg,
                        confirmText: '成为VIP',
                        confirm: function () {
                            goBuyVip();
                            _Modal.hide();
                        },
                        showCancel: true,
                        cancel: function () {
                            _Modal.hide();
                        }
                    });
                    break;
                }
            case "3":
                {
                    _Modal.show({
                        title: '',
                        content: msg,
                        confirmText: '去发起拼团',
                        confirm: function () {
                            goto("/coupon/group/product/" + subdic["skuid"] + "/0?userid=" + subdic["userid"]);
                            _Modal.hide();
                        },
                        showCancel: true,
                        cancel: function () {
                            _Modal.hide();
                        }
                    });
                    break;
                }
        }
    });
}

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        var defsrc = imgObj.attr("src"); //console.log(orisrc)
        imgObj.attr("src", orisrc);
        //imgObj.data("orisrc", "");
        imgObj.error(function () {
            imgObj.attr("src", defsrc);
        });
    }
};

function goto(param) {
    var isapp = $("#isapp").val() == "1";
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (!isapp) {
        url = "http://www.shangjiudian.com/" + param;
    }

    this.location = url;
}

function gourl(url) {
    location.href = url;
}

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': $("#_name").val(), 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?userid=" + pubuserid + "&_isoneoff=1&_newpage=1";
}

var timeDic = [];
function gotime(timeObj) {
    timeObj.timerAction();
    timeObj.timerCloseAction();
}

//照片预览功能
var previewImage = function () {

    var _thisImg = $(this);
    var _param = { index: 0, urls: [] };
    _param.index = _thisImg.data("num");

    //get urls
    var _allImgs = $(".product-img");
    _allImgs.each(function () {
        var _oriSrc = $(this).data("showsrc").replace("_640x426", "_jupiter");
        _param.urls.push(_oriSrc);
    });

    zmjd.previewImage(_param);
}
var previewImageSuccess = function () {
    console.log("预览成功")
}
var previewImageFail = function () {
    console.log("预览失败")
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    

}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //alert(apptype)

    //init data
    _InitApp(userid, apptype, appvercode, appverno);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}