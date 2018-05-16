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
    public class sta_caronline
    {
        /// <summary>
        /// 获取未上线车辆列表
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getCarOnLine(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string uid;
            string datime;
            string sysflag;
            string onecaruser;
            if (!inparams.Keys.Contains("uid") || !inparams.Keys.Contains("datime") )
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["uid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "用户错误", null);
                    return Result;
                }
                if (inparams["datime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "车辆未在线天数错误", null);
                    return Result;
                }
                uid = inparams["uid"];
                datime = inparams["datime"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询车辆数据
                //todo
                DataTable dt = GetCarOnLine(sysflag, uid, datime,onecaruser);
                //for (int i = 1; i <= dt.Rows.Count; i++)
                //{
                //    dt.Rows[i - 1]["NO"] = i;
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
        /// 调用存储过程查询未上线车辆列表
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="uid">用户ID</param>
        /// <param name="datime">车辆未在线天数</param>
        /// <returns></returns>
        public static DataTable GetCarOnLine(string sysflag, string uid, string datime, string onecaruser)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@UserName", uid), new SqlParameter("@NotOnLine", datime), new SqlParameter("@OneCarUser",onecaruser) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QC_CarOnLine"), Parameters,"onlinetbl",600).Tables[0];

        }

        /// <summary>
        /// 获取未上线车辆列表导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getCarOnLineOut(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string uid;
            string datime;
            string sysflag;
            string onecaruser;
            if (!inparams.Keys.Contains("uid") || !inparams.Keys.Contains("datime"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["uid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "未获取到用户", null);
                    return Result;
                }
                if (inparams["datime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "未在线天数不能为空", null);
                    return Result;
                }
                uid = inparams["uid"];
                datime = inparams["datime"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询车辆数据
                //todo
                DataTable contentData = GetCarOnLine(sysflag, uid, datime,onecaruser);
                //for (int i = 1; i <= contentData.Rows.Count; i++)
                //{
                //    contentData.Rows[i - 1]["NO"] = i;
                //}
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "GPS时间", "未在线天数" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "未上线车辆信息统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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