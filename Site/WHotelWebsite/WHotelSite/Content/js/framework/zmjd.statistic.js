﻿var _statistic = {
    entity: {
        SourceKey: "",
        Page: "",
        Action: "",
        ActionValue: "",
        Element: "",
        Url: "",
        RefererUrl: "",
        PageTitle: "",
        SessionId: "",
        IP: "",
        Browser: "",
        DeviceHeight: 0,
        DeviceWidth: 0,
        DeviceId: "",
        AppVer: "",
        OS: "",
        Language: "",
        UserID: 0,
        Data: ""
    },
    init: function () {

        this.entity.Url = location.href;
        this.entity.RefererUrl = document.referrer;
        this.entity.PageTitle = document.title;
        this.entity.SessionId = this.sessionid();
        this.entity.IP = "";
        this.entity.Browser = navigator.userAgent;
        this.entity.DeviceHeight = window.screen.height;
        this.entity.DeviceWidth = window.screen.width;
        this.entity.DeviceId = this.deviceid();
        this.entity.AppVer = "";
        this.entity.OS = this.os();
        this.entity.Language = this.broinfo.language;
        this.entity.UserID = this.userid();
        this.entity.Data = "";

    },
    userid: function () {
        var _userid = _statistic.$p()["userid"];
        if (!_userid) {
            _userid = 0;
        }
        return _userid;
    },
    sessionid: function () {

        var _sessionid = sessionStorage.getItem('_sessionid');
        if (_sessionid) {
            return _sessionid;
        }
        else {
            var _sessionid = _statistic.newguid();
            sessionStorage.setItem('_sessionid', _sessionid);
            return _sessionid;
        }
    },
    deviceid: function () {
        var _deviceid = "";
        return _deviceid;
    },
    os: function () {

        var _os = "";

        if (_statistic.broinfo.v.ios) {
            _os = "iOS";
        }
        else if (_statistic.broinfo.v.android) {
            _os = "Android";
        }
        else if (_statistic.broinfo.v.mobile){
            _os = "Mobile";
        }
        else if (_statistic.broinfo.v.weixin) {
            _os = "Weixin";
        }
        else {
            _os = "PC";
        }

        return _os;
    },
    broinfo: {
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
    },
    $p: function () {
        var url = location.search;
        var theRequest = new Object();
        if (url.indexOf("?") != -1) {
            var str = url.substr(1);
            strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                theRequest[strs[i].split("=")[0].toLowerCase()] = decodeURI(strs[i].split("=")[1]);
            }
        }
        return theRequest;
    },
    newguid: function () {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
            if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                guid += "-";
        }
        return guid;
    },
    push: function (_page, _action, _actionvalue, _sourcekey, _element) {

        this.entity.Page = _page;
        this.entity.Action = _action;
        this.entity.ActionValue = _actionvalue;
        this.entity.SourceKey = _sourcekey;
        this.entity.Element = _element;

        try {

            $.ajax({
                type: 'POST',
                url: "http://logapi.zmjiudian.com/api/eventlog",
                data: JSON.stringify(this.entity),
                contentType: "application/json;",
                datatype: 'JSON ',
                success: function (_data) {
                    console.log(_data)
                }
            });

        } catch (e) {

        }
    }
}
_statistic.init();

//_statistic.push("今日特价", "访问", "", "", "");

//console.log("数据收集")
//console.log(_statistic)