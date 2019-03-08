
var groupnos = $("#grounpnos").val()
var _Config = new Config();


//_Config.APIUrl = "http://api.dev.jiudian.corp";
//Config.APIUrl = "http://api.zmjd100.com";

var loadFunction = function () {



    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();

    var tabburList = [];
    var _groupNos = groupnos.split(',');
    for (var _idnum = 0; _idnum < _groupNos.length; _idnum++) {
        var _groupNo = parseInt(_groupNos[_idnum]);
        var _tabburlItem = {
            groupNo: _groupNo,
            name: "其它",
            start: 0,
            count: 20,
            packageList: {}
        }

        switch (_groupNo.toString()) {
            case "180501": { var _n = "主题"; _tabburlItem.name = _n; break; }
            case "180502": { var _n = "目的地"; _tabburlItem.name = _n; break; }
        }
        tabburList.push(_tabburlItem);
    }


    //初始绑定tabs
    var productTabData = new Vue({
        el: "#tabbur",
        data: {
            "tabDetailList": tabburList
        }
    })




    var albumList = [];

    var tabListArray = {};

    //加载页面是加载数据
    var loadData = function (_groupNo) {

        var _tabListItemKey = "tl-" + _groupNo;
        var _tabListItemData = tabListArray[_tabListItemKey];
        if (_tabListItemData) {
            packageData.albumDataList = tabListArray[_tabListItemKey];
        }
        else {
            $(".scrollpageloading").show();
            $.get(_Config.APIUrl + "/api/hotel/GetPackageAlbumsByGroupNo", { groupNo: _groupNo }, function (data) {
                if (data) {
                    for (var i = 0; i < data.length; i++) {
                        var album = {};
                        album.albumId = data[i].ID;
                        album.Name = data[i].Name;
                        album.CompleteCoverPicSUrl = data[i].CompleteCoverPicSUrl;
                        album.actionUrl = "http://www.zmjiudian.com/package/collection/" + data[i].ID + "?showType=1&_newpage=1";
                        packageData.albumDataList.push(album);
                    }

                    Vue.nextTick(function () {

                        $(".scrollpageloading").hide();

                        tabListArray[_tabListItemKey] = packageData.albumDataList;

                        $(".albumlist img").lazyload({
                            threshold: 20,
                            placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load2-16x9.png",
                            effect: "fadeIn"
                        });
                    });
                }
            })
        }

        
    }
    loadData(tabburList[0].groupNo);

    //选中当前菜单
    var filterItemClick = function () {
        var _mitem = $(this);
        var _sel = $(_mitem).data("sel");
        if (_sel === 0) {
            packageData.albumDataList = [];
            //单选
            $(".t-item").each(function () {
                $(this).removeClass("scroll-seled");
                $(this).data("sel", 0);
            });
            $(_mitem).addClass("scroll-seled");
            $(_mitem).data("sel", 1);
            var _groupNo = $(_mitem).data("groupno");
            loadData(_groupNo);
        }
    }
    var bindEvent = function () {
        $(".t-item").each(function () {
            //var _mitem = $(this);
            //设置宽度
            if (tabburList.length > 0) {
                var _chuNum = (tabburList.length > 4 ? 4.4 : tabburList.length);
                var _itemWidth = winWidth / _chuNum;
                $(this).css("width", _itemWidth);
            }
            $(this).click(filterItemClick);
        })
    }
    bindEvent()
    var packageData = new Vue({
        el: "#album",
        data: {
            "albumDataList": albumList
        }
    })
}


$(function () {
    loadFunction()
})
function gourl(url) {
    location.href = url;
}

