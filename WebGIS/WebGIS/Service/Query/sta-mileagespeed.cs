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
    public class sta_mileagespeed
    {
        /// <summary>
        /// 查询车辆超速信息
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMileageSpeed(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string ispeed;
            string carnum;
            string cuid;
            string carownname;
            string line;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("ispeed") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
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
                if (inparams["ispeed"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "限制速度不能为空", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                sysflag = inparams["sysflag"];
                ispeed = inparams["ispeed"];
                carnum = inparams["carnum"];
                cuid = inparams["cuid"];
                carownname = inparams["carownname"];
                line = inparams["line"];
                sysuid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询车辆超速信息
                //todo
                DataTable dt = GetMileageSpeedByIds(sysflag, sysuid, stime, etime, ispeed, carnum, cuid, carownname, line, onecaruser);
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
        /// 查询车辆超速信息详细
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMileageSpeedoutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string ispeed;
            string carnum;
            string cuid;
            string carownname;
            string line;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("ispeed") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
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
                if (inparams["ispeed"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "限制速度不能为空", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                sysflag = inparams["sysflag"];
                ispeed = inparams["ispeed"];
                carnum = inparams["carnum"];
                cuid = inparams["cuid"];
                carownname = inparams["carownname"];
                line = inparams["line"];
                sysuid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询车辆超速信息详细
                //todo
                DataTable contentData = GetMileageSpeedByIds(sysflag, sysuid, stime, etime, ispeed, carnum, cuid, carownname, line, onecaruser);
                contentData.Columns.Remove("c_ID");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "开始时间", "结束时间", "超速时间", "超速次数" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "超速统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        /// 调用存储过程超速车辆列表
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <param name="ispeed">限速</param>
        /// <returns></returns>
        public static DataTable GetMileageSpeedByIds(string sysflag, string uid, string stime, string etime, string ispeed, string carnum, string cuid, string carownname, string line,string onecaruser)
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
            SqlParameter[] Parameters = { new SqlParameter("@strTime", stime), new SqlParameter("@endTime", etime), new SqlParameter("@iSpeed", ispeed), new SqlParameter("@CarNum", carnum), new SqlParameter("@CUID", cuid), new SqlParameter("@CarOwnName", carownname), new SqlParameter("@Line", line), new SqlParameter("@UID", uid), new SqlParameter("@OneCarUser", onecaruser) };
                    //DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_SpeedAllSearch"), Parameters).Tables[0];
                    return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_SpeedAllSearchTest"), Parameters,null,3600).Tables[0];
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
        /// 查询车辆超速详细信息
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMileageSpeedInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string cid;
            string sysflag;
            string ispeed;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("ispeed"))
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
                if (inparams["ispeed"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "限制速度不能为空", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"].ToString();
                sysflag = inparams["sysflag"];
                ispeed = inparams["ispeed"].ToString();

                //调用存储过程查询车辆超速详细信息
                //todo
                DataTable dt = GetMileageSpeedInfoById(sysflag, cid, stime, etime, ispeed);
                //for (int i = 1; i <= dt.Rows.Count; i++)
                //{
                //    dt.Rows[i - 1]["No_ID"] = i;
                //}

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
        /// 查询车辆超速详细信息导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMileageSpeedInfoout(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string cid;
            string sysflag;
            string ispeed;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("ispeed"))
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
                if (inparams["ispeed"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "限制速度不能为空", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"].ToString();
                sysflag = inparams["sysflag"];
                ispeed = inparams["ispeed"].ToString();
                //调用存储过程查询车辆超速详细信息
                //todo
                DataTable dt = GetMileageSpeedInfoById(sysflag, cid, stime, etime, ispeed);
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "起始时间", "结束时间", "最快速度", "超速时间" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                npoiHelper.WorkbookName = "超速统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        /// 调用存储过程查询车辆超速详细信息
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <param name="ispeed">超速速度</param>
        /// <returns></returns>
        public static DataTable GetMileageSpeedInfoById(string sysflag, string cid, string stime, string etime, string ispeed)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@c_ID", cid), new SqlParameter("@startTime", stime), new SqlParameter("@endTime", etime), new SqlParameter("@iSpeed", ispeed) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_SpeedEverySearch"), Parameters, null, 300).Tables[0];

        }

    }



}