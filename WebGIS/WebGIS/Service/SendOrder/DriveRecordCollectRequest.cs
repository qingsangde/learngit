using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;

namespace SysService
{
    public class DriveRecordCollectRequest
    {
        public ResponseResult sendDriveRecordCollectOrder(Dictionary<string, string> inparams)
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
                string start = inparams["start"];
                string end = inparams["end"];
                long startsecond = 0;
                long endsecond = 0;
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
                            switch (cmd)
                            {
                                case "0x00":
                                    sr.Desc = "采集记录仪执行标准版本";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x01":
                                    sr.Desc = "采集当前驾驶人信息";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x02":
                                    sr.Desc = "采集记录仪的实时时钟";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x03":
                                    sr.Desc = "采集累计行程里程";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x04":
                                    sr.Desc = "采集记录仪脉冲系数";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x05":
                                    sr.Desc = "采集车辆信息";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x06":
                                    sr.Desc = "采集记录仪状态信号配置信息";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x07":
                                    sr.Desc = "采集记录仪唯一性编号";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                                case "0x08":
                                    sr.Desc = "采集指定的行驶速度记录";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                case "0x09":
                                    sr.Desc = "采集指定的位置信息记录";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                case "0x10":
                                    sr.Desc = "采集指定的事故疑点记录";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                case "0x11":
                                    sr.Desc = "采集指定的超时驾驶记录";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                case "0x12":
                                    sr.Desc = "采集指定的驾驶人身份记录";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                case "0x13":
                                    sr.Desc = "采集指定的外部供电记录";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                case "0x14":
                                    sr.Desc = "采集指定的参数修改记录";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                case "0x15":
                                    sr.Desc = "采集指定的速度状态日志";
                                    startsecond = Convert.ToInt64((DateTime.Parse(start) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    endsecond = Convert.ToInt64((DateTime.Parse(end) - DateTime.Parse("1970-01-01 00:00:00").ToLocalTime()).TotalSeconds);
                                    break;
                                default:
                                    sr.Desc = "";
                                    startsecond = 0;
                                    endsecond = 0;
                                    break;
                            }
                            sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            bool result = SendOrderHander.Send_CTS_DriveRecordDataCollectionRequest(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO), Convert.ToUInt32(cmd,16), 0, startsecond, endsecond);
                            sr.Res = result;
                            Result = new ResponseResult(ResState.Success, "", sr);
                        }
                        else  //不在线，无法下发指令
                        {
                            Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发采集指令！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发采集指令！", null);
                    }
                } //不在线，无法下发指令
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发采集指令！", null);
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