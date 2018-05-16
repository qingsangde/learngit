using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;
using CommLibrary;
using System.Data.SqlClient;
using System.Data;

namespace SysService
{
    public class OrderPhotoResponse
    {
        public ResponseResult GetLastPhoto(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                string sysflag = inparams["sysflag"];
                string cids = inparams["cids"];
                SqlParameter[] Parameters0 = new SqlParameter[1];
                Parameters0[0] = new SqlParameter("@CIDS", cids);
                DataTable dt = csh.FillDataSet(sysflag, WebProc.Proc("GetLastPhotoByCids"), Parameters0).Tables[0];

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
    }
}