;(function($, w, layer) {
    $.fn.HomeTab = function(opts) {
        var _this = this;
        var $this = $(this);
        var ul = $this.find('ul').first();

        var contentWrapper = opts.contentWrapper;
        if(!contentWrapper) return console.log('必须指定内容区');

        var max = opts.max;

        var tagsWidth = 46;
        var index = 0;          // 标签编号
        var current = null;   // 当前标签编号
        var last = null;
        var tabs = []; 
        var liWidth = 46;    
        
        // 通过编号获取标签
        function getTabByNumber(num) {
            var val = $.map(tabs, function(tab) {
                if(tab.index == num) {
                    return tab;
                }
            });

            if(val && val.length > 0) return val[0];
            return null;
        }

        // 通过连接地址获取标签
        function getTabByUrl(url) {
            var val = $.map(tabs, function(tab) {
                if(tab.url == url) {
                    return tab;
                }
            });

            if(val && val.length > 0) return val[0];
            return null;
        }

        function append(title, url, idx, caller) {
            var li = $('<li><span>' + title + '</span><span class="btn-inline"><i class="fa fa-times"></i></span></li>');
            var content = $('<div class="home-tab-content' + idx + '"><iframe id="frame-' + idx + '" name="frame-' + idx + '" src="' + url + '" /></div>');

            ul.append(li);
            contentWrapper.append(content);
            var newFrame = document.getElementById('frame-' + idx).contentWindow;
            newFrame._winCaller = caller;

            li.on('click', function() {
                _this.toggle(idx);
            }).on('click', '.btn-inline', function(e) {
                e.stopPropagation();
                _this.close(idx);
            });

            var w = li.outerWidth();
            liWidth += w;
            //ul.css('width', liWidth);

            tabs.push({ url: url, index: idx, li: li, content: content, width: w });
        }

        function showDefault() {
            ul.find('li').first().addClass('active').siblings().removeClass('active');
            contentWrapper.children().eq(0).show().siblings().hide();
            last = current;
            current = null;
        }

        

        // 打开一个新标签
        _this.open = function(title, url, caller) {
            if(tabs.length >= max) {
                return layer.msg('打开过多标签，请关闭一些后再试');
            }
            if(current && current.url == url) return;
            var tab = getTabByUrl(url);
            if(tab != null) {
                _this.toggle(tab.index);
            } else {
                index ++;
                append(title, url, index, caller);
                _this.toggle(index);
            }
        };

        // 关闭一个标签
        _this.close = function(number) {
            var tab = getTabByNumber(number);
            if (!tab) return;

            var iframe = $('#frame-' + number)[0].contentWindow;
            if (iframe.onbeforeunload) {
                iframe.onbeforeunload();
            }

            // 删除dom
            tab.li.remove();
            tab.content.remove();

            // 从数组中移除
            tabs = $.grep(tabs, function(tab, i) {
                return tab.index != number;
            });

            liWidth -= tab.width;
            //ul.css('width', liWidth);

            if(current && number == current.index) {
                if(tabs.length == 0 || !last) {
                    showDefault();
                } else {
                    //lastTab = getTabByNumber(last.index);
                    //if(!lastTab) {
                    //    showDefault();
                    //} else {
                    //    _this.toggle(last.index);
                    //}
                    last = tabs[tabs.length - 1];
                    _this.toggle(last.index);
                }
            }
        };

        // 切换标签
        _this.toggle = function(number) {
            var tab = getTabByNumber(number);
            if(!tab) showDefault();

            tab.li.addClass('active').siblings().removeClass('active');
            tab.content.show().siblings().hide();

            last = current;
            current = tab;

            var left = tab.li.position().left;
            var scrollL = $this.scrollLeft();
            var total = $this.width();

            //console.log(left)

            if(scrollL > left - 45) {
                $this.animate({scrollLeft: left - 60}, 200);
            }

            if(total + scrollL < left + tab.width) {
                $this.animate({scrollLeft: (left + tab.width - total + 60)}, 200);
            }
        };

        // 关闭当前标签
        _this.closeCurrent = function() {
            if(current) {
                _this.close(current.index);
            }
        };

        // 关闭其他标签
        _this.closeOthers = function() {
            var tempTabs = $.grep(tabs, function(tab, idx) {
                return tab != current;
            });

            $.each(tempTabs, function(idx, item) {
                item.li.remove();
                item.content.remove();
            });

            tabs = [];
            last = null;

            if(current) {
                tabs.push(current);
            } else {
                tabs = [];
            }
        };

        // 关闭所有标签
        _this.closeAll = function() {
            showDefault();
            $.each(tabs, function(idx, item) {
                item.li.remove();
                item.content.remove();
            });
            tabs = [];
            last = null;
        };

        _this.scrollL = function() {
           var l = $this.scrollLeft();
           $this.animate({scrollLeft: (l - 200)});
        };

        _this.scrollR = function() {
            var l = $this.scrollLeft();
            $this.animate({scrollLeft: (l + 200)});
         };

        $this.find('li').first().on('click', function() {
            showDefault();
        });

        return _this;
    }
})(jQuery, window, layer);