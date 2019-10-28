using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.DAO;
using CMCS.Common.Entities.CarTransport;
using CMCS.DapperDber.Dbs.OracleDb;
using CMCS.Common;
using CMCS.Common.Entities;
using CMCS.Common.Views;
using CMCS.DapperDber.Util;
using CMCS.Common.Enums;
using CMCS.Common.Utilities;
using CMCS.Common.Entities.Fuel;

namespace CMCS.CarTransport.DAO
{
    /// <summary>
    /// 汽车过衡业务
    /// </summary>
    public class WeighterDAO
    {
        private static WeighterDAO instance;

        public static WeighterDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new WeighterDAO();
            }

            return instance;
        }

        private WeighterDAO()
        { }

        public OracleDapperDber SelfDber
        {
            get { return Dbers.GetInstance().SelfDber; }
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();
        CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();

        #region 入厂煤业务

        /// <summary>
        /// 获取指定日期已完成的入厂煤运输记录
        /// </summary>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public List<View_BuyFuelTransport> GetFinishedBuyFuelTransport(DateTime dtStart, DateTime dtEnd)
        {
            return SelfDber.Entities<View_BuyFuelTransport>("where GrossWeight!=0 and TareWeight!=0 and InFactoryTime>=:dtStart and InFactoryTime<:dtEnd order by InFactoryTime desc", new { dtStart = dtStart, dtEnd = dtEnd });
        }

        /// <summary>
        /// 获取指定日期未完成的入厂煤运输记录
        /// </summary>
        /// <returns></returns>
        public List<View_BuyFuelTransport> GetUnFinishBuyFuelTransport(DateTime dtStart, DateTime dtEnd)
        {
            return SelfDber.Entities<View_BuyFuelTransport>("where (GrossWeight=0 or TareWeight=0) and IsUse=1 and UnFinishTransportId is not null and InFactoryTime>=:dtStart and InFactoryTime<:dtEnd order by InFactoryTime desc", new { dtStart = dtStart, dtEnd = dtEnd });
        }

        /// <summary>
        /// 保存入厂煤运输记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="weight">重量</param>
        /// <param name="place"></param>
        /// <returns></returns>
        public bool SaveBuyFuelTransport(string transportId, decimal weight, DateTime dt, string place, string gatetype)
        {
            CmcsBuyFuelTransport transport = SelfDber.Get<CmcsBuyFuelTransport>(transportId);
            if (transport == null) return false;

            //string tempName = "";

            //if (stepname == eTruckInFactoryStep.入厂.ToString())
            //    tempName = "重车";
            //else if (stepname == eTruckInFactoryStep.重车.ToString())
            //    tempName = "轻车";
            //else if (stepname == eTruckInFactoryStep.轻车.ToString())
            //    tempName = "轻车";

            if (gatetype == eTruckInFactoryStep.重车.ToString())
            {
                transport.StepName = eTruckInFactoryStep.重车.ToString();
                transport.GrossWeight = weight;
                transport.GrossPlace = place;
                transport.GrossTime = dt;
            }
            else if (gatetype == eTruckInFactoryStep.轻车.ToString())
            {
                transport.StepName = eTruckInFactoryStep.轻车.ToString();
                transport.TareWeight = weight;
                transport.TarePlace = place;
                transport.TareTime = dt;
                transport.SuttleWeight = transport.GrossWeight - transport.TareWeight;


                //回皮则更新运输记录到批次
                commonDAO.InsertWaitForHandleEvent("汽车智能化_同步入厂煤运输记录到批次", transport.Id);

            }
            else
                return false;

            return SelfDber.Update(transport) > 0;
        }

        /// <summary>
        /// 保存入厂煤运输记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="weight">重量</param>
        /// <param name="place"></param>
        /// <returns></returns>
        public bool SaveBuyFuelTransport(string transportId, decimal weight, DateTime dt, string place)
        {
            int result = 0;
            if (weight <= 0)
            {
                Log4Neter.Error("保存运输记录", new Exception("重量异常"));
                return false;
            }
            CmcsBuyFuelTransport transport = SelfDber.Get<CmcsBuyFuelTransport>(transportId);
            if (transport == null)
            {
                Log4Neter.Error("保存运输记录", new Exception("运输记录为空"));
                return false;
            }

            if (transport.GrossWeight == 0)
            {
                transport.StepName = eTruckInFactoryStep.重车.ToString();
                transport.GrossWeight = weight;
                transport.GrossPlace = place;
                transport.GrossTime = dt;

                result = SelfDber.Update(transport);
                if (result == 0)
                    Log4Neter.Error("更新数据失败", new Exception("更新数据失败"));
            }
            else if (transport.TareWeight == 0)
            {
                transport.StepName = eTruckInFactoryStep.轻车.ToString();
                transport.TareWeight = weight;
                transport.TarePlace = place;
                transport.TareTime = dt;

                #region 自动扣吨算法
                //删除自动扣吨
                commonDAO.SelfDber.DeleteBySQL<CmcsBuyFuelTransportDeduct>("where TransportId=:TransportId and (DeductType='自动' or OperUser='自动')", new { TransportId = transport.Id });
                //计算扣吨
                IList<CmcsBuyFuelTransportDeduct> DeductList = commonDAO.SelfDber.Entities<CmcsBuyFuelTransportDeduct>("where TransportId=:TransportId", new { TransportId = transport.Id });
                if (DeductList != null) transport.DeductWeight = DeductList.Sum(a => a.DeductWeight);

                transport.SuttleWeight = transport.GrossWeight - transport.TareWeight;
                transport.CheckWeight = transport.SuttleWeight - transport.DeductWeight;
                transport.ProfitWeight = transport.SuttleWeight - transport.TicketWeight;

                //自动扣吨
                if (transport.IsAutoDeduct == 1)
                {
                    decimal autoDeduct = transport.SuttleWeight - transport.TicketWeight;
                    if (autoDeduct > 0)
                    {
                        CmcsBuyFuelTransportDeduct cmcsBuyFuelTransportDeduct = new CmcsBuyFuelTransportDeduct();
                        cmcsBuyFuelTransportDeduct.TransportId = transport.Id;
                        cmcsBuyFuelTransportDeduct.DeductWeight = autoDeduct;
                        cmcsBuyFuelTransportDeduct.DeductType = "自动";
                        cmcsBuyFuelTransportDeduct.OperDate = DateTime.Now;
                        cmcsBuyFuelTransportDeduct.OperUser = "自动";
                        if (commonDAO.SelfDber.Insert(cmcsBuyFuelTransportDeduct) > 0)
                        {
                            //transport.SuttleWeight -= autoDeduct;
                            transport.CheckWeight = transport.SuttleWeight - autoDeduct - transport.DeductWeight;
                            transport.ProfitWeight = transport.SuttleWeight - transport.TicketWeight;
                            transport.DeductWeight += autoDeduct;
                        }
                    }
                }
                #endregion

                // 回皮即完结
                transport.IsFinish = 1;

                result = SelfDber.Update(transport);
                if (result == 0)
                    Log4Neter.Error("更新数据失败", new Exception("更新数据失败"));
            

               commonDAO.InsertWaitForHandleEvent("汽车智能化_同步入厂煤运输记录到批次", transport.Id);
               commonDAO.RemoveUnFinishTransport(transport.Id);

                    // 生成批次以及采制化三级编码数据 
               CmcsInFactoryBatch inFactoryBatch = carTransportDAO.GCQCInFactoryBatchByBuyFuelTransport(transport);
             
            }
            else
            {
                Log4Neter.Error("保存运输记录", new Exception("毛重皮重异常"));
                return false;
            }
            //int result = SelfDber.Update(transport);
            //if (result == 0)
            //    Log4Neter.Error("更新数据失败", new Exception("更新数据失败"));
            return result > 0;
        }

        /// <summary>
        /// 验证称重是否成功
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public bool CheckBuyFuelTransport(CmcsBuyFuelTransport transport)
        {
            if (CommonAppConfig.GetInstance().AppIdentifier.Contains("1") || CommonAppConfig.GetInstance().AppIdentifier.Contains("2"))//轻车
            {
                if (transport.TareWeight > 0 && transport.SuttleWeight > 0)
                    return true;
            }
            else if (CommonAppConfig.GetInstance().AppIdentifier.Contains("3") || CommonAppConfig.GetInstance().AppIdentifier.Contains("4"))//重车
            {
                if (transport.GrossWeight > 0)
                    return true;
            }
            //if (transport.GrossWeight > 0 && transport.TareWeight == 0 && transport.GrossPlace == CommonAppConfig.GetInstance().AppIdentifier) return true;//秤毛重
            //if (transport.TareWeight > 0 && transport.TarePlace == CommonAppConfig.GetInstance().AppIdentifier) return true;//秤皮重
            return false;
        }
        #endregion

        #region 其他物资业务

        /// <summary>
        /// 获取指定日期已完成的其他物资运输记录
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<CmcsGoodsTransport> GetFinishedGoodsTransport(DateTime dtStart, DateTime dtEnd)
        {
            return SelfDber.Entities<CmcsGoodsTransport>("where SuttleWeight>0 and InFactoryTime>=:dtStart and InFactoryTime<:dtEnd order by secondtime desc", new { dtStart = dtStart, dtEnd = dtEnd });
        }

        /// <summary>
        /// 获取未完成的其他物资运输记录
        /// </summary>
        /// <returns></returns>
        public List<CmcsGoodsTransport> GetUnFinishGoodsTransport()
        {
            return SelfDber.Entities<CmcsGoodsTransport>("where SuttleWeight=0 and IsUse=1 and Id in (select TransportId from " + EntityReflectionUtil.GetTableName<CmcsUnFinishTransport>() + " where CarType=:CarType) order by InFactoryTime desc", new { CarType = eCarType.其他物资.ToString() });
        }

        /// <summary>
        /// 保存其他物资运输记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="weight">重量</param>
        /// <param name="place"></param>
        /// <returns></returns>
        public bool SaveGoodsTransport(string transportId, decimal weight, DateTime dt, string place)
        {
            CmcsGoodsTransport transport = SelfDber.Get<CmcsGoodsTransport>(transportId);
            if (transport == null) return false;

            if (weight <= 0)
            {
                Log4Neter.Error("保存运输记录", new Exception("重量异常"));
                return false;
            }
            if (transport.FirstWeight == weight) return false;
            if (transport.FirstWeight == 0)
            {
                transport.StepName = eTruckInFactoryStep.重车.ToString();
                transport.FirstWeight = weight;
                transport.FirstPlace = place;
                transport.FirstTime = dt;
            }
            else if (transport.SecondWeight == 0)
            {
                transport.StepName = eTruckInFactoryStep.轻车.ToString();
                transport.SecondWeight = weight;
                transport.SecondPlace = place;
                transport.SecondTime = dt;
                transport.SuttleWeight = Math.Abs(transport.SecondWeight - transport.FirstWeight);

                // 回皮即完结
                transport.IsFinish = 1;
                commonDAO.RemoveUnFinishTransport(transport.Id);
            }
            else
                return false;

            return SelfDber.Update(transport) > 0;
        }

        /// <summary>
        /// 验证称重是否成功
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public bool CheckGoodsTransport(CmcsGoodsTransport transport)
        {
            if (transport.FirstWeight <= 0) return false;//未秤毛重
            if (transport.FirstWeight > 0 && transport.SecondWeight == 0 && transport.FirstPlace == CommonAppConfig.GetInstance().AppIdentifier) return true;//秤毛重
            if (transport.SecondWeight > 0 && transport.SecondPlace == CommonAppConfig.GetInstance().AppIdentifier) return true;//秤皮重
            return false;
        }
        #endregion
    }
}
