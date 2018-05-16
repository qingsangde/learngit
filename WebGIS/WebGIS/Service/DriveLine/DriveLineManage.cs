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
    public class DriveLineManage
    {
        public ResponseResult getDriveLineList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string linenname;
            string uid;
            try
            {
                sysflag = inparams["sysflag"];
                linenname = inparams["linenname"];
                uid = inparams["sysuid"];

                DataTable dt = getList(sysflag, linenname, uid);
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

        private DataTable getList(string sysflag, string linenname, string uid)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@Lname", linenname);
                oaPara[1] = new SqlParameter("@UID", uid);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(sysflag, WebProc.Proc("X80Proc_DriveLine_List_Select"), oaPara, "linetable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseResult getOneLine(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string lid;
            try
            {
                sysflag = inparams["sysflag"];
                lid = inparams["lid"];

                DataTable dt = getLineMarkerList(sysflag, lid);
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

        private DataTable getLineMarkerList(string sysflag, string lid)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@LID", lid);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(sysflag, WebProc.Proc("X80Proc_DriveLine_Marker_Select"), oaPara, "linetable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ResponseResult AddDriveLine(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string lname;
            string centerlng;
            string centerlat;
            string lradius;
            string desc;
            string dealercode;
            string linemarkers;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["sysuid"];
                lname = inparams["lname"];
                centerlng = inparams["centerlng"];
                centerlat = inparams["centerlat"];
                lradius = inparams["lradius"];
                desc = inparams["desc"];
                dealercode = inparams["dealercode"];
                linemarkers = inparams["linemarkers"];

                int res = InsertDriveLine(sysflag, uid, lname, centerlng, centerlat, lradius, desc, dealercode,linemarkers);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "新建试乘试驾线路成功！", "1");
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

        private int InsertDriveLine(string sysflag, string uid, string lname, string centerlng, string centerlat, string lradius, string desc, string dealercode, string linemarkers)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[8];
                oaPara[0] = new SqlParameter("@L_Name", lname);
                oaPara[1] = new SqlParameter("@L_CenterLng", centerlng);
                oaPara[2] = new SqlParameter("@L_CenterLat", centerlat);
                oaPara[3] = new SqlParameter("@L_RADIUS", lradius);
                oaPara[4] = new SqlParameter("@L_Desc", desc);
                oaPara[5] = new SqlParameter("@DEALERCODE", dealercode);
                oaPara[6] = new SqlParameter("@UID", uid);
                oaPara[7] = new SqlParameter("@MarkerS", linemarkers);

                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("X80Proc_DriveLine_Insert"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ResponseResult DeleteDriveLines(Dictionary<string, string> inparams)
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

                int res = DeleteLines(sysflag, uid, ids);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "删除试乘试驾线路成功！", "1");
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

        private int DeleteLines(string sysflag, string uid, string ids)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@ids", ids);
                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("X80Proc_DriveLine_Delete"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ResponseResult getLineCarsList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string lid;
            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["uid"];
                lid = inparams["lid"];

                DataTable[] dtArr = getLineCarList(sysflag, uid, lid);
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

        private DataTable[] getLineCarList(string sysflag, string uid, string lid)
        {
            DataTable[] list = new DataTable[2];
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@Uid", uid);
                oaPara[1] = new SqlParameter("@LineId", lid);
                DataSet ds = oSqlUtil.FillDataSet(sysflag, WebProc.Proc("X80Proc_DriveLineCar_List"), oaPara, "linecartable", 30);
                if (ds.Tables.Count == 2)
                {
                    list[0] = ds.Tables[0]; //线路已关联车辆
                    list[1] = ds.Tables[1]; //线路尚未关联车辆
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseResult SetLineCar(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string lid;
            string tnos;

            try
            {
                sysflag = inparams["sysflag"];
                lid = inparams["lid"];
                uid = inparams["uid"];
                tnos = inparams["tnos"];

                int res = SaveLineCar(sysflag, uid, lid, tnos);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "试乘试驾线路匹配车辆关系设置成功！", "1");
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

        private int SaveLineCar(string sysflag, string uid, string lid, string tnos)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@LineId", lid);
                oaPara[1] = new SqlParameter("@TNOS", tnos);
                oaPara[2] = new SqlParameter("@Uid", uid);

                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("X80Proc_DriveLineCar_Set"), oaPara, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}