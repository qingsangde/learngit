using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary.Log;
using CommLibrary;
using System.Data.SqlClient;
using WebGIS;

namespace SysService
{
    public class sys_log
    {
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="logmodel">操作日志类</param>
        public void WriteSysLog(LogModel logmodel)
        {
            ComSqlHelper csh = new ComSqlHelper();

            SqlParameter[] Parameters = new SqlParameter[6];
            Parameters[0] = new SqlParameter("@UserID", logmodel.UserId);
            Parameters[1] = new SqlParameter("@LogType", logmodel.LogType);
            Parameters[2] = new SqlParameter("@LogContent", logmodel.LogContent);
            Parameters[3] = new SqlParameter("@LogResult", logmodel.LogResult);
            Parameters[4] = new SqlParameter("@OneCarUser", logmodel.OneCarUser);
            Parameters[5] = new SqlParameter("@SysName", logmodel.SysName);


            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(logmodel.SysFlag),WebProc.Proc("SysLogInsert"), Parameters, true);
        }
    }
}