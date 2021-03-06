﻿
var _Modal = {

    configObject: {
        title: '提示',
        content: '',
        textAlign: 'left',
        confirmText: '确定',
        confirmColor: '#3E9EC0',
        showClose: false,
        showCancel: false,
        cancelText: '取消',
        cancelColor: '#919191',
        confirm: function () { },
        cancel: function () { },
        close: function () { }
    },

    init: function () {

        var _self = this;

        _self._modal_mask = $("._modal-mask");
        _self._modal_section = $("._modal-section");
        _self._tit = _self._modal_section.find("._tit");
        _self._content = _self._modal_section.find("._body");
        _self._close = _self._modal_section.find("._close");
        _self._left = _self._modal_section.find("._left");
        _self._right = _self._modal_section.find("._right");
        _self._confirm = _self._modal_section.find("._confirm");
        _self._cancel = _self._modal_section.find("._cancel");
        _self._ok = _self._modal_section.find("._ok");

        //set title & content
        _self._tit.html(_self.configObject.title);
        _self._content.html(_self.configObject.content);

        if (_self.configObject.textAlign) {
            _self._content.css("text-align", _self.configObject.textAlign);
        }

        //confirm & cancel
        if (_self.configObject.showCancel) {

            _self._confirm.html(_self.configObject.confirmText);
            _self._cancel.html(_self.configObject.cancelText);
            _self._left.show();
            _self._right.show();
            _self._ok.hide();

            _self._confirm.unbind("click");
            _self._confirm.click(_self.configObject.confirm);

            _self._cancel.unbind("click");
            _self._cancel.click(_self.configObject.cancel);

        }
        else {

            _self._ok.html(_self.configObject.confirmText);
            _self._ok.show();
            _self._left.hide();
            _self._right.hide();

            _self._ok.unbind("click");
            _self._ok.click(_self.configObject.confirm);

        }

        //关闭
        if (_self.configObject.showClose) {

            _self._close.show();
            _self._close.click(function () {
                _self.hide();

                if (_self.configObject.close && _self.configObject.close != undefined && _self.configObject.close != 'undefined') {
                    try { _self.configObject.close(); } catch (e) { }
                }
                else {
                    try { _self.configObject.cancel(); } catch (e) { }
                }
            });
        }
        else {
            _self._close.hide();
        }

    },
    setConfigObj: function (object) {

        var _self = this;

        _self.configObject.title = object.title;
        _self.configObject.content = object.content;
        _self.configObject.textAlign = object.textAlign;
        _self.configObject.confirmText = object.confirmText;
        _self.configObject.confirmColor = object.confirmColor;
        _self.configObject.showCancel = object.showCancel;
        _self.configObject.showClose = object.showClose;
        _self.configObject.cancelText = object.cancelText;
        _self.configObject.confirm = object.confirm;
        _self.configObject.cancel = object.cancel;
        _self.configObject.close = object.close;

    },
    show: function (object) {

        var _self = this;

        _self.setConfigObj(object);
        _self.init();
        _self.autoPosition();
        _self._modal_section.show();
        _self._modal_mask.show();

    },
    hide: function () {

        var _self = this;

        _self._modal_mask.hide();
        _self._modal_section.hide();

    },
    autoPosition: function () {

        var _self = this;

        var _wwidth = $(window).width();
        var _wheight = $(window).height();
        var _left = 0;
        var _top = 0;
        var _sectionWidth = 250;
        var _sectionHeight = 250;
        if (_wwidth >= 330) {
            _sectionWidth = 300;
        }
        _left = (_wwidth - _sectionWidth) / 2;
        _top = (_wheight - _sectionHeight) / 2;
        _self._modal_section.css("left", _left);
        _self._modal_section.css("top", _top);

    },
    createElement: function () {

        var _self = this;

        var _html = '<div class="_modal-mask" style="display:none;"></div>';
        _html += '<div class="_modal-section" style="display:none;">';
        //_html += '<div class="_close zmjd-iconfont">&#xe615;</div>';
        _html += '<div class="_close zmjd-iconfont"><img src="http://whfront.b0.upaiyun.com/web/lib/modal/icon_close.png" alt="关闭"></div>';
        _html += '<div class="_tit">' + _self.configObject.title + '</div>';
        _html += '<div class="_body">' + _self.configObject.content + '</div>';
        _html += '<div class="_btns">';
        _html += '<div class="_left"><a href="javascript:;" class="_confirm">' + _self.configObject.confirmText + '</a></div>';
        _html += '<div class="_right"><a href="javascript:;" class="_cancel">' + _self.configObject.cancelText + '</a></div>';
        _html += '<div style="clear:both;"></div>';
        _html += '<a href="javascript:;" class="_ok">' + _self.configObject.confirmText + '</a>';
        _html += '</div>';
        _html += '</div>';

        $('body').append(_html);
    }

}

window.onresize = function () {
    _Modal.autoPosition();
}

//_Modal Create
_Modal.createElement();


var _Loading = {

    configObject: {

    },

    init: function () {

        var _self = this;

        _self._loading_mask = $("._loading-mask");
        _self._loading_section = $("._loading-section");
    },
    setConfigObj: function (object) {

        var _self = this;

    },
    show: function (object) {

        var _self = this;

        _self.setConfigObj(object);
        _self.init();
        _self.autoPosition();
        _self._loading_section.show();
        //_self._loading_mask.show();

    },
    hide: function () {

        var _self = this;

        //_self._loading_mask.hide();
        _self._loading_section.hide();

    },
    autoPosition: function () {

        var _self = this;

        var _wwidth = $(window).width();
        var _wheight = $(window).height();
        var _left = 0;
        var _top = 0;
        var _sectionWidth = 60;
        var _sectionHeight = 60;
        _left = (_wwidth - _sectionWidth) / 2;
        _top = (_wheight - _sectionHeight) / 2;
        _self._loading_section.css("left", _left);
        _self._loading_section.css("top", _top);

    },
    createElement: function () {

        var _self = this;

        var _html = '<div class="_loading-section" style="display:none;"><img src="http://whfront.b0.upaiyun.com/app/img/loading-changes.gif" alt="loading" /></div>';
        $('body').append(_html);
    }
}

//_Loading Create
_Loading.createElement();