var deviceMain;
var video;

function addEvent(obj, name, func) {
    if (obj.attachEvent) {
        obj.attachEvent("on" + name, func);
    } else {
        obj.addEventListener(name, func, false);
    }
}

function plugin() { return document.getElementById("view1"); }
function view() { return document.getElementById("view1"); }

function InitLt() {
    // 设备接入和丢失。type 设备类型，1表示视频设备，2表示音频设备；idx设备索引； dbt 1表示设备到达， 2表示设备丢失
    addEvent(plugin(), 'DevChange', function (type, idx, dbt) {
        if (type == 1) { // 视频设备
            if (1 == dbt) {
                // 获取设备摄像头序号  Global_GetEloamType(var type, var idx);
                // type 类型，1表示视频设备，2表示音频设备；idx 设备索引
                // 返回设备摄像头序号，1 为主摄像头，2、3 为辅摄像头，-1 表示获取失败
                var deviceType = plugin().Global_GetEloamType(1, idx);
                if (1 == deviceType) {
                    // 设备到达
                    if (null == deviceMain) {
                        // Global_CreateDevice 创建设备。
                        // type 设备类型，1表示视频设备，2表示音频设备；idx设备索引。
                        // 返回设备摄像头序号，1为主摄像头，2、3为辅摄像头，-1表示获取失败
                        deviceMain = plugin().Global_CreateDevice(1, idx);
                        if (deviceMain) {
                            OpenVideo();
                        }
                    }
                }
            } else {
                // 设备丢失
                OnDeviceDisconnect();
            }
        }
    });

    // 设置窗口名称；参数 "view"表示预览窗口，"thumb"表示缩略图
    if (view().Global_SetWindowName) {
        view().Global_SetWindowName("view");
        plugin().Global_InitDevs();
    }

}

function ClearLt() {
    console.log("清理资源");
    if (video) {
        view().View_SetText("", 0);
        plugin().Video_Release(video);
        video = null;
    }
    if (deviceMain) {
        plugin().Device_Release(deviceMain);
        deviceMain = null;
    }
    if (plugin().Global_DeinitDevs) {
        plugin().Global_DeinitDevs();
    }
}

function OpenVideo() {
    CloseVideo();
    var dev = deviceMain;
    var subType = 1;    // 1、YUY2；2、MJPG；3、UYVY
    // 创建视频。var Device_CreateVideo(var dev, var resolution, var subtype);
    // dev 设备句柄； resolution 分辨率索引； subtype 子类型，1 表示YUY2 ，2 表示MJPG ，4表示UYVY,传0自动选择一个类型
    video = plugin().Device_CreateVideo(dev, 3, 0);
    if (video) {
        view().View_SelectVideo(video);
        view().Video_RotateRight(video);
        view().View_SetText("打开视频中，请等待...", 0);
    }
}

function CloseVideo() {
    if (video) {
        view().View_SetText("", 0);
        plugin().Video_Release(video);
        video = null;
    }
}

function OnDeviceDisconnect() {
    alert("设备已丢失");
}

function Left() {
    if (video) {
        plugin().Video_RotateLeft(video);
    }
}

function Right() {
    if (video) {
        plugin().Video_RotateRight(video);
    }
}


function Scan(fn) {
    var img = plugin().Video_CreateImage(video, 0, view().View_GetObject());
    var imgCode = plugin().Image_GetBase64(img, 2, 0);
    if (imgCode) {
        $.post("/Upload/Base64", { image: imgCode }, function (data) {
            fn(data);
        });
    } else {
        alert("拍照失败");
    }
    plugin().Image_Release(img);
}

window.onbeforeunload = function () {
    ClearLt();
}

//InitLt();