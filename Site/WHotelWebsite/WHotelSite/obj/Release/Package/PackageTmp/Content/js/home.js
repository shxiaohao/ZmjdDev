$(function () {
    var citySuggestTemplate = _.template($("#city-suggest-template").html());
    var interestSuggestTemplate = _.template($("#interest-suggest-template").html());
    var $citySuggest = $('.city-suggest');
    var $citySuggestControl = $('#citySuggestControl1');
    var $citySuggestControl2 = $('#citySuggestControl2');
    var $citySuggestControl3 = $('#citySuggestControl3');
    var $interestSuggest = $('.interest-suggest');
    var $interestSuggestControl = $('.interestSuggestControl');
    var $search = $('#search');
    var $dropSuggest = $('.custom-dropdown-menu');
    var dropSuggestTemplate = _.template($("#drop-suggest-template").html());
    var mobileCityTemplate = _.template($("#mobile-city-template").html());
    var $selectcity = $('.selectcity');
    var city = [];

    $.getJSON('/Portal/GetDistrictSuggest', function (citySuggest) {
        var list = _.rest(citySuggest);
        _.each(list, function (o) {
            city = city.concat(o.List);
        });
        $citySuggest.html(citySuggestTemplate({ citySuggest: citySuggest }));
        $selectcity.html(mobileCityTemplate({ citySuggest: citySuggest }));
    });

    $selectcity.on('click', '.titlelink', function () {
        var $el = $(this);
        var $next = $el.next('.l3');
        $('.l3').each(function () {
            if (!$(this).is($next)) {
                $(this).hide();
            }
        });
        $next.toggle();
    });

    $.getJSON('/Portal/InterestListSelect', { districtId: 0, lat: 31.2303924560547, lng: 121.473701477051 }, function (interestSuggest) {
        $interestSuggest.html(interestSuggestTemplate({ interestSuggest: interestSuggest }));
    });


    $search.on('click', function () {
        var districtId = $search.data('districtid');
        var interestId = $search.data('interestid');
        var sctype = $search.data('sctype');
        var type = $search.data('type');
        var url = '';
        if (+sctype == 2) {
            url = "/zone" + districtId;
        } else {
            url = "/city" + districtId;
        }
        if (type == "sight" && +interestId > 0) {
            url += "/sight" + interestId;
        } else if (+interestId > 0) {
            url += "/theme" + interestId;
        }
        location.href = url;
    });
    var $suggest = $('.suggest');
    $suggest.on('click', ' .close', function () {
        console.log($suggest);
        $suggest.hide();
    });

    $suggest.on('click', '.tab li', function () {
        var $el = $(this);
        $el.siblings('.active').removeClass('active');
        $el.addClass('active');
        var i = $el.index();
        var $item = $('.item:eq(' + i + ')', $suggest);
        $item.siblings('.active').removeClass('active');
        $item.addClass('active');
    });

    function suggest($control, $container) {
        var hide = function () {
            $container.hide();
        }
        $container.hover(function () {
            $control.off('blur', hide);
        }, function () {
            $control.on('blur', hide);
        });

        $control.on('focus', function () {
            $('.suggest').hide();
            var offset = $control.offset();
            $container.css({ 'left': offset.left, 'top': offset.top + $control.outerHeight() + 1 });
            $container.show();
            $container.data('target', $control.attr('id'));
        }).on('blur', hide);
    }

    $('.citySuggestControl').each(function () {
        suggest($(this), $citySuggest);
    });

    suggest($interestSuggestControl, $interestSuggest);

    $interestSuggest.on('click', function () {
        //$interestSuggestControl.focus();
    });

    $citySuggest.on('click', function () {
        var id = $citySuggest.data('target');
        var $control = $('#' + id);
        //$control.focus();
    });

    $dropSuggest.on('click', function () {
        var id = $citySuggest.data('target');
        var $control = $('#' + id);
        $control.focus();
    });

    $interestSuggest.on('click', 'li[data-interestid]', function () {
        var $el = $(this);
        var interestid = $el.data('interestid');
        var type = $el.data('type');
        $search.data('interestid', interestid);
        $search.data('type', type);
        $interestSuggestControl.val($el.text());
        $interestSuggest.hide();
        return false;
    });

    $('select.J-change-city-top').on('change', function () {
        var $el = $(this);
        var districtId = $el.val();
        var option = $el.find("option:selected");
        var lat = option.attr("data-lat");
        var lng = option.attr("data-lng");
        var onlyDistrctPre = option.text().indexOf('周边') == -1 ? "d_" : "";
        $.get('/Portal/InterestList', { districtId: districtId, lat: lat, lng: lng, category: onlyDistrctPre + 'zbzt' }, function (content) {
            $('.interest-list', '.zbzt').fadeOut(function () {
                $(this).replaceWith(content).fadeIn();
            });
        });
        $.get('/Portal/InterestList', { districtId: districtId, lat: lat, lng: lng, category: onlyDistrctPre + 'jqjd' }, function (content) {
            $('.interest-list', '.jqjd').fadeOut(function () {
                $(this).replaceWith(content).fadeIn();
            });
        });
    });

    $('img[data-original]').lazyload({
        event: 'scrollstop'
    });

    function citykeyup() {
        var $el = $(this);
        var keyword = _.str.trim($el.val()).toUpperCase();
        if (!keyword) {
            $dropSuggest.hide();
            $citySuggest.show();
            return;
        }
        var hide = function () {
            $dropSuggest.hide();
        }
        $dropSuggest.hover(function () {
            $el.off('blur', hide);
        }, function () {
            $el.on('blur', hide);
        });
        $citySuggest.hide();

        var ret = [];

        var headers = city.filter(function (o) {
            var header = _.map(o.PinYin.match(/(\w+)/g), function (a) { return a[0]; }).join('');
            return _.str.startsWith(header.toUpperCase(), keyword);
        });
        if (headers.length > 0) {
            ret = ret.concat(headers);
        }

        var pinyins = city.filter(function (o) {
            return !_.some(ret, function (a) { return a.Districtid == o.Districtid; }) && _.str.startsWith(o.PinYin.replace(/\s/g, '').toUpperCase(), keyword);
        });
        if (pinyins.length > 0) {
            ret = ret.concat(pinyins);
        }

        var names = city.filter(function (o) {
            return !_.some(ret, function (a) { return a.Districtid == o.Districtid; }) && _.str.startsWith(o.Name, keyword);
        });
        if (names.length > 0) {
            ret = ret.concat(names);
        }

        ret = _.first(ret, 10);
        if (ret && ret.length) {
            var offset = $el.offset();
            $dropSuggest.html(dropSuggestTemplate({ items: ret }));
            $dropSuggest.css({ 'left': offset.left, 'top': offset.top + $el.outerHeight() + 1 });
            $dropSuggest.children('li').css('width', $el.outerWidth());
            $dropSuggest.show();
        } else {
            $dropSuggest.hide();
        }
        $dropSuggest.data('target', $el.attr('id'));
    }
    var throttled = _.throttle(citykeyup, 300);
    $citySuggestControl.on('keyup', throttled);
    $citySuggestControl2.on('keyup', throttled);
    $citySuggestControl3.on('keyup', throttled);

    suggestClick($dropSuggest, '[data-districtId]');
    suggestClick($citySuggest, '[data-districtId]');
    function suggestClick($p, selector) {
        $p.on('click', selector, function () {
            var $el = $(this);
            var districtId = $el.data('districtid');
            var lat = $el.data('lat');
            var lng = $el.data('lng');
            var sctype = $el.data('sctype');
            var id = $p.data('target');


            if (!id) {
                return null;
            }
            var $control = $('#' + id);
            var text = $el.text();
            $p.hide();
            if (id == 'citySuggestControl1') {
                $search.data('districtid', districtId).data('sctype', sctype).data('interestid', 0);
                setTimeout(function () {
                    if (sctype == 2) {
                        districtId = 0;
                    }
                    $.get('/Portal/InterestListSelect', { districtId: districtId, lat: lat, lng: lng, category: category }, function (interestSuggest) {
                        $interestSuggest.html(interestSuggestTemplate({ interestSuggest: interestSuggest }));
                        var $items = $('.item', $interestSuggest);
                        $items.each(function (i, o) {
                            if ($('li', $(o)).length <= 1) {
                                $interestSuggest.find('.tab li:eq(' + i + ')').removeClass('active').hide();
                                $interestSuggest.find('.item:eq(' + i + ')').hide();
                                $interestSuggest.find('.tab li:eq(' + (i == 1 ? 2 : 1) + ')').addClass('active');
                                $interestSuggest.find('.item:eq(' + (i == 1 ? 2 : 1) + ')').show();
                            }
                        });
                    });
                }, 1);
                $interestSuggestControl.val('全部主题、景区');
            } else {
                if (text.indexOf('周边') == -1) {
                    text = text + '及周边';
                }
                $control.val($el.text());
                var $container = $control.closest('.container');
                var category = $container.hasClass('zbzt') ? 'zbzt' : 'jqjd';
                $.get('/Portal/InterestList', { districtId: districtId, lat: lat, lng: lng, category: category }, function (content) {
                    $('.interest-list', $container).fadeOut(function () {
                        $(this).replaceWith(content).fadeIn();
                    });
                });
            }
            $control.val(text);
            return false;
        });
    }

    $('.nav-tabs2').on('click', 'li', function () {
        var $el = $(this);
        var $target = $($el.data('target'));
        $el.siblings('li').removeClass('active').end().addClass('active');
        $target.siblings('div').addClass('hidden').end().removeClass('hidden');
    });

    var arr7 = $('.jqlist img:first').attr('src');
    var arr6 = arr7.replace('icon-aarow7.png', 'icon-aarow6.png');

    $('.jqlist').on('click', 'a', function () {
        var $el = $(this);
        var $img = $('.mobilenav img', $el);
        $('.mobilenav img').each(function() {
            if ($(this).is($img)) {
                if ($img.attr('src') == arr6) {
                    $img.attr('src', arr7);
                } else {
                    $img.attr('src', arr6);
                }
            } else {
                $(this).attr('src', arr7);
            }
        });

        $el.next('.jdlist').siblings('.jdlist').hide().end().toggle();
    });

    $('.listtitle').on('click', function () {
        $selectcity.toggle();
        $('.mobilesearchhotel').hide();
    });

    $('.btn-img01').on('click', function () {
        $('.mobilesearchhotel').toggle();
        $selectcity.hide();
    });

    $('.btn01').on('click', function () {
        var keyword = $('.input01').val();
        if (keyword) {
            $.get('/Portal/SearchList', { keyword: keyword }, function (html) {
                $('.mobilesearchhotel .quicklist').html(html);
            });
        }
    });    
})