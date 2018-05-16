﻿var mapObj;         //地图对象
var overView;       //鹰眼插件
var type;
var toolBar;        //工具条插件
var ruler;          //测距插件
var mouseTool;      //鼠标工具
var editorTool;     //编辑工具

var inforWindow;
var MGeocoder;
var circle = false;
var polygon = false;

//地图初始化
function MapInit() {
    mapObj = new AMap.Map("container", {
        view: new AMap.View2D({
            center: new AMap.LngLat(105, 40),  //地图中心点-西安
            resizeEnable: true,
            zoom: 4 //地图显示的缩放级别           
        }),
        keyboardEnable: false
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
    //绘图形
    mapObj.plugin(["AMap.MouseTool"], function () {
        mouseTool = new AMap.MouseTool(mapObj);
        //mouseTool.circle(circleOption);   //使用鼠标工具绘制圆
        AMap.event.addListener(mouseTool, "draw", completeDraw);
    });

    AMap.event.addListener(mapObj, "complete", completeEventHandler);
}

function completeEventHandler() {
    //setPage();
}

function completeDraw(e) {
    mouseTool.close(false);
    var graphType = $("#hidGraphT").val();
    if (graphType == "1") {
        circle = e.obj;
    }
    else if (graphType == "2") {
        polygon = e.obj;
    }
    SetFenceContent(e.obj);
}



//-------------------以下为地图操作按钮事件----------------------
//测距
function doRuler() {
    ruler.turnOn();
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
//刷新地图
function reloadMap() {
    mapObj.destroy();
    MapInit();
}
//清除绘制
function clearDraw() {
    if (mouseTool) {
        mouseTool.close(true);
    }
}
//画圆
function drawYuan() {
    $("#hidGraphT").attr("value", "1");
    //设置圆的属性
    var circleOption = {
        strokeColor: "#F33", //线颜色
        strokeOpacity: 1, //线透明度
        strokeWeight: 3, //线粗细度
        fillColor: "#ee2200", //填充颜色
        fillOpacity: 0//填充透明度
    };
    mouseTool.circle(circleOption);   //使用鼠标工具绘制圆 
}
//画方
function drawFang() {
    $("#hidGraphT").attr("value", "2");
    //设置多边形的属性
    var polygonOption = {
        strokeColor: "#FF33FF", //线颜色   
        strokeOpacity: 0.2, //线透明度    
        strokeWeight: 3,    //线宽    
        fillColor: "#ee2200", //填充色   
        fillOpacity: 0.35//填充透明度 
    };
    mouseTool.polygon(polygonOption);   //使用鼠标工具绘制多边形 
}

function addCircle(content) {
    mapObj.clearMap(); //清除地图覆盖物
    var subArr = new Array();
    subArr = content.split(";");
    var myradius = subArr[1];
    var LngLatArr = new Array();
    LngLatArr = subArr[0].split(",");
    var Lng = LngLatArr[1];
    var Lat = LngLatArr[0];
    circle = new AMap.Circle({
        center: new AMap.LngLat(Lng, Lat), // 圆心位置
        radius: myradius, //半径
        strokeColor: "#F33", //线颜色
        strokeOpacity: 1, //线透明度
        strokeWeight: 3, //线粗细度
        fillColor: "#ee2200", //填充颜色
        fillOpacity: 0//填充透明度
    });
    circle.setMap(mapObj);

    mapObj.setFitView();
}

function addPolygon(content) {
    mapObj.clearMap(); //清除地图覆盖物

    var polygonArr = new Array(); //多边形覆盖物节点坐标数组  

    var subArr = new Array();
    subArr = content.split(";");
    if (subArr.length > 0) {
        for (var i = 0; i < subArr.length; i++) {
            var LngLatArr = new Array();
            LngLatArr = subArr[i].split(",");
            var Lng = LngLatArr[1];
            var Lat = LngLatArr[0];
            polygonArr.push(new AMap.LngLat(Lng, Lat));
        }
    }

    polygon = new AMap.Polygon({
        path: polygonArr, //设置多边形边界路径   
        strokeColor: "#FF33FF", //线颜色   
        strokeOpacity: 0.2, //线透明度    
        strokeWeight: 3,    //线宽    
        fillColor: "#ee2200", //填充色   
        fillOpacity: 0.35//填充透明度 
    });
    polygon.setMap(mapObj);
    mapObj.setFitView();
}

function EditDraw() {
    var graphType = $("#hidGraphT").val();
    if (graphType == "1") {
        mapObj.plugin(["AMap.CircleEditor"], function () {
            editorTool = new AMap.CircleEditor(mapObj, circle);
            editorTool.open();
        });

    }
    else if (graphType == "2") {
        mapObj.plugin(["AMap.PolyEditor"], function () {
            editorTool = new AMap.PolyEditor(mapObj, polygon);
            editorTool.open();
        });

    }
    else {
        $.messager.alert("系统提示：", "当前地图上没有可编辑的图形！！！");
    }
}

function EndEditDraw() {
    var graphType = $("#hidGraphT").val();
    if (graphType == "1") {
        editorTool.close();
        SetFenceContent(circle);
    }
    else if (graphType == "2") {
        editorTool.close();
        SetFenceContent(polygon);
    }
    else {
        $.messager.alert("系统提示：", "当前地图上没有可编辑的图形！！！");
    }
}


function SetFenceContent(graph) {
    var content = "";
    var graphType = $("#hidGraphT").val();
    if (graphType == "1") {//圆形
        var circlecenter = graph.getCenter();
        var radius = graph.getRadius().toFixed(2);
        content = graph.getCenter().getLat() + "," + graph.getCenter().getLng() + ";" + radius;
        $("#hidContent").attr("value", content);
    }
    else if (graphType == "2") {//多边形
        var pointArray = graph.getPath();
        if (pointArray.length > 0) {
            for (var i = 0; i < pointArray.length; i++) {
                content += pointArray[i].getLat() + "," + pointArray[i].getLng() + ";"
            }
            content = content.substring(0, content.length - 1);
        }
        $("#hidContent").attr("value", content);
    }
    else {
        $.messager.alert("系统提示：", "当前地图上没有图形！！！");
    }
}