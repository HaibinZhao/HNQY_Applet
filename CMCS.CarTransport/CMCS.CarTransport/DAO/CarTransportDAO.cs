﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.DAO;
using CMCS.Common.Entities;
using CMCS.DapperDber.Dbs.OracleDb;
using CMCS.Common;
using CMCS.Common.Utilities;
using CMCS.Common.Entities.CarTransport;
using CMCS.DapperDber.Util;
using CMCS.Common.Enums;
using CMCS.Common.Views;
using CMCS.CarTransport.Views;
using CMCS.Common.Entities.Fuel;

namespace CMCS.CarTransport.DAO
{
    /// <summary>
    /// 汽车智能化业务
    /// </summary>
    public class CarTransportDAO
    {
        private static CarTransportDAO instance;

        public static CarTransportDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new CarTransportDAO();
            }

            return instance;
        }

        private CarTransportDAO()
        { }

        public OracleDapperDber SelfDber
        {
            get { return Dbers.GetInstance().SelfDber; }
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();

        #region 车辆管理

        /// <summary>
        /// 根据车牌号获取车辆信息
        /// </summary>
        /// <param name="carNumber"></param>
        /// <returns></returns>
        public CmcsAutotruck GetAutotruckByCarNumber(string carNumber)
        {
            return SelfDber.Entity<CmcsAutotruck>("where CarNumber=:CarNumber", new { CarNumber = carNumber });
        }

        /// <summary>
        /// 根据标签卡号获取车辆信息
        /// </summary>
        /// <param name="carNumber"></param>
        /// <returns></returns>
        public CmcsAutotruck GetAutotruckByTagId(string tagId)
        {
            CmcsEPCCard ePCCard = SelfDber.Entity<CmcsEPCCard>("where TagId=:TagId", new { TagId = tagId });
            if (ePCCard != null) return SelfDber.Entity<CmcsAutotruck>("where EPCCardId=:EPCCardId", new { EPCCardId = ePCCard.Id });

            return null;
        }

        #endregion

        #region 省份简称

        /// <summary>
        /// 获取省份简称，并按照使用次数降序
        /// </summary>
        /// <returns></returns>
        public List<CmcsProvinceAbbreviation> GetProvinceAbbreviationsInOrder()
        {
            return Dbers.GetInstance().SelfDber.Entities<CmcsProvinceAbbreviation>("order by UseCount desc");
        }

        /// <summary>
        /// 增加省份简称的使用次数
        /// </summary>
        /// <param name="paName">省份简称</param>
        public void AddProvinceAbbreviationUseCount(string paName)
        {
            Dbers.GetInstance().SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsProvinceAbbreviation>() + " set UseCount=UseCount+1 where PaName=:PaName", new { PaName = paName });
        }

        #endregion

        #region 批次与采制化

        /// <summary>
        /// 根据运输记录判断批次是否已生成，并返回。
        /// 根据入厂时间（实际到厂时间）、供煤单位、煤种判断
        /// </summary>
        /// <param name="buyFuelTransport"></param>
        /// <returns></returns>
        public CmcsInFactoryBatch HasInFactoryBatch(CmcsBuyFuelTransport buyFuelTransport, string batch = "")
        {
            DateTime dt = buyFuelTransport.CreateDate.AddHours(-commonDAO.GetCommonAppletConfigInt32("汽车分批时间点"));
            //if (buyFuelTransport.TareTime.Year > 2000) dt = buyFuelTransport.TareTime.AddHours(-commonDAO.GetCommonAppletConfigInt32("汽车分批时间点"));
            if (!string.IsNullOrEmpty(batch) && batch.Length == 17)
                return SelfDber.Entity<CmcsInFactoryBatch>("where Batch=:Batch order by CreateDate desc", new { Batch = batch });
            else
                return SelfDber.Entity<CmcsInFactoryBatch>("where Batch like '%-'|| to_char(:CreateDate,'YYYYMMDD') ||'-%' and SupplierId=:SupplierId and MineId=:MineId and FuelKindId=:FuelKindId and IsCTAutoCreate=1 and IsCheck=0", new { CreateDate = dt, SupplierId = buyFuelTransport.SupplierId, MineId = buyFuelTransport.MineId, FuelKindId = buyFuelTransport.FuelKindId });
        }

        /// <summary>
        /// 生成制定日期的批次编码
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="dtFactArriveDate">实际到厂时间</param>
        /// <returns></returns>
        public string CreateNewBatchNumber(string prefix, DateTime dtFactArriveDate)
        {
            DateTime dt = dtFactArriveDate.AddHours(-commonDAO.GetCommonAppletConfigInt32("汽车分批时间点"));
            CmcsInFactoryBatch entity = SelfDber.Entity<CmcsInFactoryBatch>("where Batch like '%-'||to_char(:CreateDate,'YYYYMMDD')||'-%' and Batch like :Prefix || '%' order by Batch desc", new { CreateDate = dt, Prefix = prefix + "-" + dt.ToString("yyyyMMdd") });

            if (entity == null)
                return string.Format("{0}-{1}-01", prefix, dt.ToString("yyyyMMdd"));
            else
                return string.Format("{0}-{1}-{2}", prefix, dt.ToString("yyyyMMdd"), (Convert.ToInt16(entity.Batch.Replace(string.Format("{0}-{1}-", prefix, dt.ToString("yyyyMMdd")), "")) + 1).ToString().PadLeft(2, '0'));
        }

        /// <summary>
        /// 根据运输记录生成批次并返回。
        /// 根据入厂时间（实际到厂时间）、供煤单位、煤种生成，已存在则不创建
        /// </summary>
        /// <param name="buyFuelTransport"></param>
        /// <returns></returns>
        public CmcsInFactoryBatch GCQCInFactoryBatchByBuyFuelTransport(CmcsBuyFuelTransport buyFuelTransport)
        {
            string batch = string.Empty;
            if (!string.IsNullOrEmpty(buyFuelTransport.YbNum) && buyFuelTransport.YbNum.Length == 17)
                batch = buyFuelTransport.YbNum.Replace("QCYB", "QDQC");
            bool isSuccess = true;
            CmcsInFactoryBatch entity = HasInFactoryBatch(buyFuelTransport, batch);
            if (entity == null)
            {
                entity = new CmcsInFactoryBatch()
                {
                    Batch = batch == "" ? CreateNewBatchNumber("QDQC", buyFuelTransport.CreateDate) : batch,
                    BatchType = "汽车",
                    FactArriveDate = buyFuelTransport.InFactoryTime,
                    FuelKindId = buyFuelTransport.FuelKindId,
                    FuelKindName = buyFuelTransport.FuelKindName,
                    SupplierId = buyFuelTransport.SupplierId,
                    MineId = buyFuelTransport.MineId,
                    RunDate = buyFuelTransport.InFactoryTime,
                    TransportCompanyId = buyFuelTransport.TransportCompanyId,
                    Remark = "由汽车煤智能化自动创建",
                    IsFinish_BM = 0,
                    IsCheck = 0,
                    IsCTAutoCreate = 1,
                    LMYBId = buyFuelTransport.LMYBId
                };

                // 创建新批次
                isSuccess = SelfDber.Insert(entity) > 0;
            }

            if (isSuccess)
            {
                // 生成采制化数据记录
                CmcsRCSampling rCSampling = commonDAO.GCSamplingMakeAssay(entity, buyFuelTransport.SamplingType, "由汽车煤智能化自动创建", eAssayType.三级编码化验, "入厂");

                buyFuelTransport.SamplingId = rCSampling.Id;
                buyFuelTransport.InFactoryBatchId = entity.Id;
            }

            return entity;
        }

        /// <summary>
        /// 根据采样单id获取采样单
        /// </summary>
        /// <param name="samplingId">采样单id</param>
        /// <returns></returns>
        public CmcsRCSampling GetRCSamplingById(string samplingId)
        {
            return SelfDber.Entity<CmcsRCSampling>("where Id=:Id", new { Id = samplingId });
        }

        #endregion

        #region 公共

        /// <summary>
        /// 根据车Id获取未完成的运输记录
        /// </summary>
        /// <param name="autotruckId">车Id</param>
        /// <returns></returns>
        public CmcsUnFinishTransport GetUnFinishTransportByAutotruckId(string autotruckId, string carType)
        {
            return SelfDber.Entity<CmcsUnFinishTransport>("where AutotruckId=:AutotruckId and CarType=:CarType order by CreateDate desc", new { AutotruckId = autotruckId, CarType = carType });
        }

        /// <summary>
        /// 根据车牌号获取未完成的运输记录
        /// </summary>
        /// <param name="autotruckId">车牌号</param>
        /// <returns></returns>
        public List<View_UnFinishTransport> GetUnFinishTransportByCarNumber(string carNumber, string sqlWhere)
        {
            List<View_UnFinishTransport> res = SelfDber.Entities<View_UnFinishTransport>(sqlWhere);
            if (!string.IsNullOrEmpty(carNumber))
            {
                return res.Where(a =>
                {
                    if (a.CarNumber.ToUpper().Contains(carNumber.ToUpper())) return true;

                    return false;
                }).ToList();
            }
            else
                return res;
        }

        /// <summary>
        /// 根据运输记录id查找入厂煤运输记录
        /// </summary>
        /// <param name="transportId">未完成的运输记录Id</param>
        /// <returns></returns>
        public CmcsBuyFuelTransport GetBuyFuelTransportById(string transportId)
        {
            return SelfDber.Get<CmcsBuyFuelTransport>(transportId);
        }

        /// <summary>
        /// 根据标签号id查找入厂煤运输记录
        /// </summary>
        /// <param name="tagId">标签Id</param>
        /// <returns></returns>
        public CmcsBuyFuelTransport GetBuyFuelTransportByEpcId(string epcId)
        {
            return SelfDber.Entity<CmcsBuyFuelTransport>("where AUTOTRUCKID=(select id from cmcstbautotruck where epccardid=:EpcId)", new { EpcId = epcId });
        }

        /// <summary>
        /// 根据车辆ID查找入厂煤运输记录
        /// </summary>
        /// <param name="tagId">车辆Id</param>
        /// <returns></returns>
        public CmcsBuyFuelTransport GetBuyFuelTransportByAutoTruckId(string autoTruckId)
        {
            return SelfDber.Entity<CmcsBuyFuelTransport>("where AUTOTRUCKID=:AutoTruckId order by CreateDate desc", new { AutoTruckId = autoTruckId });
        }

        /// <summary>
        /// 将指定车的未完结运输记录强制更改为无效
        /// </summary>
        /// <param name="autotruckId">车Id</param>
        public void ChangeUnFinishTransportToInvalid(string autotruckId)
        {
            if (string.IsNullOrEmpty(autotruckId)) return;

            foreach (CmcsUnFinishTransport unFinishTransport in SelfDber.Entities<CmcsUnFinishTransport>("where AutotruckId=:AutotruckId", new { AutotruckId = autotruckId }))
            {
                if (unFinishTransport.CarType == eCarType.入厂煤.ToString())
                {
                    SelfDber.Execute("Update " + EntityReflectionUtil.GetTableName<CmcsBuyFuelTransport>() + " set IsUse=0 where Id=:Id", new { Id = unFinishTransport.TransportId });
                    SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                }
                if (unFinishTransport.CarType == eCarType.其他物资.ToString())
                {
                    SelfDber.Execute("Update " + EntityReflectionUtil.GetTableName<CmcsGoodsTransport>() + " set IsUse=0 where Id=:Id", new { Id = unFinishTransport.TransportId });
                    SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                }
                if (unFinishTransport.CarType == eCarType.来访车辆.ToString())
                {
                    SelfDber.Execute("Update " + EntityReflectionUtil.GetTableName<CmcsVisitTransport>() + " set IsUse=0 where Id=:Id", new { Id = unFinishTransport.TransportId });
                    SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id);
                }
            }
        }

        /// <summary>
        /// 生成运输记录流水号
        /// </summary>
        /// <param name="carType">车类型</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string CreateNewTransportSerialNumber(eCarType carType, DateTime dt)
        {
            string prefix = "Null";

            if (carType == eCarType.入厂煤)
            {
                prefix = "RCM";

                CmcsBuyFuelTransport entity = SelfDber.Entity<CmcsBuyFuelTransport>("where to_char(CreateDate,'yyyymmdd')=to_char(:CreateDate,'yyyymmdd') and SerialNumber like :Prefix || '%' order by InFactoryTime desc", new { CreateDate = dt, Prefix = prefix });
                if (entity == null)
                    return prefix + dt.ToString("yyMMdd") + "001";
                else
                    return prefix + dt.ToString("yyMMdd") + (Convert.ToInt16(entity.SerialNumber.Replace(prefix + dt.ToString("yyMMdd"), "")) + 1).ToString().PadLeft(3, '0');
            }
            else if (carType == eCarType.其他物资)
            {
                prefix = "WZ";

                CmcsGoodsTransport entity = SelfDber.Entity<CmcsGoodsTransport>("where to_char(CreateDate,'yyyymmdd')=to_char(:CreateDate,'yyyymmdd') and SerialNumber like :Prefix || '%' order by InFactoryTime desc", new { CreateDate = dt, Prefix = prefix });
                if (entity == null)
                    return prefix + dt.ToString("yyMMdd") + "001";
                else
                    return prefix + dt.ToString("yyMMdd") + (Convert.ToInt16(entity.SerialNumber.Replace(prefix + dt.ToString("yyMMdd"), "")) + 1).ToString().PadLeft(3, '0');
            }
            else if (carType == eCarType.来访车辆)
            {
                prefix = "LF";

                CmcsVisitTransport entity = SelfDber.Entity<CmcsVisitTransport>("where to_char(CreateDate,'yyyymmdd')=to_char(:CreateDate,'yyyymmdd') and SerialNumber like :Prefix || '%' order by InFactoryTime desc", new { CreateDate = dt, Prefix = prefix });
                if (entity == null)
                    return prefix + dt.ToString("yyMMdd") + "001";
                else
                    return prefix + dt.ToString("yyMMdd") + (Convert.ToInt16(entity.SerialNumber.Replace(prefix + dt.ToString("yyMMdd"), "")) + 1).ToString().PadLeft(3, '0');
            }

            return prefix + dt.ToString("yyMMdd") + DateTime.Now.Second.ToString().PadLeft(3, '0');
        }

        /// <summary>
        /// 根据汽车入厂路线设置，判断当前是否准许通过，不通过则返回下一地点的位置
        /// </summary>
        /// <param name="carType">车类型</param>
        /// <param name="currentStepName">此车当前所处步骤</param>
        /// <param name="thisSetpName">当前地点代表的步骤</param>
        /// <param name="thisPlaceName">当前地点</param>
        /// <param name="nextPlace">返回下一地点的位置</param>
        /// <returns></returns>
        public bool CheckNextTruckInFactoryWay(string carType, string currentStepName, string thisSetpName, string thisPlaceName, out string nextPlace)
        {
            nextPlace = string.Empty;

            // 查找启用的路线，若没有启用的线路则通过
            CmcsTruckInFactoryWay truckInFactoryWay = SelfDber.Entity<CmcsTruckInFactoryWay>("where WayType=:WayType", new { WayType = carType });
            if (truckInFactoryWay == null) return true;

            // 查找当前所处的步骤
            CmcsTruckInFactoryWayDetail currentTruckInFactoryWayDetail = SelfDber.Entity<CmcsTruckInFactoryWayDetail>("where StepName=:StepName and TruckInFactoryWayId=:TruckInFactoryWayId", new { StepName = currentStepName, TruckInFactoryWayId = truckInFactoryWay.Id });
            if (currentTruckInFactoryWayDetail == null) return false;

            // 查找下一步骤
            CmcsTruckInFactoryWayDetail nextTruckInFactoryWayDetail = SelfDber.Entity<CmcsTruckInFactoryWayDetail>("where TruckInFactoryWayId=:TruckInFactoryWayId and StepNumber=:StepNumber", new { TruckInFactoryWayId = truckInFactoryWay.Id, StepNumber = currentTruckInFactoryWayDetail.StepNumber + 1 });
            if (nextTruckInFactoryWayDetail == null) return false;

            // 首先判断步骤是否符合条件
            if (!thisSetpName.Contains(nextTruckInFactoryWayDetail.StepName) || !("|" + nextTruckInFactoryWayDetail.WayPalce + "|").Contains("|" + thisPlaceName + "|"))
            {
                // 步骤不符合

                string[] nextPlaces = nextTruckInFactoryWayDetail.WayPalce.Split('|');
                nextPlace = (nextPlaces.Length > 0) ? nextPlaces[0] : string.Empty;
                return false;
            }
            //else
            //{
            //    // 步骤符合，再判断地点是否符合条件

            //    if (!("|" + nextTruckInFactoryWayDetail.WayPalce + "|").Contains("|" + thisPlaceName + "|")) return false;
            //}

            return true;
        }

        /// <summary>
        /// 保存未完成运输记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="carType"></param>
        /// <param name="autoTruckId"></param>
        /// <returns></returns>
        public bool SaveUnFinishTransport(string transportId, eCarType carType, string autoTruckId)
        {
            CmcsUnFinishTransport unFinishTransport = Dbers.GetInstance().SelfDber.Entity<CmcsUnFinishTransport>("where TransportId=:TransportId and CarType=:CarType and AutotruckId=:AutotruckId", new { TransportId = transportId, CarType = carType.ToString(), AutotruckId = autoTruckId });
            if (unFinishTransport == null)
            {
                unFinishTransport = new CmcsUnFinishTransport();
                unFinishTransport.TransportId = transportId;
                unFinishTransport.CarType = carType.ToString();
                unFinishTransport.AutotruckId = autoTruckId;
                unFinishTransport.PrevPlace = CommonAppConfig.GetInstance().AppIdentifier;
                return Dbers.GetInstance().SelfDber.Insert(unFinishTransport) > 0;
            }
            return false;
        }
        #endregion

        #region 入厂煤

        /// <summary>
        /// 保存入厂煤运输记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="weight">重量</param>
        /// <param name="place"></param>
        /// <returns></returns>
        public bool SaveBuyFuelTransport(CmcsBuyFuelTransport transport)
        {
            if (transport == null) return false;
            if (transport.GrossWeight > 0)
            {
                transport.StepName = eTruckInFactoryStep.重车.ToString();
                if (transport.GrossTime.Year < 2000) transport.GrossTime = DateTime.Now;
                if (transport.TareWeight > 0)
                {
                    transport.StepName = eTruckInFactoryStep.轻车.ToString();
                    if (transport.TareTime.Year < 2000) transport.TareTime = DateTime.Now;

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
                                transport.CheckWeight = transport.SuttleWeight - autoDeduct - transport.DeductWeight;
                                transport.ProfitWeight = transport.SuttleWeight - transport.TicketWeight;
                                transport.DeductWeight += autoDeduct;
                            }
                        }
                    }
                    #endregion

                    transport.IsFinish = 1;
                    //回皮则更新运输记录到批次
                    commonDAO.InsertWaitForHandleEvent("汽车智能化_同步入厂煤运输记录到批次", transport.Id);
                    commonDAO.RemoveUnFinishTransport(transport.Id);
                }
            }

            return SelfDber.Update(transport) > 0;
        }

        #endregion

        #region 其他物资

        /// <summary>
        /// 根据标签号id查找其他物资运输记录
        /// </summary>
        /// <param name="tagId">标签Id</param>
        /// <returns></returns>
        public CmcsGoodsTransport GetGoodsTransportByEpcId(string epcId)
        {
            return SelfDber.Entity<CmcsGoodsTransport>("where AUTOTRUCKID=(select id from cmcstbautotruck where epccardid=:EpcId)", new { EpcId = epcId });
        }

        /// <summary>
        /// 保存其他物资运输记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="weight">重量</param>
        /// <param name="place"></param>
        /// <returns></returns>
        public bool SaveGoodsTransport(CmcsGoodsTransport transport)
        {
            if (transport == null) return false;
            if (transport.FirstWeight > 0)
            {
                if (transport.FirstTime.Year < 2000) transport.FirstTime = DateTime.Now;
                if (transport.SecondWeight > 0)
                {
                    if (transport.SecondTime.Year < 2000) transport.SecondTime = DateTime.Now;
                    transport.SuttleWeight = Math.Abs(transport.FirstWeight - transport.SecondWeight);
                    transport.IsFinish = 1;
                    //回皮则更新运输记录到批次
                    commonDAO.InsertWaitForHandleEvent("汽车智能化_同步其他物资运输记录到批次", transport.Id);
                    commonDAO.RemoveUnFinishTransport(transport.Id);
                }
            }

            return SelfDber.Update(transport) > 0;
        }

        #endregion
    }
}