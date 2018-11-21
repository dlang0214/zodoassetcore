// 默认
$(function() {
    // 下拉按钮
    $('.btn-dropdown').on('mouseenter', function() {
        console.log('sadf')
        var dl = $(this).next('.dropdown-children');
        dl.fadeIn();
    }).on('mouseleave', function() {
        var dl = $(this).next('.dropdown-children');
        dl.hide();
    });

    $('.dropdown-children').on('mouseenter', function() {
        $(this).show();
    }).on('mouseleave click', function() {
        $(this).fadeOut();
    });

    // 选择框
});

// 弹窗选择