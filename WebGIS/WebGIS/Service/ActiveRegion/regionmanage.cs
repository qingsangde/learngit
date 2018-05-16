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
    public class regionmanage
    {
        public ResponseResult getRegionList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string regionname;
            string uid;
            try
            {
                sysflag = inparams["sysflag"];
                regionname = inparams["regionname"];
                uid = inparams["sysuid"];

                DataTable dt = getList(sysflag, regionname, uid);
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

        private DataTable getList(string sysflag, string regionname, string uid)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@Rname", regionname);
                oaPara[1] = new SqlParameter("@UID", uid);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(sysflag, WebProc.Proc("X80Proc_Region_List_Select"), oaPara, "regiontable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ResponseResult AddRegion(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string rname;
            string centerlng;
            string centerlat;
            string rradius;
            string desc;
            string dealercode;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["sysuid"];
                rname = inparams["rname"];
                centerlng = inparams["centerlng"];
                centerlat = inparams["centerlat"];
                rradius = inparams["rradius"];
                desc = inparams["desc"];
                dealercode = inparams["dealercode"];

                int res = InsertRegion(sysflag, uid, rname, centerlng, centerlat, rradius, desc, dealercode);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "新建活动区域成功！", "1");
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

        private int InsertRegion(string sysflag, string uid, string rname, string centerlng, string centerlat, string rradius, string desc, string dealercode)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[7];
                oaPara[0] = new SqlParameter("@R_Name", rname);
                oaPara[1] = new SqlParameter("@R_CENTER_LNG", centerlng);
                oaPara[2] = new SqlParameter("@R_CENTER_LAT", centerlat);
                oaPara[3] = new SqlParameter("@R_RADIUS", rradius);
                oaPara[4] = new SqlParameter("@R_DESC", desc);
                oaPara[5] = new SqlParameter("@DEALERCODE", dealercode);
                oaPara[6] = new SqlParameter("@UID", uid);

                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("X80Proc_Region_Insert"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseResult EditRegion(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string rname;
            string centerlng;
            string centerlat;
            string rradius;
            string desc;
            string regionid;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["sysuid"];
                rname = inparams["rname"];
                centerlng = inparams["centerlng"];
                centerlat = inparams["centerlat"];
                rradius = inparams["rradius"];
                desc = inparams["desc"];
                regionid = inparams["regionid"];

                int res = EditTheRegion(sysflag, uid, rname, centerlng, centerlat, rradius, desc, regionid);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "编辑活动区域成功！", "1");
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

        private int EditTheRegion(string sysflag, string uid, string rname, string centerlng, string centerlat, string rradius, string desc, string regionid)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[7];
                oaPara[0] = new SqlParameter("@R_Name", rname);
                oaPara[1] = new SqlParameter("@R_CENTER_LNG", centerlng);
                oaPara[2] = new SqlParameter("@R_CENTER_LAT", centerlat);
                oaPara[3] = new SqlParameter("@R_RADIUS", rradius);
                oaPara[4] = new SqlParameter("@R_DESC", desc);
                oaPara[5] = new SqlParameter("@R_Id", regionid);
                oaPara[6] = new SqlParameter("@UID", uid);

                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("X80Proc_Region_Update"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseResult DeleteRegion(Dictionary<string, string> inparams)
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

                int res = DeleteRegions(sysflag, uid, ids);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "删除活动区域成功！", "1");
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

        private int DeleteRegions(string sysflag, string uid, string ids)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@ids", ids);
                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("X80Proc_Region_Delete"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseResult getRegionCarsList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string rid;
            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["uid"];
                rid = inparams["rid"];

                DataTable[] dtArr = getRegionCarList(sysflag, uid, rid);
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

        private DataTable[] getRegionCarList(string sysflag, string uid, string rid)
        {
            DataTable[] list = new DataTable[2];
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@Uid", uid);
                oaPara[1] = new SqlParameter("@RegionId", rid);
                DataSet ds = oSqlUtil.FillDataSet(sysflag, WebProc.Proc("X80Proc_RegionCar_List"), oaPara, "regioncartable", 30);
                if (ds.Tables.Count == 2)
                {
                    list[0] = ds.Tables[0]; //活动区域已关联车辆
                    list[1] = ds.Tables[1]; //活动区域尚未关联车辆
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseResult SetRegionCar(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string rid;
            string tnos;

            try
            {
                sysflag = inparams["sysflag"];
                rid = inparams["rid"];
                uid = inparams["uid"];
                tnos = inparams["tnos"];

                int res = SaveRegionCar(sysflag, uid, rid, tnos);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "活动区域匹配车辆关系设置成功！", "1");
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

        private int SaveRegionCar(string sysflag, string uid, string rid, string tnos)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@RegionId", rid);
                oaPara[1] = new SqlParameter("@TNOS", tnos);
                oaPara[2] = new SqlParameter("@Uid", uid);

                return oSqlUtil.ExecuteSPNoQuery(sysflag, WebProc.Proc("X80Proc_RegionCar_Set"), oaPara, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}