/////////////
/// for mobile

function Calendar(onselect, options) {
    this.options = options;
    this.oneday = 24 * 3600 * 1000;
    this.build();
    this.onselect = onselect;
}

Calendar.format = function (date) {
    return [date.getFullYear(), date.getMonth() + 1, date.getDate()].join('-').replace(/-(\d)\b/g, '-0$1');
};
Calendar.parse = function (dateStr) {
    dateStr = dateStr.replace(/(\d{4})-(\d?\d)-(\d?\d)/, '$2/$3/$1');
    return new Date(dateStr);
};
Calendar.getRemarks = function (options) {
    var limits = [], me = this;
    $.each([0, -1, 1], function () {
        var text = me.limitationText(options, this);
        if (text) {
            limits.push(text);
        }
    });
    return limits;
};
Calendar.limitationText = function (options, type) {
    var limit = options && options.limit;
    if (!limit) {
        return false;
    }
    var numText = function (n) {
        return n < 11 ? '一两三四五六七八九十'.charAt(n - 1) : n + '';
    };
    if (limit.max == limit.min) {
        if (type == 0 && limit.max > 0) {
            return limit.max == 1 ? '只能选择住一晚' : '只能选择' + numText(limit.max) + '晚连住';
        }
    } else {
        if (type == 1 && limit.max > 1) {
            return '最多只能订' + numText(limit.max) + '晚';
        }
        if (type == -1 && limit.min > 1) {
            return numText(limit.min) + '晚起订';
        }
    }
    return false;
};

Calendar.prototype = {
    show: function (anchor) {
        ActionSheet.show(this.element, $.proxy(this, 'hookup'), $.noop, anchor);
        this.deselect();
    },
    hide: function () {
        this.ondismiss && this.ondismiss();
        ActionSheet.hide(this.element);
    },
    select: function (date) {
        this.cellForDate(date).addClass('on');
    },
    selectRange: function (from, to) {
        if (!this.isRangeSelectable(from, to)) {
            return false;
        }
        this.deselect();
        for (var date = new Date(from.getTime()) ; date <= to; date.setDate(date.getDate() + 1)) {
            this.select(date);
        }
        this.cellForDate(from).addClass('ci');
        this.cellForDate(to).addClass('co');
        return true;
    },
    deselect: function () {
        this.selectedCells().removeClass('on ci co pre');
    },

    isRangeSelectable: function (from, to) {
        for (var date = new Date(from.getTime()) ; date < to; date.setDate(date.getDate() + 1)) {
            var cell = this.cellForDate(date);
            if (!cell.length || cell.is('.ex, .gray')) {
                return false;
            }
        }
        return true;
    },
    validateRange: function (from, to) {
        console.log(this.options)
        var limit = this.options && this.options.limit;
        var error = null;
        if (limit && (limit.min || limit.max)) {
            var delta = Math.round((to - from) / this.oneday);
            if (limit.min == limit.max && delta != limit.min) {
                error = 0;
            } else if (delta < limit.min) {
                error = -1;
            } else if (limit.max > 0 && delta > limit.max) {
                error = 1;
            }
        }
        if (error !== null) {
            alert('此套餐' + Calendar.limitationText(this.options, error) + ', 请重新选择');
            return false;
        }
        return true;
    },
    shouldAutoSelect: function () {
        var limit = this.options && this.options.limit;
        return limit && limit.min && limit.min == limit.max;
    },
    autoSelect: function (from) {
        var limit = this.options && this.options.limit;
        return this.selectRange(from, new Date(from.getTime() + limit.min * this.oneday));
    },
    autoScroll: function (clickedCell) {
        var endCell = this.selectedCells().last();
        var monthBox = endCell.closest('.months');
        var boxLine = monthBox.offset().top + monthBox.innerHeight();
        var cellLine = endCell.offset().top + endCell.outerHeight();
        if (boxLine < cellLine) {
            this.iscroll.scrollToElement(this.cells.eq(this.cells.index(clickedCell) - 7)[0], 300);
        }
    },
    preSelect: function (cell) {
        this.deselect();
        cell.addClass('pre on ci');
    },
    weekendDates: function (offset) {
        var date = new Date(this.todayDate.getTime() + this.oneday * (6 - this.todayDate.getDay())),
			counter = 0;
        while (counter < offset) {
            date.setDate(date.getDate() + 7);
            ++counter;
        }
        return [date, new Date(date.getTime() + this.oneday)];
    },

    onClick: function (e) {
        var clickedCell = $(e.target).closest('a');
        var clickedDate = this.dateForCell(clickedCell);

        if (this.shouldAutoSelect()) {
            if (this.autoSelect(clickedDate)) {
                this.autoScroll(clickedCell);
                return;
            }
        }
        var activeCell = this.activeCell();
        if (activeCell.length) {
            this.deselect();
            if (activeCell[0] == clickedCell[0]) {
                return;
            }
            var anchorDate = this.dateForCell(activeCell);
            var from = anchorDate < clickedDate ? anchorDate : clickedDate;
            var to = anchorDate < clickedDate ? clickedDate : anchorDate;
            if (this.selectRange(from, to)) {
                return;
            }
        }
        this.preSelect(clickedCell);
    },

    onClickEx: function (e) {
        var selectedCells = this.selectedCells();
        if (!selectedCells.length) {
            return;
        }
        var from = this.dateForCell(selectedCells.eq(0));
        var clickedCell = $(e.target).closest('a');
        var to = this.dateForCell(clickedCell);
        if (from < to) {
            this.selectRange(from, to);
        }
    },

    onConfirm: function () {
        var selectedCells = this.selectedCells();
        if (selectedCells.length > 1) {
            var d1 = this.dateForCell(selectedCells.first()),
				d2 = this.dateForCell(selectedCells.last());
            if (this.validateRange(d1, d2) && this.onselect) {
                this.onselect(d1, d2);
                this.hide();
            }
        } else {
            alert('请选择入住和离店日期');
        }
    },

    selectedCells: function () {
        return this.cells.filter('.on');
    },
    activeCell: function () {
        return this.cells.filter('.on.ci.pre');
    },

    cellForDate: function (date) {
        var index = Math.round((date - this.startDate) / this.oneday);
        return this.cells.eq(index);
    },
    dateForCell: function (elem) {
        var index = this.cells.index(elem);
        return new Date(this.startDate.getTime() + index * this.oneday);
    },

    // XXX 客户端时间不可靠
    build: function () {
        var today = Calendar.parse(Calendar.format(new Date)),
			first = today,
			end = new Date(today.getFullYear(), today.getMonth() + 2, today.getDate()),
			grayDays = {},
			contents = [];
        if (this.options) {
            first = Calendar.parse(this.options.start);
            end = Calendar.parse(this.options.end);
            $.each(this.options.grayDays, function () {
                grayDays[this] = 1;
            });
        }
        while (first > today) {
            first.setDate(first.getDate() - 1);
            grayDays[Calendar.format(first)] = 1;
        }
        var start = new Date(first.getTime() - first.getDay() * this.oneday),
			current = new Date(start.getTime()),
			begin = true;
        for (; current <= end; current.setDate(current.getDate() + 1)) {
            if (begin || current.getDate() == 1) {
                if (!begin) {
                    contents.push('</div>');
                }
                begin = false;
                contents.push(
					'<div class="cap">', current.getFullYear(), '年', current.getMonth() + 1, '月</div>',
					'<div class="days">'
				);
                var padding = new Date(current.getTime() - this.oneday);
                while (padding.getDay() != 6) {
                    contents.push('<a class="ex">0</a>');
                    padding.setDate(padding.getDate() - 1);
                }
            }
            if (current < today) {
                contents.push('<a class="ex">');
            } else if (current < first || grayDays[Calendar.format(current)] == 1) {
                contents.push('<a class="gray">');
            } else {
                contents.push('<a href="javascript:;">');
            }
            contents.push(current.getTime() == today.getTime() ? '今天' : current.getDate(), '</a>');
        }
        contents.push('</div>');

        var remarks = Calendar.getRemarks(this.options);
        remarks.push('灰色日期为不可订');
        var html = [
			'<div class="calendar">',
				'<div class="ctrls top">',
					'<div class="tip">', '*', remarks.join(' *'), '</div>',
				'</div>',
                '<div class="weekdays">',
					'<b class="w">日</b><b>一</b><b>二</b><b>三</b><b>四</b><b>五</b><b class="w">六</b>',
				'</div>',
				'<div class="months" id="months"><div class="months_i">',
					contents.join(''),
				'</div></div>',
				'<div class="ctrls bottom">',
					'<a id="confirm" href="javascript:;">完成</a>',
				'</div>',
			'</div>'
        ].join('');

        this.element = $(html);
        var firstMonth = this.element.find('.days:first');
        if (firstMonth.find('a:not(.ex)').length == 0) {
            firstMonth.add(firstMonth.prevAll()).remove();
        }
        this.cells = this.element.find('.days a:not(.ex)');

        this.startDate = first;
        this.endDate = end;
        this.todayDate = today;
    },
    hookup: function () {
        if (!this.iscroll) {
            this.iscroll = new iScroll('months');
            this.element
				.on('click', '.days a:not(.ex,.gray)', $.proxy(this, 'onClick'))
				.on('click', '.days a.gray', $.proxy(this, 'onClickEx'))
				.on('click', '#confirm', $.proxy(this, 'onConfirm'));
        }
        setTimeout($.proxy(this.iscroll, 'refresh'), 200);
    }
};

/////////////
/// for web

Calendar.ctrip = function (elem, options, onChange) {
    var configs = {
        options: options,
        listeners: {
            onChange: onChange || $.noop
        },
        festival: {},
        monthWidth: 270,
        classNames: {
            select: 'selectstart',
            nothismonth: 'lastday',
            blankdate: 'notselect',
            today: 'todayyes',
            tomorrow: '',
            aftertomorrow: '',
        },
        template: {
            styles: '\n',
            wrapper: [
				'<div class="calendar1">',
					'<div class="row" id="${uid}"></div>',
					'<a href="javascript:;" class="btn-prev" data-bind="prev"></a>',
					'<a href="javascript:;" class="btn-next" data-bind="next"></a>',
				'</div>'
            ].join(''),
            calendar: [
				'<div class="col-xs-6">',
					'<div class="row">',
						'<div class="col-xs-12 text-center month">${title}</div>',
					'</div>',
					'<table cellspacing="0" cellpadding="0" border="0" class="calendar1table">',
						'<tr><th>日</th><th>一</th><th>二</th><th>三</th><th>四</th><th>五</th><th>六</th></tr>',
						'{{each week}}',
						'<tr>',
							'{{each day}}',
							'<td ${classes} ${attr}>${day}</td>',
							'{{/each}}',
						'</tr>',
						'{{/each}}',
					'</table>',
				'</div>'
            ].join('')
        }
    };
    cQuery(elem).regMod('calendar', '6.0', configs);
};

Calendar.ctripTwins = function (elIn, elOut, calendarOptions, onChange) {
    Calendar.ctrip(elIn, Calendar.ctripOptions(calendarOptions, {
        nextEl: elOut,
    }, false), function () {
        var el = $(elOut), ref = Calendar.parse($(elIn).val());
        if (ref && Calendar.parse(el.val()) <= ref) {
            el.val(Calendar.format(new Date(ref.getTime() + 86400000)));
        }
    });
    Calendar.ctrip(elOut, Calendar.ctripOptions(calendarOptions, {
        reference: elIn
    }, true), function () {
        onChange && onChange({
            checkIn: $(elIn).val(),
            checkOut: $(elOut).val()
        });
    });
    showSpinner.prefetch();
};

Calendar.ctripOptions = function (calendarOptions, extra, forCheckOut) {
    var options = calendarOptions || {
        start: '#',
        end: null,
        grayDays: []
    };
    var ret = {
        minDate: options.start,
        maxDate: options.end,
        prohibit: options.grayDays,
        showWeek: false,
        step: 2
    };
    var adjust = function (dateStr, days) {
        return Calendar.format(new Date(Calendar.parse(dateStr).getTime() + days * 86400000));
    };
    if (forCheckOut && ret.minDate != '#') {
        var array = [], dict = {};
        $.each(ret.prohibit, function () {
            var t = adjust(this, 1);
            if (!dict[t]) {
                dict[t] = 1;
                array.push(t);
            }
        });
        ret.prohibit = array;
        ret.maxDate = adjust(ret.maxDate, 1);
        delete ret.minDate;
    }
    return $.extend(ret, extra);
}