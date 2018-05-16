$(function () {
    //事件初始化
    var nowDate = new Date();
    var preDate = new Date(nowDate.getTime() - 24 * 60 * 60 * 1000);  //前一天
    $('#txtStime').datetimebox('setValue', preDate.format("yyyy-MM-dd hh:mm:ss"));
    $('#txtEtime').datetimebox('setValue', nowDate.format("yyyy-MM-dd hh:mm:ss"));

    $('#dgg').datagrid({ loadFilter: pagerFilter });
   
});

//-------------模糊匹配查询车辆-----begin------
var acarno;
function autoQuery() {
    acarno = $('#selCarNo').combobox('getText');
    if (acarno.length > 2) {
        var UserCookie = GetUserInfo();
        var mydata = { "sid": "track-selectcars", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "carno": acarno };
        BindData_FrontPage(mydata, setAutoQueryData);
    }
}
function setAutoQueryData(data) {
    $('#loading-track').window('close');
    if (data.total > 0) {
        $('#selCarNo').combobox('clear');
        $('#selCarNo').combobox('loadData', data.records);
        $('#selCarNo').combobox('setText', acarno);
    }
    else {
        $('#selCarNo').combobox('loadData',[]);
        $('#selCarNo').combobox('setText', "");
    }
   
}
//-------------模糊匹配查询车辆------end------


var myParam;
function doSelect() {
    //获取输入参数
    myParam = getParams();
    //获取数据
    getData(); 
    $('#dg').datagrid('loadData', []);
    $('#dgg').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', []);
    DrawChart([], "", 100);
}

function getParams() {
    this.cid = $('#selCarNo').combobox('getValue');
    this.cno = $('#selCarNo').combobox('getText');
    this.st = $('#txtStime').datetimebox('getValue');
    this.et = $('#txtEtime').datetimebox('getValue');
    return this;
}

//获取数据
function getData() {
    if (myParam.cid == "") {
        $.messager.alert('系统提示', '请选择车牌号！');
    }
    else if (myParam.st == "") {
        $.messager.alert('系统提示', '请输入开始时间！');
    }
    else if (myParam.et == "") {
        $.messager.alert('系统提示', '请输入结束时间！');
    }
    else {
      
        //发送ajax请求
        var UserCookie = GetUserInfo();
        var mydata = { "sid": "driving-analysis", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),
            "cid": myParam.cid,
            "st": myParam.st,
            "et": myParam.et
        };
        BindData_FrontPage(mydata, setData);
    }
}

function setData(data) {
    $('#loading-track').window('close');
    if (data.total > 0) {
        $('#dg').datagrid('loadData', data.records.Statistics);
        DrawChart(getChartData(data.records.Statistics[0]), myParam.cno, data.total);
        $('#dgg').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', data.records.Details);
    }
    else {
        $.messager.alert('系统提示', '没有查询到相关数据！');
    }
}


function getChartData(obj) {
    var data = [
		{ name: '急加速', value: obj.jjs, color: '#a5c2d5' },
	   	{ name: '大油门', value: obj.dym, color: '#cbab4f' },
	   	{ name: '急刹车', value: obj.jsc, color: '#76a871' },
	   	{ name: '怠速时间过长', value: obj.ds, color: '#76a871' },
	   	{ name: '冷车高速行驶', value: obj.lc, color: '#a56f8f' },
	   	{ name: '水温异常', value: obj.sw, color: '#c12c44' },
//	   	{ name: '电压异常', value: obj.dy, color: '#a56f8f' },
	 ];
	   	return data;
}


function Export() {

    $('#win').window('open');  

//    var data = [
//		{ name: '急加速', value: 7, color: '#a5c2d5' },
//	   	{ name: '大油门', value: 5, color: '#cbab4f' },
//	   	{ name: '急刹车', value: 12, color: '#76a871' },
//	   	{ name: '怠速时间过长', value: 12, color: '#76a871' },
//	   	{ name: '冷车高速行驶', value: 15, color: '#a56f8f' },
//	   	{ name: '水温异常', value: 13, color: '#c12c44' },
//	   	{ name: '电压异常', value: 15, color: '#a56f8f' }
//	 ];

//    DrawChart(data, myParam.cno, 30);


    
}


function DrawChart(data, carno, Total) {
   var iHeight= $("#canvasDiv").height()-2;
   var iWidth=  $("#canvasDiv").width()-2;

    var chart = new iChart.Column2D({
        render: 'canvasDiv', //渲染的Dom目标,canvasDiv为Dom的ID
        data: data, //绑定数据
        title:carno+ '驾驶行为分析', //设置标题
        align: 'center',
        width: iWidth, //设置宽度，默认单位为px
        height: iHeight, //设置高度，默认单位为px
        shadow: false, //激活阴影
        shadow_color: '#c7c7c7', //设置阴影颜色
        coordinate: {//配置自定义坐标轴
            scale: [{//配置自定义值轴
                position: 'left', //配置左值轴	
                start_scale: 0, //设置开始刻度为0
                end_scale: Total, //设置结束刻度为26
                scale_space: Math.round(Total / 5), //设置刻度间距
                listeners: {//配置事件
                    parseText: function (t, x, y) {//设置解析值轴文本
                        return { text: t + " 次" }
                    }
                }
            }]
        }
    });
    chart.draw();
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