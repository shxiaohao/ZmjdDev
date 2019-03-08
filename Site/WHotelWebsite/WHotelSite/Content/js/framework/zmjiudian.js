var GetRequest = function () {
    var url = location.search;
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = decodeURI(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
var $P = GetRequest();

//init app info
var ZmjdUserId = 0;
var AppType = "";
var AppVerCode = "0";
var AppVerNo = "4.2";
var LocalLat = 0;
var LocalLng = 0;
var IsApp = false;
var IsThanVer4_4 = false;
var IsThanVer4_6 = false;
var IsThanVer4_6_1 = false;
var IsThanVer4_7 = false;
var IsThanVer4_8 = false;
var IsThanVer5_0 = false;
var IsThanVer5_1 = false;
var IsThanVer5_2 = false;
var IsThanVer5_3 = false;
var IsThanVer5_4 = false; 
var IsThanVer5_6 = false;
var IsThanVer5_6_1 = false;
var IsThanVer5_6_2 = false;
var IsThanVer5_7 = false;
var IsThanVer5_8 = false;
var IsThanVer5_8_1 = false;
var IsThanVer6_3 = false;
var IsIos = false;
var IsAndroid = false;
var IsInWeixin = false;
var StopFlexslider = false;
var StopFlexsliderY = false;
var _InitApp = function (userid, apptype, appvercode, appverno, locallat, locallng) {

    ZmjdUserId = userid != undefined ? parseInt(userid) : 0;
    AppType = apptype;
    AppVerCode = appvercode;
    AppVerNo = appverno;
    LocalLat = locallat;
    LocalLng = locallng;

    IsApp = AppType && AppType != "" && AppType != null && AppType != undefined;
    if (IsApp) {

        var _code = parseInt(AppVerCode);

        //app ver
        switch (AppType) {
            case "iOSApp": {
                if (_code >= 338) IsThanVer5_8_1 = true;
                if (_code >= 336) IsThanVer5_8 = true;
                if (_code >= 330) IsThanVer5_7 = true;
                if (_code >= 325) IsThanVer5_6_2 = true;
                if (_code >= 323) IsThanVer5_6_1 = true;
                if (_code >= 322) IsThanVer5_6 = true;
                if (_code >= 320) IsThanVer5_4 = true;
                if (_code >= 314) IsThanVer5_3 = true;
                if (_code >= 306) IsThanVer5_2 = true;
                if (_code >= 302) IsThanVer5_1 = true;
                if (_code >= 301) IsThanVer5_0 = true;
                if (_code >= 300) IsThanVer4_8 = true;
                if (_code >= 284) IsThanVer4_7 = true;
                if (_code >= 263) IsThanVer4_6_1 = true;
                if (_code >= 260) IsThanVer4_6 = true;
                if (_code >= 232) IsThanVer4_4 = true;
                if (_code >= 387) IsThanVer6_3 = true;
                IsIos = true;
                break;
            }
            case "android": {
                if (_code >= 71) IsThanVer5_8_1 = true;
                if (_code >= 70) IsThanVer5_8 = true;
                if (_code >= 69) IsThanVer5_7 = true;
                if (_code >= 68) IsThanVer5_6_2 = true;
                if (_code >= 67) IsThanVer5_6 = true;
                if (_code >= 62) IsThanVer5_2 = true;
                if (_code >= 61) IsThanVer5_1 = true;
                if (_code >= 60) IsThanVer5_0 = true;
                if (_code >= 59) IsThanVer4_8 = true;
                if (_code >= 58) IsThanVer4_7 = true;
                if (_code >= 51) IsThanVer4_6_1 = true;
                if (_code >= 50) IsThanVer4_6 = true;
                if (_code >= 46) IsThanVer4_4 = true;
                if (_code >= 83) IsThanVer6_3 = true;
                IsAndroid = true;
                break;
            }
        }
    }

    var inweixin = $P["inweixin"];
    IsInWeixin = inweixin != null && inweixin != "" && inweixin != undefined && inweixin == "1";

    var _newtitle = $P["_newtitle"];
    if (_newtitle && _newtitle == "1") {
        //document.body.style.marginTop = '22px';
    }
}

String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

function formatDate(date, format) {
    if (!date) return;
    if (!format) format = "yyyy-MM-dd";
    switch (typeof date) {
        case "string":
            date = new Date(date.replace(/-/g, "/"));
            break;
        case "number":
            date = new Date(date);
            break;
    }
    if (!date instanceof Date) return;
    var dict = {
        "yyyy": date.getFullYear(),
        "M": date.getMonth() + 1,
        "d": date.getDate(),
        "H": date.getHours(),
        "m": date.getMinutes(),
        "s": date.getSeconds(),
        "MM": ("" + (date.getMonth() + 101)).substr(1),
        "dd": ("" + (date.getDate() + 100)).substr(1),
        "HH": ("" + (date.getHours() + 100)).substr(1),
        "mm": ("" + (date.getMinutes() + 100)).substr(1),
        "ss": ("" + (date.getSeconds() + 100)).substr(1)
    };

    return format.replace(/(yyyy|MM?|dd?|HH?|ss?|mm?)/g, function () {
        return dict[arguments[0]];
    });
}

Array.prototype.baoremove = function (dx) {
    if (isNaN(dx) || dx > this.length) { return false; }
    this.splice(dx, 1);
}

String.prototype.AllTrim = function () {
    return this.replace(/\s/g, "");
}
String.prototype.Trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}
String.prototype.LTrim = function () {
    return this.replace(/(^\s*)/g, "");
}
String.prototype.RTrim = function () {
    return this.replace(/(\s*$)/g, "");
}

var removeIndex = function(arr, obj) {
    for (var i = 0; i < arr.length; i++) {
        var temp = arr[i];
        if (!isNaN(obj)) {
            temp = i;
        }
        if (temp == obj) {
            for (var j = i; j < arr.length; j++) {
                arr[j] = arr[j + 1];
            }
            arr.length = arr.length - 1;
        }
    }
}

var plusXing = function (str, frontLen, endLen) {
    var len = str.length - frontLen - endLen;
    var xing = '';
    for (var i = 0; i < len; i++) {
        xing += '*';
    }
    return str.substr(0, frontLen) + xing + str.substr(str.length - endLen);
}

var returnFloat = function(value) {
    var value = Math.round(parseFloat(value) * 100) / 100;
    var xsd = value.toString().split(".");
    if (xsd.length == 1) {
        value = value.toString();// + ".00";
        return value;
    }
    if (xsd.length > 1) {
        if (xsd[1].length < 2) {
            value = value.toString();
        }
        return value;
    }
}

//移动终端浏览器版本信息
var B = {
    v: function () {
        var u = navigator.userAgent, app = navigator.appVersion;
        return {
            trident: u.indexOf('Trident') > -1,    //IE内核
            presto: u.indexOf('Presto') > -1,	   //opera内核
            webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
            gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
            mobile: !!u.match(/AppleWebKit.*Mobile.*/), 		//是否为移动终端
            ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), 	//ios终端
            android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或uc浏览器
            iPhone: u.indexOf('iPhone') > -1, 	//是否为iPhone或者QQHD浏览器
            iPad: u.indexOf('iPad') > -1, 		//是否iPad
            webApp: u.indexOf('Safari') == -1, 	//是否web应该程序，没有头部与底部
            weixin: u.toLowerCase().match(/MicroMessenger/i) == "micromessenger"	//是否微信
        };
    }(),
    language: (navigator.browserLanguage || navigator.language).toLowerCase()
}

var reloadPage = function (_) {
    if (B.v.ios) {
        location.reload(_);
    }
    else {
        if (location.href.indexOf('?') >= 0) {
            location.href = location.href + "&v=1";
        }
        else {
            location.href = location.href + "?v=1";
        }
    }
}

var gourl = function (url) {
    if (url) {
        location.href = url;
    }
}

var Store = {
    Get: function (key) {
        var obj;
        try {
            if (window.localStorage) {
                obj = localStorage.getItem(key);
                if (obj) {
                    obj = JSON.parse(obj);
                }
            }
        } catch (e) { }
        return obj;
    },
    Set: function (key, obj) {
        try {
            if (window.localStorage) {
                obj = JSON.stringify(obj);
                localStorage.setItem(key, obj);
            }
        } catch (e) { }
    },
    Remove: function (key) {
        try {
            if (window.localStorage) {
                localStorage.removeItem(key);
            }
        } catch (e) { }
    }
}

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        var defsrc = imgObj.attr("src");
        imgObj.attr("src", orisrc);
        imgObj.data("orisrc", "");
        imgObj.error(function () {
            imgObj.attr("src", defsrc);
        });
    }
};

//根据评分获取对应的iconfont文本
var getScoreHtml = function (hotelScore) {
    if (hotelScore) {
        var des = "";
        if (hotelScore < 0.5) {
            des = "&#xe607;&#xe607;&#xe607;&#xe607;&#xe607;";
        } else if (hotelScore < 1) {
            des = "&#xe606;&#xe607;&#xe607;&#xe607;&#xe607;";
        } else if (hotelScore < 1.5) {
            des = "&#xe605;&#xe607;&#xe607;&#xe607;&#xe607;";
        } else if (hotelScore < 2) {
            des = "&#xe605;&#xe606;&#xe607;&#xe607;&#xe607;";
        } else if (hotelScore < 2.5) {
            des = "&#xe605;&#xe605;&#xe607;&#xe607;&#xe607;";
        } else if (hotelScore < 3) {
            des = "&#xe605;&#xe605;&#xe606;&#xe607;&#xe607;";
        } else if (hotelScore < 3.5) {
            des = "&#xe605;&#xe605;&#xe605;&#xe607;&#xe607;";
        } else if (hotelScore < 4) {
            des = "&#xe605;&#xe605;&#xe605;&#xe606;&#xe607;";
        } else if (hotelScore < 4.5) {
            des = "&#xe605;&#xe605;&#xe605;&#xe605;&#xe607;";
        } else if (hotelScore < 5) {
            des = "&#xe605;&#xe605;&#xe605;&#xe605;&#xe606;";
        } else {
            des = "&#xe605;&#xe605;&#xe605;&#xe605;&#xe605;";
        }
        return des;
    } else {
        return ""
    }
}

////zmjd config object
//var Config = function () {
//    this.APIUrl = "http://api.zmjiudian.com";
//    //this.APIUrl = "http://192.168.1.115";
//    //this.APIUrl = "http://api.zmjd100.com";//"http://192.168.1.114:8000";//"http://192.168.1.113:8000";//
//}

var $$ = {
    Get: function (url, param, callback) {
        var self = this;
        self.request(false, url, param, callback);
    },
    Post: function (url, param, callback) {
        var self = this;
        self.request(true, url, param, callback);
    },
    request: function (ispost, url, param, callback) {

        if (window.localStorage) {

            //对浮点的参数，处理保留小数点后两位（更多的考虑是坐标参数，小数点后位数太多浮动太大）
            for (var _item in param) {
                try {
                    var _val = param[_item];
                    if (!isNaN(_val) && _val.indexOf('.') >= 0) {
                        param[_item] = parseFloat(_val.substring(0, _val.indexOf('.') + 3));
                    }
                } catch (e) { }
            }

            var _datakey = url.replace(/http:\/\//g, "").replace(/\//g, "") + "_" + JSON.stringify(param).replace(/"/g, "").replace(/:/g, "").replace(/,/g, "").replace(/{/g, "").replace(/}/g, "");
            var _timestampkey = _datakey + "_timestamp";

            var _data = Store.Get(_datakey);
            var _timestamp = Store.Get(_timestampkey);
            var _now = new Date();

            if (!_data || _data == null || _data == undefined) {

                if (ispost) {
                    $.post(url, param, function (_result) {
                        if (_result) {
                            callback(_result);
                            Store.Set(_datakey, _result);
                            Store.Set(_timestampkey, new Date().getTime());
                        }
                    });
                }
                else {
                    $.get(url, param, function (_result) {
                        if (_result) {
                            callback(_result);
                            Store.Set(_datakey, _result);
                            Store.Set(_timestampkey, new Date().getTime());
                        }
                    });
                }
            }
            else {

                //return cache
                callback(_data);

                //ref cache
                var _timeDiff = (_now.getTime() - _timestamp) / 1000;	//秒
                if (_timeDiff >= 180) {

                    setTimeout(function () {
                        if (ispost) {
                            $.post(url, param, function (_result) {
                                if (_result) {
                                    Store.Set(_datakey, _result);
                                    Store.Set(_timestampkey, new Date().getTime());
                                }
                            });
                        }
                        else {
                            $.get(url, param, function (_result) {
                                if (_result) {
                                    Store.Set(_datakey, _result);
                                    Store.Set(_timestampkey, new Date().getTime());
                                }
                            });
                        }
                    }, 0);
                }
            }
        }
        else {

            if (ispost) $.post(url, param, callback);
            else $.get(url, param, callback);
        }
    }
}

//h5与原生的交互API
var zmjd = {
    //复制功能
    copyTxt: function (txt) {
        if (IsIos) {

            if (IsThanVer5_7) {
                var _data = {
                    "method": "copyTxt",
                    "param": txt,
                    "success": "copyTxtSuccess",
                    "fail": "copyTxtFail",

                };
                this.iosBaseHandler(_data);
            }
            else {
                var _u = 'whotelapp://runFunc?data={"method":"copyTxt","param":"' + txt + '","success":"copyTxtSuccess","fail":"copyTxtFail"}';
                location.href = _u;
            }

        }
        else {
            whotel.copyTxt(txt, "copyTxtSuccess", "copyTxtFail");
        }
    },
    //设置原生分享配置
    //param 参数格式  {title:"",content:"",photoUrl:"",shareLink:"",shareMenu:"WechatSession,WechatTimeline,WechatFavorite,QQ,Qzone,Sina,Paste"/"All",useMiniApp:true,miniAppId:"MiniAppId_zmjdlite",miniAppShareLink:""}
    setShareConfig: function (param) {
        if (IsIos) {
            var _data = {
                "method": "setShareConfig",
                "param": param,
                "success": "setShareConfigSuccess",
                "fail": "setShareConfigFail"
            };
            this.iosBaseHandler(_data);

        }
        else {
            var _paramStr = JSON.stringify(param);
            whotel.setShareConfig(_paramStr, "setShareConfigSuccess", "setShareConfigFail");
        }
    },
    //图片预览
    previewImage: function (param) {
        var paramStr = JSON.stringify(param); //console.log(paramStr)
        if (IsIos) {
            if (IsThanVer5_7) {
                var _data = {
                    "method": "previewImage",
                    "param": param,
                    "success": "previewImageSuccess",
                    "fail": "previewImageFail",
                };
                this.iosBaseHandler(_data);
            }
            else {
                var _u = 'whotelapp://runFunc?data={"method":"previewImage","param":' + paramStr + ',"success":"previewImageSuccess","fail":"previewImageFail"}';
                //console.log(_u)
                location.href = _u;
            }
        }
        else {
            whotel.previewImage(paramStr, "previewImageSuccess", "previewImageFail")
        }
    },
    //显示内容输入框
    showInput: function (param) {
        /*
		例子
		whotelapp://runFunc?data={"method":"showInput","param":{"mode":1,"placeholder":"回复小明：","maxlength":100},"success":"showInputSuccess","fail":"showInputFail"}
		whotel.showInput({"mode":1,"placeholder":"回复小明：","maxlength":100}, "showInputSuccess", "showInputFail")
		*/
        var paramStr = JSON.stringify(param); //console.log(paramStr)
        if (IsIos) {
            var _u = 'whotelapp://runFunc?data={"method":"showInput","param":' + paramStr + ',"success":"showInputSuccess","fail":"showInputFail"}';
            location.href = _u;
        }
        else {
            whotel.showInput(paramStr, "showInputSuccess", "showInputFail")
        }
    },
    //给APP标记用户信息发生了变更
    userinfoChanged: function (param) {
        if (IsIos) {
            var _data = {
                method: "userinfoChanged",
                param: param,
                success: "userinfoChangedSuccess",
                fail: "userinfoChangedFail"
            };
            this.iosBaseHandler(_data);
        }
        else {
            whotel.userinfoChanged("", "userinfoChangedSuccess", "userinfoChangedFail")
        }
    },
    //跳转到原生的指定支付方式进行支付
    whotelAppPay: function (param) {
        if (IsIos) {
            var _data = {
                "method": "getWhotelPayData",
                "param": param,
                "success": "getWhotelPayDataSuccesss",
                "fail": "getWhotelPayDataFail",

            };
            this.iosBaseHandler(_data);
        }
        else {
            var paramStr = JSON.stringify(param); //console.log(paramStr)
            whotel.getWhotelPayData(paramStr, "getWhotelPayDataSuccesss", "getWhotelPayDataFail");
        }
    },
    //获取Android Chinapay SDK 返回的Android机支持的支付方式，如小米Pay， 华为Pay 
    whotelGetAndroidSupportedPayMode: function () {
        if (IsAndroid) { 
            whotel.getPayMode("", "getAndroidSupportedPayModeSuccess","getAndroidSupportedPayModeFail" );
        } 
    },
    //返回上一页并刷新
    pageBack: function (param) {
        if (IsIos) {
            var _data = {
                "method": "pageBack",
                "param": param,
                "success": "pageBackSuccesss",
                "fail": "pageBackFail",

            };
            this.iosBaseHandler(_data);
        }
        else {
            var paramStr = JSON.stringify(param); //console.log(paramStr)
            whotel.pageBack(paramStr, "pageBackSuccesss", "pageBackFail");
        }
    },
    //打开地图
    openLocation: function (param) {
        if (IsIos) {
            var _data = {
                "method": "openLocation",
                "param": param,
                "success": "openLocationSuccesss",
                "fail": "openLocationFail",
            };
            this.iosBaseHandler(_data);
        }
        else {
            var paramStr = JSON.stringify(param); //console.log(paramStr)
            whotel.openLocation(paramStr, "openLocationSuccesss", "openLocationFail");
        }
    },
    //ios的基础处理函数
    iosBaseHandler: function (data) {
        window.webkit.messageHandlers.whotel.postMessage(data);
    }
}

var Global = {

    //监听器
    Monito: {
        ListenerTimers:[],
        Publisher: function (controller, action, mode) {
            var _key = controller + "_" + action + ":" + mode;
            Store.Set(_key, 1);
        },
        Listener: function (controller, action, mode, timeout) {

            var _key = controller + "_" + action + ":" + mode; 
            if (!timeout || timeout == undefined || timeout == 'undefined') timeout = 1000;
            this.ListenerTimers[_key] = setInterval(function () {

                var _get = Store.Get(_key);
                if (_get && (_get == "1" || _get == 1)) {

                    switch (mode) {
                        //刷新
                        case 1: {
                            Store.Remove(_key);
                            location.reload();
                            break;
                        }
                        //..
                        case 2: {
                            break;
                        }
                    }

                }

            }, timeout);
        }
    }
    ,
    //Url推荐器
    UrlReferrer: {
        Key: "UrlReferrerKey",
        Set: function (_set) {
            //{'name':'','url':'','imgsrc':''}
            this.Clear();
            Store.Set(this.Key, _set);
        },
        Get: function () {
            var _get = Store.Get(this.Key);
            if (!_get || !_get.name || !_get.url) {
                _get = { 'name': '', 'url': '', 'imgsrc': '' };
            }
            return _get;
        },
        Clear: function () {
            Store.Remove(this.Key);
        },
        Check: function (sFun, fFun) {
            var _get = this.Get();
            if (_get.name && _get.url) {
                sFun(_get);
            }
            else {
                fFun(_get);
            }
        }
    }
}