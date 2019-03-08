$(function () {
	$('div.visible-xs button:contains(返回), div.back a:has(img)').click(function () {	   
	        history.go(-1);
	});
});


function endWith(orgStr, str) {
    if (str == null || str == "" || orgStr.length == 0 || str.length > orgStr.length)
        return false;
    if (orgStr.substring(orgStr.length - str.length) == str)
        return true;
    else
        return false;
    return true;
}

function showSpinner(b) {
	if (b) {
		$('<div class="spinner dark"><a></a></div>').appendTo(document.body).find('a').css('opacity', 0).animate({opacity: 1}, 400);
	} else {
		$('body > div.spinner').remove();
	}
}
showSpinner.prefetch = function () {
	window.contentUrl && (new Image().src = contentUrl + "/images/spinner.gif");
};

ActionSheet = {
    show: function (elem, onshow, onhide, anchor, exShowObj) {
		var self = this;
		self.onhide = onhide;
		self.div = $('<div class="actionsheet">')
			.appendTo('body')
			.on('click', function (e) {
				e.target == self.div[0] && self.hide();
			});

		if (anchor && anchor.is(':visible')) {
			elem.addClass('anchored')
				.css({
					left: anchor.offset().left,
					top: anchor.offset().top + anchor.outerHeight(),
				});
			self.div.css('opacity', 0.1);
		} else {
			self.div.css('opacity', '')
		}
		self.elem = elem.show().appendTo(document.body);
		$(window).on('resize.actionsheet', function () {
			self.hide();
		});
		onshow && onshow();

		self.exShowObj = exShowObj;
	},

    hide: function () {
        this.div.remove();
		this.elem.detach();
		$(window).off('resize.actionsheet');

	    try {
	        this.exShowObj.hide();
	    } catch (e) {

	    }
	}
};

//redirect http://www.zmjiudian.com
function redirectWww()
{
    var href = location.href.toLocaleLowerCase(); //href = "http://zmjiudian.com";
    var http = "http://";
    var www = "www.";
    var sitebase = "zmjiudian.com";
    var site = "www.zmjiudian.com";

    href = href.replace(http, "");
    if (href.indexOf(www) < 0 && href.indexOf(sitebase) >= 0) {
        location.href = http + www + href;
    }
}