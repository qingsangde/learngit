var UserCookie;

var OptionalCarsArr = new Array();
var FenceCarsArr = new Array();
var optionnalCarsFilter = new Array();
var selectedCarsFilter = new Array();
var selRegionId = 0;

$(function () {
    //$('#loading-track').window('close');
    UserCookie = GetUserInfo();
    MapInit();
    setPage();
});

function setPage() {
    InitHiddenField();
    LoadCombogrid();
    InitDataGrid();
}

function InitHiddenField() {
    $("#spanFence").hide();
}

function InitDataGrid() {
    $('#dgOptional').datagrid({
        title: '待选列表',
        border: false,
        collapsible: false, //是否可折叠的  
        fit: true, //自动大小  
        pagination: true, //分页控件
        autoRowHeight: false,
        rownumbers: true, //行号  
        pageSize: 10, //每页显示的记录条数，默认为5  
        pageList: [5, 10, 15]//可以设置每页记录条数的列表  
    });

    $('#dgSelected').datagrid({
        title: '已选列表',
        border: false,
        collapsible: false, //是否可折叠的  
        fit: true, //自动大小  
        pagination: true, //分页控件 
        autoRowHeight: false,
        rownumbers: true, //行号  
        pageSize: 10, //每页显示的记录条数，默认为5  
        pageList: [5, 10, 15]//可以设置每页记录条数的列表  
    });

    SetPager();


}

function SetPager() {
    $('#dgOptional').datagrid({ loadFilter: pagerFilter });
    $('#dgSelected').datagrid({ loadFilter: pagerFilter });

    var p = $('#dgOptional').datagrid('getPager');

    $(p).pagination({
        showRefresh: false,
        showPageList: false,
        beforePageText: '', //页数文本框前显示的汉字  
        afterPageText: '/{pages}',
        displayMsg: ''
    });
    var p1 = $('#dgSelected').datagrid('getPager');

    $(p1).pagination({
        showRefresh: false,
        showPageList: false,
        beforePageText: '', //页数文本框前显示的汉字  
        afterPageText: '/{pages}',
        displayMsg: ''
    });
}

function LoadCombogrid() {
    $("#divFilter").hide();
    var rname = "";
    var mydata = {
        "sid": "activeregion-getlist",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "regionname": rname
    };
    BaseGetData(mydata, setData);
    //$.post("GetFence.ashx", { key: key, fname: fname, almtype: almtype, graphtype: graphtype }, setData);
}

function setData(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            var g = $('#ff').combogrid('grid');
            g.datagrid('loadData', obj.result.records);
            $('#ff').combogrid({
                onClickRow: function (rowIndex, rowData) {
                    //alert("选择的grid中的数据如下：名称:" + rowData.Name + " 形状:" + rowData.GType + " 报警类型:" + rowData.AlmType + " 区域内容:" + rowData.Con);                            
                    selRegionId = rowData.R_Id;
                    $('#spanRname')[0].innerHTML = rowData.R_NAME;
                    $('#spanDealerName')[0].innerHTML = rowData.DEALERNAME;
                    $("#spanFence").show();
                    $("#divFilter").hide();
                    DrawCircle(rowData.R_CENTER_LNG, rowData.R_CENTER_LAT, rowData.R_RADIUS);
                    BindCarList();
                }
            });
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('活动区域信息查询，服务器超时！');
            }
        }
    }
}

function DrawCircle(lng, lat, radius) {
    addCircle(lat + "," + lng + ";" + radius);
}

function BindCarList() {
    $('#loading-track').window('open');
    var mydata = {
        "sid": "activeregion-getrelcar",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "uid": UserCookie["UID"],
        "rid": selRegionId
    };
    BaseGetData(mydata, setCarData);
}

function setCarData(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            selecttnos = [];
            OptionalCarsArr = obj.result.records[1];
            FenceCarsArr = obj.result.records[0];
            //alert("状态：" + data.Status);
            var p = $('#dgOptional').datagrid('getPager');

            $(p).pagination({
                pageNumber: 1
            });

            var p1 = $('#dgSelected').datagrid('getPager');

            $(p1).pagination({
                pageNumber: 1
            });
            BindGridData();
        }
    }
    $('#loading-track').window('close');
    $("#divFilter").show();
}

//绑定车辆列表
function BindGridData() {
    $('#dgOptional').datagrid('loadData', OptionalCarsArr);
    $('#dgSelected').datagrid('loadData', FenceCarsArr);
}

function DoSelection() {
    //选择车辆
    var rowindexArr = new Array();
    var rows = $('#dgOptional').datagrid('getSelections');
    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "未选中任何车辆！！！", "error");
        return;
    }
    else {
        for (var i = 0; i < rows.length; i++) {
            var index = getindex(OptionalCarsArr, rows[i]); // $('#dgOptional').datagrid('getRowIndex', rows[i]);
            rowindexArr.push(index);
            //            $('#dgSelected').datagrid('insertRow', {
            //                index: 0, // 索引从0开始
            //                row: rows[i]
            //            });
            FenceCarsArr.unshift(rows[i]);
            //selecttnos.push(rows[i].TNO);
        }
        if (rowindexArr.length > 0) {//将选择的行索引倒序排列，删除时倒序删除
            rowindexArr.sort(function (a, b) {
                return b - a
            });
            for (var j = 0; j < rowindexArr.length; j++) {
                //$('#dgOptional').datagrid('deleteRow', rowindexArr[j]);//从待选datagrid中删除已选择的车辆
                OptionalCarsArr.splice(rowindexArr[j], 1);
            }
        }
    }
    BindGridData();
}

function getindex(arr, rowdata) {
    if (arr != null && arr.length > 0) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].TNO == rowdata.TNO) {
                return i;
            }
        }
    }
    return -1;
}

function DoRevSelection() {
    //反选车辆
    var rowindexArr = new Array();
    var rows = $('#dgSelected').datagrid('getSelections');
    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "未选中任何车辆！！！", "error");
        return;
    }
    else {
        for (var i = 0; i < rows.length; i++) {
            var index = getindex(FenceCarsArr, rows[i]); //$('#dgSelected').datagrid('getRowIndex', rows[i]);
            rowindexArr.push(index);
            //            $('#dgOptional').datagrid('insertRow', {
            //                index: 0, // 索引从0开始
            //                row: rows[i]
            //            });
            OptionalCarsArr.unshift(rows[i]);
        }
        if (rowindexArr.length > 0) {//将选择的行索引倒序排列，删除时倒序删除
            rowindexArr.sort(function (a, b) {
                return b - a
            });
            for (var j = 0; j < rowindexArr.length; j++) {
                //$('#dgSelected').datagrid('deleteRow', rowindexArr[j]); //从已选datagrid中删除反选的车辆
                FenceCarsArr.splice(rowindexArr[j], 1);
            }
        }
    }
    BindGridData();
}

function DoFilter() {
    $('#loading-track').window('open');
    optionnalCarsFilter = $.extend(true, [], OptionalCarsArr);
    selectedCarsFilter = $.extend(true, [], FenceCarsArr);

    var selcontent = $('#selContent').combobox("getValue");
    var seltype = $('#selType').combobox("getValue");
    var provalue = $('#txtSearch').val();
    var property = "NAME";

    switch (selcontent) {
        case "1":  //按车牌号过滤
            property = "CarNo";
            break;
        case "2":  //按所属企业过滤
            property = "CarOwnName";
            break;
        case "3": //SIM卡号
            property = "SimCode";
            break;
        case "4": //车台类型
            property = "Ter_name";
            break;
        case "5": //终端号码
            property = "TNO";
            break;
        default:
            property = "CarNo";
            break;
    }

    switch (seltype) {
        case "1":
            //过滤待选列表
            $('#dgSelected').datagrid('loadData', FenceCarsArr);  //已选列表保持不变（未过滤状态）

            optionnalCarsFilter = getfilter(optionnalCarsFilter, property, provalue);

            var opts = $('#dgOptional').datagrid('options');
            opts.pageNumber = 1;
            var p = $('#dgOptional').datagrid('getPager');

            $(p).pagination({
                pageNumber: 1
            });

            $('#dgOptional').datagrid('loadData', optionnalCarsFilter);  //待选列表绑定过滤后的


            break;
        case "2":
            //过滤已选列表

            $('#dgOptional').datagrid('loadData', OptionalCarsArr);  //待选列表保持不变（未过滤状态）

            selectedCarsFilter = getfilter(selectedCarsFilter, property, provalue);

            var opts = $('#dgSelected').datagrid('options');
            opts.pageNumber = 1;
            var p = $('#dgSelected').datagrid('getPager');

            $(p).pagination({
                pageNumber: 1
            });


            $('#dgSelected').datagrid('loadData', selectedCarsFilter);  //已选列表绑定过滤后的
            break;

        default:
            //不做任何操作
            break;
    }

    $('#loading-track').window('close');
}


function DoSave() {
    $('#saving_hold').window('open');
    var selecttnos = new Array();
    if (FenceCarsArr.length > 0) {
        for (var i = 0; i < FenceCarsArr.length; i++) {
            var tno = FenceCarsArr[i].TNO;
            selecttnos.push(tno);
        }
    }
    var tnos = selecttnos.join(",");
    var mydata = {
        "sid": "activeregion-setrelcar",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "uid": UserCookie["UID"],
        "rid": selRegionId,
        "tnos": tnos
    };
    BaseGetData(mydata, resetCarData);

    //$.post("SaveFenceCar.ashx", { key: key, uid: uid, fid: fid, tnos: tnos }, resetCarData);
}

function resetCarData(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            $.messager.alert("操作提示", obj.msg, "info");
            $('#dgOptional').datagrid('loadData', OptionalCarsArr);
            $('#dgSelected').datagrid('loadData', FenceCarsArr);
            $('#txtSearch').attr("value", "");
            $('#selContent').combobox("setValue", "0");
            $('#selType').combobox("setValue", "1");
            $('#saving_hold').window('close');
        }
        else {
            $.messager.alert("系统提示：", obj.msg, "error");
            $('#saving_hold').window('close');
        }
    }
}



/**
* 从对象数组中获取属性为objPropery，值为objValue元素的对象
* @param Array arrCar  数组对象
* @param String objPropery  对象的属性
* @param String objPropery  对象的值
* @return Array 过滤后的数组
*/
function getfilter(arrCar, objPropery, objValue) {
    return $.grep(arrCar, function (cur, i) {
        return (cur[objPropery].toString().indexOf(objValue.toString()) > -1)
    });
}

function pagerFilter(data) {
    //alert(data.length);
    if (typeof data.length == 'number' && typeof data.splice == 'function') {	// is array
        data = {
            total: data.length,
            rows: data
        }
    }
    var dg = $(this);
    var opts = dg.datagrid('options');
    var pager = dg.datagrid('getPager');
    pager.pagination({
        onSelectPage: function (pageNum, pageSize) {
            opts.pageNumber = pageNum;
            opts.pageSize = pageSize;
            pager.pagination('refresh', {
                pageNumber: pageNum,
                pageSize: pageSize
            });
            dg.datagrid('loadData', data);
        }
    });
    if (!data.originalRows) {
        data.originalRows = (data.rows);
    }
    var start = (opts.pageNumber - 1) * parseInt(opts.pageSize);
    var end = start + parseInt(opts.pageSize);
    data.rows = (data.originalRows.slice(start, end));
    return data;
}



