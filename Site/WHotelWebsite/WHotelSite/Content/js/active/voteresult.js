var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var activeId = $("#activeId").val();
var voteId = $("#voteId").val();    //当前被投票者的编号（目前存储的是ActiveRuleEx的HotelId字段）

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
                "voteInfo": {
                    "thisVoteInfo": null,
                    "voteResultList": [],
                }
            }
        })
    }
    init();

    var loadActiveInfo = function () {

        //获取活动信息
        var _dic = { "activeId": activeId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetOneWeixinActive", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                activeData.activeInfo = _data;

                Vue.nextTick(function () {


                });
            }
            
        });
    }
    loadActiveInfo();

    var loadVoteInfo = function () {

        //获取活动信息
        var _dic = { "activeId": activeId };
        $.get(_Config.APIUrl + "/api/weixinapi/GetActiveRuleExsByActiveId", _dic, function (_data) {

            console.log(_data);
            if (_data) {

                //存储当前指定被投票者的对象
                var _thisVoteItem = null;

                //过滤投票者
                for (var _num = 0; _num < _data.length; _num++) {

                    var _item = _data[_num];

                    //头像图片格式更换
                    _item.PicUrl = _item.PicUrl.replace("_jupiter", "_290x290").replace('_theme', '_290x290').replace('_small', '_290x290');

                    //是否当前被投票者
                    if (_item.HotelId == voteId) {
                        _thisVoteItem = _item;
                    }

                    //编号格式化
                    _item.Number = (_item.HotelId < 10 ? "0" + _item.HotelId : _item.HotelId);

                    //排名
                    _item.Ranking = (_num + 1);
                }

                //所有投票记录
                activeData.voteInfo.voteResultList = _data;

                //当前被投票者
                activeData.voteInfo.thisVoteInfo = _thisVoteItem;

                Vue.nextTick(function () {


                });
            }
        });
    }
    loadVoteInfo();

});
