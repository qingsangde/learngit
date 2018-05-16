using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebGIS;
using CommLibrary;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Text;
using Aspose.Cells;
using System.IO;
using Newtonsoft.Json;
using System.Web.UI;


namespace SysService
{
    public class sys_car
    {
        
        /// <summary>
        /// 检索用户权限车辆，供选择(公共车辆选择模块使用),取出数据放在Main页面中的js全局变量
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult GetOptionalCarsNew(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                string sysflag = inparams["sysflag"];
                string sysuid = inparams["sysuid"];
                string onecaruser = inparams["onecaruser"];
                SqlParameter[] Parameters0 = new SqlParameter[2];
                Parameters0[0] = new SqlParameter("@UID", Int32.Parse(sysuid));
                Parameters0[1] = new SqlParameter("@OneCarUser", Int32.Parse(onecaruser));
                DataTable dt = csh.FillDataSet(sysflag, WebProc.Proc("GetCarInfo"), Parameters0).Tables[0];
                //查询车辆的在线状态填充到数据表中
                List<long> cids = new List<long>();
                foreach (DataRow dr in dt.Rows)
                {
                    cids.Add(long.Parse(dr["cid"].ToString()));
                }

                //monitor mo = new monitor();
                //WebGIS.RealtimeDataServer.CarRealData[] realdata = mo.CarRealDataByCids(sysflag, cids.ToArray());
              
                
                DataColumn dc = new DataColumn();
                dc.ColumnName = "os";
                dc.DefaultValue = "停止";
                dt.Columns.Add(dc);

                //Dictionary<long, string> carOnlineStatus = new Dictionary<long, string>();
                //foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
                //{
                //    carOnlineStatus.Add(w.Carid, w.OnlineStatusStr);
                //}



                //====修改wx2016-3-31======
                List<CarsStatus> realdata = RDSConfig.GetCarOnlineStatus(sysflag, cids.ToArray());
                Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                foreach (CarsStatus carStatus in realdata)
                {
                    carOnlineStatus.Add(carStatus.cid, carStatus.status);
                }
                //=========================



                foreach (DataRow dr in dt.Rows)
                {
                    long cid=long.Parse(dr["cid"].ToString());
                    if(carOnlineStatus.ContainsKey(cid))
                        dr["os"] = carOnlineStatus[cid]==1?"上线":"停止";
                }
                //完成在线状态数据添加

                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }
        
       

        public ResponseResult getXiaoDaiCarSeach(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                string sysflag = inparams["sysflag"];
                string uid = inparams["sysuid"];
                string carno = inparams["carno"];
                string vin = inparams["vin"];
                string active = inparams["active"];
                string lockstatus = inparams["lockstatus"];
                string timelimit = inparams["timelimit"];

                //调用存储过程查询车辆数据

                SqlParameter[] Parameters0 = new SqlParameter[6];
                Parameters0[0] = new SqlParameter("@Uid", Int32.Parse(uid));
                Parameters0[1] = new SqlParameter("@CarNo", carno);
                Parameters0[2] = new SqlParameter("@DPH", vin);
                Parameters0[3] = new SqlParameter("@Activ", Int32.Parse(active));
                Parameters0[4] = new SqlParameter("@Lockstatus", Int32.Parse(lockstatus));
                Parameters0[5] = new SqlParameter("@TimeLimit", Int32.Parse(timelimit));
                DataTable dt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_JFQZ_QueryCarInfo"), Parameters0).Tables[0];
                
                //查询车辆的在线状态填充到数据表中
                List<long> cids = new List<long>();
                foreach (DataRow dr in dt.Rows)
                {
                    cids.Add(long.Parse(dr["CID"].ToString()));
                }
                monitor mo = new monitor();
                WebGIS.RealtimeDataServer.CarRealData[] realdata = mo.CarRealDataByCids(sysflag, cids.ToArray());


                DataColumn dc = new DataColumn();
                dc.ColumnName = "os";
                dc.DefaultValue = "停止";
                dt.Columns.Add(dc);

                DataColumn dc1 = new DataColumn();
                dc1.ColumnName = "alarmstr";
                dc1.DefaultValue = "正常";
                dt.Columns.Add(dc1);

                Dictionary<long, string> carOnlineStatus = new Dictionary<long, string>();
                Dictionary<long, string> carAlarmStr = new Dictionary<long, string>();
                foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
                {
                    carOnlineStatus.Add(w.Carid, w.OnlineStatusStr);
                    carAlarmStr.Add(w.Carid, w.AlarmStr);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    long cid = long.Parse(dr["CID"].ToString());
                    if (carOnlineStatus.ContainsKey(cid))
                    {
                        dr["os"] = carOnlineStatus[cid];
                        dr["alarmstr"] = carAlarmStr[cid];
                    }
                }


                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                res.isallresults = 1;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }


        public ResponseResult getCarDetailSearch(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                string sysflag = inparams["sysflag"];
                string uid = inparams["sysuid"];
                string cid = inparams["cid"];

                //调用存储过程查询车辆数据

                SqlParameter[] Parameters0 = new SqlParameter[1];
                Parameters0[0] = new SqlParameter("@cid", long.Parse(cid));

                DataTable dt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_JFQZ_GetCarInfoByCid"), Parameters0).Tables[0];

                //查询车辆的在线状态填充到数据表中
                List<long> cids = new List<long>();
                foreach (DataRow dr in dt.Rows)
                {
                    cids.Add(long.Parse(dr["CID"].ToString()));
                }
                monitor mo = new monitor();
                WebGIS.RealtimeDataServer.CarRealData[] realdata = mo.CarRealDataByCids(sysflag, cids.ToArray());


                DataColumn dc = new DataColumn();
                dc.ColumnName = "os";
                dc.DefaultValue = "停止";
                dt.Columns.Add(dc);

                Dictionary<long, string> carOnlineStatus = new Dictionary<long, string>();
                foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
                {
                    carOnlineStatus.Add(w.Carid, w.OnlineStatusStr);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    long thecid = long.Parse(dr["CID"].ToString());
                    if (carOnlineStatus.ContainsKey(thecid))
                        dr["os"] = carOnlineStatus[thecid];
                }


                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                res.isallresults = 1;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }
        ///// <summary>
        ///// 字符串数组转换整形数组
        ///// </summary>
        ///// <param name="Content">字符串数组</param>
        ///// <returns></returns>
        //public static long[] ToIntArray(string[] Content)
        //{
        //    long[] c = new long[Content.Length];
        //    for (int i = 0; i < Content.Length; i++)
        //    {
        //        c[i] = Convert.ToInt64(Content[i].ToString());
        //    }
        //    return c;
        //}

        //public ResponseResult getCarLastTrack(Dictionary<string, string> inparams)
        //{
        //    ResponseResult Result = null;

        //    if (!inparams.Keys.Contains("CIDs"))
        //    {
        //        Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
        //        return Result;
        //    }
        //    string sysflag = inparams["sysflag"];
        //    ResList list_res = new ResList();
        //    try
        //    {
        //        string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
        //        //调用存储过程或WebGIS实时数据服务接口查询车辆轨迹数据
        //        DataTable dt = new DataTable();

        //        //初始化DataTable的列
        //        dt.Columns.Add(new DataColumn("Alarm", typeof(long)));
        //        dt.Columns.Add(new DataColumn("AlarmStr", typeof(string)));
        //        dt.Columns.Add(new DataColumn("AltitudeMeters", typeof(int)));
        //        dt.Columns.Add(new DataColumn("CarNum", typeof(string)));
        //        dt.Columns.Add(new DataColumn("Carid", typeof(long)));
        //        dt.Columns.Add(new DataColumn("Heading", typeof(int)));
        //        dt.Columns.Add(new DataColumn("HeadingStr", typeof(string)));
        //        dt.Columns.Add(new DataColumn("Lati", typeof(float)));
        //        dt.Columns.Add(new DataColumn("Long", typeof(float)));
        //        dt.Columns.Add(new DataColumn("Speed", typeof(float)));
        //        dt.Columns.Add(new DataColumn("Status", typeof(int)));
        //        dt.Columns.Add(new DataColumn("StatusStr", typeof(string)));
        //        dt.Columns.Add(new DataColumn("SumMiles", typeof(int)));
        //        dt.Columns.Add(new DataColumn("TDateTime", typeof(DateTime)));
        //        dt.Columns.Add(new DataColumn("TNO", typeof(long)));
        //        //sPositionAdditionalInfo
        //        WebGIS.RealtimeDataServer.CarRealData[] res = new WebGIS.RealtimeDataServer.CarRealData[] { };
        //        //调用WebGIS实时数据服务接口查询车辆轨迹数据
        //        if (souflag == "RDS")
        //        {
        //            string[] cids = inparams["CIDs"].Split(',');
        //            WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
        //            res = wc.GetCarsData(sysflag, ToIntArray(cids));

        //        }
        //        else
        //        {     //从数据库查询车辆最后轨迹信息并转化处理
        //            res = CarDataConvent(GetCarsLastTrackForDB(sysflag, inparams["CIDs"]));
        //        }

        //        //利用查询结果填充DataTable
        //        foreach (WebGIS.RealtimeDataServer.CarRealData carRealData in res)
        //        {
        //            DataRow dr = dt.NewRow();
        //            dr["Alarm"] = carRealData.Alarm;
        //            dr["AlarmStr"] = carRealData.AlarmStr;
        //            dr["AltitudeMeters"] = carRealData.AltitudeMeters;
        //            dr["CarNum"] = carRealData.CarNum;
        //            dr["Carid"] = carRealData.Carid;
        //            dr["Heading"] = carRealData.Heading;
        //            dr["HeadingStr"] = carRealData.HeadingStr;
        //            dr["Lati"] = carRealData.Lati;
        //            dr["Long"] = carRealData.Long;
        //            dr["Speed"] = carRealData.Speed;
        //            dr["Status"] = carRealData.Status;
        //            dr["StatusStr"] = carRealData.StatusStr;
        //            dr["SumMiles"] = carRealData.SumMiles;
        //            dr["TDateTime"] = carRealData.TDateTime;
        //            dr["TNO"] = carRealData.TNO;
        //            dr["Status"] = carRealData.Status;
        //            dt.Rows.Add(dr);
        //        }

        //        //
        //        list_res.total = dt.Rows.Count;
        //        list_res.page = 0;
        //        list_res.size = 0;
        //        list_res.records = dt;

        //        Result = new ResponseResult(ResState.Success, "操作成功", list_res);
        //    }
        //    catch (Exception ex)
        //    {
        //        Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
        //    }


        //    return Result;
        //}
        ///// <summary>
        ///// 从数据库获取车辆最后轨迹信息
        ///// </summary>
        ///// <param name="sysflag">系统标志</param>
        ///// <param name="cids">车辆id字符串已逗号（,）分割</param>
        ///// <returns>返回车辆最后轨迹信息</returns>
        //private DataTable GetCarsLastTrackForDB(string sysflag, string cids)
        //{
        //    ComSqlHelper csh = new ComSqlHelper();
        //    SqlParameter[] Parameters = { new SqlParameter("@cids", cids) };
        //    return csh.FillDataSet(sysflag, WebProc.Proc("QSProc_GetCarsLastTrack"), Parameters).Tables[0];

        //}

        //private WebGIS.RealtimeDataServer.CarRealData[] CarDataConvent(DataTable dt)
        //{
        //    List<WebGIS.RealtimeDataServer.CarRealData> res = new List<WebGIS.RealtimeDataServer.CarRealData>();
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        DataRow dtr = dt.Rows[i];
        //        WebGIS.RealtimeDataServer.CarRealData cdm = new WebGIS.RealtimeDataServer.CarRealData();
        //        cdm.Carid = int.Parse(dtr["CID"].ToString());
        //        cdm.CarNum = dtr["CarNo"].ToString();
        //        cdm.Alarm = long.Parse(dtr["T_ALARM"].ToString());
        //        cdm.AlarmStr = CarDataConvert.ConvertAlarm(cdm.Alarm);
        //        cdm.AltitudeMeters = int.Parse(dtr["T_AltitudeMeters"] == DBNull.Value ? "0" : dtr["T_AltitudeMeters"].ToString());
        //        cdm.TDateTime = DateTime.Parse(dtr["T_DateTime"].ToString());
        //        cdm.Heading = int.Parse(dtr["T_Heading"].ToString());
        //        cdm.HeadingStr = CarDataConvert.ConvertDir(cdm.Heading);
        //        double[] s = CarDataConvert.ConvertCoordToGCJ02(double.Parse(dtr["T_Long"].ToString()), double.Parse(dtr["T_Lati"].ToString()));
        //        cdm.Lati = s[1];
        //        cdm.Long = s[0];
        //        cdm.Speed = float.Parse(dtr["T_Speed"].ToString());
        //        cdm.Status = int.Parse(dtr["T_Status"].ToString());
        //        cdm.StatusStr = CarDataConvert.ConvertStatus(cdm.Status);
        //        cdm.SumMiles = int.Parse(dtr["T_SumMiles"].ToString());
        //        if (dtr["TNO"] == DBNull.Value)
        //            continue;
        //        cdm.TNO = long.Parse(dtr["TNO"].ToString()); ;
        //        //cdm.sPositionAdditionalInfo = timeResponse.Tir[i].sPositionAdditionalInfo;
        //        res.Add(cdm);


        //    }
        //    return res.ToArray();
        //}

        ////
        ////
        //public ResponseResult EasyUIGrid2Excel(HttpContext context, Dictionary<string, string> inparams)
        //{
        //    var jsonString = inparams["excelWorkSheet"];
        //    //使用Newtonsoft.Json.Linq.JObject将json字符串转化成结构不固定的Class类
        //    JObject jsonObject = JObject.Parse(jsonString);


        //    string fileName = String.Concat(jsonObject["sheetName"], DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xls");

        //    //解决中文文件名乱码只在IE中有效
        //    //   filename = HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8);
        //    if (context.Request.UserAgent.ToLower().IndexOf("msie") > -1)
        //    {
        //        //当客户端使用IE时，对其进行编码；
        //        //使用 ToHexString 代替传统的 UrlEncode()；
        //        fileName = Helpers.CommonHelper.ToHexString(fileName);
        //    }
        //    if (context.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
        //    {
        //        //为了向客户端输出空格，需要在当客户端使用 Firefox 时特殊处理

        //        context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
        //    }
        //    else
        //        context.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);

        //    string extension = System.IO.Path.GetExtension(fileName);
        //    context.Response.ContentType = Helpers.CommonHelper.GetMimeType(extension);
        //    context.Response.ContentEncoding = Encoding.UTF8;
        //    Workbook workbook = Object2Workbook(jsonObject, context);

        //    //save
        //    string basepath = HttpRuntime.AppDomainAppPath.ToString();
        //    //string filePath = @"UI\Excel\Query\";
        //    string filePath = @"";
        //    string sd = basepath + filePath + fileName;
        //    workbook.Save(sd);

        //    //
        //    ResponseResult Result = new ResponseResult(ResState.Success, "", filePath + fileName);
        //    return Result;
        //}

        ////
        //private Workbook Object2Workbook(JObject jsonObject, HttpContext context)
        //{
        //    #region Aspose.Cell引用
        //    Aspose.Cells.License licExcel = new License();  //Aspose.Cells申明
        //    if (File.Exists(context.Server.MapPath("~/bin/Service/_extend/cellLic.lic")))
        //        licExcel.SetLicense(context.Server.MapPath("~/bin/Service/_extend/cellLic.lic"));
        //    #endregion

        //    Workbook workbook = new Workbook();

        //    Worksheet sheet = workbook.Worksheets[0];


        //    Styles styles = workbook.Styles;
        //    int styleIndex = styles.Add();
        //    Aspose.Cells.Style borderStyle = styles[styleIndex];

        //    borderStyle.Borders.DiagonalStyle = CellBorderType.None;
        //    borderStyle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        //    Cells cells = sheet.Cells;
        //    //sheet.FreezePanes(1, 1, 1, 0);//冻结第一行
        //    sheet.Name = jsonObject["sheetName"].ToString();//接受前台的Excel工作表名



        //    //为标题设置样式    
        //    Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式
        //    styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        //    styleTitle.Font.Name = "宋体";//文字字体
        //    styleTitle.Font.Size = 18;//文字大小
        //    styleTitle.Font.IsBold = true;//粗体

        //    //题头样式
        //    Style styleHeader = workbook.Styles[workbook.Styles.Add()];//新增样式
        //    styleHeader.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        //    styleHeader.Font.Name = "宋体";//文字字体
        //    styleHeader.Font.Size = 14;//文字大小
        //    styleHeader.Font.IsBold = true;//粗体
        //    styleHeader.IsTextWrapped = true;//单元格内容自动换行
        //    styleHeader.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //    styleHeader.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //    styleHeader.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //    styleHeader.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

        //    //内容样式
        //    Style styleContent = workbook.Styles[workbook.Styles.Add()];//新增样式
        //    styleContent.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        //    styleContent.Font.Name = "宋体";//文字字体
        //    styleContent.Font.Size = 12;//文字大小
        //    styleContent.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //    styleContent.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //    styleContent.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //    styleContent.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;


        //    var rowCount = jsonObject["rows"].ToArray().Count();//表格行数
        //    var columnCount = jsonObject["columns"].ToArray().Count();//表格列数


        //    //生成行1 标题行   
        //    cells.Merge(0, 0, 1, columnCount);//合并单元格
        //    cells[0, 0].PutValue(jsonObject["sheetName"]);//填写内容
        //    cells[0, 0].Style = styleTitle;
        //    cells.SetRowHeight(0, 25);

        //    //生成题头列行
        //    for (int i = 0; i < columnCount; i++)
        //    {
        //        cells[1, i].PutValue(jsonObject["columns"].ToArray()[i]["title"]);
        //        cells[1, i].Style = styleHeader;
        //        cells.SetRowHeight(1, 23);
        //    }

        //    //生成地址列
        //    cells[1, columnCount].PutValue("地址");
        //    cells[1, columnCount].Style = styleHeader;
        //    cells.SetRowHeight(1, 23);

        //    //调用接口进行地址解析
        //    List<AddressConvert.DLngLat> convertAddrList = new List<AddressConvert.DLngLat>(); //存放需要解析地址的轨迹点的坐标

        //    for (int i = 0; i < rowCount; i++)
        //    {
        //        //
        //        AddressConvert.DLngLat dlnglat = new AddressConvert.DLngLat();
        //        dlnglat.Lng = double.Parse(jsonObject["rows"].ToArray()[i]["Long"].ToString());
        //        dlnglat.Lat = double.Parse(jsonObject["rows"].ToArray()[i]["Lati"].ToString());
        //        convertAddrList.Add(dlnglat);
        //    }

        //    //
        //    string[] conArr = CommLibrary.AddressConvert.AddConvertBatch(convertAddrList);


        //    //生成内容行,第三行起始
        //    //生成数据行
        //    for (int i = 0; i < rowCount; i++)
        //    {
        //        for (int k = 0; k < columnCount; k++)
        //        {
        //            var currentColumnName = jsonObject["columns"].ToArray()[k]["field"];
        //            var b = jsonObject["rows"].ToArray()[i][currentColumnName.ToString()];
        //            cells[2 + i, k].PutValue(jsonObject["rows"].ToArray()[i][currentColumnName.ToString()]);
        //            //cells[2 + i, k].PutValue(jsonObject.rows[i][currentColumnName.Value]);
        //            cells[2 + i, k].Style = styleContent;
        //        }

        //        //地址行
        //        cells[2 + i, columnCount].PutValue(conArr[i]);
        //        //cells[2 + i, k].PutValue(jsonObject.rows[i][currentColumnName.Value]);
        //        cells[2 + i, columnCount].Style = styleContent;


        //        cells.SetRowHeight(2 + i, 22);
        //    }


        //    //添加制表日期
        //    cells[2 + rowCount, columnCount].PutValue("制表日期:" + DateTime.Now.ToShortDateString());
        //    sheet.AutoFitColumns();//让各列自适应宽度
        //    sheet.AutoFitRows();//让各行自适应宽度
        //    return workbook;
        //}

        //public ResponseResult getCarHisTrack(Dictionary<string, string> inparams)
        //{
        //    ResponseResult Result = null;

        //    if (!inparams.Keys.Contains("CID") || !inparams.Keys.Contains("Sdatetime") || !inparams.Keys.Contains("Edatetime"))
        //    {
        //        Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
        //        return Result;
        //    }
        //    try
        //    {


        //        int carid = int.Parse(inparams["CID"]);
        //        string Sdatetime = inparams["Sdatetime"];
        //        string Edatetime = inparams["Edatetime"];
        //        //调用存储过程查询车辆轨迹数据
        //        //todo
        //        DataTable dt = new DataTable();








        //        ResList list_res = new ResList();
        //        if (dt.Rows.Count > 0)
        //        {
        //            //进行坐标纠偏  

        //            //add end
        //            list_res.total = dt.Rows.Count;
        //            list_res.page = 0;
        //            list_res.size = 0;
        //            list_res.records = dt;

        //            Result = new ResponseResult(ResState.Success, "", list_res);
        //        }
        //        else
        //        {
        //            list_res.total = 0;
        //            list_res.page = 0;
        //            list_res.size = 0;
        //            list_res.records = dt;
        //            Result = new ResponseResult(ResState.Success, "", list_res);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
        //    }


        //    return Result;
        //}
        //private string GetDir(double s_fx)
        //{
        //    string fxf = "↑";
        //    if (s_fx >= 0 && s_fx < 22.5 || s_fx > 337.5 && s_fx <= 360)
        //    {
        //        fxf = "↑";
        //    }
        //    else if (22.5 < s_fx && s_fx < 67.5)
        //    {
        //        fxf = "↗";
        //    }
        //    else if (67.5 < s_fx && s_fx < 112.5)
        //    {
        //        fxf = "→";
        //    }
        //    else if (112.5 < s_fx && s_fx < 157.5)
        //    {
        //        fxf = "↘";
        //    }
        //    else if (157.5 < s_fx && s_fx < 202.5)
        //    {
        //        fxf = "↓";
        //    }
        //    else if (202.5 < s_fx && s_fx < 247.5)
        //    {
        //        fxf = "↙";
        //    }
        //    else if (247.5 < s_fx && s_fx < 292.5)
        //    {
        //        fxf = "←";
        //    }
        //    else if (292.5 < s_fx && s_fx < 337.5)
        //    {
        //        fxf = "↖";
        //    }

        //    return fxf;
        //}
    }



}