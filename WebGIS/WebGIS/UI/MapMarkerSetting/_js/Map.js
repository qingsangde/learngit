var mapObj;         //地图对象
var overView;       //鹰眼插件
var type;           //地图类型
var toolBar;        //工具条插件
var ruler;          //测距插件
var mouseTool;
var MGeocoder;
var addMarker = false;   //点击添加标记为true
var addMarkerSuccess = false;   //添加标注成功
var marker;         //标记
var x;
var y;


function MapInit() {
    mapObj = new AMap.Map("container", {
        view: new AMap.View2D({
            zoom: 14, //地图显示的缩放级别 
            resizeEnable: true
            
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
    //加载地理编码插件
    mapObj.plugin(["AMap.Geocoder"], function () {
        MGeocoder = new AMap.Geocoder({
            radius: 1000,
            extensions: "all"
        });

    });
    //添加地图类型切换插件     
    mapObj.plugin(["AMap.MapType"], function () {
        type = new AMap.MapType({ defaultType: 0 });
        mapObj.addControl(type);
    });
    //绘图形
    mapObj.plugin(["AMap.MouseTool"], function () {
        mouseTool = new AMap.MouseTool(mapObj);
        //mouseTool.circle(circleOption);   //使用鼠标工具绘制圆
    });

//    AMap.event.addListener(mapObj, "complete", completeEventHandler);
//    AMap.event.addListener(mapObj, "zoomstart", mZoomStart);
//    AMap.event.addListener(mapObj, "zoomend", mZoomend);

    //添加标记
    mapObj.on('click', function (e) {
        if (addMarker) {
            x = e.lnglat.getLng();
            y = e.lnglat.getLat();
            $('#MarkName')[0].value = "";
            $('#MarkNote')[0].value = "";
            $('#AddMarkerWindows').window('open');
        }
    });
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
    mapObj.setZoomAndCenter(4);
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
//清除绘制
function clearDraw() {
    if (mouseTool) {
        mouseTool.close(true);
    }
}


//-------------------以下为添加标注事件----------------------
function addMark() {        //添加标注
    var f = $('#data_grid').datagrid('getRows');
   // if (!f) {
    if (f.length==0) {   
        $("#hidRowId").attr("value", "0");
        addMarker = true;
    }
    else {
        $.messager.alert("系统提示：", "该经销商已存在标注数据！请选择修改操作！", "error");
    }
}
function addSave() {
    var M_Name = $("#MarkName").val(); 
    if (M_Name == null || M_Name == "") {
        $.messager.alert("系统提示：", "请输入标注名称！！", "error");
        return;
    } else {
        //保存操作
        saveAddAction(x, y);        
    }
    addMarker = false;
    //$('#AddMarkerWindows').window('close');
}

function drawMarker(MarkerTable, x, y) {
    mapObj.clearMap();
    for (var i = 0; i < MarkerTable.length; i++) {
        //初始化标记
        marker = new AMap.Marker({
            icon: "../_styles/images/marker_sprite.png",
            position: [MarkerTable[i]["M_Lng"], MarkerTable[i]["M_Lat"]],
            map: mapObj
        });


//        // 设置标注名称
//        marker.setLabel({//label默认蓝框白底左上角显示，样式className为：amap-marker-label
//            offset: new AMap.Pixel(20, 20), //修改label相对于maker的位置
//            content: MarkerTable[i]["M_Name"]
//        });
//        //设置标注备注
//        marker.setTitle(MarkerTable[i]["M_Desc"]);


        var thisinfoWindow = new AMap.InfoWindow({
            content: "<h3><font color=\"#00a6ac\">" + MarkerTable[i]["M_Name"] + "</font></h3>",
            autoMove: false,
            offset: new AMap.Pixel(0, -30)
        });

        thisinfoWindow.open(mapObj, [MarkerTable[i]["M_Lng"], MarkerTable[i]["M_Lat"]]);



    }
    if(x == "" && y == "")
        mapObj.setCenter(new AMap.LngLat(MarkerTable[0]["M_Lng"], MarkerTable[0]["M_Lat"]));
    else
        mapObj.setCenter(new AMap.LngLat(y, x));
}

function addDel() {
    addMarker = false;
    $('#AddMarkerWindows').window('close');
}

function markerFocus(x, y) {
    mapObj.setCenter(new AMap.LngLat(y, x));
}

