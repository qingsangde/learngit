using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;

namespace SysService
{
    public class TermSearchRequestDown
    {
        public ResponseResult sendTermSearchOrder(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {
                string token = inparams["token"];
                string sysflag = inparams["sysflag"];
                string tno = inparams["tno"];
                string carno = inparams["carno"];
                string cid = inparams["cid"];
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
                            sr.Desc = "车辆点名";
                            sr.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            bool result = SendOrderHander.Send_CTS_TermSearchRequest(sysflag, token, long.Parse(sr.CID), long.Parse(sr.TNO));
                            sr.Res = result;
                            Result = new ResponseResult(ResState.Success, "", sr);
                        }
                        else
                        {
                            Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发点名指令！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseResult(ResState.OtherError, "车辆不在线，无法下发点名指令！", null);
                    }
                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "后台服务故障，暂时无法下发指令！", null);
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