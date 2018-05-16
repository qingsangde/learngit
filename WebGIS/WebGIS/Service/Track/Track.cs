using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;
using System.Data;
using CommLibrary;
using System.Data.SqlClient;
using System.Configuration;

namespace SysService
{
    public class Track
    {
        string SysFlag = "";
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getTracks(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;
            string cid;
            string cno;
            string st;
            string et;
            Double os;
            Double od;
            string of;

            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("cno") ||
                  !inparams.Keys.Contains("st") || !inparams.Keys.Contains("et") || !inparams.Keys.Contains("of") ||
                 !inparams.Keys.Contains("os") || !inparams.Keys.Contains("od")
                 )
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                if (inparams["cid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["cno"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["st"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["et"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["os"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "速度阀值不能为空", null);
                    return Result;
                }
                if (inparams["od"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "漂移阀值不能为空", null);
                    return Result;
                }
                if (inparams["of"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "地址解析不能为空", null);
                    return Result;
                }
                 sysflag = inparams["sysflag"];
                 SysFlag = sysflag;
                 cid = inparams["cid"];
                 cno = inparams["cno"];
                 st = inparams["st"];
                 et = inparams["et"];
                 os =Double.Parse(inparams["os"]);
                 od =Double.Parse(inparams["od"]);
                 of = inparams["of"];

                //调用存储过程查询车辆数据
                 DataTable dt = getTracks(sysflag, cid,cno, st, et,os,od,of);
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

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getTracksOutPut(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;
            string cid;
            string cno;
            string st;
            string et;
            Double os;
            Double od;
            string of;

            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("cno") ||
                 !inparams.Keys.Contains("st") || !inparams.Keys.Contains("et") || !inparams.Keys.Contains("of")||
                !inparams.Keys.Contains("os") || !inparams.Keys.Contains("od") 
                )
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                if (inparams["cid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["cno"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["st"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["et"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["os"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "速度阀值不能为空", null);
                    return Result;
                }
                if (inparams["od"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "漂移阀值不能为空", null);
                    return Result;
                }
                if (inparams["of"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "地址解析不能为空", null);
                    return Result;
                }
                sysflag = inparams["sysflag"];
                SysFlag = inparams["sysflag"];
                cid = inparams["cid"];
                cno = inparams["cno"];
                st = inparams["st"];
                et = inparams["et"];
                os = Double.Parse(inparams["os"]);
                od = Double.Parse(inparams["od"]);
                of = inparams["of"];

            
                //调用存储过程查询车辆数据
                DataTable dt = getTracks(sysflag, cid, cno, st, et,os,od, of);
                dt.Columns.Remove("od");
                dt.Columns.Remove("os");
                dt.Columns.Remove("odd");

                dt.Columns.Remove("T_ALARM808");
                dt.Columns.Remove("T_ALARMExt808");
                dt.Columns.Remove("T_Status808");
                dt.Columns.Remove("T_StatusExt808");


                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "卫星定位时间", "速度", "经度", "纬度", "方向", "海拔", "运行状态", "报警状态", "里程", "地址" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);

                npoiHelper.WorkbookName = cno +" "+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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

        #region 获取轨迹部分
        public DataTable getTracks(string sysflag, string cid, string cno, string st, string et, Double os, Double od, string of)
        {
            DataTable dt = null;
            try
            {
               //数据库获取数据
               dt=GetTracksFromDB(sysflag,cid,st,et);
               if(dt.Rows.Count>0){
                   //格式化、纠偏、报警解析、里程计算
                   dt = ConvertTable(dt,cno,os,od); 
                   if(of=="1"){
                       //地址解析
                       dt = AddAddress(dt);
                   }
               }
               return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable GetTracksFromDB(string sysflag, string cid, string st, string et)
        {
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@cid", cid);
                oaPara[1] = new SqlParameter("@BEGINTime", st);
                oaPara[2] = new SqlParameter("@ENDTime", et);

                DataTable dt = new DataTable();
                return csh.FillDataSet(sysflag, WebProc.Proc("GetTrackByCid"), oaPara).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable ConvertTable(DataTable track, string cno, Double os, Double od) 
        {
            try
            {
                //从数据库查询车辆最后轨迹信息并转化处理
                string OldSplitDate = System.Configuration.ConfigurationManager.AppSettings[SysFlag + "_OldSplitDate"];
                DateTime olddate = DateTime.MinValue;
                if (!string.IsNullOrEmpty(OldSplitDate) && DateTime.TryParse(OldSplitDate, out olddate))
                {
                    
                }
                 

                track=FormatTable(track);//格式化table
                for (int i = 0; i < track.Rows.Count; i++)
                {
                    DataRow dtr = track.Rows[i];

                    //序号
                    dtr["no"] = i + 1;
                    //车牌号
                    dtr["cno"] = cno;
                    //Gps时间
                    dtr["dt"] = dtr["T_DateTime"];

                    //速度
                    Double sp = Double.Parse(dtr["T_Speed"].ToString().Trim());
                    dtr["sp"] = sp;
                    if(sp<os){
                        dtr["os"] = false;
                    }
                    else{
                        dtr["os"] = true;
                    }
                    //经纬度纠偏
                    double[] s = CarDataConvert.ConvertCoordToGCJ02(double.Parse(dtr["T_Long"].ToString()), double.Parse(dtr["T_Lati"].ToString()));
                    //经度
                    dtr["lng"] =  Math.Round(s[0],8);
                    //纬度
                    dtr["lat"] =  Math.Round(s[1],8);

                    //方向
                    dtr["dir"] = CarDataConvert.ConvertDir(int.Parse(dtr["T_Heading"].ToString()));
                    //海拔
                    dtr["tam"] = Double.Parse(dtr["T_AltitudeMeters"] == DBNull.Value ? "0" : dtr["T_AltitudeMeters"].ToString()).ToString().Trim();
                    dtr["T_ALARM808"] = Double.Parse(dtr["T_ALARM808"] == DBNull.Value ? "0" : dtr["T_ALARM808"].ToString()).ToString().Trim();
                    dtr["T_ALARMExt808"] = Double.Parse(dtr["T_ALARMExt808"] == DBNull.Value ? "0" : dtr["T_ALARMExt808"].ToString()).ToString().Trim();
                    dtr["T_Status808"] = Double.Parse(dtr["T_Status808"] == DBNull.Value ? "0" : dtr["T_Status808"].ToString()).ToString().Trim();
                    dtr["T_StatusExt808"] = Double.Parse(dtr["T_StatusExt808"] == DBNull.Value ? "0" : dtr["T_StatusExt808"].ToString()).ToString().Trim();
                    if (DateTime.Parse(dtr["T_DateTime"].ToString()) > olddate)
                    { 
                        //报警状态
                        dtr["ta"] = AlarmStatusAnalysis.ConvertMain.AlarmConvertMain(long.Parse(dtr["T_ALARM"].ToString()), long.Parse(dtr["T_ALARM808"].ToString()), long.Parse(dtr["T_ALARMExt808"].ToString()));
                         //运行状态
                        dtr["ts"] = AlarmStatusAnalysis.ConvertMain.StatusConvertMain(int.Parse(dtr["T_Status"].ToString()), int.Parse(dtr["T_Status808"].ToString()), int.Parse(dtr["T_StatusExt808"].ToString()));
                    }
                    else
                    {
                        //报警状态
                        dtr["ta"] = AlarmStatusAnalysis.ConvertMain.AlarmConvertMain(long.Parse(dtr["T_ALARM"].ToString()), long.Parse(dtr["T_ALARM808"].ToString()), long.Parse(dtr["T_ALARMExt808"].ToString()),false);
                        //运行状态
                        dtr["ts"] = AlarmStatusAnalysis.ConvertMain.StatusConvertMain(int.Parse(dtr["T_Status"].ToString()), int.Parse(dtr["T_Status808"].ToString()), int.Parse(dtr["T_StatusExt808"].ToString()),false);
                    }
                  
                   
                    //里程解析
                    dtr["tsm"] = Double.Parse(dtr["T_SumMiles"].ToString().Trim());
                   

                    if (i == 0)
                    {
                        dtr["od"] =false;
                        dtr["odd"] = 0;
                    }
                    else
                    {
                        DataRow dtrLast = track.Rows[i-1];
                        Double lat1 =Double.Parse(dtrLast["lat"].ToString());
                        Double lng1 = Double.Parse(dtrLast["lng"].ToString());
                        Double lat2 = Double.Parse(dtr["lat"].ToString());
                        Double lng2 = Double.Parse(dtr["lng"].ToString());
                        Double sod =Math.Round(CarDataConvert.GetDistance(lat1, lng1, lat2, lng2),2);
                        if (sod<od)
                        {
                            dtr["od"] = false;
                        }
                        else{
                            dtr["od"] = true;
                        }
                        dtr["odd"] = Math.Round(Double.Parse(dtrLast["odd"].ToString()) + sod,2);
                    }
                    //地址解析
                    dtr["add"] =null;
                }

                //移除无用列
                track.Columns.Remove("cid");
                track.Columns.Remove("T_DateTime");
                track.Columns.Remove("T_Lati");
                track.Columns.Remove("T_Long");
                track.Columns.Remove("T_Heading");
                track.Columns.Remove("T_Speed");
                track.Columns.Remove("T_Status");
                track.Columns.Remove("T_ALARM");
                track.Columns.Remove("T_SumMiles");
                track.Columns.Remove("T_AltitudeMeters");

                return track;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable FormatTable(DataTable track) 
        {
            try
            {
                track.Columns.Add("no", Type.GetType("System.String"));     //序号
                track.Columns.Add("cno", Type.GetType("System.String"));    //车牌号
                track.Columns.Add("dt", Type.GetType("System.DateTime"));     //GPS时间
                track.Columns.Add("sp", Type.GetType("System.Double"));     //速度

                track.Columns.Add("lng", Type.GetType("System.Double"));     //经度
                track.Columns.Add("lat", Type.GetType("System.Double"));     //纬度
                track.Columns.Add("dir", Type.GetType("System.String"));     //方向
                track.Columns.Add("tam", Type.GetType("System.Double"));     //海拔

                track.Columns.Add("ts", Type.GetType("System.String"));    //运行状态
               
                track.Columns.Add("ta", Type.GetType("System.String"));    //报警状态
                
                track.Columns.Add("tsm", Type.GetType("System.Double"));    //Gps里程

                track.Columns.Add("os", Type.GetType("System.Boolean"));     //计算即时里程
                track.Columns.Add("od", Type.GetType("System.Boolean"));     //计算即时里程

                track.Columns.Add("odd", Type.GetType("System.Double"));    //计算总里程
                track.Columns.Add("add", Type.GetType("System.String"));    //地址

                return track;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        
        }

      
        /// <summary>
        /// 批量地址解析
        /// </summary>
        /// <param name="dt">轨迹数据DataTable</param>
        /// <returns>地址解析后的DataTable</returns>
        private DataTable AddAddress(DataTable dt)
        {
            try
            {
                    DataTable addrDt = dt.Copy();
                    List<AddressConvert.DLngLat> convertAddrList = new List<AddressConvert.DLngLat>(); //存放需要解析地址的轨迹点的坐标
                    List<int> ConvertIndexList = new List<int>(); //ConvertIndexList存放需要解析坐标的轨迹点的索引
                    int rowcount = dt.Rows.Count;
                    double LngDiff = double.Parse(ConfigurationManager.AppSettings["DiffLng"]);
                    double LatDiff = double.Parse(ConfigurationManager.AppSettings["DiffLat"]);

                    //遍历DataTable，将需要解析地址的坐标，存放到convertAddrList列表中
                    for (int i = 0; i < rowcount; i++)
                    {
                        int l = convertAddrList.Count;
                        if ((i == 0) || (i > 0 && (Math.Abs(double.Parse(dt.Rows[i]["lng"].ToString()) - convertAddrList[l - 1].Lng) > LngDiff || Math.Abs(double.Parse(dt.Rows[i]["lat"].ToString()) - convertAddrList[l - 1].Lat) > LatDiff)))
                        {

                            AddressConvert.DLngLat dlnglat = new AddressConvert.DLngLat();
                            ConvertIndexList.Add(i);
                            dlnglat.Lng = double.Parse(dt.Rows[i]["lng"].ToString());
                            dlnglat.Lat = double.Parse(dt.Rows[i]["lat"].ToString());
                            convertAddrList.Add(dlnglat);
                        }
                    }

                    string[] conArr =CommLibrary.AddressConvert.AddConvertBatch(convertAddrList);

                    if (conArr.Length == convertAddrList.Count)
                    {
                        for (int i = 0; i < rowcount; i++)
                        {
                            int arrIndex = GetArrIndex(i, ConvertIndexList);
                            if (arrIndex > -1)
                            {
                                addrDt.Rows[i]["add"] = conArr[arrIndex];
                            }
                            else
                            {
                                addrDt.Rows[i]["add"] = addrDt.Rows[i - 1]["add"];
                            }
                        }
                    }

                    return addrDt;
             }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private int GetArrIndex(int i, List<int> ConvertIndexList)
        {
            for (int j = 0; j < ConvertIndexList.Count; j++)
            {
                if (i == ConvertIndexList[j])
                {
                    return j;
                }
            }
            return -1;
        }

        #endregion
    }
}