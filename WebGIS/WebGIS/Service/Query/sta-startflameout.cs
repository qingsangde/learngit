﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebGIS;
using CommLibrary;
using System.Data.SqlClient;
using System.Web;


namespace SysService
{
    public class sta_startflameout
    {
        /// <summary>
        /// 查询车辆启动熄火数据列表
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getSFCar(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string cid;
            string sysflag;
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
                if (inparams["cid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                //if (inparams["sysflag"] == "")
                //{
                //    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                //    return Result;
                //}
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"];
                sysflag = inparams["sysflag"];
                //调用存储过程查询车辆启动熄火数据
                //todo
                DataTable dt = GetStatusCars(sysflag, cid, stime, etime);


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
        /// 调用存储过程获取启动熄火车辆列表
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetStatusCars(string sysflag, string cid, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@CARID", int.Parse(cid)), new SqlParameter("@BTIME", stime), new SqlParameter("@ETIME", etime) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QP_ParkingInfo"), Parameters, null, 1800).Tables[0];

        }

        /// <summary>
        /// 启动熄火车辆列表导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getSFCaroutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string cid;
            string sysflag;
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
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"];
                sysflag = inparams["sysflag"];

                //调用存储过程查询启动熄火车辆列表
                //todo
                DataTable contentData = GetStatusCars(sysflag, cid, stime, etime);
            
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "状态", "时间", "位置", "停车（运行）时长" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "启动熄火统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath =  @"UI\Excel\Query\" ;
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                return Result;
            }
            catch(Exception ex)
            {
                Result = new ResponseResult(ResState.OtherError, "", ex.Message + ex.StackTrace);
                return Result;
            }
        }
    }
}