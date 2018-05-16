using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebGIS;
using CommLibrary;
using System.Data.SqlClient;
using System.Web;
using System.Reflection;
using System.Collections;

namespace SysService
{
    public class sta_signalStat
    {
        public ResponseResult getSignalStatistic(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string CarNum;
            string CUID;
            string CarOwnName;
            string Line;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["etime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                CarNum = inparams["carnum"];
                CUID = inparams["cuid"];
                CarOwnName = inparams["carownname"];
                Line = inparams["line"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询统计信号量
                DataTable dt = GetSignalStatistics(sysflag, sysuid, stime, etime, CarNum, CUID, CarOwnName, Line, onecaruser);
                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                res.isallresults = 0;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        public ResponseResult getSignalStatisticsoutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string CarNum;
            string CUID;
            string CarOwnName;
            string Line;
            string sysuid;
            string sysflag;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["etime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                CarNum = inparams["carnum"];
                CUID = inparams["cuid"];
                CarOwnName = inparams["carownname"];
                Line = inparams["line"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                

                DataTable contentData = GetSignalStatistics(sysflag, sysuid, stime, etime, CarNum, CUID, CarOwnName, Line, onecaruser);
                contentData.Columns.Remove("CID");


                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "所属企业", "速度次数", "点火开关次数", "制动信号次数", "左转向灯次数", "右转向灯次数", "远光灯次数", "近光灯次数" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "信号量统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        public static DataTable GetSignalStatistics(string sysflag, string uid, string stime, string etime, string carnum, string cuid, string carownname, string line, string onecaruser)
        {
            int IsUnion = 0;
            DateTime et = DateTime.Parse(etime).Date;
            DateTime today = DateTime.Today;
            if ((today - et).Days == 0)  //结束时间是今天，则查询需要从作业表和轨迹表取数据
            {
                IsUnion = 1;
            }


            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@UID", uid), 
                                            new SqlParameter("@T_TimeBegin", stime), 
                                            new SqlParameter("@T_TimeEnd", etime), 
                                            new SqlParameter("@CarNum", carnum), 
                                            new SqlParameter("@CUID", cuid), 
                                            new SqlParameter("@CarOwnName", carownname), 
                                            new SqlParameter("@Line", line), 
                                            new SqlParameter("@OneCarUser", onecaruser),
                                            new SqlParameter("@IsUnion", IsUnion)
                                            
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWCProc_IC_SignalStatRA"), Parameters, null, 3600).Tables[0];

        }

        public ResponseResult getSignalInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            int cid;
            string sysflag;
            int signaltype;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("signaltype"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["etime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["cid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                if (inparams["signaltype"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "信号量类型错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = int.Parse(inparams["cid"].ToString());
                sysflag = inparams["sysflag"];
                signaltype = int.Parse(inparams["signaltype"].ToString());

                //调用存储过程查询信号量详细信息
                DataTable dt = GetSignalInfoById(sysflag, cid, stime, etime, signaltype);

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

        public ResponseResult getSignalInfoStatisticsoutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            int cid;
            string sysflag;
            int signaltype;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("signaltype"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["etime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = int.Parse(inparams["cid"].ToString());
                sysflag = inparams["sysflag"];
                signaltype = int.Parse(inparams["signaltype"].ToString());

                //调用存储过程查询信号量详细信息
                DataTable contentData = GetSignalInfoById(sysflag, cid, stime, etime, signaltype); ;
                contentData.Columns.Remove("CID");


                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "所属企业", "GPS时间", "信号名称", "速度值" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "信号量统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        private DataTable GetSignalInfoById(string sysflag, int cid, string stime, string etime, int signaltype)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid), 
                                            new SqlParameter("@T_TimeBegin", stime), 
                                            new SqlParameter("@T_TimeEnd", etime), 
                                            new SqlParameter("@SignalType", signaltype)
                                            
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWCProc_IC_SignalStatDetailRA"), Parameters, null, 3600).Tables[0];
        }


    }
}