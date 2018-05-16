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
    public class LoanOrg   
    {
        #region 定义
        #endregion 定义
        #region 初始化
        #endregion 初始化
        #region 事件

        /// <summary>
        /// 基础信息列表查询事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getLoanOrgList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string sysuid;
            string orgName;

            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("orgName"))
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
                orgName = inparams["orgName"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];

                //调用方法取得数据表格
                DataTable dt = GetSelectData(sysflag, orgName);

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
        /// 编辑页初始化
        /// </summary>
        /// <param name="inparams">参数</param>
        /// <returns>查询结果</returns>
        public ResponseResult editByOrgNo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string orgNo;
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("orgNo") || !inparams.Keys.Contains("sysuid"))
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

                orgNo = inparams["orgNo"];
                sysflag = inparams["sysflag"];

                DataTable dt = GetOneOrgSearch(sysflag, orgNo);
                int Total = 1;
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
        /// 删除事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult deleteLoanOrg(Dictionary<string, string> inparams) 
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string ids;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["sysuid"];
                ids = inparams["ids"];

                int res = DeleteOrg(sysflag, uid, ids);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "删除成功！", "1");
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

        /// <summary>
        /// 新增保存事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult addSaveLoanOrg(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;                 //数据库标识
            string sysuid;                  //操作者uid
            string orgName;                 //机构名称
            string remarks;                 //备注


            if (!inparams.Keys.Contains("orgName") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid"))
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

                sysflag = inparams["sysflag"];                           //数据库标识
                orgName = inparams["orgName"];                           //机构名称 
                remarks = inparams["remarks"];                           //备注
                sysuid = inparams["sysuid"];                             //操作者uid

                int judgeflag = EditJudge(sysflag, "", orgName);

                if (judgeflag == 0)
                {
                    //保存
                    int res = AddSave(sysflag, orgName, remarks, sysuid);
                    if (res > 0)
                    {
                        Result = new ResponseResult(ResState.Success, "成功机构信息！", "");
                    }
                }
                else
                {
                    if (judgeflag == 1)
                    {
                        Result = new ResponseResult(ResState.OperationFailed, "机构名称不能重复！", "");
                        return Result;
                    }
                }

            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        /// <summary>
        /// 修改保存事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult editSaveOrgNo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;                 //数据库标识
            string sysuid;                  //操作者uid
            string orgNo;                   //机构编号 
            string orgName;                 //机构名称
            string remarks;                 //备注
            if (!inparams.Keys.Contains("orgNo") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("orgName") || !inparams.Keys.Contains("sysuid"))
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

                sysflag = inparams["sysflag"];                           //数据库标识
                orgNo = inparams["orgNo"];                               //机构编号
                orgName = inparams["orgName"];                           //机构名称
                sysuid = inparams["sysuid"];                             //操作者uid
                remarks = inparams["remarks"];                           //备注
                int judgeflag = EditJudge(sysflag, orgNo, orgName);

                if (judgeflag == 0)
                {

                    //保存
                    int res = editSave(sysflag, orgNo, orgName, remarks, sysuid);
                    if (res > 0)
                    {
                        Result = new ResponseResult(ResState.Success, "成功保存机构信息！", "");

                    }

                }
                else
                {
                    if (judgeflag == 1)
                    {
                        Result = new ResponseResult(ResState.OperationFailed, "机构名称不能重复！", "");
                        return Result;
                    }

                }

            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        #endregion 事件

        #region 方法

        /// <summary>
        /// 根据车辆ID取得编辑页dataTable
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="orgNo"></param>
        /// <returns></returns>
        private DataTable GetOneOrgSearch(string sysflag, string orgNo)
        {

            ComSqlHelper csh = new ComSqlHelper();
            DataTable dt = new DataTable();
            dt = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_Query_LoanOrg"), null, null, 1800).Tables[0];
            DataView dv = dt.AsDataView();

            //机构名称
            if (!string.IsNullOrEmpty(orgNo))
            {
                dv.RowFilter = "OrgNo = '" + orgNo.Trim() + "'";
            }

            return dv.ToTable();
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="uid"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private int DeleteOrg(string sysflag, string uid, string ids)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@ids", ids);
                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_Delete_LoanOrg"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增保存方法
        /// </summary>
        /// <param name="sysuid"></param>
        /// <returns>更新结果 0:失败;非0:成功</returns>
        private int AddSave(string sysflag, string orgName, string remarks,  string sysuid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@OrgName",orgName), 
                                            new SqlParameter("@Remarks",remarks), 
                                            new SqlParameter("@UserId",sysuid)
                                        };
                res = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_InsertLoanOrg"), Parameters, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// 修改保存方法
        /// </summary>
        ///  /// <param name="orgNo">机构编号</param>
        /// <param name="orgName">机构</param>
        /// <param name="remarks">备注</param>
        /// <returns>更新结果 0:失败;非0:成功</returns>
        private int editSave(string sysflag, string orgNo, string orgName, string remarks, string sysuid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@OrgNo",orgNo), 
                                            new SqlParameter("@Remarks",remarks), 
                                            new SqlParameter("@OrgName",orgName),
                                            new SqlParameter("@UserId",sysuid)
                                        };
                res = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_UpdateLoanOrg"), Parameters, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// 验证机构名称是否重复
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">机构编号</param>
        /// <param name="carno">机构名称</param>
        /// <returns>0:不重复;1：重复</returns>
        private int EditJudge(string sysflag, string orgNo, string orgName)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@OrgNo", orgNo), 
                                            new SqlParameter("@OrgName", orgName)
                                        };
                res = Convert.ToInt32(csh.ExecuteSPScalar(sysflag, WebProc.Proc("ALPrco_LoanOrg_Edit_Jugde"), Parameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="orgName">机构名称</param>
        /// <returns></returns>
        public static DataTable GetSelectData(string sysflag, string orgName) 
        {
            ComSqlHelper csh = new ComSqlHelper();
            DataTable dt = new DataTable();
            dt = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_Query_LoanOrg"), null, null, 1800).Tables[0];
            DataView dv = dt.AsDataView();

            //机构名称
            if (!string.IsNullOrEmpty(orgName))
            {
                dv.RowFilter = "OrgName like '%" + orgName.Trim() + "%'";
            }
           
            return dv.ToTable();
        }
        #endregion 方法

    }
}