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
    public class CarManage
    {
        /// <summary>
        /// 车辆列表查询
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getCarList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string sysuid;
            string carno;
            string cid;
            string cuid;
            string carownname;
            string line;
            if (!inparams.Keys.Contains("carno") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("line"))
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
                carno = inparams["carno"];
                cid = inparams["cid"];
                cuid = inparams["cuid"];
                carownname = inparams["carownname"];
                line = inparams["line"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];

                //调用存储过程查询车辆列表
                DataTable dt = GetCarListSearch(sysflag, sysuid, carno, cid, cuid, carownname, line);
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

        private DataTable GetCarListSearch(string sysflag, string sysuid, string carno, string cid, string cuid, string carownname, string line)
        {
            DataTable dt = new DataTable();

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CarNo", carno), 
                                            new SqlParameter("@Cid", cid), 
                                            new SqlParameter("@Cuid", cuid), 
                                            new SqlParameter("@CarOwnName", carownname), 
                                            new SqlParameter("@LineName", line), 
                                            new SqlParameter("@U_ID", sysuid)
                                        };
                dt = csh.FillDataSet(sysflag, WebProc.Proc("QWProc_Wm_Car_Select0"), Parameters, null, 3600).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public ResponseResult getOneCar(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string cid;
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysuid"))
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

                cid = inparams["cid"];
                sysflag = inparams["sysflag"];

                DataTable[] dtArr = GetOneCarSearch(sysflag, cid);
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

        private DataTable[] GetOneCarSearch(string sysflag, string cid)
        {
            DataTable[] dtArr = new DataTable[7];

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid)
                                        };
                DataSet ds = csh.FillDataSet(sysflag, WebProc.Proc("QWProc_Wm_Car_InitData_Edit0"), Parameters, null, 3600);
                if (ds.Tables.Count == 7)
                {
                    dtArr[0] = ds.Tables[0];
                    dtArr[1] = ds.Tables[1];
                    dtArr[2] = ds.Tables[2];
                    dtArr[3] = ds.Tables[3];
                    dtArr[4] = ds.Tables[4];
                    dtArr[5] = ds.Tables[5];
                    dtArr[6] = ds.Tables[6];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtArr;
        }


        public ResponseResult getAllCarPlaceLevel(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string cid;
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid"))
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

                sysflag = inparams["sysflag"];

                DataTable dt = GetPlaceLevelList(sysflag);
                int Total = dt.Rows.Count; ;
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

        private DataTable GetPlaceLevelList(string sysflag)
        {
            DataTable dt = new DataTable();

            try
            {
                ComSqlHelper csh = new ComSqlHelper();

                dt = csh.FillDataSet(sysflag, WebProc.Proc("QWProc_Wm_GetAllCarPlaceLevel"), null, null, 3600).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }


        /// <summary>
        /// 编辑保存
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult SaveCarInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;                 //数据库标识
            string cid;                     //车辆CID 
            string carno;                   //车牌号
            string licenseplatecolor;       //车牌颜色
            string carnoremark;             //车牌号备注
            string fdjh;                    //发动机号
            string dph;                     //底盘号
            string energytypepkey;          //发动机类型
            string ctid;                    //车辆类型
            string cuid;                    //车辆用途
            string carcolor;                //车辆颜色
            string cgroup;                  //车辆组
            string carnationality;          //车籍地编码
            string vehiclenationality;      //车籍地编码
            string transtype;               //行业编码
            string vehicletype;             //809车型
            string cardname;                //司机姓名
            string cardno;                  //驾驶证号
            string cardbz;                  //司机备注
            string carda;                   //司机住址
            string cardt;                   //司机联系电话
            string carownname;              //车主名称
            string carownadd;               //车主地址
            string carowntel;               //车主联系电话
            string carownpas;               //车主密码
            string owersid;                 //许可证号
            string sysuid;                  //操作者uid

            if (!inparams.Keys.Contains("carno") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysuid"))
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
                sysflag = inparams["sysflag"];                 //数据库标识
                cid = inparams["cid"];                     //车辆CID 
                carno = inparams["carno"];                   //车牌号
                licenseplatecolor = inparams["licenseplatecolor"];       //车牌颜色
                carnoremark = inparams["carnoremark"];             //车牌号备注
                fdjh = inparams["fdjh"];                    //发动机号
                dph = inparams["dph"];                     //底盘号
                energytypepkey = inparams["energytypepkey"];          //发动机类型
                ctid = inparams["ctid"];                    //车辆类型
                cuid = inparams["cuid"];                    //车辆用途
                carcolor = inparams["carcolor"];                //车辆颜色
                cgroup = inparams["cgroup"];                  //车辆组
                carnationality = inparams["carnationality"];          //车籍地编码
                vehiclenationality = inparams["vehiclenationality"];      //车籍地编码
                transtype = inparams["transtype"];               //行业编码
                vehicletype = inparams["vehicletype"];             //809车型
                cardname = inparams["cardname"];                //司机姓名
                cardno = inparams["cardno"];                  //驾驶证号
                cardbz = inparams["cardbz"];                  //司机备注
                carda = inparams["carda"];                   //司机住址
                cardt = inparams["cardt"];                   //司机联系电话
                carownname = inparams["carownname"];              //车主名称
                carownadd = inparams["carownadd"];               //车主地址
                carowntel = inparams["carowntel"];               //车主联系电话
                carownpas = inparams["carownpas"];               //车主密码
                owersid = inparams["owersid"];                 //许可证号
                sysuid = inparams["sysuid"];                  //操作者uid
                int judgeflag = EditJudge(sysflag, cid, carno, dph, fdjh, cardno);

                if (judgeflag == 0)
                {
                    //保存
                    int res = SaveInfo(sysflag, cid, carno, licenseplatecolor, carnoremark, fdjh, dph, energytypepkey, ctid, cuid, carcolor, cgroup, carnationality, vehiclenationality, transtype, vehicletype, cardname, cardno, cardbz, carda, cardt, carownname, carownadd, carowntel, carownpas, owersid, sysuid);
                    if (res > 0)
                    {
                        Result = new ResponseResult(ResState.Success, "成功保存车辆信息！", "");
                    }
                }
                else
                {
                    if (judgeflag == 1)
                    {
                        Result = new ResponseResult(ResState.OperationFailed, "车牌号不能重复！", "");
                        return Result;
                    }

                    if (judgeflag == 2)
                    {
                        Result = new ResponseResult(ResState.OperationFailed, "底盘号不能重复！", "");
                        return Result;
                    }

                    if (judgeflag == 3)
                    {
                        Result = new ResponseResult(ResState.OperationFailed, "发动机号不能重复！", "");
                        return Result;
                    }

                    if (judgeflag == 4)
                    {
                        Result = new ResponseResult(ResState.OperationFailed, "驾驶证号不能重复！", "");
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

        private int SaveInfo(string sysflag, string cid, string carno, string licenseplatecolor, string carnoremark, string fdjh, string dph, string energytypepkey, string ctid, string cuid, string carcolor, string cgroup, string carnationality, string vehiclenationality, string transtype, string vehicletype, string cardname, string cardno, string cardbz, string carda, string cardt, string carownname, string carownadd, string carowntel, string carownpas, string owersid, string sysuid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID",cid), 
                                            new SqlParameter("@CarNo",carno), 
                                            new SqlParameter("@LicensePlateColor",licenseplatecolor),
                                            new SqlParameter("@CarNoRemark",carnoremark),
                                            new SqlParameter("@FDJH",fdjh),
                                            new SqlParameter("@DPH",dph), 
                                            new SqlParameter("@EnergyTypePKey",energytypepkey),
                                            new SqlParameter("@CTID",ctid),
                                            new SqlParameter("@CUID",cuid),
                                            new SqlParameter("@CarColor",carcolor),
                                            new SqlParameter("@CGroup",cgroup),
                                            new SqlParameter("@CarNationality",carnationality),
                                            new SqlParameter("@VehicleNationality",vehiclenationality),
                                            new SqlParameter("@TransType",transtype),
                                            new SqlParameter("@VehicleType",vehicletype),
                                            new SqlParameter("@CarDName",cardname),
                                            new SqlParameter("@CarDNo", cardno),
                                            new SqlParameter("@CarDBZ",cardbz),
                                            new SqlParameter("@CarDA",carda),
                                            new SqlParameter("@CarDT",cardt),
                                            new SqlParameter("@CarOwnName",carownname),
                                            new SqlParameter("@CarOwnAdd",carownadd),
                                            new SqlParameter("@CarOwnTel",carowntel),
                                            new SqlParameter("@CarOwnPas",carownpas),
                                            new SqlParameter("@OwersID",owersid),
                                            new SqlParameter("@USER_ID",sysuid)
                                        };
                res = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("QWProc_Wm_Car_Edit_forLog0"), Parameters, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        private int EditJudge(string sysflag, string cid, string carno, string dph, string fdjh, string cardno)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid), 
                                            new SqlParameter("@CarNo", carno), 
                                            new SqlParameter("@DPH", dph), 
                                            new SqlParameter("@FDJH", fdjh), 
                                            new SqlParameter("@CarDNo", cardno)
                                        };
                res = Convert.ToInt32(csh.ExecuteSPScalar(sysflag, WebProc.Proc("QWProc_Wm_Car_Edit_Judge"), Parameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }
    }
}