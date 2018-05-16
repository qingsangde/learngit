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
    public class Fence
    {
        public ResponseResult getFenceList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string fname;
            string almtype;
            string graphtype;
            try
            {
                sysflag = inparams["sysflag"];
                fname = inparams["fname"];
                almtype = inparams["almtype"];
                graphtype = inparams["graphtype"];

                DataTable dt = getList(sysflag, fname, almtype, graphtype);
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

        public ResponseResult AddEditFence(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string OpType;
            string fid; 
            string fname;
            string almtype;
            string gratype;
            string content;
            string desc;
            string cuser;
            string upuser;

            try
            {
                sysflag = inparams["sysflag"];
                OpType = inparams["OpType"];
                fid = inparams["fid"];
                fname = inparams["fname"];
                almtype = inparams["almtype"];
                gratype = inparams["gratype"];
                content = inparams["content"];
                desc = inparams["desc"];
                cuser = inparams["cuser"];
                upuser = inparams["upuser"];

                int res = InsertOrUpdateFence(sysflag,OpType,fid,fname,almtype,gratype,content,desc,cuser,upuser);

                if (res >= 0)
                {

                    Result = new ResponseResult(ResState.Success, "维护电子围栏成功！", "1");
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

        public ResponseResult DelFence(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string sysflag;
                string fids;
                sysflag = inparams["sysflag"];
                fids = inparams["fids"];
                int res = DeleteFence(sysflag, fids);
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
        /// 获取电子围栏数据列表
        /// </summary>
        /// <param name="key">数据库版本</param>
        /// <returns></returns>
        public DataTable getList(string key, string fname, string almtype, string graphtype)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@Fname", fname);
                oaPara[1] = new SqlParameter("@AlmType", almtype);
                oaPara[2] = new SqlParameter("@GraType", graphtype);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, WebProc.Proc("QSProc_QS_Fence_List_Select"), oaPara, "fencetable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 添加或更新电子围栏
        /// </summary>
        /// <param name="key"></param>
        /// <param name="OpType"></param>
        /// <param name="fid"></param>
        /// <param name="fname"></param>
        /// <param name="almtype"></param>
        /// <param name="gratype"></param>
        /// <param name="content"></param>
        /// <param name="desc"></param>
        /// <param name="cuser"></param>
        /// <param name="upuser"></param>
        /// <returns></returns>
        public int InsertOrUpdateFence(string key, string OpType, string fid, string fname, string almtype, string gratype, string content, string desc, string cuser, string upuser)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[9];
                oaPara[0] = new SqlParameter("@OpType", OpType);
                oaPara[1] = new SqlParameter("@F_ID", Int32.Parse(fid));
                oaPara[2] = new SqlParameter("@F_Name", fname);
                oaPara[3] = new SqlParameter("@F_AlarmType", Int32.Parse(almtype));
                oaPara[4] = new SqlParameter("@F_GraphType", Int32.Parse(gratype));
                oaPara[5] = new SqlParameter("@F_Content", content);
                oaPara[6] = new SqlParameter("@F_Desc", desc);
                oaPara[7] = new SqlParameter("@Create_User", Int32.Parse(cuser));
                oaPara[8] = new SqlParameter("@Update_User", Int32.Parse(upuser));

                return oSqlUtil.ExecuteSPNoQuery(key, WebProc.Proc("QSProc_QS_Fence_InsertOrUpdate"), oaPara, false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteFence(string key, string ids)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@ids", ids);


                return oSqlUtil.ExecuteSPNoQuery(key, WebProc.Proc("QSProc_QS_Fence_Delete"), oaPara, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}