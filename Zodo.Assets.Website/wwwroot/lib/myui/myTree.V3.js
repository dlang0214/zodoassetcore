;(function($,layer,w) {
    $.fn.MyTree = function (opt) {
        var self = $(this);
        var id = self.attr("id");
        var sets = {
            selectMode: [true, true, false, true],           // 多选模式。级联选中上级，级联选中下级，级联取消上级，级联取消下级
        };

        $.extend(sets, opt);

        // 数据
        self.dataSource = [];
        self.flattenSource = [];
        self.parentMap = [];

        self.url = self.data("url") || sets.url;
        self.clickFn = sets.clickFn || null;
        self.dblClickFn = sets.dblClickFn || null;
        self.completeFn = sets.completeFn || null;
        self.type = self.data("type") || "get";

        // 监听
        self.on("click", "dt:not('.disabled')", function () {
            //if (self.dblClickFn && $.isFunction(self.dblClickFn)) {
            //    return false;
            //}

            if(sets.isMulti) {
                $(this).find(":checkbox").trigger("click");
            } else {
                var checkType = true;                                   // 点击类型，true为选中，false为取消选中
                if (!$(this).hasClass("selected")) {                     // 若单击的项目已选中
                    self.find(".selected").removeClass("selected");
                    $(this).addClass("selected");                       // 切换样式
                } else {                                                // 若点击的项目未选中
                    if (self.dblClickFn && $.isFunction(self.dblClickFn)) {
                    } else {
                        $(this).removeClass("selected");
                        checkType = false;
                    }
                }

                if (self.clickFn && $.isFunction(self.clickFn)) {        // 执行单击动作
                    if (self.dblClickFn && $.isFunction(self.dblClickFn)) {
                    } else {
                        var idx = Number($(this).data("idx"));
                        var nodeData = self.flattenSource[idx];             // 获取对应的数据项
                        self.clickFn(nodeData, checkType);
                    }
                }
            }
        }).on("dblclick", "dt:not('.disabled')", function(e) {
            e.stopPropagation();
            if (self.dblClickFn && $.isFunction(self.dblClickFn)) {
                var idx = Number($(this).data("idx"));
                var nodeData = self.flattenSource[idx]; 
                self.dblClickFn(nodeData);
            }
        });

        self.on("click", ":checkbox:not('.disabled')", function(e) {
            e.stopPropagation();
            var checkType = $(this).prop("checked");                // 点击方式，true选中，false取消选中

            if(checkType) {                                         // 选中
                if(sets.selectMode[0]) {                            // 如果关联上级
                    var idx = Number($(this).data("idx"));
                    var num = idx;
                    while(num > 0) {
                        var num = self.parentMap[num];
                        $("#" + id + "cb" + num).prop("checked", true);
                    }
                }
                if(sets.selectMode[1]) {
                    $(this).parent().next("dd").find(":input").prop("checked", true);
                }
            } else {
                if(sets.selectMode[2]) {

                }
                if(sets.selectMode[3]) {
                    $(this).parent().next("dd").find(":input").prop("checked", false);
                }
            }

            if(self.clickFn && $.isFunction(self.clickFn)) {        // 执行单击动作
                var idx = Number($(this).data("idx"));
                var nodeData = self.flattenSource[idx];             // 获取对应的数据项
                self.clickFn(nodeData, checkType);
            }
        });

        self.on("click", ".toggle-button", function(e) {
            e.stopPropagation();
            $(this).find("i").first().toggleClass("fa-caret-right");    // 切换三角的方向
            $(this).find("i").last().toggleClass("fa-folder-open");     // 切换三角的方向
            $(this).parent().next("dd").toggle();                       // 切换子项目的显示和隐藏
        });

        // 渲染节点
        function renderNode(isMulti, nodeLevel, parentIndex, nodeData) {
            var nodeHtml = [];

            nodeHtml.push("<dt id='");
            nodeHtml.push(id + "dt" + tempIndex);
            nodeHtml.push("'");
            var className = [];
            if(nodeData.selected) { className.push("selected"); }           // 是否选中
            if(nodeData.disabled) { className.push("disabled"); }           // 是否无效
            nodeHtml.push(className.join(" "));                             // 是否选中
            nodeHtml.push(" data-idx='" + tempIndex + "'");　　　　　　　　　 // 序号
            nodeHtml.push(" data-val='" + nodeData.value + "'>");           // 值

            //deHtml.push("<span class='blank' style='width: " + (18 * nodeLevel) + "px'></span>");
            nodeHtml.push("<span class='toggle-button' style='padding-left: " + (18 * nodeLevel) + "px'>");

            var hasChildren = nodeData.children && nodeData.children.length > 0;
            if(hasChildren) {
                nodeHtml.push("<i class='sj fa fa-caret-down'></i>");
                nodeHtml.push("<i class='fa fa-folder fa-folder-open'></i>")
            } else {
                nodeHtml.push("|- ");
                nodeHtml.push("<i class='fa fa-file'></i>");
            }
            nodeHtml.push("</span>");

            if(isMulti) {
                nodeHtml.push("<input type='checkbox' id='" + (id + "cb" + tempIndex));
                nodeHtml.push("' data-idx='" + tempIndex + "' value='" + nodeData.value + "' /> ");
            }
            nodeHtml.push(nodeData.title);
            nodeHtml.push("</dt>");
            self.parentMap[tempIndex] = parentIndex;
            var tempParentIndex = tempIndex;
            tempIndex += 1;
            var flatNode = {};
            $.extend(flatNode, nodeData);
            if(flatNode.children) { delete(flatNode.children); }
            self.flattenSource.push(flatNode);
            if(hasChildren) {
                nodeHtml.push("<dd><dl>");
                $.each(nodeData.children, function(idx, item) {
                    nodeHtml.push(renderNode(isMulti, nodeLevel + 1, tempParentIndex, item));
                });
                nodeHtml.push("</dl></dd>");
            }
            return nodeHtml.join("");
        }

        // 渲染dom
        var tempIndex = 0;
        self.render = function (data) {
            var nodeLevel = 0;
            var html = [];
            self.dataSource = (data && data.length > 0) ? data : self.dataSource;
            html.push("<dl>");
            $.each(self.dataSource, function(idx, node) {
                html.push(renderNode(sets.isMulti, nodeLevel, -1, node));
            });
            self.html(html.join(""));
            html.push("</dl>");

            if (self.completeFn || $.isFunction(self.completeFn)) {
                self.completeFn();
            }
        }

        // ajax获取数据并渲染
        self.get = function() {
            $.ajax({
                url: self.url,
                data: {},
                type: self.type,
                beforeSend: function() {},
                success: function(data) {
                    if(data.code == 200) {
                        self.dataSource = data.body;
                        self.render();
                    } else if (data.code == 401) {
                        // 未登录
                        layer.alert("您的帐号已过期，请重新登录！", function () {
                            top.location.href = "/Account/Login";
                        });
                    } else if (data.code == 403) {
                        // 无权限
                        layer.alert("您无权使用此功能，如有疑义请联系管理员！");
                    } else {
                        // 其他错误
                        layer.msg(data.message);
                    }
                },
                error: function() {},
                complete: function() {}
            });
        };

        // 获取选中数据
        self.getSelectedItems = function() {
            var result = [];
            if(sets.isMulti) {
                var selectedCbs = self.find(":checkbox:checked");
                $.each(selectedCbs, function(idx, item) {
                    var index = $(item).data("idx");
                    var nodeData = self.flattenSource[index];
                    if(nodeData) {
                        result.push(nodeData);
                    }
                });
            } else {
                var selectedDt = self.find("dt.selected");
                if(selectedDt.length > 0) {
                    var dt = selectedDt.first();
                    var idx = Number(dt.data("idx"));
                    result.push(self.flattenSource[idx]);
                }
            }
            return result;
        }

        // 设置值
        self.setSelectedItems = function(vals) {
            if($.isArray(vals)) {
                $.each(self.flattenSource, function(idx, item) {
                    if($.inArray(item.value, vals) >= 0) {
                        if(sets.isMulti) {
                            $("#" + id + "cb" + idx).prop("checked", true);
                        } else {
                            $("#" + id + "dt" + idx).trigger("click");
                        }
                    }
                });
            } else {
                $.each(self.flattenSource, function(idx, item) {
                    if(item.value == vals) {
                        if(sets.isMulti) {
                            $("#" + id + "cb" + idx).prop("checked", true);
                        } else {
                            var dom = $("#" + id + "dt" + idx);
                            if(!dom.hasClass("selected")) {
                                $("#" + id + "dt" + idx).trigger("click");
                            }
                        }
                    }
                });
            }
        }

        self.setSelectedItemsByPropName = function (propName, vals) {
            console.log(propName, vals)
            if ($.isArray(vals)) {
                $.each(self.flattenSource, function (idx, item) {
                    if ($.inArray(item[propName], vals) >= 0) {
                        if (sets.isMulti) {
                            $("#" + id + "cb" + idx).prop("checked", true);
                        } else {
                            $("#" + id + "dt" + idx).trigger("click");
                        }
                    }
                });
            } else {
                $.each(self.flattenSource, function (idx, item) {
                    if (item[propName] == vals) {
                        if (sets.isMulti) {
                            $("#" + id + "cb" + idx).prop("checked", true);
                        } else {
                            var dom = $("#" + id + "dt" + idx);
                            if (!dom.hasClass("selected")) {
                                $("#" + id + "dt" + idx).trigger("click");
                            }
                        }
                    }
                });
            }
        }

        self.getItemByValue = function (val) {
            var result = null;
            $.each(self.flattenSource, function (idx, item) {
                if (item.value == val) {
                    result = item;
                    return false;
                }
            });
            return result;
        }

        var autoLoad = self.data("auto");
        if (autoLoad) {
            self.get();
        }

        return self;
    };
}) (jQuery, layer, window);