using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;
using CommLibrary.Proto;
using System.Collections;

namespace SysService
{
    public class TermParamDown
    {
        //public static Hashtable hashtable = new Hashtable();
        public ResponseResult sendOverSpeedOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string maxspeed = inparams["maxspeed"];
                List<CTS_SetTermParamDown.TerminalParamItem> paramlist = new List<CTS_SetTermParamDown.TerminalParamItem>();
                CTS_SetTermParamDown.TerminalParamItem tp = new CTS_SetTermParamDown.TerminalParamItem();
                tp.nParamID = 0x0055;
                tp.nValue = uint.Parse(maxspeed);
                tp.nParamLen = 4;
                paramlist.Add(tp);
                List<SendResult> resList = new List<SendResult>();
                string[] cids = inparams["cids"].Split(',');
                string[] tnos = inparams["tnos"].Split(',');
                string[] carnos = inparams["carnos"].Split(',');

                string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
                WebGIS.RealtimeDataServer.CarRealData[] res = new WebGIS.RealtimeDataServer.CarRealData[] { };

                RDSConfigModel rc = RDSConfig.GetRDS(sysflag);
                WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
                //调用WebGIS实时数据服务接口查询车辆轨迹数据
                if (souflag == "RDS" && rc != null && rc.RunFlag)
                {
                    wc.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                    res = wc.GetCarsData(sysflag, ToInt64Array(cids));
                    bool flag = false;//是否有不在线车辆
                    if (res.Length > 0)
                    {
                        foreach (WebGIS.RealtimeDataServer.CarRealData carRealData in res)
                        {
                            if (carRealData.OnlineStatus == 2)
                            {
                                flag = true;//如果有不在线车辆，flag置为true，跳出循环
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        Result = new ResponseResult(ResState.OtherError, "包含不在线车辆，无法下发指令！", null);
                    }
                    else
                    {
                        int cidcount = cids.Length;
                        int tnocount = tnos.Length;
                        int carnocount = carnos.Length;
                        if (cidcount == tnocount && cidcount == carnocount)
                        {
                            for (int i = 0; i < cidcount; i++)
                            {
                                SendResult sr = new SendResult();
                                sr.CID = cids[i];
                                sr.TNO = tnos[i];
                                sr.CarNo = carnos[i];
                                sr.Res = false;
                                sr.Desc = "设置终端最大速度";
                                sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                resList.Add(sr);
                            }
                            foreach (SendResult sr in resList)
                            {
                                bool result = SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), paramlist);
                                sr.Res = result;
                            }
                            Result = new ResponseResult(ResState.Success, "", resList);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.ParamsImperfect, "车辆参数不匹配", null);
                        }
                    }

                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "包含不在线车辆，无法下发指令！", null);
                }


            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            //SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
            return Result;
        }


        public ResponseResult sendCallTransferOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string ip = inparams["ip"];
                string tcp = inparams["tcp"];
                string udp = inparams["udp"];
                List<CTS_SetTermParamDown.TerminalParamItem> paramlist = new List<CTS_SetTermParamDown.TerminalParamItem>();
                CTS_SetTermParamDown.TerminalParamItem tp = new CTS_SetTermParamDown.TerminalParamItem();
                tp.nParamID = 0x0013;
                tp.sValue = System.Text.Encoding.Default.GetBytes(ip);
                tp.nParamLen = Convert.ToUInt32(System.Text.Encoding.Default.GetBytes(ip).Length);
                paramlist.Add(tp);
                CTS_SetTermParamDown.TerminalParamItem tp1 = new CTS_SetTermParamDown.TerminalParamItem();
                tp1.nParamID = 0x0018;
                tp1.nValue = uint.Parse(tcp);
                tp1.nParamLen = 4;
                paramlist.Add(tp1);
                CTS_SetTermParamDown.TerminalParamItem tp2 = new CTS_SetTermParamDown.TerminalParamItem();
                tp2.nParamID = 0x0019;
                tp2.nValue = uint.Parse(udp);
                tp2.nParamLen = 4;
                paramlist.Add(tp2);
                List<SendResult> resList = new List<SendResult>();
                string[] cids = inparams["cids"].Split(',');
                string[] tnos = inparams["tnos"].Split(',');
                string[] carnos = inparams["carnos"].Split(',');

                string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
                WebGIS.RealtimeDataServer.CarRealData[] res = new WebGIS.RealtimeDataServer.CarRealData[] { };

                RDSConfigModel rc = RDSConfig.GetRDS(sysflag);
                WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
                //调用WebGIS实时数据服务接口查询车辆轨迹数据
                if (souflag == "RDS" && rc != null && rc.RunFlag)
                {
                    wc.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                    res = wc.GetCarsData(sysflag, ToInt64Array(cids));
                    bool flag = false;//是否有不在线车辆
                    if (res.Length > 0)
                    {
                        foreach (WebGIS.RealtimeDataServer.CarRealData carRealData in res)
                        {
                            if (carRealData.OnlineStatus == 2)
                            {
                                flag = true;//如果有不在线车辆，flag置为true，跳出循环
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        Result = new ResponseResult(ResState.OtherError, "含不在线车辆，无法下发指令！", null);
                    }
                    else
                    {
                        int cidcount = cids.Length;
                        int tnocount = tnos.Length;
                        int carnocount = carnos.Length;

                        if (cidcount == tnocount && cidcount == carnocount)
                        {
                            for (int i = 0; i < cidcount; i++)
                            {
                                SendResult sr = new SendResult();
                                sr.CID = cids[i];
                                sr.TNO = tnos[i];
                                sr.CarNo = carnos[i];
                                sr.Res = false;
                                sr.Desc = "设置车辆呼转参数";
                                sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                resList.Add(sr);
                            }
                            foreach (SendResult sr in resList)
                            {
                                bool result = SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), paramlist);
                                sr.Res = result;
                            }
                            Result = new ResponseResult(ResState.Success, "", resList);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.ParamsImperfect, "车辆参数不匹配", null);
                        }
                    }

                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "含不在线车辆，无法下发指令！", null);
                }


            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            //SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
            return Result;
        }


        public ResponseResult sendTimeIntervalOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string timeinterval = inparams["timeinterval"];
                List<CTS_SetTermParamDown.TerminalParamItem> paramlist = new List<CTS_SetTermParamDown.TerminalParamItem>();
                CTS_SetTermParamDown.TerminalParamItem tp = new CTS_SetTermParamDown.TerminalParamItem();
                tp.nParamID = 0x0029;
                tp.nValue = uint.Parse(timeinterval);
                tp.nParamLen = 4;
                paramlist.Add(tp);
                List<SendResult> resList = new List<SendResult>();
                string[] cids = inparams["cids"].Split(',');
                string[] tnos = inparams["tnos"].Split(',');
                string[] carnos = inparams["carnos"].Split(',');

                string souflag = System.Configuration.ConfigurationManager.AppSettings["MonitorDataSource"];
                WebGIS.RealtimeDataServer.CarRealData[] res = new WebGIS.RealtimeDataServer.CarRealData[] { };

                RDSConfigModel rc = RDSConfig.GetRDS(sysflag);
                WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
                //调用WebGIS实时数据服务接口查询车辆轨迹数据
                if (souflag == "RDS" && rc != null && rc.RunFlag)
                {
                    wc.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                    res = wc.GetCarsData(sysflag, ToInt64Array(cids));
                    bool flag = false;//是否有不在线车辆
                    if (res.Length > 0)
                    {
                        foreach (WebGIS.RealtimeDataServer.CarRealData carRealData in res)
                        {
                            if (carRealData.OnlineStatus == 2)
                            {
                                flag = true;//如果有不在线车辆，flag置为true，跳出循环
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        Result = new ResponseResult(ResState.OtherError, "包含不在线车辆，无法下发指令！", null);
                    }
                    else
                    {
                        int cidcount = cids.Length;
                        int tnocount = tnos.Length;
                        int carnocount = carnos.Length;
                        if (cidcount == tnocount && cidcount == carnocount)
                        {
                            for (int i = 0; i < cidcount; i++)
                            {
                                SendResult sr = new SendResult();
                                sr.CID = cids[i];
                                sr.TNO = tnos[i];
                                sr.CarNo = carnos[i];
                                sr.Res = false;
                                sr.Desc = "设置上传时间间隔";
                                sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                resList.Add(sr);
                            }
                            foreach (SendResult sr in resList)
                            {
                                bool result = SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), paramlist);
                                sr.Res = result;
                            }
                            Result = new ResponseResult(ResState.Success, "", resList);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.ParamsImperfect, "车辆参数不匹配", null);
                        }
                    }

                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "包含不在线车辆，无法下发指令！", null);
                }


            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            //SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
            return Result;
        }

        private byte[] uintToByte(uint a)
        {
            byte[] va = new byte[2];
            va[1] = (byte)(a >> 8);
            va[0] = (byte)(a);
            return va;
        }

        /* 销贷锁车指令下发改为透传
        public ResponseResult sendXiaoDaiLockOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string tno = inparams["tno"];
                string carno = inparams["carno"];
                string cid = inparams["cid"];
                string paramvalue = inparams["paramvalue"];
                string rev = inparams["rev"];
                List<CTS_SetTermParamDown.TerminalParamItem> paramlist = new List<CTS_SetTermParamDown.TerminalParamItem>();
                CTS_SetTermParamDown.TerminalParamItem tp = new CTS_SetTermParamDown.TerminalParamItem();
                tp.nParamID = 0x0001;
                //tp.nValue = Convert.ToUInt32(paramvalue,16);
                switch (paramvalue)
                {
                    case "0x00":
                        tp.sValue = new byte[] { 0x00 };
                        break;
                    case "0xA8":
                        tp.sValue = new byte[] { 0xA8 };
                        break;
                    case "0x8A":
                        tp.sValue = new byte[] { 0x8A };
                        break;
                    case "0xAA":
                        tp.sValue = new byte[] { 0xAA };
                        break;
                }
                tp.nParamLen = 1;
                paramlist.Add(tp);

                if (paramvalue == "0xA8" || paramvalue == "0x8A")
                {
                    CTS_SetTermParamDown.TerminalParamItem tp2 = new CTS_SetTermParamDown.TerminalParamItem();
                    tp2.nParamID = 0x0007;
                    tp2.sValue = uintToByte(uint.Parse(rev));
                    tp2.nValue = uint.Parse(rev);
                    tp2.nParamLen = 2;
                    paramlist.Add(tp2);
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
                            sr.Desc = "车辆锁定设置";
                            sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            bool result = SendOrderHander.Send_CTS_SetTermParamDownQMExtend(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), paramlist);
                            sr.Res = result;
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
         */

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

        /* 销贷激活/关闭改为透传
        public ResponseResult sendXiaoDaiVinOrder(Dictionary<string, string> inparams)
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
                string rev = inparams["rev"];
                string orderDesc = "";
                List<CTS_SetTermParamDown.TerminalParamItem> paramlist = new List<CTS_SetTermParamDown.TerminalParamItem>();
                CTS_SetTermParamDown.TerminalParamItem tp = new CTS_SetTermParamDown.TerminalParamItem();
                tp.nParamID = 0x0005;
                tp.sValue = new byte[] { 0xA8 };  //使用立即加锁功能用特殊转速参数处理GPS激活及取消指令
                if (vintype == "0xAA")
                {                    
                    orderDesc = "车辆GPS功能激活";
                }
                else if (vintype == "0x55")
                {
                    orderDesc = "车辆GPS功能取消";
                }
                tp.nParamLen = 1;
                paramlist.Add(tp);

                //绑定解绑VIN指令改成GPS激活取消指令
                if (vintype == "0xAA")  //激活
                {
                    CTS_SetTermParamDown.TerminalParamItem tp2 = new CTS_SetTermParamDown.TerminalParamItem();
                    tp2.nParamID = 0x0007;
                    uint constkey = 32768 + uint.Parse(GetLastStr(tno, 4));
                    tp2.sValue = uintToByte(constkey);
                    tp2.nValue = constkey;
                    tp2.nParamLen = 2;
                    paramlist.Add(tp2);
                }
                else  //取消
                {
                    CTS_SetTermParamDown.TerminalParamItem tp2 = new CTS_SetTermParamDown.TerminalParamItem();
                    tp2.nParamID = 0x0007;
                    tp2.sValue = new byte[] { 0x00, 0x00 }; ;
                    tp2.nValue = 0;
                    tp2.nParamLen = 2;
                    paramlist.Add(tp2);
                }

                //CTS_SetTermParamDown.TerminalParamItem tp2 = new CTS_SetTermParamDown.TerminalParamItem();
                //tp2.nParamID = 0x0007;
                //tp2.nValue = uint.Parse(rev); ;
                //tp2.nParamLen = 2;
                //paramlist.Add(tp2);

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
                            bool result = SendOrderHander.Send_CTS_SetTermParamDownQMExtend(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), paramlist);
                            sr.Res = result;
                            Result = new ResponseResult(ResState.Success, "", sr);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发" + orderDesc +"指令！", null);
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
         */

        public string GetLastStr(string str, int num)
        {
            int count = 0;
            if (str.Length > num)
            {
                count = str.Length - num;
                str = str.Substring(count, num);
            }
            return str;
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

        /*销贷还款日期及提醒设置指令改为透传
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
                List<CTS_SetTermParamDown.TerminalParamItem> paramlist = new List<CTS_SetTermParamDown.TerminalParamItem>();
                if (!string.IsNullOrEmpty(datevalue))
                {
                    DateTime dt = DateTime.Parse(datevalue);
                    CTS_SetTermParamDown.TerminalParamItem tp = new CTS_SetTermParamDown.TerminalParamItem();
                    tp.nParamID = 0x0002;
                    tp.sValue = new byte[] { Convert.ToByte(toBCD(dt.Year % 100)), Convert.ToByte(toBCD(dt.Month)), Convert.ToByte(toBCD(dt.Day)) };
                    tp.nParamLen = 3;
                    paramlist.Add(tp);
                }


                if (!string.IsNullOrEmpty(dayvalue))
                {
                    CTS_SetTermParamDown.TerminalParamItem tp1 = new CTS_SetTermParamDown.TerminalParamItem();
                    tp1.nParamID = 0x0003;
                    tp1.nValue = uint.Parse(dayvalue);
                    tp1.nParamLen = 2;
                    paramlist.Add(tp1);
                }

                if (!string.IsNullOrEmpty(minutevalue))
                {
                    CTS_SetTermParamDown.TerminalParamItem tp2 = new CTS_SetTermParamDown.TerminalParamItem();
                    tp2.nParamID = 0x0004;
                    tp2.nValue = uint.Parse(minutevalue);
                    tp2.nParamLen = 2;
                    paramlist.Add(tp2);
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
                            sr.Desc = "销贷参数设置";
                            sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            bool result = SendOrderHander.Send_CTS_SetTermParamDownQMExtend(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), paramlist);
                            sr.Res = result;
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
         */
        /// <summary>
        /// 字符串数组转换整形数组
        /// </summary>
        /// <param name="Content">字符串数组</param>
        /// <returns></returns>
        public static long[] ToInt64Array(string[] Content)
        {
            long[] c = new long[Content.Length];
            for (int i = 0; i < Content.Length; i++)
            {
                c[i] = Convert.ToInt64(Content[i].ToString());
            }
            return c;
        }
    }
}