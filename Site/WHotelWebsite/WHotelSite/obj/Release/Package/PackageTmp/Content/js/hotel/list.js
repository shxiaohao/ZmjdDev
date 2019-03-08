$(function () {
    var $page = $('.pager1');
    srollpage($page,function(content) {
        $page.before(content);
    });

    $('.text-center').on('click', '.more', function () {
        var $el = $(this);
        $el.text('收起').removeClass('more').addClass('less').blur();
        var $dl = $el.parent().prev();
        $dl.css('height', 'auto');
    });

    $('.text-center').on('click', '.less', function () {
        var $el = $(this);
        $el.text('更多主题').removeClass('less').addClass('more').blur();
        var $dl = $el.parent().prev();
        $dl.css('height', '210px');
    });
});


