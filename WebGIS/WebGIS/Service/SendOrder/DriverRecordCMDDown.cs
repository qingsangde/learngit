using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;

namespace SysService
{
    public class DriverRecordCMDDown
    {
        public ResponseResult sendDriveRecordCMDDownOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string tno = inparams["tno"];
                string carno = inparams["carno"];
                string cid = inparams["cid"];
                string cmd = inparams["cmd"];
                string platenum = inparams["platenum"];
                string platetype = inparams["platetype"];
                string vin = inparams["vin"];
                string quotient = inparams["quotient"];


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
                            bool result = false;
                            switch (cmd)
                            {
                                case "0x82":
                                    sr.Desc = "行驶记录仪-设置车辆信息";
                                    sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    result = SendOrderHander.Send_DriveRecordDownCMDDown_VIN_NUM_TYPE(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), vin, platenum, platetype);
                                    sr.Res = result;
                                    break;
                                case "0xC3":
                                    sr.Desc = "行驶记录仪-设置脉冲系数";
                                    sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    result = SendOrderHander.Send_DriveRecordDownCMDDown_CharacterQuotient(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), uint.Parse(quotient));
                                    sr.Res = result;
                                    break;

                                default:
                                    sr.Desc = "";
                                    sr.Res = result;
                                    break;
                            }


                            Result = new ResponseResult(ResState.Success, "", sr);
                        }
                        else  //不在线，无法下发指令
                        {
                            Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发指令！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发指令！", null);
                    }
                } //不在线，无法下发指令
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发指令！", null);
                }

            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            //SendOrderHander.Send_CTS_SetTermParamDown(sysflag, token, cid, tno, paramlist);
            return Result;
        }
    }
}