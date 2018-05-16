using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CommLibrary;
using Aspose.Cells;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Data.SqlClient;
using WebGIS;
using WebGIS.RealtimeDataServer;

namespace SysService
{
    public class monitor
    {
        /// <summary>
        /// 字符串数组转换整形数组
        /// </summary>
        /// <param name="Content">字符串数组</param>
        /// <returns></returns>
        public static long[] ToIntArray(string[] Content)
        {
            long[] c = new long[Content.Length];
            for (int i = 0; i < Content.Length; i++)
            {
                c[i] = Convert.ToInt64(Content[i].ToString());
            }
            return c;
        }
        string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
        public ResponseResult getCarLastTrack(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            if (!inparams.Keys.Contains("CIDs"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            string sysflag = inparams["sysflag"];
            ResList list_res = new ResList();
            try
            {

                //调用存储过程或WebGIS实时数据服务接口查询车辆轨迹数据
                DataTable dt = new DataTable();

                //初始化DataTable的列
                dt.Columns.Add(new DataColumn("Alarm", typeof(long)));
                dt.Columns.Add(new DataColumn("Alarm808", typeof(long)));
                dt.Columns.Add(new DataColumn("AlarmExt808", typeof(long)));
                dt.Columns.Add(new DataColumn("AlarmStr", typeof(string)));
                dt.Columns.Add(new DataColumn("AltitudeMeters", typeof(int)));
                dt.Columns.Add(new DataColumn("CarNum", typeof(string)));
                dt.Columns.Add(new DataColumn("Carid", typeof(long)));
                dt.Columns.Add(new DataColumn("Heading", typeof(int)));
                dt.Columns.Add(new DataColumn("HeadingStr", typeof(string)));
                dt.Columns.Add(new DataColumn("Lati", typeof(float)));
                dt.Columns.Add(new DataColumn("Long", typeof(float)));
                dt.Columns.Add(new DataColumn("Speed", typeof(float)));
                dt.Columns.Add(new DataColumn("Status", typeof(long)));
                dt.Columns.Add(new DataColumn("Status808", typeof(long)));
                dt.Columns.Add(new DataColumn("StatusExt808", typeof(long)));
                dt.Columns.Add(new DataColumn("StatusStr", typeof(string)));
                dt.Columns.Add(new DataColumn("SumMiles", typeof(int)));
                dt.Columns.Add(new DataColumn("TDateTime", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("TNO", typeof(long)));
                dt.Columns.Add(new DataColumn("OnlineStatus", typeof(int)));
                dt.Columns.Add(new DataColumn("OnlineStatusStr", typeof(string)));
                //sPositionAdditionalInfo
                WebGIS.RealtimeDataServer.CarRealData[] res = new WebGIS.RealtimeDataServer.CarRealData[] { };

                res = CarRealDataByCids(sysflag, ToIntArray(inparams["CIDs"].Split(',')));


                //利用查询结果填充DataTable
                foreach (WebGIS.RealtimeDataServer.CarRealData carRealData in res)
                {
                    DataRow dr = dt.NewRow();
                    dr["Alarm"] = carRealData.Alarm;
                    dr["Alarm808"] = carRealData.Alarm808;
                    dr["AlarmExt808"] = carRealData.AlarmExt808;
                    dr["AlarmStr"] = carRealData.AlarmStr;
                    dr["AltitudeMeters"] = carRealData.AltitudeMeters;
                    dr["CarNum"] = carRealData.CarNum;
                    dr["Carid"] = carRealData.Carid;
                    dr["Heading"] = carRealData.Heading;
                    dr["HeadingStr"] = carRealData.HeadingStr;
                    dr["Lati"] = carRealData.Lati;
                    dr["Long"] = carRealData.Long;
                    dr["Speed"] = carRealData.Speed;
                    dr["Status"] = carRealData.Status;
                    dr["Status808"] = carRealData.Status808;
                    dr["StatusExt808"] = carRealData.StatusExt808;
                    dr["StatusStr"] = carRealData.StatusStr;
                    dr["SumMiles"] = carRealData.SumMiles;
                    dr["TDateTime"] = carRealData.TDateTime;
                    dr["TNO"] = carRealData.TNO;
                    dr["OnlineStatus"] = carRealData.OnlineStatus;
                    dr["OnlineStatusStr"] = carRealData.OnlineStatusStr;
                    //carRealData.sPositionAdditionalInfo
                    dt.Rows.Add(dr);
                }

                //
                list_res.total = dt.Rows.Count;
                list_res.page = 0;
                list_res.size = 0;
                list_res.records = dt;

                Result = new ResponseResult(ResState.Success, "操作成功", list_res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
                LogHelper.WriteError("getCarLastTrack", ex);
            }


            return Result;
        }
        /// <summary>
        /// 获取车辆实时数据导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getCarLastTrackOut(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            if (!inparams.Keys.Contains("CIDs"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            string sysflag = inparams["sysflag"];
            ResList list_res = new ResList();
            try
            {

                //调用存储过程或WebGIS实时数据服务接口查询车辆轨迹数据
                DataTable dt = new DataTable();

                //初始化DataTable的列
               
                dt.Columns.Add(new DataColumn("CarNum", typeof(string)));
                dt.Columns.Add(new DataColumn("TDateTime", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Speed", typeof(float)));
                dt.Columns.Add(new DataColumn("StatusStr", typeof(string)));
                dt.Columns.Add(new DataColumn("AlarmStr", typeof(string)));
                dt.Columns.Add(new DataColumn("OnlineStatusStr", typeof(string)));               
                dt.Columns.Add(new DataColumn("HeadingStr", typeof(string)));
                dt.Columns.Add(new DataColumn("SumMiles", typeof(int)));
                dt.Columns.Add(new DataColumn("Lati", typeof(float)));
                dt.Columns.Add(new DataColumn("Long", typeof(float)));
                dt.Columns.Add(new DataColumn("Address", typeof(string)));
                //dt.Columns.Add(new DataColumn("Alarm", typeof(long)));
                //dt.Columns.Add(new DataColumn("Alarm808", typeof(long)));
                //dt.Columns.Add(new DataColumn("AlarmExt808", typeof(long)));
                //dt.Columns.Add(new DataColumn("Carid", typeof(long)));
                //dt.Columns.Add(new DataColumn("Heading", typeof(int)));
                //dt.Columns.Add(new DataColumn("Status", typeof(long)));
                //dt.Columns.Add(new DataColumn("Status808", typeof(long)));
                //dt.Columns.Add(new DataColumn("StatusExt808", typeof(long)));
                // dt.Columns.Add(new DataColumn("AltitudeMeters", typeof(int)));   
                //dt.Columns.Add(new DataColumn("TNO", typeof(long)));
                //dt.Columns.Add(new DataColumn("OnlineStatus", typeof(int)));
               
                //sPositionAdditionalInfo
                WebGIS.RealtimeDataServer.CarRealData[] res = new WebGIS.RealtimeDataServer.CarRealData[] { };

                res = CarRealDataByCids(sysflag, ToIntArray(inparams["CIDs"].Split(',')));

                List<AddressConvert.DLngLat> convertAddrList = new List<AddressConvert.DLngLat>(); //存放需要解析地址的轨迹点的坐标

                
                //利用查询结果填充DataTable
                foreach (WebGIS.RealtimeDataServer.CarRealData carRealData in res)
                {
                    DataRow dr = dt.NewRow();
                 
                    dr["AlarmStr"] = carRealData.AlarmStr;                  
                    dr["CarNum"] = carRealData.CarNum;                  
                    dr["HeadingStr"] = carRealData.HeadingStr;
                    dr["Lati"] = carRealData.Lati;
                    dr["Long"] = carRealData.Long;
                    dr["Speed"] = carRealData.Speed;                   
                    dr["StatusStr"] = carRealData.StatusStr;
                    dr["SumMiles"] = carRealData.SumMiles;
                    dr["TDateTime"] = carRealData.TDateTime;   
                    dr["OnlineStatusStr"] = carRealData.OnlineStatusStr;
                    dt.Rows.Add(dr);
                    AddressConvert.DLngLat dlnglat = new AddressConvert.DLngLat();
                    dlnglat.Lng = carRealData.Long;
                    dlnglat.Lat =carRealData.Lati;
                    convertAddrList.Add(dlnglat);
                }
                string[] conArr = CommLibrary.AddressConvert.AddConvertBatch(convertAddrList);
                for (int i = 0; i < dt.Rows.Count;i++ )
                {
                    dt.Rows[i]["Address"]=conArr[i];
                }

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "GPS时间", "车速", "车辆状态", "报警状态", "在线状态", "方向", "里程", "经度", "纬度", "地址" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                npoiHelper.WorkbookName = "车辆实时监控数据" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\Query\";
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                return Result;
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OtherError, "", ex.Message + ex.StackTrace);
                return Result;
            }
        }
        public WebGIS.RealtimeDataServer.CarRealData[] CarRealDataByCids(string sysflag, long[] cids)
        {
            
            RDSConfigModel rc = RDSConfig.GetRDS(sysflag);
            WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
            //调用WebGIS实时数据服务接口查询车辆轨迹数据
            if (souflag == "RDS" && rc != null && rc.RunFlag)
            {

                wc.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                List<CarRealData> tempres = new List<CarRealData>();
                int reqlength = 500;

                List<long> tempp = new List<long>();
                for (int i = 0; i < cids.Length; i++)
                {
                    tempp.Add(cids[i]);
                    if (tempp.Count == reqlength || i == cids.Length - 1)
                    {
                        try
                        {
                            tempres.AddRange(wc.GetCarsData(sysflag, tempp.ToArray()));

                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteError("远程车辆实时数据请求异常", ex, false);
                        }
                        tempp.Clear();
                    }
                }
                return tempres.ToArray();

            }
            else
            {     //从数据库查询车辆最后轨迹信息并转化处理
                string OldSplitDate = System.Configuration.ConfigurationManager.AppSettings[sysflag + "_OldSplitDate"];
                DateTime olddate = DateTime.MinValue;
                if (!string.IsNullOrEmpty(OldSplitDate) && DateTime.TryParse(OldSplitDate, out olddate))
                {

                }
                string strcids = string.Join(",", cids);
                return CarDataConvent(GetCarsLastTrackForDB(sysflag, strcids), olddate);
            }
        }

        /// <summary>
        /// 从数据库获取车辆最后轨迹信息
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cids">车辆id字符串已逗号（,）分割</param>
        /// <returns>返回车辆最后轨迹信息</returns>
        private DataTable GetCarsLastTrackForDB(string sysflag, string cids)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cids", cids) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QSProc_GetCarsLastTrack"), Parameters).Tables[0];

        }
        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="OldDate">扩展报警状态位新老数据分割时间 </param>
        /// <returns></returns>
        private WebGIS.RealtimeDataServer.CarRealData[] CarDataConvent(DataTable dt, DateTime OldDate)
        {
            List<WebGIS.RealtimeDataServer.CarRealData> res = new List<WebGIS.RealtimeDataServer.CarRealData>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dtr = dt.Rows[i];
                WebGIS.RealtimeDataServer.CarRealData cdm = new WebGIS.RealtimeDataServer.CarRealData();
                cdm.TDateTime = DateTime.Parse(dtr["T_DateTime"].ToString());
                cdm.Carid = int.Parse(dtr["CID"].ToString());
                cdm.CarNum = dtr["CarNo"].ToString();
                cdm.Alarm = long.Parse(dtr["T_ALARM"].ToString());
                cdm.Alarm808 = long.Parse(dtr["T_Alarm808"].ToString());
                cdm.AlarmExt808 = long.Parse(dtr["T_AlarmExt808"].ToString());


                cdm.AltitudeMeters = int.Parse(dtr["T_AltitudeMeters"] == DBNull.Value ? "0" : dtr["T_AltitudeMeters"].ToString());

                cdm.Heading = int.Parse(dtr["T_Heading"].ToString());
                cdm.HeadingStr = CarDataConvert.ConvertDir(cdm.Heading);
                double[] s = CarDataConvert.ConvertCoordToGCJ02(double.Parse(dtr["T_Long"].ToString()), double.Parse(dtr["T_Lati"].ToString()));
                cdm.Lati = s[1];
                cdm.Long = s[0];
                cdm.Speed = float.Parse(dtr["T_Speed"].ToString());
                cdm.Status = int.Parse(dtr["T_Status"].ToString());
                cdm.Status808 = int.Parse(dtr["T_Status808"].ToString());
                cdm.StatusExt808 = int.Parse(dtr["T_StatusExt808"].ToString());
                if (cdm.TDateTime > OldDate)
                {
                    cdm.AlarmStr = AlarmStatusAnalysis.ConvertMain.AlarmConvertMain(cdm.Alarm, cdm.Alarm808, cdm.AlarmExt808);
                    cdm.StatusStr = AlarmStatusAnalysis.ConvertMain.StatusConvertMain(cdm.Status, cdm.Status808, cdm.StatusExt808);
                }
                else
                {
                    cdm.AlarmStr = AlarmStatusAnalysis.ConvertMain.AlarmConvertMain(cdm.Alarm, cdm.Alarm808, cdm.AlarmExt808, false);
                    cdm.StatusStr = AlarmStatusAnalysis.ConvertMain.StatusConvertMain(cdm.Status, cdm.Status808, cdm.StatusExt808, false);
                }

                cdm.SumMiles = int.Parse(dtr["T_SumMiles"].ToString());
                if (dtr["TNO"] == DBNull.Value)
                    continue;
                cdm.TNO = long.Parse(dtr["TNO"].ToString()); ;
                //cdm.sPositionAdditionalInfo = timeResponse.Tir[i].sPositionAdditionalInfo;

                cdm.OnlineStatus = (DateTime.Now - cdm.TDateTime).TotalMinutes < 5 ? 1 : 2;

                if (cdm.OnlineStatus == 2)
                {
                    cdm.OnlineStatusStr = "停止";
                }
                else
                {
                    cdm.OnlineStatusStr = "上线";
                }

                res.Add(cdm);


            }
            return res.ToArray();
        }


        //
        //
        public ResponseResult EasyUIGrid2Excel(HttpContext context, Dictionary<string, string> inparams)
        {
            var jsonString = inparams["excelWorkSheet"];
            //使用Newtonsoft.Json.Linq.JObject将json字符串转化成结构不固定的Class类
            JObject jsonObject = JObject.Parse(jsonString);


            string fileName = String.Concat(jsonObject["sheetName"], DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xls");

            //解决中文文件名乱码只在IE中有效
            //   filename = HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8);
            if (context.Request.UserAgent.ToLower().IndexOf("msie") > -1)
            {
                //当客户端使用IE时，对其进行编码；
                //使用 ToHexString 代替传统的 UrlEncode()；
                fileName = WebGIS.CommonHelper.ToHexString(fileName);
            }
            if (context.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                //为了向客户端输出空格，需要在当客户端使用 Firefox 时特殊处理

                context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            }
            else
                context.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);

            string extension = System.IO.Path.GetExtension(fileName);
            context.Response.ContentType = WebGIS.CommonHelper.GetMimeType(extension);
            context.Response.ContentEncoding = Encoding.UTF8;
            Workbook workbook = Object2Workbook(jsonObject, context);

            //save
            string basepath = HttpRuntime.AppDomainAppPath.ToString();
            string filePath = @"UI\Excel\Monitor";
            //string filePath = @"";
            string sd = basepath + filePath + fileName;
            workbook.Save(sd);

            //
            ResponseResult Result = new ResponseResult(ResState.Success, "", filePath + fileName);
            return Result;
        }

        //
        private Workbook Object2Workbook(JObject jsonObject, HttpContext context)
        {
            #region Aspose.Cell引用
            Aspose.Cells.License licExcel = new License();  //Aspose.Cells申明
            if (File.Exists(context.Server.MapPath("~/bin/Service/_extend/cellLic.lic")))
                licExcel.SetLicense(context.Server.MapPath("~/bin/Service/_extend/cellLic.lic"));
            #endregion

            Workbook workbook = new Workbook();

            Worksheet sheet = workbook.Worksheets[0];


            Styles styles = workbook.Styles;
            int styleIndex = styles.Add();
            Aspose.Cells.Style borderStyle = styles[styleIndex];

            borderStyle.Borders.DiagonalStyle = CellBorderType.None;
            borderStyle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            Cells cells = sheet.Cells;
            //sheet.FreezePanes(1, 1, 1, 0);//冻结第一行
            sheet.Name = jsonObject["sheetName"].ToString();//接受前台的Excel工作表名



            //为标题设置样式    
            Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            styleTitle.Font.Name = "宋体";//文字字体
            styleTitle.Font.Size = 18;//文字大小
            styleTitle.Font.IsBold = true;//粗体

            //题头样式
            Style styleHeader = workbook.Styles[workbook.Styles.Add()];//新增样式
            styleHeader.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            styleHeader.Font.Name = "宋体";//文字字体
            styleHeader.Font.Size = 14;//文字大小
            styleHeader.Font.IsBold = true;//粗体
            styleHeader.IsTextWrapped = true;//单元格内容自动换行
            styleHeader.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //内容样式
            Style styleContent = workbook.Styles[workbook.Styles.Add()];//新增样式
            styleContent.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            styleContent.Font.Name = "宋体";//文字字体
            styleContent.Font.Size = 12;//文字大小
            styleContent.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            styleContent.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            styleContent.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            styleContent.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;


            var rowCount = jsonObject["rows"].ToArray().Count();//表格行数
            var columnCount = jsonObject["columns"].ToArray().Count();//表格列数


            //生成行1 标题行   
            cells.Merge(0, 0, 1, columnCount);//合并单元格
            cells[0, 0].PutValue(jsonObject["sheetName"]);//填写内容
            cells[0, 0].Style = styleTitle;
            cells.SetRowHeight(0, 25);

            //生成题头列行
            for (int i = 0; i < columnCount; i++)
            {
                cells[1, i].PutValue(jsonObject["columns"].ToArray()[i]["title"]);
                cells[1, i].Style = styleHeader;
                cells.SetRowHeight(1, 23);
            }

            //生成地址列
            cells[1, columnCount].PutValue("地址");
            cells[1, columnCount].Style = styleHeader;
            cells.SetRowHeight(1, 23);

            //调用接口进行地址解析
            List<AddressConvert.DLngLat> convertAddrList = new List<AddressConvert.DLngLat>(); //存放需要解析地址的轨迹点的坐标

            for (int i = 0; i < rowCount; i++)
            {
                //
                AddressConvert.DLngLat dlnglat = new AddressConvert.DLngLat();
                dlnglat.Lng = double.Parse(jsonObject["rows"].ToArray()[i]["Long"].ToString());
                dlnglat.Lat = double.Parse(jsonObject["rows"].ToArray()[i]["Lati"].ToString());
                convertAddrList.Add(dlnglat);
            }

            //
            string[] conArr = CommLibrary.AddressConvert.AddConvertBatch(convertAddrList);


            //生成内容行,第三行起始
            //生成数据行
            for (int i = 0; i < rowCount; i++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    var currentColumnName = jsonObject["columns"].ToArray()[k]["field"];
                    var b = jsonObject["rows"].ToArray()[i][currentColumnName.ToString()];
                    cells[2 + i, k].PutValue(jsonObject["rows"].ToArray()[i][currentColumnName.ToString()]);
                    //cells[2 + i, k].PutValue(jsonObject.rows[i][currentColumnName.Value]);
                    cells[2 + i, k].Style = styleContent;
                }

                //地址行
                cells[2 + i, columnCount].PutValue(conArr[i]);
                //cells[2 + i, k].PutValue(jsonObject.rows[i][currentColumnName.Value]);
                cells[2 + i, columnCount].Style = styleContent;


                cells.SetRowHeight(2 + i, 22);
            }


            //添加制表日期
            cells[2 + rowCount, columnCount].PutValue("制表日期:" + DateTime.Now.ToShortDateString());
            sheet.AutoFitColumns();//让各列自适应宽度
            sheet.AutoFitRows();//让各行自适应宽度
            return workbook;
        }


        private string GetDir(double s_fx)
        {
            string fxf = "↑";
            if (s_fx >= 0 && s_fx < 22.5 || s_fx > 337.5 && s_fx <= 360)
            {
                fxf = "↑";
            }
            else if (22.5 < s_fx && s_fx < 67.5)
            {
                fxf = "↗";
            }
            else if (67.5 < s_fx && s_fx < 112.5)
            {
                fxf = "→";
            }
            else if (112.5 < s_fx && s_fx < 157.5)
            {
                fxf = "↘";
            }
            else if (157.5 < s_fx && s_fx < 202.5)
            {
                fxf = "↓";
            }
            else if (202.5 < s_fx && s_fx < 247.5)
            {
                fxf = "↙";
            }
            else if (247.5 < s_fx && s_fx < 292.5)
            {
                fxf = "←";
            }
            else if (292.5 < s_fx && s_fx < 337.5)
            {
                fxf = "↖";
            }

            return fxf;
        }
    }
}