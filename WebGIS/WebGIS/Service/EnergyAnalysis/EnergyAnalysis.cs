using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebGIS;
using CommLibrary;
using System.Data.SqlClient;
using System.Web;

namespace SysService
{
    public class EnergyAnalysis
    {
        public ResponseResult QueryList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string CarNo;
            string CarOwnName;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("CarNo") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("CarOwnName"))
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
                CarNo = inparams["CarNo"];
                CarOwnName = inparams["CarOwnName"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询能耗分析总表
                //todo
                DataTable dt = EnergyAnalysisList(sysflag, sysuid, stime, etime, CarNo, CarOwnName, onecaruser);
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

        public ResponseResult ListExport(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string CarNo;
            string CarOwnName;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("CarNo") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("CarOwnName"))
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
                CarNo = inparams["CarNo"];
                CarOwnName = inparams["CarOwnName"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询能耗分析总表
                //todo
                DataTable contentData = EnergyAnalysisList(sysflag, sysuid, stime, etime, CarNo, CarOwnName, onecaruser);
                contentData.Columns.Remove("CID");
                // 添加列
                contentData.Columns.Add("DurationStr");
                // 处理时间，转换为“时、分、秒”
                foreach (DataRow row in contentData.Rows)
                {
                    if ((int)row["SumDuration"] != 0)
                    {
                        row["DurationStr"] = AnalysisDuration((long)row["SumDuration"]);
                    }
                    else
                    {
                        row["DurationStr"] = "0";
                    }
                }

                contentData.Columns.Remove("SumDuration");


                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "开始时间", "结束时间", "行驶里程", "油耗", "平均油耗", "尿素消耗", "平均尿素消耗", "最高速度", "平均速度", "停车次数","行驶总时长" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "能耗分析总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\";
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                return Result;
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        private DataTable EnergyAnalysisList(string sysflag, string sysuid, string stime, string etime, string CarNo, string CarOwnName, string onecaruser)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@UID", sysuid), 
                                            new SqlParameter("@StartTime", stime), 
                                            new SqlParameter("@EndTime", etime), 
                                            new SqlParameter("@CarNo", CarNo),
                                            new SqlParameter("@CarOwnName", CarOwnName),
                                            new SqlParameter("@OneCarUser", onecaruser) 
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("LiuTe_EnergyAnalysis_QueryList"), Parameters, null, 3600).Tables[0];
        }

        public ResponseResult QueryDetail(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string sysuid;
            string cid;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag"))
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
                cid = inparams["cid"];
                sysflag = inparams["sysflag"];
                //调用存储过程查询能耗分析明细表
                //todo
                DataTable dt = EnergyAnalysisDetail(sysflag, cid, stime, etime);
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

        public ResponseResult DetailExport(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string sysuid;
            string cid;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag"))
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
                cid = inparams["cid"];
                sysflag = inparams["sysflag"];
                //调用存储过程查询能耗分析明细表
                //todo
                DataTable contentData = EnergyAnalysisDetail(sysflag, cid, stime, etime);
                contentData.Columns.Remove("CID");
                contentData.Columns.Add("StartAddress");
                contentData.Columns.Add("EndAddress");
                // 添加列
                contentData.Columns.Add("DurationStr");
                // 处理时间，转换为“时、分、秒”
                foreach (DataRow row in contentData.Rows)
                {
                    if ((int)row["Duration"] != 0)
                    {
                        row["DurationStr"] = AnalysisDuration((long)row["Duration"]);
                    }
                    else
                    {
                        row["DurationStr"] = "0";
                    }
                }

                contentData.Columns.Remove("Duration");

                List<CommLibrary.AddressConvert.DLngLat> corrds = new List<AddressConvert.DLngLat>();
                for (int i = 0; i < contentData.Rows.Count; i++)
                {
                    string sc = contentData.Rows[i]["FromLatLng"].ToString();
                    sc = string.IsNullOrEmpty(sc) ? "0,0" : sc;
                    string slng = sc.Split(',')[1];
                    string slat = sc.Split(',')[0];
                    string ec = contentData.Rows[i]["ToLatLng"].ToString();
                    ec = string.IsNullOrEmpty(ec) ? "0,0" : ec;
                    string elng = ec.Split(',')[1];
                    string elat = ec.Split(',')[0];
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
                string[] headerDataArray = { "车牌号", "开始时间", "结束时间", "开始位置坐标", "结束位置坐标", "行驶里程", "油耗", "尿素消耗", "平均速度", "开始地址", "结束地址", "行驶时长" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "能耗分析明细表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\";
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                return Result;
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        private DataTable EnergyAnalysisDetail(string sysflag, string cid, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid), 
                                            new SqlParameter("@StartTime", stime), 
                                            new SqlParameter("@EndTime", etime)
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("LiuTe_EnergyAnalysis_QueryDetail"), Parameters, null, 3600).Tables[0];
        }


        private string AnalysisDuration(long durationSum)
        {
            if (durationSum != 0)
            {
                long hour = 0;
                long minute = 0;
                long second = 0;
                hour = durationSum / 3600;
                minute = durationSum % 3600 / 60;
                second = durationSum % 60;
                return hour + "小时" + minute + "分" + second + "秒";
            }
            else return "0";
        }
    }
}