(function ($) {

    // 交换样式
    $.fn.swapClass = function (c1, c2) {
        return this.removeClass(c1).addClass(c2);
    };

    // 切换样式
    $.fn.switchClass = function (c1, c2) {
        if (this.hasClass(c1)) {
            return this.swapClass(c1, c2);
        }
        else {
            return this.swapClass(c2, c1);
        }
    };

    $.fn.TreeView = function (settings) {
        var _sets = {
            api: null,                      // 数据URL
            type: "GET",                    // 数据请求方式
            param: null,                    // 查询参数

            text: "title",                  // 项目文本
            value: "value",                 // 项目值
            children: "children",           // 下级
            icon: null,                     // 图标
            enabled: "enabled",             // 是否可以选中
            isMulti: false,                 // 是否多选

            values: null,

            isDisabledWhenHasChildren: false,

            successFn: null,

            onClick: null,                  // 单击事件
            onDblClick: null,               // 双击事件
            convertSource: null,            // 处理数据源的函数
            explandLevel: 100,              // 级别小于n的项目展开

            selectModel: [true, true, false, true]  // 选择模式，当项目被选中或取消选中时，如何处理关联的上下级
        };

        $.extend(_sets, settings);

        var selectLinkParent = _sets.selectModel[0];          // 选中时关联选中上级
        var selectLinkChildren = _sets.selectModel[1];        // 选中时关联选中下级
        var unSelectLinkParent = _sets.selectModel[2];        // 取消选中时关联取消上级
        var unSelectLinkChildren = _sets.selectModel[3];      // 取消选中时关联取消下级

        var me = $(this);
        var itemIdx = 0;
        var _flatData = [];
        var _parentData = [];

        setItemClickListener();

        function setMainDom(data) {
            var dt = [];
            dt.push("<dl>");
            $.each(data, function (i, item) {
                setNode(dt, item, 0, -1);
            });
            dt.push("</dl>");
            me.html(dt.join(""));
        }

        function setNode(arr, nodeData, level, parent) {
            level = level || 0;                 // 级别

            var t = nodeData[_sets.text];       //
            var v = nodeData[_sets.value];      //
            var a = nodeData[_sets.enabled];    //
            var c = nodeData[_sets.children];   //

            a = (a == null || a == undefined) ? true : false;

            _flatData[itemIdx] = nodeData;      // 把数据存入数组
            _parentData[itemIdx] = parent;

            if (c && $.isArray(c) && c.length > 0) {
                arr.push("<dt data-id='", itemIdx, "' data-val='", v, "'");
                if (_sets.isDisabledWhenHasChildren || !a) {
                    arr.push(" class='disabled'");
                }
                arr.push("><span class='blank' style='width: ", level * 18, "px'></span>");
                if (_sets.explandLevel && level < _sets.explandLevel) {
                    arr.push("<span class='toggle-button'><i class='sj fa fa-caret-down'></i></span>");
                } else {
                    arr.push("<span class='toggle-button'><i class='sj fa fa-caret-down fa-caret-right'></i></span>");
                }
                if (_sets.isMulti) {
                    arr.push("<input type='checkbox' id='cb", itemIdx, "' value='", itemIdx, "' data-val='", v, "'");
                    if (!a) {
                        arr.push(" class='disabled'");
                    }
                    if (_sets.values && _sets.isMulti) {
                        if ($.inArray(v, _sets.values) >= 0) {
                            arr.push(" checked='checked'");
                        }
                    }
                    arr.push(" /> ");
                } else {
                    arr.push("<i class='fa fa-folder fa-folder-open'></i>");
                }
                arr.push(t);
                arr.push("</dt><dd");
                if (_sets.explandLevel != undefined && level >= _sets.explandLevel) {
                    arr.push(" style='display: none'");
                }
                arr.push(">");
                var sLevel = level + 1;
                var _p = itemIdx;
                itemIdx += 1;
                arr.push("<dl>");
                $.each(c, function (i, node) {
                    setNode(arr, node, sLevel, _p);
                });
                arr.push("</dl>");
                arr.push("</dd>")
            } else {
                arr.push("<dt ", (a ? "" : " class='disabled'"), "data-id='", itemIdx, "' data-val='", v, "'>");
                arr.push("<span class='blank' style='width: ", level * 18, "px;'></span>");
                arr.push("<span>|-</span> ");
                if (settings.isMulti) {
                    arr.push("<input type='checkbox' id='cb", itemIdx, "' value='", itemIdx, "' data-val='", v, "'");
                    if (!a) {
                        arr.push(" class='disabled'");
                    }
                    if (_sets.values && _sets.isMulti) {
                        if ($.inArray(v, _sets.values) >= 0) {
                            arr.push(" checked='checked'");
                        }
                    }
                    arr.push(" /> ");
                } else {
                    arr.push("<i class='fa fa-folder fa-file'></i>");
                }
                arr.push(t);
                arr.push("</dt><dd></dd>");
                itemIdx += 1;
            }
        }

        function setItemClickListener() {
            me.on("click", ":checkbox", function (e) {
                e.stopPropagation();

                if ($(this).prop("checked")) {
                    if (selectLinkChildren) {
                        $(this).parent().next("dd").find(":checkbox").prop("checked", true);
                    }
                    if (selectLinkParent) {
                        var c = $(this).val();
                        var p = _parentData[c];
                        $("#cb" + p).prop("checked", true);
                        while (p > 0) {
                            c = p;
                            p = _parentData[c];
                            $("#cb" + p).prop("checked", true);
                        }
                    }
                } else {
                    if (unSelectLinkChildren) {
                        $(this).parent().next("dd").find(":checkbox").prop("checked", false);
                    }
                    if (unSelectLinkParent) {
                        var c = $(this).val();
                        var p = _parentData[c];
                        $("#cb" + p).prop("checked", false);
                        while (p > 0) {
                            c = p;
                            p = _parentData[c];
                            $("#cb" + p).prop("checked", false);
                        }
                    }
                }
            });

            me.on("click", ".sj", function (e) {
                e.stopPropagation();
                $(this).toggleClass("fa-caret-right").parent().parent().next("dd").toggle();
            });

            me.on("click", "dt:not('.disabled')", function (e) {
                if (!_sets.isMulti) {
                    if (!$(this).hasClass("selected")) {
                        me.find("dt").removeClass("selected");
                        $(this).addClass("selected");
                    }
                    var i = $(this).data("id");
                    var item = _flatData[i];
                    if (_sets.onClick && $.isFunction(_sets.onClick)) {
                        _sets.onClick(item);
                    }
                } else {
                    var cb = $(this).find(":checkbox");
                    cb.trigger("click");
                }
            }).on("dblclick", "dt:not('.disabled')", function (e) {
                if (!_sets.isMulti) {
                    var i = $(this).data("id");
                    var item = _flatData[i];
                    if (_sets.onDblClick && $.isFunction(_sets.onDblClick)) {
                        _sets.onDblClick(item);
                    }
                } else {
                    return false;
                }
            });
        }

        me.flatData = function () {
            return _flatData;
        }

        me.renderDom = function (data) {
            me.data = data;
            setMainDom(data);
        }

        me.render = function () {
            $.ajax({
                url: _sets.api,
                data: _sets.param,
                type: _sets.type,
                success: function (response) {
                    var d;
                    if (_sets.convertSource && $.isFunction(_sets.convertSource)) {
                        d = _sets.convertSource(response);
                    } else {
                        d = response;
                    }
                    me.renderDom(d);
                    if (_sets.successFn && $.isFunction(_sets.successFn)) {
                        _sets.successFn();
                    }
                }
            });
        }

        me.getSelectedItem = function () {
            var result = me.getSelectedItems();
            if (result.length > 0) {
                return result[0];
            } else {
                return null;
            }
        }

        me.getSelectedItems = function () {
            var result = [];
            if (_sets.isMulti) {
                $.each(me.find(":checkbox"), function (idx, item) {
                    if ($(item).prop("checked")) {
                        result.push(_flatData[idx]);
                    }
                });
            } else {
                var dt = me.find("dt.selected");
                if (dt.length > 0) {
                    dt = dt[0];
                    var idx = $(dt).data("id");
                    result.push(_flatData[idx]);
                }
            }
            return result;
        }

        me.setValue = function (val) {
            if (!_sets.isMulti) {
                me.find("dt").removeClass("selected");
                $.each(me.find("dt"), function (idx, item) {
                    if ($(item).data("val") == val) {
                        $(item).addClass("selected");
                    }
                });
            } else {
                if ($.isArray(val)) {
                    $.each(me.find(":checkbox"), function (idx, item) {
                        if ($.inArray($(item).data("val"), val) >= 0) {
                            $(item).prop("checked", true);
                        } else {
                            $(item).prop("checked", false);
                        }
                    });
                } else {
                    $.each(me.find(":checkbox"), function (idx, item) {
                        if ($(item).data("val") == val) {
                            $(item).prop("checked", true);
                        } else {
                            $(item).prop("checked", false);
                        }
                    });
                }
            }
        }

        me.getByValue = function (val) {
            if (!_sets.isMulti) {
                var result = null;
                $.each(_flatData, function (idx, item) {
                    if (item[_sets.value] == val) {
                        result = item;
                        return false;
                    }
                });
                return result;
            }
        };

        me.clearValue = function() {
            if (!_sets.isMulti) {
                me.find("dt").removeClass("selected");
            } else {
                me.find(":checkbox").prop("checked", false);
            }
        }

        return me;
    };

})(jQuery)