var UserCookie;
//var operatetype = "New";
var dealer = false;


$(function () {
    UserCookie = GetUserInfo();
    MapInit();
    setPage();
});

function setPage() {
    $('#loading-track').window('open');
    LoadDatagrid();
    InitDealerInfo();
}

function startQuery() {
    LoadDatagrid();
}


function LoadDatagrid() {
    var lname = $("#linename").val();
    var mydata = {
        "sid": "driveline-getlist",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "linenname": lname
    };
    BaseGetData(mydata, setData);
}

function setData(obj) {
    $('#loading-track').window('close');
    if (obj != null) {
        if (obj.state == 100) {
            var LineArray = obj.result.records;
            $('#dg').datagrid({ idField: "L_ID", loadFilter: pagerFilter }).datagrid('loadData', LineArray);
            $('#dg').datagrid('clearSelections');
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('试乘试驾线路信息查询，服务器超时！');
            }
        }
    }
}

function InitDealerInfo() {
    var mydata = {
        "sid": "dealer-getbyuid",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString()
    };
    BaseGetData(mydata, setDealerData);
}

function setDealerData(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            var DealerArray = obj.result.records;
            dealer = DealerArray[0];
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('经销商信息查询，服务器超时！');
            }
        }
    }
}

function closeDetails() {
    $('#details').window('close');
}


function openAdd() {
    mapObj.clearMap();
    circle = false;
    polyline = false;
    //operatetype = "New";
    if (dealer) {
        var lng = dealer.M_Lng;
        var lat = dealer.M_Lat;
        if (lng != null && lat != null && lng != "" && lat != "") {
            $('#win_center_lng').val(lng);
            $('#win_center_lat').val(lat);
            $('#win_radius').val('50');
            addCircle(lat + "," + lng + ";" + 50);
            AddMarker(lng, lat);
            AddInfoWindow(lng, lat, dealer.MarkerName);
        }
    }
    $('#btnSave').show();
    $('#btnCancel').show();
    $('#btnPositionSet').show(); 
    $('#details').window({
        title: '添加试乘试驾线路',
        iconCls: 'icon-add'
    });
    $('#details').window('open');
}

function openView() {


    var rows = $('#dg').datagrid('getSelections');

    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "请选择要查看的线路！！！", "error");
        return;
    }
    else {
        if (rows.length > 1) {
            $.messager.alert("系统提示：", "每次只能查看一条线路信息！！！", "error");
            return;
        }
    }



    var rowdata = rows[0];

    mapObj.clearMap();
    if (dealer) {
        var lng = dealer.M_Lng;
        var lat = dealer.M_Lat;
        if (lng != null && lat != null && lng != "" && lat != "") {
//            $('#win_center_lng').val(lng);
//            $('#win_center_lat').val(lat);
//            $('#win_radius').val('50');
//            addCircle(lat + "," + lng + ";" + 50);
            AddMarker(lng, lat);
            AddInfoWindow(lng, lat, dealer.MarkerName);
        }
    }

    var mydata = {
        "sid": "driveline-getmarker",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "lid": rowdata.L_ID
    };

    BaseGetData(mydata, ViewLineRes);

}

function ViewLineRes(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            var linemakerList = obj.result.records;
            if (linemakerList.length > 0) {
                var linemakerstr = "";
                var first = linemakerList[0];
                $('#win_center_lng').val(first.L_CenterLng);
                $('#win_center_lat').val(first.L_CenterLat);
                $('#win_radius').val(first.L_RADIUS);
                $('#win_lname').val(first.L_Name);
                $('#win_desc').val(first.L_Desc);
                addCircle(first.L_CenterLat + "," + first.L_CenterLng + ";" + first.L_RADIUS);
                for (var i = 0; i < linemakerList.length; i++) {
                    linemakerstr += linemakerList[i].M_Lat + "," + linemakerList[i].M_Lng + ";"
                }
                linemakerstr = linemakerstr.substring(0, linemakerstr.length - 1);
                addPolyline(linemakerstr);
                $('#btnSave').hide();
                $('#btnCancel').hide();
                $('#btnPositionSet').hide(); 
                $('#details').window({
                    title: '查看试乘试驾线路',
                    iconCls: 'icon-yan'
                });
                $('#details').window('open');
            }
            else {
                $.messager.alert("系统提示：", "所选线路有误！！！", "error");
            }
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('线路信息查询，服务器超时！');
            }
        }
    }
}


function saveGraph() {
//    if (circle) {
//        circle.setMap(null);
//        circle = null;
//    }
    var lng0 = $('#win_center_lng').val();
    var lat0 = $('#win_center_lat').val();
    var radius0 = $('#win_radius').val();
    if (radius0 > 80) {
        $.messager.alert("系统提示：", "区域半径不能超过80米！！！");
    }
    else {
        addCircle(lat0 + "," + lng0 + ";" + radius0);
    }
}


function saveDetails() {
    var uid = UserCookie["UID"];
    //新建
    var lname = $('#win_lname').val();
    var centerlng = $('#win_center_lng').val();
    var centerlat = $('#win_center_lat').val();
    var lradius = $('#win_radius').val();
    if (lradius > 80) {
        $.messager.alert("系统提示：", "区域半径不能超过80米！！！");
        return;
    }
    var rdesc = $('#win_desc').val();
    var dealercode = dealer.CODE;


    if (centerlng != dealer.M_Lng || centerlat != dealer.M_Lat) {
        $.messager.confirm("操作提示", "区域中心点不是4S店坐标点,点击<确定>使用新的圆心,点击<取消>重回4S店为圆心！", function (data) {
            if (!data) {
                circle.setCenter(new AMap.LngLat(dealer.M_Lng, dealer.M_Lat));
                $('#win_center_lng').val(dealer.M_Lng);
                $('#win_center_lat').val(dealer.M_Lat);
                centerlng = dealer.M_Lng;
                centerlat = dealer.M_Lat;
            }
        });
    }

    var linemarkers = "";
    if (lineMarkersArray.length > 0) {
        for (var i = 0; i < lineMarkersArray.length; i++) {
            linemarkers += lineMarkersArray[i].getLng() + "," + lineMarkersArray[i].getLat() + ";"
        }
        linemarkers = linemarkers.substring(0, linemarkers.length - 1);
        var mydata = {
            "sid": "driveline-add",
            "sysuid": uid,
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "lname": lname,
            "centerlng": centerlng,
            "centerlat": centerlat,
            "lradius": lradius,
            "desc": rdesc,
            "dealercode": dealercode,
            "linemarkers": linemarkers
        };
        BaseGetData(mydata, OperateLineRes);
    }
    else {
        $.messager.alert("系统提示：", "线路上没有点或者线路不存在,请重画线路！", "error");
    }   
}

function OperateLineRes(obj) {

    if (obj != null) {
        if (obj.state == 100) {
            $.messager.alert("系统提示：", obj.msg, "info");
            setTimeout(function () {
                $('#details').window('close');
            }, 1000);
            startQuery();
        }
        else {
            $.messager.alert("系统提示：", obj.msg, "error");

            setTimeout(function () {
                $('#details').window('close');
            }, 1000);
        }
    }
}


function deleteDriveLine() {
    mapObj.clearMap();
    var ids = "";
    var rows = $('#dg').datagrid('getSelections');

    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "请选择要删除的试乘试驾线路！！！", "error");
        return;
    }
    else {
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].L_ID + ",";
        }

        ids = ids.substring(0, ids.length - 1);
    }

    $.messager.confirm("操作提示", "确定删除所选线路吗？", function (data) {
        if (data) {
            var mydata = {
                "sid": "driveline-delete",
                "sysuid": UserCookie["UID"],
                "sysflag": UserCookie["sysflag"].toString(),
                "token": UserCookie["token"].toString(),
                "ids": ids
            };
            BaseGetData(mydata, OperateLineRes);
        }
        else {
            return;
        }

    });
}




function pagerFilter(data) {
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
