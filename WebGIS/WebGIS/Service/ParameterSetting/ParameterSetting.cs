using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;
using System.Data;
using CommLibrary;
using System.Data.SqlClient;
using System.Text;

namespace SysService
{
    public class ParameterSetting
    {
        #region 接口

        #region 查询一览数据
        /// <summary>
        /// 查询一览数据
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>一览数据</returns>
        public ResponseResult conditionSearch(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag = string.Empty;
            string sysuid = string.Empty;
            string rid = string.Empty;
            string ConditionCarNo = string.Empty;
            string ConditionDPH = string.Empty;
            string ConditionSimCode = string.Empty;
            string ConditionTNO = string.Empty;
            string ConditionOrgNo = string.Empty;
            string ConditionLockStatus = string.Empty;
            string ConditionLendDateFrom = string.Empty;
            string ConditionLendDateTo = string.Empty;
            string ConditionServiceEDayFrom = string.Empty;
            string ConditionServiceEDayTo = string.Empty;
            string ConditionLeftDays = string.Empty;
            string ConditionPayStatus = string.Empty;

            if (!inparams.Keys.Contains("sid") ||
                !inparams.Keys.Contains("sysuid") ||
                !inparams.Keys.Contains("token") ||
                !inparams.Keys.Contains("sysflag") ||
                !inparams.Keys.Contains("rid") ||
                !inparams.Keys.Contains("ConditionCarNo") ||
                !inparams.Keys.Contains("ConditionDPH") ||
                !inparams.Keys.Contains("ConditionSimCode") ||
                !inparams.Keys.Contains("ConditionTNO") ||
                !inparams.Keys.Contains("ConditionOrgNo") ||
                !inparams.Keys.Contains("ConditionLockStatus") ||
                !inparams.Keys.Contains("ConditionLendDateFrom") ||
                !inparams.Keys.Contains("ConditionLendDateTo") ||
                !inparams.Keys.Contains("ConditionServiceEDayFrom") ||
                !inparams.Keys.Contains("ConditionServiceEDayTo") ||
                !inparams.Keys.Contains("ConditionLeftDays") ||
                !inparams.Keys.Contains("ConditionPayStatus"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                sysflag = inparams["sysflag"];
                sysuid = inparams["sysuid"];
                rid = inparams["rid"];
                ConditionCarNo = inparams["ConditionCarNo"];
                ConditionDPH = inparams["ConditionDPH"];
                ConditionSimCode = inparams["ConditionSimCode"];
                ConditionTNO = inparams["ConditionTNO"];
                ConditionOrgNo = inparams["ConditionOrgNo"];
                ConditionLockStatus = inparams["ConditionLockStatus"];
                ConditionLendDateFrom = inparams["ConditionLendDateFrom"];
                ConditionLendDateTo = inparams["ConditionLendDateTo"];
                ConditionServiceEDayFrom = inparams["ConditionServiceEDayFrom"];
                ConditionServiceEDayTo = inparams["ConditionServiceEDayTo"];
                ConditionLeftDays = inparams["ConditionLeftDays"];
                ConditionPayStatus = inparams["ConditionPayStatus"];

                //调用存储过程查询一览数据
                DataTable dt = daoGetDataListByCondition(sysflag, sysuid, rid, ConditionCarNo, ConditionDPH, ConditionSimCode, ConditionTNO, ConditionOrgNo, ConditionLockStatus, ConditionLendDateFrom, ConditionLendDateTo, ConditionServiceEDayFrom, ConditionServiceEDayTo, ConditionLeftDays, ConditionPayStatus);
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
        #endregion 查询一览数据

        #region 导出数据
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>一览数据</returns>
        public ResponseResult export(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag = string.Empty;
            string sysuid = string.Empty;
            string rid = string.Empty;
            string ConditionCarNo = string.Empty;
            string ConditionDPH = string.Empty;
            string ConditionSimCode = string.Empty;
            string ConditionTNO = string.Empty;
            string ConditionOrgNo = string.Empty;
            string ConditionLockStatus = string.Empty;
            string ConditionLendDateFrom = string.Empty;
            string ConditionLendDateTo = string.Empty;
            string ConditionServiceEDayFrom = string.Empty;
            string ConditionServiceEDayTo = string.Empty;
            string ConditionLeftDays = string.Empty;
            string ConditionPayStatus = string.Empty;

            if (!inparams.Keys.Contains("sid") ||
                !inparams.Keys.Contains("sysuid") ||
                !inparams.Keys.Contains("token") ||
                !inparams.Keys.Contains("sysflag") ||
                !inparams.Keys.Contains("rid") ||
                !inparams.Keys.Contains("ConditionCarNo") ||
                !inparams.Keys.Contains("ConditionDPH") ||
                !inparams.Keys.Contains("ConditionSimCode") ||
                !inparams.Keys.Contains("ConditionTNO") ||
                !inparams.Keys.Contains("ConditionOrgNo") ||
                !inparams.Keys.Contains("ConditionLockStatus") ||
                !inparams.Keys.Contains("ConditionLendDateFrom") ||
                !inparams.Keys.Contains("ConditionLendDateTo") ||
                !inparams.Keys.Contains("ConditionServiceEDayFrom") ||
                !inparams.Keys.Contains("ConditionServiceEDayTo") ||
                !inparams.Keys.Contains("ConditionLeftDays") ||
                !inparams.Keys.Contains("ConditionPayStatus"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                sysflag = inparams["sysflag"];
                sysuid = inparams["sysuid"];
                rid = inparams["rid"];
                ConditionCarNo = inparams["ConditionCarNo"];
                ConditionDPH = inparams["ConditionDPH"];
                ConditionSimCode = inparams["ConditionSimCode"];
                ConditionTNO = inparams["ConditionTNO"];
                ConditionOrgNo = inparams["ConditionOrgNo"];
                ConditionLockStatus = inparams["ConditionLockStatus"];
                ConditionLendDateFrom = inparams["ConditionLendDateFrom"];
                ConditionLendDateTo = inparams["ConditionLendDateTo"];
                ConditionServiceEDayFrom = inparams["ConditionServiceEDayFrom"];
                ConditionServiceEDayTo = inparams["ConditionServiceEDayTo"];
                ConditionLeftDays = inparams["ConditionLeftDays"];
                ConditionPayStatus = inparams["ConditionPayStatus"];

                //调用存储过程查询一览数据
                DataTable dtTemp = daoGetDataListByCondition(sysflag, sysuid, rid, ConditionCarNo, ConditionDPH, ConditionSimCode, ConditionTNO, ConditionOrgNo, ConditionLockStatus, ConditionLendDateFrom, ConditionLendDateTo, ConditionServiceEDayFrom, ConditionServiceEDayTo, ConditionLeftDays, ConditionPayStatus);
                if (dtTemp.Rows.Count == 0)
                {
                    Result = new ResponseResult(ResState.OperationFailed, "未检索到数据,导出失败！", "");
                }
                else
                {
                    // 格式化列
                    DataTable dt = dtTemp.Clone();
                    dt.Columns["ServiceEDay"].DataType = typeof(string);
                    dt.Columns["RepaymentDateTemp"].DataType = typeof(string);
                    for (int i = 0; i < dtTemp.Rows.Count; i++)
                    {
                        DataRow row = dt.NewRow();
                        if (!string.IsNullOrWhiteSpace(dtTemp.Rows[i]["ServiceEDay"].ToString()))
                        {
                            row["ServiceEDay"] = dtTemp.Rows[i]["ServiceEDay"].ToString().Substring(0, dtTemp.Rows[i]["ServiceEDay"].ToString().Length - 8);
                        }
                        if (!string.IsNullOrWhiteSpace(dtTemp.Rows[i]["RepaymentDateTemp"].ToString()))
                        {
                            row["RepaymentDateTemp"] = dtTemp.Rows[i]["RepaymentDateTemp"].ToString().Substring(0, dtTemp.Rows[i]["RepaymentDateTemp"].ToString().Length - 8);
                        }
                        dt.Rows.Add(row);
                        for (int j = 0; j < dtTemp.Columns.Count; j++)
                        {
                            if (string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
                            {
                                dt.Rows[i][j] = dtTemp.Rows[i][j];
                            }
                        }
                    }

                    // 设置导入内容
                    NPOIHelper npoiHelper = new NPOIHelper();

                    string workbookName = string.Empty;
                    string[] headerDataArray = { "车牌号", "VIN", "SIM卡号", "终端ID", "服务有效期止日", "到期还款日", "还款到期日提前提醒天数", "无通讯连接最长连续时间/天", "逾期天数", "还款状态", "锁车状态", "激活状态" };
                    for (int i = 0; i < 6; i++)
                    {
                        dt.Columns.RemoveAt(12);
                    }
                    string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                    npoiHelper.ContentData = contentDataArray;
                    npoiHelper.HeaderData = headerDataArray;
                    if (rid.Equals("25"))
                    {
                        workbookName = "参数设置";
                    }
                    else if (rid.Equals("26"))
                    {
                        workbookName = "激活服务设置";
                    }
                    npoiHelper.WorkbookName = workbookName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    string basepath = HttpRuntime.AppDomainAppPath.ToString();
                    string filePath = @"UI\Excel\Query\";
                    string sd = basepath + filePath;
                    npoiHelper.saveExcel(sd);
                    Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                }
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }
        #endregion 导出数据

        #region 设置参数
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>成功结果</returns>
        public ResponseResult set(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string uid = string.Empty;
            long cid = 0;
            long tno = 0;
            string datevalue = string.Empty;
            string dayvalue = string.Empty;
            string minutevalue = string.Empty;
            string CarNo = string.Empty;
            string OrgNo = string.Empty;
            string Lockstatus = string.Empty;
            try
            {
                #region 取参数
                if (!inparams.Keys.Contains("sid") ||
                    !inparams.Keys.Contains("sysuid") ||
                    !inparams.Keys.Contains("token") ||
                    !inparams.Keys.Contains("sysflag") ||
                    !inparams.Keys.Contains("uid") ||
                    !inparams.Keys.Contains("cid") ||
                    !inparams.Keys.Contains("tno") ||
                    !inparams.Keys.Contains("datevalue") ||
                    !inparams.Keys.Contains("dayvalue") ||
                    !inparams.Keys.Contains("minutevalue") ||
                    !inparams.Keys.Contains("CarNo") ||
                    !inparams.Keys.Contains("OrgNo") ||
                    !inparams.Keys.Contains("Lockstatus"))
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                    return Result;
                }
                uid = inparams["uid"];
                if (!string.IsNullOrWhiteSpace(inparams["cid"]))
                {
                    cid = Convert.ToInt64(inparams["cid"]);
                }
                if (!string.IsNullOrWhiteSpace(inparams["tno"]))
                {
                    tno = Convert.ToInt64(inparams["tno"]);
                }
                datevalue = inparams["datevalue"];
                dayvalue = inparams["dayvalue"];
                minutevalue = inparams["minutevalue"];
                CarNo = inparams["CarNo"];
                OrgNo = inparams["OrgNo"];
                Lockstatus = inparams["Lockstatus"];
                #endregion 取参数

                #region 调用WebService接口
                WebGIS.WebGISService.WebGISServiceSoap soap = new WebGIS.WebGISService.WebGISServiceSoapClient();
                WebGIS.WebGISService.XD_setParamOrderRequest request = new WebGIS.WebGISService.XD_setParamOrderRequest();
                WebGIS.WebGISService.XD_setParamOrderRequestBody requestBody = new WebGIS.WebGISService.XD_setParamOrderRequestBody();
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                DataSet ds = new DataSet();
                ds.ReadXml(basepath + "ALConfig.xml");
                DataTable dt = ds.Tables[0];
                string sysflag = dt.Rows[0]["WebServiceSysFlag"].ToString();
                sysflag = sysflag.Replace("\r\n", "").Trim();
                requestBody.sysflag = sysflag;
                requestBody.cid = cid;
                requestBody.tno = tno;
                requestBody.datevalue = datevalue;
                requestBody.dayvalue = dayvalue;
                requestBody.minutevalue = minutevalue;
                request.Body = requestBody;
                WebGIS.WebGISService.XD_setParamOrderResponse response = soap.XD_setParamOrder(request);
                WebGIS.WebGISService.XD_setParamOrderResponseBody responseBody = response.Body;
                WebGIS.WebGISService.ResponseResult result = responseBody.XD_setParamOrderResult;
                #endregion 调用WebService接口

                #region 调用存储过程
                if (result.state == 100)
                {
                    int iMinuteValue = Convert.ToInt32(minutevalue);
                    iMinuteValue = iMinuteValue / 24 / 60;
                    string parameter = "设置" + "到期还款日为" + datevalue + ",还款到期日提前提醒天数" + dayvalue + ",无通讯连接最长连续时间为" + iMinuteValue + "天";
                    sysflag = inparams["sysflag"];
                    //int iResult = daoSaveSysLog(sysflag, cid.ToString(), CarNo, OrgNo, uid, parameter, Lockstatus);
                    Result = new ResponseResult(ResState.Success, result.msg, 0);
                }
                else
                {
                    Result = new ResponseResult(ResState.OperationFailed, result.msg, result.result);
                }
                #endregion 调用存储过程
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }
        #endregion 设置参数

        #region 设置激活
        /// <summary>
        /// 设置激活
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>成功结果</returns>
        public ResponseResult active(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string uid = string.Empty;
            long cid = 0;
            long tno = 0;
            string vin = string.Empty;
            string energytype = string.Empty;
            int orderType = 0;
            string CarNo = string.Empty;
            string OrgNo = string.Empty;
            string Lockstatus = string.Empty;
            try
            {
                #region 取参数
                if (!inparams.Keys.Contains("sid") ||
                    !inparams.Keys.Contains("sysuid") ||
                    !inparams.Keys.Contains("token") ||
                    !inparams.Keys.Contains("sysflag") ||
                    !inparams.Keys.Contains("uid") ||
                    !inparams.Keys.Contains("cid") ||
                    !inparams.Keys.Contains("tno") ||
                    !inparams.Keys.Contains("vin") ||
                    !inparams.Keys.Contains("energytype") ||
                    !inparams.Keys.Contains("orderType") ||
                    !inparams.Keys.Contains("CarNo") ||
                    !inparams.Keys.Contains("OrgNo") ||
                    !inparams.Keys.Contains("Lockstatus"))
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                    return Result;
                }
                uid = inparams["uid"];
                if (!string.IsNullOrWhiteSpace(inparams["cid"]))
                {
                    cid = Convert.ToInt64(inparams["cid"]);
                }
                if (!string.IsNullOrWhiteSpace(inparams["tno"]))
                {
                    tno = Convert.ToInt64(inparams["tno"]);
                }
                vin = inparams["vin"];
                energytype = inparams["energytype"];
                if (!inparams["orderType"].ToString().Equals(string.Empty))
                {
                    orderType = Convert.ToInt32(inparams["orderType"]);
                }
                CarNo = inparams["CarNo"];
                OrgNo = inparams["OrgNo"];
                Lockstatus = inparams["Lockstatus"];
                #endregion 取参数

                #region 调用WebService接口
                WebGIS.WebGISService.WebGISServiceSoap soap = new WebGIS.WebGISService.WebGISServiceSoapClient();
                WebGIS.WebGISService.XD_activateOrderRequest request = new WebGIS.WebGISService.XD_activateOrderRequest();
                WebGIS.WebGISService.XD_activateOrderRequestBody requestBody = new WebGIS.WebGISService.XD_activateOrderRequestBody();
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                DataSet ds = new DataSet();
                ds.ReadXml(basepath + "ALConfig.xml");
                DataTable dt = ds.Tables[0];
                string sysflag = dt.Rows[0]["WebServiceSysFlag"].ToString();
                sysflag = sysflag.Replace("\r\n", "").Trim();
                requestBody.sysflag = sysflag;
                requestBody.cid = cid;
                requestBody.tno = tno;
                requestBody.vin = vin;
                requestBody.energytype = energytype;
                requestBody.orderType = orderType;
                request.Body = requestBody;
                WebGIS.WebGISService.XD_activateOrderResponse response = soap.XD_activateOrder(request);
                WebGIS.WebGISService.XD_activateOrderResponseBody responseBody = response.Body;
                WebGIS.WebGISService.ResponseResult result = responseBody.XD_activateOrderResult;
                #endregion 调用WebService接口

                #region 调用存储过程
                if (result.state == 100)
                {
                    string parameter = "设置激活状态为";
                    if (orderType == 0xAA)
                    {
                        parameter += "激活";
                    }
                    else if (orderType == 0x55)
                    {
                        parameter += "关闭";
                    }
                    sysflag = inparams["sysflag"];
                    //int iResult = daoSaveSysLog(sysflag, cid.ToString(), CarNo, OrgNo, uid, parameter, Lockstatus);
                    Result = new ResponseResult(ResState.Success, result.msg, 1);
                }
                else
                {
                    Result = new ResponseResult(ResState.OperationFailed, result.msg, result.result);
                }
                #endregion 调用存储过程
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }
        #endregion 设置激活

        #endregion 接口

        #region 数据接口

        #region 调用存储过程查询一览数据
        /// <summary>
        /// 查询一览数据
        /// </summary>
        /// <param name="sysflag">系统表示</param>
        /// <param name="sysuid">用户id</param>
        /// <param name="rid">角色id</param>
        /// <param name="ConditionCarNo">车牌号</param>
        /// <param name="ConditionDPH">VIN</param>
        /// <param name="ConditionSimCode">sim卡号</param>
        /// <param name="ConditionTNO">终端id</param>
        /// <param name="ConditionOrgNo">机构id</param>
        /// <param name="ConditionLockStatus">锁车状态</param>
        /// <param name="ConditionLendDateFrom">到期还款日开始</param>
        /// <param name="ConditionLendDateTo">到期还款日结束</param>
        /// <param name="ConditionServiceEDayFrom">服务有效期开始</param>
        /// <param name="ConditionServiceEDayTo">服务有效期结束</param>
        /// <param name="ConditionLeftDays">剩余日期</param>
        /// <param name="ConditionPayStatus">还款状态</param>
        /// <returns>查询结果</returns>
        private DataTable daoGetDataListByCondition(string sysflag, string sysuid, string rid, string ConditionCarNo, string ConditionDPH, string ConditionSimCode, string ConditionTNO, string ConditionOrgNo, string ConditionLockStatus, string ConditionLendDateFrom, string ConditionLendDateTo, string ConditionServiceEDayFrom, string ConditionServiceEDayTo, string ConditionLeftDays, string ConditionPayStatus)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@sysuid", sysuid) };
            if (rid.Equals("26"))
            {
                Parameters[0] = new SqlParameter("@sysuid", string.Empty);
            }
            DataTable dt = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_SelectParameterSetting"), Parameters, null, 1800).Tables[0];
            StringBuilder sbWhere = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(ConditionCarNo))
            {
                sbWhere.Append(" AND CarNo like '%" + ConditionCarNo + "%'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionDPH))
            {
                sbWhere.Append(" AND DPH like '%" + ConditionDPH + "%'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionSimCode))
            {
                sbWhere.Append(" AND SimCode like '%" + ConditionSimCode + "%'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionTNO))
            {
                sbWhere.Append(" AND TNO like '%" + ConditionTNO + "%'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionOrgNo))
            {
                sbWhere.Append(" AND OrgNo = '" + ConditionOrgNo + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionLockStatus))
            {
                sbWhere.Append(" AND LockStatus ='" + ConditionLockStatus + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionLendDateFrom))
            {
                sbWhere.Append(" AND RepaymentDateTemp >= '" + ConditionLendDateFrom + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionLendDateTo))
            {
                sbWhere.Append(" AND RepaymentDateTemp <= '" + ConditionLendDateTo + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionServiceEDayFrom))
            {
                sbWhere.Append(" AND ServiceEDay >= '" + ConditionServiceEDayFrom + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionServiceEDayTo))
            {
                sbWhere.Append(" AND ServiceEDay <= '" + ConditionServiceEDayTo + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionLeftDays))
            {
                sbWhere.Append(" AND LeftDays <= '" + ConditionLeftDays + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionPayStatus))
            {
                sbWhere.Append(" AND PayStatus = '" + ConditionPayStatus + "'");
            }
            if (sbWhere.Length != 0)
            {
                sbWhere.Remove(0, 4);
                DataView dv = dt.AsDataView();
                dv.RowFilter = sbWhere.ToString();
                dt = dv.ToTable();
            }
            return dt;
        }
        #endregion 调用存储过程查询一览数据

        //#region 调用存储过程新增系统日志
        //private int daoSaveSysLog(string sysflag, string CID, string CarNumber, string OrgNo, string OperatorId, string Parameter, string Lockstatus)
        //{
        //    ComSqlHelper csh = new ComSqlHelper();
        //    SqlParameter[] Parameters = { new SqlParameter("@CID", CID),
        //                                  new SqlParameter("@CarNumber", CarNumber),
        //                                  new SqlParameter("@OrgNo", OrgNo),
        //                                new SqlParameter("@OperatorId", OrgNo),
        //                                new SqlParameter("@Parameter", Parameter),
        //                                new SqlParameter("@ApplyId", null),
        //                                new SqlParameter("@ApplyLockStatusCd", null),
        //                                new SqlParameter("@ApplyTime", null),
        //                                new SqlParameter("@ApplyReason", null),
        //                                new SqlParameter("@ApproveTime", null),
        //                                new SqlParameter("@ApproveReason", null),
        //                                new SqlParameter("@LockStatusCd", Lockstatus),
        //                                new SqlParameter("@LockWayCd", null)};
        //    int iResult = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_SaveSysLog"), Parameters, false);
        //    return iResult;
        //}
        //#endregion 调用存储过程新增系统日志

        #endregion 数据接口
    }
}