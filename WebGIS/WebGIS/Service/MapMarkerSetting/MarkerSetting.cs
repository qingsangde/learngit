using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary;
using System.Data.SqlClient;
using WebGIS;
using System.Data;

namespace SysService
{
    public class MarkerSetting
    {

        public ResponseResult getMarkerList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string SMarkName;
            string sysuid;
            try
            {
                sysflag = inparams["sysflag"];
                SMarkName = inparams["SMarkName"];
                sysuid = inparams["sysuid"];

                DataTable dt = getList(sysflag, SMarkName, sysuid);
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
 

        public ResponseResult CreateMarker(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string sysuid;
            string RowId;
            string OpType;
            string M_Name;
            string M_Lat;
            string M_Lng;
            string M_Desc;
            string cuser;
            string upuser;
            string DealerCode;
            string pramDealerCode;
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                sysflag = inparams["sysflag"];
                sysuid = inparams["sysuid"];     
                pramDealerCode = inparams["DealerCode"];
                RowId = inparams["RowId"];
                OpType = inparams["OpType"];
                M_Name = inparams["M_Name"];
                M_Lat = inparams["M_Lat"];
                M_Lng = inparams["M_Lng"];
                M_Desc = inparams["M_Desc"];
                cuser = inparams["cuser"];
                upuser = inparams["upuser"];

                DataTable dt = getDealerCode(sysflag, sysuid);
                DealerCode = dt.Rows[0][2].ToString();
                if (DealerCode.Equals(""))
                {
                    Result = new ResponseResult(ResState.OperationFailed, "登录用户不是经销商！请核对用户名！", "0");
                    return Result;
                }

                if (OpType.Equals("Edit")) {        //编辑时要校验经销商代码
                    if (!DealerCode.Equals(pramDealerCode)) {
                        Result = new ResponseResult(ResState.OperationFailed, "登录用户不具有修改权限！", "0");
                        return Result;
                    }
                }

                int res = InsertMarker(sysflag, RowId, DealerCode, OpType, M_Name, M_Lat, M_Lng, M_Desc, cuser, upuser);

                if (res >= 0)
                {
                    Result = new ResponseResult(ResState.Success, "操作成功！", "1");
                }
                else
                {
                    Result = new ResponseResult(ResState.OperationFailed, "该经销商已存在标注数据！请选择修改操作！", "0");
                }
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }

        public ResponseResult DelMarker(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string sysflag;
                string sysuid;
                string Ids;
                string pramDealerCodes;
                sysflag = inparams["sysflag"];
                sysuid = inparams["sysuid"]; 
                Ids = inparams["ids"];
                pramDealerCodes = inparams["DealerCodes"];

                string DealerCode;
                DataTable dt = getDealerCode(sysflag, sysuid);
                DealerCode = dt.Rows[0][2].ToString();

                if (!DealerCode.Equals(pramDealerCodes))
                {
                    Result = new ResponseResult(ResState.OperationFailed, "登录用户不具有删除权限！", "0");
                    return Result;
                }


                int res = DeleteMarker(sysflag, Ids, pramDealerCodes);
                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "删除电子围栏成功！", "1");
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
        /// 获取地图标注数据列表
        /// </summary>
        /// <param name="key">数据库版本</param>
        /// <returns></returns>
        public DataTable getList(string key, string SMarkName, string sysuid)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@SMarkName", SMarkName);
                oaPara[1] = new SqlParameter("@Uid", sysuid);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, WebProc.Proc("X80Proc_Marker_List_Select_New"), oaPara, "fencetable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 获取DealerCode
        /// </summary>
        /// <param name="key">数据库版本</param>
        /// <returns></returns>
        public DataTable getDealerCode(string key, string Uid)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@UID", Uid);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, WebProc.Proc("X80Proc_GetDealerCodeByUid"), oaPara, "fencetable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 添加地图标注
        /// </summary>
        /// <param name="key"></param>
        /// <param name="DealerCode"></param>
        /// <param name="OpType"></param>
        /// <param name="M_Name"></param>
        /// <param name="M_Lat"></param>
        /// <param name="M_Lng"></param>
        /// <param name="M_Desc"></param>
        /// <param name="cuser"></param>
        /// <param name="upuser"></param>
        /// <returns></returns>
        public int InsertMarker(string key, string RowId, string DealerCode, string OpType, string M_Name, string M_Lat, string M_Lng, string M_Desc, string cuser, string upuser)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[9];
                oaPara[0] = new SqlParameter("@DEALERCODE", DealerCode);
                oaPara[1] = new SqlParameter("@Id", RowId);
                oaPara[2] = new SqlParameter("@OpType", OpType);
                oaPara[3] = new SqlParameter("@M_Name", M_Name);
                oaPara[4] = new SqlParameter("@M_Lat", decimal.Parse(M_Lat));
                oaPara[5] = new SqlParameter("@M_Lng", decimal.Parse(M_Lng));
                oaPara[6] = new SqlParameter("@M_Desc", M_Desc);
                oaPara[7] = new SqlParameter("@Create_User", Int32.Parse(cuser));
                oaPara[8] = new SqlParameter("@Update_User", Int32.Parse(upuser));

                return oSqlUtil.ExecuteSPNoQuery(key, WebProc.Proc("X80Proc_Marker_Dealer"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteMarker(string key, string ids, string DealerCodes)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@Id", Int32.Parse(ids));

                return oSqlUtil.ExecuteSPNoQuery(key, WebProc.Proc("X80Proc_Marker_Delete"), oaPara, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}