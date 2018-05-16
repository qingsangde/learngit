using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CommLibrary;
using System.Text;
using System.Data.SqlClient;

namespace WebGIS.Service
{
    public class AnnulLog   
    {
        #region 定义
        /// <summary>
        /// 经销商角色
        /// </summary>
        private const string R_JXS = "25";

        /// <summary>
        /// 启明角色
        /// </summary>
        private const string R_QM = "26"; 
        #endregion 定义

        #region 初始化
        #endregion 初始化
        #region 事件

        /// <summary>
        /// 系统日志列表查询事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getAnnulLogList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string lockStatusCd;
            string userrole;
            string sysuid;    
            string carNo;
            string orgNo;
            string applyId;
            string applyReason;
            string applyTimeStart;
            string applyTimeEnd;
            string approveId;
            string approveTimeStart;
            string approveTimeEnd;
            string lockWayCd;

            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("userrole") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("lockStatusCd") ||
                !inparams.Keys.Contains("carNo") ||!inparams.Keys.Contains("orgNo") || !inparams.Keys.Contains("applyId") || !inparams.Keys.Contains("applyReason") ||
                !inparams.Keys.Contains("applyTimeStart") || !inparams.Keys.Contains("applyTimeEnd") || !inparams.Keys.Contains("approveId") ||
                !inparams.Keys.Contains("approveTimeStart") || !inparams.Keys.Contains("approveTimeEnd") || !inparams.Keys.Contains("lockWayCd"))
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
                carNo = inparams["carNo"];
                lockStatusCd = inparams["lockStatusCd"];
                carNo = inparams["carNo"];
                orgNo = inparams["orgNo"];
                applyId = inparams["applyId"];
                applyReason = inparams["applyReason"];
                applyTimeStart = inparams["applyTimeStart"];
                applyTimeEnd = inparams["applyTimeEnd"];
                approveId = inparams["approveId"];
                approveTimeStart = inparams["approveTimeStart"];
                approveTimeEnd = inparams["approveTimeEnd"];
                lockWayCd = inparams["lockWayCd"];
                sysflag = inparams["sysflag"];
                userrole = inparams["userrole"];
                sysuid = inparams["sysuid"];

                //调用方法取得数据表格
                DataTable dt = GetSelectData( sysflag,userrole, sysuid,  lockStatusCd,  carNo,  orgNo,  applyId,  applyReason,  applyTimeStart,
             applyTimeEnd,  approveId,  approveTimeStart,  approveTimeEnd,  lockWayCd);

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

     
        /// <summary>
        /// 初始化时生成下拉框
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getAllCmbOnLoad(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid"))
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

                sysflag = inparams["sysflag"];

                //调用方法取得下拉框数据表格
                DataTable[] dtArr = GetAllCmb(sysflag);
                int Total = dtArr.Length;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dtArr;
                res.isallresults = 0;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        /// <summary>
        /// 导出事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult doExport(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string lockStatusCd;
            string userrole;
            string sysuid;
            string carNo;
            string orgNo;
            string applyId;
            string applyReason;
            string applyTimeStart;
            string applyTimeEnd;
            string approveId;
            string approveTimeStart;
            string approveTimeEnd;
            string lockWayCd;

            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("userrole") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("lockStatusCd") ||
                !inparams.Keys.Contains("carNo") || !inparams.Keys.Contains("orgNo") || !inparams.Keys.Contains("applyId") || !inparams.Keys.Contains("applyReason") ||
                !inparams.Keys.Contains("applyTimeStart") || !inparams.Keys.Contains("applyTimeEnd") || !inparams.Keys.Contains("approveId") ||
                !inparams.Keys.Contains("approveTimeStart") || !inparams.Keys.Contains("approveTimeEnd") || !inparams.Keys.Contains("lockWayCd"))
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
                carNo = inparams["carNo"];
                lockStatusCd = inparams["lockStatusCd"];
                carNo = inparams["carNo"];
                orgNo = inparams["orgNo"];
                applyId = inparams["applyId"];
                applyReason = inparams["applyReason"];
                applyTimeStart = inparams["applyTimeStart"];
                applyTimeEnd = inparams["applyTimeEnd"];
                approveId = inparams["approveId"];
                approveTimeStart = inparams["approveTimeStart"];
                approveTimeEnd = inparams["approveTimeEnd"];
                lockWayCd = inparams["lockWayCd"];
                sysflag = inparams["sysflag"];
                userrole = inparams["userrole"];
                sysuid = inparams["sysuid"];

                //调用方法取得数据表格
                DataTable contentData = GetSelectData( sysflag, userrole,sysuid, lockStatusCd,  carNo,  orgNo,  applyId,  applyReason,  applyTimeStart,
             applyTimeEnd,  approveId,  approveTimeStart,  approveTimeEnd,  lockWayCd);

                //页面没有数据的情况，提示信息
                if (contentData.Rows.Count == 0)
                {
                    Result = new ResponseResult(ResState.OperationFailed, "未检索到数据,导出失败！", "");
                }
                else
                {
                    //移除车辆ID列和锁车状态code列
                    contentData.Columns.Remove("OrgNo");
                    contentData.Columns.Remove("LockWayCd");
                    contentData.Columns.Remove("LockStatusCd");
                    contentData.Columns.Remove("ApplyReasonCd");
                    NPOIHelper npoiHelper = new NPOIHelper();
                    string[] headerDataArray = { "车牌号", "放款机构", "申请人", "申请时间", "申请内容", "审批人" ,
                                           
                                                "审批时间", "锁车状态","锁车途径" ,"操作人员","操作时间"
                                           };
                    string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                    npoiHelper.WorkbookName = "系统日志表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    // 设置导入内容
                    npoiHelper.HeaderData = headerDataArray;
                    npoiHelper.ContentData = contentDataArray;
                    string basepath = HttpRuntime.AppDomainAppPath.ToString();
                    string filePath = @"UI\Excel\Query\";
                    string sd = basepath + filePath;
                    npoiHelper.saveExcel(sd);

                    Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                }
                return Result;
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OtherError, "", ex.Message + ex.StackTrace);
                return Result;
            }
        }



        #endregion 事件

        #region 方法

        /// <summary>
        /// 取得页面下拉框数据表格
        /// </summary>
        /// <param name="sysflag">sysflag</param>
        /// <returns>DataTable</returns>
        private DataTable[] GetAllCmb(string sysflag)
        {
            DataTable[] dtArr = new DataTable[4];

            try
            {
                ComSqlHelper csh = new ComSqlHelper();

                //调用存储过程取得DataSet
                DataSet ds = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_GetLogCmb"), null, null, 3600);
                if (ds.Tables.Count == 4)
                {
                    dtArr[0] = ds.Tables[0];
                    dtArr[1] = ds.Tables[1];
                    dtArr[2] = ds.Tables[2];
                    dtArr[3] = ds.Tables[3];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtArr;
        }

        /// <summary>
        /// 查询/导出方法
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="stime"></param>
        /// <param name="area"></param>
        /// <param name="province"></param>
        /// <param name="dealername"></param>
        /// <param name="dealercode"></param>
        /// <param name="cartype"></param>
        /// <param name="carno"></param>
        /// <returns></returns>
        public static DataTable GetSelectData(string sysflag, string userrole, string sysuid, string lockStatusCd, string carNo, string orgNo, string applyId, string applyReason, string applyTimeStart,
            string applyTimeEnd, string approveId, string approveTimeStart, string approveTimeEnd, string lockWayCd)
        {
            ComSqlHelper csh = new ComSqlHelper();
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();

            //用户角色是经销商的情况
            if (R_JXS.Equals(userrole))
            {
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@sysuid", sysuid)
                                        };
                dt = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_QuerySysLogForJXS"), Parameters, null, 1800).Tables[0];
            }
            else
            {
                dt = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_QuerySysLogForQM"), null, null, 1800).Tables[0];
            }

            //锁车状态
            if (!string.IsNullOrEmpty(lockStatusCd))
            {
                //为-1的情况
                if ("-1".Equals(lockStatusCd.Trim()))
                {
                    sb.Append( " AND LockStatusCd  NOT IN ('0','1','2','3')");
                }
                else
                {
                    sb.Append(  " AND LockStatusCd =  '" + lockStatusCd.Trim() + "'");
                }
            }

            //车牌号
            if (!string.IsNullOrEmpty(carNo))
            {
                {
                    sb.Append( " AND CarNumber like '%" + carNo.Trim() + "%'");
                }
            }

            //放款机构
            if (!string.IsNullOrEmpty(orgNo))
            {
                {
                    sb.Append( " AND OrgNo =  '" + orgNo.Trim() + "'");
                }
            }

            //申请内容
            if (!string.IsNullOrEmpty(applyReason))
            {
                {
                    sb.Append(" AND ApplyReasonCd = '" + applyReason.Trim() + "'");
                }
            }

            //申请开始时间
            if (!string.IsNullOrEmpty(applyTimeStart))
            {
                sb.Append(" AND ApplyTime  >= '" + Convert.ToDateTime(applyTimeStart).ToString("yyyy-MM-dd") + " 00:00:00'");
            }

            //申请结束时间
            if (!string.IsNullOrEmpty(applyTimeEnd))
            {
                sb.Append(" AND ApplyTime  <= '" + Convert.ToDateTime(applyTimeEnd).ToString("yyyy-MM-dd") + " 23:59:59'");
            }

            //锁车途径
            if (!string.IsNullOrEmpty(lockWayCd))
            {
                {
                    sb.Append( " AND LockWayCd =  '" + lockWayCd.Trim() + "'");
                }
            }

            //审批开始时间
            if (!string.IsNullOrEmpty(approveTimeStart))
            {
                sb.Append(" AND ApproveTime  >= '" + Convert.ToDateTime(approveTimeStart).ToString("yyyy-MM-dd") + " 00:00:00'");
            }

            //审批结束时间
            if (!string.IsNullOrEmpty(approveTimeEnd))
            {
                sb.Append(" AND ApproveTime  <= '" + Convert.ToDateTime(approveTimeEnd).ToString("yyyy-MM-dd") + " 23:59:59'");
            }

            //申请ID
            if (!string.IsNullOrEmpty(applyId))
            {
                {
                    sb.Append( " AND ApplyId like  '%" + applyId.Trim() + "%'");
                }
            }

            //审批ID
            if (!string.IsNullOrEmpty(approveId))
            {
                {
                    sb.Append(" AND ApproveId like  '%" + approveId.Trim() + "%'");
                }
            }

            if (sb.Length != 0)
            {
                sb.Remove(0, 4);
                DataView dv = dt.AsDataView();
                dv.RowFilter = sb.ToString();
                dt = dv.ToTable();
            }
            return dt;
        }

        #endregion 方法

    }
}