
var islogin = false;
function login() {
    var at = Date.parse("2015-03-30T00:00:00") / 1000;
    if (islogin) return;
    var myDate3 = new Date();

    var chaoshi = (myDate3 - mytime1) / 1000;
    if (chaoshi > 120) {
        $.messager.alert('��֤��', '��֤�볬ʱ!', 'error', RefreshImage);
        return;
    }

    var name = document.getElementById("name").value;
    var pass = document.getElementById("pass").value;
    var sysflag = document.getElementById("sysflag").value;
    addCookie("sysflag", sysflag, 1000);
    var yz = document.getElementById("yz").value;
    if (name == "") {
        $.messager.alert('�˻�', '�˻�����Ϊ��!', "question");
    } else if (pass == "") {
        $.messager.alert('����', '���벻��Ϊ��!', "question");
    } else if (yz == "") {
        $.messager.alert('��֤��', '��֤�벻��Ϊ��!', "question");
    }
    else {
        var btnlogin = document.getElementById("server");
        btnlogin.value = "��¼�С���";
        btnlogin.disabled = "disabled";
        islogin = true;
        var mydata = { "sid": "sys-user-login", "sysflag": sysflag, "account": name, "password": pass, "code": yz };
        BaseGetData(mydata, SuccessCallBack);
    }
}

function SuccessCallBack(msg) {
    var btnlogin = document.getElementById("server");
    btnlogin.value = "��¼";
    btnlogin.disabled = false;
    islogin = false;
    var arr = msg;
    var ctime = 1000; //�ͻ���Cookie��ʱ�趨Ϊ1000Сʱ
    if (arr['state'] == 100) {
        //var data = arr['result'];

        if (document.getElementById("btn_check").checked) {
            var usern = document.forms[0].name.value;
            var psw = document.forms[0].pass.value;
            SetUser(usern, psw);
        } else {
            delCookie("username");
        }

        addCookie("UserCookie", O2String(msg), ctime);

        var sysflag = document.getElementById("sysflag").value;
        window.location.href = "main.html?key=" + sysflag;


    } else {

        $.messager.alert('������Ϣ', arr['msg'], 'error', RefreshImage);
    }
}

function SetUser(usern, psw) {
    var Then = new Date()
    Then.setTime(Then.getTime() + 1866240000000)

    document.cookie = "username=" + usern + "%%" + psw + ";expires=" + Then.toGMTString();
}

function GetUser() {
    var nmpsd;
    var nm;
    var psd;
    var cookieString = new String(document.cookie);
    var cookieHeader = "username=";
    var beginPosition = cookieString.indexOf(cookieHeader);
    cookieString = cookieString.substring(beginPosition);
    var ends = cookieString.indexOf(";");
    if (ends != -1) {
        cookieString = cookieString.substring(0, ends);
    }
    if (beginPosition > -1) {
        nmpsd = cookieString.substring(cookieHeader.length);
        if (nmpsd != "") {
            beginPosition = nmpsd.indexOf("%%");
            nm = nmpsd.substring(0, beginPosition);
            psd = nmpsd.substring(beginPosition + 2);
            document.forms[0].name.value = nm;
            document.forms[0].pass.value = psd;
            if (nm != "" && psd != "") {
                document.forms[0].btn_check.checked = true;
            }
        }
    }
}
function TestCallBack(msg) {
    var btnlogin = document.getElementById("server");
    btnlogin.value = "��¼";
    btnlogin.disabled = false;
    islogin = false;

}

function TestAMap() {
    BaseGetData(null, TestCallBack);
    jQuery.support.cors = true;

    var rurl = 'http://restapi.amap.com/v3/assistant/coordinate/convert?locations=116.481499,39.990475&coordsys=gps&output=json&key=736ee15f242085b54b92927303b7752b';
    //var rurl = 'http://baidu.com';

    $.ajax({
        type: 'get',
        async: false, //�˱�Ǳ�ʾͬ��
        url: rurl,
        dataType: 'jsonp',
        timeout: 5000,
        success: function (msg) {
            if (msg.info != "ok" && msg.info != "OK") {
                $.messager.alert("��ͼ����", "�޷����ʸߵµ�ͼ������" + msg.info);
            }

        },
        error: function (msg) {
            $.messager.alert("�������", "�޷����ʸߵµ�ͼ�����������������ӡ�");
        }
    });
}