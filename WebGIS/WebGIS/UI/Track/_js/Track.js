String.prototype.trim = function () {

    return this.replace(/(^\s*)|(\s*$)/g, '');
}
var myPlayer;
$(function () {

    MapInit();
    myPlayer = new TrackPlayer();

    //滚轮事件
    $('#sddd').bind('mousewheel', doMouseWheel);
    $('#isouth').bind('mousewheel', doMouseWheel);

    //事件初始化
    var nowDate = new Date();
    var preDate = new Date(nowDate.getTime() - 24 * 60 * 60 * 1000);  //前一天


    $('#txtStime').datetimebox('setValue', preDate.format("yyyy-MM-dd hh:mm:ss"));
    $('#txtEtime').datetimebox('setValue', nowDate.format("yyyy-MM-dd hh:mm:ss"));

    //    $('#txtStime').datetimebox('setValue', "2014-11-01 13:57:33");
    //    $('#txtEtime').datetimebox('setValue', "2014-11-02 02:47:02");

    $('#dg').datagrid({
        onClickRow: function (rowIndex, rowData) {
            showSelectData(rowData.no)
        }
    });
    loadParentSelectCar();
    isSingle();
});
function loadParentSelectCar() {
    var car = parent.SelectCar;
    //alert(car.CarNum + ':' + car.Carid);
    setCarNo(car.CarNum.trim(), car.Carid);

}
//单车用户自选
function isSingle() {
    var UserCookie = GetUserInfo();
    if (UserCookie.OneCarUser == "1") {
        $('#selCarNo').combobox('setValue', UserCookie.UID);
        $('#selCarNo').combobox('setText', UserCookie.UName);

        $('#selCarNo').combobox('disable');
    }
}
//定位监控选择
function setCarNo(carno, cid) {
    if (carno != "" && cid != "") {
        $('#selCarNo').combobox('setValue', cid);
        $('#selCarNo').combobox('setText', carno);
    }
}

//------------------界面事件控制模型--------------
function showGridRows() {
    $('#w').window('open');
}
function showRows(ev) {
    if ($(ev)[0].checked) {
        $('#dg').datagrid('showColumn', $(ev)[0].name);
    }
    else {
        $('#dg').datagrid('hideColumn', $(ev)[0].name);
    }
}

//-------------模糊匹配查询车辆-----begin------
var carno
function autoQuery() {
    carno = $('#selCarNo').combobox('getText');
    if (carno.length > 2) {
        var UserCookie = GetUserInfo();
        var mydata = { "sid": "track-selectcars", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "carno": carno };
        BindData_FrontPage(mydata, setAutoQueryData);
    }
}
function setAutoQueryData(data) {
    $('#selCarNo').combobox('clear');
    if (data.total > 0) {

        $('#selCarNo').combobox('loadData', data.records);
        $('#selCarNo').combobox('setText', carno);
    }
}
//-------------模糊匹配查询车辆------end------

//查询按钮事件
var myParam;
function doOK() {
    //    EndM.setCenter();
    //    EndM.hide();

    doStop();
    myParam = getParams();
    //获取界面查询条件
    loadTrack();
}

//导出
function exportExcel() {
    if (myPlayer.oData != null) {
        //发送ajax请求
        var UserCookie = GetUserInfo();
        var mydata = { "sid": "track-exporttracks", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),
            "cid": myParam.cid,
            "cno": myParam.cno,
            "st": myParam.st,
            "et": myParam.et,
            "os": myParam.os,
            "od": myParam.od,
            "of": "1"
        };
        ExcelExportEX(mydata);
    }
    else {
        $.messager.alert('系统提示', '没有可导出的轨迹数据！');
    }
}

function ExcelExportEX(UrlData) {
    $('#ww').window('open');
    var async = true;
    var demo = O2String(UrlData);
    var rootpath = getBasePath();

    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        //timeout: 120000,
        dataType: 'json',
        error: function (d) {
            $('#ww').window('close');
            $.messager.alert("查询失败", "错误代码：" + d.status + "  " + d.statusText);
        },
        data: demo,
        success: function (resdata) {
            $('#ww').window('close');
            if (resdata != null && resdata != "") {
                var arr = resdata;
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                }
                else {
                    if (resdata.state == 100) {
                        var asa = rootpath + '\\' + resdata.result;
                        window.open(asa, '_self');
                        //$.messager.alert("操作提示", resdata.result, "ok");
                    }
                    else {
                        $.messager.alert("操作失败", resdata.msg, "ok");
                    }
                }
            }
        }
    });
}

//请求轨迹
function loadTrack() {
    if (myParam.cid == "") {
        $.messager.alert('系统提示', '请选择车牌号！');
    }
    else if (myParam.cno == "") {
        $.messager.alert('系统提示', '请选择车牌号！');
    }
    else if (myParam.st == "") {
        $.messager.alert('系统提示', '请输入开始时间！');
    }
    else if (myParam.et == "") {
        $.messager.alert('系统提示', '请输入结束时间！');
    }
    else if (myParam.os == "") {
        $.messager.alert('系统提示', '请输入超速阀值！');
    }
    else if (myParam.od == "") {
        $.messager.alert('系统提示', '请输入漂移阀值！');
    }
    else {
        $('#ww').window('open');
        //发送ajax请求
        var UserCookie = GetUserInfo();
        var mydata = { "sid": "track-gettracks", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),
            "cid": myParam.cid,
            "cno": myParam.cno,
            "st": myParam.st,
            "et": myParam.et,
            "os": myParam.os,
            "od": myParam.od,
            "of": "0"
        };
        BindData_FrontPage(mydata, setTrackData);
    }
}
//请求轨迹回调
function setTrackData(data) {

    if (data.total > 0) {
        myPlayer.clear();
        myPlayer.loadData(data.records);
        if (data.total == 10000) {
            $.messager.show({
                title: '消息提示',
                msg: '您所查询的轨迹已超出1万条，系统将自动调取最近1万条数据!',
                timeout: 5000,
                showType: 'slide'
            });
        }
    }
    else {
        $.messager.alert('系统提示', '没有轨迹数据！');
    }
    $('#ww').window('close');
}

//---------播放控制部分-----
//播放按钮
function doPlay() {
    Play();
}

//暂停
function doPause() {
    window.clearTimeout(myPlayer.oTask);
    myPlayer.oTask = false;
}
//停止
function doStop() {
    window.clearTimeout(myPlayer.oTask);
    myPlayer.clear();
}
//加速播放
function doAddSP() {
    if (myPlayer.oTimer > 100) {
        myPlayer.oTimer = myPlayer.oTimer - 100;

        //var y = Number($("#tdPP")[0].innerText);

        $("#tdPP").empty();
        $("#tdPP").append(myPlayer.oTimer / 1000);

    }
}

//减速播放
function doCutSP() {
    if (myPlayer.oTimer < 1000) {
        myPlayer.oTimer = myPlayer.oTimer + 100;
        //var y = Number($("#tdPP")[0].innerText);
        $("#tdPP").empty();
        $("#tdPP").append(myPlayer.oTimer / 1000);

    }
}


//递归方法
function Play() {
    if (myPlayer.oData) {
        if (myPlayer.oFrame < myPlayer.oData.length) {
            myPlayer.oFrame++;
            $('#sProgress').slider('setValue', myPlayer.oFrame);
            myPlayer.setCarMarker();
            addOneRow();
            myPlayer.oTask = window.setTimeout(Play, myPlayer.oTimer);
        }
        else {
            window.clearTimeout(myPlayer.oTask);
            myPlayer.oTask = false;
            $.messager.show({
                title: '消息提示',
                msg: '轨迹回放完毕！',
                timeout: 3000,
                showType: 'slide'
            });
        }
    }
}

function getPlaySpeed() {
    var val = $('#sSpeed').spinner('getValue');
    return Math.round(1000 / val);
}

function doMouseWheel(event, delta) {
    if (myPlayer.oData) {
        var iVal = myPlayer.oFrame + delta;
        if (iVal < 1) {
            return false;
        }
        else if (iVal > myPlayer.oData.length) {
            return false;
        }
        else {
            if (delta == 1) {
                myPlayer.oFrame++;
                $('#sProgress').slider('setValue', myPlayer.oFrame);
                myPlayer.setCarMarker();
                addOneRow();
            }
            else if (delta == -1) {
                myPlayer.oFrame--;
                $('#sProgress').slider('setValue', myPlayer.oFrame);
                myPlayer.setCarMarker();
                myPlayer.setGrid();
            }
            else {
                return false;
            }
        }
    }
}


//逐条增加
function addOneRow() {
    var obj = myPlayer.getOFrameData();
    $('#dg').datagrid('insertRow', {
        index: 0, // 索引从0开始
        row: obj
    });
    var i = $('#dg').datagrid('getRows').length;
    if (i > myPlayer.oPageNo) {
        $('#dg').datagrid('deleteRow', (i - 1));
    }
    $('#dg').datagrid('selectRow', 0);
}





//----------------显示控制部分
//终端超速显示
function showOS1Map(ev) {

    $("#chkOS2").attr("checked", false);
    $("#chkOD").attr("checked", false);

    if ($(ev)[0].checked) {
        cluster.setMarkers(myPlayer.oOS1Arr);
    }
    else {
        cluster.removeMarkers(myPlayer.oOS1Arr);
    }
}

//计算超速显示
function showOS2Map(ev) {
    $("#chkOS1").attr("checked", false);
    $("#chkOD").attr("checked", false);
    if ($(ev)[0].checked) {
        cluster.setMarkers(myPlayer.oOS2Arr);
    }
    else {
        cluster.clearMarkers();
    }
}

//计算漂移显示
function showODMap(ev) {
    $("#chkOS1").attr("checked", false);
    $("#chkOS2").attr("checked", false);
    if ($(ev)[0].checked) {
        cluster.setMarkers(myPlayer.oODArr);
    }
    else {
        cluster.removeMarkers(myPlayer.oODArr);
    }
}

//地址解析
function showAdd(ev) {
    var obj = myPlayer.getData(1);
    if (obj != null) {
        //解析地址
        if (obj.add == "") {
            $.messager.show({
                title: '消息提示',
                msg: '马上为您解析地址，请稍等！',
                timeout: 3000,
                showType: 'slide'
            });
            //发送ajax请求
            var UserCookie = GetUserInfo();
            var mydata = { "sid": "track-gettracks", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),
                "cid": myParam.cid,
                "cno": myParam.cno,
                "st": myParam.st,
                "et": myParam.et,
                "os": myParam.os,
                "od": myParam.od,
                "of": "1"
            };
            BindData_FrontPage(mydata, addAddrs);
        }
    }
    if ($(ev)[0].checked) {
        $('#dg').datagrid('showColumn', 'add');
    }
    else {
        $('#dg').datagrid('hideColumn', 'add');
    }
}

function addAddrs(data) {
    if (data.total > 0) {
        myPlayer.oData = data.records;
    }
    else {
        $.messager.alert('系统提示', '地址解析错误！');
    }
}

//------------------------播放器模型---------------------
//异常数据点点击事件
function showWinInfos(ev) {
    showSelectData(ev.target.getExtData())
}

function showSelectData(iNo) {
    myPlayer.oFrame = parseInt(iNo);
    //myPlayer.showWindowInfo();
    myPlayer.setGrid();
    myPlayer.setCarMarker();
}

//车辆标志点击事件
function showCarWinInfos() {
    myPlayer.showWindowInfo();
    myPlayer.setGrid();
}

//信息窗口地址解析
function getInfoWinContent(obj) {
    var info = [];
    info.push("<div style=\"padding:0px 0px 0px 0px;\"><b>" + obj.cno + "</b>");
    info.push("序号 : " + obj.no);
    info.push("时间 : " + obj.dt);
    info.push("速度: " + obj.sp + "km/h");
    if (obj.add != "") {
        info.push("地址: " + obj.add);
    }
    info.push("</div>");
    return info.join("<br/>");
}
