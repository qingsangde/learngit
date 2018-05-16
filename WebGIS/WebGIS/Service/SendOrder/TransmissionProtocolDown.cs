using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;
using CommLibrary.Proto;
using System.Collections;
using CommLibrary;
using System.Data.SqlClient;

namespace SysService
{
    public class TransmissionProtocolDown
    {
        public ResponseResult sendXiaoDaiLockOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string tno = inparams["tno"];
                string LDataType = inparams["LDataType"];
                string carno = inparams["carno"];
                string cid = inparams["cid"];
                string paramvalue = inparams["paramvalue"];
                string rev = inparams["rev"];
                string nj = inparams["nj"];
                string lockreason = inparams["lockreason"];
                string unlockreason = inparams["unlockreason"];

                string orderDesc = "";
                string TransmissionDataStr = "";
                byte[] datas = new byte[6];
                datas[0] = 0x0;
                datas[1] = 0x7;
                switch (paramvalue)
                {
                    case "0xA8":

                        orderDesc = "立即锁车";
                        if (LDataType == "1" && rev != "")
                        {
                            nj = "0";
                            datas[2] = 0x1;
                        }
                        if (LDataType == "2" && nj != "")
                        {
                            rev = "0";
                            datas[2] = 0x3;
                        }

                        datas[4] = (byte)(uint.Parse(rev));
                        datas[3] = (byte)(uint.Parse(rev) >> 8);
                        datas[5] = (byte)(uint.Parse(nj));
                        break;
                    case "0x8A":
                        orderDesc = "立即解锁";
                        datas[2] = 0x6;
                        datas[4] = (byte)(uint.Parse(rev));
                        datas[3] = (byte)(uint.Parse(rev) >> 8);
                        datas[5] = (byte)(100);
                        //TransmissionDataStr = "0007" + "06" + uint.Parse(rev).ToString("X2");
                        break;

                }

                string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
                WebGIS.RealtimeDataServer.CarRealData res = new WebGIS.RealtimeDataServer.CarRealData();
                RDSConfigModel rc = RDSConfig.GetRDS(sysflag);
                WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
                //调用WebGIS实时数据服务接口查询车辆轨迹数据
                if (souflag == "RDS" && rc != null && rc.RunFlag)
                {
                    wc.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                    res = wc.GetCarData(sysflag, long.Parse(cid));
                    if (res != null)
                    {
                        if (res.OnlineStatus == 1)//在线，下发指令
                        {
                            SendResult sr = new SendResult();
                            sr.CID = cid;
                            sr.TNO = tno;
                            sr.CarNo = carno;
                            sr.Res = false;
                            sr.Desc = orderDesc;
                            sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //bool result = SendOrderHander.Send_CTS_TransmissionProtocol(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), 0x8F02, strToToHexByte(TransmissionDataStr));

                            bool result = SendOrderHander.Send_CTS_TransmissionProtocol(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), 0x8F02, datas);
                            sr.Res = result;
                            if (result)
                            {
                                //如果下发返回True，则写入数据库记录
                                UpdateJFQZExtend_Transmission(sysflag, long.Parse(cid), string.Empty, -1, -1, Convert.ToInt32(rev), lockreason, unlockreason);
                            }
                            Result = new ResponseResult(ResState.Success, "", sr);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发车辆锁定设置指令！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseResult(ResState.OtherError, "后台服务异常，无法下发车辆锁定设置指令！", null);
                    }
                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "后台服务停止运行，无法下发车辆锁定设置指令！", null);
                }


            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            //SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
            return Result;
        }

        public ResponseResult sendXiaoDaiActivateOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string tno = inparams["tno"];
                string carno = inparams["carno"];
                string cid = inparams["cid"];
                string vin = inparams["vin"];
                string vintype = inparams["vintype"];
                string energytype = inparams["energytype"];
                string orderDesc = "";
                string TransmissionDataStr = "";

                byte FDJType = Convert.ToByte(energytype == "" ? "0" : energytype);  //发动机类型（日后从数据库中由车辆信息取出）
                byte[] datas = new byte[4];
                datas[0] = 0x0;
                datas[1] = 0x6;

                if (vintype == "0xAA")
                {
                    orderDesc = "车辆销贷功能激活";
                    datas[2] = 0x1;
                    datas[3] = FDJType;
                    TransmissionDataStr = "0006" + "01" + FDJType.ToString();
                }
                else if (vintype == "0x55")
                {
                    orderDesc = "车辆销贷功能关闭";
                    datas[2] = 0x2;
                    datas[3] = FDJType;
                    TransmissionDataStr = "0006" + "02" + FDJType;
                }

                string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
                WebGIS.RealtimeDataServer.CarRealData res = new WebGIS.RealtimeDataServer.CarRealData();
                RDSConfigModel rc = RDSConfig.GetRDS(sysflag);
                WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
                //调用WebGIS实时数据服务接口查询车辆轨迹数据
                if (souflag == "RDS" && rc != null && rc.RunFlag)
                {
                    wc.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                    res = wc.GetCarData(sysflag, long.Parse(cid));
                    if (res != null)
                    {
                        if (res.OnlineStatus == 1)//在线，下发指令
                        {
                            SendResult sr = new SendResult();
                            sr.CID = cid;
                            sr.TNO = tno;
                            sr.CarNo = carno;
                            sr.Res = false;
                            sr.Desc = orderDesc;
                            sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            bool result = SendOrderHander.Send_CTS_TransmissionProtocol(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), 0x8F02, datas);
                            sr.Res = result;

                            Result = new ResponseResult(ResState.Success, "", sr);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发" + orderDesc + "指令！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发" + orderDesc + "指令！", null);
                    }
                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发" + orderDesc + "指令！", null);
                }


            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            //SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
            return Result;
        }

        public ResponseResult sendXiaoDaiParamOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string tno = inparams["tno"];
                string carno = inparams["carno"];
                string cid = inparams["cid"];
                string datevalue = inparams["datevalue"];
                string dayvalue = inparams["dayvalue"];
                string minutevalue = inparams["minutevalue"];

                string TransmissionDataStr = "0005";
                byte[] datas = new byte[9];
                datas[0] = 0x0;
                datas[1] = 0x5;

                if (!string.IsNullOrEmpty(datevalue))
                {

                    DateTime dt = DateTime.Parse(datevalue);

                    datas[2] = Convert.ToByte(toBCD(dt.Year % 100));
                    datas[3] = Convert.ToByte(toBCD(dt.Month));
                    datas[4] = Convert.ToByte(toBCD(dt.Day));
                    TransmissionDataStr += Convert.ToByte(toBCD(dt.Year % 100)).ToString() + Convert.ToByte(toBCD(dt.Month)).ToString() + Convert.ToByte(toBCD(dt.Day)).ToString();
                }
                else
                {
                    datas[2] = Convert.ToByte(toBCD(0));
                    datas[3] = Convert.ToByte(toBCD(0));
                    datas[4] = Convert.ToByte(toBCD(0));
                    TransmissionDataStr += Convert.ToByte(toBCD(0)).ToString() + Convert.ToByte(toBCD(0)).ToString() + Convert.ToByte(toBCD(0)).ToString(); ;
                }

                if (!string.IsNullOrEmpty(dayvalue))
                {
                    datas[6] = (byte)(uint.Parse(dayvalue));
                    datas[5] = (byte)(uint.Parse(dayvalue) >> 8);
                    TransmissionDataStr += uint.Parse(dayvalue).ToString("X2");
                }
                else
                {
                    datas[6] = (byte)(0);
                    datas[5] = (byte)(0 >> 8);
                    TransmissionDataStr += 0.ToString("X2");
                }

                if (!string.IsNullOrEmpty(minutevalue))
                {
                    datas[8] = (byte)(uint.Parse(minutevalue));
                    datas[7] = (byte)(uint.Parse(minutevalue) >> 8);
                    TransmissionDataStr += uint.Parse(minutevalue).ToString("X2");
                }
                else
                {
                    datas[8] = (byte)(0);
                    datas[7] = (byte)(0 >> 8);
                    TransmissionDataStr += 0.ToString("X2");
                }

                string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
                WebGIS.RealtimeDataServer.CarRealData res = new WebGIS.RealtimeDataServer.CarRealData();
                RDSConfigModel rc = RDSConfig.GetRDS(sysflag);
                WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
                //调用WebGIS实时数据服务接口查询车辆轨迹数据
                if (souflag == "RDS" && rc != null && rc.RunFlag)
                {
                    wc.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                    res = wc.GetCarData(sysflag, long.Parse(cid));
                    if (res != null)
                    {
                        if (res.OnlineStatus == 1)//在线，下发指令
                        {
                            SendResult sr = new SendResult();
                            sr.CID = cid;
                            sr.TNO = tno;
                            sr.CarNo = carno;
                            sr.Res = false;
                            sr.Desc = "销贷-设置还款日期及提醒时间";
                            sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            bool result = SendOrderHander.Send_CTS_TransmissionProtocol(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), 0x8F02, datas);
                            sr.Res = result;
                            if (result)
                            {
                                //如果下发返回True，则写入数据库记录
                                UpdateJFQZExtend_Transmission(sysflag, long.Parse(cid), datevalue, string.IsNullOrEmpty(dayvalue) ? -1 : Int32.Parse(dayvalue), string.IsNullOrEmpty(minutevalue) ? -1 : Int32.Parse(minutevalue), -1, string.Empty, string.Empty);
                            }
                            Result = new ResponseResult(ResState.Success, "", sr);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发销贷参数设置指令！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发销贷参数设置指令！", null);
                    }
                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发销贷参数设置指令！", null);
                }


            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            //SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
            return Result;
        }


        int toBCD(int a)
        {
            int ret = 0, shl = 0;
            while (a > 0)
            {
                ret |= (a % 10) << shl;
                a /= 10;
                shl += 4;
            }
            return ret;
        }


        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private byte[] uintToByte(uint a)
        {
            byte[] va = new byte[2];
            va[1] = (byte)(a >> 8);
            va[0] = (byte)(a);
            return va;
        }

        /// <summary>
        /// 销贷透传指令记录
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="RepaymentDate"></param>
        /// <param name="RemindDays"></param>
        /// <param name="DisconTimes"></param>
        /// <param name="SetSpeed"></param>
        /// <param name="LockDesc"></param>
        /// <param name="UnlockDesc"></param>
        private void UpdateJFQZExtend_Transmission(string sysflag, long cid, string RepaymentDate, int RemindDays, int DisconTimes, int SetSpeed, string LockDesc, string UnlockDesc)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@cid", cid), 
                                            new SqlParameter("@RepaymentDate", RepaymentDate), 
                                            new SqlParameter("@RemindDays", RemindDays), 
                                            new SqlParameter("@DisconTimes", DisconTimes), 
                                            new SqlParameter("@SetSpeed", SetSpeed), 
                                            new SqlParameter("@LockDesc", LockDesc), 
                                            new SqlParameter("@UnlockDesc", UnlockDesc)
                                        };
            csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("QWGProc_JFQZ_UpdateJFQZExtend_Transmission"), Parameters, false);
        }

    }
}