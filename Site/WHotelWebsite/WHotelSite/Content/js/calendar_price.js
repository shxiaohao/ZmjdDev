/////////////
/// for mobile

function Calendar(onselect, options, onselectRange, onselectSingle, exShowObj, showVipPrice) {

    this.showVipPrice = showVipPrice;

    this.options = options;
	this.oneday = 24 * 3600 * 1000;
	this.build();
	this.onselect = onselect;
	this.onselectRange = onselectRange;
	this.onselectSingle = onselectSingle;
	this.exShowObj = exShowObj;
    this.isSingleSelect = false;
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
	    ActionSheet.show(this.element, $.proxy(this, 'hookup'), $.noop, anchor, this.exShowObj);
		this.deselect();

	    try {
	        this.exShowObj.show();
	    } catch (e) {

        }
    },
    showValid: function (anchor) {
        ActionSheet.show(this.element, $.proxy(this, 'hookup'), $.noop, anchor, this.exShowObj);
        this.deselect();

        try {
            this.exShowObj.show();
        } catch (e) {

        }

        //setTimeout(function () {

        //    try {

        //        //滚动到最近一个可用日期的位置
        //        if ($("#months .months_i") && $("#months .valid")) {
        //            var _goScrollTop = $($("#months .valid")[0]).offset().top - ($(window).height() - $(".calendar").height()) - 100 - 200;
        //            //console.log($(window).height())
        //            //console.log($(".calendar").height())
        //            //console.log($($("#months .valid")[0]).offset().top)
        //            //console.log(_goScrollTop)
        //            $("#months .months_i")[0].style = "transition-property: -webkit-transform;transform - origin: 0px 0px 0px;transform: translate3d(0px, -" + _goScrollTop + "px, 0px) scale(1);";
        //        }

        //    } catch (e) {

        //    }

        //}, 0);
    },
	hide: function () {
		this.ondismiss && this.ondismiss();
		ActionSheet.hide(this.element);
		
	    try {
	        this.exShowObj.hide();
	    } catch (e) {

	    }
	},
	select: function (date) {
	    this.cellForDate(date).addClass('on');

	    try {
	        if (this.isSingleSelect) {
	            var _from = new Date(date.getTime());
	            var _to = new Date(date.getTime());
	            _to.setDate(_to.getDate() + 1);
	            this.onselectSingle(_from, new Date(_to));
	        }
	    } catch (e) {

	    }
	},
	selectRange: function (from, to) {
		if (!this.isRangeSelectable(from, to)) {
			return false;
		}
		this.deselect();
		for (var date = new Date(from.getTime()); date <= to; date.setDate(date.getDate() + 1)) {
			this.select(date);
		}
		this.cellForDate(from).addClass('ci');
		this.cellForDate(to).addClass('co');

	    try {
	        this.onselectRange(from, to);
	    } catch (e) {

	    }

		return true;
	},
	deselect: function () {
		this.selectedCells().removeClass('on ci co pre');
	},

	isRangeSelectable: function (from, to) {
		for (var date = new Date(from.getTime()); date < to; date.setDate(date.getDate() + 1)) {
			var cell = this.cellForDate(date);
			if (!cell.length || cell.is('.ex, .gray')) {
				return false;
			}
		}
		return true;
	},
	validateRange: function (from, to) {

	    if (this.isSingleSelect) {
	        return true;
	    }

		var limit = this.options && this.options.limit;
		var error = null;
		if (limit && (limit.min || limit.max)) {
			var delta = Math.round((to - from) / this.oneday);
			if (limit.min == limit.max && delta != limit.min) {
				error = 0;
			} else if (delta < limit.min) {
				error = -1;
			} else if (delta > limit.max) {
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
		
        //单选类型的日期，不需要自动选择
		if (!this.isSingleSelect) {
		    if (this.shouldAutoSelect()) {
		        if (this.autoSelect(clickedDate)) {
		            this.autoScroll(clickedCell);
		            return;
		        }
		    }
		}

		var activeCell = this.activeCell();
		if (activeCell.length) {

		    //如果是单选，则不处理range选择；不能去除选中；
		    if (!this.isSingleSelect) {

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
		}

		this.preSelect(clickedCell);

	    try {
	        if (this.isSingleSelect) {
	            var _from = new Date(clickedDate.getTime());
	            var _to = new Date(clickedDate.getTime());
	            _to.setDate(_to.getDate() + 1);
	            this.onselectSingle(_from, new Date(_to));
	        }
	    } catch (e) {

	    }
	},

	onClickEx: function (e) {
		var selectedCells = this.selectedCells();
		if (!selectedCells.length) {
			return;
		}
		var from = this.dateForCell(selectedCells.eq(0));
		var clickedCell = $(e.target).closest('a');
		var to = this.dateForCell(clickedCell);
		if (from < to && !this.isSingleSelect) {
			this.selectRange(from, to);
		}
	},

	onConfirm: function () {
	    //debugger;
		var selectedCells = this.selectedCells();
		if (selectedCells.length > 0) {
		    //console.log(selectedCells)

		    var selectId = "0";
		    try { selectId = selectedCells.data("id"); } catch (e) { }

			var d1 = this.dateForCell(selectedCells.first()),
				d2 = this.dateForCell(selectedCells.last());
			if (this.validateRange(d1, d2) && this.onselect) {
			    this.onselect(d1, d2, selectId);
				this.hide();
			}
		} else {
		    if (this.isSingleSelect) {
		        alert('请选择一个日期');
		    }
		    else {
		        alert('请选择入住和离店日期');
		    }
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
            priceList = {},
            vipPriceList = {},
            idList = {},
			contents = [];
		if (this.options) {
			first = Calendar.parse(this.options.start);
			end = Calendar.parse(this.options.end);
			$.each(this.options.grayDays, function () {
				grayDays[this] = 1;
			});
            priceList = this.options.priceList;
            vipPriceList = this.options.vipPriceList;
			idList = this.options.idList;
		}
		while (first > today) {
			first.setDate(first.getDate() - 1);
			grayDays[Calendar.format(first)] = 1;
		}
		var start = new Date(first.getTime() - first.getDay() * this.oneday), 
			current = new Date(start.getTime()),
			begin = true;

		//contents.push('<div class="weekdays">',
		//				'<b class="w">日</b><b>一</b><b>二</b><b>三</b><b>四</b><b>五</b><b class="w">六</b>',
		//			  '</div>');

		var dayIndex = 0;
		var idIndex = 0;
		for(; current <= end; current.setDate(current.getDate() + 1)) {
			if (begin || current.getDate() == 1) {
				if (!begin) {
					contents.push('</div>');
				}
				begin = false;
				contents.push(
					'<div class="cap">', current.getFullYear(),'年', current.getMonth() + 1, '月</div>',
					'<div class="days">'
				);
				var padding = new Date(current.getTime() - this.oneday);
				while(padding.getDay() != 6) {
					contents.push('<a class="ex">0</a>');
					padding.setDate(padding.getDate() - 1);
				}
			}
			if (current < today) {
			    contents.push('<a class="ex">');
			    contents.push(current.getTime() == today.getTime() ? '今天' : current.getDate(), '</a>');
			}
			else if (current < first || grayDays[Calendar.format(current)] == 1) {
			    contents.push('<a class="gray">');
			    contents.push(current.getTime() == today.getTime() ? '今天' : current.getDate(), '</a>');

			    dayIndex++;
			    idIndex++;
			}
			else {

			    var dayVal = current.getTime() == today.getTime() ? '今天' : current.getDate();

			    //date id
			    var id = "0";
			    try {
			        //console.log(idIndex);
			        id = idList[idIndex];
			    } catch (e) { }

			    //check day price
			    var sellprice = 0;
                try {
                    if (this.showVipPrice) {
                        sellprice = parseInt(vipPriceList[dayIndex]); //sellprice = 888;
                    }
                    else {
                        sellprice = parseInt(priceList[dayIndex]); //sellprice = 888;
                    }
			    } catch (e) { }
                
			    if (sellprice > 0) {
                    contents.push('<a class="valid pri" href="javascript:;" data-hp="1" data-id="' + id + '" data-price="' + sellprice + '">');
			        contents.push(dayVal);
			        contents.push('</a>');
			    }
			    else {
			        contents.push('<a class="valid" href="javascript:;" data-id="' + id + '">');
			        contents.push(dayVal, '</a>');
			    }

			    dayIndex++;
			    idIndex++;
			}
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
					'<a id="confirm" href="javascript:;">确定</a>',
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

        //set price
		this.element.find('.days a').each(function ()
		{
		    if ($(this).data("hp") == "1") {
		        var priceVal = $(this).data("price");
                var phtml = "<div class='pv'><span>￥</span>" + priceVal + "</div>";
		        $(this).html($(this).html() + phtml);
		    }
		});
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