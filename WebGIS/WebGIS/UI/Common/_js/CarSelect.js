var carArray = new Array();
var arrlength = 0;
var filterFlag = false;
var single = true;
var filtercarArray = new Array();
var filterlength = 0;
var singscid = null;
$(function () {
    InitDataGrid();

    InitData();
});

function InitDataGrid() {
    $('#dgOptional').datagrid({
        title: '待选列表',
        border: false,
        collapsible: false, //是否可折叠的  
        fit: true, //自动大小  
        pagination: true, //分页控件
        autoRowHeight: false,
        rownumbers: false, //行号 
        singleSelect: single,
        pageSize: 15, //每页显示的记录条数，默认为10  
        pageList: [5, 10, 15], //可以设置每页记录条数的列表  
        onLoadSuccess: function (data) {

            if (data) {

                $.each(data.rows, function (index, item) {

                    if (item.ck == "1") {

                        $('#dgOptional').datagrid('checkRow', index);

                    }

                });

            }

        },
        onCheck: function (rowIndex, rowData) {

            for (var i = 0; i < arrlength; i++) {
                if (carArray[i].CID == rowData.CID) {
                    carArray[i].ck = "1";
                    break;
                }
            }
            if (filterFlag && filterlength > 0) {
                for (var j = 0; j < filterlength; j++) {
                    if (filtercarArray[j].CID == rowData.CID) {
                        filtercarArray[j].ck = "1";
                        break;
                    }
                }
            }
            if (single && singscid != rowData.CID) { //如果是单选，那需要把原来选择的车辆取消
                if (singscid != null) {
                    for (var k = 0; k < arrlength; k++) {
                        if (carArray[k].CID == singscid) {
                            carArray[k].ck = "0";
                            break;
                        }
                    }
                    if (filterFlag && filterlength > 0) {
                        for (var l = 0; l < filterlength; l++) {
                            if (filtercarArray[l].CID == singscid) {
                                filtercarArray[l].ck = "0";
                                break;
                            }
                        }
                    }
                }
                singscid = rowData.CID;
            }
            //carArray[rowIndex].ck = true;
        },
        onUncheck: function (rowIndex, rowData) {
            for (var i = 0; i < arrlength; i++) {
                if (carArray[i].CID == rowData.CID) {
                    carArray[i].ck = "0";
                    break;
                }
            }
            if (filterFlag && filterlength > 0) {
                for (var j = 0; j < filterlength; j++) {
                    if (filtercarArray[j].CID == rowData.CID) {
                        filtercarArray[j].ck = "0";
                        break;
                    }
                }
            }
        },
        onCheckAll: function (rows) {
            var len = rows.length;
            for (var i = 0; i < len; i++) {
                var data = rows[i];
                for (var j = 0; j < arrlength; j++) {
                    if (carArray[j].CID == data.CID) {
                        carArray[j].ck = "1";
                        break;
                    }
                }
                if (filterFlag && filterlength > 0) {
                    for (var k = 0; k < filterlength; k++) {
                        if (filtercarArray[k].CID == data.CID) {
                            filtercarArray[k].ck = "1";
                            break;
                        }
                    }
                }
            }
        },
        onUncheckAll: function (rows) {
            var len = rows.length;
            for (var i = 0; i < len; i++) {
                var data = rows[i];
                for (var j = 0; j < arrlength; j++) {
                    if (carArray[j].CID == data.CID) {
                        carArray[j].ck = "0";
                        break;
                    }
                }
                if (filterFlag && filterlength > 0) {
                    for (var k = 0; k < filterlength; k++) {
                        if (filtercarArray[k].CID == data.CID) {
                            filtercarArray[k].ck = "0";
                            break;
                        }
                    }
                }
            }
        }
    });
    $('#dgOptional').datagrid({ loadFilter: pagerFilter });
    //$('#dgSelected').datagrid({ loadFilter: pagerFilter });

    var p = $('#dgOptional').datagrid('getPager');

    $(p).pagination({
        showRefresh: false,
        showPageList: true,
        beforePageText: '', //页数文本框前显示的汉字  
        afterPageText: '/{pages}',
        displayMsg: '{from}-{to}/共{total}条'
    });
}

function InitData() {
    $('#spanSt')[0].innerHTML = new Date().toLocaleString();
    arrlength = 3000;
    for (var i = 0; i < 3000; i++) { //模拟3000辆车检索
        var sel = new Array();
        sel.push("1");
        sel.push("2");
        var carInfo = new Object();
        if (i % 2 == 0) {
            carInfo.ck = "0";
        }
        else {
            carInfo.ck = "0";
        }
        if (i == 0) {
            if (sel.indexOf("1") > -1) {
                carInfo.ck = "1";
            }
        }
        
        carInfo.CID = i + 1;
        var no = "0000" + (i + 1).toString();
        if (i < 1000) {
            carInfo.carno = "吉A" + no.substr(no.length - 4);
        }
        else if (i < 2000) {
            carInfo.carno = "吉B" + no.substr(no.length - 4);
        }
        else {
            carInfo.carno = "吉C" + no.substr(no.length - 4);
        }
        carInfo.tno = (i + 1).toString();
        carInfo.sim = (i + 1).toString();
        if (i % 3 == 0) {
            carInfo.cu = "客运车辆";
            carInfo.tname = "有为";
            carInfo.cont = "TCP";
            carInfo.cowner = "客运企业" + (i + 1).toString();
        }
        else if (i % 3 == 1) {
            carInfo.cu = "货运车辆";
            carInfo.tname = "国脉";
            carInfo.cont = "UDP";
            carInfo.cowner = "货运企业" + (i + 1).toString();
        }
        else if (i % 3 == 2) {
            carInfo.cu = "测试车辆";
            carInfo.tname = "国脉";
            carInfo.cont = "TCP";
            carInfo.cowner = "启明信息" + (i + 1).toString();
        }
        carArray.push(carInfo);
    }
    var opts = $('#dgOptional').datagrid('options');
    opts.pageNumber = 1;
    var p = $('#dgOptional').datagrid('getPager');

    $(p).pagination({
        pageNumber: 1
    });
    $("#dgOptional").datagrid("loadData", carArray);
    $('#spanDt')[0].innerHTML = new Date().toLocaleString();
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
* 从对象数组中删除属性为objPropery，值为objValue元素的对象
* @param Array arrCar  数组对象
* @param String objPropery  对象的属性
* @param String objPropery  对象的值
* @return Array 过滤后数组
*/
function remove(arrCar, objPropery, objValue) {
    return $.grep(arrCar, function (cur, i) {
        return cur[objPropery] != objValue;
    });
}
/**
* 从对象数组中获取属性为objPropery，值为objValue元素的对象
* @param Array arrCar  数组对象
* @param String objPropery  对象的属性
* @param String objPropery  对象的值
* @return Array 过滤后的数组
*/
function get(arrCar, objPropery, objValue) {
    return $.grep(arrCar, function (cur, i) {
        return (cur[objPropery].toString().indexOf(objValue.toString()) > -1)

        //return cur[objPropery] == objValue;
    });
}
/**
* 显示对象数组信息
* @param String info  提示信息
* @param Array arrCar  对象数组
*/
function showCarInfo(info, arrCar) {
    $.each(arrCar, function (index, callback) {
        info += "Person id:" + arrCar[index].id + " name:" + arrCar[index].name + " sex:" + arrCar[index].sex + " age:" + arrCar[index].age + "/r/t";
    });
    alert(info);
}


function FilterCar() {
    filterFlag = true;
    $('#spanSt')[0].innerHTML = new Date().toLocaleString();
    filtercarArray = carArray;
    filtercarArray = get(filtercarArray, "cowner", "启明");
    filterlength = filtercarArray.length;
    var opts = $('#dgOptional').datagrid('options');
    opts.pageNumber = 1;
    var p = $('#dgOptional').datagrid('getPager');

    $(p).pagination({
        pageNumber: 1
    });
    $("#dgOptional").datagrid("loadData", filtercarArray);
    $('#spanDt')[0].innerHTML = new Date().toLocaleString();
}

function AddArrayIndexOf() { //如果当前浏览器不支持数组的indexOf方法，则手动添加该方法
    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (elt /*, from*/) {
            var len = this.length >>> 0;

            var from = Number(arguments[1]) || 0;
            from = (from < 0)
         ? Math.ceil(from)
         : Math.floor(from);
            if (from < 0)
                from += len;

            for (; from < len; from++) {
                if (from in this &&
          this[from] === elt)
                    return from;
            }
            return -1;
        };
    }
}


