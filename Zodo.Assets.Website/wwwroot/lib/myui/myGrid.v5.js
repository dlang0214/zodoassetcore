;(function($, w) {
    $.fn.MyGrid = function(opts) {
        var _this = this;
        var $this = $(this);

        // 必要的配置项，从opts中获取并初始化
        var cols = opts.columns || [];
        var height = opts.height || null;
        var hasPager = opts.isPager || false;
        var isMulti = opts.isMulti || false;
        var onClick = opts.click || undefined;
        var dblClick = opts.dblClick || undefined;
        var pagerClass = opts.pagerClass || 'pager';
        var handlers = opts.handlers;

        if(cols.length == 0) {
            return console.log('必须指定显示的列');
        }

        // 是否有横向滚动条
        var hasHScrollbar = false;
        // 最外层宽度
        var boxWidth = $this.width();
        //console.log(boxWidth)
        // 表格的宽度
        var tableWidth = 0;
        // Body的高度
        var bodyHeight = 0;
        // 固定列的高度
        var fixedHeight = 0;
        // 左侧固定列
        var leftFixedCol = [];
        // 右侧固定列
        var rightFixedCol = [];
        // 左侧固定列宽度
        var leftFixedColWidth = 0;
        // 右侧固定列宽度
        var rightFixedColWidth = 0;

        //
        var headTable, bodyTable, leftFixBox, rightFixBox, bodyTbody,
            leftBody, rightBody, lastCol;

        // 要渲染的数据
        var data = [];
        var selectedIndex = null;
        var selectedIndexes = [];

        init();
        var pager = hasPager ? new Pager(_this, 20) : null;
        listen();

        // 初始化
        function init() {
            clacWidth();
            clacHeight();
            var headerColGroup = '<colgroup>';
            var bodyColGroup = '<colgroup>';
            var headerTr = '<thead><tr>';
            $.each(cols, function(idx, col) {
                headerColGroup += '<col width="' + col.width + '"' + (col.isLastCol ? ' class="last-col"' : '') + ' />';
                bodyColGroup += '<col width="' + col.width + '"' + (col.isLastCol ? ' class="last-col"' : '') + ' />';
                if(col.type == 'checkbox') {
                    headerTr += '<th style="content-center">';
                    headerTr += '<input type="checkbox" class="checkall" />';
                } else {
                    headerTr += '<th>';
                    headerTr += col.title || '--' + '</th>';
                }
            });
            headerColGroup += '<col width="17" /></colgroup>';
            bodyColGroup += '</colgroup>';
            headerTr += '<th></th></tr></thead>';
            var html = '';
            html += '<div class="grid-top-right-mask"></div>';
            html += '<div class="grid-header">';
            html += '<table class="table head-table">' + headerColGroup + headerTr + '</table>';
            html += '</div>';
            html += '<div class="grid-body scroll-y" style="height: ' + bodyHeight + 'px">';
            html += '<table class="table body-table">' + bodyColGroup + '<tbody><tr><td colspan="' + cols.length + '" class="empty">暂无数据</td></tr></tbody></table>';
            html += '</div>';
            if(leftFixedColWidth > 0) {
                html += '<div class="grid-fixed-left" style="width: ' + (leftFixedColWidth + 1) + 'px">';
                html += '<div class="fixed-left-header">';
                html += '</div>';
                html += '<div class="fixed-left-body scroll-y fixed-scroll-y">';
                html += '</div>';
                html += '</div>';
            }
            if(rightFixedCol.length > 0) {
                html += '<div class="grid-fixed-right" style="width: ' + (rightFixedColWidth + 1) + 'px">';
                html += '<div class="fixed-right-header">';
                html += '</div>';
                html += '<div class="fixed-right-body scroll-y fixed-scroll-y">';
                html += '</div>';
            }
            html += '</div>';

            $this.html(html);

            head = $this.find('.grid-header');
            body = $this.find('.grid-body');
            headTable = $this.find('.grid-header table');
            bodyTable = $this.find('.grid-body table');
            leftFixBox = $this.find('.grid-fixed-left');
            rightFixBox = $this.find('.grid-fixed-right');
            bodyTbody = bodyTable.find('tbody');
            leftBody = leftFixBox.find('.fixed-left-body');
            rightBody = rightFixBox.find('.fixed-right-body');

            headTable.width(tableWidth + 17);
            bodyTable.width(tableWidth);
            
            if(leftFixedColWidth > 0) {
                leftFixBox.find('.fixed-left-header').append($(head.find('table').clone()));
                leftFixBox.find('.fixed-left-body').append($(body.find('table').clone()));
            }
            if(rightFixedColWidth > 0) {
                rightFixBox.find('.fixed-right-header').append($(head.find('table').clone()));
                rightFixBox.find('.fixed-right-body').append($(body.find('table').clone()));
            }

            if(bodyHeight && leftFixedCol.length > 0) {
                if(hasHScrollbar) {
                    leftFixBox.css('bottom', '17px');
                } else {
                    leftFixBox.css('bottom', '0');
                }
            } else {
                leftFixBox.css('bottom', '0');
            }
            if(bodyHeight && rightFixedCol.length > 0) {
                if(hasHScrollbar) {
                    rightFixBox.css('bottom', '17px');
                } else {
                    rightFixBox.css('bottom', '0');
                }
            } else {
                rightFixBox.css('bottom', '0');
            }
            if(bodyHeight) {
                $this.find('.grid-body').height(bodyHeight);
                if(leftFixedCol.length > 0) {
                    if(hasHScrollbar) {
                        leftBody.height(bodyHeight - 17);
                    } else {
                        leftBody.height(bodyHeight);
                    }
                }
                if(rightFixedCol.length > 0) {
                    if(hasHScrollbar) {
                        rightBody.height(bodyHeight - 17);
                    } else {
                        rightBody.height(bodyHeight);
                    }
                }
            }
        }

        function resize() {
            var boxWidth = body.width();
            var spare = boxWidth - otherColsWidth - 18;
            if(spare < lastNotFixedCol.orgWidth) {
                spare = lastNotFixedCol.orgWidth;
                hasHScrollbar = true;
            } else {
                hasHScrollbar = false;
            }
            tableWidth = spare + otherColsWidth;
            $this.find('.head-table').css('width', tableWidth + 17 + 'px');
            $this.find('.body-table').css('width', (tableWidth) + 'px')
            $('.last-col', $this).attr('width', spare);

            if(hasHScrollbar && leftFixedColWidth > 0) {
                leftFixBox.css('bottom', '17px');
                leftBody.height(bodyHeight - 17);
            } else {
                leftFixBox.css('bottom', '0');
                leftBody.height(bodyHeight);
            }

            if(hasHScrollbar && rightFixedColWidth > 0) {
                rightFixBox.css('bottom', '17px');
                rightBody.height(bodyHeight - 17);
            } else {
                rightFixBox.css('bottom', '0');
                rightBody.height(bodyHeight);
            }

            // 非固定高度
            if(bodyHeight) {
                clacHeight();
                $this.find('.grid-body').height(bodyHeight);
                if(leftFixedCol.length > 0) {
                    if(hasHScrollbar) {
                        leftBody.height(bodyHeight - 17);
                    } else {
                        leftBody.height(bodyHeight);
                    }
                }
                if(rightFixedCol.length > 0) {
                    if(hasHScrollbar) {
                        rightBody.height(bodyHeight - 17);
                    } else {
                        rightBody.height(bodyHeight);
                    }
                }
            } else {
                body.css('height', 'auto')
                if(leftFixedCol.length > 0) {
                    leftBody.css('height', 'auto')
                }
                if(rightFixedCol.length > 0) {
                    rightBody.css('height', 'auto')
                }
            }
        }

        // 监听
        function listen() {
            body.on('scroll', function() {
                var scrollLeft = $(this).scrollLeft();
                head.scrollLeft(scrollLeft);

                var scrollTop = $(this).scrollTop();
                $this.find('.fixed-scroll-y').scrollTop(scrollTop);
            });

            $(w).on('resize', function() {
                resize();
            });

            if(isMulti) {
                $this.on('click', '.checkall', function(e) {
                    e.stopPropagation();
                    
                    var checked = $(this).prop('checked');
                    if(checked) {
                        $this.find(':checkbox').prop('checked', true);
                    } else {
                        $this.find(':checkbox').prop('checked', false);
                    }
                });
            }

            if(!isMulti) {
                $this.on('click', 'tbody tr', function() {
                    if(!$(this).hasClass('selected')) {
                        var idx = $(this).data('idx');
                        $this.find('.tr-' + idx).addClass('selected').siblings().removeClass('selected');
                        selectedIndex = idx;
                        if(onClick) {
                            onClick(data[selectedIndex]);
                        }
                    }
                });
            } else {
                $this.on('click', 'tbody tr', function() {
                    var cb = $(this).find(':checkbox');
                    cb.trigger('click');
                });
            }

            $this.on('dblclick', 'tbody tr', function() {
                var idx = $(this).data('idx');
                if(dblClick) {
                    dblClick(data[idx]);
                }
            });

            $this.on('mouseover', 'tbody tr', function() {
                var cn = $(this).attr('class');
                $this.find('.' + cn).addClass('hover').siblings().removeClass('hover');
            });

            $this.on('click', '.grid-button', function() {
                var func = $(this).data('func');
                // console.log(func);
                var idx = $(this).parents('tr').data('idx');
                var d = data[idx];
                if(handlers[func]) {
                    handlers[func](d);
                }
            });

            $this.on('click', 'tbody :checkbox', function(e) {
                e.stopPropagation();

                if($(this).prop('checked')) {
                    var count = $this.find('tbody :checked').length;
                    if(count == data.length) {
                        $this.find('.checkall').prop('checked', true);
                    }
                } else {
                    $this.find('.checkall').prop('checked', false);
                }
            });
        }

        var lastNotFixedCol,    // 最后一个非固定列
            otherColsWidth;     // 除了最后一个非固定列以外所有列的宽度
        // 计算宽度
        function clacWidth() {
            var lastNotFixedColIndex = null;
            if(isMulti) {
                cols.splice(0, 0, { title: '', width: 40, type: 'checkbox', fixed: 'left' });
            }
            $.each(cols, function(idx, col) {
                // 如果没有指定列的宽度，指定为默认的120
                if(!col.width) {
                    col.width = 120;
                }
                // 累加表格的总宽度
                tableWidth += col.width;
                // 筛选左侧固定列和右侧固定列
                if(col.fixed == 'left') {
                    leftFixedCol.push(col);
                    leftFixedColWidth += col.width;
                } else if(col.fixed == 'right') {
                    rightFixedCol.push(col);
                    rightFixedColWidth += col.width;
                } else {
                    lastNotFixedColIndex = idx;
                }
            });
            // 如果表格宽度小于总宽度，
            // 拉长最后一列的宽度，使表格宽度等于总宽度
            lastNotFixedCol = cols[lastNotFixedColIndex];
            lastNotFixedCol.isLastCol = true;
            lastNotFixedCol.orgWidth = lastNotFixedCol.width;
            if(boxWidth < tableWidth + 18) {
                hasHScrollbar = true;
            } else {
                hasHScrollbar = false;
                var spare = boxWidth - tableWidth - 18;
                lastNotFixedCol.width += spare;
                tableWidth = boxWidth - 17;
            }
            otherColsWidth = tableWidth - lastNotFixedCol.width;
        }

        // 计算高度
        function clacHeight() {
            var totalHeight;

            if($.isFunction(height)) {
                // 如果高度指定的是function
                // 总高度设定为函数返回值
                totalHeight = height();
            } else if($.isNumeric(height)) {
                
                if(height >= 0) {
                    if(height > 200) {
                        totalHeight = height;
                    }
                } else {
                    totalHeight = $(w).height() + height;
                }
            }

            if(totalHeight) {
                bodyHeight = totalHeight - 35;
            }
            if(hasPager) {
                bodyHeight = bodyHeight - 40;
            }
            if(hasHScrollbar) {
                fixedHeight = bodyHeight - 17;
            } else {
                fixedHeight = bodyHeight;
            }
        }

        function cloneBody() {
            body = body || $this.find('.grid-body table');
            if(leftFixedColWidth > 0) {
                leftBody.empty().append($(bodyTable).clone());
            }
            if(rightFixedColWidth > 0) {
                rightBody.empty().append($(bodyTable).clone());
            }
        }

        // 第六部分：分页
        function Pager(domTBody, pagesize, cssClass) {
            this.pageSize = pagesize || 20;
            this.pageCount = 0;
            this.recordCount = 0;
            this.pageIndex = 1;

            var pagerDom = $('<div class="' + pagerClass + ' grid-pager"></div>');
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
                this.recordCount = total;
                this.pageCount = Math.ceil(this.recordCount / this.pageSize);
                firstBtn.removeClass('disabled');
                prevBtn.removeClass('disabled');
                nextBtn.removeClass('disabled');
                lastBtn.removeClass('disabled');
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
            bodyTbody.html(html);
            cloneBody();
        };
        
        // 渲染中间的表格
		_this.renderRow = function(rowData, index) {
			var h = '<tr class="tr-' + index + '" data-idx="' + index + '">';
			$.each(cols, function(idx, col) {
				h += _this.renderColumn(col, rowData[col.field], index);
			});	
			h += '</tr>';
			return h;
        };
        
        // 渲染单元格
		_this.renderColumn = function(col, colData, index) {
			var val = colData || '';
			if(col.do != undefined) {
				var v = col.do(colData);
				return '<td><div class="' + col.className + '">' + v + '</div></td>';
			}
			switch (col.type) {
				case 'checkbox':
					val = '<input type="checkbox" value="' + index + '" />';
					break;
				case 'indexNum':
					val = ++index;
					break;;
				default:
					break;
			}
			return '<td><div class="' + col.className + '">' + val + '</div></td>';
        };
        
        return _this;
    }
})(jQuery, window);