
function validateDates(dict, silent) {
	var d1 = Calendar.parse(dict.checkIn),
		d2 = Calendar.parse(dict.checkOut);
	if (!d1 || !d2) {
		silent || alert('输入日期格式有误');
		return false;
	}
	if (d1 >= d2) {
		silent || alert('入住时间应早于离店时间');
		return false;
	}
	return true;
}

window.isMobile && $(function () {
	// main calendar
	var checkIn = Calendar.parse($('#checkIn').val());
	var checkOut = Calendar.parse($('#checkOut').val());
	var onSelect = function (newCheckIn, newCheckOut) {
		if (newCheckIn - checkIn || newCheckOut - checkOut) {
			location.search = '?' + $.param({
				checkIn: Calendar.format(newCheckIn),
				checkOut: Calendar.format(newCheckOut) 
			});
			$('<div class="spinner dark"><a></a></div>').appendTo(document.body).find('a').css('opacity', 0).animate({opacity: 1}, 400);
		}
	};
	var calendar = null;
	$('#dates').on('click', function () {
		calendar || (calendar = new Calendar(onSelect, window.calendarOptions));
		calendar.show();
		calendar.selectRange(checkIn, checkOut);
	});

	// package calendar
	$('.btn.unavailable').click(function () {
		showSpinner(true);
		$.getJSON($(this).data('calendarOptions'), function (dict) {
			var calendar = new Calendar(function (cIn, cOut) {
				location.search = '?' + $.param({
					checkin: Calendar.format(cIn),
					checkout: Calendar.format(cOut)
				});
			}, dict);
			calendar.show();
		}).fail(function () {
			alert('网络请求失败，请稍后重试');
		}).always(function () {
			showSpinner(false);
		});
	});
});

window.isMobile || $(function () {
	// main calendar
	Calendar.ctripTwins('#checkIn', '#checkOut', window.calendarOptions);

	$('#confirmChange').on('click', function () {
		var dict = {
			checkIn: $('#checkIn').val(),
			checkOut: $('#checkOut').val() 
		};
		if (validateDates(dict)) {
			showSpinner(true);
			location.search = '?' + $.param(dict);
		}
	});

	window.calendarOptions && !function () {
		var remarks = Calendar.getRemarks(calendarOptions);
		if (remarks.length) {
			remarks.unshift('');
			$('#remarks').text(remarks.join(' * '));
		}
	}();

	// package calendar
	$('.btn.unavailable').click(function () {
		showSpinner(true);
		var request = $.getJSON($(this).data('calendarOptions')).done(function (data) {
			showSpinner(false);
			showDialog();
			initCalendar(data);
			$('#packageCheckIn').focus();
		}).fail(function () {
			alert('网络请求失败，请稍后重试');
		});
		var showDialog = $.proxy(bootbox, 'dialog', {
			title: "查看可售日",
			message: [
				'<form class="form-inline" role="form">',
					'<div class="form-group" style="margin-right:15px">',
						'<label>入住时间</label>',
						'<input id="packageCheckIn" type="text" class="form-control">',
					'</div>',
					'<div class="form-group" style="margin-right:15px">',
						'<label>离店时间</label>',
						'<input id="packageCheckOut" type="text" class="form-control">',
					'</div>',
				'</form>'
			].join(''),
			buttons: {
				main: {
					label: "确定",
					className: "btn-primary",
					callback: function() {
						var dict = {
							checkIn: $('#packageCheckIn').val(),
							checkOut: $('#packageCheckOut').val()
						};
						if (validateDates(dict)) {
							showSpinner(true);
							location.search = '?' + $.param(dict);
						} else {
							return false;
						}
					}
				}
			},
			animate: false
		});
		var initCalendar = function (data) {
			Calendar.ctripTwins('#packageCheckIn', '#packageCheckOut', data);
		};
	});
});

showSpinner.prefetch();
