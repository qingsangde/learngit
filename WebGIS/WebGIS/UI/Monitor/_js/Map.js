var mapObj;         //地图对象
var overView;       //鹰眼插件
var type;           //地图类型
var toolBar;        //工具条插件
var ruler;          //测距插件
var mouseTool;
var MGeocoder;
var mapIsOK = false;
var markers = [];
var cluster;
var selectedMarkerPosition;
var infoWindow = null;
var isAutoCloseInfoWindow = false;

//地图初始化
function MapInit() {
    mapObj = new AMap.Map("container", {
        view: new AMap.View2D({
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

    AMap.event.addListener(mapObj, "complete", completeEventHandler);

    AMap.event.addListener(mapObj, "zoomstart", mZoomStart);
    AMap.event.addListener(mapObj, "zoomend", mZoomend);

    mapObj.plugin(["AMap.CitySearch"], function () {
        //实例化城市查询类
        var citysearch = new AMap.CitySearch();
        //自动获取用户IP，返回当前城市
        citysearch.getLocalCity();
        AMap.event.addListener(citysearch, "complete", function (result) {
            if (result && result.city && result.bounds) {
                var cityinfo = result.city;
                var citybounds = result.bounds;

                //地图显示当前城市
                mapObj.setBounds(citybounds);
            }
            else {
                alert(result.info);
            }
        });
        AMap.event.addListener(citysearch, "error", function (result) { alert(result.info); });
    });
    
}

/*
*添加点聚合
*/
function addCluster(tag) {
    if (cluster) {
        cluster.setMap(null);
    }
    if (tag == 1) {
        var sts = [{ url: "http://developer.amap.com/wp-content/uploads/2014/06/1.png", size: new AMap.Size(32, 32), offset: new AMap.Pixel(-16, -30) },
			{ url: "http://developer.amap.com/wp-content/uploads/2014/06/2.png", size: new AMap.Size(32, 32), offset: new AMap.Pixel(-16, -30) },
			{ url: "http://developer.amap.com/wp-content/uploads/2014/06/3.png", size: new AMap.Size(48, 48), offset: new AMap.Pixel(-24, -45), textColor: '#CC0066'}];
        mapObj.plugin(["AMap.MarkerClusterer"], function () {
            cluster = new AMap.MarkerClusterer(mapObj, markers, { styles: sts });
        });
    }
    else {
        mapObj.plugin(["AMap.MarkerClusterer"], function () {
            cluster = new AMap.MarkerClusterer(mapObj, markers);
        });
    }
}

function GeneratePoint(selectedCar) {
    //
    var selectedMarkerLng = null;
    var selectedMarkerLat = null;

    //
    mapObj.removeControl(markers);

    //
    markers.splice(0, markers.length);

    //
    for (var i = 0; i < carsdata.length; i++) {

        var lngSpan = carsdata[i].Long;
        var latSpan = carsdata[i].Lati;
        var markerPosition = new AMap.LngLat(lngSpan, latSpan);

        //
        if (selectedCar == carsdata[i].CarNum) {
            selectedMarkerLng = lngSpan;
            selectedMarkerLat = latSpan;
        }
        

        var title = "车牌号：" + carsdata[i].CarNum + "\n";
        title += "车辆状态：" + carsdata[i].StatusStr;

        var alarm = 0;
        if (carsdata[i].AlarmStr == "无" || carsdata[i].AlarmStr == "正常") 
        //if (carsdata[i].Alarm == 0 && data[i].carsdata == 0 && data[i].carsdata == 0) 
        {
            alarm = 0;
        } 
        else if (carsdata[i].AlarmStr.indexOf("劫警") == -1) 
        {
                alarm = 1;
        }
        else 
        { 
            alarm = 2;
        }  

        var IconOptions = {
            size: new AMap.Size(32, 32),
            image: getIcon(carsdata[i].Heading, alarm, carsdata[i].OnlineStatus),        
            imageSize: new AMap.Size(32, 32),
            imageOffset: new AMap.Pixel(0, 0)
        };
        var myIcon = new AMap.Icon(IconOptions);

        //
        var marker = new AMap.Marker({
            //map:mapObj,
            position: markerPosition, //基点位置
            icon: myIcon, //marker图标，直接传递地址url
            offset: { x: -8, y: -34} //相对于基点的位置
        });
        marker.setTitle(title);
        markers.push(marker);

    }

    //
    addCluster(0);

    //
    if (selectedCar != null) {
        getAddress(selectedMarkerLng, selectedMarkerLat);
    }
}

//
function getAddress(longt, lat) {
    //
    var curPos = new AMap.LngLat(longt, lat);

    //逆地理编码
    MGeocoder.getAddress(curPos, function (status, result) {
        if (status === 'complete' && result.info === 'OK') {
            geocoder_CallBack(result);
        }
    });
    selectedMarkerPosition = curPos;
}

//回调函数
function geocoder_CallBack(data) {
    var address;

    //返回地址描述
    address = data.regeocode.formattedAddress;

    //
    //构建信息窗体中显示的内容
    var info = [];
    //info.push("<div style=\"padding:0px 0px 0px 4px;\">");
    var carNO = "车牌 : " + selectedCarNO;
    info.push(carNO);
    var addressRet = "地址 : " + address + "</div>";
    info.push(addressRet);

    //
    if (infoWindow != null) {
        if (infoWindow.getIsOpen() == true) {
            isAutoCloseInfoWindow = true;
            infoWindow.close();
        }
    }
 
    //
    infoWindow = new AMap.InfoWindow({
        content: info.join("<br/>"),
        autoMove: true,
        size: new AMap.Size(150, 0),
        offset: { x: 0, y: -30 }
    });
    infoWindow.open(mapObj, selectedMarkerPosition);
    //setMapCenter(selectedMarkerPosition.getLng(), selectedMarkerPosition.getLat());
    mapObj.setCenter(selectedMarkerPosition);
    mapObj.setZoomAndCenter(14, selectedMarkerPosition);

    //
    AMap.event.addListener(infoWindow, 'close', function () {
        if (isAutoCloseInfoWindow == false) {
            selectedCarNO = null;
        }
        isAutoCloseInfoWindow = false;
    });
}

var isStop = false;
function mZoomStart() {
    
}

function mZoomend() {
    
}


function completeEventHandler() {
    
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
//画圆
function drawYuan() {
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
    //设置多边形的属性
    var polygonOption = {
        strokeColor: "#FF33FF",
        strokeOpacity: 1,
        strokeWeight: 2
    };
    mouseTool.polygon(polygonOption);   //使用鼠标工具绘制多边形 
}



//var CarMarker;
function getCarMarker() {
    var CarMarker = new AMap.Marker({
        position: new AMap.LngLat(105, 40),
        icon: '../../Libs/themes/cars/che4_1.gif',
        offset: new AMap.Pixel(-20, -20),
        autoRotation: true
    });

    AMap.event.addListener(CarMarker, "click", showCarWinInfos);
    //CarMarker.hide();
    return CarMarker;
}

function getPolyline() {
    var polyline = new AMap.Polyline({
        map: mapObj,
        geodesic: true,
        path: [],
        zIndex:3,
        strokeColor: "#00A", //线颜色             
        strokeOpacity: 1, //线透明度             
        strokeWeight: 3, //线宽             
        strokeStyle: "solid"//线样式         
    });
    return polyline;
}



function setMapCenter(longt,lat) {
    mapObj.setZoomAndCenter(14,new AMap.LngLat(longt, lat));
}

function getIcon(s_fx, s_alarm, s_onlinsta) {
    var returnflag = GetQueryString("returnflag");
    if (returnflag == 'cm') {
        return getIcon2(s_fx, s_alarm, s_onlinsta)
    }
    else {
        var fxf = "_styles/images/huinorth32.png";
        if (s_onlinsta == 2) {
            if (s_fx >= 0 && s_fx < 22.5 || s_fx > 337.5 && s_fx <= 360) {
                fxf = "_styles/images/huinorth32.png";
            } else if (22.5 < s_fx && s_fx < 67.5) {
                fxf = "_styles/images/huieastnorth32.png";
            }
            else if (67.5 < s_fx && s_fx < 112.5) {
                fxf = "_styles/images/huieast32.png";
            }
            else if (112.5 < s_fx && s_fx < 157.5) {
                fxf = "_styles/images/huieastsouth32.png";
            }
            else if (157.5 < s_fx && s_fx < 202.5) {
                fxf = "_styles/images/huisouth32.png";
            }
            else if (202.5 < s_fx && s_fx < 247.5) {
                fxf = "_styles/images/huiwestsouth32.png";
            }
            else if (247.5 < s_fx && s_fx < 292.5) {
                fxf = "_styles/images/huiwest32.png";
            }
            else if (292.5 < s_fx && s_fx < 337.5) {
                fxf = "_styles/images/huiwestnorth32.png";
            }
            return fxf;
        }
        if (s_fx >= 0 && s_fx < 22.5 || s_fx > 337.5 && s_fx <= 360) {
            if (s_alarm == 0) {
                fxf = "_styles/images/bluenorth32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yellownorth32.png";
            }
            else {
                fxf = "_styles/images/rednorth32.png";
            }
        }
        else if (22.5 < s_fx && s_fx < 67.5) {
            if (s_alarm == 0) {
                fxf = "_styles/images/blueeastnorth32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yelloweastnorth32.png";
            }
            else {
                fxf = "_styles/images/redeastnorth32.png";
            }
        }
        else if (67.5 < s_fx && s_fx < 112.5) {
            if (s_alarm == 0) {
                fxf = "_styles/images/blueeast32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yelloweast32.png";
            }
            else {
                fxf = "_styles/images/redeast32.png";
            }
        }
        else if (112.5 < s_fx && s_fx < 157.5) {
            if (s_alarm == 0) {
                fxf = "_styles/images/blueeastsouth32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yelloweastsouth32.png";
            }
            else {
                fxf = "_styles/images/redeastsouth32.png";
            }
        }
        else if (157.5 < s_fx && s_fx < 202.5) {
            if (s_alarm == 0) {
                fxf = "_styles/images/bluesouth32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yellowsouth32.png";
            }
            else {
                fxf = "_styles/images/redsouth32.png";
            }
        }
        else if (202.5 < s_fx && s_fx < 247.5) {
            if (s_alarm == 0) {
                fxf = "_styles/images/bluewestsouth32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yellowwestsouth32.png";
            }
            else {
                fxf = "_styles/images/redwestsouth32.png";
            }
        }
        else if (247.5 < s_fx && s_fx < 292.5) {
            if (s_alarm == 0) {
                fxf = "_styles/images/bluewest32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yellowwest32.png";
            }
            else {
                fxf = "_styles/images/redwest32.png";
            }
        }
        else if (292.5 < s_fx && s_fx < 337.5) {
            if (s_alarm == 0) {
                fxf = "_styles/images/bluewestnorth32.png";
            }
            else if (s_alarm == 1) {
                fxf = "_styles/images/yellowwestnorth32.png";
            }
            else {
                fxf = "_styles/images/redwestnorth32.png";
            }
        }
        return fxf;
    }
}

function getIcon2(s_fx, s_alarm, s_onlinsta) {
    var fxf = "_styles/images/灰-北32.png";
    if (s_onlinsta == 2) {
        if (s_fx >= 0 && s_fx < 22.5 || s_fx > 337.5 && s_fx <= 360) {
            fxf = "_styles/images/灰-北32.png";
        } else if (22.5 < s_fx && s_fx < 67.5) {
            fxf = "_styles/images/灰-东北32.png";
        }
        else if (67.5 < s_fx && s_fx < 112.5) {
            fxf = "_styles/images/灰-东32.png";
        }
        else if (112.5 < s_fx && s_fx < 157.5) {
            fxf = "_styles/images/灰-东南32.png";
        }
        else if (157.5 < s_fx && s_fx < 202.5) {
            fxf = "_styles/images/灰-南32.png";
        }
        else if (202.5 < s_fx && s_fx < 247.5) {
            fxf = "_styles/images/灰-西南32.png";
        }
        else if (247.5 < s_fx && s_fx < 292.5) {
            fxf = "_styles/images/灰-西32.png";
        }
        else if (292.5 < s_fx && s_fx < 337.5) {
            fxf = "_styles/images/灰-西北32.png";
        }
        return fxf;
    }
    if (s_fx >= 0 && s_fx < 22.5 || s_fx > 337.5 && s_fx <= 360) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-北32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-北32.png";
        }
        else {
            fxf = "_styles/images/红-北32.png";
        }
    }
    else if (22.5 < s_fx && s_fx < 67.5) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-东北32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-东北32.png";
        }
        else {
            fxf = "_styles/images/红-东北32.png";
        }
    }
    else if (67.5 < s_fx && s_fx < 112.5) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-东32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-东32.png";
        }
        else {
            fxf = "_styles/images/红-东32.png";
        }
    }
    else if (112.5 < s_fx && s_fx < 157.5) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-东南32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-东南32.png";
        }
        else {
            fxf = "_styles/images/红-东南32.png";
        }
    }
    else if (157.5 < s_fx && s_fx < 202.5) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-南32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-南32.png";
        }
        else {
            fxf = "_styles/images/红-南32.png";
        }
    }
    else if (202.5 < s_fx && s_fx < 247.5) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-西南32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-西南32.png";
        }
        else {
            fxf = "_styles/images/红-西南32.png";
        }
    }
    else if (247.5 < s_fx && s_fx < 292.5) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-西32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-西32.png";
        }
        else {
            fxf = "_styles/images/红-西32.png";
        }
    }
    else if (292.5 < s_fx && s_fx < 337.5) {
        if (s_alarm == 0) {
            fxf = "_styles/images/蓝-西北32.png";
        }
        else if (s_alarm == 1) {
            fxf = "_styles/images/红-西北32.png";
        }
        else {
            fxf = "_styles/images/红-西北32.png";
        }
    }
    return fxf;
}
