function srollpage($page,insertfunc) {
    var $win = $(window);
    var $doc = $(document);
    var isload = false;
    if (!!window.isMobile) {
        $page.hide();
        $win.on('scroll', function () {
            var $current = $page.find('.active');
            var $next = $current.next('.item');
            if ($next.length) {
                var nextUrl = $next.find('a').attr('href');
                var winTop = $win.scrollTop();
                var winHeight = $win.height();
                var docHeight = $doc.height();
                var bufferHeight = $(window).height();
                if (nextUrl && winTop > docHeight - winHeight - bufferHeight) {
                    if (!isload) {
                        isload = true;
                        $.get(nextUrl, function (content) {
                            insertfunc(content);
                            $current.removeClass('active');
                            $next.addClass('active');
                            isload = false;
                        });
                    }
                }
            }
        });
    }
}

