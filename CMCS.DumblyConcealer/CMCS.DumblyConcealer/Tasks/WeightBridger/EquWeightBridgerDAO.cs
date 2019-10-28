﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.WeightBridger.Entities;
using CMCS.Common;
using CMCS.Common.Entities;
using CMCS.Common.DAO;
using CMCS.Common.Enums;
using CMCS.Common.Entities.TrainInFactory;
using CMCS.Common.Entities.Fuel;

namespace CMCS.DumblyConcealer.Tasks.WeightBridger
{
    public class EquWeightBridgerDAO
    {
        private static EquWeightBridgerDAO instance;

        public static EquWeightBridgerDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new EquWeightBridgerDAO();
            }
            return instance;
        }

        private EquWeightBridgerDAO()
        {

        }

        /// <summary>
        /// 同步轨道衡过衡数据，并在火车出厂后将皮重回写
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SyncLwCarsInfo(Action<string, eOutputType> output)
        {
            int res = 0;
            
            List<Lwcarsinfo> list = DcDbers.GetInstance().WeightBridger_Dber.Entities<Lwcarsinfo>("Where TrainsDate>=@TrainsDate and FaZhan<>'' and FaZhan<>'中转' and JingZhong>10 order by TrainsDate asc,TrainsTime asc,Number asc", new { TrainsDate = DateTime.Now.AddDays(- CommonDAO.GetInstance().GetAppletConfigInt32("轨道衡数据读取天数")).Date });
            foreach (Lwcarsinfo entity in list)
            {
                string pKId = entity.ID + "-" + entity.CheHao + "-" + entity.TrainsDate.ToShortDateString() + "-" + entity.TrainsTime;

                CmcsTrainWeightRecord trainWeightRecord = Dbers.GetInstance().SelfDber.Entity<CmcsTrainWeightRecord>("where PKID=:PKID", new { PKID = pKId });
                if (trainWeightRecord == null)
                {
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsTrainWeightRecord>(
                        new CmcsTrainWeightRecord
                        {
                            SupplierName = entity.FaHuoDanWei,
                            MineName = "",
                            FuelKind = "",
                            StationName = entity.FaZhan,
                            PKID = pKId,
                            MachineCode = GlobalVars.MachineCode_GDH_1,
                            TrainNumber = entity.CheHao,
                            TrainType = entity.CheXing,
                            TicketWeight = entity.PiaoZhong,
                            GrossWeight = entity.MaoZhong,
                            SkinWeight = entity.PiZhong,
                            StandardWeight = entity.JingZhong,
                            Speed = entity.SuDu,
                            MesureMan = entity.JiLiangYuan,
                            ArriveTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime),
                            GrossTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime),
                            SkinTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime),
                            LeaveTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime),
                            DataFlag = 0
                        }
                  );
                }
                else
                {
                    // 判断轨道衡记录中的重量信息是否变更，条件成立则更新批次明细
                    if (trainWeightRecord.TicketWeight != entity.PiaoZhong || trainWeightRecord.GrossWeight != entity.MaoZhong
                        || trainWeightRecord.SkinWeight != entity.PiZhong)
                    {
                        CmcsTransport transport = Dbers.GetInstance().SelfDber.Entity<CmcsTransport>("where PKID=:PKID", new { PKID = trainWeightRecord.Id });
                        if (transport != null)
                        {
                            // 净重=毛重-皮重
                            // 验收量=净重-扣矸-扣水
                            // 盈亏=验收量-矿发量
                            transport.TicketWeight = entity.PiaoZhong;
                            transport.GrossWeight = entity.MaoZhong;
                            transport.SkinWeight = entity.PiZhong;
                            transport.StandardWeight = entity.JingZhong;
                            transport.CheckQty = transport.StandardWeight - transport.KgWeight;
                            transport.MarginWeight = transport.StandardWeight - transport.CheckQty;

                            Dbers.GetInstance().SelfDber.Update(transport);
                        }
                    }

                    trainWeightRecord.SupplierName = entity.FaHuoDanWei;
                    trainWeightRecord.MineName = "";
                    trainWeightRecord.FuelKind = "";
                    trainWeightRecord.StationName = entity.FaZhan;
                    if (!String.IsNullOrEmpty(entity.CheHao)) trainWeightRecord.TrainNumber = entity.CheHao;
                    trainWeightRecord.TrainType = entity.CheXing;
                    trainWeightRecord.TicketWeight = entity.PiaoZhong;
                    trainWeightRecord.GrossWeight = entity.MaoZhong;
                    trainWeightRecord.SkinWeight = entity.PiZhong;
                    trainWeightRecord.StandardWeight = entity.JingZhong;
                    trainWeightRecord.Speed = entity.SuDu;
                    trainWeightRecord.MesureMan = entity.JiLiangYuan;
                    trainWeightRecord.ArriveTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime);
                    trainWeightRecord.GrossTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime);
                    trainWeightRecord.SkinTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime);
                    trainWeightRecord.LeaveTime = Convert.ToDateTime(entity.TrainsDate.ToShortDateString() + " " + entity.TrainsTime);
                    trainWeightRecord.DataFlag = 0;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsTrainWeightRecord>(trainWeightRecord);
                }
                if (res > 0 && String.IsNullOrEmpty(entity.CheHao))
                {
                    CommonDAO.GetInstance().SaveSysMessage(eMessageType.轨道衡.ToString(), "车号为空请补录!", eMessageType.轨道衡.ToString());
                }
            }
            output(string.Format("同步轨道衡数据 {0} 条（集中管控 > 第三方）", res), eOutputType.Normal);
            return res;
        }
    }
}