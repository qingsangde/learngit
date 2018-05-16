var UserCookie;
var operatetype = "New";
var dealer = false;
var RegionArray = new Array();
var EditRID = 0;

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
    var arname = $("#regionname").val();
    var mydata = {
        "sid": "activeregion-getlist",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "regionname": arname
    };
    BaseGetData(mydata, setData);
}

function setData(obj) {
    $('#loading-track').window('close');
    if (obj != null) {
        if (obj.state == 100) {
            RegionArray = obj.result.records;
            $('#dg').datagrid({ idField: "R_Id", loadFilter: pagerFilter }).datagrid('loadData', RegionArray);
            $('#dg').datagrid('clearSelections');
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('活动区域信息查询，服务器超时！');
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

function openAdd() {
    if (RegionArray.length > 0) {
        var len = RegionArray.length;
        for (var i = 0; i < len; i++) {
            if (RegionArray[i].DEALERCODE == dealer.CODE) {
                $.messager.alert("系统提示：", "登录经销商已维护过活动区域，不可再创建！", "error");
                return;
            }
        }
    }
    mapObj.clearMap();
    operatetype = "New";
    if (dealer) {
        var lng = dealer.M_Lng;
        var lat = dealer.M_Lat;
        if (lng != null && lat != null && lng != "" && lat != "") {
            $('#win_center_lng').val(lng);
            $('#win_center_lat').val(lat);
            $('#win_radius').val('5000');
            addCircle(lat + "," + lng + ";" + 5000);
            AddMarker(lng, lat);
            AddInfoWindow(lng, lat, dealer.MarkerName);
        }
    }
    $('#details').window({
        title: '添加活动区域',
        iconCls: 'icon-add'
    });
    $('#details').window('open');
}

function openEdit() {
    //根据选择的围栏信息，初始化地图中的图形及栅栏其他信息
    var rows = $('#dg').datagrid('getSelections');

    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "请选择要修改的活动区域！！！", "error");
        return;
    }
    else {
        if (rows.length > 1) {
            $.messager.alert("系统提示：", "每次只能修改一个区域信息！！！", "error");
            return;
        }
    }



    var rowdata = rows[0];

    if (rowdata.DEALERCODE != dealer.CODE) {
        $.messager.alert("系统提示：", "只能修改自己店的活动区域！", "error");
        return;
    }
    else {
        mapObj.clearMap();
        operatetype = "Edit";
        EditRID = rowdata.R_Id;
        $('#win_rname').val(rowdata.R_NAME);
        $('#win_desc').val(rowdata.R_DESC);
        var lng = rowdata.R_CENTER_LNG;
        var lat = rowdata.R_CENTER_LAT;
        var radius = rowdata.R_RADIUS;
        if (lng != null && lat != null && lng != "" && lat != "") {
            $('#win_center_lng').val(lng);
            $('#win_center_lat').val(lat);
            $('#win_radius').val(radius);
            addCircle(lat + "," + lng + ";" + radius);
            AddMarker(lng, lat);
            AddInfoWindow(lng, lat, dealer.MarkerName);
        }

        $('#details').window({
            title: '编辑活动区域',
            iconCls: 'icon-edit'
        });
        $('#details').window('open');
    }
}

function saveGraph() {
    if (circle) {
        circle.setMap(null);
        circle = null;
    }
    var lng0 = $('#win_center_lng').val();
    var lat0 = $('#win_center_lat').val();
    var radius0 = $('#win_radius').val();
    addCircle(lat0 + "," + lng0 + ";" + radius0);
}

function saveDetails() {
    var uid = UserCookie["UID"];
    if (operatetype == "New") {
        //新建
        var rname = $('#win_rname').val();
        var centerlng = $('#win_center_lng').val();
        var centerlat = $('#win_center_lat').val();
        var rradius = $('#win_radius').val();
        var rdesc = $('#win_desc').val();
        var dealercode = dealer.CODE;


        if (centerlng != dealer.M_Lng || centerlat != dealer.M_Lat) {
            $.messager.alert("系统提示：", "活动区域中心应该是4S店坐标点！", "error");
            setTimeout(function () {
                circle.setCenter(new AMap.LngLat(dealer.M_Lng, dealer.M_Lat));
                $('#win_center_lng').val(dealer.M_Lng);
                $('#win_center_lat').val(dealer.M_Lat)
            }, 1000);

            return;
        }

        var mydata = {
            "sid": "activeregion-add",
            "sysuid": uid,
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "rname": rname,
            "centerlng": centerlng,
            "centerlat": centerlat,
            "rradius": rradius,
            "desc": rdesc,
            "dealercode": dealercode
        };
        BaseGetData(mydata, OperateRegionRes);
    }
    else {
        //修改
        var rname = $('#win_rname').val();
        var centerlng = $('#win_center_lng').val();
        var centerlat = $('#win_center_lat').val();
        var rradius = $('#win_radius').val();
        var rdesc = $('#win_desc').val();

        if (centerlng != dealer.M_Lng || centerlat != dealer.M_Lat) {
            $.messager.alert("系统提示：", "活动区域中心应该是4S店坐标点！", "error");
            setTimeout(function () {
                circle.setCenter(new AMap.LngLat(dealer.M_Lng, dealer.M_Lat));
                $('#win_center_lng').val(dealer.M_Lng);
                $('#win_center_lat').val(dealer.M_Lat)
            }, 1000);
            return;
        }

        var mydata = {
            "sid": "activeregion-edit",
            "sysuid": uid,
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "rname": rname,
            "centerlng": centerlng,
            "centerlat": centerlat,
            "rradius": rradius,
            "desc": rdesc,
            "regionid": EditRID
        };
        BaseGetData(mydata, OperateRegionRes);
    }
}

function OperateRegionRes(obj) {

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

function closeDetails() {
    $('#details').window('close');
}

function deleteRegion() {
    mapObj.clearMap();
    var ids = "";
    var rows = $('#dg').datagrid('getSelections');

    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "请选择要删除的活动区域！！！", "error");
        return;
    }
    else {
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].R_Id + ",";
        }

        ids = ids.substring(0, ids.length - 1);
    }

    $.messager.confirm("操作提示", "确定删除所选活动区域吗？", function (data) {
        if (data) {
            var mydata = {
                "sid": "activeregion-delete",
                "sysuid": UserCookie["UID"],
                "sysflag": UserCookie["sysflag"].toString(),
                "token": UserCookie["token"].toString(),
                "ids": ids
            };
            BaseGetData(mydata, OperateRegionRes);
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
