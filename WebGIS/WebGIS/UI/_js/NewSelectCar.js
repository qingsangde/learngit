var carArray = new Array();
var arrlength = 0;
var filterFlag = false;
var single = true;
var filtercarArray = new Array();
var filterlength = 0;
var singscid = null;
var selcids = null;
var callFn;

$(function () {
    InitDataGrid();
});

//window open完成触发事件函数
function showinfo() {
    $('#loading-track').window('open');
    setTimeout(InitWindow, 200);
}

//window close完成触发事件函数
function cleardata() {
    $("#dgOptional").datagrid("loadData", []);
}
//车辆选择完毕确定按钮Click事件函数
function CallOk() {

    if (filterFlag && filterlength > 0) {
        callFn(getfilter(filtercarArray, "ck", "1"));
    }
    else {
        callFn(getfilter(carArray, "ck", "1"));
    }

    $('#wSelCarNew').window('close');
}

//window初始化
function InitWindow() {
    carArray = [];
    arrlength = 0
    filterFlag = false;
    filtercarArray = [];
    filterlength = 0
    $('#dgOptional').datagrid({ singleSelect: single });
    //InitDataGrid();
    //AddArrayIndexOf();  //如果浏览器不支持数组的indexOf方法，则手动添加该方法
    InitData();
    var dd3 = new Date().toLocaleString();
    //    setInterval(function () {
    //        UpdateCount();
    //    }, 500);
}

//window open为了提高open速度，此处仅初始化部分js参数
function OpenWindow(ck, callback, sel) {
    var dd2 = new Date().toLocaleString();
    $('#wSelCarNew').window('open');
    single = ck;
    selcids = sel;
    callFn = callback;
    $('#lblCarCount')[0].innerHTML = "0";
}
//已选车辆数定时刷新（因为涉及到单选、多选、全选，没法实时刷新）
function UpdateCount() {
    var selcount = 0;
    if (filterFlag && filterlength > 0) {
        selcount = getfilter(filtercarArray, "ck", "1").length;
    }
    else {
        selcount = getfilter(carArray, "ck", "1").length;
    }


    $('#lblCarCount')[0].innerHTML = selcount.toString();
}

//DataGrid属性事件初始化
function InitDataGrid() {
    $('#dgOptional').datagrid({
        toolbar: '#dgtoolbar',
        border: false,
        collapsible: false, //是否可折叠的  
        fit: true, //自动大小  
        pagination: true, //分页控件
        autoRowHeight: false,
        rownumbers: true, //行号 
        //singleSelect: single,
        pageSize: 10, //每页显示的记录条数，默认为10  
        pageList: [5, 10, 15] //可以设置每页记录条数的列表
    });

    $('#dgOptional').datagrid({ loadFilter: pagerFilter });

    $('#dgOptional').datagrid({
        onLoadSuccess: function (data) {//初始化checkbox列
            if (data) {
                $.each(data.rows, function (index, item) {
                    if (item.ck == "1") {
                        $('#dgOptional').datagrid('checkRow', index);
                    }
                });
                UpdateCount();
            }
        },
        onCheck: function (rowIndex, rowData) {

            for (var i = 0; i < arrlength; i++) {
                if (carArray[i].cid == rowData.cid) {
                    carArray[i].ck = "1";


                    break;
                }
            }
            if (filterFlag && filterlength > 0) {
                for (var j = 0; j < filterlength; j++) {
                    if (filtercarArray[j].cid == rowData.cid) {
                        filtercarArray[j].ck = "1";
                        break;
                    }
                }
            }
            if (single && singscid != rowData.cid) { //如果是单选，那需要把原来选择的车辆取消
                if (singscid != null) {
                    for (var k = 0; k < arrlength; k++) {
                        if (carArray[k].cid == singscid) {
                            carArray[k].ck = "0";
                            break;
                        }
                    }
                    if (filterFlag && filterlength > 0) {
                        for (var l = 0; l < filterlength; l++) {
                            if (filtercarArray[l].cid == singscid) {
                                filtercarArray[l].ck = "0";
                                break;
                            }
                        }
                    }
                }
                singscid = rowData.cid;
            }
            UpdateCount();
        },
        onUncheck: function (rowIndex, rowData) {
            for (var i = 0; i < arrlength; i++) {
                if (carArray[i].cid == rowData.cid) {
                    carArray[i].ck = "0";

                    break;
                }
            }
            if (filterFlag && filterlength > 0) {
                for (var j = 0; j < filterlength; j++) {
                    if (filtercarArray[j].cid == rowData.cid) {
                        filtercarArray[j].ck = "0";
                        break;
                    }
                }
            }
            UpdateCount();
        },
        onCheckAll: function (rows) {
            var len = rows.length;
            for (var i = 0; i < len; i++) {
                var data = rows[i];
                for (var j = 0; j < arrlength; j++) {
                    if (carArray[j].cid == data.cid) {
                        carArray[j].ck = "1";

                        break;
                    }
                }
                if (filterFlag && filterlength > 0) {
                    for (var k = 0; k < filterlength; k++) {
                        if (filtercarArray[k].cid == data.cid) {
                            filtercarArray[k].ck = "1";
                            break;
                        }
                    }
                }
            }
            UpdateCount();
        },
        onUncheckAll: function (rows) {
            var len = rows.length;
            for (var i = 0; i < len; i++) {
                var data = rows[i];
                for (var j = 0; j < arrlength; j++) {
                    if (carArray[j].cid == data.cid) {
                        carArray[j].ck = "0";
                        break;
                    }
                }
                if (filterFlag && filterlength > 0) {
                    for (var k = 0; k < filterlength; k++) {
                        if (filtercarArray[k].cid == data.cid) {
                            filtercarArray[k].ck = "0";
                            break;
                        }
                    }
                }
            }
            UpdateCount();
        }
    });



}
function selContentChange(a, aq) {
    var seltype = $('#selContent').combobox("getValue");
    if (seltype == 7) {
        $('#txtSearch').hide();
        $('#comSearch').next(".combo").show();
        $('#comSearch').combobox("setValue", "上线");
    }
    else {
        $('#comSearch').next(".combo").hide();
        $('#txtSearch').show();
    }
}
//数据初始化
function InitData() {

    if (single) {

        $('#allseldiv').hide();
    }
    else {
        $('#allseldiv').show();
    }
    $('#selContent').combobox("setValue", "1");
    $('#selContent').combobox({ onChange: function (w, aq) { selContentChange(w, aq); } });
    $('#txtSearch').val("");
    $('#comSearch').next(".combo").hide();
    $('#txtSearch').show();

    if (UserCarArray != null && UserCarArray.length > 0) {
        arrlength = UserCarArray.length;
        //carArray = UserCarArray;
        carArray = $.extend(true, [], UserCarArray);
        if (selcids != null && selcids.length > 0) {
            for (var i = 0; i < arrlength; i++) {
                if ((selcids.join(',') + ",").indexOf(UserCarArray[i].cid.toString() + ",") > -1) {
                    carArray[i].ck = "1";
                }
                else {
                    carArray[i].ck = "0";
                }
            }
        }
    }
    var opts = $('#dgOptional').datagrid('options');
    opts.pageNumber = 1;
    var p = $('#dgOptional').datagrid('getPager');

    $(p).pagination({
        pageNumber: 1
    });
    $("#dgOptional").datagrid("loadData", carArray);
    $('#loading-track').window('close');
    //$('#spanDt')[0].innerHTML = new Date().toLocaleString();
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

//车辆数据筛选
function FilterCar() {
    for (var i = 0; i < arrlength; i++) {
        carArray[i].ck = "0";
    }
    if (filterFlag && filterlength > 0) {
        $.each(filtercarArray, function (i, obj) {
            obj.ck = "0";
        });
    }
    //$('#spanSt')[0].innerHTML = new Date().toLocaleString();
    var seltype = $('#selContent').combobox("getValue");
    var provalue = $('#txtSearch').val();
    var property = "";
    if (seltype == "1") {
        property = "carno";  //车牌号为过滤条件
    }
    if (seltype == "2") {
        property = "cowner"; //所属企业为过滤条件
    }
    if (seltype == "3") {
        property = "sim";   //SIM卡号为过滤条件
    }
    if (seltype == "4") {
        property = "tname";  //车台类型为过滤条件
    }
    if (seltype == "5") {
        property = "tno";   //终端号为过滤条件
    }
    if (seltype == "6") {
        property = "cu";   //车辆用途为过滤条件
    }
    if (seltype == "7") {
        property = "os";   //车辆在线状态为过滤条件
        provalue = $('#comSearch').combobox("getValue");
    }
    if (seltype == "8") {
        property = "dph";
    }

    filterFlag = true;
    filtercarArray = $.extend(true, [], carArray);
    if (seltype != null && seltype != "0") {
        filtercarArray = getfilter(filtercarArray, property, provalue);
    }
    filterlength = filtercarArray.length;
    var opts = $('#dgOptional').datagrid('options');
    opts.pageNumber = 1;
    var p = $('#dgOptional').datagrid('getPager');

    $(p).pagination({
        pageNumber: 1
    });
    $("#dgOptional").datagrid("loadData", filtercarArray);
    //$('#spanDt')[0].innerHTML = new Date().toLocaleString();
}

//取消选择
function redo() {
    for (var i = 0; i < arrlength; i++) {
        carArray[i].ck = "0";
    }

    if (filterFlag && filterlength > 0) {
        for (var j = 0; j < filterlength; j++) {
            filtercarArray[j].ck = "0";
        }
        $("#dgOptional").datagrid("loadData", filtercarArray);
    }
    else {
        $("#dgOptional").datagrid("loadData", carArray);
    }
    UpdateCount();
}

function alldo() {
    if (filterFlag && filterlength > 0) {
        $.each(filtercarArray, function (i, obj) {
            var thecid = obj.cid;
            obj.ck = "1";
            //            $.each(carArray, function (j,obj0) {
            //                if (obj0.cid == thecid) {
            //                    obj0.ck = "1";
            //                }
            //                else {
            //                    obj0.ck = "0";
            //                }
            //            });
        });

        //        for (var j = 0; j < filterlength; j++) {
        //            filtercarArray[j].ck = "1";
        ////            for (var k = 0; k < arrlength; k++) {
        ////                if (carArray[k].cid == filtercarArray[j].cid) {
        ////                    carArray[k].ck = "1";
        ////                    break;
        ////                }
        ////            }
        //        }
        $("#dgOptional").datagrid("loadData", filtercarArray);
    }
    else {
        for (var i = 0; i < arrlength; i++) {
            carArray[i].ck = "1";
        }
        $("#dgOptional").datagrid("loadData", carArray);
    }
    UpdateCount();
}