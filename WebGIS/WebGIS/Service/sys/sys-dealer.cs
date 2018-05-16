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
    public class sys_dealer
    {
        public ResponseResult getLoginDealer(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["sysuid"];

                DataTable dt = getList(sysflag,uid);
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

        private DataTable getList(string sysflag, string uid)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@UID", uid);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(sysflag, WebProc.Proc("X80Proc_GetDealerCodeByUid"), oaPara, "dealertable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}