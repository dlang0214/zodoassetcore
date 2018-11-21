;(function($, w) {
    $.fn.MyGrid = function(opts) {
        var _this = this;
        var $this = $(this);

        // 配置
        var cols = opts.columns;
        var isPager = opts.isPager;
        var isMulti = opts.isMulti;
        var isAutoWidth = opts.autoWidth || false;
        var pageSize = opts.pageSize || 20;
        var height = opts.height;

        if(isMulti) {
            cols.splice(0, 0, { title: '', width: 40, type: 'checkbox' });
        }

        // 数据
        var data = [];
        var pageIndex = 1;

        // 选中数据
        var selectedItems = [];
        var selectedItem = null;

        // 尺寸
        var widths = {
            // 表格宽度
            table: 0,
            total: $this.width()
        };
        var heights = {
            // 内容高度
            body: 0
        };

        // doms
        var header = $('<div class="grid-header"></div>');
        var body = $('<div class="grid-body"></div>');
        var headerTable = $('<table class="table header-table"></table>');
        var bodyTable = $('<table class="table body-table"></table>');
        var bodyTableTBody = $('<tbody class="body-tbody"></tbody>');

        // 临时数据
        var temp = {
            lastAutoWidthCol: null,
            autoColWidth: 0,
            fixedColTotal: 0
        };

        // 初始化doms
        clacWidth();
        clacHeight();
        initDoms();
        resetWidth();
        resetHeight();
        listen();

        // 初始化dom
        function initDoms() {
            var colHtml = '';
            var thHtml = '';
            $.each(cols, function(idx, col) {
                var t = col.type == 'checkbox' ? 
                            '<th class="content-center"><input type="checkbox" class="checkall" /></th>' : 
                            '<th>' + col.title + '</th>';
                if(col.isLastAutoWidthCol) {
                    colHtml += '<col width="' + col.width + '" class="last-col" />';
                    thHtml += t;
                } else {
                    colHtml += '<col width="' + col.width + '" />';
                    thHtml += t;
                }
            });
            bodyTable.append($('<colgroup>' + colHtml + '</colgroup>'));
            headerTable.append($('<colgroup>' + colHtml + '<col width="17"></colgroup><thead><tr>' + thHtml + '<th></th></tr></thead>'));

            bodyTable.append(bodyTableTBody);
            body.append(bodyTable);
            header.append(headerTable);
            $this.append(header);
            $this.append(body);
        }

        // 重置dom宽度
        function resetWidth() {
            $('.last-col').attr('width', temp.lastAutoWidthCol.width);
            headerTable.css('width', widths.table + 17);
            bodyTable.css('width', widths.table);
        }

        // 重置dom高度
        function resetHeight() {
            if(height) {
                body.css('height', heights.body);
            }
        }

        // 计算宽度
        function clacWidth() {
            var fixedWidth = 0;
            $.each(cols, function(idx, col) {
                if(!col.width) {
                    col.width = 120;
                    col.autoWidth = true;
                } else {
                    col.autoWidth = false;
                }
                widths.table += col.width;
                if(col.autoWidth) {
                    temp.lastAutoWidthCol = col;
                }
            });

            if(temp.lastAutoWidthCol == null) {
                temp.lastAutoWidthCol = cols[cols.length - 1];
            }
            temp.lastAutoWidthCol.origWidth = temp.lastAutoWidthCol.width;
            temp.lastAutoWidthCol.isLastAutoWidthCol = true;

            var spare = widths.total - widths.table - 18;
            if(spare > 0) {
                if(temp.lastAutoWidthCol != null) {
                    temp.lastAutoWidthCol.width += spare;
                }
                widths.table = widths.total - 17;
            }
            temp.fixedColTotal = widths.table - temp.lastAutoWidthCol.width;
        }

        // 计算高度
        function clacHeight() {
            if(height) {
                if($.isFunction(height)) {
                    heights.body = height() - 2;
                } else if($.isNumeric(height)) {
                    if(height <= 0) {
                        heights.body = $(w).height() + height - 2;
                    } else {
                        heights.body = height;
                    }
                }
            }
        }

        function listen() {
            body.on('scroll', function() {
                header.scrollLeft($(this).scrollLeft());
            });

            $(w).on('resize', function() {
                widths.total = $this.width();
                var lastColWidth = widths.total - temp.fixedColTotal - 18;
                if(lastColWidth > temp.lastAutoWidthCol.width) {
                    temp.lastAutoWidthCol.width = lastColWidth;
                } else {
                    if(lastColWidth < temp.lastAutoWidthCol.origWidth) {
                        temp.lastAutoWidthCol.width = temp.lastAutoWidthCol.origWidth;
                    } else {
                        temp.lastAutoWidthCol.width = lastColWidth;
                    }
                }
                widths.table = temp.fixedColTotal + temp.lastAutoWidthCol.width;
                resetWidth();

                if(height && $.isFunction(height)) {
                    clacHeight();
                    resetHeight();
                } else if(height != undefined && $.isNumeric(height) && height < 0) {
                    clacHeight();
                    resetHeight();
                }
            });

            body.on('click', 'tr', function() {
                $(this).addClass('selected').siblings().removeClass('selected');
                var idx = $(this).data('idx');
                selectedItem = data[idx];
            });

            body.on('click', '.grid-button', function() {
                var func = $(this).data('func');
                console.log(w, func, w[func])
                if(w[func]) {
                    return w[func]();
                }
            });

            if(isMulti) {
                $this.on('click', '.checkall', function() {
                    if($(this).prop('checked')) {
                        body.find(':checkbox').prop('checked', true);
                    } else {
                        body.find(':checkbox').prop('checked', false);
                    }
                });

                body.on('click', ':checkbox', function() {
                    if($(this).prop('checked')) {
                        var count = body.find(':checkbox:checked').length;
                        if(count == data.length) {
                            $this.find('.checkall').prop('checked', true);
                        }
                    } else {
                        $this.find('.checkall').prop('checked',false);
                    }
                });
            }
        }

        // 解析json，并生成数据
		_this.render = function(d) {
            var html = '';
            if(d && d.length > 0) {
                data = d;
            } else {
                data = [];
            }
			$.each(data, function(idx, item) {
				html += _this.renderRow(item, idx);
			});
            bodyTableTBody.html(html);
        };
        
        // 生成tr
		_this.renderRow = function(rowData, index) {
			var h = '<tr class="tr-' + index + '" data-idx="' + index + '">';
			$.each(cols, function(idx, col) {
				h += _this.renderColumn(col, rowData[col.field], index);
			});	
			h += '</tr>';
			return h;
        };
        
        // 生成td
		_this.renderColumn = function(col, colData, index) {
            var val = colData || '';
            var cssName = 'cell';

            if(col.className) {
                cssName += ' ' + col.className;
            }

			if(col.do != undefined) {
				var v = col.do(colData);
				return '<td><div class="' + cssName + '">' + v + '</div></td>';
			}
			switch (col.type) {
                case 'checkbox':
                    cssName += ' content-center';
					val = '<input type="checkbox" value="' + index + '" />';
					break;
				case 'indexNum':
					val = ++index;
					break;
				default:
					break;
			}
			return '<td><div class="' + cssName + '">' + val + '</div></td>';
        };

        _this.getSelectedItem = function() {
            return selectedItem;
        };

        _this.getSelectedItems = function() {
            var items = [];
            $.each(body.find(':checked'), function(idx, item) {
                var index = $(item).val();
                items.push(data[index]);
            });
            return items;
        };

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
                    if (response.Code === 200) {
                        if (dataConvertFn && $.isFunction(dataConvertFn)) {
                            data = dataConvertFn(response.Body);
                        } else {
                            data = response.Body;
                        }
                        _this.render(data);
                        if (isPager) pager.setPager(response.RecordCount, start);
                        if (renderCompleteFn && $.isFunction(renderCompleteFn)) renderCompleteFn();
                    } else if (data.Code === 401) {
                        layer.alert("您的登录信息已过期，请重新登录！", {
                            icon: 6,
                            yes: function () {
                                top.location.href = "/Account/Login";
                            }
                        });
                    } else {
                        layer.msg(data.Message);
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

        return _this;
    };
})(jQuery, window);