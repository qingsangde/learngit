Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

function addSeconds(Seconds) {
    var d = new Date();
    st = parseInt(Seconds);
    d.setTime(st * 1000);
    return d.Format("yyyy-MM-dd hh:mm:ss");
}

function strToDate(str) {
    str = str.replace(/-/g, "/");
    var date = new Date(str);
    return date;
}

function strAddDay(str, Day) {
    var oDate = strToDate(str);
    var t = oDate.getTime() + 1000 * 60 * 60 * 24 * Day;
    var tt = new Date(t);
    return tt.Format("yyyy-MM-dd hh:mm:ss");
}
function strCutDay(str, Day) {
    var oDate = strToDate(str);
    var t = oDate.getTime() - 1000 * 60 * 60 * 24 * Day;
    var tt = new Date(t);
    return tt.Format("yyyy-MM-dd hh:mm:ss");
}

function strGetNow() {
    var myDate = new Date();
    return myDate.toLocaleDateString() + " " + myDate.toLocaleTimeString();
}

