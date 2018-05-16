var UserCookie;

$(function () {
    UserCookie = GetUserInfo();
    MapInit();
    setPage();
});

function setPage() {
    $('#loading-track').window('open');
    InitHiddenUid();
    LoadDatagrid();
}

function InitHiddenUid() {
    var uid = UserCookie["UID"];
    $("#hidUid").attr("value", uid);
}

function LoadDatagrid() {
    var key = UserCookie["sysflag"].toString();
    $("#hidKey").attr("value", key);
    var fname = $("#fencename").val();
    var almtype = $("#alarmType").combobox("getValue");
    var graphtype = $("#graphType").combobox("getValue");

    var mydata = {
        "sid": "fence-getlist",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "fname": fname,
        "almtype": almtype,
        "graphtype": graphtype
    };
    BaseGetData(mydata, setData);
}

function setData(obj) {
    $('#loading-track').window('close');
    if (obj != null) {
        if (obj.state == 100) {
            var FenceArray = obj.result.records;
            $('#dg').datagrid({ idField: "Id", loadFilter: pagerFilter }).datagrid('loadData', FenceArray);
            $('#dg').datagrid('clearSelections');
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('电子围栏信息查询，服务器超时！');
            }
        }
    }
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





function openAdd() {
    mapObj.clearMap();
    $("#hidFid").attr("value", "0");
    $("#hidOpType").attr("value", "New");
    $("#win_almtype").combobox("setValue", "1");
    $("#win_fname").attr("value", "");
    $("#win_desc").attr("value", "");
    $('#details').window({
        title: '添加电子围栏',
        iconCls: 'icon-add'
    });
    $('#details').window('open');
}


function openEdit() {
    mapObj.clearMap();
    $("#hidOpType").attr("value", "Edit");
    //alert($("#hidOpType").val());
    //根据选择的围栏信息，初始化地图中的图形及栅栏其他信息
    var rows = $('#dg').datagrid('getSelections');

    if (rows.length <= 0) {
        //alert("请选择要修改的电子围栏！");
        $.messager.alert("系统提示：", "请选择要修改的电子围栏！！！", "error");
        return;
    }
    else {
        if (rows.length > 1) {
            $.messager.alert("系统提示：", "每次只能修改一个围栏信息！！！", "error");
            return;
        }
    }
    var rowdata = rows[0];

    $("#hidFid").attr("value", rowdata.Id);
    $("#win_almtype").combobox("setValue", rowdata.Alm);
    $("#win_fname").attr("value", rowdata.Name);
    $("#win_desc").attr("value", rowdata.Desc);
    $("#hidContent").attr("value", rowdata.Con);
    if (rowdata.Gragh == 1) { //圆形

        $("#hidGraphT").attr("value", "1");
        addCircle(rowdata.Con); //在地图上画出区域
    }
    else {//多边形
        $("#hidGraphT").attr("value", "2");
        addPolygon(rowdata.Con); //在地图上画出区域
    }
    $('#details').window({
        title: '编辑电子围栏',
        iconCls: 'icon-edit'
    });

    $('#details').window('open');
}

function startQuery() {
    LoadDatagrid();
}

function closeDetails() {
    $('#details').window('close');
}

function saveDetails() {
    var key = $("#hidKey").val();
    var OpType = $("#hidOpType").val();   //操作类型 
    var fid = $("#hidFid").val(); //围栏id
    var fname = $("#win_fname").val(); //围栏名称
    var almtype = $("#win_almtype").combobox("getValue"); //报警类型
    var gratype = $("#hidGraphT").val();   //形状
    var content = $("#hidContent").val();  //区域内容
    var desc = $("#win_desc").val();   //围栏备注（描述）
    var cuser;  //创建者
    var upuser; //更新者
    if (OpType == "New") {
        cuser = $("#hidUid").val();
        upuser = $("#hidUid").val();
    }
    else {
        cuser = "-1";
        upuser = $("#hidUid").val();
    }
    if (gratype != "1" && gratype != "2") {
        $.messager.alert("系统提示：", "请在地图中绘制区域！！！", "error");
    }
    else {
        var mydata = {
            "sid": "fence-addedit",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "OpType": OpType,
            "fid": fid,
            "fname": fname,
            "almtype": almtype,
            "gratype": gratype,
            "content": content,
            "desc": desc,
            "cuser": cuser,
            "upuser": upuser
        };
        BaseGetData(mydata, FinishOperate);
        //$.post("AddEditFence.ashx", { key: key, OpType: OpType, fid: fid, fname: fname, almtype: almtype, gratype: gratype, content: content, desc: desc, cuser: cuser, upuser: upuser }, FinishOperate);
    }
}

function FinishOperate(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            $.messager.alert("操作提示", "操作成功！", "info");
            setTimeout(function () {
                $('#details').window('close');
            }, 1000);
            LoadDatagrid();
        }
        else {
            $.messager.alert("系统提示：", "操作失败！！！", "error");
            setTimeout(function () {
                $('#details').window('close');
            }, 1000);
        }
    }
}

function deleteFence() {
    mapObj.clearMap();
    var key = $("#hidKey").val();
    var ids = "";
    var rows = $('#dg').datagrid('getSelections');

    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "请选择要删除的电子围栏！！！", "error");
        return;
    }
    else {
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].Id + ",";
        }

        ids = ids.substring(0, ids.length - 1);
    }

    $.messager.confirm("操作提示", "确定删除所选围栏吗？", function (data) {
        if (data) {
            var mydata = {
                "sid": "fence-delete",
                "sysuid": UserCookie["UID"],
                "sysflag": UserCookie["sysflag"].toString(),
                "token": UserCookie["token"].toString(),
                "ids": ids 
            };
            BaseGetData(mydata, FinishDelete);
            //$.post("DeleteFence.ashx", { key: key, ids: ids }, FinishDelete);
        }
        else {
            return;
        }

    });
}


function FinishDelete(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            
            $.messager.alert("操作提示", "删除成功！", "info");
            LoadDatagrid();
        }
        else {
            $.messager.alert("系统提示：", "操作失败！！！", "error");
        }
    }
} 