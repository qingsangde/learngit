var CID_sf;
var stagrid;
var chart1;
/*
初始化
*/
$(function () {
    stagrid = $('#data_grid');
    UserCookie = GetUserInfo();
    if (UserCookie.OneCarUser == "1") {
        CID_sf = UserCookie.UID.toString();
        $('#txtCarNo').val(UserCookie.UName.toString());
        $("#tb-selcar").attr({ "disabled": "disabled" });
        $("#tb-selcar").css("display", "none");  
    }
    var myDate = new Date();
    var st = myDate.format("yyyy-MM") + "-01 00:00:00";
    var et = myDate.format("yyyy-MM-dd hh:mm:ss");
    $("#STime").datetimebox('setValue', st);
    $("#ETime").datetimebox('setValue', et);
    document.getElementById("dagrid").style.display = "";
    document.getElementById("tudiv").style.display = "none";
   

    
});

//查询车辆行驶速度
function queryData_ss() {
//    document.getElementById("dagrid").style.display = "none";
//    document.getElementById("tu").style.display = "";

    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();

    var STime = $('#STime').datetimebox('getValue');
    var ETime = $('#ETime').datetimebox('getValue');
    if (!ComYue(STime, ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }
//    if (!dateComparison(STime, ETime)) {
//        $.messager.alert("操作提示", "查询时间不能大于31天！", "error");
//        return;
//    }
    document.getElementById('tudiv').innerHTML = "";
    document.getElementById('tudiv').innerHTML = "<canvas id='chartCanvas13' width='800' height='480' style='padding:40px;'> 很多小孩的浏览器不支持HTML5，你的也不例外！</canvas>";
    document.getElementById("dagrid").style.display = "";
    document.getElementById("tudiv").style.display = "none";
    var mydata = { "sid": "sta-searhspeed-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "Page": "1", "Size": defultSize, "stime": STime, "etime": ETime, "cid": CID_sf };
    BindDataGrid_FrontPage_SearchSpeed(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//查询车辆行驶速度导出
function Export() {
    
    var UserCookie = GetUserInfo();
    var STime = $('#STime').datetimebox('getValue');
    var ETime = $('#ETime').datetimebox('getValue');
    if (!dateComparison(STime, ETime)) {
        $.messager.alert("操作提示", "导出时间不能跨越月份！", "error");
        return;
    }

//    var STime = '2013-07-01';
//    var ETime = '2013-07-08';

    var mydata = { "sid": "sta-searhspeed-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "Page": "1", "Size": defultSize, "stime": STime, "etime": ETime, "cid": CID_sf };
    ExcelExport(mydata);
}


//单选
function selectCarSingle() {
    CarSelect(true, CallBackFun);
}
//选车回调
function CallBackFun(obj) {
    CID_sf = "";
    CID_sf = obj[0].cid.toString();
    var carNOs = obj[0].carno.toString();
    $('#txtCarNo').val(carNOs.toString());
}
//显示数据图表
function tuShow() {
    //document.getElementById('dd').innerHTML = "";
        document.getElementById("dagrid").style.display = "none";
        document.getElementById("tudiv").style.display = "";
////        var ssssss = alldata;
//        chart1.data = [70, 25, 10, 8, 6, 5, 2, 2];

        //chart1.draw();
    }
//显示数据列表
function lbShow() {
        document.getElementById("dagrid").style.display = "";
        document.getElementById("tudiv").style.display = "none";

}




/*
绑定DataGrid 前端分页
UrlData：URL参数
CallBack：回调函数
Async：同步异步
*/
function BindDataGrid_FrontPage_SearchSpeed(UrlData, CallBack, Async) {
    $('#loading-track').window('open');
    var async = true;
    if (Async == false) {
        async = false;
    }
    var demo = O2String(UrlData);
    var rootpath = getBasePath();
    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        dataType: 'json',
        data: demo,
        success: function (resdata) {

            if (resdata.state == 104) {
                LoginTimeout('服务器超时！');
            }
            if (resdata.result != null && resdata.result != "") {
                var arr = resdata;
                //                if (resdata.state == 104) {
                //                    LoginTimeout('服务器超时！');
                //                }
                //                else {

                alldata = arr.result.records;
                if (arr.result.total == 0) {
                    $.messager.alert("提示信息", "未检索到数据！");
                    $('#loading-track').window('close');
                    return;
                }
                var pg = $('#data_grid').datagrid("getPager");

                var ta = 0;
                var tb = 0;
                var tc = 0;
                var td = 0;
                var te = 0;
                var tf = 0;
                var tg = 0;
                var th = 0;
                for (var i = 0; i < arr.result.total; i++) {
                    var lsbl = alldata[i].T_Speed;
                    if (lsbl >= 0 && lsbl < 21) {
                        ta++;
                    } else if (lsbl > 20 && lsbl < 41) {
                        tb++;
                    }
                    else if (lsbl > 40 && lsbl < 61) {
                        tc++;
                    }
                    else if (lsbl > 60 && lsbl < 81) {
                        td++;
                    }
                    else if (lsbl > 80 && lsbl < 101) {
                        te++;
                    }
                    else if (lsbl > 100 && lsbl < 121) {
                        tf++;
                    }
                    else if (lsbl > 120 && lsbl < 141) {
                        tg++;
                    }
                    else if (lsbl > 140) {
                        th++;
                    }
                }
                chart1 = new AwesomeChart('chartCanvas13');
                chart1.title = "行驶速度分析柱状图";
                chart1.labels = ['0-20(Km/h)', '21-40(Km/h)', '41-60(Km/h)', '61-80(Km/h)', '81-100(Km/h)', '101-120(Km/h)', '121-140(Km/h)', '>140(Km/h)'];
                chart1.colors = ['#006CFF', '#FF6600', '#34A038', '#945D59', '#93BBF4', '#F493B8'];
                chart1.randomColors = true;
                chart1.data = [ta, tb, tc, td, te, tf, tg, th];
                chart1.draw();

                $("#data_grid").datagrid("loadData", alldata.slice(0, 20));
                $('#loading-track').window('close');

                pg.pagination('refresh', {
                    total: alldata.length,
                    pageNumber: 1,
                    pageSize: 20
                });
                CallBack(async, alldata);
                // }
            } else {
                $.messager.alert("提示信息", resdata.msg.toString());
            }
        }
    });
}
