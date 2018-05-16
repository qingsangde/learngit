var mapObj;         //地图对象
var overView;       //鹰眼插件
var type;           //地图类型
var toolBar;        //工具条插件
var ruler;          //测距插件
var mouseTool;
var inforWindow;
var mapIsOK = false;
var cluster;
var markers = [];

function getM(flag) {
    var CarMarker
    if (flag == 0) {
        CarMarker = new AMap.Marker({
            position: mapObj.getCenter(),
            icon: '_styles/cars/bM.png',
            offset: new AMap.Pixel(-20, -20),
            zIndex:1,
            autoRotation: true
        });
    }
    else {
        CarMarker = new AMap.Marker({
            position: mapObj.getCenter(),
            icon: '_styles/cars/eM.png',
            offset: new AMap.Pixel(-20, -20),
            zIndex: 2,
            autoRotation: true
        });
    }
    return CarMarker;
}

//地图初始化
function MapInit() {
    mapObj = new AMap.Map("container", {
        view: new AMap.View2D({
            center: new AMap.LngLat(105, 40),  //地图中心点-西安
            resizeEnable: true,
            zoom: 4, //地图显示的缩放级别 
            rotation: 0
        }),
        lang: "zh_cn"
    });

    //鹰眼
    mapObj.plugin(["AMap.OverView"], function () {
        overView = new AMap.OverView();
        mapObj.addControl(overView);
    });
    //ToolBar插件
    mapObj.plugin(["AMap.ToolBar"], function () {
        toolBar = new AMap.ToolBar();
        mapObj.addControl(toolBar);
    });
    //添加距离量测插件
    mapObj.plugin(["AMap.RangingTool"], function () {
        ruler = new AMap.RangingTool(mapObj);
        AMap.event.addListener(ruler, "end", function (e) {
            ruler.turnOff();
        });
    });

    inforWindow = new AMap.InfoWindow({
        autoMove: true,
        content: "无地址信息！"
    });

    //添加地图类型切换插件     
    mapObj.plugin(["AMap.MapType"], function () {
        type = new AMap.MapType({ defaultType: 0 });
        mapObj.addControl(type);
    });

    //聚合点
    mapObj.plugin(["AMap.MarkerClusterer"], function () {
        cluster = new AMap.MarkerClusterer(mapObj, markers, { maxZoom: 16 });
    });
    //    AMap.event.addListener(mapObj, "complete", completeEventHandler);

    //    AMap.event.addListener(mapObj, "zoomstart", mZoomStart);
    //    AMap.event.addListener(mapObj, "zoomend", mZoomend);

    $('#loading-mask').fadeOut(1000);
}

var isStop = false;
function mZoomStart() {
    if (tmTrack) {
        window.clearTimeout(tmTrack);
        tmTrack = false;
        isStop = true;
    }
}

function mZoomend() {
    if (isStop) {
        Play();
        isStop = false;
    }
}

function completeEventHandler() {
    $('#loading-mask').fadeOut(1000);
}

//-------------------以下为地图操作按钮事件----------------------
//测距
function doRuler() {
    ruler.turnOn();
}
//打印
function doPrint() {
    window.print();
}
//缩放最小
function zoomOut() {
    mapObj.setZoomAndCenter(4, new AMap.LngLat(105, 40));
}
//缩放最大
function zoomIn() {
    if (marker) {
        mapObj.setZoomAndCenter(15, marker.getPosition());
    }
    else {
        mapObj.setZoom(15);
    }
}

function getCarMarker() {
    var CarMarker = new AMap.Marker({
        position: new AMap.LngLat(105, 40),
        icon: '_styles/cars/che4_1.gif',
        offset: new AMap.Pixel(-20, -20),
        autoRotation: true
    });
    AMap.event.addListener(CarMarker, "click", showCarWinInfos);
    return CarMarker;
}

function getPolyline() {
    var polyline = new AMap.Polyline({
        map: mapObj,
        geodesic: true,
        path: [],
        strokeColor: "#00A", //线颜色             
        strokeOpacity: 1, //线透明度             
        strokeWeight: 3, //线宽             
        strokeStyle: "solid"//线样式         
    });
    return polyline;
}

function getInfoWindow() {
    var myInfoWindow = new AMap.InfoWindow({
        autoMove: true,
        content: "无地址信息！"//,
    });
    return myInfoWindow;
}

function getIcon(dir) {
    switch (dir) {
        case "北": return "_styles/cars/che1_1.png";
        case "东北": return "_styles/cars/che1_2.png";
        case "东": return "_styles/cars/che1_3.png";
        case "东南": return "_styles/cars/che1_4.png";
        case "南": return "_styles/cars/che1_5.png";
        case "西南": return "_styles/cars/che1_6.png";
        case "西": return "_styles/cars/che1_7.png";
        case "西北": return "_styles/cars/che1_8.png";
    }
    return "_styles/cars/che1_1.png";
}