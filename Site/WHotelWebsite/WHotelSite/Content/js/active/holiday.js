//让日期选择列表支持横向滑动
var dateListScroll = new IScroll('#dateList', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });

var curLi = $("#dateList ._scroller").find(".curli");
var liNum = parseInt(curLi.data("num"));
if (liNum > 2) {
    //如果选择的项靠后，则自动滚动到指定区域
    dateListScroll.scrollToElement('li:nth-child(' + (liNum - 1) + ')', 500);
}

function gourl(url) {
    location.href = url;
}

$(function () {
    var groupTits = $(".group .tit");
    groupTits.each(function () {
        $(this).click(function () {
            var _titItem = $(this).find(".tit-item");
            var _num = _titItem.data("num");
            var listObj = $("#group-list-" + _num);
            if (listObj) {
                var _op = listObj.data("op");
                if (_op == "1") {
                    listObj.hide();
                    listObj.data("op", "0");
                    _titItem.addClass("tit-close");
                }
                else {
                    listObj.show();
                    listObj.data("op", "1");
                    _titItem.removeClass("tit-close");
                    $("html,body").animate({ scrollTop: _titItem.offset().top - 65 }, 300);
                }
            }
        });
    });

    //收起
    var stopBars = $(".stop-group-bar");
    stopBars.each(function () {
        $(this).click(function () {
            var _num = $(this).data("num");
            var listObj = $("#group-list-" + _num);
            if (listObj) {
                listObj.slideUp(200);
                listObj.data("op", "0");

                var _titItem = listObj.parent().find(".tit-item");
                _titItem.addClass("tit-close");

                $("html,body").animate({ scrollTop: _titItem.offset().top - 70 }, 200);
            }
        });
    });

    //局部的查看更多
    var showPartMoreBars = $(".show-partmore-bar");
    showPartMoreBars.each(function () {
        $(this).click(function () {
            var _num = $(this).data("num");
            var _items = $("#group-list-" + _num + " .item");
            if (_items) {
                _items.each(function () {
                    $(this).show();
                });
            }

            var _stopGroupBar = $("#stop-group-bar-" + _num)
            _stopGroupBar.show();
            $(this).hide();
        });
    });

    //menu top
    var menuPanelOriTop = $('.menu-panel').offset().top;

    $(window).scroll(
        function () {
            var m_st = Math.max(document.body.scrollTop || document.documentElement.scrollTop);
            if (m_st < menuPanelOriTop) {
                $('#menu-panel').removeClass("menu-panel-fixed");
            } else {
                if ($('#menu-panel').attr("class").indexOf("fixed") < 0) {
                    $('#menu-panel').addClass("menu-panel-fixed");
                }
            }
        });
});

function loadHolidayContent(_strCheckDate, _userid, _swvip) {
    _Loading.show();
    $.get('/Active/HolidayContent', { strCheckDate: _strCheckDate, userid: _userid, swvip: _swvip }, function (html) {
        if (html) {
            $("#holiday-content").hide().html(html).fadeIn(200);
            //让日期选择列表支持横向滑动
            var dateListScroll = new IScroll('#dateList', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });

            var curLi = $("#dateList ._scroller").find(".curli");
            var liNum = parseInt(curLi.data("num"));
            if (liNum > 2) {
                //如果选择的项靠后，则自动滚动到指定区域
                dateListScroll.scrollToElement('li:nth-child(' + (liNum - 1) + ')', 500);
            }
            //menu top
            var menuPanelOriTop = $('.menu-panel').offset().top;
            $(window).scroll(
                function () {
                    var m_st = Math.max(document.body.scrollTop || document.documentElement.scrollTop);
                    if (m_st < menuPanelOriTop) {
                        $('#menu-panel').removeClass("menu-panel-fixed");
                    } else {
                        if ($('#menu-panel').attr("class").indexOf("fixed") < 0) {
                            $('#menu-panel').addClass("menu-panel-fixed");
                        }
                    }
                });
        }
        _Loading.hide();
    });
};