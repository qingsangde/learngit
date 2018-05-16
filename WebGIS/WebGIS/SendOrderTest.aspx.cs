using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommLibrary.Proto;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;
using System.Globalization;
using SysService;
using CommLibrary;
using System.Data;

namespace WebGIS
{
    public partial class SendOrderTest : System.Web.UI.Page
    {
        string sysflag = "HRBKY";
        string token = "1234567890123456789";
        long cid = 0;
        long tno = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void getData()
        {
            cid = long.Parse(this.txtCID.Text);
            tno = long.Parse(txtTNO.Text);
        }
        protected void btnSendPhoto_Click(object sender, EventArgs e)
        {
            getData();
            SendOrderHander.Send_CTS_ImageRequestDown(sysflag, token, cid, tno);
        }

        protected void btnReadTerParam_Click(object sender, EventArgs e)
        {
            getData();
            string df = DropDownList1.SelectedItem.ToString().Replace("0x", "");
            uint cmd = uint.Parse(df);
            SendOrderHander.Send_CTS_DriveRecordDataCollectionRequest(sysflag, token, cid, tno, cmd);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            getData();
            SendOrderHander.Send_CTS_TermSearchRequest(sysflag, token, cid, tno);
        }

        protected void btnOverSpeed_Click(object sender, EventArgs e)
        {
            getData();
            List<CTS_SetTermParamDown.TerminalParamItem> paramlist = new List<CTS_SetTermParamDown.TerminalParamItem>();
            CTS_SetTermParamDown.TerminalParamItem tp = new CTS_SetTermParamDown.TerminalParamItem();
            tp.nParamID = 0x0055;
            tp.nValue = uint.Parse(txtmaxSpeed.Text);
            tp.nParamLen = 4;
            paramlist.Add(tp);
            SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
        }

        protected void btnIPSet_Click(object sender, EventArgs e)
        {

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            OrderResponse or = new OrderResponse();
            ResponseResult result = or.QueryOrderResultOut(null);
            Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            string strRes = JsonConvert.SerializeObject(result, timeConverter);
            TextBox1.Text = strRes;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            string strRes = "";
            TextBox1.Text = "";
            List<OrderResultModel> orms = SendOrderHander.GetOrderResults(token);
            foreach (OrderResultModel orm in orms)
            {
                if (orm == null) return;
                if (orm.dwPackageType == 30)
                {
                    if (orm.dwOperation1 == 10)
                    {
                        MemoryStream ms = new MemoryStream(orm.ResData.byteContext);
                        COM_TimeResponseUp Response = ProtoBuf.Serializer.Deserialize<COM_TimeResponseUp>(ms);
                        ms.Close();
                        strRes += JsonConvert.SerializeObject(Response) + "\n";
                        //TextBox1.Text = strRes;
                    }
                    else if (orm.dwOperation1 == 33)
                    {
                        MemoryStream ms = new MemoryStream(orm.ResData.byteContext);
                        CTS_MediaDataUp mediaResponse = ProtoBuf.Serializer.Deserialize<CTS_MediaDataUp>(ms);
                        if (mediaResponse.nMediaType == 0)
                        {
                            MemoryStream ms3 = new MemoryStream(mediaResponse.sMediaData, 0, mediaResponse.sMediaData.Length);
                            //Image returnImage = Image.FromStream(ms3);
                            System.Drawing.Image i = Bitmap.FromStream(ms3, true);
                            string bpath = HttpRuntime.AppDomainAppPath.ToString();
                            string path = @"TempImage\" + "temp" + mediaResponse.dwTNO + "t" + DateTime.Now.Minute + DateTime.Now.Second + ".png";
                            i.Save(bpath + path);
                            Image1.ImageUrl = path;
                        }
                        ms.Close();
                    }
                    else if (orm.dwOperation1 == 41)
                    {
                        MemoryStream ms = new MemoryStream(orm.ResData.byteContext);
                        CTS_DriveRecordDataUpResponse Response = ProtoBuf.Serializer.Deserialize<CTS_DriveRecordDataUpResponse>(ms);
                        ms.Close();
                        MemoryStream ms2 = new MemoryStream(Response.sDataBlock);
                        object subdata = null;
                        switch (Response.nCMD)
                        {
                            case 0x00://命令字0x00  采集记录仪执行标准版本
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderVersionUp>(ms2);
                                    break;
                                }
                            case 0x01://命令字0x01  采集当前驾驶人信息(机动车驾驶证号码)
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_DriverCodeDrivingLicUp>(ms2);
                                    break;
                                }
                            case 0x02://命令字0x02  采集记录仪的实时时钟
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_CurrentTimeDataUp>(ms2);
                                    break;
                                }
                            case 0x03://命令字0x03  采集累计行程里程
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_CarSumMilesUp>(ms2);
                                    break;
                                }
                            case 0x04://命令字0x04  采集记录仪脉冲系数
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_CarCharacterQuotientUp>(ms2);
                                    break;
                                }
                            case 0x05://命令字0x05  采集车辆信息(车辆识别代号、机动车号牌号码、机动车号牌分类)
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_CarRecorderVinPlateAndTypeUp>(ms2);
                                    break;
                                }

                            case 0x06://命令字0x06  采集记录仪状态信号配置信息
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderStatusConfigUp>(ms2);
                                    break;
                                }
                            case 0x07://命令字0x07  采集记录仪唯一性编号
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderIdentifierUp>(ms2);
                                    break;
                                }
                            case 0x08://命令字0x08  采集指定的行驶速度记录
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderSpeedRecordUp>(ms2);
                                    break;
                                }
                            case 0x09://命令字0x09  采集指定的位置信息记录
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderPositionRecordUp>(ms2);
                                    break;
                                }
                            case 0x10://命令字0x00  采集记录仪执行标准版本
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_AccidentDoubtRecordUp>(ms2);
                                    break;
                                }
                            case 0x11://命令字0x00  采集记录仪执行标准版本
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_OverTimeDrivedRecordUp>(ms2);
                                    break;
                                }
                            case 0x12://命令字0x00  采集记录仪执行标准版本
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_DriverLoginRecordUp>(ms2);
                                    break;
                                }
                            case 0x13://命令字0x00  采集记录仪执行标准版本
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_PowerSupplyRecordUp>(ms2);
                                    break;
                                }
                            case 0x14://命令字0x00  采集记录仪执行标准版本
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_ParamModifyRecordUp>(ms2);
                                    break;
                                }
                            case 0x15://命令字0x00  采集记录仪执行标准版本
                                {
                                    subdata = ProtoBuf.Serializer.Deserialize<CTS_SpeedStatusLogUp>(ms2);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }


                        strRes += JsonConvert.SerializeObject(subdata);


                    }
                }
            }
            TextBox1.Text = strRes;
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            ComSqlHelper csh = new ComSqlHelper();
            DataTable contentData = csh.Query("SELECT  *   FROM  [Qc_CarRunStatus]", "HRBKY").Tables[0]; 
            contentData.Columns.Remove("CID");
            contentData.Columns.Add("StartAddress");
            contentData.Columns.Add("EndAddress");

            List<CommLibrary.AddressConvert.DLngLat> corrds = new List<AddressConvert.DLngLat>();
            for (int i = 0; i < contentData.Rows.Count; i++)
            {
                string sc = contentData.Rows[i]["StartCoord"].ToString();
                sc = string.IsNullOrEmpty(sc) ? "0-0" : sc;
                string slng = sc.Split('-')[1];
                string slat = sc.Split('-')[0];
                string ec = contentData.Rows[i]["EndCoord"].ToString();
                ec = string.IsNullOrEmpty(ec) ? "0-0" : ec;
                string elng = ec.Split('-')[1];
                string elat = ec.Split('-')[0];
                CommLibrary.AddressConvert.DLngLat sdl = new AddressConvert.DLngLat();
                sdl.Lat = double.Parse(slat);
                sdl.Lng = double.Parse(slng);
                CommLibrary.AddressConvert.DLngLat edl = new AddressConvert.DLngLat();
                edl.Lat = double.Parse(elat);
                edl.Lng = double.Parse(elng);
                corrds.Add(sdl);
                corrds.Add(edl);
            }
            string[] address = AddressConvert.AddConvertBatch(corrds);
            for (int i = 0; i < contentData.Rows.Count; i++)
            {
                contentData.Rows[i]["StartAddress"] = address[i * 2];
                contentData.Rows[i]["EndAddress"] = address[i * 2 + 1];
            }


            NPOIHelper npoiHelper = new NPOIHelper();
            string[] headerDataArray = { "车牌号", "所属企业", "车辆用途", "运营线路", "开始时间", "开始速度", "结束时间", "结束速度", "开始经纬度", "结束经纬度", "开始地址", "结束地址" };
            string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
            npoiHelper.WorkbookName = "车辆运行统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            // 设置导入内容
            npoiHelper.HeaderData = headerDataArray;
            npoiHelper.ContentData = contentDataArray;
            string basepath = HttpRuntime.AppDomainAppPath.ToString();
            string filePath = @"UI\Excel\Query\";
            string sd = basepath + filePath;
            npoiHelper.saveExcel(sd);
        }




    }
}