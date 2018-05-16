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
    public class sta_ComTestDriveAnalyse
    {
        /// <summary>
        /// 查询车辆启动熄火数据列表
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getSFCar(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            
            string sysflag;

            string stime;
            string etime;
            string area;
            string province;

            string dealername;
            string dealercode;
            string cartype;
            string carno;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag"))
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
              
                sysflag = inparams["sysflag"];

                stime = inparams["stime"];
                etime = inparams["etime"];
                area = inparams["area"];
                province = inparams["province"];

                dealername = inparams["dealername"];
                dealercode = inparams["dealercode"];
                cartype = inparams["cartype"];
                carno = inparams["carno"];



                //调用存储过程查询车辆启动熄火数据
                //todo
                DataTable dt = GetStatusCars(sysflag, stime, etime, area, province, dealername, dealercode, cartype, carno);


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
        public static DataTable GetStatusCars(string sysflag, string stime, string etime, string area, string province,
            string dealername, string dealercode, string cartype, string carno
            )
        {


        

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                          new SqlParameter("@stime", stime), 
                                          new SqlParameter("@etime", etime), 
                                          new SqlParameter("@area", area), 
                                          new SqlParameter("@province", province), 
                                          new SqlParameter("@dealername", dealername), 
                                          new SqlParameter("@dealercode", dealercode), 
                                          new SqlParameter("@cartype", cartype), 
                                          new SqlParameter("@carno", carno)
                                        



                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("X80Proc_ColligateAnalyse"), Parameters, null, 1800).Tables[0];

        }

        /// <summary>
        /// 启动熄火车辆列表导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getSFCaroutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;

            string stime;
            string etime;
            string area;
            string province;

            string dealername;
            string dealercode;
            string cartype;
            string carno;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") ||  !inparams.Keys.Contains("sysflag"))
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

                sysflag = inparams["sysflag"];

                stime = inparams["stime"];
                etime = inparams["etime"];
                area = inparams["area"];
                province = inparams["province"];

                dealername = inparams["dealername"];
                dealercode = inparams["dealercode"];
                cartype = inparams["cartype"];
                carno = inparams["carno"];

                //调用存储过程查询启动熄火车辆列表
                //todo
                DataTable contentData = GetStatusCars(sysflag, stime, etime, area, province, dealername, dealercode, cartype, carno);

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "销售大区", "销售省区", "经销商代码", "经销商名称", "车型", "车牌号" ,
                                           
                                           "试乘试驾次数", "试乘试驾里程", "总行驶里程", "试乘试驾里程占比", "试乘试驾时间", "总行驶时间" ,

                                           "试乘试驾时间占比", "驶出活动范围次数", "驶出活动范围里程", "驶出活动范围时间", "驶出活动范围里程占比", "驶出活动范围时间占比","ID" 
                                           };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "试乘试驾综合分析表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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