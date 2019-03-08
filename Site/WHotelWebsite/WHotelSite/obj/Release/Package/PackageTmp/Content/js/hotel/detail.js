// Change Dates
window.isMobile || $(function () {
	Calendar.ctripTwins('#checkIn', '#checkOut', window.calendarOptions, function (data) {
		if (Calendar.parse(data.checkOut) > Calendar.parse(data.checkIn)) {
			showSpinner(true);
			location.search = '?' + $.param(data);
		}
	});

	showSpinner.prefetch();
});
