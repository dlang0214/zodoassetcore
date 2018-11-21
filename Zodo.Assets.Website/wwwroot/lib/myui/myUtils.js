// 封装layer弹窗
// 弹窗打开一个页面，iframe
openIframe = function (options) {
    Loading(true);
    var defaults = {
        id: null,
        title: '系统窗口',
        width: 100,
        height: 100,
        url: '',
        shade: 0.3,
        btn: ['确认', '关闭'],
        callBack: null
    };
    var options = $.extend(defaults, options);
    var _width = ($(window).width() > defaults.width) ? defaults.width : $(window).width();
    var _height = ($(window).height() > defaults.height) ? defaults.height : $(window).height();
    top.layer.open({
        id: defaults.id,
        type: 2,
        shade: defaults.shade,
        title: defaults.title,
        fix: false,
        area: [_width + 'px', _height + 'px'],
        content: defaults.url,
        btn: defaults.btn,
        yes: function () {
            defaults.callBack(defaults.id)
        }, cancel: function () {
            if (defaults.cancel != undefined)
            {
                defaults.cancel();
            }
            return true;
        }
    });
}

// 弹窗提，文本或者页面dom
dialogContent = function (options) {
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        content: '',
        btn: ['确认', '关闭'],
        callBack: null
    };
    var options = $.extend(defaults, options);
    top.layer.open({
        id: defaults.id,
        type: 1,
        title: defaults.title,
        fix: false,
        area: [defaults.width + 'px', defaults.height + 'px'],
        content: defaults.content,
        btn: defaults.btn,
        yes: function () {
            defaults.callBack(defaults.id)
        }
    });
}

// 弹窗提醒，文本
dialogAlert = function (content, icon) {
    top.layer.alert(content, {
        icon: icon,
        title: "提醒"
    });
}

// 确认狂
dialogConfirm = function (content, callBack) {
    top.layer.confirm(content, {
        icon: 7,
        title: "提示",
        btn: ['确认', '取消'],
    }, function () {
        callBack(true);
    }, function () {
        callBack(false)
    });
}

// 提醒文本，自动关闭
dialogMsg = function (content, icon) {
    top.layer.msg(content, { icon: icon, time: 4000, shift: 5 });
}

// 关闭弹窗
dialogClose = function () {
    try {
        var index = top.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        var $IsdialogClose = top.$("#layui-layer" + index).find('.layui-layer-btn').find("#IsdialogClose");
        var IsClose = $IsdialogClose.is(":checked");
        if ($IsdialogClose.length == 0) {
            IsClose = true;
        }
        if (IsClose) {
            top.layer.close(index);
        } else {
            location.reload();
        }
    } catch (e) {
        alert(e)
    }
}