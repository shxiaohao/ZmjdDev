var isApp = $("#isApp").val();

$(function () {

    var lastOpenObj = null;
    var hideAllListTit = function ()
    {
        try {
            if (lastOpenObj && lastOpenObj != null)
            {
                var l = lastOpenObj.data("l");
                var s = lastOpenObj.data("s");
                var groupObj = $("#c-list-section-" + l);
                groupObj.hide();
                lastOpenObj.data("s", "0");
            }
        } catch (e) {

        }
    };

    $(".c-list-tit").each(function ()
    {
        var tit_obj = $(this);
        tit_obj.click(function ()
        {
            var l = tit_obj.data("l");
            var s = tit_obj.data("s");
            var groupObj = $("#c-list-section-" + l);
            if (s == "1") {
                groupObj.hide();
                tit_obj.data("s", "0");
            }
            else {

                hideAllListTit();

                groupObj.show();
                tit_obj.data("s", "1");

                lastOpenObj = tit_obj;

                $("html,body").animate({ scrollTop: tit_obj.offset().top - 5 }, 300);
            }
        });
    });

    $("#c-s-inp").focus(function () {
        $(".cityquicklist-bg").fadeIn();
        $(".cityquicklist-bg").data("sw", "1");
        if (isApp == "1") {
            $(".city-topbar").addClass("s-fixed");
            $(".s-inp").addClass("s-fixed2");
            $(".cityquicklist").css("top", "7em");
            $(".cityquicklist-bg").css("top", "7em");
        }
        else {
            $(".s-inp").addClass("s-fixed");
        }
    });

    var searchInpTimer = null;
    var cpLock = false;
    var sInp = $("#c-s-inp")[0];
    sInp.addEventListener('compositionstart', function () { cpLock = true; })
    sInp.addEventListener('compositionend', function () { cpLock = false; })
    sInp.addEventListener('input', function () {
        clearTimeout(searchInpTimer);
        if (!cpLock) {

            if ($('#c-s-inp').val() == "") {
                clearTimeout(searchInpTimer);
                $(".cityquicklist").hide();
                $('.cityquicklist').html("");
            }

            searchInpTimer = setTimeout(function () {
                var keyword = $('#c-s-inp').val();
                if (keyword) {
                    $.get('/App/SearchCityList', { keyword: keyword }, function (html) {
                        if (html && html != "" && html.indexOf("li") >= 0) {
                            showCityListElem(html);
                        }
                    });
                }
            },
            500);
        }
    });

    $("#c-s-inp").keydown(function () {
        clearTimeout(searchInpTimer);
    });

    $(".cityquicklist-bg").click(function () {
        hideCityListElem();
    });
});

//cancel
var checkCancel = function () {
    
    if ($(".cityquicklist-bg").data("sw") == "1") {
        hideCityListElem();
        return false;
    }
    return true;
};

var hideCityListElem = function () {
    if (isApp == "1") {
        $(".city-topbar").removeClass("s-fixed");
        $(".s-inp").removeClass("s-fixed2");
    }
    else {
        $(".s-inp").removeClass("s-fixed");
    }

    $('#c-s-inp').val("");
    $(".cityquicklist").hide();
    $('.cityquicklist').html("");
    $(".cityquicklist-bg").fadeOut();
    $(".cityquicklist-bg").data("sw", "0");
};

var showCityListElem = function (html) {
    $('.cityquicklist').show();
    $('.cityquicklist').html(html);
    $(".cityquicklist-bg").data("sw", "1");
};

var initCitySearchHistory = function () {
    var historyVal = getCitySearchHistory();
    if (historyVal != "null" && historyVal != "") {
        var dic = {};
        dic["history"] = historyVal;
        $.get('/App/GenCitySearchHistory', dic, function (result) {
            try {
                if (result && result.length > 0) {
                    var html = "<ul>";
                    for (var i = 0; i < result.length; i++) {
                        var item = result[i];
                        html += "<li onclick=\"districtClick('" + item.url + "', " + item.id + ", '" + item.name + "')\">" + item.name + "</li>";
                    }
                    html += "<ul>";
                    $(".s-history .s-list").html(html);
                    $(".s-history").show();
                }
            } catch (e) {

            }
        });
    }
};
var setCitySearchHistory = function (id, name) {
    var historyVal = getCitySearchHistory();
    if (historyVal != "null") {
        var item = (id + "," + name); if (historyVal != "") item += ";";
        historyVal = item + historyVal;

        localStorage.CitySearchHistory = historyVal;
    }
};
var getCitySearchHistory = function () {
    if (window.localStorage) {
        if (localStorage.CitySearchHistory == undefined) localStorage.CitySearchHistory = "";
        return localStorage.CitySearchHistory;
    }
    return "null";
}
initCitySearchHistory();
