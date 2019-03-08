$(function () {
    var $page = $('.pager1');
    srollpage($page, function (content) {
        $('.comment-list').append(content);
    });
});


