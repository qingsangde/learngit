var UserCookie;
var addMarkerAction = false;   //点击添加标记为true
var modMarker = false;   //点击修改标记为true
var delMarker = false;   //点击删除标记为true
$(function () {
    UserCookie = GetUserInfo();
    setPage();
    MapInit();  
});

function setPage() {
    //$('#loading-marker').window('open');      //页面加载模块有问题
    LoadDatagrid();
}

function LoadDatagrid() {
    var SMarkName = $("#SMarkName").val();
    var mydata = {
        "sid": "marker-getlist",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "SMarkName": SMarkName
    };
    BaseGetData(mydata, setData);
}

function startQuery() {
    LoadDatagrid();
}

function saveAddAction(x, y) {
    addMarkerAction = true; 
    var mydata = {
        "sid": "markersetting-addmarker",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "RowId": "0",
        "DealerCode": "",
        "OpType": "New",        //操作类型 
        "M_Name": $("#MarkName").val(),
        "M_Lat": y,     //经度
        "M_Lng": x,     //纬度
        "M_Desc": $("#MarkNote").val(),
        "cuser": UserCookie["UID"],
        "upuser": UserCookie["UID"]
    };
    BaseGetData(mydata, FinishOperate);
}


function modMark() {            //修改标注
    modMarker = true;   
    var rows = $('#data_grid').datagrid('getSelections');
    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "请选择要修改的地图标注！", "error");
        return;
    }
    else {
        if (rows.length > 1) {
            $.messager.alert("系统提示：", "每次只能修改一个标注信息！", "error");
            return;
        }
    }
    var rowdata = rows[0];
    $("#oldMarkName").attr("value", rowdata.M_Name);
    $("#oldMarkNote").attr("value", rowdata.M_Desc);
    $("#newMarkName").attr("value", "");
    $("#newMarkNote").attr("value", "");
    $('#ModMarkerWindows').window('open');
}

function modSave() {
    var rows = $('#data_grid').datagrid('getSelections');
    var rowdata = rows[0];
    var M_Name = $("#newMarkName").val();
    if (M_Name == null || M_Name == "") {
        $.messager.alert("系统提示：", "请输入标注名称！！", "error");
        return;
    } else {
        //保存操作
        var mydata = {
            "sid": "markersetting-addmarker",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "RowId": rowdata.ID,
            "DealerCode": rowdata.DEALERCODE,
            "OpType": "Edit",  //操作类型
            "M_Name": M_Name,
            "M_Lat": rowdata.M_Lat,
            "M_Lng": rowdata.M_Lng,
            "M_Desc": $("#newMarkNote").val(),
            "cuser": UserCookie["UID"],
            "upuser": UserCookie["UID"]
        };
        BaseGetData(mydata, FinishOperate);

    }

}


function delMark() {        //删除标注，非批量操作，一个经销商只有一个标注
    delMarker = true;
    var rows = $('#data_grid').datagrid('getSelections');

    if (rows.length <= 0) {
        $.messager.alert("系统提示：", "请选择要删除的地图标注！", "error");
        return;
    }
    else {
        if (rows.length > 1) {
            $.messager.alert("系统提示：", "每次只能删除一个标注信息！", "error");
            return;
        }
    }
    
//        for (var i = 0; i < rows.length; i++) {
//            ids += rows[i].ID + ",";
//            DealerCodes += rows[i].DEALERCODE + ",";
//        }
//        ids = ids.substring(0, ids.length - 1);
//        DealerCodes = DealerCodes.substring(0, ids.length - 1);
    

    $.messager.confirm("操作提示", "确定删除所选标注吗？", function (data) {
        if (data) {
            var mydata = {
                "sid": "marker-delete",
                "sysuid": UserCookie["UID"],
                "sysflag": UserCookie["sysflag"].toString(),
                "token": UserCookie["token"].toString(),
                "ids": rows[0].ID,
                "DealerCodes": rows[0].DEALERCODE 
            };
            BaseGetData(mydata, FinishDelete);
        }
        else {
            return;
        }

    });
} 
   
    
    function FinishOperate(obj) {
        if (obj != null) {
            if (obj.result == 1) {      //操作成功 
                $.messager.alert("操作提示", obj.msg, "info");
                if (addMarkerAction) {
                    $('#AddMarkerWindows').window('close');   
                }
                if (modMarker) {
                    $('#ModMarkerWindows').window('close');    
                }
                LoadDatagrid();
            }
            
            else {
                $.messager.alert("操作提示：", obj.msg, "error");
                if (addMarkerAction) {
                    $('#AddMarkerWindows').window('close');
                }
                if (modMarker) {
                    $('#ModMarkerWindows').window('close');
                }
            }
        }
    }

    function setData(obj) {
        if (obj != null) {
            if (obj.state == 100) {
                var MarkerArray = obj.result.records;
                //$('#data_grid').datagrid({ idField: "Id", loadFilter: pagerFilter }).datagrid('loadData', MarkerArray);
                $('#data_grid').datagrid({ idField: "Id" }).datagrid('loadData', MarkerArray);
                var rows = $('#data_grid').datagrid('getSelections');
                if (modMarker) {
                    drawMarker(MarkerArray, rows[0].M_Lat, rows[0].M_Lng);
                    $('#data_grid').datagrid('clearSelections');
                }
                else if (addMarkerAction)
                    drawMarker(MarkerArray, MarkerArray[MarkerArray.length - 1].M_Lat, MarkerArray[MarkerArray.length - 1].M_Lng);
                else
                    drawMarker(MarkerArray, "", "");
            }
            else {
                if (obj.state == 104) {
                    LoginTimeout('地图标注信息查询，服务器超时！');
                }
            }
        }

        addMarkerAction = false;
        modMarker = false;
        delMarker = false;
    }

    function FinishDelete(obj) {
        if (obj != null) {
            if (obj.state == 100) {
                $.messager.alert("操作提示", "删除成功！", "info");
                LoadDatagrid();
            }
            else {
                if (obj.state == 104) {
                    LoginTimeout('服务器超时！');
                }
                else {
                    $.messager.alert('错误信息', obj.msg, 'error');
                }
            }
        }
    } 

