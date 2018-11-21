; (function ($, w, layer) {
    $.fn.AssetSelector = function () {
        var cateUrl = '/AssetCate/Tree';
        var assetUrl = '/Asset/Get';

        var cates = [];
        var assets = [];

        var catesReady = false;
        var assetsReady = false;

        // 附加dom

        $(function () {
            $.get(cateUrl, function (response) {
                cates = response.Body;
                catesReady = true;
            });

            $.get(assetUrl, function (response) {
                assets = response.Body;
                assetsReady = true;
            });
        });
    }
})(jQuery, window, layer);