$(document).ready(function () {
    
    var isapp = $("#isapp").val() == "1";
    var isThanVer46 = $("#isThanVer46").val() == "1";
    var pubhotelid = $("#hotelid").val();
    var pubhotelname = $("#hotelname").val();
    var pubpid = $("#pid").val();
    var pubuserid = $("#userid").val();
    var pubalbumid = $("#albumid").val();
    var pubexchangeid = $("#exchangeid").val();

    //init calendar
    var calendar = null;

    // main calendar
    var onSelect = function (newCheckIn, newCheckOut)
    {
        $("#checkIn").val(Calendar.format(newCheckIn));
        $("#checkOut").val(Calendar.format(newCheckOut));

        var cin = formatDate(newCheckIn, "yyyy-MM-dd");
        var cout = formatDate(newCheckOut, "yyyy-MM-dd");
    };

    var onselectRange = function (newCheckIn, newCheckOut)
    {
        $("#checkIn").val(Calendar.format(newCheckIn));
        $("#checkOut").val(Calendar.format(newCheckOut));

        var cin = formatDate(newCheckIn, "yyyy-MM-dd");
        var cout = formatDate(newCheckOut, "yyyy-MM-dd");

        $("#checkIn").val(cin);
        $("#checkOut").val(cout);

        var dic = {};
        dic["hotelid"] = pubhotelid;
        dic["pid"] = pubpid;
        dic["checkIn"] = cin;
        dic["checkOut"] = cout;
        dic["userid"] = pubuserid;
        dic["dateSelectType"] = 1;

        $.get('/Hotel/GetPackageCalendarPrice', dic, function (content) {
            var price = content.price;
            var tip = content.tip;
            var days = content.days;

            $(".price-line .pval").html("￥" + price);
            $(".tip-line").html(tip);

            $("#nightCount").val(days);
        });
    };

    //show calendar
    calendar || (calendar = new Calendar(onSelect, window.calendarOptions, onselectRange, function () { }, $(".sub-go")));

    //select calendar
    //$(".submit").click(function () {
    //    calendar.show();

    //    //def calendar selectRange
    //    var cin = $("#checkIn").val();
    //    var cout = $("#checkOut").val();
    //    calendar.selectRange(new Date(cin), new Date(cout));
    //});
    $(".submit").click(function () {
        
        location.href = "/coupon/exchange/" + pubexchangeid + "?userid=" + pubuserid + "&hotelid=" + pubhotelid + "&pid=" + pubpid + "&_newpage=1";

    });

    showSpinner.prefetch();

    function zhengchu(x, y) {
        var z = parseInt(x / y);
        if (z * y == x) {
            return true;
        }
        return false;
    }

    function formatDate(date, format) {
        if (!date) return;
        if (!format) format = "yyyy-MM-dd";
        switch (typeof date) {
            case "string":
                date = new Date(date.replace(/-/, "/"));
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

    function formatToDays(sm) {
        return sm / 1000 / 60 / 60 / 24;
    }

    var loadPackageContent = function (_pid, _userid, _albumid) {
        $.get('/Hotel/PackageContent', { pid: _pid, userid: _userid, albumid: _albumid }, function (html) {
            if (html) {
                $("#package-content").hide().html(html).fadeIn(200);
                showRoomInfo(0);
                setTimeout(function () {
                    $("html,body").animate({ scrollTop: $(".activity .tit").offset().top - 20 }, 300);
                }, 500);
            }
        });
    };

    $(".room-item").each(function () {
        $(this).click(function () {
            var _pid = $(this).data("pid");
            loadPackageContent(_pid, pubuserid, pubalbumid);
            //location.href = "/Hotel/Package/" + _pid + "?userid=" + pubuserid;
        });
    });
});
