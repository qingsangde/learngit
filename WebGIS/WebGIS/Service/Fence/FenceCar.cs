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
    public class FenceCar
    {
        public ResponseResult getFenceCarsList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string fid;
            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["uid"];
                fid = inparams["fid"];

                DataTable[] dtArr = getFenceCarList(sysflag, uid, fid);
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


        public ResponseResult SetFenceCar(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string fid;
            string tnos;

            try
            {
                sysflag = inparams["sysflag"];
                fid = inparams["fid"];
                uid = inparams["uid"];
                tnos = inparams["tnos"];

                int res = SaveFenceCar(sysflag,uid,fid,tnos);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "电子围栏-车辆关系设置成功！", "1");
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
        /// 查询电子栅栏车辆关联关系
        /// </summary>
        /// <param name="key"></param>
        /// <param name="uid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        private DataTable[] getFenceCarList(string key, string uid, string fid)
        {
            DataTable[] list = new DataTable[2];
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@Uid", uid);
                oaPara[1] = new SqlParameter("@F_ID", fid);
                DataSet ds = oSqlUtil.FillDataSet(key, WebProc.Proc("QSProc_QS_FenceCar_List"), oaPara, "fencecartable", 30);
                if (ds.Tables.Count == 2)
                {
                    list[0] = ds.Tables[0]; //电子围栏已关联车辆
                    list[1] = ds.Tables[1]; //电子围栏尚未关联车辆
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int SaveFenceCar(string key, string uid, string fid, string tnos)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@FID", fid);
                oaPara[1] = new SqlParameter("@TNOS", tnos);
                oaPara[2] = new SqlParameter("@UID", uid);

                return oSqlUtil.ExecuteSPNoQuery(key, WebProc.Proc("QSProc_QS_Fence_InsertFenceCar"), oaPara, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}