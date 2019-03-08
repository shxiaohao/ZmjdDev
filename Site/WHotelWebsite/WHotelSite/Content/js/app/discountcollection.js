$(function ()
{
    $(".menu-panel ul li").each(function ()
    {
        $(this).click(function ()
        {
            var num = $(this).data("num");
            var tabId = "tab-" + num;
            var tabObj = $("#" + tabId);

            $(".menu-panel ul li").each(function () { $(this).removeClass("sel"); });
            if (num == "2") {
                $(".l-def-1").addClass("l-def-1-2");
                $(".c-def-2").addClass("c-def-2-2");
                $(".r-def-1").removeClass("r-def-1-2");
            }
            else if (num == "3") {
                $(".l-def-1").removeClass("l-def-1-2");
                $(".r-def-1").addClass("r-def-1-2");
            }
            else {
                $(".l-def-1").removeClass("l-def-1-2");
                $(".c-def-2").removeClass("c-def-2-2");
                $(".r-def-1").removeClass("r-def-1-2");
            }
            $(this).addClass("sel");

            $(".dc-panel .tab").each(function () { $(this).hide(); });
            var dt = tabObj.data("dt");
            var range = tabObj.data("rg");
            if (dt == "0") {
                $.get('/App/DiscountDistrictCollection', { range: range }, function (html) {
                    if (html) {
                        tabObj.html(html);
                        tabObj.show();
                        tabObj.data("dt", "1");
                        $("html,body").animate({ scrollTop: 0 }, 300);
                    }
                });
            }
            else {
                tabObj.show();
                $("html,body").animate({ scrollTop: 0 }, 300);
            }
        });
    });

});

function gotopage(param) {
    var isApp = $("#isApp").val();
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    if (isApp == "0") {
        url = "http://www.zmjiudian.com/" + param;
    }
    this.location = url;
}

function gourl(url) {
    location.href = url;
}

$(window).scroll(
    function () {
        var m_st = Math.max(document.body.scrollTop || document.documentElement.scrollTop);
        if (m_st >= ($('.menu-panel').offset().top + 100)) {
            $('#menu-panel').addClass("menu-panel-fixed");
        } else {
            $('#menu-panel').removeClass("menu-panel-fixed");
        }
});
