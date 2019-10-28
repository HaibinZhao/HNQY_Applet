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
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Enums;
using CMCS.Common.Entities.iEAA;

namespace CMCS.CarTransport.DAO
{
    /// <summary>
    /// 汽车入厂排队业务
    /// </summary>
    public class QueuerDAO
    {
        private static QueuerDAO instance;

        public static QueuerDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new QueuerDAO();
            }

            return instance;
        }

        private QueuerDAO()
        { }

        public OracleDapperDber SelfDber
        {
            get { return Dbers.GetInstance().SelfDber; }
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();
        CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();

        #region 入厂煤业务

        /// <summary>
        /// 生成入厂煤运输排队记录，同时生成批次信息以及采制化三级编码
        /// </summary>
        /// <param name="autotruck">车</param>
        /// <param name="supplier">供煤单位</param>
        /// <param name="mine">矿点</param>
        /// <param name="transportCompany">运输单位</param>
        /// <param name="fuelKind">煤种</param>
        /// <param name="ticketWeight">矿发量</param>
        /// <param name="inFactoryTime">入厂时间</param>
        /// <param name="remark">备注</param>
        /// <param name="place">地点</param>
        /// <param name="samplingType">采样方式</param> 
        /// <param name="isAutoDeduct">是否自动扣吨</param>
        /// <param name="isCountsSampler">是否分配至多台采样机</param>
        /// <returns></returns>
        public bool JoinQueueBuyFuelTransport(CmcsAutotruck autotruck, CmcsSupplier supplier, CmcsMine mine, CmcsTransportCompany transportCompany, CmcsFuelKind fuelKind, decimal ticketWeight, DateTime inFactoryTime, string remark, string place, string samplingType, string cmbSampling, bool isAutoDeduct, bool isCountsSampler, CmcsLMYB lmYb = null)
        {
            CmcsBuyFuelTransport transport = new CmcsBuyFuelTransport
            {
                SerialNumber = carTransportDAO.CreateNewTransportSerialNumber(eCarType.入厂煤, inFactoryTime),
                AutotruckId = autotruck.Id,
                CarNumber = autotruck.CarNumber,
                SupplierId = supplier.Id,
                SupplierName = supplier.Name,
                MineId = mine.Id,
                MineName = mine.Name,
                TransportCompanyId = transportCompany.Id,
                TransportCompanyName = transportCompany.Name,
                FuelKindId = fuelKind.Id,
                FuelKindName = fuelKind.FuelName,
                TicketWeight = ticketWeight,
                InFactoryTime = inFactoryTime,
                IsFinish = 0,
                IsUse = 1,
                SamplingType = samplingType,
                FPSamplePlace = cmbSampling,
                StepName = eTruckInFactoryStep.入厂.ToString(),
                Remark = remark,
                IsAutoDeduct = isAutoDeduct ? 1 : 0,
                LMYBId = lmYb != null ? lmYb.Id : "",
                YbNum = lmYb != null ? lmYb.YbNum : ""
            };
            //transport.FPSamplePlace = CarSamplerDAO.GetInstance().FPSampleByBuyFuelTransport(transport);
            // 生成批次以及采制化三级编码数据 
            CmcsInFactoryBatch inFactoryBatch = carTransportDAO.GCQCInFactoryBatchByBuyFuelTransport(transport);
            if (inFactoryBatch != null)
            {

                //生产采样方案
                CmcsSamplingScheme SamplingScheme = SelfDber.Entity<CmcsSamplingScheme>("where INFACTORYBATCHID=:INFACTORYBATCHID", new { INFACTORYBATCHID = inFactoryBatch.Id });
                if (SamplingScheme == null)
                {
                    Fultbtransfer CmcsLMYB = SelfDber.Entity<Fultbtransfer>("where to_char(InFactoryTime,'yyyy-MM-dd')=:CreateDate and SupplierId=:SupplierId and FuelKindId=:FuelKindId and MineId=:MineId and TRANSFERTYPE='公路'", new { CreateDate = DateTime.Now.ToString("yyyy-MM-dd"), SupplierId = supplier.Id, MineId = mine.Id, FuelKindId = fuelKind.Id });
                    if (CmcsLMYB != null)
                    {
                        CmcsSamplingScheme CmcsSamplingScheme = new CmcsSamplingScheme
                        {
                            TRANSFERID = CmcsLMYB.Id,
                            PointCount = getPointCount(CmcsLMYB.TRANSFERNUMBER),
                            INFACTORYBATCHID = inFactoryBatch.Id
                        };
                        SelfDber.Insert(CmcsSamplingScheme);
                    }
                }

                if (SelfDber.Insert(transport) > 0)
                {
                    // 插入未完成运输记录
                    return SelfDber.Insert(new CmcsUnFinishTransport
                    {
                        TransportId = transport.Id,
                        CarType = eCarType.入厂煤.ToString(),
                        AutotruckId = autotruck.Id,
                        PrevPlace = place,
                    }) > 0;
                }
            }



            return false;
        }

        /// <summary>
        /// 根据预报车数得到每个车采样点数
        /// </summary>
        /// <param name="carCount"></param>
        /// <returns></returns>
        private int getPointCount(Decimal carCount)
        {
            int PointCount = 3;
            if (carCount == 1)
            {
                PointCount = 18;
            }
            else if (carCount == 2)
            {

                PointCount = 9;
            }
            else if (carCount > 2 && carCount < 9)
            {

                PointCount = 6;
            }
            else
            {
                PointCount = 3;
            }

            return PointCount;
        }

        /// <summary>
        /// 获取指定日期已完成的入厂煤运输记录
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<View_BuyFuelTransport> GetFinishedBuyFuelTransport(DateTime dtStart, DateTime dtEnd)
        {
            return SelfDber.Entities<View_BuyFuelTransport>("where IsFinish=1 and InFactoryTime>=:dtStart and InFactoryTime<:dtEnd order by InFactoryTime desc", new { dtStart = dtStart, dtEnd = dtEnd });
        }

        /// <summary>
        /// 获取未完成的入厂煤运输记录
        /// </summary>
        /// <returns></returns>
        public List<View_BuyFuelTransport> GetUnFinishBuyFuelTransport()
        {
            return SelfDber.Entities<View_BuyFuelTransport>("where IsFinish=0 and IsUse=1 and UnFinishTransportId is not null order by InFactoryTime desc");
        }

        /// <summary>
        /// 更改入厂煤运输记录的无效状态
        /// </summary>
        /// <param name="buyFuelTransportId"></param>
        /// <param name="isValid">是否有效</param>
        /// <returns></returns>
        public bool ChangeBuyFuelTransportToInvalid(string buyFuelTransportId, bool isValid)
        {
            if (isValid)
            {
                // 设置为有效
                CmcsBuyFuelTransport buyFuelTransport = SelfDber.Get<CmcsBuyFuelTransport>(buyFuelTransportId);
                if (buyFuelTransport != null)
                {
                    if (SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsBuyFuelTransport>() + " set IsUse=1 where Id=:Id", new { Id = buyFuelTransportId }) > 0)
                    {
                        if (buyFuelTransport.IsFinish == 0)
                        {
                            SelfDber.Insert(new CmcsUnFinishTransport
                            {
                                AutotruckId = buyFuelTransport.AutotruckId,
                                CarType = eCarType.入厂煤.ToString(),
                                TransportId = buyFuelTransport.Id,
                                PrevPlace = "未知"
                            });
                        }

                        return true;
                    }
                }
            }
            else
            {
                // 设置为无效

                if (SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsBuyFuelTransport>() + " set IsUse=0 where Id=:Id", new { Id = buyFuelTransportId }) > 0)
                {
                    SelfDber.DeleteBySQL<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = buyFuelTransportId });

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据车牌号获取指定到达日期的入厂煤来煤预报
        /// </summary>
        /// <param name="carNumber">车牌号</param>
        /// <param name="inFactoryTime">预计到达日期</param>
        /// <returns></returns>
        public List<CmcsLMYB> GetBuyFuelForecastByCarNumber(string carNumber, DateTime inFactoryTime)
        {
            return SelfDber.Query<CmcsLMYB>("select l.* from " + EntityReflectionUtil.GetTableName<CmcsLMYBDetail>() + " ld left join " + EntityReflectionUtil.GetTableName<CmcsLMYB>() + " l on l.Id=ld.lmybid where ld.CarNumber=:CarNumber and to_char(InFactoryTime,'yyyymmdd')=to_char(:InFactoryTime,'yyyymmdd') order by l.InFactoryTime desc",
                new { CarNumber = carNumber.Trim(), InFactoryTime = inFactoryTime }).ToList();
        }

        #endregion


        #region 其他物资业务

        /// <summary>
        /// 生成其他物资运输排队记录
        /// </summary>
        /// <param name="autotruck">车辆</param>
        /// <param name="supply">供货单位</param>
        /// <param name="receive">收货单位</param>
        /// <param name="goodsType">物资类型</param>
        /// <param name="inFactoryTime">入厂时间</param>
        /// <param name="remark">备注</param>
        /// <param name="place">地点</param>
        /// <returns></returns>
        public bool JoinQueueGoodsTransport(CmcsAutotruck autotruck, CmcsSupplyReceive supply, CmcsSupplyReceive receive, CmcsGoodsType goodsType, DateTime inFactoryTime, string remark, string place)
        {
            CmcsGoodsTransport transport = new CmcsGoodsTransport
            {
                SerialNumber = carTransportDAO.CreateNewTransportSerialNumber(eCarType.其他物资, inFactoryTime),
                AutotruckId = autotruck.Id,
                CarNumber = autotruck.CarNumber,
                SupplyUnitId = supply.Id,
                SupplyUnitName = supply.UnitName,
                ReceiveUnitId = receive.Id,
                ReceiveUnitName = receive.UnitName,
                GoodsTypeId = goodsType.Id,
                GoodsTypeName = goodsType.GoodsName,
                InFactoryTime = inFactoryTime,
                IsFinish = 0,
                IsUse = 1,
                StepName = eTruckInFactoryStep.入厂.ToString(),
                Remark = remark
            };

            if (SelfDber.Insert(transport) > 0)
            {
                // 插入未完成运输记录
                return SelfDber.Insert(new CmcsUnFinishTransport
                {
                    TransportId = transport.Id,
                    CarType = eCarType.其他物资.ToString(),
                    AutotruckId = autotruck.Id,
                    PrevPlace = place,
                }) > 0;
            }

            return false;
        }

        /// <summary>
        /// 获取指定日期已完成的其他物资运输记录
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<CmcsGoodsTransport> GetFinishedGoodsTransport(DateTime dtStart, DateTime dtEnd)
        {
            return SelfDber.Entities<CmcsGoodsTransport>("where IsFinish=1 and InFactoryTime>=:dtStart and InFactoryTime<:dtEnd order by InFactoryTime desc", new { dtStart = dtStart, dtEnd = dtEnd });
        }

        /// <summary>
        /// 获取未完成的其他物资运输记录
        /// </summary>
        /// <returns></returns>
        public List<CmcsGoodsTransport> GetUnFinishGoodsTransport()
        {
            return SelfDber.Entities<CmcsGoodsTransport>("where IsFinish=0 and IsUse=1 and Id in (select TransportId from " + EntityReflectionUtil.GetTableName<CmcsUnFinishTransport>() + " where CarType=:CarType) order by InFactoryTime desc", new { CarType = eCarType.其他物资.ToString() });
        }

        /// <summary>
        /// 更改其他物资运输记录的无效状态
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="isValid">是否有效</param>
        /// <returns></returns>
        public bool ChangeGoodsTransportToInvalid(string transportId, bool isValid)
        {
            if (isValid)
            {
                // 设置为有效
                CmcsGoodsTransport buyFuelTransport = SelfDber.Get<CmcsGoodsTransport>(transportId);
                if (buyFuelTransport != null)
                {
                    if (SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsGoodsTransport>() + " set IsUse=1 where Id=:Id", new { Id = transportId }) > 0)
                    {
                        if (buyFuelTransport.IsFinish == 0)
                        {
                            SelfDber.Insert(new CmcsUnFinishTransport
                            {
                                AutotruckId = buyFuelTransport.AutotruckId,
                                CarType = eCarType.其他物资.ToString(),
                                TransportId = buyFuelTransport.Id,
                                PrevPlace = "未知"
                            });
                        }

                        return true;
                    }
                }
            }
            else
            {
                // 设置为无效

                if (SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsGoodsTransport>() + " set IsUse=0 where Id=:Id", new { Id = transportId }) > 0)
                {
                    SelfDber.DeleteBySQL<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = transportId });

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 来访车辆业务

        /// <summary>
        /// 生成来访车辆运输排队记录
        /// </summary>
        /// <param name="autotruck">车辆</param> 
        /// <param name="inFactoryTime">入厂时间</param>
        /// <param name="remark">备注</param>
        /// <param name="place">地点</param>
        /// <returns></returns>
        public bool JoinQueueVisitTransport(CmcsAutotruck autotruck, DateTime inFactoryTime, string remark, string place)
        {
            CmcsVisitTransport transport = new CmcsVisitTransport
            {
                SerialNumber = carTransportDAO.CreateNewTransportSerialNumber(eCarType.来访车辆, inFactoryTime),
                AutotruckId = autotruck.Id,
                CarNumber = autotruck.CarNumber,
                InFactoryTime = inFactoryTime,
                IsFinish = 0,
                IsUse = 1,
                StepName = eTruckInFactoryStep.入厂.ToString(),
                Remark = remark
            };

            if (SelfDber.Insert(transport) > 0)
            {
                // 插入未完成运输记录
                return SelfDber.Insert(new CmcsUnFinishTransport
                {
                    TransportId = transport.Id,
                    CarType = eCarType.来访车辆.ToString(),
                    AutotruckId = autotruck.Id,
                    PrevPlace = place,
                }) > 0;
            }

            return false;
        }

        /// <summary>
        /// 获取指定日期已完成的来访车辆运输记录
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<CmcsVisitTransport> GetFinishedVisitTransport(DateTime dtStart, DateTime dtEnd)
        {
            return SelfDber.Entities<CmcsVisitTransport>("where IsFinish=1 and InFactoryTime>=:dtStart and InFactoryTime<:dtEnd order by InFactoryTime desc", new { dtStart = dtStart, dtEnd = dtEnd });
        }

        /// <summary>
        /// 获取未完成的来访车辆运输记录
        /// </summary>
        /// <returns></returns>
        public List<CmcsVisitTransport> GetUnFinishVisitTransport()
        {
            return SelfDber.Entities<CmcsVisitTransport>("where IsFinish=0 and IsUse=1 and Id in (select TransportId from " + EntityReflectionUtil.GetTableName<CmcsUnFinishTransport>() + " where CarType=:CarType) order by InFactoryTime desc", new { CarType = eCarType.来访车辆.ToString() });
        }

        /// <summary>
        /// 更改来访车辆运输记录的无效状态
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="isValid">是否有效</param>
        /// <returns></returns>
        public bool ChangeVisitTransportToInvalid(string transportId, bool isValid)
        {
            if (isValid)
            {
                // 设置为有效
                CmcsVisitTransport buyFuelTransport = SelfDber.Get<CmcsVisitTransport>(transportId);
                if (buyFuelTransport != null)
                {
                    if (SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsVisitTransport>() + " set IsUse=1 where Id=:Id", new { Id = transportId }) > 0)
                    {
                        if (buyFuelTransport.IsFinish == 0)
                        {
                            SelfDber.Insert(new CmcsUnFinishTransport
                            {
                                AutotruckId = buyFuelTransport.AutotruckId,
                                CarType = eCarType.来访车辆.ToString(),
                                TransportId = buyFuelTransport.Id,
                                PrevPlace = "未知"
                            });
                        }

                        return true;
                    }
                }
            }
            else
            {
                // 设置为无效

                if (SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsVisitTransport>() + " set IsUse=0 where Id=:Id", new { Id = transportId }) > 0)
                {
                    SelfDber.DeleteBySQL<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = transportId });

                    return true;
                }
            }

            return false;
        }

        #endregion
        #region 系统管理

        /// <summary>
        /// 生成模块编码
        /// </summary>
        /// <returns></returns>
        public string CreateModuleno()
        {
            string moduleno = "0000";

            SysModule entity = SelfDber.Entities<SysModule>(" order by moduleno desc").FirstOrDefault();
            if (entity != null && !string.IsNullOrEmpty(entity.Moduleno))
            {
                int Count = Convert.ToInt32(entity.Moduleno) + 1;
                moduleno = Count.ToString().PadLeft(4, '0');
                return moduleno;
            }
            else
            {
                return moduleno;
            }
        }

        /// <summary>
        /// 生成功能编码
        /// </summary>
        /// <returns></returns>
        public string CreateResourceno(SysModule module, out int orderno)
        {
            string moduleNo = module.Moduleno;
            List<SysResource> entitys = SelfDber.Entities<SysResource>("where moduleid='" + module.Id + "' order by orderno desc");
            if (entitys.Count > 0)
            {
                SysResource resourceFirst = entitys[0];
                int Count = Convert.ToInt32(resourceFirst.Resno.Replace(moduleNo, "")) + 1;
                orderno = resourceFirst.OrderNO + 1;
                return moduleNo + Count.ToString().PadLeft(2, '0');
            }
            else
            {
                orderno = 0;
                return moduleNo = moduleNo + "01";
            }
        }

        /// <summary>
        /// 得到模块功能，没有就返回默认
        /// </summary>
        /// <param name="SysModule"></param>
        /// <returns></returns>
        public List<SysResource> GetResources(SysModule module, bool isInit)
        {
            List<SysResource> listResource = new List<SysResource>();
            if (isInit)
            {
                SysResource resource = new SysResource();
                resource.Resno = module.Moduleno + "01";
                resource.ResName = "查看";
                resource.ModuleId = module.Id;
                resource.OrderNO = 0;
                listResource.Add(resource);
                resource = new SysResource();
                resource.Resno = module.Moduleno + "02";
                resource.ResName = "新增";
                resource.ModuleId = module.Id;
                resource.OrderNO = 1;
                listResource.Add(resource);
                resource = new SysResource();
                resource.Resno = module.Moduleno + "03";
                resource.ResName = "修改";
                resource.ModuleId = module.Id;
                resource.OrderNO = 2;
                listResource.Add(resource);
                resource = new SysResource();
                resource.Resno = module.Moduleno + "04";
                resource.ResName = "删除";
                resource.ModuleId = module.Id;
                resource.OrderNO = 3;
                listResource.Add(resource);
            }
            else
                listResource = SelfDber.Entities<SysResource>("where moduleid='" + module.Id + "'");

            return listResource;
        }

        /// <summary>
        /// 判断是否有权限
        /// </summary>
        /// <returns></returns>
        public bool CheckPower(string ModuleDll, string ResourceResno, User CurrentLoginUser)
        {
            //超级管理员不需要判断权限
            if (CurrentLoginUser.USERKIND == "超级用户")
                return true;

            SysModule module = SelfDber.Entity<SysModule>("where ModuleDll=:ModuleDll", new { ModuleDll = ModuleDll });
            if (module != null)
            {
                SysResource resource = SelfDber.Entity<SysResource>("where ModuleId=:ModuleId and Resno=:Resno", new { ModuleId = module.Id, Resno = module.Moduleno + ResourceResno });
                if (resource != null)
                    return SelfDber.Entity<SysResourceUser>("where ResourceId=:ResourceId and UserId=:UserId", new { ResourceId = resource.Id, UserId = CurrentLoginUser.PartyId }) == null ? false : true;
            }
            return false;
        }
        #endregion
    }
}
