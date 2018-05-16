var stagrid;
var ms_STime;
var ms_ETime;
var alldataInfo;
var CIDS;
var sel_CID;
var sel_alarmtype;
var UserCookie;


var detailrows;
var curcid;
var ImgUrl;
var curitemid;
/*
初始化
*/
$(function () {
    UserCookie = GetUserInfo();
//    if (UserCookie.OneCarUser == "1") {
//        CIDS = UserCookie.UID.toString();
//        $('#txtCarNo').val(UserCookie.UName.toString());
//        $("#tb-selcar").attr({ "disabled": "disabled" });
//        $("#tb-selcar").css("display", "none");
//    }
    var myDate = new Date();
    var st = myDate.format("yyyy-MM-dd") + " 00:00:00";
    var et = myDate.format("yyyy-MM-dd") + " 23:59:59";
    $("#STime").datetimebox('setValue', st);
    $("#ETime").datetimebox('setValue', et);
    stagrid = $('#data_grid');
    initgrid();
});

//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (value.toString() == "0") {
                $.messager.alert("操作提示", "本条记录无统计明细！", "error");
                return;
            }
            if (field.toString() == "pcount") {
                sel_CID = getRow(rowIndex)["CID"].toString();
                //alert(sel_CID);
                querycarphotos(sel_CID);
            }
        }
    });
}

function queryData_Ms() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    //$('#STime').datebox('setValue','2013-08-01');
    //$('#ETime').datebox('setValue', '2013-08-20');
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    ms = 1 * 24 * 60 * 60 * 1000;
    if (!dateCompare(ms_STime, ms_ETime, ms)) {
        $.messager.alert('操作提示', '历史照片查询时间跨度不能大于24小时！', 'error');
        return;
    }
    var tdh = $("#txtTdh").combobox('getValue');
    //    ms_STime = '2013-08-01';
    //    ms_ETime = '2013-08-20';

    //var CID = '18,12,1,2,3,4,5,6,7,8,9';
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-historyphotos-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "carnum": $("#txtCph").val(), "cuid": $("#txtClyt").val(), "carownname": $("#txtSsqy").val(), "line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString(), "tdh": tdh };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

function CompressExport() {
    var ImgCompressUrl="";
    var key = UserCookie["sysflag"].toString();
    if (key == "JFJY") {
        ImgCompressUrl = "http://dptest.qm.cn:3779"; //内网：10.44.30.67:9009
    }
    else if (key == "HRBKY") {
        ImgCompressUrl = "http://dptest.qm.cn:3799"; //内网：10.44.30.121:9009;
    }
    else if (key == "HRBHY") {
        ImgCompressUrl = "http://dptest.qm.cn:3993";  //内网：10.44.30.59:9009
    }
    else if (key == "YQWL") {
        ImgCompressUrl = "http://dptest.qm.cn:3789"; //内网：10.44.30.57:9009
    }

    var uid = UserCookie["UID"].toString();
    var carno = $("#txtCph").val();
    var carownname = $("#txtSsqy").val();
    var cuid = $("#txtClyt").val();
    var line = $("#txtYyxl").val();
    var st = Date.parse($('#STime').datetimebox('getValue').toString().replace(" ","T"))/1000;
    var et = Date.parse($('#ETime').datetimebox('getValue').toString().replace(" ", "T"))/1000;
    var tdh = $("#txtTdh").combobox('getValue');
    var openurl = ImgCompressUrl + "/UI/PhotosDown.htm?key=" + key + "&uid=" + uid + "&carno=" + carno + "&carownname=" + carownname + "&cuid=" + cuid + "&line=" + line + "&st=" + st + "&et=" + et + "&tdh=" + tdh;

    window.open(openurl, 'newwindow', 'height=600,width=800,top=100,left=100,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no');
}

function getRow(index) {
    var data = stagrid.datagrid("getRows");
    if (data) {
        if (data.length > index) {
            return data[index];
        }
        else {
            return "";
        }
    }
    else {
        return "";
    }
}


//查看照片 begin
function querycarphotos(selcid) {
    $('#loading-photo').window('open');
    //照片查看小窗体
    $('#dgdetail').datagrid({
        nowrap: true, rownumbers: false, border: false,
        fit: true, fitColumns: true, pagination: false, singleSelect: true,
        idField: "CID", showHeader: false,
        columns: [[
		{
		    field: "CPath", title: "照片缩略图", width: 85, align: "left",
		    formatter: function (value, row, index) { return SetImgDetail(row); }
		}
	]]
    });

    var sysflag = UserCookie["sysflag"].toString();
    if (sysflag == "JFJY") {
        ImgUrl = "http://dptest.qm.cn:3006";
    }
    else if (sysflag == "HRBKY") {
        ImgUrl = "http://dptest.qm.cn:3007";
    }
    else if (sysflag == "HRBHY") {
        ImgUrl = "http://dptest.qm.cn:3992";
    }
    else if (sysflag == "YQWL") {
        ImgUrl = "http://dptest.qm.cn:3008";
    }
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    var tdh = $("#txtTdh").combobox('getValue');
    var mydata = {
        "sid": "sta-historyphotos-onecar",
        "sysuid": UserCookie["UID"],
        "sysflag": sysflag,
        "token": UserCookie["token"].toString(),
        "cid": selcid,
        "stime": ms_STime, 
        "etime": ms_ETime,
        "tdh": tdh
    };
    BaseGetData(mydata, setDetailGrid);
}

function setDetailGrid(data) {
    $('#loading-photo').window('close');
    $('#photodetails').window('open');
    if (data.state == 100) {
        detailrows = data.result.records;
        $('#curImg').attr("src", GetImgUrl(detailrows[0].CPath) + ".JPEG");
        $('#spanCarNo')[0].innerHTML = detailrows[0].CarNo;
        $('#spanOwnName')[0].innerHTML = detailrows[0].CarOwnName;
        $('#spanTime')[0].innerHTML = detailrows[0].createtime;
        $('#hidCurIndex').attr("value", 0);

        $('#hidTotal').attr("value", detailrows.length);
        $('#dgdetail').datagrid('loadData', detailrows);
        $('#dgdetail').datagrid({
            onClickRow: function (rowIndex, rowData) {
                $('#hidCurIndex').attr("value", rowIndex);
                $('#curImg').attr("src", GetImgUrl(rowData.CPath) + ".JPEG");
                $('#spanCarNo')[0].innerHTML = rowData.CarNo;
                $('#spanOwnName')[0].innerHTML = rowData.CarOwnName;
                $('#spanTime')[0].innerHTML = rowData.createtime;
            }
        });
        $('#dgdetail').datagrid('scrollTo', 0);
        $('#dgdetail').datagrid('selectRow', 0);
       
    }
    else {
        if (data.state == 104) {
            LoginTimeout('服务器超时！');
        }
        else {
            $.messager.alert('提示', data.msg, 'error');
        }
    }
}


function GoBack() {
    var curIndex = parseInt($('#hidCurIndex').val());
    if (curIndex <= 0) {
        $.messager.alert("系统提示：", "当前已经是第一张！", "error");
        return;
    }
    else {
        curIndex = curIndex - 1;
        $('#hidCurIndex').attr("value", curIndex);
        $('#curImg').attr("src", GetImgUrl(detailrows[curIndex].CPath) + ".JPEG");
        $('#spanCarNo')[0].innerHTML = detailrows[curIndex].CarNo;
        $('#spanOwnName')[0].innerHTML = detailrows[curIndex].CarOwnName;
        $('#spanTime')[0].innerHTML = detailrows[curIndex].createtime;
        $('#dgdetail').datagrid('scrollTo', curIndex);
        $('#dgdetail').datagrid('selectRow', curIndex);
    }
}

function GoForward() {
    var curIndex = parseInt($('#hidCurIndex').val());
    var total = parseInt($('#hidTotal').val());
    if (curIndex >= total - 1) {
        $.messager.alert("系统提示：", "当前已经是最后一张！", "error");
        return;
    }
    else {
        curIndex = curIndex + 1;
        $('#hidCurIndex').attr("value", curIndex);
        $('#curImg').attr("src", GetImgUrl(detailrows[curIndex].CPath) + ".JPEG");
        $('#spanCarNo')[0].innerHTML = detailrows[curIndex].CarNo;
        $('#spanOwnName')[0].innerHTML = detailrows[curIndex].CarOwnName;
        $('#spanTime')[0].innerHTML = detailrows[curIndex].createtime;
        $('#dgdetail').datagrid('scrollTo', curIndex);
        $('#dgdetail').datagrid('selectRow', curIndex);
    }
}

function SetImgDetail(row) {
    var img = "<input type='image' src='" + GetImgUrl(row.CPath) + ".JPEG" + "' style='width:75px; height:56px; padding:3px;cursor:pointer;' />"
    return img;
}

//将图片查询结果转换成真实的图片地址
function GetImgUrl(cpath) {

    return ImgUrl + cpath.replace(" ", "%20");
}
