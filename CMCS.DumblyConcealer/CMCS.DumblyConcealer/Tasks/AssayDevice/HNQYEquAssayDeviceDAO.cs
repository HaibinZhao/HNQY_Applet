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
    public class HNQYEquAssayDeviceDAO
    {
        private static HNQYEquAssayDeviceDAO instance;

        public static HNQYEquAssayDeviceDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new HNQYEquAssayDeviceDAO();
            }
            return instance;
        }

        private HNQYEquAssayDeviceDAO()
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
            foreach (HNQYCly_SDS212 entity in Dbers.GetInstance().SelfDber.Entities<HNQYCly_SDS212>("where TestDate>= :TestDate and Name is not null", new { TestDate = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                CmcsSulfurStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsSulfurStdAssay>("where PKID=:PKID", new { PKID = entity.PKID });
                if (item == null)
                {
                    item = new CmcsSulfurStdAssay();
                    item.SampleNumber = entity.Name;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.Weight;
                    item.Stad = entity.Stad;
                    item.AssayUser = entity.Tester;
                    item.AssayTime = entity.TestDate;
                    item.OrderNumber = 0;
                    item.IsEffective = 0;
                    item.PKID = entity.PKID;

                    res += Dbers.GetInstance().SelfDber.Insert<CmcsSulfurStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.Name;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.Weight;
                    item.Stad = entity.Stad;
                    item.AssayUser = entity.Tester;
                    item.AssayTime = entity.TestDate;
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
            foreach (HNQYLry_SDC entity in Dbers.GetInstance().SelfDber.Entities<HNQYLry_SDC>("where TestDate>= :TestDate and ManualNumber is not null", new { TestDate = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                CmcsHeatStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsHeatStdAssay>("where PKID=:PKID", new { PKID = entity.PKID });
                if (item == null)
                {
                    item = new CmcsHeatStdAssay();
                    item.SampleNumber = entity.ManualNumber;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = Convert.ToDecimal(entity.SampleWeight);
                    item.Qbad = Convert.ToDecimal(entity.Qbad);
                    item.AssayUser = entity.Tester;
                    item.AssayTime = entity.TestDate;
                    item.IsEffective = 0;
                    item.PKID = entity.PKID;

                    res += Dbers.GetInstance().SelfDber.Insert<CmcsHeatStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.ManualNumber;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = Convert.ToDecimal(entity.SampleWeight);
                    item.Qbad = Convert.ToDecimal(entity.Qbad);
                    item.AssayUser = entity.Tester;
                    item.AssayTime = entity.TestDate;

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
            foreach (HNQYSfy_PxmAData entity in Dbers.GetInstance().SelfDber.Entities<HNQYSfy_PxmAData>("where TestDate>= :TestDate and SampleNo is not null", new { TestDate = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                string pkid = entity.PKID;

                CmcsMoistureStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsMoistureStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsMoistureStdAssay();
                    item.SampleNumber = entity.SampleNo;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.MTare;
                    item.SampleWeight = entity.MMass;
                    item.WaterPer = entity.Mt;
                    item.AssayUser = entity.TestMan;
                    item.IsEffective = 0;
                    item.PKID = pkid;
                    item.AssayTime = entity.TestDate;
                    item.WaterType = entity.MType.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsMoistureStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.SampleNo;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.MTare;
                    item.SampleWeight = entity.MMass;
                    item.WaterPer = entity.Mt;
                    item.AssayUser = entity.TestMan;
                    item.AssayTime = entity.TestDate;
                    item.WaterType = entity.MType.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Update<CmcsMoistureStdAssay>(item);
                }
            }
            output(string.Format("生成标准水分仪数据 {0} 条", res), eOutputType.Normal);
            return res;
        }

        /// <summary>
        /// 保存标准工分仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToProximateStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;

            // .水分仪 型号：5E-MW6510
            foreach (HNQYGfy_PxmAData2018 entity in Dbers.GetInstance().SelfDber.Entities<HNQYGfy_PxmAData2018>("where TestDate>= :TestDate and SampleNo is not null", new { TestDate = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                string pkid = entity.PKID;

                CmcsProximateStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsProximateStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsProximateStdAssay();
                    item.SampleNumber = entity.SampleNo;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.MTare;
                    item.SampleWeight = entity.MMass;
                    item.VMass = entity.VMass;
                    item.VTare = entity.VTare;
                    item.VBurned = entity.VBurned;
                    item.Mad = entity.Mad;
                    item.Vad = entity.Vad;
                    item.Aad = entity.Aad;
                    item.AssayUser = entity.TestMan;
                    item.IsEffective = 0;
                    item.PKID = pkid;
                    item.AssayTime = entity.TestDate;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsProximateStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.SampleNo;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.MTare;
                    item.SampleWeight = entity.MMass;
                    item.VMass = entity.VMass;
                    item.VTare = entity.VTare;
                    item.VBurned = entity.VBurned;
                    item.Mad = entity.Mad;
                    item.Vad = entity.Vad;
                    item.Aad = entity.Aad;
                    item.AssayUser = entity.TestMan;
                    item.AssayTime = entity.TestDate;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsProximateStdAssay>(item);
                }
            }
            output(string.Format("生成标准工分仪数据 {0} 条", res), eOutputType.Normal);
            return res;
        }



        /// <summary>
        /// 保存元素分析仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToElementStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;

            // .水分仪 型号：5E-MW6510
            foreach (HNQYysfxy_Data entity in Dbers.GetInstance().SelfDber.Entities<HNQYysfxy_Data>("where TestDate>= :TestDate and SampleNo is not null", new { TestDate = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                string pkid = entity.PKID;

                CmcsElementStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsElementStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsElementStdAssay();
                    item.SAMPLENUMBER = entity.SampleNo;
                    item.FACILITYNUMBER = entity.MachineCode;
                    item.CONTAINERWEIGHT = 0;
                    item.SAMPLEWEIGHT =Decimal.Parse(entity.SAMPLEWEIGHT);
                    item.NAD = Decimal.Parse(entity.NAD);
                    item.HAD = Decimal.Parse(entity.HAD);
                    item.CAD = Decimal.Parse(entity.CAD);
                    item.ASSAYUSER = entity.TESTMAN;
                    item.IsEffective = 0;
                    item.PKID = pkid;
                    item.ASSAYTIME = entity.TestDate;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsElementStdAssay>(item);
                }
                else
                {
                    item.SAMPLENUMBER = entity.SampleNo;
                    item.FACILITYNUMBER = entity.MachineCode;
                    item.CONTAINERWEIGHT = 0;
                    item.SAMPLEWEIGHT = Decimal.Parse(entity.SAMPLEWEIGHT);
                    item.NAD = Decimal.Parse(entity.NAD);
                    item.HAD = Decimal.Parse(entity.HAD);
                    item.CAD = Decimal.Parse(entity.CAD);
                    item.ASSAYUSER = entity.TESTMAN;
                    item.ASSAYTIME = entity.TestDate;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsElementStdAssay>(item);
                }
            }
            output(string.Format("生成元素分析仪数据 {0} 条", res), eOutputType.Normal);
            return res;
        }



        /// <summary>
        /// 保存灰融仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToFusionPointStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;

            // .水分仪 型号：5E-MW6510
            foreach (HNQYHry_SDAF2000 entity in Dbers.GetInstance().SelfDber.Entities<HNQYHry_SDAF2000>("where SYRQ>= :SYRQ and MANNO is not null", new { SYRQ = DateTime.Now.AddDays(-Convert.ToInt32(commonDAO.GetAppletConfigString("化验设备数据读取天数"))).Date }))
            {
                string pkid = entity.PKID;

                CmcsFusionPointStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsFusionPointStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsFusionPointStdAssay();
                    item.SampleNumber = entity.MANNO;
                    item.FacilityNumber = entity.MachineCode;
                    item.DT = entity.DTWD;
                    item.ST = entity.STWD;
                    item.HT = entity.HTWD;
                    item.FT = entity.FTWD;
                    //item.NAD = Decimal.Parse(entity.NAD);
                    //item.HAD = Decimal.Parse(entity.HAD);
                    //item.CAD = Decimal.Parse(entity.CAD);
                    //item.ASSAYUSER = entity.TESTMAN;
                    item.IsEffective = 0;
                    item.PKID = pkid;
                    item.AssayTime = entity.SYRQ;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsFusionPointStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.MANNO;
                    item.FacilityNumber = entity.MachineCode;
                    item.DT = entity.DTWD;
                    item.ST = entity.STWD;
                    item.HT = entity.HTWD;
                    item.FT = entity.FTWD;
                    item.AssayTime = entity.SYRQ;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsFusionPointStdAssay>(item);
                }
            }
            output(string.Format("生成元素分析仪数据 {0} 条", res), eOutputType.Normal);
            return res;
        }




        #region 保存化验室网络信息点

        /// <summary>
        /// 保存化验室网络信息点
        /// </summary>
        /// <returns></returns>
        public void SaveHYSWLSingalData(Action<string, eOutputType> output)
        {
            output(string.Format("开始处理化验室网络信息点"), eOutputType.Normal);
            IList<AssayTemp> reslist = new List<AssayTemp>();
            var list = Dbers.GetInstance().SelfDber.Entities<CmcsRCAssay>("where AssayDate>=:st and AssayDate<:et order by AssayDate asc ", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            for (int i = 0; i < list.Count; i++)
            {
                AssayTemp temp = new AssayTemp();
                temp.AssayCode = list[i].BillNumber;
                IList<CmcsHeatStdAssay> list1 = Dbers.GetInstance().SelfDber.Entities<CmcsHeatStdAssay>("where SampleNumber=:SampleNumber ", new { SampleNumber = list[i].BillNumber });
                IList<CmcsMoistureStdAssay> list2 = Dbers.GetInstance().SelfDber.Entities<CmcsMoistureStdAssay>("where SampleNumber=:SampleNumber ", new { SampleNumber = list[i].BillNumber });
                IList<CmcsProximateStdAssay> list3 = Dbers.GetInstance().SelfDber.Entities<CmcsProximateStdAssay>("where SampleNumber=:SampleNumber", new { SampleNumber = list[i].BillNumber });
                IList<CmcsSulfurStdAssay> list4 = Dbers.GetInstance().SelfDber.Entities<CmcsSulfurStdAssay>("where SampleNumber=:SampleNumber", new { SampleNumber = list[i].BillNumber });
                if (list1.Count == 0 && list2.Count == 0 && list3.Count == 0 && list4.Count == 0)
                {
                    temp.FinishStatus = "待化验";
                }
                if (list1.Count > 0 || list2.Count > 0 || list3.Count > 0 || list4.Count > 0)
                {
                    temp.FinishStatus = "化验中";
                }
                if (list1.Count > 0 && list2.Count > 0 && list3.Count > 0 && list4.Count > 0)
                {
                    temp.FinishStatus = "已化验";
                }
                temp.CheckStatus = list[i].ISCHECK == "1" ? "已审核" : "未审核";
                reslist.Add(temp);
            }

            decimal yihuayan = reslist.Where(a => a.FinishStatus == "已化验").Count();
            decimal huayanzhong = reslist.Where(a => a.FinishStatus == "化验中").Count();
            decimal daihuayan = reslist.Where(a => a.FinishStatus == "待化验").Count();
            decimal yishenhe = reslist.Where(a => a.CheckStatus == "已审核").Count();
            decimal weishenhe = reslist.Where(a => a.CheckStatus == "未审核").Count();

            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "化验总数_数量", list.Count.ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "化验完成_数量", yihuayan.ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "化验中_数量", huayanzhong.ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "待化验数_数量", daihuayan.ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "已审核_数量", yishenhe.ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "未审核_数量", weishenhe.ToString());

            //保存化验室温度湿度信号点
            //string[] array = new string[] { "水分测定室", "工业分析仪", "测硫室", "测热室", "元素分析室", "天平室", "灰熔点室", "煤样交接室" };

            //foreach (string signName in array)
            //{
            //    HYTB温湿度仪 item = GetTopOneByMachineCode(signName, "DESC");
            //    CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, signName + "_温度", item.Logs_ChOne.ToString());
            //    CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, signName + "_湿度", item.Logs_ChTwo.ToString());
            //}

            //保存每台设备的化验数据信号点
            int[] res = new int[12];//依次分别为： 量热仪、 水分仪、测硫仪、工业分析仪、天平、元素分析仪

            IList<CmcsHeatStdAssay> listHeat = Dbers.GetInstance().SelfDber.Entities<CmcsHeatStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CmcsMoistureStdAssay> listMoisture = Dbers.GetInstance().SelfDber.Entities<CmcsMoistureStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CmcsSulfurStdAssay> listSulfur = Dbers.GetInstance().SelfDber.Entities<CmcsSulfurStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CmcsProximateStdAssay> listProximate = Dbers.GetInstance().SelfDber.Entities<CmcsProximateStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            //IList<CmcsScaleAssay> listScaleAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where AssayDate>=:st and AssayDate<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            //IList<CmcsElementAssay> listElementAssay = Dbers.GetInstance().SelfDber.Entities<CmcsElementAssay>("where AssayDate>=:st and AssayDate<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });

            res[0] = Convert.ToInt32((listHeat.Where(a => a.FacilityNumber == "#1量热仪").Count() / 2).ToString("f0"));
            res[1] = Convert.ToInt32((listHeat.Where(a => a.FacilityNumber == "#2量热仪").Count() / 2).ToString("f0"));
            res[2] = Convert.ToInt32((listHeat.Where(a => a.FacilityNumber == "#3量热仪").Count() / 2).ToString("f0"));
            res[3] = Convert.ToInt32((listHeat.Where(a => a.FacilityNumber == "#4量热仪").Count() / 2).ToString("f0"));
            res[4] = Convert.ToInt32((listMoisture.Where(a => a.FacilityNumber == "#1水分仪").Count() / 2).ToString("f0"));
            res[5] = Convert.ToInt32((listMoisture.Where(a => a.FacilityNumber == "#2水分仪").Count() / 2).ToString("f0"));
            //res[6] = Convert.ToInt32((listMoisture.Where(a => a.FacilityNumber == "3#水分仪").Count() / 2).ToString("f0"));
            res[6] = Convert.ToInt32((listSulfur.Where(a => a.FacilityNumber == "#1测硫仪").Count() / 2).ToString("f0"));
            res[7] = Convert.ToInt32((listSulfur.Where(a => a.FacilityNumber == "#2测硫仪").Count() / 2).ToString("f0"));
            res[8] = Convert.ToInt32((listSulfur.Where(a => a.FacilityNumber == "#3测硫仪").Count() / 2).ToString("f0"));
            res[9] = Convert.ToInt32((listProximate.Where(a => a.FacilityNumber == "#1工分仪").Count() / 2).ToString("f0"));
            res[10] = Convert.ToInt32((listProximate.Where(a => a.FacilityNumber == "#2工分仪").Count() / 2).ToString("f0"));

            //res[8] = Convert.ToInt32((listScaleAssay.Where(a => a.MachineCode == "1#天平").Count() / 2).ToString("f0"));
            //res[9] = Convert.ToInt32((listScaleAssay.Where(a => a.MachineCode == "2#天平").Count() / 2).ToString("f0"));
            //res[10] = Convert.ToInt32((listElementAssay.Where(a => a.FacilityNumber == "1#元素分析仪").Count() / 2).ToString("f0"));
            //res[11] = Convert.ToInt32((listElementAssay.Where(a => a.FacilityNumber == "2#元素分析仪").Count() / 2).ToString("f0"));
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#量热仪_数量", res[0].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#量热仪_数量", res[1].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "3#量热仪_数量", res[2].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "4#量热仪_数量", res[3].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#水分仪_数量", res[4].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#水分仪_数量", res[5].ToString());
            //CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "3#水分仪_数量", res[6].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#测硫仪_数量", res[6].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#测硫仪_数量", res[7].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "3#测硫仪_数量", res[8].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#工分仪_数量", res[9].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#工分仪_数量", res[10].ToString());
            output(string.Format("结束处理化验室网络信息点"), eOutputType.Normal);
        }

        #endregion
    }
}
