;(function ($, w, layer) {
    $.fn.MyGrid = function (opts) {
        var _this = this;

        // 要现实的列配置
        var cols = opts.columns;
        // 是否分页
        var isPager = opts.pager == undefined ? false : opts.pager;
        // 是否多选
        var isMulti = opts.multi == undefined ? false : opts.multi;
        // 是否树形
        var isTree = opts.tree == undefined ? false : opts.tree;
        // 值所在的列
        var keyColumn = opts.keyColumn || 'Id';
        // 层级所在的列
        var levelColumn = opts.levelColumn || 'Level';
        // 是否自动加载数据
        var isAuto = opts.auto == undefined ? false : opts.auto;
        // 数据内按钮调用的方法集
        var handlers = opts.handlers;
        // 控件总高度，可以是数值或方法
        var height = opts.height;
        // 单击行事件
        var clickFn = opts.click;
        // 双击行事件
        var dblClickFn = opts.dblClick;
        // 每页显示的数量
        var pageSize = opts.pageSize;
        // ajax获取数据的路径或网址
        var api = opts.api;
        // ajax请求方式
        var type = opts.type || 'get';
        // ajax请求前运行
        var ajaxBeforeSendFn = opts.ajaxBefore || null;
        // ajax请求成功后，开始生成html之前运行
        var dataConvertFn = opts.ajaxSuccess || null;
        // ajax请求成功并渲染html结束后运行
        var renderCompleteFn = opts.renderComplete || null;
        // ajax请求失败
        var ajaxErrorFn = opts.ajaxError || null;
        // ajax请求完成
        var ajaxCompleteFn = opts.ajaxComplete || null;
        // 请求参数
        var filterFn = opts.filterFn || null;

        var data = [];
        var selectedItem = null;
        var selectedItems = [];

        var pager = isPager ? new Pager(_this, pageSize) : null;

        var doms = {
            header: $('<div class="grid-header"></div>'),
            headerTable: $('<table class="table table-striped"></table>'),
            body: $('<div class="grid-body scroll-y"></div>'),
            bodyTable: $('<table class="table table-striped"></table>'),
            left: $('<div class="grid-fixed-left"></div>'),
            right: $('<div class="grid-fixed-right"></div>'),
            leftHeader: $('<div class="fixed-left-header"></div>'),
            rightHeader: $('<div class="fixed-right-header"></div>'),
            leftBody: $('<div class="fixed-left-body scroll-y"></div>'),
            rightBody: $('<div class="fixed-right-body scroll-y"></div>'),
            leftBodyTable: $('<table class="table table-striped left-fixed-body-table"></table>'),
            rightBodyTable: $('<table class="table table-striped right-fixed-body-table"></table>'),
            leftHeaderTable: $('<table class="table table-striped left-fixed-header-table"></table>'),
            rightHeaderTable: $('<table class="table table-striped right-fixed-header-table"></table>'),
            lastCols: null,
            bodyTbody: null,
            leftTbody: null,
            rightTbody: null,
            scrollY: null,
            checkAll: undefined
        };

        var widths = {
            left: 0,
            right: 0,
            bodyTable: 0,
            headerTable: 0,
            lastAutoWidthCol: 0,
            fixedWidthCol: 0
        };

        var heights = {
            total: 0,
            body: 0
        };

        var leftCols = [];
        var rightCols = [];
        var centerCols = [];
        var lastAutoWidthCol = null;
        var hasHScrollBar = false;

        var hasLeft = false;
        var hasRight = false;

        if (isMulti) {
            cols.splice(0, 0, {
                title: '',
                type: 'checkbox',
                width: 40,
                fixed: 'left'
            });
        }

        $.each(cols, function (idx, col) {
            if (!col.width) {
                col.width = 120;
                col.autoWidth = true;
            }
            if (col.fixed == 'left') {
                widths.left += col.width;
                leftCols.push(col);
                hasLeft = true;
            } else if (col.fixed == 'right') {
                widths.right += col.width;
                rightCols.push(col);
                hasRight = true;
            } else {
                lastAutoWidthCol = col;
                centerCols.push(col);
            }
            widths.bodyTable += col.width;
        });

        if (lastAutoWidthCol == null) {
            lastAutoWidthCol = centerCols[centerCols.length - 1];
        }

        lastAutoWidthCol.isLast = true;
        lastAutoWidthCol.origWidth = lastAutoWidthCol.width;

        widths.fixedWidthCol = widths.bodyTable - lastAutoWidthCol.width;

        calcWidth();
        initDom();
        setWidth(true);
        calcHeight();
        setHeight();
        listen();


        // 1. 操作dom
        //    1.1 初始化dom
        function initDom() {
            var leftColGroup = '<colgroup>';
            var rightColGroup = '<colgroup>';
            var centerColGroup = '<colgroup>';
            var leftTHead = '<thead><tr>';
            var rightTHead = '<thead><tr>';
            var centerTHead = '<thead><tr>';

            $.each(leftCols, function (index, col) {
                leftColGroup += '<col width="' + col.width + '" />';
                if (col.type == 'checkbox') {
                    leftTHead += '<th class="content-center"><div class="cell"><input type="checkbox" class="checkall"></div></th>';
                } else {
                    leftTHead += '<th><div class="cell">' + col.title + '</div></th>';
                }
            });

            $.each(rightCols, function (index, col) {
                rightColGroup += '<col width="' + col.width + '" />';
                rightTHead += '<th><div class="cell">' + col.title + '</div></th>';
            });

            $.each(cols, function (index, col) {
                if (col.isLast) {
                    centerColGroup += '<col width="' + col.width + '" class="last-col" />';
                } else {
                    centerColGroup += '<col width="' + col.width + '" />';
                }
                centerTHead += '<th><div class="cell">' + col.title + '</div></th>';
            });

            var centerHeaderColGroup = centerColGroup;
            var centerHeaderTHead = centerTHead;

            centerHeaderColGroup += '<col width="17" />';
            centerHeaderTHead += '<th><div class="cell"></div></th>';

            leftColGroup += '</colgroup>';
            rightColGroup += '</colgroup>';
            centerColGroup += '</colgroup>';
            leftTHead += '</tr></thead>';
            rightTHead += '</tr></thead>';
            centerTHead += '</tr></thead>';
            centerHeaderColGroup += '</colgroup>';
            centerHeaderTHead += '</tr></thead>';

            doms.headerTable.append($(centerHeaderColGroup));
            doms.headerTable.append($(centerHeaderTHead));
            doms.bodyTable.append($(centerColGroup));
            doms.bodyTbody = $('<tbody class="center-tbody"></tbody>');
            doms.bodyTable.append(doms.bodyTbody);

            doms.header.append(doms.headerTable);
            doms.body.append(doms.bodyTable);

            _this.append(doms.header);
            _this.append(doms.body);
            _this.append($('<div class="grid-top-right-mask"></div>'));

            if (hasLeft) {
                doms.leftHeaderTable.append($(leftColGroup));
                doms.leftHeaderTable.append($(leftTHead));
                doms.leftBodyTable.append($(leftColGroup));
                doms.leftTbody = $('<tbody class="left-tbody"></tbody>');
                doms.leftBodyTable.append(doms.leftTbody);

                doms.leftHeader.append(doms.leftHeaderTable);
                doms.leftBody.append(doms.leftBodyTable);

                doms.left.append(doms.leftHeader);
                doms.left.append(doms.leftBody);

                _this.append(doms.left);
            }

            if (hasRight) {
                doms.rightHeaderTable.append($(rightColGroup));
                doms.rightHeaderTable.append($(rightTHead));
                doms.rightBodyTable.append($(rightColGroup));
                doms.rightTbody = $('<tbody class="right-tbody"></tbody>');
                doms.rightBodyTable.append(doms.rightTbody);

                doms.rightHeader.append(doms.rightHeaderTable);
                doms.rightBody.append(doms.rightBodyTable);

                doms.right.append(doms.rightHeader);
                doms.right.append(doms.rightBody);

                _this.append(doms.right);
            }
        }

        //    1.2 控制显示dom
        function dataShow() {
            if(!data || data.length == 0) {
                if(left) doms.left.hide();
                if(right) doms.right.hide();

                doms.bodyTbody.html('<tr class="empty"><td colspan="' + cols.length + '">暂无数据</td></tr>');
            } else {
                if(left) doms.left.show();
                if(right) doms.right.show();
            }
        }

        // 2. 处理宽度
        //    2.1 计算宽度
        function calcWidth() {
            var total = _this.width();
            var temp = total - widths.fixedWidthCol - 18;

            if (temp > lastAutoWidthCol.origWidth) {
                lastAutoWidthCol.width = temp;
                hasHScrollBar = false;
            } else {
                lastAutoWidthCol.width = lastAutoWidthCol.origWidth;
                hasHScrollBar = true;
            }
            widths.lastAutoWidthCol = lastAutoWidthCol.width;
            widths.bodyTable = widths.fixedWidthCol + widths.lastAutoWidthCol;

        }

        //    2.2 设置dom宽度
        function setWidth(isInit) {
            doms.headerTable.css('width', widths.bodyTable + 17 + 'px');
            doms.bodyTable.css('width', widths.bodyTable + 'px');

            doms.left.css('width', widths.left + 1 + 'px');
            doms.right.css('width', widths.right + 1 + 'px');

            if (hasLeft) {
                if (hasHScrollBar) {
                    doms.left.css('bottom', '17px');
                } else {
                    doms.left.css('bottom', '0');
                }
            }

            if (hasRight) {
                if (hasHScrollBar) {
                    doms.right.css('bottom', '17px');
                } else {
                    doms.right.css('bottom', '0');
                }
            }

            if (!isInit) {
                getLastCols().prop('width', lastAutoWidthCol.width);
            }
        }

        // 3. 处理高度
        //    3.1 计算高度
        function calcHeight() {
            if (height) {
                if ($.isNumeric(height)) {
                    if (height > 0) {
                        if (isPager) {
                            heights.body = height - 40 - 2 - 39;
                        } else {
                            heights.body = height - 2 - 39;
                        }
                    } else {
                        heights.body = $(w).height() + height - 2 - 39;
                    }
                } else if ($.isFunction(height)) {
                    if(isPager) {
                        heights.body = height() - 40 - 2 - 39;
                    } else {
                        heights.body = height() - 2 - 39;
                    }
                }
            }

            if (heights.body < 200) {
                heights.body = 200;
            }
        }

        //    3.2 设置dom高度
        function setHeight() {
            doms.body.css('height', heights.body + 'px');
            if (leftCols.length > 0) {
                if (!hasHScrollBar) {
                    doms.leftBody.css('height', heights.body + 'px');
                } else {
                    doms.leftBody.css('height', heights.body - 17 + 'px');
                }
            }
            if (rightCols.length > 0) {
                if (!hasHScrollBar) {
                    doms.rightBody.css('height', heights.body + 'px');
                } else {
                    doms.rightBody.css('height', heights.body - 17 + 'px');
                }
            }
        }

        // 4. 获取dom
        //    4.1 获取.auto-width的<col>元素
        function getLastCols() {
            if (!doms.lastCols) {
                doms.lastCols = _this.find('.last-col');
            }
            return doms.lastCols;
        }

        //    4.2 获取数据表格的tbody
        function getBodyTbody() {
            if (!doms.bodyTbody) {
                doms.bodyTbody = _this.find('.grid-body tbody');
            }
            return doms.bodyTbody;
        }

        //    4.3 获取左侧数据表格的tbody
        function getLeftTbody() {
            if (!doms.leftTbody) {
                doms.leftTbody = _this.find('.grid-fixed-left-table tbody');
            }
            return doms.leftTbody;
        }

        //    4.4 获取右侧数据表格tbody
        function getRightTbody() {
            if (!doms.rightTbody) {
                doms.rightTbody = _this.find('.grid-fixed-right-table tbody');
            }
            return doms.rightTbody;
        }

        //    4.5 获取可以y轴滚动的元素，左中右的body
        function getScrollY() {
            if (!doms.scrollY) {
                doms.scrollY = _this.find('.scroll-y');
            }
            return doms.scrollY;
        }

        //    4.6 获取全选的checkbox
        function getCheckAll() {
            if (doms.checkAll == null) {
                doms.checkAll = _this.find('.checkall');
            }
            return doms.checkAll;
        }

        // 5. 渲染数据
        //    5.1 渲染中间的表格
        function renderRow(rowData, index) {
            var h = '<tr class="tr-' + index + '" data-idx="' + index + '"' + (isTree ? ' data-level="' + rowData['Level'] + '"' : '') + '>';
            $.each(cols, function (idx, col) {
                if (col.fixed == 'left' || col.fixed == 'right') {
                    h += '<td><div class="cell"></div></td>';
                } else {
                    h += renderColumn(col, rowData[col.field], index, rowData['Level'], rowData);
                }
            });
            h += '</tr>';
            return h;
        };

        //    5.2 渲染左侧表格
        function renderLeftRow(rowData, index) {
            var h = '<tr class="tr-' + index + '" data-idx="' + index + '"' + (isTree ? ' data-level="' + rowData[levelColumn] + '"' : '') + '>';
            $.each(leftCols, function (idx, col) {
                h += renderColumn(col, rowData[col.field], index, rowData['Level'], rowData);
            });
            h += '</tr>';
            return h;
        };

        //    5.3 渲染右侧表格
        function renderRightRow(rowData, index) {
            var h = '<tr class="tr-' + index + '" data-idx="' + index + '"' + (isTree ? ' data-level="' + rowData[levelColumn] + '"' : '') + '>';
            $.each(rightCols, function (idx, col) {
                h += renderColumn(col, rowData[col.field], index, rowData['Level'], rowData);
            });
            h += '</tr>';
            return h;
        };

        //    5.4 渲染单元格
        function renderColumn(col, colData, index, level, rowData) {
            var val = '';
            if ($.isNumeric(colData)) {
                val = colData;
            } else {
                val = colData || '';
            }
            var cssName = 'cell';
            var tdClass = col.className ? ' class="' + col.className + '"' : '';
            //if (col.className) {
            //    cssName += ' ' + col.className;
            //}

            if (col.do != undefined) {
                var v = col.do(rowData);
                return '<td' + tdClass + '><div class="' + cssName + '">' + v + '</div></td>';
            }
            switch (col.type) {
                case 'checkbox':
                    cssName += ' content-center';
                    val = '<input type="checkbox" value="' + index + '" class="cb-' + index + '" />';
                    break;
                case 'indexNum':
                    val = ++index;
                    break;
                case 'tree':
                    val = treeText(val, level);
                    break;
                default:
                    break;
            }
            return '<td' + tdClass + '><div class="' + cssName + '">' + val + '</div></td>';
        };

        //    5.5 树形列表的显示
        function treeText(txt, level) {
            if(level && level > 1) {
                var blank = '<span class="inline-blank" style="width: ' + ((level - 1) * 18) + 'px"></span> |- ';
                //blank += '<span class="btn-toggle">+</span>'
                return blank + txt;
            } else {
                return txt;
            }
        }

        // 6. 事件监听
        function listen() {
            // 6.1 监听数据区域的滚动条事件
            doms.body.on('scroll', function () {
                var scrollTop = $(this).scrollTop();
                getScrollY().scrollTop(scrollTop);

                var scrollLeft = $(this).scrollLeft();
                doms.header.scrollLeft(scrollLeft);
            });

            // 6.2 监听数据行点击、hover、双击事件
            _this.on('click', 'tbody tr', function () {
                var idx = $(this).data('idx');
                var d = data[idx];
                selectedItem = d;

                if (!isMulti) {
                    if (!$(this).hasClass('selected')) {
                        _this.find('.tr-' + idx).addClass('selected').siblings().removeClass('selected');
                    }
                } else {
                    var cb = _this.find('.cb-' + idx);
                    cb.trigger('click');
                }

                if (clickFn && $.isFunction(clickFn)) {
                    clickFn(selectedItem);
                }
            }).on('mouseover', 'tbody tr', function () {
                var idx = $(this).data('idx');
                _this.find('.tr-' + idx).addClass('hover').siblings().removeClass('hover');
            }).on('dblclick', 'tbody tr', function () {
                if (isMulti) {
                    return console.log('多选表格不支持双击事件');
                }
                if (dblClickFn && $.isFunction(dblClickFn)) {
                    dblClickFn(selectedItem);
                }
            });

            if (isMulti) {
                // 6.3 监听checkbox的点击事件
                doms.leftTbody.on('click', ':checkbox', function (e) {
                    e.stopPropagation();

                    if (!$(this).prop('checked')) {
                        getCheckAll().prop('checked', false);
                    } else {
                        if (data.length == _this.find(':checked').length) {
                            getCheckAll().prop('checked', true);
                        }
                    }
                });

                // 6.4 监听全选checkbox的点击事件
                getCheckAll().on('click', function () {
                    _this.find('tbody :checkbox').prop('checked', $(this).prop('checked'));
                });
            }

            // 6.5 监听窗口resize事件
            $(w).on('resize', function () {
                calcWidth();
                setWidth(false);
                calcHeight();
                setHeight();
            });

            // 6.6 监听数据内点击事件
            _this.on('click', '.grid-button', function (e) {
                e.stopPropagation();
                var tr = $(this).parents('tr');
                if (!tr) return;
                tr.trigger('click');

                var func = $(this).data('func');
                if (!func || !handlers[func]) return;
                handlers[func](selectedItem);
            });

            // 6.7 监听分页事件
            if (isPager) {
                pager.addEventListener(function () {
                    _this.pull()
                });
            }
        }

        // 7. 分页
        function Pager(domTBody, pagesize, cssClass) {
            this.pageSize = pagesize || 20;
            this.pageCount = 0;
            this.recordCount = 0;
            this.pageIndex = 1;
            var pagerClass = opts.pagerClass || 'grid-pager pager';

            var pagerDom = $('<div class="' + pagerClass + '"></div>');
            domTBody.after(pagerDom);
            var buttondomTBody = $('<div class="buttons"></div>');
            var info = $('<div class="info"></div>');
            pagerDom.append(buttondomTBody);
            pagerDom.append(info);
            var firstBtn = $('<a class="disabled"><i class="fa fa-fast-backward"></i></a>');
            buttondomTBody.append(firstBtn);
            var prevBtn = $('<a class="disabled"><i class="fa fa-backward"></i></a>');
            buttondomTBody.append(prevBtn);
            var nextBtn = $('<a class="disabled"><i class="fa fa-forward"></i></a>');
            buttondomTBody.append('<b>第</b>');
            var currentInput = $('<input value="1" type="text" maxlength="4" />');
            buttondomTBody.append(currentInput);
            buttondomTBody.append('<b>页</b>');
            buttondomTBody.append(nextBtn);
            var lastBtn = $('<a class="disabled"><i class="fa fa-fast-forward"></i></a>');
            buttondomTBody.append(lastBtn);
            var pageSize = $('<select name="pageSize"><option value="20">每页20条</option><option value="30">每页30条</option><option value="50">每页50条</option></select>');
            buttondomTBody.append(pageSize);

            this.addEventListener = function (fn) {
                var that = this;
                firstBtn.on('click', function () {
                    if ($(this).hasClass('disabled')) return;
                    that.pageIndex = 1;
                    currentInput.val(that.pageIndex);
                    fn(that);
                });
                prevBtn.on('click', function () {
                    if ($(this).hasClass('disabled')) return;
                    if (that.pageIndex > 1) {
                        that.pageIndex--;
                    } else {
                        that.pageIndex = 1;
                    }
                    currentInput.val(that.pageIndex);
                    fn(that);
                });
                nextBtn.on('click', function () {
                    if ($(this).hasClass('disabled')) return;
                    that.pageIndex++;
                    if (that.pageIndex > that.pageCount) {
                        that.pageIndex = that.pageCount;
                    }
                    currentInput.val(that.pageIndex);
                    fn(that);
                });
                lastBtn.on('click', function () {
                    if ($(this).hasClass('disabled')) return;
                    that.pageIndex = that.pageCount;
                    currentInput.val(that.pageIndex);
                    fn(that);
                });
                currentInput.on('change', function () {
                    var v = Number($(this).val());
                    if (!v || v < 0 || v > that.pageCount) return $(this).val(that.pageIndex);
                    that.pageIndex = v;
                    currentInput.val(that.pageIndex);
                    fn(that);
                });
                pageSize.on('change', function () {
                    var s = Number($(this).val());
                    if (!s || s < 0) return $(this).val(that.pageSize);
                    that.pageSize = s;
                    pageSize.val(that.pageSize);
                    currentInput.val("1");
                    that.pageIndex = 1;
                    fn(that);
                });
            };

            this.setPager = function (total, start) {
                var that = this;
                this.recordCount = total;
                this.pageCount = Math.ceil(this.recordCount / this.pageSize);
                firstBtn.removeClass('disabled');
                prevBtn.removeClass('disabled');
                nextBtn.removeClass('disabled');
                lastBtn.removeClass('disabled');
                currentInput.val(that.pageIndex);
                if (this.pageIndex === 1) {
                    firstBtn.addClass('disabled');
                    prevBtn.addClass('disabled');
                }
                if (this.pageIndex >= this.pageCount) {
                    nextBtn.addClass('disabled');
                    lastBtn.addClass('disabled');
                }
                var msg = '共计' + this.recordCount + '条记录，每页显示' + this.pageSize + '条，共' + this.pageCount + '页，用时' + (new Date().getTime() - start) + '毫秒';
                info.html(msg);
            };
        }

        // 8. 获取查询参数
        function getFilter() {
            var param;
            if (filterFn && $.isFunction(filterFn)) {
                param = filterFn();
            }

            if ($.isArray(param)) {
                if (isPager) {
                    param.push({
                        'name': 'pageSize',
                        'value': pager.pageSize
                    });
                    param.push({
                        'name': 'pageIndex',
                        'value': pager.pageIndex
                    });
                }
                param.push({
                    'r': Math.random()
                });
            } else if ($.isPlainObject(param)) {
                if (isPager) {
                    $.extend(true, param, {
                        'pageSize': pager.pageSize,
                        'pageIndex': pager.pageIndex,
                        'r': Math.random()
                    });
                } else {
                    $.extend(true, param, {
                        'r': Math.random()
                    });
                }
            } else {
                param = (param ? param + '&r=' : 'r=') + Math.random();
                if (isPager) {
                    param += '&pageSize=' + pager.pageSize + '&pageIndex=' + pager.pageIndex;
                }
            }
            return param || {};
        }

        // 9. 网络请求异常的通用处理
        //    9.1 请求成功，但返回数据异常
        function codeError(response) {
            switch (response.Code) {
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
                    layer.alert('您无权查看此数据', {
                        icon: 2
                    });
                    break;
                case 2001:
                    // 没找到页面，应该不会出现在这里
                    layer.alert('您请求的接口不存在', {
                        icon: 2
                    });
                    break;
                case 2002:
                    // 请求数据不存在
                    layer.alert('您请求的数据不存在或已删除', {
                        icon: 2
                    });
                    break;
                case 3001:
                    // 请求参数不符
                    layer.alert('您提交的数据不合法，请检查后重试', {
                        icon: 2
                    });
                    break;
                case 4001:
                    // 提交的数据验证失败
                    layer.alert('您提交的数据模型验证失败，请检查后重试', {
                        icon: 2
                    });
                    break;
                default:
                    // 其他错误
                    layer.alert('异常：' + response.Message, {
                        icon: 2
                    });
                    break;
            }
        }

        //    9.2 请求失败，网络异常
        function requestError() {
            layer.alert('网络异常，请稍候再试！若此问题出现多次，请联系管理员', {
                icon: 2
            });
        }

        //    9.3 服务器返回数据异常
        function serverError() {
            layer.alert('系统异常，请联系管理员', {
                icon: 2
            });
        }

        // 10. 公共方法
        //    10.1 解析json，并生成数据
        _this.render = function (d) {
            var html = '';
            var leftHtml = '';
            var rightHtml = '';
            if (d && d.length > 0) {
                data = d;
            }

            if(!data || data.length == 0) {
                if(hasLeft) {
                    doms.left.hide();
                    doms.leftTbody.html('');
                };
                if(hasRight) {
                    doms.right.hide();
                    doms.rightTbody.html('');
                }
                doms.bodyTbody.html('<tr class="empty"><td colspan="' + cols.length + '">暂无数据</td></tr>');
            } else {
                if(hasLeft) doms.left.show();
                if(hasRight) doms.right.show();

                $.each(data, function (idx, item) {
                    html += renderRow(item, idx);
                    leftHtml += renderLeftRow(item, idx);
                    rightHtml += renderRightRow(item, idx);
                });
                getBodyTbody().html(html);
                if (leftCols.length > 0) {
                    getLeftTbody().html(leftHtml);
                }
                if (rightCols.length > 0) {
                    getRightTbody().html(rightHtml);
                }
            }
        };

        //   10.2 获取选中数据
        _this.getSelectedItem = function () {
            return selectedItem;
        };

        //   10.3 获取选中数据数组
        _this.getSelectedItems = function () {
            var items = [];
            if (!isMulti && selectedItem) {
                items.push(selectedItem);
            } else {
                $.each(doms.left.find(':checked'), function (idx, item) {
                    var index = $(item).val();
                    items.push(data[index]);
                });
            }
            return items;
        };

        //   10.6 获取选中数据的值
        _this.getSelectedValue = function() {
            var item = _this.getSelectedItem();
            if(item) {
                return item[keyColumn];
            } else {
                return null;
            }
        };

        //   10.7 获取选中数据的值数组
        _this.getSelectedValues = function() {
            var items = _this.getSelectedItems();
            var temp = [];
            $.each(items, function(idx, item) {
                if(item[keyColumn]) {
                    temp.push(item[keyColumn]);
                }
            });
            return temp;
        };

        //   10.4 拉取数据
        _this.pull = function () {
            if (type == 'get' || type == 'GET') {
                _this.get();
            } else if (type == 'post' || type == 'POST') {
                _this.post();
            } else {
                console.log('不受支持的type参数。可选值[get, GET, post, POST]');
            }

            var start = new Date().getTime();
            data = [];
            $.ajax({
                url: api,
                data: getFilter(),
                type: type,
                contentType: 'JSON',
                beforeSend: function () {
                    layer.load(2);
                    if (ajaxBeforeSendFn && $.isFunction(ajaxBeforeSendFn)) {
                        ajaxBeforeSendFn();
                    }
                },
                success: function (response) {
                    if (response.code === 200) {
                        if (dataConvertFn && $.isFunction(dataConvertFn)) {
                            data = dataConvertFn(response.body);
                        } else {
                            data = response.body;
                        }
                        _this.render(data);
                        if (isPager) pager.setPager(response.recordCount, start);
                        if (renderCompleteFn && $.isFunction(renderCompleteFn)) renderCompleteFn();
                    } else if (data.code === 401) {
                        layer.alert("您的登录信息已过期，请重新登录！", {
                            icon: 6,
                            yes: function () {
                                top.location.href = "/Account/Login";
                            }
                        });
                    } else {
                        layer.msg(data.message);
                    }
                },
                error: function (event, xhr, options, exc) {
                    if (ajaxErrorFn && $.isFunction(ajaxErrorFn)) {
                        ajaxErrorFn(event, xhr, options, exc);
                    } else {
                        layer.msg("系统异常，请联系管理员");
                    }
                },
                complete: function () {
                    layer.closeAll('loading');
                    if (ajaxCompleteFn && $.isFunction(ajaxCompleteFn)) {
                        ajaxCompleteFn();
                    }
                }
            });
        };

        //   10.5 手动设置高度
        _this.resetHeight = function (h) {
            if (h) {
                height = h;
            }
            calcHeight();
            setHeight();
        };

        //   10.8 重新拉取数据
        _this.reload = function () {
            if (api) {
                _this.pull();
            } else {
                _this.render();
            }
        };

        //   10.9 搜索
        _this.search = function () {
            if (isPager && pager) {
                pager.pageIndex = 1;
            }
            if (api) {
                _this.pull();
            } else {
                _this.render();
            }
        };


        if (isAuto) {
            _this.pull();
        } else {
            _this.render();
        }
        return _this;
    };
})(jQuery, window, layer);

