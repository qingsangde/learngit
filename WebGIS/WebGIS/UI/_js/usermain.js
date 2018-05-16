var userInfo;
var UserCookie;
var UserCarArray = new Array();
var browser = 0;
$(function () {
    var name = GetQueryString("username");
    var pass = GetQueryString("password");
    var key = GetQueryString("key");
    var returnflag = GetQueryString("returnflag");
    if (name == null) {
        alert('账户不能为空!');
    } else if (pass == null) {
        alert('密码不能为空!');
    } else if (key == null) {
        alert('系统标示不能为空!');
    }
    else {
        var sysflag = KeyConvent(key)
        addCookie("sysflag", sysflag );
        var mydata = { "sid": "sys-user-autologin", "account": name, "sysflag": sysflag, "password": pass };
        BaseGetData(mydata, SuccessCallBack);
    }

});
function KeyConvent(key) {
    var sysflag = key;
    switch (key) {
        case "v10":
            sysflag = "JFJY";
            break;
        case "v20":
            sysflag = "HRBKY";
            break;
        case "v30":
            sysflag = "HRBHY";
            break;
        case "v40":
            sysflag = "YQWL";
            break;
    }
    return sysflag;
}

function SuccessCallBack(msg) {
    var arr = msg;
    var ctime = 1;
    if (arr['state'] == 100) {
        addCookie("UserCookie", O2String(msg), ctime);
        UserCookie = GetUserInfo();
        loadpage();
        if (pagflag == 'cm') {
            InitUserCars();
        }

    } else {
        alert(arr['msg']);
    }

}
var pagflag
function loadpage() {
    var returnflag = GetQueryString("returnflag");
    pagflag=returnflag;
    var url = "Query/Query.html"
    switch (returnflag) {
        case "cm":
            url = "Monitor/Monitor.html?returnflag=" + returnflag;
            document.title = "车辆实时监控";
            break;
        case "003":
            url = "Query/Query.html";
            document.title = "统计分析";
            break;
       
        default:
            url = "Query/Query.html?returnflag=" + returnflag;
            document.title = "统计分析";
            break;
    }

    $('#ifr').attr("src", url);
}

//初始化用户权限车辆
function InitUserCars() {
    var mydata = {
        "sid": "sys-user-getallcar",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "onecaruser": UserCookie["OneCarUser"].toString()
    };
    BaseGetData(mydata, BindCarInfo);
}
var isf = true;
function BindCarInfo(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            UserCarArray = obj.result.records;
            if (isf && pagflag=='cm') {
                $.messager.alert('提示', '车辆信息获取成功！总计：' + obj.result.records.length, 'info');
                isf = false;
            }
            //王虎修改 增加定时获取用户车辆数据，用于数据中车辆在线状态的更新 定时5分钟
            setTimeout('InitUserCars', 600000);
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('服务器超时！');
            }
        }
    }
    //loadpage();  //将权限车辆初始化完毕再加载iframe页面
}

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
} 