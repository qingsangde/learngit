using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WebGIS;
using CommLibrary;

namespace SysService
{
    public class CarInfo
    {
        public ResponseResult getCarNos(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string carno;
            string token;
            if (!inparams.Keys.Contains("carno") || !inparams.Keys.Contains("token"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["carno"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "车牌号不能为空", null);
                    return Result;
                }
                if (inparams["token"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统回话标识错误", null);
                    return Result;
                }
                carno = inparams["carno"];
                token = inparams["token"];

                //调用存储过程查询车辆数据
                DataTable dt = getCarNos(carno, token);
                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        private DataTable getCarNos(String carno, string token)
        {
            DataTable dt = null;
            try
            {
                SessionModel sm = new SessionModel();
                sm = SessionManager.GetSession(token);
                dt = sm.cars.Clone();
                DataRow[] rows = sm.cars.Select("carno like '%" + carno + "%'");
                int h;
                if (rows.Length < 10)
                {
                    h = rows.Length;
                }
                else
                {
                    h = 10;
                }
                for (int i = 0; i < h; i++)
                {
                    dt.ImportRow(rows[i]);
                }
            }
            catch (Exception)
            {
                dt = null;
            }
            return dt;
        }
    }
}