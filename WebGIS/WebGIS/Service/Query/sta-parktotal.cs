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
    public class sta_parktotal
    {
        /// <summary>
        /// 查询停车信息
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getParkTotal(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string stoptime;
            string carnum;
            string cuid;
            string carownname;
            string line;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("stoptime") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
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
                if (inparams["stoptime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "停车时间不能为空", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                sysflag = inparams["sysflag"];
                stoptime = inparams["stoptime"];
                carnum = inparams["carnum"];
                cuid = inparams["cuid"];
                carownname = inparams["carownname"];
                line = inparams["line"];
                sysuid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询停车信息
                //todo
                DataTable dt = GetParkTotalByIds(sysflag, sysuid, stime, etime, stoptime, carnum, cuid, carownname, line,onecaruser);
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
        /// 停车信息导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getParkTotaloutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string stoptime;
            string carnum;
            string cuid;
            string carownname;
            string line;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("stoptime") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
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
                if (inparams["stoptime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "停车时间不能为空", null);
                    return Result;
                }

                stime = inparams["stime"];
                etime = inparams["etime"];
                sysflag = inparams["sysflag"];
                stoptime = inparams["stoptime"];
                carnum = inparams["carnum"];
                cuid = inparams["cuid"];
                carownname = inparams["carownname"];
                line = inparams["line"];
                sysuid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询停车信息
                //todo
                DataTable dt = GetParkTotalByIds(sysflag, sysuid, stime, etime, stoptime, carnum, cuid, carownname, line,onecaruser);
                //for (int i = 1; i <= dt.Rows.Count; i++)
                //{
                //    dt.Rows[i - 1]["No_ID"] = i;
                //}
                dt.Columns.Remove("c_ID");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "开始时间", "结束时间", "停车时长", "停车次数" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                npoiHelper.WorkbookName = "停车统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        /// <summary>
        /// 调用存储过程查询停车信息
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <param name="stoptime">停车时长</param>
        /// <returns></returns>
        public static DataTable GetParkTotalByIds(string sysflag, string uid, string stime, string etime, string stoptime, string carnum, string cuid, string carownname, string line, string onecaruser)
        {
            ComSqlHelper csh = new ComSqlHelper();
            //int fgcd = 500;
            //cid = cid.TrimEnd(',');
            //string[] cidarray = cid.Split(',');
            //string subcids = "";
            //int count = 0;
            //DataTable resdt = null;
            //for (int i = 0; i < cidarray.Length; i++)
            //{
            //    subcids += cidarray[i] + ",";
            //    count++;
            //    if (count >= fgcd || i == cidarray.Length - 1)
            //    {
            //        //subcids = subcids.TrimEnd(',');
            SqlParameter[] Parameters = { new SqlParameter("@StartTime", stime), new SqlParameter("@ToTime", etime), new SqlParameter("@StopTime", stoptime), new SqlParameter("@CarNum", carnum), new SqlParameter("@CUID", cuid), new SqlParameter("@CarOwnName", carownname), new SqlParameter("@Line", line), new SqlParameter("@UID", uid), new SqlParameter("@OneCarUser", onecaruser) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_GetParkTotalInFoTest"), Parameters,null,7200).Tables[0];
            //        DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_GetParkTotalInFo"), Parameters).Tables[0];
            //        if (resdt == null)
            //        {
            //            resdt = tempdt.Copy();
            //        }
            //        else
            //        {
            //            //foreach (DataRow dr in tempdt.Rows)
            //            //{
            //            //    resdt.ImportRow(dr);
            //            //}
            //            resdt.Merge(tempdt);
            //        }
            //        count = 0;
            //        subcids = "";
            //    }
            //}

            //for (int i = 1; i <= resdt.Rows.Count; i++)
            //{
            //    resdt.Rows[i - 1]["AutoID"] = i;
            //}
            //return resdt;
            
        }


        /// <summary>
        /// 查询停车信息明细
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getParkTotalInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string cid;
            string sysflag;
            string stoptime;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("stoptime"))
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
                if (inparams["stoptime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "停车时间不能为空", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"].ToString();
                sysflag = inparams["sysflag"];
                stoptime = inparams["stoptime"];
                
                //调用存储过程查询停车明细数据
                //todo
                DataTable dt = GetParkTotalInfoById(sysflag, cid, stime, etime, stoptime);

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
        /// 调用存储过程查询停车明细数据
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <param name="stoptime">停车时长</param>
        /// <returns></returns>
        public static DataTable GetParkTotalInfoById(string sysflag, string cid, string stime, string etime, string stoptime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@C_NO", cid), new SqlParameter("@StartTime", stime), new SqlParameter("@ToTime", etime), new SqlParameter("@StopTime", stoptime) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_GetParkDetailsInFo"), Parameters,null,300).Tables[0];

        }

        /// <summary>
        /// 停车明细导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getParkTotalinfooutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string cid;
            string sysflag;
            string stoptime;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("stoptime"))
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
                if (inparams["stoptime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "停车时间不能为空", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"].ToString();
                sysflag = inparams["sysflag"];

                stoptime = inparams["stoptime"];
                //调用存储过程查询停车明细
                //todo
                DataTable contentData = GetParkTotalInfoById(sysflag, cid, stime, etime, stoptime);

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "开始时间", "结束时间", "停车时长", "位置(纬度-经度)" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "停车统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

    }



}