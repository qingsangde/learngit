var UserCookie;
var operatetype = "New";
var dealer = false;
var RegionArray = new Array();
var EditOrgNo = 0;
var OrgLevel = new Array();
var LockLevel = new Array();
var EngineLevel = new Array();
var CustomerLevel = new Array();
var LoanLevel = new Array();
var userRole; //用户角色

$(function () {
    UserCookie = GetUserInfo();
    setPage();
});

function setPage() {
    LoadDatagrid();
}

//点击查询按钮 查询事件
function DoQuery() {
    LoadDatagrid();
}

//查询方法
function LoadDatagrid() {
//取得查询条件
    var orgName = $("#txtOrgName").val(); //放款机构name
    var uid = UserCookie["UID"];
    var mydata = {
        "sid": "loanOrg-list-search",
        "sysuid": uid,
        "token": UserCookie["token"],
        "sysflag": "YQWL",
        "orgName": orgName
    };
    BaseGetData(mydata, setData);
}

//查询后回调
function setData(obj) {
    $('#loading-track').window('close');
    if (obj != null) {
        if (obj.state == 100) {
            RegionArray = obj.result.records;
            $('#data_grid').datagrid({ idField: "CID", loadFilter: pagerFilter }).datagrid('loadData', RegionArray);
            $('#data_grid').datagrid('clearSelections');
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('放款机构查询，服务器超时！');
            }
        }
    }
}

//分页
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

//点击新建按钮
function openAdd() {

    operatetype == "New";
    $('#baseInfoEdit').window('open');
    $('#eOrgName').textbox('setValue', "");
    $('#eRemarks').textbox('setValue', "");
}

//点击取消按钮
function DoCancel() {
    $('#baseInfoEdit').window('close');
}

//点击保存按钮
function DoSave() {
    var uid = UserCookie["UID"];
    //新增保存
    if (operatetype == "New") {


        DoSaveAdd();
    }
    else {
        //修改保存

        DoSaveEdit();
    }
    $('#baseInfoEdit').window('close');
    $('#Div1').window('open');
}


//点击修改按钮事件
function openEdit() {

    //画面制御

    var rows = $('#data_grid').datagrid('getSelections');
    if (rows.length == 1) {
        operatetype = "Edit";
        EditOrgNo = rows[0].OrgNo;
        //弹出修改页面
                var mydata = {
            "sid": "loanOrg-one-search",
            "sysuid": UserCookie["UID"],
            "token": UserCookie["token"],
            "sysflag": UserCookie["sysflag"],
            "orgNo": EditOrgNo
        };

        BaseGetData(mydata, EditInitRes);
    }
    else {
        $.messager.alert('错误信息', '请选择一行！', 'error');
    }
}

//修改页面初始化
function EditInitRes(obj) {

    if (obj.state == 100) {

        $('#baseInfoEdit').window('open');

        var orgMessage = obj.result.records[0];  //放款机构信息

        //页面赋值
        $("#eOrgNo").textbox("setValue", orgMessage.OrgNo);
        $("#eOrgName").textbox("setValue", orgMessage.OrgName);
        $("#eRemarks").textbox("setValue", orgMessage.Remarks);

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

//点击删除按钮事件
function DoDelete() {

    var rows = $('#data_grid').datagrid('getSelections');
    var ids = "";
    if (rows != null && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            ids += "'"+ rows[i].OrgNo+"'" + ",";
        }
        ids = ids.substring(0, ids.length - 1);

        $.messager.confirm("操作提示", "您确定删除所选内容吗？", function (data) {
            if (data) {
                var mydata = {
                    "sid": "loanOrg-delete",
                    "sysuid": UserCookie["UID"],
                    "sysflag": UserCookie["sysflag"],
                    "token": UserCookie["token"],
                    "ids": ids
                };
                BaseGetData(mydata, deleteLoanOrgRes);
            }
            else {
                return;
            }
        });
    }
    else {
        $.messager.alert('系统提示：', '请先选择行！', 'error');

    }
}

//删除后回调
function deleteLoanOrgRes(obj) {
        if (obj != null) {
            if (obj.state == 100) {
                $.messager.alert("系统提示：", obj.msg, "info");
                DoQuery();
            }
            else {
                $.messager.alert("系统提示：", obj.msg, "error");
            }
        }
    }

    //编辑保存
    function DoSaveEdit() {
        var sysflag = "YQWL";//数据库标识
        var sysuid = UserCookie["UID"];
        var orgNo = EditOrgNo; //机构编号
        var orgName = $("#eOrgName").val().trim(); //机构名称
        var remarks = $("#eRemarks").val().trim(); //备注
        var mydata = {
            "sid": "loanOrg-edit-save",
            "sysflag": sysflag,
            "token": UserCookie["token"],
            "userRole": UserCookie.RID,
            "orgNo": orgNo,
            "orgName": orgName,
            "remarks": remarks,
            "sysuid": sysuid
        };

        BaseGetData(mydata, EditSaveRes);
    }

    //新增保存
    function DoSaveAdd() {
        var sysflag = "YQWL"; //数据库标识
        var sysuid = UserCookie["UID"];
        var orgName = $("#eOrgName").val().trim(); //机构名称
        var remarks = $("#eRemarks").val().trim(); //备注

        var mydata = {
            "sid": "loanOrg-add-save",
            "sysflag": sysflag,
            "token": UserCookie["token"],
            "userRole": UserCookie.RID,
            "orgName": orgName,
            "remarks": remarks,
            "sysuid": sysuid
        };

        BaseGetData(mydata, EditSaveRes);
        
    }

    function EditSaveRes(obj) {
        if (obj != null) {
            if (obj.state == 100) {
                //刷新一览
                LoadDatagrid();
                $.messager.alert('提示信息', obj.msg, 'info');
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
