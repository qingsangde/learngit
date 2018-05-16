var visitedphotocidArr = new Array();
var photocids;
var visitedpositioncidArr = new Array();
var visitedrecordkeyArr = new Array();
var photocount = 0;
var infocount = 0;

var detailrows;
var curcid;
var ImgUrl;
var curitemid;

var oldordercount = 0;
var ordercount = 0;
var callcount = 0;

var CallTrackPointer; //存储定位监控页面中的方法指针
function SetPointer(fn) {
    CallTrackPointer = fn;
}

function SetOrderNumber() {
    oldordercount = 0;
    callcount = 0;
    ordercount++;
    //alert(ordercount);
}

function UpdatePicture() {
    if (photocount <= 0) {
        $("#imgphoto").attr("src", "_styles/images/nophoto.png");
        //$("#imgphoto").attr("onclick", "");
    }
    else {
        $("#imgphoto").attr("src", "_styles/images/photo.png");
        //$("#imgphoto").attr("onclick", "openphotos();");
    }

    if (infocount <= 0) {
        $("#imginfo").attr("src", "_styles/images/noinfo.png");
        //$("#imginfo").attr("onclick", ""); 
    }
    else {
        $("#imginfo").attr("src", "_styles/images/info.png");
        //$("#imginfo").attr("onclick", "openrecords();");
    }
}

function ReceiveOrder() {

    if (ordercount > 0) {
        
        var mydata = {
            "sid": "order-receive-getall",
            "sysuid": UserCookie["UID"].toString(),
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "visitedphotocids": visitedphotocidArr.join(","),
            "visitedpositioncids": visitedpositioncidArr.join(","),
            "visitedrecordkeys": visitedrecordkeyArr.join(",")
        };
        if (ordercount == oldordercount) {
            callcount++;
        }
        else {
            callcount = 0;
        }
        if (callcount < 100) {
            BaseGetData(mydata, InitOrderInfo);
        }
        else { //清空指令计数
            ordercount = 0;
            visitedpositioncidArr = [];
            visitedphotocidArr = [];
            visitedrecordkeyArr = [];
            photocids = "";
        }
    }
    else {
        setTimeout(ReceiveOrder, 1000);
    }
}

function InitOrderInfo(obj) {
        visitedpositioncidArr = [];
        visitedphotocidArr = [];
        visitedrecordkeyArr = [];
    if (obj != null && obj != "") {
        if (obj.state == 100) {
            oldordercount = ordercount;

            //定位
            if (obj.result.PositionResponseList != null && obj.result.PositionResponseList.length > 0) {
                ordercount = ordercount - obj.result.PositionResponseList.length;
                if (ordercount < 0) {
                    ordercount = 0;
                }
                CallTrackPointer();
                var postionarr = obj.result.PositionResponseList;
                var positoncount = postionarr.length;
                for (var i = 0; i < positoncount; i++) {
                    var positonkey = UserCookie["token"].toString() + "|" + postionarr[i].toString() + "|" + UserCookie["sysflag"].toString();
                    visitedpositioncidArr.push(positonkey);
                }
            }
            else {
                visitedpositioncidArr = [];
                var ss = visitedpositioncidArr.join(",");
            }
            //照片
            if (obj.result.ImageResponseList != null && obj.result.ImageResponseList.length > 0) {
                photocount = obj.result.ImageResponseList.length;
                photocids = obj.result.ImageResponseList.join(",");

            }

            //行驶记录仪
            if (obj.result.RecordResponseList != null && obj.result.RecordResponseList.length > 0) {
                infocount = obj.result.RecordResponseList.length;
                var data = obj.result.RecordResponseList;
                $("#dgRecords").datagrid({ rownumbers: true,
                    singleSelect: true,
                    autoRowHeight: false,
                    pagination: false,
                    fit: true,
                    border: true,
                    loadMsg: '',
                    fitColumns: true,
                    columns: [[
		            { field: "carno", title: "车牌号", width: 85, align: "left" },
                    { field: "cmdDesc", title: "指令名称", width: 250, align: "left" }
	                ]]
                });
                $("#dgRecords").datagrid("loadData", data);
                $('#dgRecords').datagrid({ onClickRow: function (rowIndex, rowData) {
                    //增加代码：点击行，弹出窗体，根据不同的命令字，弹出不同的窗体，显示相应的内容
                    OpenSubWindow(rowIndex, rowData);
                }
                });

            }
            else {
                $("#dgRecords").datagrid({ rownumbers: true,
                    singleSelect: true,
                    autoRowHeight: false,
                    pagination: false,
                    fit: true,
                    border: true,
                    loadMsg: '',
                    fitColumns: true,
                    columns: [[
		            { field: "carno", title: "车牌号", width: 85, align: "left" },
                    { field: "cmdDesc", title: "指令名称", width: 250, align: "left" }
	                ]]
                });
                $("#dgRecords").datagrid("loadData", []);

            }
            //透传指令响应结果
            if (obj.result.TransmissionResponseList != null && obj.result.TransmissionResponseList.length > 0) {
                var alertStr = "";
                var len = obj.result.TransmissionResponseList.length;
                for (var i = 0; i < len; i++) {
                    alertStr += "车辆 " + obj.result.TransmissionResponseList[i].carno + " " + obj.result.TransmissionResponseList[i].transmissionDesc + " " + obj.result.TransmissionResponseList[i].transmissionResult + "\r";
                }

                $.messager.alert('提示', alertStr);
            }



        }
        //        else {
        //            if (obj.state == 104) {
        //                LoginTimeout('服务器超时！');
        //            }
        //            else {
        //                //$.messager.alert('提示', obj.msg, 'error');
        //            }
        //        }
    }
    setTimeout(ReceiveOrder, 20000);
}


//查看照片 begin
function openphotos() {
    if (photocount > 0) {
        $('#loading-photo').window('open');
        //照片查看小窗体
        $('#dgdetail').datagrid({
            nowrap: true, rownumbers: false, border: false,
            fit: true, fitColumns: true, pagination: false, singleSelect: true,
            idField: "CID", showHeader: false,
            columns: [[
		{
		    field: "CPath", title: "照片缩略图", width: 85, align: "left",
		    formatter: function (value, row, index) { return SetImgDetail(row); }
		}
	]]
        });

        var sysflag = UserCookie["sysflag"].toString();
        if (sysflag == "JFJY") {
            ImgUrl = "http://202.98.11.79:3006";
        }
        else if (sysflag == "HRBKY") {
            ImgUrl = "http://202.98.11.79:3007";
        }
        else if (sysflag == "HRBHY") {
            ImgUrl = "http://10.44.30.59";
        }
        else if (sysflag == "YQWL") {
            ImgUrl = "http://202.98.11.79:3008";
        }
        var mydata = {
            "sid": "order-receive-getphoto",
            "sysuid": UserCookie["UID"],
            "sysflag": sysflag,
            "token": UserCookie["token"].toString(),
            "cids": photocids
        };
        BaseGetData(mydata, setDetailGrid);
    }
    else {
        $('#loading-photo').window('close');
    }
}

function setDetailGrid(data) {
    $('#loading-photo').window('close');
    $('#photodetails').window('open');
    if (data.state == 100) {
        detailrows = data.result.records;
        if (detailrows.length > 0) {
            photocount = photocount - detailrows.length;
            ordercount = ordercount - detailrows.length;
            $('#curImg').attr("src", GetImgUrl(detailrows[0].CPath) + ".JPEG");
            $('#spanCarNo')[0].innerHTML = detailrows[0].CarNo;
            $('#spanOwnName')[0].innerHTML = detailrows[0].CarOwnName;
            $('#spanTime')[0].innerHTML = detailrows[0].createtime;
            $('#hidCurIndex').attr("value", 0);

            $('#hidTotal').attr("value", detailrows.length);
            $('#dgdetail').datagrid('loadData', detailrows);
            $('#dgdetail').datagrid({
                onClickRow: function (rowIndex, rowData) {
                    $('#hidCurIndex').attr("value", rowIndex);
                    $('#curImg').attr("src", GetImgUrl(rowData.CPath) + ".JPEG");
                    $('#spanCarNo')[0].innerHTML = rowData.CarNo;
                    $('#spanOwnName')[0].innerHTML = rowData.CarOwnName;
                    $('#spanTime')[0].innerHTML = rowData.createtime;
                }
            });
            $('#dgdetail').datagrid('scrollTo', 0);
            $('#dgdetail').datagrid('selectRow', 0);

            var pcount = detailrows.length;
            if (pcount > 0) {
                for (var i = 0; i < pcount; i++) {
                    var photokey = UserCookie["token"].toString() + "|" + detailrows[i]["CID"].toString() + "|" + UserCookie["sysflag"].toString();
                    visitedphotocidArr.push(photokey);
                }
            }
        }
//        if (photocids.split(",").length == detailrows.length) {
//            photocids = "";
//        }
    }
    else {
        if (data.state == 104) {
            LoginTimeout('服务器超时！');
        }
        else {
            $.messager.alert('提示', data.msg, 'error');
        }
    }
}


function GoBack() {
    var curIndex = parseInt($('#hidCurIndex').val());
    if (curIndex <= 0) {
        $.messager.alert("系统提示：", "当前已经是第一张！", "error");
        return;
    }
    else {
        curIndex = curIndex - 1;
        $('#hidCurIndex').attr("value", curIndex);
        $('#curImg').attr("src", GetImgUrl(detailrows[curIndex].CPath) + ".JPEG");
        $('#spanCarNo')[0].innerHTML = detailrows[curIndex].CarNo;
        $('#spanOwnName')[0].innerHTML = detailrows[curIndex].CarOwnName;
        $('#spanTime')[0].innerHTML = detailrows[curIndex].createtime;
        $('#dgdetail').datagrid('scrollTo', curIndex);
        $('#dgdetail').datagrid('selectRow', curIndex);
    }
}

function GoForward() {
    var curIndex = parseInt($('#hidCurIndex').val());
    var total = parseInt($('#hidTotal').val());
    if (curIndex >= total - 1) {
        $.messager.alert("系统提示：", "当前已经是最后一张！", "error");
        return;
    }
    else {
        curIndex = curIndex + 1;
        $('#hidCurIndex').attr("value", curIndex);
        $('#curImg').attr("src", GetImgUrl(detailrows[curIndex].CPath) + ".JPEG");
        $('#spanCarNo')[0].innerHTML = detailrows[curIndex].CarNo;
        $('#spanOwnName')[0].innerHTML = detailrows[curIndex].CarOwnName;
        $('#spanTime')[0].innerHTML = detailrows[curIndex].createtime;
        $('#dgdetail').datagrid('scrollTo', curIndex);
        $('#dgdetail').datagrid('selectRow', curIndex);
    }
}

function SetImgDetail(row) {
    var img = "<input type='image' src='" + GetImgUrl(row.CPath) + ".JPEG" + "' style='width:75px; height:56px; padding:3px;cursor:pointer;' />"
    return img;
}

//将图片查询结果转换成真实的图片地址
function GetImgUrl(cpath) {

    return ImgUrl + cpath.replace(" ", "%20");
}

//查看照片 end

//查看记录仪 begin
function openrecords() {
    if (infocount > 0) {
        $('#wRecords').window('open');
    }
    else {
        $('#wRecords').window('close');
    }
}

function OpenSubWindow(rowIndex, rowData) {
    var thiskey;
    if (rowData != null) {
        //visitedrecordkeyArr

        var cmd = rowData.nCMD;
        thiskey = UserCookie["token"].toString() + "|" + rowData.cid.toString() + "|" + rowData.sysflag.toString() + "|" + rowData.dwPackageType.toString() + "|" + rowData.dwOperation1.toString() + "|" + rowData.nCMD.toString();

        var data = rowData.ResponseData;
        switch (cmd) {
            case 0x00:
                $("#txtCarno00").attr("value", rowData.carno);
                var year = data.Year.toString();
                var reviseno = "00" + data.ReviseNO.toString();
                $("#txtVersion").attr("value", year + reviseno.substr(reviseno.length - 2));
                $("#w00").window('open');
                break;
            case 0x01:
                $("#txtCarno01").attr("value", rowData.carno);
                $("#txtDrivingLicNum").attr("value", data.sDrivingLicNum);
                $("#w01").window('open');
                break;
            case 0x02:
                $("#txtCarno02").attr("value", rowData.carno);
                $("#txtRealtime").attr("value", data.nDateTime);
                $("#w02").window('open');
                break;
            case 0x03:
                $("#txtCarno03").attr("value", rowData.carno);
                $("#txtInitialMiles").attr("value", data.nInitialMilesValue);
                $("#txtSumMiles").attr("value", data.nSumMilesValue);
                $("#txtInstallTime").attr("value", data.nInstallDateTime);
                $("#txtRealtime03").attr("value", data.nRealDateTime);
                $("#w03").window('open');
                break;
            case 0x04:
                $("#txtCarno04").attr("value", rowData.carno);
                $("#txtCharacterQuotient").attr("value", data.nCharacterQuotientValue);
                $("#txtRealtime04").attr("value", data.nRealDateTime);
                $("#w04").window('open');
                break;
            case 0x05:
               // $("#txtCarno05").attr("value", rowData.carno);
                $("#txtCarno05").attr("value", data.sCarPlateNum);
                $("#txtVinNum").attr("value", data.sVinNum);
                $("#txtCarType").attr("value", data.sCarPlateType);
                $("#w05").window('open');
                break;
            case 0x06:
                $("#txtCarno06").attr("value", rowData.carno);
                $("#txtStatus").attr("value", data.sStatus);
                $("#w06").window('open');
                break;
            case 0x07:
                $("#txtCarno07").attr("value", rowData.carno);
                $("#txtUniqueNo").attr("value", data.s3cCode + "-" + data.sProductType + "-" + data.nDatetime.toString().replace(/-/g, "").substr(2, 6) + "-" + data.nSerialNO);
                $("#w07").window('open');
                break;
            //            case 0x08:     
            //                $("#txtCarno08").attr("value", rowData.carno);     
            //                var SpeedStatusItem = new Array();     
            //                if (data.OneMinuteItemList != null && data.OneMinuteItemList.length > 0) {     
            //                    var len8 = data.OneMinuteItemList.length;     
            //                    for (var i = 0; i < len8; i++) {     
            //                        SpeedStatusItem = SpeedStatusItem.concat(data.OneMinuteItemList[i].SpeedStatusItemList);     
            //                    }     
            //                }     
            //                $("#dg08").datagrid("loadData", SpeedStatusItem);     
            //                $("#w08").window('open');     
            default:
                $("#txtCarnoOther").attr("value", rowData.carno);
                $("#txtOther").attr("value", JSON.stringify(data));
                $("#wOther").window('open');
                break;
        }
    }

    $('#dgRecords').datagrid('deleteRow', rowIndex);
    visitedrecordkeyArr.push(thiskey);
    ordercount--;
    infocount--;
    //    var allrows = $('#dgRecords').datagrid('getRows');
    //    if (allrows.length == 0) {
    //        $("#imginfo").attr("src", "_styles/images/noinfo.png");
    //        $("#imginfo").attr("onclick", "");
    //    }
}

//查看记录仪 end

