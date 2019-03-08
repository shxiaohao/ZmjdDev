$(function () {
    
});
      
mui.init();

var slider = mui("#slider");
slider.slider({
    interval: 3000
});

function goto(param) {
    var url = "http://www.zmjiudian.com/" + param;
    this.location = url;
}

function gotopage(param) {
    var url = "http://www.zmjiudian.com/" + param;
    this.location = url;
}

function gourl(url) {
    location.href = url;
}

$("img").lazyload({
    threshold: 220,
    placeholder: "http://whfront.b0.upaiyun.com/app/img/home/pic-loading.png",
    effect: "fadeIn"
});

var timerTags = $(".timer-tag");
var timeDic = [];
if (timerTags) {
    for (var i = 0; i < timerTags.length; i++) {

        timeDic[i] = {
            timerEntity: null,
            nowTime: null,
            endDate: null,
            pubTimer: null,
            initNowtime: function () {
                this.nowTime = new Date(
                parseInt(this.timerEntity.data("year0"))
                , parseInt(this.timerEntity.data("month0"))
                , parseInt(this.timerEntity.data("day0"))
                , parseInt(this.timerEntity.data("hour0"))
                , parseInt(this.timerEntity.data("minute0"))
                , parseInt(this.timerEntity.data("second0"))
                    ).getTime();
            },
            initEndtime: function () {
                this.endDate = new Date(
                parseInt(this.timerEntity.data("year1"))
                , parseInt(this.timerEntity.data("month1"))
                , parseInt(this.timerEntity.data("day1"))
                , parseInt(this.timerEntity.data("hour1"))
                , parseInt(this.timerEntity.data("minute1"))
                , parseInt(this.timerEntity.data("second1"))
                    ).getTime();
            },
            init: function () {
                this.initNowtime();
                this.initEndtime();
            },
            timerAction: function () {
                var t = this.endDate - this.nowTime;
                var d = Math.floor(t / (1000 * 60 * 60 * 24));
                var h = Math.floor(t / 1000 / 60 / 60 % 24) + (d * 24);
                var m = Math.floor(t / 1000 / 60 % 60);
                var s = Math.floor(t / 1000 % 60);

                var timehtml = h <= 0 ? "还有" + m + "分" + s + "秒开始" : "还有" + h + "小时" + m + "分开始";

                this.timerEntity.html(timehtml);
                //$("#timer-tag-0").html(timehtml);

                try {

                    if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                        clearInterval(this.pubTimer);
                        this.stopAction();
                    }

                } catch (e) { }

                this.nowTime = this.nowTime + 1000;
            },
            stopAction: function () {
                this.timerEntity.html("进行中");
            }
        };

        //build
        timeDic[i].timerEntity = $(timerTags[i]);

        //init
        timeDic[i].init();

        //start
        timeDic[i].timerAction();
        setInterval("gotime(timeDic[" + i + "])", 1000);
    }
}

function gotime(timeObj)
{
    timeObj.timerAction();
}