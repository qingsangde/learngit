//--------------查询条件部分------------------------

function getParams() {
    this.cid = $('#selCarNo').combobox('getValue');
    this.cno = $('#selCarNo').combobox('getText');
    this.st = $('#txtStime').datetimebox('getValue');
    this.et = $('#txtEtime').datetimebox('getValue');
    this.os =Number($('#txtSpeed').numberspinner('getValue'));     //超速阀值
    this.od =Number($('#txtDistance').numberspinner('getValue'));  //漂移阀值

    $('input[name="chk"]:checked').each(function () {
        $(this).attr("checked", false);
    });
    return this;
}


//--------------播放器控制部分------------------------

function TrackPlayer() {
    this.oTask = false;         //播放状态 false:停止;true:播放
    this.oFrame = 0;            //当前播放进度
    this.oData = null;          //轨迹数据
    this.oPageNo = 10;          //数据列表显示行数

    this.StartM = getM(0);
    this.EndM = getM(1);

    this.oTimer = 1000;

    this.oOS1Arr = [];
    this.oOS2Arr = [];
    this.oODArr = [];
    this.oPolylineArr = [];
    this.oCarMarker = getCarMarker();   //车辆地图标志
    this.oInfoWindow = getInfoWindow(); //地图信息窗体
    this.oPolyline = getPolyline();     //地图折线 
}

//清空界面
TrackPlayer.prototype.clear = function () {
    mapObj.clearMap();          //地图清空
    cluster.clearMarkers();     //清除聚合点
    this.oTask = false;         //播放状态 false:停止;true:播放
    this.oFrame = 0;            //当前播放进度
    //this.oTimer = 1000;
    this.oData = null;          //轨迹数据
    this.oCarMarker = getCarMarker();   //车辆地图标志
    this.oInfoWindow = getInfoWindow(); //地图信息窗体
    this.oPolyline = getPolyline();     //地图折线 
    this.oOS1Arr = [];
    this.oOS2Arr = [];
    this.oODArr = [];
    markers = [];

    //播放进度清空
    $('#sProgress').slider({
        min: 0,
        max: 0,
        value: 0,
        step: 10,
        rule: [1, 0]
    });

    //Grid清空
    $('#dg').datagrid('loadData', []);

    //单选框清空
    $('input[name="chk"]:checked').each(function () {
        $(this).attr("checked", false);
    });

    //统计信息
    $("#tdAll").empty();
    $("#tdOS1").empty();
    $("#tdOS2").empty();
    $("#tdOD").empty();
    $("#tdSP").empty();

    $("#tdAll").append("0条");
    $("#tdOS1").append("0条");
    $("#tdOS2").append("0条");
    $("#tdOD").append("0条");
}

//加载数据
TrackPlayer.prototype.loadData = function (data) {
    this.oData = data;
    if (this.oData.length > 0) {
        this.oFrame = 1;
        this.oOS1Arr = this.getOS1Arr();
        this.oOS2Arr = this.getOS2Arr();
        this.oODArr = this.getODArr();
        markers = [];
        //显示起始点
        this.setM();
        //绘制轨迹线;
        this.setTrackline();
        //地图显示第一点轨迹
        this.setCarMarker();
        //数据列表显示第一条数据
        this.setGrid();
        //地图中心点定位
        mapObj.setZoomAndCenter(10, new AMap.LngLat(this.oData[0].lng, this.oData[0].lat));
        //播放进度条
        $('#sProgress').slider({
            min: 1,
            max: data.length,
            value: 1,
            step: 10,
            rule: [1, data.length],
            onSlideEnd: function (value) {
                if (myPlayer.oTask == false) {
                    myPlayer.oFrame = value;
                    myPlayer.setCarMarker();
                    myPlayer.setGrid();
                }
            }
        });
        //统计信息
        $("#tdAll").empty();
        $("#tdOS1").empty();
        $("#tdOS2").empty();
        $("#tdOD").empty();
        $("#tdAll").append(data.length + "条");
        $("#tdOS1").append(this.oOS1Arr.length + "条");
        $("#tdOS2").append(this.oOS2Arr.length + "条");
        $("#tdOD").append(this.oODArr.length + "条");
    }
}
//设置起始点
TrackPlayer.prototype.setM = function () {
    //this.oData
    this.StartM.setPosition(new AMap.LngLat(this.oData[0].lng, this.oData[0].lat));
    this.EndM.setPosition(new AMap.LngLat(this.oData[(this.oData.length - 1)].lng, this.oData[(this.oData.length - 1)].lat));

    this.StartM.setMap(mapObj);
    this.EndM.setMap(mapObj);
}


//终端超速报警
TrackPlayer.prototype.getOS1Arr = function () {
    var arr = [];
    if (this.oData != null) {
        $(this.oData).each(function () {
            if (this.ta == "超速报警") {
                var mPos = new AMap.LngLat(this.lng, this.lat);
                var marker = new AMap.Marker({
                    position: mPos,
                    icon: "_styles/blueD.png",
                    offset: { x: -6, y: -6 },
                    extData: this.no
                });
                AMap.event.addListener(marker, "click", showWinInfos);
                arr.push(marker);
            }
        });
    }
    return arr;
}

//计算超速报警
TrackPlayer.prototype.getOS2Arr = function () {
    var arr = [];
    //设定阀值
    if (this.oData != null) {
        $(this.oData).each(function () {
            if (this.os) {
                var mPos = new AMap.LngLat(this.lng, this.lat);
                var marker = new AMap.Marker({
                    position: mPos,
                    icon: "_styles/redD.png",
                    offset: { x: -6, y: -6 },
                    extData: this.no
                });
                AMap.event.addListener(marker, "click", showWinInfos);
                arr.push(marker);
            }
        });
    }
    return arr;
}

//漂移统计报警
TrackPlayer.prototype.getODArr = function () {
    var arr = [];
    //设定阀值
    if (this.oData != null) {
        $(this.oData).each(function () {
            if (this.od) {
                var mPos = new AMap.LngLat(this.lng, this.lat);
                var marker = new AMap.Marker({
                    position: mPos,
                    icon: "_styles/greenD.png",
                    offset: { x: -6, y: -6 },
                    extData: this.no
                });
                AMap.event.addListener(marker, "click", showWinInfos);
                arr.push(marker);
            }
        });
    }
    return arr;
}

//设置轨迹数地图标记;
TrackPlayer.prototype.setCarMarker = function () {
    var obj = this.getOFrameData();
    if (obj) {
        var poi = new AMap.LngLat(obj.lng, obj.lat);
        var icon = getIcon(obj.dir); //车辆类型未定义----------需修改---------
        this.oCarMarker.setIcon(icon);
        this.oCarMarker.setPosition(poi);
        this.oCarMarker.setMap(mapObj);

        var mapBounds = mapObj.getBounds();
        if (!mapBounds.contains(poi)) { //在地图视野显示
            mapObj.setCenter(poi);
        }

        $("#tdSP").empty();
        $("#tdSP").append(this.oFrame + "/" + this.oData.length);
    }
}

//设置轨迹数据列表;
TrackPlayer.prototype.setGrid = function () {
    var arr = [];
    if (this.oFrame <= this.oPageNo) {
        arr = this.oData.slice(0, this.oFrame);
    }
    else {
        arr = this.oData.slice((this.oFrame - this.oPageNo), this.oFrame);
    }
    $('#dg').datagrid('loadData', arr.reverse());
    $('#dg').datagrid('selectRow', 0);
}

//绘制轨迹线;
TrackPlayer.prototype.setTrackline = function () {
    var polyline = new AMap.Polyline({
        map: mapObj,
        geodesic: true,
        path: getLineArr(this.oData),
        strokeColor: "#00A", //线颜色             
        strokeOpacity: 1, //线透明度             
        strokeWeight: 3, //线宽             
        strokeStyle: "solid"//线样式         
    });
    mapObj.setFitView();
}


//获取当前轨迹信息
TrackPlayer.prototype.getOFrameData = function () {
    return this.getData(this.oFrame);
}

//获取轨迹,i:轨迹编号;返回一条轨迹数据
TrackPlayer.prototype.getData = function (i) {
    if (this.oData != null || i >= this.oData.length) {
        return this.oData[i - 1];
    }
    else {
        return null;
    }
}


//显示信息窗口
TrackPlayer.prototype.showWindowInfo = function () {
    var obj = this.getOFrameData();
    if (obj) {
        var poi = new AMap.LngLat(obj.lng, obj.lat);
        var str = getInfoWinContent(obj, false);
        this.oInfoWindow.setContent(str);
        this.oInfoWindow.open(mapObj, poi);
    }
}


//轨迹画线抽虚算法------------
//获取轨迹线
function getLineArr(Data) {
    var arr = [];
    if (Data.length <= 1000) {
        arr = distinctArr(0.000001, Data);
    }
    else if (Data.length > 1000 && Data.length <= 1500) {
        arr = distinctArr(0.00005, Data);
    }
    else if (Data.length > 1500 && Data.length <= 2000) {
        arr = distinctArr(0.0001, Data);
    }
    else {
        arr = distinctArr(0.001, Data);
    }
    return arr;
}

//抽虚点
function distinctArr(diff, Data) {
    var arr = [];
    var lngX = parseFloat(Data[0].lng);
    var latY = parseFloat(Data[0].lat);
    arr.push(new AMap.LngLat(lngX, latY));
    var sX;
    var sY;
    for (var i = 1; i < Data.length; i++) {
        var NlngX = parseFloat(Data[i].lng);
        var NlatY = parseFloat(Data[i].lat);
        sX = Math.abs(NlngX - lngX);
        sY = Math.abs(NlatY - latY);
        if ((sX > diff) || (sY > diff)) {
            arr.push(new AMap.LngLat(NlngX, NlatY));
            lngX = NlngX;
            latY = NlatY;
        }
    }
    return arr;
}