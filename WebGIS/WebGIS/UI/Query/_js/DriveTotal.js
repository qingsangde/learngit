var webBrowser;
var alldata;
var defultSize = 10;
var stagrid;
var marker, mapObj;
var arr = new Array();
var UserCookie;
String.prototype.trim = function () {

    return this.replace(/(^\s*)|(\s*$)/g, '');
}
//定义救援点弹窗对象
var jyinfoWindow = null;
var lnglat;
/*
导出表格文件
UrlData：URL参数
*/
function ExcelExport(UrlData) {
    $('#loading-track').window('open');
    var async = true;
    var demo = O2String(UrlData);
    var rootpath = getBasePath();

    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        //timeout: 120000,
        dataType: 'json',
        error: function (d) {
            $('#loading-track').window('close');
            $.messager.alert("查询失败", "错误代码：" + d.status + "  " + d.statusText);
        },
        data: demo,
        success: function (resdata) {
            $('#loading-track').window('close');
            if (resdata != null && resdata != "") {
                var arr = resdata;
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                }
                else {
                    if (resdata.state == 100) {
                        var asa = rootpath + '\\' + resdata.result;
                        window.open(asa, '_self');
                        //$.messager.alert("操作提示", resdata.result, "ok");
                    }
                    else {
                        $.messager.alert("操作失败", resdata.msg, "ok");
                    }
                }
            }
        }
    });
}
var reqDate = new Date();
var respDate = new Date();
/*
绑定DataGrid 前端分页
UrlData：URL参数
CallBack：回调函数
Async：同步异步
*/
function BindDataGrid_FrontPage(UrlData, CallBack, Async) {
    $('#loading-track').window('open');
    var async = true;
    if (Async == false) {
        async = false;
    }
    var demo = O2String(UrlData);
    var rootpath = getBasePath();
    reqDate = new Date();
    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        //timeout: 1000000,
        dataType: 'json',
        data: demo,
        error: function (d) {
            $('#loading-track').window('close');
            $.messager.alert("查询失败", "错误代码：" + d.status + "  " + d.statusText);
        },
        success: function (resdata) {
            $('#loading-track').window('close');
            if (resdata.state == 104) {
                LoginTimeout('服务器超时！');
            }
            if (resdata.result != null && resdata.result != "") {
                var arr = resdata;

                alldata = arr.result.records;
                //if (resdata.result.total == 0) {
                   // $.messager.alert("提示信息", "未检索到数据！");

               // }
                var pg = $('#data_grid').datagrid("getPager");
                $("#data_grid").datagrid("loadData", alldata.slice(0, 20));
                pg.pagination('refresh', {
                    total: alldata.length,
                    pageNumber: 1,
                    pageSize: 20
                });
                CallBack(async, alldata);
                respDate = new Date();
                var countStr = "";
                if (alldata.length == 1000) {
                    countStr = "只显示前1000条数据，如需查看全部数据，请使用<导出>功能!"
                }
                else {
                    countStr = "";
                }
                //$.messager.alert("查询成功", "总计:" + alldata.length + ",耗时:" + (respDate - reqDate) / 1000 + "s." + countStr);
                $("#TotalTimes").text("试乘试驾总次数：" + alldata.length + "次");
                $("#OutDriveTimes").text("偏离试驾路线总次数：" + alldata.length + "次");
                $("#OutAreaTimes").text("驶出活动范围总次数：" + alldata.length + "次");
                // }
            } else {
                $.messager.alert("提示信息", resdata.msg.toString());
            }
        }
    });
}

/*
返回回调
UrlData：URL参数
CallBack：回调函数
Async：同步异步
*/
function BindData_FrontPage(UrlData, CallBack, Async) {
    $('#loading-track').window('open');
    var async = true;
    if (Async == false) {
        async = false;
    }
    var demo = O2String(UrlData);
    var rootpath = getBasePath();
    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        dataType: 'json',
        data: demo,
        error: function (d) {
            $('#loading-track').window('close');
            $.messager.alert("查询失败", "错误代码：" + d.status + "  " + d.statusText);
        },
        success: function (resdata) {
            if (resdata.result != null && resdata.result != "") {
                var arr = resdata;
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                }
                else {
                    CallBack(arr.result);
                }
            } else {
                $.messager.alert("提示信息", resdata.msg.toString());
            }
        }
    });
}




/*
绑定成功DataGrid回调函数 前端分页
Async：同步异步
*/
function BingDataGrid_LoadSuccessCallBack_FrontPage(Async, urlData) {
    var pg = $('#data_grid').datagrid("getPager");
    if (pg) {
        pg.pagination({
            onSelectPage: function (pageNumber, pageSize) {
                var start = (pageNumber - 1) * pageSize;
                var end = start + pageSize;

                $("#data_grid").datagrid("loadData", alldata.slice(start, end));
                pg.pagination('refresh', {
                    total: alldata.length,
                    pageNumber: pageNumber
                });
            },
            onLoadSuccess: function () {

            }
        });
    }

}
/*
绑定DataGrid
UrlData：URL参数
CallBack：回调函数
Async：同步异步
*/
function BindDataGrid(UrlData, CallBack, Async) {
    var async = true;
    if (Async == false) {
        async = false;
    }
    var demo = O2String(UrlData);
    var rootpath = getBasePath();
    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        dataType: 'json',
        data: demo,
        success: function (resdata) {
            if (resdata != null && resdata != "") {
                var arr = resdata;
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                }
                else {

                    alldata = arr.result.records;
                    if (data.total == 0) {
                        $.messager.alert("提示信息", "未检索到数据！");
                    }
                    $('#data_grid').datagrid({
                        data: data,
                        onLoadSuccess: function (data) {
                            CallBack(async, demo);
                        }
                    });
                }
            } else {
                $.messager.alert("提示信息", resdata.msg.toString());
            }
        }
    });
}
/*
绑定成功DataGrid回调函数
Async：同步异步
*/
function BingDataGrid_LoadSuccessCallBack(Async, urlData) {

    var pg = $('#data_grid').datagrid("getPager");
    if (pg) {
        pg.pagination({
            onSelectPage: function (pageNumber, pageSize) {
                defultSize = pageSize;
                var demo = eval('(' + urlData + ")");
                demo.Page = pageNumber.toString();
                demo.Size = pageSize.toString();
                var rootpath = getBasePath();
                $.ajax({
                    type: 'POST',
                    async: Async, //此标记标示同步
                    url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
                    dataType: 'jsonp',
                    data: O2String(demo),
                    success: function (msg) {
                        if (msg != "null") {
                            var arr = msg;
                            if (msg.state == 104) {
                                LoginTimeout('服务器超时！');
                            }
                            else {

                                var data = arr.result;
                                $('#data_grid').datagrid({
                                    data: data,
                                    pageNumber: pageNumber,
                                    pageSize: pageSize
                                });
                            }
                        }
                    }
                });
            }
        });
    }
}
function BaseGetData(Data, CallBack, Async, re_data) {
    var async = true;
    if (Async == false) {
        async = false;
    }
    var rootpath = getBasePath();
    var Requrl = rootpath + '/Service/HttpService.ashx?random=' + Math.random();
    // alert(1)
    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: Requrl,
        dataType: 'json',
        data: O2String(Data),
        success: function (msg) {
            if (msg.state == 104) {
                LoginTimeout('服务器超时！');
            }
            else {
                CallBack(msg, re_data);
            }
        }
    });
}

function callOperate(msg, re_data) {
    var arr = msg;
    var mes = arr['msg'];
    if (arr['state'] == 100) {
        $.messager.alert("提示信息", mes);
        BindDataGrid(re_data, BingDataGrid_LoadSuccessCallBack);
    } else if (msg.state == 104) {
        LoginTimeout('服务器超时！');
    }
    else {
        $.messager.alert("操作失败", mes);
        var error = arr['result'];
        if (error != "" && error != null) {
            var arr = error.split("*");
            if (arr.length > 1) {
                var str = "已经存在“" + arr[1] + "--" + arr[0] + "”对应数据";
                $.messager.alert("操作失败", str);
            }
            else {
                $.messager.alert("操作失败", error);
            }
        }
    }
}
function getBasePath() {
    var curWwwPath = window.document.location.href;
    var pathName = window.document.location.pathname;
    var pos = curWwwPath.indexOf("UI");
    var localhostPaht = curWwwPath.substring(0, pos - 1);
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    return localhostPaht;
}

//获取Cookie用户信息
function GetUserInfo(point) {
    if (getCookie("UserCookie") != null && getCookie("UserCookie") != "") {
        var json = eval('(' + getCookie("UserCookie").toString() + ')');
        return json["result"];
    } else {
        LoginTimeout('');
    }

}

function CarRealDate(Async, urlData) {
    LoginTimeout('CarRealDate');
}
// 换面超时 跳转登入页面
function LoginTimeout(mes) {
    //$.messager.alert('登录超时', '登录超时，请重新登录!', 'error', Logout);
    $.messager.alert('登录超时', '登录超时，请重新登录!' + mes, 'error', Logout);
    return;
}
//画面用户退出  清空用户信息
function Logout() {
    var rootpath = getBasePath();
    var loginurl = rootpath + "/UI/";
    //var sysflag = UserCookie["sysflag"];
    var sysflag = GetQueryString("key"); //window.top.document.getElementById("sysflag").value;
    if (sysflag == null || sysflag == '') {
        sysflag = GetQueryString("key");
        if (sysflag == null || sysflag == '')
            loginurl += "compag.html"
        loginurl += sysflag + ".html"
    }
    else {
        loginurl += sysflag + ".html"
    }
    delCookie("UserCookie");
    //此次需要修改成跳转到指定登陆页面
    top.location.href = loginurl;
}

function StartHeart() {
    var mydata = {
        "sid": "sys-user-heart",
        "sysflag": UserCookie["sysflag"],
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString()
    };
    var rootpath = getBasePath();
    // alert(1)
    $.ajax({
        type: 'POST',
        async: true, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        dataType: 'json',
        data: O2String(mydata),
        success: function (msg) {
            //var arr =  msg ;

            if (msg.state == 100) {
                setTimeout(StartHeart, 30 * 1000);
            }
            else if (msg.state == 104) {

                LoginTimeout('');
            }
            else {
                setTimeout(StartHeart, 30 * 1000);
            }
        },
        error: function (mes) {
            // $.messager.alert("提示信息","心跳访问错误！"+mes);
            setTimeout(StartHeart, 60 * 1000);
        }
    });

}


function cookie(name) {    //获得cookie 的值

    var cookieArray = document.cookie.split("; "); //得到分割的cookie名值对    

    var cookie = new Object();

    for (var i = 0; i < cookieArray.length; i++) {

        var arr = cookieArray[i].split("=");       //将名和值分开    

        if (arr[0] == name) return unescape(arr[1]); //如果是指定的cookie，则返回它的值 

    }
    return "";
}



function delCookie(name)//删除cookie
{

    document.cookie = name + "=;expires=" + (new Date(0)).toGMTString();

}

function extend(superObj, subObj) {
    //获得父对象的原型对象 
    subObj.getSuper = superObj.prototype;
    //将父对象的属性给子对象 	
    for (var i in superObj) {
        subObj[i] = superObj[i];
    }
    return subObj;
}

function getCookie(objName) {//获取指定名称的cookie的值

    var arrStr = document.cookie.split(";");

    for (var i = 0; i < arrStr.length; i++) {

        var temp = arrStr[i].split("=");

        if (temp[0] == objName) return unescape(temp[1]);

    }

}



function addCookie(objName, objValue, objHours) {      //添加cookie

    var str = objName + "=" + escape(objValue);

    if (objHours > 0) {                               //为时不设定过期时间，浏览器关闭时cookie自动消失

        var date = new Date();

        var ms = objHours * 3600 * 1000;

        date.setTime(date.getTime() + ms);

        str += "; expires=" + date.toGMTString();

    }

    document.cookie = str;

}



function SetCookie(name, value)     //两个参数，一个是cookie的名子，一个是值
{

    var Days = 30;               //此 cookie 将被保存 30 天

    var exp = new Date();       //new Date("December 31, 9998");

    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);

    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();

}

function getCookie(name)//取cookies函数        
{

    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));

    if (arr != null) return unescape(arr[2]); return null;



}

function delCookie(name)//删除cookie
{

    var exp = new Date();

    exp.setTime(exp.getTime() - 1);

    var cval = getCookie(name);

    if (cval != null) document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();

}

var O2String = function (O) {
    //return JSON.stringify(jsonobj);

    var S = [];
    var J = "";
    if (Object.prototype.toString.apply(O) === '[object Array]') {
        for (var i = 0; i < O.length; i++)
            S.push(O2String(O[i]));
        J = '[' + S.join(',') + ']';
    }
    else if (Object.prototype.toString.apply(O) === '[object Date]') {
        J = "new Date(" + O.getTime() + ")";
    }
    else if (Object.prototype.toString.apply(O) === '[object RegExp]' || Object.prototype.toString.apply(O) === '[object Function]') {
        J = O.toString();
    }
    else if (Object.prototype.toString.apply(O) === '[object Object]') {
        for (var i in O) {
            O[i] = typeof (O[i]) == 'string' ? '"' + O[i] + '"' : (typeof (O[i]) === 'object' ? O2String(O[i]) : O[i]);
            S.push(i + ':' + O[i]);
        }
        J = '{' + S.join(',') + '}';
    }

    return J;
};
function arraycontains(array, value) {
    var i = array.length;
    while (i--) {
        if (array[i] === value) {
            return true;
        }
    }
    return false;

}

function getGrowser() {
    var OsObject = "";
    var iever = document.getElementById("iever");
    if (navigator.userAgent.indexOf("MSIE") > 0) {
        if (navigator.userAgent.indexOf("MSIE 6.0") > 0) {
            $.messager.alert("提示信息", "您的浏览器为IE6,建议升级为IE8以上或使用谷歌浏览器。");
        }
        else if (navigator.userAgent.indexOf("MSIE 7.") > 0) {

            return 7;
        }
        else if (navigator.userAgent.indexOf("MSIE 8.") > 0) {

            return 8;
        }
        else if (navigator.userAgent.indexOf("MSIE 9.") > 0) {

            return 9;
        }
        else if (navigator.userAgent.indexOf("MSIE 10.") > 0) {

            return 10;
        }
        else if (navigator.userAgent.indexOf("MSIE 11.") > 0) {

            return 11;
        }

        return "MSIE";
    }
    if (isFirefox = navigator.userAgent.indexOf("Firefox") > 0) {
        if (iever) iever.innerText = "Firefox";
        return "Firefox";
    }
    if (isCamino = navigator.userAgent.indexOf("Chrome") > 0) {
        if (iever) iever.innerText = "Chrome";
        return "Chrome";
    }

    return "other";
}


//function clickEvent(oEvent) {
//    var oEvent = oEvent ? oEvent : window.event;
//    oEvent.preventDefault(); //阻止超链接
//    var tar;
//    if (navigator.appName == "Microsoft Internet Explorer") {
//        tar = oEvent.srcElement;
//    } else {
//        tar = oEvent.target;
//    }
//    if (tar.getAttribute("disabled")) {
//        return false; //阻止点击事件
//    }
//    return true;
//}
function trims(str) {
    str = str.replace(/^(\s|\u00A0)+/, '');
    var strlength = str.length - 1;
    for (var i = strlength; i >= 0; i--) {
        if (/\S/.test(str.charAt(i))) {
            str = str.substring(0, i + 1);
            break;
        }
    }
    return str;
}

//添加遮罩和提示框

function AddRunningDiv() {

    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(document).height() }).appendTo("body");

    $("<div class=\"datagrid-mask-msg\"></div>").html("正在处理，请稍候...").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(document).height() - 45) / 2 });

}

//取消遮罩和提示框

function MoveRunningDiv() {

    $("div[class='datagrid-mask']").remove();

    $("div[class='datagrid-mask-msg']").remove();

}

//JS日期比较判断时间范围是否大于31天
function dateComparison(start, end) {
    var startD = new Date(Date.parse(start.replace(/-/g, "/")));
    var endD = new Date(Date.parse(end.replace(/-/g, "/")));
    var days = parseInt((endD.getTime() - startD.getTime()) / (1000 * 60 * 60 * 24));
    if (days > 30) {
        return false;
    } else {
        return true;
    }
}


/**//* 格式化日期 */
Date.prototype.format = function (style) {

    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),      //day
        "h+": this.getHours(),     //hour
        "m+": this.getMinutes(),   //minute
        "s+": this.getSeconds(),   //second
        "w+": "天一二三四五六".charAt(this.getDay()),   //week
        "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(style)) {
        style = style.replace(RegExp.$1,
    (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(style)) {
            style = style.replace(RegExp.$1,
        RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return style;
};

function findTop() {
    var count = 0;
    var toppage = this;
    while (!toppage.OpenWindow && count < 5) {
        toppage = toppage.parent;
        count++;
    }

    return toppage;
}


//公共车辆选择入口函数
//ck:是否单选。false：多选，true：单选（bool）
//callback:回调函数，接收参数为选择的车辆数据行
//sel：已选车辆CID数组
function CarSelect(ck, callback, sel) {
    var dd1 = new Date().toLocaleString();
    var toppage = findTop();
    if (toppage.OpenWindow)
        toppage.OpenWindow(ck, callback, sel);
}

//发送报警到页面
function SendAlarmInfo(info) {
    var toppage = findTop();
    if (toppage.UpdateAlarmInfo) toppage.UpdateAlarmInfo(info);
}

function SetFunction(callback) {
    var toppage = findTop();
    if (toppage.SetPointer) toppage.SetPointer(callback);
}


//实例化点标记
function addMarker(lng, lat) {
    lnglat = new AMap.LngLat(lng.toString(), lat.toString());
    Geocoder();
    marker = new AMap.Marker({
        icon: "../_styles/images/marker_sprite.png",
        position: lnglat
    });
    marker.setMap(mapObj);  //在地图上添加点
    mapObj.setZoomAndCenter(11, lnglat);
}
//根据经纬度坐标解析地理位置
function Geocoder() {
    var MGeocoder;
    //加载地理编码插件  
    mapObj.plugin(["AMap.Geocoder"], function () {
        MGeocoder = new AMap.Geocoder({
            extensions: "all"
        });
        //返回地理编码结果   
        AMap.event.addListener(MGeocoder, "complete", geocoder_CallBack);
        //逆地理编码  
        MGeocoder.getAddress(lnglat);
    });
}

//回调函数
function geocoder_CallBack(data) {
    jyinfoWindow = new AMap.InfoWindow({
        content: "<h3><font color=\"#00a6ac\">" + data.regeocode.formattedAddress + "</font></h3>",
        //size: new AMap.Size(300, 0),
        autoMove: true,
        offset: new AMap.Pixel(0, -30)
    });

    jyinfoWindow.open(mapObj, lnglat);
}

function ComYue(start, end) {
    var a = start.substring(0, 4);
    var b = start.substring(5, 7);
    if (start.substring(0, 4) == end.substring(0, 4)) {
        if (start.substring(5, 7) == end.substring(5, 7)) {
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}
function ComRi(start, end) {
    var a = start.substring(0, 4);
    var b = start.substring(5, 7);
    var c = start.substring(8, 10);
    if (start.substring(0, 4) == end.substring(0, 4)) {
        if (start.substring(5, 7) == end.substring(5, 7)) {
            if (start.substring(8, 10) == end.substring(8, 10)) {
                return true;
            } else {
                return false;
            }

        } else {
            return false;
        }
    } else {
        return false;
    }
}

function SetOrderCount() {
    top.SetOrderNumber();
}


//JS日期比较判断时间范围是否在规定范围内
function dateCompare(start, end, ms) {//ms:是毫秒数，检查两个时间的差值，是否在指定的毫秒范围内
    var startD = new Date(Date.parse(start.replace(/-/g, "/")));
    var endD = new Date(Date.parse(end.replace(/-/g, "/")));
    var ms0 = parseInt(endD.getTime() - startD.getTime());
    if (ms0 > ms) {
        return false;
    } else {
        return true;
    }
}


function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
function GetTopQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = top.window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}