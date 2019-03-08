!function(t) {
    var e = {_cache_: {},VERSION: "1.0.2",DKEY: "yyyy-mm",MASK: "yyyy-mm-dd",log: function(t) {
            return e._cache_.logs || (e._cache_.logs = []), (t ? void e._cache_.logs.push(t) : e._cache_.logs)
        },pad: function(t, e) {
            for (t += "", e = e || 2; t.length < e; )
                t = "0" + t;
            return t
        },format: function(t, a) {
            a = a || this.MASK;
            var n = (/d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|"[^"]*"|'[^']*'/g), i = t.getDate(), s = e.pad, r = (t.getDay(), t.getMonth()), o = t.getFullYear(), c = t.getHours(), d = t.getMinutes(), h = t.getSeconds(), l = t.getTime(), p = {d: i,dd: s(i),m: r + 1,mm: s(r + 1),yy: String(o).slice(2),yyyy: o,H: c,M: d,S: h,L: s(l, 3)};
            return a.replace(n, function(t) {
                return (t in p ? p[t] : t.slice(1, t.length - 1))
            })
        },parseDateInt: function(t) {
            return "string" == typeof t && (t = t.toDate()), (t ? parseInt(e.format(t, "yyyymmdd"), 10) : !1)
        },addMonth: function(t, e) {
            return new Date(t.getFullYear(), t.getMonth() + e, 1)
        },get: function(t) {
            if (!t)
                return e.log("Date is null, Can't create calendar data structure."), null;
            var a = e.format(t, "yyyy-mm");
            return e._cache_[a] || e._create(t, a), e._cache_[a]
        },_create: function(t, a) {
            for (var n = t.getFullYear(), i = t.getMonth(), s = (t.getDay(), t.getDate(), new Date(n, i, 1).getDay() - 1), r = (s + new Date(n, i + 1, 0).getDate(), []), o = 0; 42 > o; o++) {
                var c = new Date(n, i, o - s), d = e.format(c, "yyyy-mm-dd"), h = {o: c,k: d,i: parseInt(d.replace((/-/gi), ""), 10),y: c.getFullYear(),m: c.getMonth(),d: c.getDate(),w: c.getDay()};
                r.push(h)
            }
            return e._cache_[a] = r, r
        }}, a = new Date, n = null, i = function(i, s) {
        this.setting = {options: {container: t.container,reference: !1,step: 2,offset: {},minDate: "#",maxDate: null,startDate: null,endDate: null,permit: null,prohibit: null,weekday: "0123456",render: "default",showAlways: !1,showOptions: !1,showWeek: !0,nextEl: null,rangeColor: "#c3daf7",hoverColor: "#d9e5f4",defaultDate: null,date: null,tipText: "yyyy-mm-dd",zindex: 9999},string: {header: "请选择日期",title: "yyyy年m月",week: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],weekText: ["pic_sun", "pic_mon", "pic_tue", "pic_wed", "pic_thu", "pic_fir", "pic_sat"],todayText: ["pic_today", "pic_tomorrow", "pic_aftertomorrow"]},classNames: {select: "day_selected",nothismonth: "day_over",blankdate: "day_no",today: "today",tomorrow: "",aftertomorrow: ""},listeners: {onBeforeShow: null,onShow: null,onChange: null},template: {wrapper: '<div class="calendar_wrap">\n<div class="calendar_content" id="${uid}"></div>\n<a class="month_prev" href="javascript:void(0);" data-bind="prev"></a>\n<a class="month_next" href="javascript:void(0);" data-bind="next"></a>\n</div>',calendar: '<div class="calendar_month${monthclass}">\n<div class="calendar_title">${title}</div>\n<dl class="calendar_day">\n<dt class="weekend">日</dt><dt>一</dt><dt>二</dt><dt>三</dt><dt>四</dt><dt>五</dt><dt class="weekend">六</dt>\n<dd>\n{{each data}}\n{{if isfestival}}<a href="javascript:void(0);" ${classes}><span class="c_day_festival" ${attr}>${day}</span>{{else}}<a href="javascript:void(0);" ${classes} ${attr}>${day}{{/if}}</a>\n{{/each}}\n</dd>\n</dl>\n</div>',styles: "\n.calendar_wrap{position:relative;display:inline-block;padding:3px 5px 0;font-size:12px;font-family:tahoma,​Arial,​Helvetica,​simsun,​sans-serif;border:1px solid #999;background:#fff;*display:inline;*zoom:1;box-shadow:0 3px 5px #ccc;}.calendar_content{background:#bbb;}.calendar_month{float:left;overflow:hidden;width:182px;text-align:center;}.calendar_title{height:23px;line-height:23px;font-weight:bold;color:#fff;text-align:center;background-color:#004fb8;}.month_next,.month_prev{position:absolute;top:3px;width:23px;height:23px;color:#fff;background:#2d7ce7 url(http://pic.c-ctrip.com/cquery/un_calender_index.png) no-repeat;cursor:pointer;cursor:pointer;}.month_prev{left:5px;}.month_next{right:5px;float:right;background-position:100% 0;}.month_prev:hover{background-color:#62adf1;background-position:0 -26px;}.month_next:hover{background-color:#62adf1;background-position:100% -26px;}.calendar_day{overflow:hidden;padding-top:4px;padding-bottom:5px;}.calendar_day dt{display:inline;float:left;width:26px;height:22px;line-height:20px;color:#666;border-bottom:2px solid #fff;background:#ececec;}.calendar_day .weekend{font-weight:bold;color:#f90;}.calendar_day dd{_width:185px;}.calendar_day dd a{float:left;overflow:hidden;width:26px;height:24px;line-height:22px;border:1px solid #fff;border-width:1px 0;font-size:11px;font-weight:bold;color:#005ead;text-decoration:none;cursor:pointer;}.calendar_day a:hover,.calendar_day .today,.calendar_day .day_selected,.calendar_day .c_festival_select,.calendar_day .c_festival_select:hover{background:#e6f4ff url(http://pic.c-ctrip.com/cquery/un_calender_index.png) no-repeat;}.calendar_day a:hover{background-color:#e6f4ff;background-position:-26px -53px;text-decoration:none;}.calendar_day .today{background-color:#fff5d1;background-position:0 -82px;}.calendar_day .day_over,.calendar_day .day_no{font-weight:normal;color:#dbdbdb;outline:none;cursor:default;}.calendar_day .day_over:hover,.calendar_day .day_no:hover{background:#fff;}.calendar_day .day_selected,.calendar_day .day_selected:hover{background-color:#629be0;background-position:0 -53px;color:#fff;}.calendar_day .c_festival_select,.calendar_day .c_festival_select:hover{background-color:#ffe6a6;background-image:url(http://pic.c-ctrip.com/cquery/un_calender_index.png);background-position:0 -111px;}.calendar_month.other{width:192px;}.calendar_month.other .calendar_day{margin-left:4px;padding-left:4px;border-left:2px solid #bbb;}.calendar_day .c_festival_select,.calendar_day .c_festival_select:hover{background-color:#ffe6a6;background-image:url(http://pic.c-ctrip.com/cquery/un_calender_index.png);background-position:0 -111px;}.c_yuandan span,.c_chuxi span,.c_chunjie span,.c_yuanxiao span,.c_qingming span,.c_wuyi span,.c_duanwu span,.c_zhongqiu span,.c_guoqing span,.c_jintian span,.c_shengdan span{width:24px;height:24px;background-image:url(http://pic.c-ctrip.com/cquery/un_festivals.png?v=1);background-repeat:no-repeat;text-indent:-9999em;overflow:hidden;display:block;}.c_yuandan span{background-position:0 0;}.c_chuxi span{background-position:0 -32px;}.c_chunjie span{background-position:0 -64px;}.c_yuanxiao span{background-position:0 -96px;}.c_qingming span{background-position:0 -128px;}.c_wuyi span{background-position:0 -160px;}.c_duanwu span{background-position:0 -192px;}.c_zhongqiu span{background-position:0 -224px;}.c_guoqing span{background-position:0 -256px;}.c_jintian span{background-position:0 -288px;}.c_shengdan span{ background-position: 0 -320px;}.c_calender_date{display:inline-block;color:#666;text-align:right;position:absolute;z-index:1;}.calendar_wrap:before,.calendar_wrap:after{content:'.';display:block;overflow:hidden;visibility:hidden;font-size:0;line-height:0;width:0;height:0;}.calendar_wrap:after{clear:both;}\n"},festival: {}};
        var r = this;
        if (r.target = t(i), s && (t.extend(!0, this.setting, s), s.options && s.options.date)) {
            var o = s.options.date.toDate();
            o && (a = o)
        }
        n = e.parseDateInt(a), r.uid = "calendar" + r.target.uid(), this._fragment = document.createDocumentFragment();
        var c = !1;
        if (this.setting.options.showAlways)
            c || (r._init(), c = !0), r.show(), this._isShow = !1;
        else {
            var d = "focus";
            r.target.bind(d, function() {
                c || (r._init(), c = !0), r.show()
            }), r.target.bind("mousedown", function() {
                c || (r._init(), c = !0), r.show()
            })
        }
    };
    i.prototype = {_init: function() {
            var a = this.setting.options;
            this.container = t(a.container), this.step = 1 * (a.step || 1), this.setMinDate(a.minDate), a.maxDate && (a.maxDate = a.maxDate.toDate(), this._max = e.parseDateInt(a.maxDate)), a.starDate && (this._start = e.parseDateInt(a.startDate)), a.endDate && (this._end = e.parseDateInt(a.endDate)), a.reference && (this.reference = t(a.reference)), this._initTpl()
        },_initTpl: function() {
            var e = this.uid, a = (this.setting.string, this.setting.template), n = this.setting.options, i = '<hr style="display:none;line-height:0;font-size:0;border:none;" /><style>' + a.styles.replace((/(\s*)([^\{\}]+)\{/g), function(t, a, n) {
                return a + n.replace((/([^,]+)/g), "#" + e + " $1") + "{"
            }) + "</style>", s = document.createElement("div");
            s.id = e, s.style.cssText = "width:" + this.setting.monthWidth * n.step + "px;z-Index:" + this.setting.options.zindex + ";";
            var r = t(s);
            r.html(i + t.tmpl.render(a.wrapper, {uid: "c_" + e})), this._layout = r, this._content = r.find("#c_" + e);
            var o = this, c = !0;
            r.bind("mousedown", function(t) {
                var e = t.target || t.srcElement;
                o._handleEvent(e), c = !1;
                var a = e.nodeName.toLowerCase();
                "select" != a && t.stop()
            }), n.showAlways || (t(document).bind("click", function(t) {
                o._checkExternalClick(t), c = !0
            }), this.target.bind("blur", function() {
                c && o.hide()
            }), r.bind("mouseup", function() {
                c = !0
            })), this.reference && (o._tempEnd = !1, r.bind("mousemove", function(t) {
                var e = t.target;
                (o.isIn(o._content[0], e) ? o._checkHoverColor(t.target) : o._tempEnd && (o._tempEnd = !1, o.update()))
            }))
        },setMinDate: function(t) {
            if (!t)
                return void (this._min = !1);
            if ("#" == t)
                this.setting.options.minDate = a, s = n;
            else {
                var i = t.toDate(), s = !1;
                i && (this.setting.options.minDate = i, s = e.parseDateInt(i))
            }
            this._min = s
        },isIn: function(t, e) {
            for (var a = e; a && 9 !== a.nodeType; ) {
                if (a == t)
                    return !0;
                a = a.parentNode
            }
            return !1
        },_checkExternalClick: function(e) {
            var a = e.target || e.srcElement, n = (t(a), this.uid);
            if (!this.isIn(this._layout[0], a) && a != this.target[0] && a.id != n) {
                if ("OPTION" == a.nodeName || "#document" == a.nodeName)
                    return !1;
                this.hide()
            }
        },_handleEvent: function(t) {
            var e = t.getAttribute("data-bind");
            if (e)
                switch (e) {
                    case "close":
                        this.hide();
                        break;
                    case "prev":
                        this.changeDrawMonth(-this.step);
                        break;
                    case "next":
                        this.changeDrawMonth(this.step);
                        break;
                    case "select":
                        this.select(t)
                }
            return !1
        },changeDrawMonth: function(t) {
            this._drawDate = e.addMonth(this._drawDate, t), this.update()
        },show: function() {
            var t = this, e = this.setting.options;
            if (!t._isShow) {
                i.__inst && i.__inst.uid != this.uid && i.__inst.hide(), this._updateOptions();
                var a = this.setting.listeners;
                a.onBerforeShow && a.onBerforeShow.call(this), t.update(), t.container.append(t._layout), e.showAlways || (t._layout.offset(this.target, t.setting.options.offset), i.__inst = this), a.onShow && a.onShow.call(this), this.setting.options.showAlways || this._layout.cover(), t._isShow = !0
            }
        },hide: function() {
            this._isShow && (this.setting.options.showAlways || this._layout.uncover(), this._fragment.appendChild(this._layout[0]), this._isShow = !1)
        },_updateOptions: function() {
            var a = this.target.data(), n = this.setting.options;
            for (var i in a)
                switch (i) {
                    case "startDate":
                        var s = e.parseDateInt(a.startDate);
                        s != this._start && (n.startDate = a.startDate, this._start = s);
                        break;
                    case "endDate":
                        var r = e.parseDateInt(a.endDate);
                        r != this._end && (n.endDate = a.endDate, this._end = r);
                        break;
                    case "minDate":
                        a.minDate = a.minDate.toDate();
                        var o = a.minDate;
                        o && o != n.minDate && (n.minDate = o, this.setMinDate(o));
                        break;
                    case "maxDate":
                        a.maxDate = a.maxDate.toDate();
                        var c = e.parseDateInt(a.maxDate);
                        c && c != this._max && (n.maxDate = a.maxDate, this._max = c);
                        break;
                    case "showWeek":
                        n.showWeek = a.showWeek;
                        break;
                    case "permit":
                        n.permit = a.permit;
                        break;
                    case "prohibit":
                        n.prohibit = a.prohibit;
                        break;
                    case "weekday":
                        n.weekday = a.weekday;
                        break;
                    case "reference":
                        this.reference = t(a.reference);
                        break;
                    case "nextEl":
                        n.nextEl = a.nextEl
                }
            if (this.reference) {
                this._tempEnd = !1;
                var d = this.reference.value(), h = d.toDate();
                h && (this.setting.options.startDate = h, this._start = e.parseDateInt(h))
            }
            this.setCurrentDate(this.target), this._start && this.currentDate && (n.endDate = this.currentDate, this._end = this.currentDint), this._parseDrawMonth()
        },_parseDrawMonth: function() {
            var t = !1, e = this.setting.options;
            (this.currentDate ? this._drawDate = this.currentDate : ((e.defaultDate ? t = e.defaultDate.toDate() : this._min && (t = e.minDate)), t || (t = a), this._drawDate = t))
        },setCurrentDate: function(t) {
            var a = t.value();
            if (a.isDate()) {
                var n = a.toDate();
                this.isPassDate(n) || (this.currentDate = n, this.currentDint = e.parseDateInt(n))
            }
        },update: function() {
            var t = this.step, a = 1, n = this._create(this._drawDate);
            if (t > a)
                for (; t > a; a++)
                    n += this._create(e.addMonth(this._drawDate, a), "other");
            if (this._content.html(n), this.setting.options.showOptions) {
                var i = this._layout.find("select"), s = this;
                i.bind("change", function() {
                    s._drawDate = new Date(i[0].value, 1 * i[1].value - 1, 1), s.update()
                })
            }
        },_create: function(a, n) {
            var i = this, s = i.setting, r = s.options, o = a.getMonth(), c = s.template, d = "", h = this._renderCalendar(a);
            if (r.showOptions) {
                var l = o + 1, p = a.getFullYear(), u = (r.minDate ? r.minDate.getFullYear() : 1900), _ = (r.maxDate ? r.maxDate.getFullYear() : 2020);
                d = '<select name="year">';
                for (var g = u; _ >= g; g++)
                    d += (g == p ? '<option value="' + g + '" selected="selected">' + g + "</options>" : '<option value="' + g + '">' + g + "</options>");
                d += "</select>年 ", d += '<select name="m">';
                for (var f = 1; 13 > f; f++)
                    d += (f == l ? '<option value="' + f + '" selected="selected">' + f + "</options>" : '<option value="' + f + '">' + f + "</options>");
                d += "</select>月"
            } else
                d = e.format(a, s.string.title);
            var m = t.tmpl.render(c.calendar, {title: d,monthclass: (n ? " " + n : ""),week: h});
            return m
        },_renderCalendar: function(t) {
            for (var a = e.get(t), n = t.getMonth(), i = this, s = 0, r = [], o = this.setting.classNames, c = {"default": function(t) {
                    return (t.m == n ? i._parseCellDate(t, o, n) : {day: "",date: null,classes: ' class="' + o.blankdate + '"'})
                },all: function(t) {
                    return i._parseCellDate(t, o, n)
                }}, d = c[this.setting.options.render]; 42 > s; s++)
                r.push(d(a[s]));
            var ret = [], week = {day: []};
            while (r.length && week.day.length < 7) {
                week.day.push(r.shift());
                if (week.day.length == 7) {
                    ret.push(week);
                    week = {day: []};
                }
            }
            week.day.length && ret.push(week);
            return ret;
        },_checkFestival: function(t) {
            var e = t.substring(5), a = this.setting.festival;
            return (a[t] ? a[t] : (a[e] ? a[e] : !1))
        },_parseCellDate: function(t, e) {
            var a = t.k, i = t.i, s = t.w, r = t.d, o = ' id="' + a + '"', c = [], d = {day: r,date: a,classes: "",attr: ""};
            if (i == n && c.push(e.today), this._checkPassDate(a, i, s))
                c.push(e.nothismonth);
            else {
                o += ' data-bind="select"';
                var h = this._checkFestival(a);
                h && (d.isfestival = !0, c.push(h[0])), this.currentDate && this.currentDint == i && c.push((d.isfestival ? "c_festival_select" : e.select)), this._start && (this._start == i && c.push((d.isfestival ? "c_festival_select" : e.select)), this._end && i > this._start && i < this._end && (o += ' style="background-color: ' + this.setting.options.rangeColor + '"'), this._tempEnd && i > Math.max(this._start, this._end || 0) && i < this._tempEnd && (o += ' style="background-color: ' + this.setting.options.hoverColor + '"'))
            }
            c.length || c.unshift('canselect');
            return c.length > 0 && (d.classes = ' class="' + c.join(" ") + '"'), d.attr = o, d
        },isPassDate: function(t) {
            var a = e.format(t), n = e.parseDateInt(t), i = t.getDay();
            return this._checkPassDate(a, n, i)
        },_checkPassDate: function(t, e, a) {
            var n = this.setting.options;
            return (n.prohibit && -1 !== n.prohibit.indexOf(t) ? !0 : (n.permit && -1 !== n.permit.indexOf(t) ? !1 : (this._min && e < this._min ? !0 : (this._max && e > this._max ? !0 : (-1 === n.weekday.indexOf(a) ? !0 : !1)))))
        },_checkHoverColor: function(t) {
            var a = t.getAttribute("data-bind"), n = ("select" == a ? !0 : !1);
            if (n) {
                var i = 0, s = t.id;
                if (s) {
                    var r = e.parseDateInt(s);
                    r && r > Math.max(this._start, this._end || 0) && (i = r)
                } else
                    i = 0;
                this._tempEnd != i && (this._tempEnd = i, this.update())
            }
        },select: function(a) {
            var n = a.id, i = n.toDate(), s = this.target[0], r = e.format(i, this.setting.options.tipText);
            this.target.value(n), this.currentDate = i, this.currentDint = e.parseDateInt(i), this.hide();
            var o = this, c = this.setting.listeners;
            c.onChange && setTimeout(function() {
                c.onChange.call(o, s, r, !1)
            }), this.target.trigger("change");
            var d = t(this.setting.options.nextEl);
            d.length && setTimeout(function() {
                d[0].focus()
            }), this.setting.options.showWeek && this.setWeek(), this.setting.options.showAlways && this.update()
        },getWhatDay: function(t) {
            var a = e.format(t);
            string = this.setting.string;
            var n = this._checkFestival(a);
            if (n)
                return n[0].replace("c_", "pic_");
            var i = new Date, s = new Date(i.getFullYear(), i.getMonth(), i.getDate()), r = parseInt((t - s) / 864e5, 10);
            return (r >= 0 && 3 > r ? string.todayText[r] : string.weekText[t.getDay()])
        },setWeek: function(e) {
            var a = null;
            a = (e ? t(e) : this.target);
            var n = a.value().toDate();
            if (n) {
                var i = this.getWhatDay(n);
                i && a.offset().width >= 105 && a.css({"background-image": "url(http://pic.c-ctrip.com/cquery/" + i + ".png)","background-position": "right center","background-repeat": "no-repeat"})
            } else
                a.css({"background-image": "none"})
        }};
    var s = {name: "calendar",version: "6.0",init: function() {
        },uninit: function() {
        },module: i};
    t.mod.reg(s)
}(cQuery);
