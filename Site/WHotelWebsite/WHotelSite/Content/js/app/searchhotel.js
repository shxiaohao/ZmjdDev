
function goto(param) {
    var url = "whotelapp://www.zmjiudian.com/" + param;
    this.location = url;
}

function gotopage(param) {
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    this.location = url;
}

function gourl(url) {
    location.href = url;
}


function checkFlashDeals()
{
    
}

$(function () {
    
    //更新背景图


    $(".search-inp").focus(function ()
    {
        $(this).addClass("search-inp-focus");
    });

    $(".search-inp").blur(function () {
        $(this).removeClass("search-inp-focus");
    });

    var searchInpTimer = null;
    $(".search-inp").keyup(function ()
    {
        clearTimeout(searchInpTimer);

        if ($('.search-inp').val() == "") {
            clearTimeout(searchInpTimer);
            $(".closeIcon").hide();
            $('.hotelquicklist').hide();
            $('.hotelquicklist').html("");
        }

        searchInpTimer = setTimeout(function ()
        {
            var keyword = $('.search-inp').val();
            if (keyword) {
                $.get('/Portal/SearchList', { keyword: keyword, page: "Home" }, function (html) {
                    if (html && html != "") {
                        $(".closeIcon").show();
                        $('.hotelquicklist').show();
                        $('.hotelquicklist').html(html);
                    }
                });
            }
        },
        500);
    });

    $(".search-inp").keydown(function ()
    {
        clearTimeout(searchInpTimer);
    });
    $(".closeIcon").click(function ()
    {
        $('.search-inp').val("");
        $('.hotelquicklist').hide();
        $('.hotelquicklist').html("");
        $(".closeIcon").hide();
    });

    //城市选择
    $("#filter-item-local").click(function ()
    {

    });

    //日期选择
    $("#filter-item-date").click(function () {

    });

    //价格星级选择
    $("#filter-item-pricestar").click(function () {

    });

    //查找酒店
    $(".search").click(function ()
    {
        //
    });

});