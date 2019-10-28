using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.AssayDevice.Entities;
using CMCS.Common;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.DAO;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice
{
    public class EquAssayDeviceDAO
    {
        private static EquAssayDeviceDAO instance;

        public static EquAssayDeviceDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new EquAssayDeviceDAO();
            }
            return instance;
        }

        private EquAssayDeviceDAO()
        {

        }

        CommonDAO commonDAO = CommonDAO.GetInstance();

        /// <summary>
        /// 生成标准测硫仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToSulfurStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;

            // .测硫仪 型号：5E-8SAII
            foreach (CLY_5E8SAII entity in Dbers.GetInstance().SelfDber.Entities<CLY_5E8SAII>("where CSRQ>= :TestTime and SHYM is not null", new { TestTime = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                CmcsSulfurStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsSulfurStdAssay>("where PKID=:PKID", new { PKID = entity.PKID });
                if (item == null)
                {
                    item = new CmcsSulfurStdAssay();
                    item.SampleNumber = entity.SHYM;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = 0;
                    item.Stad = entity.GJQL;
                    item.AssayUser = entity.HYY;
                    item.AssayTime = entity.CSRQ;
                    item.OrderNumber = 0;
                    item.IsEffective = 0;
                    item.PKID = entity.PKID;

                    res += Dbers.GetInstance().SelfDber.Insert<CmcsSulfurStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.SHYM;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = 0;
                    item.Stad = entity.GJQL;
                    item.AssayUser = entity.HYY;
                    item.AssayTime = entity.CSRQ;
                    item.OrderNumber = 0;

                    res += Dbers.GetInstance().SelfDber.Update<CmcsSulfurStdAssay>(item);
                }
            }

            output(string.Format("生成标准测硫仪数据 {0} 条", res), eOutputType.Normal);

            return res;
        }

        /// <summary>
        /// 生成标准量热仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToHeatStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;

            // .量热仪 型号：5E-C5500A双控
            foreach (LRY_5EC5500A entity in Dbers.GetInstance().SelfDber.Entities<LRY_5EC5500A>("where TestTime>= :TestTime and Mancoding is not null", new { TestTime = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                CmcsHeatStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsHeatStdAssay>("where PKID=:PKID", new { PKID = entity.PKID });
                if (item == null)
                {
                    item = new CmcsHeatStdAssay();
                    item.SampleNumber = entity.Mancoding;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = Convert.ToDecimal(entity.Weight);
                    item.Qbad = Convert.ToDecimal(entity.Qb);
                    item.AssayUser = entity.Testman;
                    item.AssayTime = entity.TestTime;
                    item.IsEffective = 0;
                    item.PKID = entity.PKID;

                    res += Dbers.GetInstance().SelfDber.Insert<CmcsHeatStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.Mancoding;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = Convert.ToDecimal(entity.Weight);
                    item.Qbad = Convert.ToDecimal(entity.Qb);
                    item.AssayUser = entity.Testman;
                    item.AssayTime = entity.TestTime;

                    res += Dbers.GetInstance().SelfDber.Update<CmcsHeatStdAssay>(item);
                }

            }

            output(string.Format("生成标准量热仪数据 {0} 条", res), eOutputType.Normal);

            return res;
        }

        /// <summary>
        /// 保存标准水分仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToMoistureStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;

            // .水分仪 型号：5E-MW6510
            foreach (SFY_5EMW6510 entity in Dbers.GetInstance().SelfDber.Entities<SFY_5EMW6510>("where BeginDate>= :TestTime and SampleName is not null", new { TestTime = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                string pkid = entity.PKID;

                CmcsMoistureStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsMoistureStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsMoistureStdAssay();
                    item.SampleNumber = entity.SampleName;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.Sample;
                    item.WaterPer = entity.Moisture;
                    item.AssayUser = entity.Operator;
                    item.IsEffective = 0;
                    item.PKID = pkid;
                    item.AssayTime = entity.BeginDate;
                    item.WaterType = entity.Content.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsMoistureStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.SampleName;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.Sample;
                    item.WaterPer = entity.Moisture;
                    item.AssayUser = entity.Operator;
                    item.AssayTime = entity.BeginDate;
                    item.WaterType = entity.Content.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Update<CmcsMoistureStdAssay>(item);
                }
            }
            output(string.Format("生成标准水分仪数据 {0} 条", res), eOutputType.Normal);
            return res;
        }
    }
}
