using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary;
using System.Data.SqlClient;
using System.Data;

namespace WebGIS 
{
    public class ExtYQWL
    {
        private carinfo GetCarCID(string sysflag, string carno)
        {
            carinfo ci = new carinfo();
            long cid = -1;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@carno", carno) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetCarInfoByCarno"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                ci.cid = long.Parse(dt.Rows[0]["CID"].ToString());
                ci.carno = dt.Rows[0]["CarNo"].ToString();
                ci.owner = dt.Rows[0]["CarOwnName"].ToString();

            }
            else
            {
                return null;
            }
            return ci;
        }
        public ResponseAppResult AppGetCarInfoByCarno(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("carno") || inparams["carno"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少carno或carno为空！", null);
                return Result;
            }


            try
            {
                string sysflag = inparams["sysflag"];
                string carno = inparams["carno"];

                carinfo sdf = GetCarCID(sysflag, carno);
                if (sdf != null)
                    Result = new ResponseAppResult(ResState.Success, "操作成功", sdf);
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "未查询到相关车辆", sdf);
                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppGetCarInfoByCarno调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public ResponseAppResult AppVINUpload(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("vins") || inparams["vins"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少vins或vins为空！", null);
                return Result;
            }

            try
            {
                string sysflag = inparams["sysflag"];
                string cid = inparams["cid"];
                string vins = inparams["vins"];
                string uploaddate = inparams["uploaddate"];
                string[] rtarray = vins.Split(',');
                ComSqlHelper csh = new ComSqlHelper();

                foreach (string vin in rtarray)
                {

                    SqlParameter[] Parameters2 = { new SqlParameter("@cid", cid)
                                                    , new SqlParameter("@vin",vin)
                                                    , new SqlParameter("@uploaddate", uploaddate)  };
                    csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("QWGProc_App_ExtVINUpload"), Parameters2, false);
                }
                Result = new ResponseAppResult(ResState.Success, "操作成功", null);

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppVINUpload调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
    }

    public class carinfo
    {
        public long cid;
        public string owner;
        public string carno;
    }
}