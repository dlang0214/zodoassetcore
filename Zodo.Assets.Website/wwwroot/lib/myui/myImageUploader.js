(function ($, w) {
    $.fn.MultiImageUploader = function (opt) {
        var self = $(this);
        var count = self.data("count") || 1;    // 最多上传图片数量
        var images = [];                        // 已上传的图片
        opt.count = count;

        var uploader = new ImgUploader(self, opt);
        var pathHolder = self.find("input[type='hidden']");
        var thumbs = self.find(".uploader-thumbs").first();

        if (opt.value) {
            images = opt.value.split(",");
            for (var i = 0; i < images.length; i++) {
                if (i < count) {
                    addImage(images[i]);
                }
            }

            if (images.length >= count) {
                self.find(".b").hide();
            }
        }

        uploader.doWhen("fileUploadSuccess", function (d) {
            if (d.code == 200) {
                result = d.body;
                images.push(d.body);
                pathHolder.val(images.join(","));
                addImage(d.body);
            } else {
                alert("上传失败:" + d.message);
            }

            if (images.length >= count) {
                self.find(".b").hide();
            } else {
                self.find(".b").show();
            }
        });

        uploader.doWhen("fileUploadError", function (e, d) {
            console.log("上传出错");
        })

        uploader.doWhen("fileTooLarge", function (e, d) {
            alert("文件太大");
        })

        uploader.doWhen("fileStartUpload", function (e, d) {
            console.log("开始上传");
        });

        function addImage(src) {
            var img = new Image();
            img.src = src;

            img.onload = function () {
                scaleImg(this, 120, 120);
                var b = $("<div class='uploader-thumb'><span>删除</span></div>");
                b.append(this);
                thumbs.append(b);

                b.find("span").on("click", function () {
                    $(this).parent().remove();
                    removeImage(src);
                    freshButtonAndStr();
                });
            }

            img.onerror = function () {
                scaleImg(this, 120, 120);
                var b = $("<div class='uploader-thumb' style='width: 120px; height: 120px;'><span>删除</span></div>");
                b.append(this);
                thumbs.append(b);

                b.find("span").on("click", function () {
                    $(this).parent().remove();
                    removeImage(src);
                    freshButtonAndStr();
                });
            }
        }

        // 缩放图片到合适的大小
        function scaleImg(img, maxW, maxH) {
            var w = img.width, h = img.height;
            var scale_x = w / maxW;
            var scale_y = h / maxH;
            var scale = scale_x > scale_y ? scale_x : scale_y;
            //if (scale > 1) {
            //    img.width = w / scale;
            //    img.height = h / scale;
            //}
            if (scale_x > scale_y) {
                img.width = maxW;
                img.height = h / scale_x;
            } else {
                img.height = maxH;
                img.width = w / scale_y;
            }
        }

        // 删除一张图片
        function removeImage(src) {
            images = $.grep(images, function (v, k) {
                return v != src;
            });
        }

        // 设置按钮是否显示、更新文本框的字符串
        function freshButtonAndStr() {
            pathHolder.val(images.join(","));

            if (images.length >= count) {
                //uploader.fileInput.attr("disabled", "disabled");
                self.find(".b").hide();
            } else {
                //uploader.fileInput.removeAttr("disabled");
                self.find(".b").show();
            }
        }

        self.getImages = function () {
            return images;
        };

        self.setImages = function (imgs) {
            images = imgs;
        }        

        self.add = function (src) {
            images.push(src);
            pathHolder.val(images.join(","));
            return addImage(src);
        }

        return self;
    }

    function ImgUploader(wrapper, opts) {
        // 变量
        var fileInput, fileButton, pathInput, thumbs;
        var listener = { noFileSelect: null, fileTooLarge: null, fileSelected: null, fileStartUpload: null, fileUploadSuccess: null, fileUploadError: null };

        var config = {
            wrapperClass: "uploadWrapper",
            selectClass: "selectButton",
            buttonClass: "btn btn-default",
            valueFormName: "",
            value: "",
            maxSize: 1024 * 1024,
            autoUpload: true
        };

        $.extend(true, config, opts);

        fileInput = $("<input type='file' class='b' style='width: 0.1px; height: 0.1px; position: absolute; opacity: 0; z-index: 2;' accept='image/gif,image/png,image/jpg,image/jpeg' />");
        fileButton = $("<a style='display: inline-block; cursor: pointer; z-index: 4' class='b btn btn-blue'>本地上传</a>");
        pathInput = $("<input type='hidden' value='" + (config.value.length > 0 ? config.value : "") + "' " + (config.valueFormName.length > 0 ? " name='" + config.valueFormName + "'" : "") + " />");
        thumbs = $("<div class='uploader-thumbs'></div>");

        wrapper.append(fileInput);
        wrapper.append(fileButton);
        wrapper.append(pathInput);
        wrapper.append(thumbs);

        var that = this;

        fileButton.on("click", function (e) {
            fileInput.trigger("click");
        })

        fileInput.on('change', function () {
            var files = $(this).prop("files");
            if (files.length == 0) {
                publish("noFileSelected");
                return;
            }

            file = files[0];
            if (file.size > config.maxSize) {
                publish("fileTooLarge");
                fileInput.val("");
                return;
            }

            if (config.autoUpload) {
                upload(file);
                fileInput.val("");
            } else {
                publish("fileSelect", [files]);
            }
        });

        this.wrapper = wrapper;
        this.fileInput = fileInput;
        this.fileButton = fileButton;
        this.getListener = function () {
            return listener;
        }
        this.doWhen = function (eventName, fn) {
            for (var p in listener) {
                if (p == eventName) {
                    if (listener[p]) {
                        listener[p] = [].concat.call(listener[p], fn);
                    } else {
                        listener[p] = fn;
                    }
                }
            }
        }

        function publish(eventName, data) {
            var fn = listener[eventName] || null;
            var i, len;
            if (fn instanceof Array) {
                for (i = 0, i < fn.length; i < len; i++) {
                    fn[i] && fn[i](data);
                }
            } else if (fn instanceof Function) {
                fn && fn(data);
            }
        }

        // 上传
        function upload(file) {
            uploadSingle(file, function () {
                publish("fileStartUpload");
            }, function (data) {
                publish("fileUploadSuccess", data);
            }, function () {
                publish("fileUploadError");
            });
        }

        // 单文件上传
        function uploadSingle(file, beforeSendCb, successCb, errorCb) {
            var formData = new FormData();
            formData.append("img", file);

            $.ajax({
                url: wrapper.data("url") + "?r=" + (+new Date()),
                type: "post",
                data: formData,
                //async: false,
                cache: false,
                contentType: false,
                processData: false,
                dataType: "json",
                beforeSend: function () {
                    fileButton.attr("disabled", "disabled");
                    beforeSendCb();
                },
                success: function (data) {
                    //console.log(data);
                    fileButton.removeAttr("disabled");
                    successCb(data);
                },
                error: function (x) {
                    //console.log(x);
                    fileButton.removeAttr("disabled");
                    errorCb();
                }
            })
        }
    }
})(jQuery, window);