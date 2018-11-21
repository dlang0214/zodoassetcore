; (function ($, w, layer) {
    w.myUI = {
        ajaxFailHandler: function (response) {
            switch (response.code) {
                case 1001:
                    // 未登录
                    layer.confirm('您尚未登录或登录信息已过期，请重新登录', {
                        btn: ['重新登录', '忽略']
                    }, function () {
                        top.location.href = "~/login";
                    });
                    break;
                case 1002:
                    // 无权限
                    layer.confirm('您已登录，但无权访问此接口，请使用其他账号登录或联系管理员', {
                        btn: ['重新登录', '忽略']
                    }, function () {
                        top.location.href = "~/login";
                    });
                    break;
                case 1003:
                    // 无数据权限
                    layer.alert('您无权查看此数据:' + response.message, {
                        icon: 2
                    });
                    break;
                case 2001:
                    // 没找到页面，应该不会出现在这里
                    layer.alert('您请求的接口不存在<br>' + response.message, {
                        icon: 2
                    });
                    break;
                case 2002:
                    // 请求数据不存在
                    layer.alert('您请求的数据不存在或已删除<br>' + response.message, {
                        icon: 2
                    });
                    break;
                case 3001:
                    // 请求参数不符
                    layer.alert('您提交的数据不合法，请检查后重试<br>' + response.message, {
                        icon: 2
                    });
                    break;
                case 4001:
                    // 提交的数据验证失败
                    layer.alert('您提交的数据验证失败，请检查后重试<br>' + response.message, {
                        icon: 2
                    });
                    break;
                default:
                    // 其他错误
                    layer.alert('未知异常<br>' + response.message, {
                        icon: 2
                    });
                    break;
            }
        },
        ajaxErrorHandler: function () {
            layer.alert('网络异常，请稍候再试！若此问题出现多次，请联系管理员', {
                icon: 2
            });
        },
        initForm: function (opts) {
            var set = {
                formDom: $('form').first(),
                submitButtonDom: $('input[type="submit"]'),
                closeSelfDom: $('#btn-closeSelf'),
                pageType: 'pop',
                beforeSubmit: null,
                beforeSendFn: null,
                successFn: null,
                errorFn: null
            };

            $.extend(set, opts);

            set.submitButtonDom.on('click', function (e) {
                // 阻止form的submit动作
                e.preventDefault();
                // 执行前置函数
                if (set.beforeSubmit && $.isFunction(set.beforeSubmit)) {
                    set.beforeSubmit();
                }
                // 验证form
                if (!set.formDom.Validform()) {
                    return $(this).removeAttr('disabled');
                }
                if (set.msg) {
                    if (!confirm(set.msg)) {
                        return false;
                    }
                }
                // 设置按钮为disabled
                $(this).attr('disabled', 'disabled');
                // 执行ajax请求
                var url = set.formDom.attr('action');
                var self = $(this);
                $.ajax({
                    url: url,
                    data: set.formDom.serialize(),
                    type: 'post',
                    beforeSend: function () {
                        if (set.beforeSendFn && $.isFunction(set.beforeSendFn)) {
                            set.beforeSendFn();
                        }
                    },
                    success: function (response) {
                        if (response.code === 200) {
                            // 操作成功
                            if (set.successFn && $.isFunction(set.successFn)) {
                                set.successFn(response);
                            } else if (set.pageType === 'pop') {
                                parent.layer.closeAll();
                                parent.grid.reload();
                            } else if (set.pageType === 'blank') {
                                window.opener.grid.reload();
                                window.close();
                            } else if (set.pageType === 'tab') {
                                if (window._winCaller && window._winCaller.grid) {
                                    top.tabs.close(window.name.substring(6));
                                    window._winCaller.grid.reload();
                                }
                            } else {
                                layer.alert('操作成功');
                            }
                        } else {
                            myUI.ajaxFailHandler(response);
                            if (set.errorFn && $.isFunction(set.errorFn)) {
                                set.errorFn();
                            }
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        myUI.ajaxErrorHandler(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                        self.removeAttr('disabled');
                    }
                });
            });

            set.closeSelfDom.on('click', function () {
                if (set.pageType == 'pop') {
                    parent.layer.closeAll();
                }
            });
        },
        closeSelf: function (type) {
            if (type == 'tab') {
                top.tabs.close(window.name.substring(6));
            } else if (type == 'blank') {
                window.close();
            } else {
                window.parent.layer.closeAll();
            }
        },
        popDiv: function (title, domId, width, height) {
            var w = width || maxPopWidth;
            var h = height || maxPopHeight;
            var t = title || '';

            layer.open({
                type: 1,
                title: t,
                area: [w + 'px', h + 'px'],
                // skin: 'layui-layer-nobg',
                content: $('#' + domId)
            });
        },
        popIframe: function (url, title, width, height, needValue) {
            var w = width || 600;
            var h = height || 360;
            var t = title || '--';

            if (needValue) {
                var id = $('#grid').getSelectedValue();
                if (!id) {
                    return layer.msg('请选择要处理的数据');
                } else {
                    url += '/' + id;
                }
            }

            layer.open({
                type: 2,
                title: t,
                area: [w + 'px', h + 'px'],
                content: url
            });
        }
    };
})(jQuery, window, layer);

; (function ($, w, layer) {

    $.fn.customerButton = function () {
        var _this = this;
        
        var opts = _this.data('options') || {};
        var action = opts.action || _this.data('action') || null;

        if (!action) {
            return console.log('必须为按钮指定action属性');
        }

        var type = opts.type || _this.data('type') || 'pop';
        var grid = opts.grid || _this.data('grid') || null;
        var title = opts.title;
        var msg = opts.msg;
        var multi = opts.multi || false;
        var before = opts.before || null;
        var ajaxType = opts.ajaxType || 'post';
        var width = opts.width || 720;
        var height = opts.height || 420;
        var callback = opts.callback || null;
        var data = null;

        // 接口|页面地址url
        function getUrl() {
            var url = '';
            if (!grid) {
                url = action;
            } else {
                var val = w[grid].getSelectedValue();
                if (!val) {
                    layer.msg('请选择要处理的数据');
                    return false;
                } else {
                    url = action + '/' + val;
                }
            }
            var newurl = doBefore(url);
            return newurl;
        }

        // 执行默认动作之前执行指定的方法
        function doBefore(url) {
            if (before && w[before]) {
                return w[before](url);
            }
            return url;
        }

        // 弹窗
        function pop() {
            var url = getUrl();
            if (!url) return false;

            layer.open({
                type: 2,
                content: url,
                title: title || '--',
                area: [width + 'px', height + 'px']
            });
        }

        // 跳转
        function jump() {
            var url = getUrl();
            if (!url) return false;

            w.location.href = url;
        }

        // ajax
        function doAjax() {
            var url = getUrl();
            if (!url) return false;
            console.log(w.rvtToken)
            $.ajax({
                url: url,
                type: ajaxType,
                data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                    if (response.code == 200) {
                        layer.msg('操作成功');
                        if (grid) {
                            w[grid].reload();
                        }
                    } else {
                        w.myUI.ajaxFailHandler(response);
                    }
                },
                error: function () {
                    w.myUI.ajaxErrorHandler();
                }
            });
        }

        // 弹窗确认并执行ajax
        function confirmAndDoAjax() {
            var url = getUrl();
            if (!url) return false;

            layer.confirm(msg, {
                button: ['确认', '取消']
            }, function () {
                $.ajax({
                    url: url,
                    type: ajaxType,
                    data: w.rvtToken,
                    success: function (response) {
                        if (response.code == 200) {
                            layer.msg('操作成功');
                            if (grid) {
                                w[grid].reload();
                            }
                        } else {
                            w.myUI.ajaxFailHandler(response);
                        }
                    },
                    error: function () {
                        w.myUI.ajaxErrorHandler();
                    }
                });
            });
        }

        // 弹窗
        function openWindow() {
            var url = getUrl();
            if (!url) return false;

            //var a = $("<a href='" + url + "' target='_blank'>").get(0);
            //var ev = document.createEvent("MouseEvents");
            //ev.initEvent("click", true, true);
            //a.dispatchEvent(ev);
            var left = (window.screen.availWidth - 1020) / 2;
            var top = (window.screen.availHeight - 620) / 2;
            window.open(url, '', 'toolbar=yes,scrollbars=yes,location=yes,width=1020,height=600,left=' + left +',top=' + top);
        }

        // 弹层
        function openDom() {
            layer.open({
                type: 1,
                title: title,
                area: [width + 'px', height + 'px'],
                content: $('#' + url)
            });
        }

        // 打开新标签
        function openTab() {
            var url = getUrl();
            if (!url) return false;
            top.tabs.open(title, url, w);
        }

        _this.on('click', function (e) {
            if (e && e.preventDefault) {
                e.preventDefault();
            } else {
                window.event.returnValue = false;
                return false;
            }

            switch (type) {
                case 'pop':
                    pop();
                    break;
                case 'jump':
                    jump();
                    break;
                case 'blank':
                    openWindow();
                    break;
                case 'ajax':
                    if (msg) {
                        confirmAndDoAjax();
                    } else {
                        doAjax();
                    }
                    break;
                case 'dom':
                    openDom();
                    break;
                case 'tab':
                    openTab();
                    break;
                default:
                    break;
            }
        });

        return _this;
    }
    
})(jQuery, window, layer);

$(function () {
    var rvt = $('input[name="__RequestVerificationToken"]');
    if (rvt.length > 0) {
        window.rvtToken = { __RequestVerificationToken: rvt.val() };
    }

    $('.btn-customer').each(function () {
        $(this).customerButton();
    });

    // 下拉按钮
    $('.btn-dropdown').on('mouseenter', function () {
        console.log('sadf')
        var dl = $(this).next('.dropdown-children');
        dl.fadeIn();
    }).on('mouseleave', function () {
        var dl = $(this).next('.dropdown-children');
        dl.hide();
    });

    $('.dropdown-children').on('mouseenter', function () {
        $(this).show();
    }).on('mouseleave click', function () {
        $(this).fadeOut();
        });

    // 标签
    $('.tab-title').on('click', function () {
        var index = $(this).data('ref');
        $(this).addClass('active').siblings().removeClass('active');

        $('.tab-content').hide();
        $('#' + index).show();
    });

    // 高级搜索
    $('.search-toggle-button').on('click', function () {
        $('#search-box').toggle();
        window.grid.resetHeight();
    });

    // 弹层选择框
    $(".pop-select-dom").on("dblclick", function () {
        var domId = $(this).data("dom");
        var title = $(this).data("title");
        var w = $(this).data("width");
        var h = $(this).data("height");
        myUI.popDiv(title || "", domId, w, h);
    });

    //　弹层选择框，IFrame
    $(".pop-select-iframe").on("dblclick", function () {
        var url = $(this).data("url");
        var title = $(this).data("title");
        var w = $(this).data("width");
        var h = $(this).data("height");
        myUI.popIframe(url, title || "", w, h);
    });

    // 查看缩略图
    $(".uploader-thumbs").viewer();
});