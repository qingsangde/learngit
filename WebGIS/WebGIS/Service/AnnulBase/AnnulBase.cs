using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CommLibrary;
using System.Text;
using System.Data.SqlClient;

namespace WebGIS.Service
{
    public class AnnulBase
    {
        #region 定义

        /// <summary>
        /// 车辆ID
        /// </summary>
        string _carId = string.Empty;

        /// <summary>
        /// 经销商角色
        /// </summary>
        private const string R_JXS = "25";

        /// <summary>
        /// 启明角色
        /// </summary>
        private const string R_QM = "26";

        #endregion 定义
        #region 初始化
        #endregion 初始化
        #region 事件

        /// <summary>
        /// 基础信息列表查询事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getAnnulBaseList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string sysuid;
            string userrole;
            string carno;
            string carvin;
            string simCode;
            string tno;
            string loanOrg;
            string status;
            string repaySDate;
            string repayEDate;
            string serviceSDate;
            string serviceEDate;

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("userrole") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("carno") ||
                !inparams.Keys.Contains("carvin") || !inparams.Keys.Contains("simCode") || !inparams.Keys.Contains("tno") ||
                !inparams.Keys.Contains("loanOrg") || !inparams.Keys.Contains("status") || !inparams.Keys.Contains("repaySDate") ||
                !inparams.Keys.Contains("repayEDate") || !inparams.Keys.Contains("serviceSDate") || !inparams.Keys.Contains("serviceEDate"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                userrole = inparams["userrole"];
                carno = inparams["carno"];
                carvin = inparams["carvin"];
                simCode = inparams["simCode"];
                tno = inparams["tno"];
                loanOrg = inparams["loanOrg"];
                status = inparams["status"];
                repaySDate = inparams["repaySDate"];
                repayEDate = inparams["repayEDate"];
                serviceSDate = inparams["serviceSDate"];
                serviceEDate = inparams["serviceEDate"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];

                //调用方法取得数据表格
                DataTable dt = GetSelectData(sysflag, userrole, sysuid, carno, carvin, simCode, tno, loanOrg, status, repaySDate, repayEDate, serviceSDate, serviceEDate);

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

        /// <summary>
        /// 编辑页初始化
        /// </summary>
        /// <param name="inparams">参数</param>
        /// <returns>查询结果</returns>
        public ResponseResult getEditMessageByCid(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string cid;

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysuid"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                cid = inparams["cid"];
                sysflag = inparams["sysflag"];

                //取得编辑行数据表格
                DataTable[] dtArr = GetBaseInfoByCid(sysflag, cid);
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

        /// <summary>
        /// 初始化时生成下拉框
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getAllCmbOnLoad(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                sysflag = inparams["sysflag"];

                //调用方法取得下拉框数据表格
                DataTable[] dtArr = GetAllCmb(sysflag);
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

        /// <summary>
        /// 根据sim卡查询赋值
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult searchBySim(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string simcode;
            string sysuid;

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("simcode"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                sysflag = inparams["sysflag"];
                simcode = inparams["simcode"];
                sysuid = inparams["sysuid"];

                //调用方法取得下拉框数据表格
                DataTable[] dtArr = GetSearchBySim(sysflag, simcode, sysuid);
                int Total = dtArr[0].Rows.Count;
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

        /// <summary>
        /// sim卡改变生成下拉框
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getSimCmb(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string simcode;
            string sysuid;

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("simcode"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                sysflag = inparams["sysflag"];
                simcode = inparams["simcode"];
                sysuid = inparams["sysuid"];

                //调用方法取得下拉框数据表格
                DataTable dt = GetSimCmb(sysflag, simcode, sysuid);
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

        /// <summary>
        /// 新增保存事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult addSaveBaseInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;                 //数据库标识
            string sysuid;                  //操作者uid
            string cid;                     //车辆CID 
            string carno;                   //车牌号
            string manufacturer;            //生产厂家
            string customerType;            //客户类型
            string customerName;            //客户名称
            string contactman;              //联系人
            string contactnumber;           //联系电话
            string contactaddress;          //联系地址
            string contractno;              //贷款合同号
            string orgno;                   //机构编号
            string orgnoname;               //机构名
            string lenddate;                //放款开始日期
            string lendenddate;             //放款结束日期
            string lendPeriods;             //贷款期数
            string paymentaccount;          //到期还款金额
            string paymentDueDay;           //到期还款日

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("carno") || !inparams.Keys.Contains("manufacturer")
                || !inparams.Keys.Contains("customerType") || !inparams.Keys.Contains("customerName") || !inparams.Keys.Contains("contactman") || !inparams.Keys.Contains("contactnumber")
                || !inparams.Keys.Contains("contactnumber") || !inparams.Keys.Contains("contactaddress") || !inparams.Keys.Contains("contractno") || !inparams.Keys.Contains("orgno")
                || !inparams.Keys.Contains("orgnoname") || !inparams.Keys.Contains("lenddate") || !inparams.Keys.Contains("lendenddate") || !inparams.Keys.Contains("lendPeriods")
                || !inparams.Keys.Contains("paymentaccount") || !inparams.Keys.Contains("paymentDueDay"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }

            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                sysflag = inparams["sysflag"];                           //数据库标识
                cid = inparams["cid"];                                   //车辆CID 
                carno = inparams["carno"];                               //车牌号码
                manufacturer = inparams["manufacturer"];                 //生产厂家
                customerType = inparams["customerType"];                 //客户类型
                customerName = inparams["customerName"];                 //客户名称
                contactman = inparams["contactman"];                     //联系人
                contactnumber = inparams["contactnumber"];               //联系电话
                contactaddress = inparams["contactaddress"];             //联系地址
                contractno = inparams["contractno"];                     //贷款合同号
                orgno = inparams["orgno"];                               //机构编号
                orgnoname = inparams["orgnoname"];                       //机构名
                lenddate = inparams["lenddate"];                         //放款开始日期
                lendenddate = inparams["lendenddate"];                   //放款结束日期
                lendPeriods = inparams["lendPeriods"];                   //贷款期数
                paymentaccount = inparams["paymentaccount"];             //到期还款金额
                paymentDueDay = inparams["paymentDueDay"];               //到期还款日
                sysuid = inparams["sysuid"];                             //操作者uid

                //验证车辆ID是否重复
                int judgeflag = AddJudge(sysflag, cid);

                //验证机构是否存在
                if (string.IsNullOrEmpty(orgno) && !string.IsNullOrEmpty(orgnoname))
                {
                    int res = OrgJudge(sysflag, orgnoname);
                    if (res == 0)
                    {
                        //新增机构
                        orgno = AddOrg(sysflag, orgnoname, sysuid).ToString();
                    }
                    else
                    {
                        orgno = res.ToString();
                    }
                }

                //判断AL_Car和AL_LoanInfo主键是否重复
                if (judgeflag == 0)
                {

                    //保存
                    int res = AddSaveInfoByJXS(sysflag, cid, carno, manufacturer, customerType, customerName, contactman, contactnumber,
                        contactaddress, contractno, orgno, lenddate, lendenddate, lendPeriods, paymentaccount, paymentDueDay, sysuid);
                    if (res > 0)
                    {
                        Result = new ResponseResult(ResState.Success, "成功保存车辆信息！", "");
                    }
                }
                else
                {
                    if (judgeflag == 1)
                    {
                        Result = new ResponseResult(ResState.OperationFailed, "已有该车辆的销售贷款信息！", "");
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

        /// <summary>
        /// 修改保存事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult editSaveBaseInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;                 //数据库标识
            string sysuid;                  //操作者uid
            string cid;                     //车辆CID 
            string carno;                   //车牌号
            string manufacturer;            //生产厂家
            string customerType;            //客户类型
            string customerName;            //客户名称
            string contactman;              //联系人
            string contactnumber;           //联系电话
            string contactaddress;          //联系地址
            string contractno;              //贷款合同号
            string orgno;                   //机构编号
            string orgnoname;               //机构名
            string lenddate;                //放款开始日期
            string lendenddate;             //放款结束日期
            string lendPeriods;             //贷款期数
            string paymentaccount;          //到期还款金额
            string paymentDueDay;           //到期还款日
            string servicesday;             //服务有效期开始日期
            string serviceeday;             //服务有效期结束日期

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("carno") || !inparams.Keys.Contains("manufacturer")
                || !inparams.Keys.Contains( "customerType") || !inparams.Keys.Contains("customerName") || !inparams.Keys.Contains("contactman") || !inparams.Keys.Contains("contactnumber")
                || !inparams.Keys.Contains("contactnumber") || !inparams.Keys.Contains("contactaddress") || !inparams.Keys.Contains("contractno") || !inparams.Keys.Contains("orgno")
                || !inparams.Keys.Contains("orgnoname") || !inparams.Keys.Contains("lenddate") || !inparams.Keys.Contains("lendenddate") || !inparams.Keys.Contains("lendPeriods")
                || !inparams.Keys.Contains("paymentaccount") || !inparams.Keys.Contains("paymentDueDay") || !inparams.Keys.Contains("servicesday") || !inparams.Keys.Contains("serviceeday"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }

            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                sysflag = inparams["sysflag"];                           //数据库标识
                cid = inparams["cid"];                                   //车辆CID 
                carno = inparams["carno"];                               //车牌号码 
                manufacturer = inparams["manufacturer"];                 //生产厂家
                customerType = inparams["customerType"];                 //客户类型
                customerName = inparams["customerName"];                 //客户名称
                contactman = inparams["contactman"];                     //联系人
                contactnumber = inparams["contactnumber"];               //联系电话
                contactaddress = inparams["contactaddress"];             //联系地址
                contractno = inparams["contractno"];                     //贷款合同号
                orgno = inparams["orgno"];                               //机构编号
                orgnoname = inparams["orgnoname"];                       //机构名
                lenddate = inparams["lenddate"];                         //放款开始日期
                lendenddate = inparams["lendenddate"];                   //放款结束日期
                lendPeriods = inparams["lendPeriods"];                   //贷款期数
                paymentaccount = inparams["paymentaccount"];             //到期还款金额
                paymentDueDay = inparams["paymentDueDay"];               //到期还款日
                servicesday = inparams["servicesday"];                   //服务有效期开始日期
                serviceeday = inparams["serviceeday"];                   //服务有效期结束日期
                sysuid = inparams["sysuid"];                             //操作者uid

                //int judgeflag = EditJudge(sysflag, cid, carno, carvin, fdjh, null);
                //验证机构是否存在
                if (string.IsNullOrEmpty(orgno) && !string.IsNullOrEmpty(orgnoname))
                {
                    int res = OrgJudge(sysflag, orgnoname);
                    if (res == 0)
                    {
                        //新增机构
                        orgno = AddOrg(sysflag, orgnoname, sysuid).ToString();
                    }
                    else
                    {
                        orgno = res.ToString();
                    }
                }
                //经销商的情况
                if (R_JXS.Equals(inparams["userrole"]))
                {
                    int isChange = IsCarChange(sysflag, cid, carno);

                    //保存
                    int res = EditSaveInfoByJXS(sysflag, cid, carno, manufacturer, customerType, customerName, contactman, contactnumber,
                        contactaddress, contractno, orgno, lenddate, lendenddate, lendPeriods, paymentaccount, paymentDueDay, sysuid);
                    if (res > 0)
                    {
                        Result = new ResponseResult(ResState.Success, "成功保存车辆信息！", "");
                    }

                    //校验车牌号码是否更改，如果更改，插入QS_CHANGE_VEHICLE_LOG
                    if (isChange == 1)
                    {
                        int ret = InsertQSChange(sysflag, cid, 1, sysuid);
                    }
                }
                //启明的情况
                else if (R_QM.Equals(inparams["userrole"]))
                {

                    //保存
                    int res = EditSaveInfoByQM(sysflag, cid, servicesday, serviceeday, sysuid);
                    if (res > 0)
                    {
                        Result = new ResponseResult(ResState.Success, "成功保存服务有效期信息！", "");
                    }
                }

            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        /// <summary>
        /// 导出事件
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult doExport(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string sysuid;
            string userrole;
            string carno;
            string carvin;
            string simCode;
            string tno;
            string loanOrg;
            string status;
            string repaySDate;
            string repayEDate;
            string serviceSDate;
            string serviceEDate;

            //判断参数
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("userrole") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("carno") ||
                !inparams.Keys.Contains("carvin") || !inparams.Keys.Contains("simCode") || !inparams.Keys.Contains("tno") ||
                !inparams.Keys.Contains("loanOrg") || !inparams.Keys.Contains("status") || !inparams.Keys.Contains("repaySDate") ||
                !inparams.Keys.Contains("repayEDate") || !inparams.Keys.Contains("serviceSDate") || !inparams.Keys.Contains("serviceEDate"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                //系统标识为空，提示信息
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                userrole = inparams["userrole"];
                carno = inparams["carno"];
                carvin = inparams["carvin"];
                simCode = inparams["simCode"];
                tno = inparams["tno"];
                loanOrg = inparams["loanOrg"];
                status = inparams["status"];
                repaySDate = inparams["repaySDate"];
                repayEDate = inparams["repayEDate"];
                serviceSDate = inparams["serviceSDate"];
                serviceEDate = inparams["serviceEDate"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];

                //调用方法取得数据表格
                DataTable contentData = GetSelectData(sysflag, userrole, sysuid, carno, carvin, simCode, tno, loanOrg, status, repaySDate, repayEDate, serviceSDate, serviceEDate);

                //页面没有数据的情况，提示信息
                if (contentData.Rows.Count == 0)
                {
                    Result = new ResponseResult(ResState.OperationFailed, "未检索到数据,导出失败！", "");
                }
                else
                {
                    //移除车辆ID列和锁车状态code列
                    contentData.Columns.Remove("CID");
                    contentData.Columns.Remove("LockStatusCode");
                    NPOIHelper npoiHelper = new NPOIHelper();
                    string[] headerDataArray = { "车牌号", "VIN", "SIM卡号", "终端ID", "服务有效期止日", "到期还款日" ,
                                           
                                                "到期还款金额", "放款机构编号","还款状态" ,"逾期天数", "锁车状态"
                                           };
                    string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                    npoiHelper.WorkbookName = "信息维护表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    // 设置导入内容
                    npoiHelper.HeaderData = headerDataArray;
                    npoiHelper.ContentData = contentDataArray;
                    string basepath = HttpRuntime.AppDomainAppPath.ToString();
                    string filePath = @"UI\Excel\Query\";
                    string sd = basepath + filePath;
                    npoiHelper.saveExcel(sd);

                    Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                }
                return Result;
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OtherError, "", ex.Message + ex.StackTrace);
                return Result;
            }
        }

        #endregion 事件

        #region 方法

        /// <summary>
        /// 根据车辆ID取得编辑页dataTable
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        private DataTable[] GetBaseInfoByCid(string sysflag, string cid)
        {
            DataTable[] dtArr = new DataTable[4];

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid)
                                        };
                //调用存储过程取得DataSet
                DataSet ds = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_QueryAllByCid"), Parameters, null, 3600);
                if (ds.Tables.Count == 3)
                {
                    dtArr[0] = ds.Tables[0];
                    dtArr[1] = ds.Tables[1];
                    dtArr[2] = ds.Tables[2];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtArr;
        }

        /// <summary>
        /// 根据simcode查询方法
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="cid"></param>
        /// <returns></returns> 
        private DataTable[] GetSearchBySim(string sysflag, string simcode, string sysuid)
        {
            DataTable[] dtArr = new DataTable[1];

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@SimCode", simcode),
                                            new SqlParameter("@sysuid",sysuid)
                                        };
                //调用存储过程取得DataSet
                DataSet ds = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_QueryCarInfoBySim"), Parameters, null, 3600);
                dtArr[0] = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtArr;
        }

        /// <summary>
        /// 根据simcode取得下拉框数据
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="cid"></param>
        /// <returns></returns> 
        private DataTable GetSimCmb(string sysflag, string simcode, string sysuid)
        {
            DataTable dt = new DataTable();

            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@SimCode", simcode),
                                            new SqlParameter("@sysuid",sysuid)
                                        };
                //调用存储过程取得DataSet
                DataSet ds = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_GetSimCmb"), Parameters, null, 3600);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }


        /// <summary>
        /// 取得页面下拉框数据表格
        /// </summary>
        /// <param name="sysflag">sysflag</param>
        /// <returns>DataTable</returns>
        private DataTable[] GetAllCmb(string sysflag)
        {
            DataTable[] dtArr = new DataTable[4];

            try
            {
                ComSqlHelper csh = new ComSqlHelper();

                //调用存储过程取得DataSet
                DataSet ds = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_GetCombox"), null, null, 3600);
                if (ds.Tables.Count == 4)
                {
                    dtArr[0] = ds.Tables[0];
                    dtArr[1] = ds.Tables[1];
                    dtArr[2] = ds.Tables[2];
                    dtArr[3] = ds.Tables[3];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtArr;
        }

        /// <summary>
        /// 校验车辆信息是否更改
        /// </summary>
        /// <param name="cid">车辆ID</param>
        /// <param name="carno">车牌号</param>
        /// <returns>0：未更改；1：更改</returns>
        private int IsCarChange(string sysflag, string cid, string carno)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid), 
                                            new SqlParameter("@CarNo", carno)
                                        };
                res = Convert.ToInt32(csh.ExecuteSPScalar(sysflag, WebProc.Proc("ALProc_IsCarChange"), Parameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        /// <summary>
        ///  插入QS_CHANGE_VEHICLE_LOG
        /// </summary>
        /// <param name="sysflag">数据库标识</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="flag">1：修改或新增；2：删除</param>
        /// <param name="uid">用户ID</param>
        /// <returns>0：失败；1：成功</returns>
        private int InsertQSChange(string sysflag, string cid, int flag, string uid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid), 
                                            new SqlParameter("@FLAG", flag), 
                                            new SqlParameter("@UserId", uid)
                                        };
                res = Convert.ToInt32(csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_InsertQSChange"), Parameters,false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        /// <summary>
        /// 经销商新增保存方法
        /// </summary>
        /// <returns>更新结果 0:失败;非0:成功</returns>
        private int AddSaveInfoByJXS(string sysflag, string cid, string carno, string manufacturer, string customerType, string customerName, string contactman, string contactnumber, string contactaddress,
           string contractno, string orgno, string lenddate, string lendenddate, string lendPeriods, string paymentaccount, string paymentDueDay, string sysuid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID",cid), 
                                            new SqlParameter("@CarNo",carno), 
                                            new SqlParameter("@Manufacturer",manufacturer),
                                            new SqlParameter("@CustomerType",customerType),
                                            new SqlParameter("@CustomerName",customerName),
                                            new SqlParameter("@ContactMan",contactman),
                                            new SqlParameter("@ContactNumber",contactnumber),
                                            new SqlParameter("@ContactAddress",contactaddress),
                                            new SqlParameter("@LoanContractNo",contractno),
                                            new SqlParameter("@OrgNo",orgno),
                                            new SqlParameter("@LendDate", lenddate),
                                            new SqlParameter("@LendEndDate", lendenddate),
                                            new SqlParameter("@LendPeriods",lendPeriods),
                                            new SqlParameter("@PaymentAccount",paymentaccount),
                                            new SqlParameter("@PaymentDueDay",paymentDueDay),
                                            new SqlParameter("@UserId",sysuid)
                                        };
                res = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_InsertBaseInfo"), Parameters, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }


        /// <summary>
        /// 经销商修改保存方法
        /// </summary>
        /// <returns>更新结果 0:失败;非0:成功</returns>
        private int EditSaveInfoByJXS(string sysflag, string cid, string carno, string manufacturer, string customerType, string customerName, string contactman,string contactnumber, 
            string contactaddress, string contractno, string orgno, string lenddate, string lendenddate, string lendPeriods, string paymentaccount, string paymentDueDay, string sysuid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID",cid), 
                                            new SqlParameter("@CarNo",carno),
                                            new SqlParameter("@Manufacturer",manufacturer),
                                            new SqlParameter("@CustomerType",customerType),
                                            new SqlParameter("@CustomerName",customerName),
                                            new SqlParameter("@ContactMan",contactman),
                                            new SqlParameter("@ContactNumber",contactnumber),
                                            new SqlParameter("@ContactAddress",contactaddress),
                                            new SqlParameter("@LoanContractNo",contractno),
                                            new SqlParameter("@OrgNo",orgno),
                                            new SqlParameter("@LendDate", lenddate),
                                            new SqlParameter("@LendEndDate", lendenddate),
                                            new SqlParameter("@LendPeriods",lendPeriods),
                                            new SqlParameter("@PaymentAccount",paymentaccount),
                                            new SqlParameter("@PaymentDueDay",paymentDueDay),
                                            new SqlParameter("@UserId",sysuid)
                                        };
                res = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_UpdateBaseInfoByJXS"), Parameters, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// 启明修改保存方法
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">车辆id</param>
        /// <param name="servicesday">服务有效期开始日期</param>
        /// <param name="serviceeday">服务有效期结束日期</param>
        /// <param name="sysuid">操作ID</param>
        /// <returns>更新结果 0:失败;非0:成功</returns>
        private int EditSaveInfoByQM(string sysflag, string cid, string servicesday, string serviceeday, string sysuid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID",cid), 
                                            new SqlParameter("@ServiceSDay",servicesday),
                                            new SqlParameter("@ServiceEDay",serviceeday),
                                            new SqlParameter("@UserId",sysuid)
                                        };
                res = csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("ALProc_Update_ALCarByQM"), Parameters, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// 验证车牌号、底盘哈、发动机号是否重复
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">车辆id</param>
        /// <param name="carno">车牌号</param>
        /// <param name="dph">底盘号</param>
        /// <param name="fdjh">发动机号</param>
        /// <param name="cardno">驾驶证号</param>
        /// <returns>0:不重复;非0：重复</returns>
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

        /// <summary>
        /// 验证车辆ID是否重复
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">车辆id</param>
        /// <returns>0:不重复;1：重复</returns>
        private int AddJudge(string sysflag, string cid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid)
                                        };
                res = Convert.ToInt32(csh.ExecuteSPScalar(sysflag, WebProc.Proc("ALProc_LoanInfo_Car_AddJugde"), Parameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }
        /// <summary>
        /// 验证机构名是否存在
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">车辆id</param>
        /// <returns>0:不重复;1：重复</returns>
        private int OrgJudge(string sysflag, string orgnoname)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@OrgName", orgnoname)
                                        };
                res = Convert.ToInt32(csh.ExecuteSPScalar(sysflag, WebProc.Proc("ALProc_OrgJugde"), Parameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }


        /// <summary>
        /// 新增机构
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">机构名称</param>
        /// <returns>0:不重复;1：重复</returns>
        private int AddOrg(string sysflag, string orgnoname, string sysuid)
        {
            int res = 0;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@OrgName", orgnoname),
                                            new SqlParameter("@UserId", sysuid)
                                        };
                res = Convert.ToInt32(csh.ExecuteSPScalar(sysflag, WebProc.Proc("ALProc_InsertLoanOrg"), Parameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }


        /// <summary>
        /// 查询/导出方法
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSelectData(string sysflag, string userrole, string sysuid, string carno, string carvin, string simCode, string tno,
            string loanOrg, string status, string repaySDate, string repayEDate, string serviceSDate, string serviceEDate)
        {
            ComSqlHelper csh = new ComSqlHelper();
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            //用户角色是经销商的情况
            if (R_JXS.Equals(userrole))
            {
                SqlParameter[] Parameters = { 
                                            new SqlParameter("@sysuid", sysuid)
                                        };
                dt = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_QueryAllBaseInfoForJXS"), Parameters, null, 1800).Tables[0];
            }
            else
            {
                dt = csh.FillDataSet(sysflag, WebProc.Proc("ALProc_QueryAllBaseInfoForQM"), null, null, 1800).Tables[0];
            }

            //锁车状态
            if (!string.IsNullOrEmpty(status))
            {
                //为-1的情况
                if ("-1".Equals(status.Trim()))
                {
                    sb.Append("AND LockStatusCode  NOT IN ('0','1','2','3')");
                }
                else
                {
                    sb.Append("AND LockStatusCode =  '" + status.Trim() + "'");
                }
            }

            //车牌号
            if (!string.IsNullOrEmpty(carno))
            {
                sb.Append("AND CarNo like '%" + carno.Trim() + "%'");
            }
            //底盘号
            if (!string.IsNullOrEmpty(carvin))
            {
                sb.Append("AND DPH like '%" + carvin.Trim() + "%'");
            }
            //sim卡号
            if (!string.IsNullOrEmpty(simCode))
            {
                sb.Append("AND SimCode like '%" + simCode.Trim() + "%'");
            }
            //终端ID
            if (!string.IsNullOrEmpty(tno))
            {
                sb.Append("AND TNO like '%" + tno.Trim() + "%'");
            }
            //放款机构
            if (!string.IsNullOrEmpty(loanOrg))
            {
                sb.Append("AND OrgNo = '" + loanOrg.Trim() + "'");
            }

            //到期还款日开始日期
            if (!string.IsNullOrEmpty(repaySDate))
            {
                sb.Append(" AND PaymentDueDay  >= '" + Convert.ToDateTime(repaySDate) + "'");
            }
            //到期还款日结束日期
            if (!string.IsNullOrEmpty(repayEDate))
            {
                sb.Append(" AND PaymentDueDay  <= '" + Convert.ToDateTime(repayEDate) + "'");
            }
            //服务有效期止日开始日期
            if (!string.IsNullOrEmpty(serviceSDate))
            {
                sb.Append(" AND ServiceEDay  >= '" + Convert.ToDateTime(serviceSDate) + "'");
            }
            //服务有效期止日结束日期
            if (!string.IsNullOrEmpty(serviceEDate))
            {
                sb.Append(" AND ServiceEDay  <= '" + Convert.ToDateTime(serviceEDate) + "'");
            }

            if (sb.Length != 0)
            {
                sb.Remove(0, 4);
                DataView dv = dt.AsDataView();
                dv.RowFilter = sb.ToString();
                dt = dv.ToTable();
            }

            DataTable dtResult = new DataTable();
            //克隆表结构
            dtResult = dt.Clone();
            foreach (DataColumn col in dtResult.Columns)
            {
                if (col.ColumnName == "PaymentAccount" || col.ColumnName == "PaymentDueDay" || col.ColumnName == "ServiceEDay")
                {
                    //修改列类型
                    col.DataType = typeof(String);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                DataRow rowNew = dtResult.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {


                    //到期还款金额
                    if (dt.Columns[i].ColumnName == "PaymentAccount")
                    {
                        if (!Convert.IsDBNull(row["PaymentAccount"]))
                        {
                            rowNew["PaymentAccount"] = String.Format("{0:N}", row["PaymentAccount"]);
                        }
                    }
                    else
                        //到期还款日
                        if (dt.Columns[i].ColumnName == "PaymentDueDay")
                        {
                            if (!Convert.IsDBNull(row["PaymentDueDay"]))
                            {
                                rowNew["PaymentDueDay"] = Convert.ToDateTime(row["PaymentDueDay"]).ToString("yyyy-MM-dd");
                            }
                        }
                        else
                            //服务有效期止日
                            if (dt.Columns[i].ColumnName == "ServiceEDay")
                            {
                                if (!Convert.IsDBNull(row["ServiceEDay"]))
                                {
                                    rowNew["ServiceEDay"] = Convert.ToDateTime(row["ServiceEDay"]).ToString("yyyy-MM-dd");
                                }
                            }
                            else
                            {

                                rowNew[i] = row[i];
                            }

                }
                dtResult.Rows.Add(rowNew);
            }
            return dtResult;
        }

        #endregion 方法

    }
}