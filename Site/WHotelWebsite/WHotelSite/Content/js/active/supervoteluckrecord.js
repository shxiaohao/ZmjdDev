var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var activeId = $("#activeId").val();
var activeDrawId = $("#activeDrawId").val();

var _Config = new Config();

var activeData;

$(function () {

    //init data
    var init = function () {

        //初始订单详情
        activeData = new Vue({
            el: "#active-obj",
            data: {
                "activeInfo": {},
                "luckRecordList": []
            }
        })
    }
    init();

    //获取当前用户的抽奖记录
    var loadSpokeItems = function () {

        var _dic = { "activedrawid": activeDrawId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveLuckRecordAndPrizeByDrawId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //格式过滤
                for (var i = 0; i < _data.length; i++) {

                    var _item = _data[i];

                    _item.RecordTimeDate = _item.RecordTime.split('T')[0];
                    _item.RecordTime = _item.RecordTime.replace(/-/g, "/");
                }

                activeData.luckRecordList = _data;

                Vue.nextTick(function () {


                });
            }

        });
    }
    loadSpokeItems();
});
