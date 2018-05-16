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
    public class LockApplication
    {
        #region 接口

        #region 查询字典项
        /// <summary>
        /// 查询字典项
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>字典项列表</returns>
        public ResponseResult getDict(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag = string.Empty;
            string dictType = string.Empty;

            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("dictType"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                sysflag = inparams["sysflag"];
                dictType = inparams["dictType"];

                //调用存储过程查询字典项
                DataTable dt = daoGetDictByType(sysflag, dictType);
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
        #endregion 查询字典项

        #region 查询放款机构
        /// <summary>
        /// 查询放款机构
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>字典项列表</returns>
        public ResponseResult getOrg(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag = string.Empty;

            if (!inparams.Keys.Contains("sysflag"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                sysflag = inparams["sysflag"];

                //调用存储过程查询放款机构
                DataTable dt = daoGetOrg(sysflag);
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
        #endregion 查询放款机构

        #region 查询终端型号
        /// <summary>
        /// 查询终端型号
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>字典项列表</returns>
        public ResponseResult getTertype(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag = string.Empty;

            if (!inparams.Keys.Contains("sysflag"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                sysflag = inparams["sysflag"];

                //调用存储过程查询终端型号
                DataTable dt = daoGetTertype(sysflag);
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
        #endregion 查询终端型号

        #region 查询发动机类型
        /// <summary>
        /// 查询发动机类型
        /// </summary>
        /// <param name="inparams">初始参数</param>
        /// <returns>发动机类型列表</returns>
        public ResponseResult getEnergyType(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag = string.Empty;

            if (!inparams.Keys.Contains("sysflag"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                sysflag = inparams["sysflag"];

                //调用存储过程查询发动机类型
                DataTable dt = daoGetEnergyType(sysflag);
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
        #endregion 查询发动机类型

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
            string ConditionTertypenum = string.Empty;
            string ConditionEnergyTypePKey = string.Empty;
            string ConditionAuditStatusCd = string.Empty;
            string ConditionActiv = string.Empty;

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
                !inparams.Keys.Contains("ConditionPayStatus") ||
                !inparams.Keys.Contains("ConditionTertypenum") ||
                !inparams.Keys.Contains("ConditionEnergyTypePKey") ||
                !inparams.Keys.Contains("ConditionAuditStatusCd") ||
                !inparams.Keys.Contains("ConditionActiv"))
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
                ConditionTertypenum = inparams["ConditionTertypenum"];
                ConditionEnergyTypePKey = inparams["ConditionEnergyTypePKey"];
                ConditionAuditStatusCd = inparams["ConditionAuditStatusCd"];
                ConditionActiv = inparams["ConditionActiv"];

                //调用存储过程查询一览数据
                DataTable dt = daoGetDataListByCondition(sysflag, sysuid, rid, ConditionCarNo, ConditionDPH, ConditionSimCode, ConditionTNO, ConditionOrgNo, ConditionLockStatus, ConditionLendDateFrom, ConditionLendDateTo, ConditionServiceEDayFrom, ConditionServiceEDayTo, ConditionLeftDays, ConditionPayStatus, ConditionTertypenum, ConditionEnergyTypePKey, ConditionAuditStatusCd, ConditionActiv);
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
            string ConditionTertypenum = string.Empty;
            string ConditionEnergyTypePKey = string.Empty;
            string ConditionAuditStatusCd = string.Empty;
            string ConditionActiv = string.Empty;

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
                !inparams.Keys.Contains("ConditionPayStatus") ||
                !inparams.Keys.Contains("ConditionTertypenum") ||
                !inparams.Keys.Contains("ConditionEnergyTypePKey") ||
                !inparams.Keys.Contains("ConditionAuditStatusCd") ||
                !inparams.Keys.Contains("ConditionActiv"))
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
                ConditionTertypenum = inparams["ConditionTertypenum"];
                ConditionEnergyTypePKey = inparams["ConditionEnergyTypePKey"];
                ConditionAuditStatusCd = inparams["ConditionAuditStatusCd"];
                ConditionActiv = inparams["ConditionActiv"];

                //调用存储过程查询一览数据
                DataTable dtTemp = daoGetDataListByCondition(sysflag, sysuid, rid, ConditionCarNo, ConditionDPH, ConditionSimCode, ConditionTNO, ConditionOrgNo, ConditionLockStatus, ConditionLendDateFrom, ConditionLendDateTo, ConditionServiceEDayFrom, ConditionServiceEDayTo, ConditionLeftDays, ConditionPayStatus, ConditionTertypenum, ConditionEnergyTypePKey, ConditionAuditStatusCd, ConditionActiv);
                if (dtTemp.Rows.Count == 0)
                {
                    Result = new ResponseResult(ResState.OperationFailed, "未检索到数据,导出失败！", "");
                }
                else
                {
                    // 格式化列
                    DataTable dt = dtTemp.Clone();
                    dt.Columns["ServiceEDay"].DataType = typeof(string);
                    dt.Columns["PaymentDueDay"].DataType = typeof(string);
                    dt.Columns["PaymentAccount"].DataType = typeof(string);
                    for (int i = 0; i < dtTemp.Rows.Count; i++)
                    {
                        DataRow row = dt.NewRow();
                        if (!string.IsNullOrWhiteSpace(dtTemp.Rows[i]["ServiceEDay"].ToString()))
                        {
                            row["ServiceEDay"] = dtTemp.Rows[i]["ServiceEDay"].ToString().Substring(0, dtTemp.Rows[i]["ServiceEDay"].ToString().Length - 8);
                        }
                        if (!string.IsNullOrWhiteSpace(dtTemp.Rows[i]["PaymentDueDay"].ToString()))
                        {
                            row["PaymentDueDay"] = dtTemp.Rows[i]["PaymentDueDay"].ToString().Substring(0, dtTemp.Rows[i]["PaymentDueDay"].ToString().Length - 8);
                        }
                        if (!string.IsNullOrWhiteSpace(dtTemp.Rows[i]["PaymentAccount"].ToString()))
                        {
                            row["PaymentAccount"] = String.Format("{0:N}", Convert.ToDouble(dtTemp.Rows[i]["PaymentAccount"].ToString()));
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
                    if (rid.Equals("25"))
                    {
                        string[] headerDataArray = { "车牌号", "VIN", "SIM卡号", "终端ID", "服务有效期止日", "到期还款日", "到期还款金额", "还款状态", "逾期天数", "锁车状态" };
                        for (int i = 0; i < 5; i++)
                        {
                            dt.Columns.RemoveAt(10);
                        }
                        string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                        npoiHelper.ContentData = contentDataArray;
                        npoiHelper.HeaderData = headerDataArray;
                        workbookName = "锁车解锁申请";
                    }
                    else if (rid.Equals("26"))
                    {
                        string[] headerDataArray = { "车牌号", "审核状态", "VIN", "SIM卡号", "终端ID", "服务有效期止日", "到期还款日", "到期还款金额", "还款状态", "逾期天数", "锁车状态", "申请公司", "申请内容", "申请理由", "申请时间" };
                        for (int i = 0; i < 12; i++)
                        {
                            dt.Columns.RemoveAt(15);
                        }
                        string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                        npoiHelper.ContentData = contentDataArray;
                        npoiHelper.HeaderData = headerDataArray;
                        workbookName = "锁车解锁";
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

        #region 申请锁车/解锁
        /// <summary>
        /// 申请锁车/解锁
        /// </summary>
        /// <param name="inparams">参数</param>
        /// <returns>成功结果</returns>
        public ResponseResult apply(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysuid = string.Empty;
            string sysflag = string.Empty;
            string ApplyLockStatusCd = string.Empty;
            string ApplyReason = string.Empty;
            string CID = string.Empty;

            if (!inparams.Keys.Contains("sid") ||
                !inparams.Keys.Contains("sysuid") ||
                !inparams.Keys.Contains("token") ||
                !inparams.Keys.Contains("sysflag") ||
                !inparams.Keys.Contains("ApplyLockStatusCd") ||
                !inparams.Keys.Contains("ApplyReason") ||
                !inparams.Keys.Contains("CID"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                ApplyLockStatusCd = inparams["ApplyLockStatusCd"];
                ApplyReason = inparams["ApplyReason"];
                CID = inparams["CID"];

                //调用存储过程新增申请数据
                int iResult = daoInserApplyControl(sysflag, sysuid, ApplyLockStatusCd, ApplyReason, CID);
                if (iResult >= 0)
                {
                    Result = new ResponseResult(ResState.Success, "申请成功！", "1");
                }
                else
                {
                    Result = new ResponseResult(ResState.OperationFailed, "发生未知错误！请联系技术人员！", "0");
                }
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }
        #endregion 申请锁车/解锁

        #region 审批锁车/解锁
        /// <summary>
        /// 审批锁车/解锁
        /// </summary>
        /// <param name="inparams">参数</param>
        /// <returns>成功结果</returns>
        public ResponseResult approve(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string id = string.Empty;
            string uid = string.Empty;
            long cid = 0;
            long tno = 0;
            int orderType = 0;
            int lockType = 0;
            uint torque = 0;
            uint rotspeed = 0;
            string lockreason = string.Empty;
            string unlockreason = string.Empty;
            try
            {
                #region 取参数
                if (!inparams.Keys.Contains("sid") ||
                    !inparams.Keys.Contains("sysuid") ||
                    !inparams.Keys.Contains("token") ||
                    !inparams.Keys.Contains("sysflag") ||
                    !inparams.Keys.Contains("id") ||
                    !inparams.Keys.Contains("uid") ||
                    !inparams.Keys.Contains("cid") ||
                    !inparams.Keys.Contains("tno") ||
                    !inparams.Keys.Contains("orderType") ||
                    !inparams.Keys.Contains("lockType") ||
                    !inparams.Keys.Contains("torque") ||
                    !inparams.Keys.Contains("rotspeed") ||
                    !inparams.Keys.Contains("lockreason") ||
                    !inparams.Keys.Contains("unlockreason"))
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                    return Result;
                }
                id = inparams["id"];
                uid = inparams["uid"];
                if (!string.IsNullOrWhiteSpace(inparams["cid"]))
                {
                    cid = Convert.ToInt64(inparams["cid"]);
                }
                if (!string.IsNullOrWhiteSpace(inparams["tno"]))
                {
                    tno = Convert.ToInt64(inparams["tno"]);
                }
                if (!string.IsNullOrWhiteSpace(inparams["orderType"]))
                {
                    orderType = Convert.ToInt32(inparams["orderType"]);
                }
                if (!string.IsNullOrWhiteSpace(inparams["lockType"]))
                {
                    tno = Convert.ToInt32(inparams["lockType"]);
                }
                if (!string.IsNullOrWhiteSpace(inparams["torque"]))
                {
                    torque = Convert.ToUInt32(inparams["torque"]);
                }
                if (!string.IsNullOrWhiteSpace(inparams["rotspeed"]))
                {
                    rotspeed = Convert.ToUInt32(inparams["rotspeed"]);
                }
                lockreason = inparams["lockreason"];
                unlockreason = inparams["unlockreason"];
                #endregion 取参数

                #region 调用WebService接口
                WebGIS.WebGISService.WebGISServiceSoap soap = new WebGIS.WebGISService.WebGISServiceSoapClient();
                WebGIS.WebGISService.XD_lockOrderRequest request = new WebGIS.WebGISService.XD_lockOrderRequest();
                WebGIS.WebGISService.XD_lockOrderRequestBody requestBody = new WebGIS.WebGISService.XD_lockOrderRequestBody();
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                DataSet ds = new DataSet();
                ds.ReadXml(basepath + "ALConfig.xml");
                DataTable dt = ds.Tables[0];
                string sysflag = dt.Rows[0]["WebServiceSysFlag"].ToString();
                sysflag = sysflag.Replace("\r\n", "").Trim();
                requestBody.sysflag = sysflag;
                requestBody.cid = cid;
                requestBody.tno = tno;
                requestBody.orderType = orderType;
                requestBody.lockType = lockType;
                requestBody.torque = torque;
                requestBody.rotspeed = rotspeed;
                requestBody.lockreason = lockreason;
                requestBody.unlockreason = unlockreason;
                request.Body = requestBody;
                WebGIS.WebGISService.XD_lockOrderResponse response = soap.XD_lockOrder(request);
                WebGIS.WebGISService.XD_lockOrderResponseBody responseBody = response.Body;
                WebGIS.WebGISService.ResponseResult result = responseBody.XD_lockOrderResult;
                #endregion 调用WebService接口

                #region 调用存储过程
                if (result.state == 100)
                {
                    sysflag = inparams["sysflag"];
                    int iResult = daoUpdateApproveControl(sysflag, id, lockreason, unlockreason, torque, rotspeed, result, uid, lockType);
                    Result = new ResponseResult(ResState.Success, result.msg, iResult);
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
        #endregion 审批锁车/解锁

        #endregion 接口

        #region 数据接口

        #region 调用存储过程查询字典项
        /// <summary>
        /// 调用存储过程查询字典项
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="dictType">字典类别</param>
        /// <returns>查询结果</returns>
        private DataTable daoGetDictByType(string sysflag, string dictType)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@dictType", dictType) };
            return csh.FillDataSet(sysflag, WebProc.Proc("ALProc_SelectDictByType"), Parameters, null, 1800).Tables[0];
        }
        #endregion 调用存储过程查询字典项

        #region 调用存储过程查询放款机构
        /// <summary>
        /// 调用存储过程查询放款机构
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <returns>查询结果</returns>
        private DataTable daoGetOrg(string sysflag)
        {
            ComSqlHelper csh = new ComSqlHelper();
            return csh.FillDataSet(sysflag, WebProc.Proc("ALProc_SelectAllOrg"), null, null, 1800).Tables[0];
        }
        #endregion 调用存储过程查询放款机构

        #region 调用存储过程查询终端型号
        /// <summary>
        /// 调用存储过程查询终端型号
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <returns>查询结果</returns>
        private DataTable daoGetTertype(string sysflag)
        {
            ComSqlHelper csh = new ComSqlHelper();
            return csh.FillDataSet(sysflag, WebProc.Proc("ALProc_SelectAllTerminalType"), null, null, 1800).Tables[0];
        }
        #endregion 调用存储过程查询终端型号

        #region 调用存储过程查询发动机类型
        /// <summary>
        /// 调用存储过程查询发动机类型
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <returns>查询结果</returns>
        private DataTable daoGetEnergyType(string sysflag)
        {
            ComSqlHelper csh = new ComSqlHelper();
            return csh.FillDataSet(sysflag, WebProc.Proc("ALProc_SelectAllEnergyType"), null, null, 1800).Tables[0];
        }
        #endregion 调用存储过程查询发动机类型

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
        /// <param name="ConditionTertypenum">终端类型</param>
        /// <param name="ConditionEnergyTypePKey">发动机类型</param>
        /// <param name="ConditionAuditStatusCd">审批状态</param>
        /// <param name="ConditionActiv">激活状态</param>
        /// <returns>查询结果</returns>
        private DataTable daoGetDataListByCondition(string sysflag, string sysuid, string rid, string ConditionCarNo, string ConditionDPH, string ConditionSimCode, string ConditionTNO, string ConditionOrgNo, string ConditionLockStatus, string ConditionLendDateFrom, string ConditionLendDateTo, string ConditionServiceEDayFrom, string ConditionServiceEDayTo, string ConditionLeftDays, string ConditionPayStatus, string ConditionTertypenum, string ConditionEnergyTypePKey, string ConditionAuditStatusCd, string ConditionActiv)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@sysuid", sysuid) };
            string procName = string.Empty;

            if (rid.Equals("25"))
            {
                procName = "ALProc_SelectLockForJXS";
            }
            else if (rid.Equals("26"))
            {
                procName = "ALProc_SelectLockForQM";
                Parameters = null;
            }
            DataTable dt = csh.FillDataSet(sysflag, WebProc.Proc(procName), Parameters, null, 1800).Tables[0];
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
                sbWhere.Append(" AND PaymentDueDay >= '" + ConditionLendDateFrom + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionLendDateTo))
            {
                sbWhere.Append(" AND PaymentDueDay <= '" + ConditionLendDateTo + "'");
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
                sbWhere.Append(" AND ISNULL(LeftDays,'0') <= '" + Convert.ToInt32(ConditionLeftDays) + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionPayStatus))
            {
                sbWhere.Append(" AND PayStatus = '" + ConditionPayStatus + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionTertypenum))
            {
                sbWhere.Append(" AND Tertypenum ='" + ConditionTertypenum + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionEnergyTypePKey))
            {
                sbWhere.Append(" AND EnergyTypePKey ='" + ConditionEnergyTypePKey + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionAuditStatusCd))
            {
                sbWhere.Append(" AND AuditStatusCd ='" + ConditionAuditStatusCd + "'");
            }
            if (!string.IsNullOrWhiteSpace(ConditionActiv))
            {
                sbWhere.Append(" AND Activ ='" + ConditionActiv + "'");
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

        #region 调用存储过程新增锁车/解锁申请
        /// <summary>
        /// 新增锁车/解锁申请
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="sysuid">用户id</param>
        /// <param name="ApplyLockStatusCd">申请状态</param>
        /// <param name="ApplyReason">申请理由</param>
        /// <param name="CID">车辆id</param>
        /// <returns>结果</returns>
        private int daoInserApplyControl(string sysflag, string sysuid, string ApplyLockStatusCd, string ApplyReason, string CID)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@CID", CID),
                                          new SqlParameter("@ApplyLockStatusCd", ApplyLockStatusCd),
                                        new SqlParameter("@ApplyReason", ApplyReason),
                                        new SqlParameter("@CREATE_USER", sysuid)};
            int iResult = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_InserApplyControl"), Parameters, false);
            return iResult;
        }
        #endregion 调用存储过程新增锁车/解锁申请

        #region 调用存储过程审批锁车/解锁申请
        private int daoUpdateApproveControl(string sysflag, string id, string lockreason, string unlockreason, uint torque, uint rotspeed, WebGIS.WebGISService.ResponseResult result, string uid, int lockType)
        {
            ComSqlHelper csh = new ComSqlHelper();
            string ApproveReason = string.Empty;
            string LockParameter = string.Empty;
            if (!lockreason.Equals(string.Empty))
            {
                ApproveReason = lockreason;
            }
            else
            {
                ApproveReason = unlockreason;
            }
            if (torque != 0)
            {
                LockParameter = torque.ToString();
            }
            else if (rotspeed != 0)
            {
                LockParameter = rotspeed.ToString();
            }
            else
            {
                LockParameter = string.Empty;
            }
            SqlParameter[] Parameters = { new SqlParameter("@Id", id),
                                             new SqlParameter("@ApproveReason", ApproveReason),
                                            new SqlParameter("@LockModeCd", lockType),
                                            new SqlParameter("@LockParameter", LockParameter),
                                            new SqlParameter("@ResponseStatusCd", result.state),
                                            new SqlParameter("@ResponseData", result.result),
                                            new SqlParameter("@UPDATE_USER", uid)};
            int iResult = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_UpdateApproveControl"), Parameters, false);
            return iResult;
        }
        #endregion 调用存储过程审批锁车/解锁申请

        #endregion 数据接口
    }
}