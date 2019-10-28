using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.DumblyConcealer.Enums;
//using CMCS.DumblyConcealer.Tasks.AssayDevice.Entities;
using CMCS.Common;
using CMCS.Common.Entities.AssayDevice;
using CMCS.DumblyConcealer.Tasks.AssayDevice.Entities;
using CMCS.Common.Entities;
using CMCS.Common.DAO;
using CMCS.Common.Entities.MisEntities;
using BST.Biz.Cmcs.Core.Util;
using System.Data;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice
{
    public class AssayDevice_DAO
    {
        private static AssayDevice_DAO instance;
        List<OriginalData> originaldata = null;//入炉化验结果集
        List<OriginalData> originaldata_cy = null;//入厂抽样化验结果集
        List<OriginalData> originaldata_cs = null;//入厂抽样化验结果集
        public static AssayDevice_DAO GetInstance()
        {
            if (instance == null)
            {
                instance = new AssayDevice_DAO();
            }
            return instance;
        }

        private AssayDevice_DAO()
        {

        }

        #region 保存测硫仪数据
        /// <summary>
        /// 保存测硫仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToSulfurStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;
            var list = DcDbers.GetInstance().AssayDevice_Dber.Entities<sulfurstdassay>("where TESTDATE>= :TESTDATE and MANNUMB is not null", new { TESTDATE = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });
            foreach (sulfurstdassay entity in list)
            {
                string pkid = entity.TESTDATE.ToShortDateString() + " " + entity.SPARE5;
                CmcsSulfurStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsSulfurStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsSulfurStdAssay();
                    item.SampleNumber = entity.MANNUMB;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.TARE;
                    item.SampleWeight = entity.SAMPLEMASS;
                    item.Stad = entity.SAD;
                    item.AssayUser = entity.TESTMAN;
                    item.AssayTime = Convert.ToDateTime(entity.TESTDATE.ToShortDateString() + " " + entity.SPARE5);
                    item.OrderNumber = 0;
                    item.IsEffective = 1;
                    item.PKID = pkid;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsSulfurStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.MANNUMB.ToString();
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.TARE;
                    item.SampleWeight = entity.SAMPLEMASS;
                    item.Stad = entity.SAD;
                    item.AssayUser = entity.TESTMAN;
                    item.AssayTime = Convert.ToDateTime(entity.TESTDATE.ToShortDateString() + " " + entity.SPARE5);
                    item.OrderNumber = 0;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsSulfurStdAssay>(item);
                }
            }
            output(string.Format("生成标准测硫仪数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }
        #endregion

        #region 保存量热仪数据
        /// <summary>
        /// 保存量热仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToHeatStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;
            var list = DcDbers.GetInstance().AssayDevice_Dber.Entities<hytb量热仪>("where CSTIME>= :CSTIME and HANDNUM is not null", new { CSTIME = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });
            foreach (hytb量热仪 entity in list)
            {
                string pkid = entity.PKID;
                CmcsHeatStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsHeatStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsHeatStdAssay();
                    item.SampleNumber = entity.HANDNUM;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.SYZL;
                    item.Qbad = entity.DTFRL / 1000m;
                    item.AssayUser = entity.HYY;
                    item.AssayTime = entity.CSTIME;
                    item.IsEffective = 1;
                    item.PKID = pkid;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsHeatStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.HANDNUM;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.SYZL;
                    item.Qbad = entity.DTFRL / 1000m;
                    item.AssayUser = entity.HYY;
                    item.AssayTime = entity.CSTIME;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsHeatStdAssay>(item);
                }

            }
            output(string.Format("生成标准量热仪数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }
        #endregion

        #region 保存水分仪数据
        /// <summary>
        /// 保存水分仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToMoistureStdAssay(Action<string, eOutputType> output)
        {
            int res = 0;
            var list = DcDbers.GetInstance().AssayDevice_Dber.Entities<hytb水分仪>("where TESTDATE>= :TESTDATE and MANNUMB is not null", new { TESTDATE = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });
            foreach (hytb水分仪 entity in list)
            {
                string pkid = entity.PKID;

                CmcsMoistureStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsMoistureStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsMoistureStdAssay();
                    item.SampleNumber = entity.MANNUMB;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.POTMASS;
                    item.SampleWeight = entity.SAMPLEMASS;
                    item.WaterPer = entity.WATERPER;
                    item.AssayUser = entity.TESTMAN;
                    item.IsEffective = 1;
                    item.PKID = pkid;
                    item.AssayTime = entity.TESTDATE.AddSeconds(Convert.ToInt32(entity.AUTONUMB.Substring(9)));
                    item.WaterType = entity.WATERTYPE.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsMoistureStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.MANNUMB;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = entity.POTMASS;
                    item.SampleWeight = entity.SAMPLEMASS;
                    item.WaterPer = entity.WATERPER;
                    item.AssayUser = entity.TESTMAN;
                    item.AssayTime = entity.TESTDATE.AddSeconds(Convert.ToInt32(entity.AUTONUMB.Substring(9)));
                    item.WaterType = entity.WATERTYPE.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Update<CmcsMoistureStdAssay>(item);
                }
            }
            output(string.Format("生成标准水分仪数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }
        #endregion

        #region 保存天平数据

        public int SaveToScale(Action<string, eOutputType> output)
        {
            int res = 0;

            var list = Dbers.GetInstance().SelfDber.Entities<hytb天平>("where 测试日期>= :TESTDATE and 试样编号 is not null and 试样编号!='可燃物分析'", new { TESTDATE = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });

            foreach (var entity in list)
            {
                CmcsScaleAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsScaleAssay>("where PKID=:PKID", new { PKID = entity.PKID });
                if (item == null)
                {
                    item = new CmcsScaleAssay();
                    item.PKID = entity.PKID;
                    item.MachineCode = entity.MACHINECODE;
                    item.SampleNumber = entity.试样编号;
                    item.AssayDate = entity.测试日期;
                    item.AssayTime = entity.测试时间;
                    item.AssayUser = entity.测试人;

                    item.CZZA = entity.瓷舟重A;
                    item.YZA = entity.样重A;
                    item.CMZA = entity.残毛重A;

                    item.CLPZM = entity.称量瓶重M;
                    item.YZM = entity.样重M;
                    item.CMZM = entity.残毛重M;

                    item.GGZV = entity.坩埚重V;
                    item.YZV = entity.样重V;
                    item.CMZV = entity.残毛V;
                    item.IsEffective = 1;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsScaleAssay>(item);
                }
                else
                {
                    item.MachineCode = entity.MACHINECODE;
                    item.SampleNumber = entity.试样编号;
                    item.AssayDate = entity.测试日期;
                    item.AssayTime = entity.测试时间;
                    item.AssayUser = entity.测试人;

                    item.CZZA = entity.瓷舟重A;
                    item.YZA = entity.样重A;
                    item.CMZA = entity.残毛重A;

                    item.CLPZM = entity.称量瓶重M;
                    item.YZM = entity.样重M;
                    item.CMZM = entity.残毛重M;

                    item.GGZV = entity.坩埚重V;
                    item.YZV = entity.样重V;
                    item.CMZV = entity.残毛V;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsScaleAssay>(item);
                }
            }
            output(string.Format("生成标准天平数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }

        #endregion

        #region 保存元素分析仪数据 氢值

        public int SaveToElement(Action<string, eOutputType> output)
        {
            int res = 0;

            var list = Dbers.GetInstance().SelfDber.Entities<hytb元素分析仪>("where CSRQ>= :TESTDATE and SYBH is not null", new { TESTDATE = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });

            foreach (var entity in list)
            {
                CmcsElementAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsElementAssay>("where PKID=:PKID", new { PKID = entity.PKID });
                if (item == null)
                {
                    item = new CmcsElementAssay();
                    item.PKID = entity.PKID;
                    item.FacilityNumber = entity.MACHINECODE;
                    item.SampleNumber = entity.SYBH;
                    item.AssayUser = entity.HYY;
                    item.SampleWeight = entity.YZ;
                    item.AssayDate = entity.CSRQ;
                    item.AssayTime = entity.KSSJ;
                    item.OrderNumber = entity.ID;
                    item.Had = Convert.ToDecimal(entity.HAD);
                    item.Hd = Convert.ToDecimal(entity.HD);
                    item.IsEffective = 1;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsElementAssay>(item);
                }
                else
                {
                    item.FacilityNumber = entity.MACHINECODE;
                    item.SampleNumber = entity.SYBH;
                    item.AssayUser = entity.HYY;
                    item.SampleWeight = entity.YZ;
                    item.AssayDate = entity.CSRQ;
                    item.AssayTime = entity.KSSJ;
                    item.OrderNumber = entity.ID;
                    item.Had = Convert.ToDecimal(entity.HAD);
                    item.Hd = Convert.ToDecimal(entity.HD);
                    res += Dbers.GetInstance().SelfDber.Update<CmcsElementAssay>(item);
                }
            }
            output(string.Format("生成标准元素分析仪数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }

        #endregion

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
                temp.AssayCode = list[i].AssayCode;
                IList<CMCS.Common.Entities.AssayDevice.CmcsHeatStdAssay> list1 = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsHeatStdAssay>("where SampleNumber=:SampleNumber or SampleNumber=:SampleNumber2", new { SampleNumber = list[i].AssayCode, SampleNumber2 = list[i].ManCode });
                IList<CMCS.Common.Entities.AssayDevice.CmcsMoistureStdAssay> list2 = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsMoistureStdAssay>("where SampleNumber=:SampleNumber or SampleNumber=:SampleNumber2 ", new { SampleNumber = list[i].AssayCode, SampleNumber2 = list[i].ManCode });
                IList<CMCS.Common.Entities.AssayDevice.CmcsScaleAssay> list3 = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsScaleAssay>("where SampleNumber=:SampleNumber or SampleNumber=:SampleNumber2 ", new { SampleNumber = list[i].AssayCode, SampleNumber2 = list[i].ManCode });
                IList<CMCS.Common.Entities.AssayDevice.CmcsSulfurStdAssay> list4 = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsSulfurStdAssay>("where SampleNumber=:SampleNumber or SampleNumber=:SampleNumber2 ", new { SampleNumber = list[i].AssayCode, SampleNumber2 = list[i].ManCode });
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
                temp.CheckStatus = list[i].WFStatus == 2 ? "已审核" : "未审核";
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
            string[] array = new string[] { "水分测定室", "工业分析仪", "测硫室", "测热室", "元素分析室", "天平室", "灰熔点室", "煤样交接室" };

            foreach (string signName in array)
            {
                HYTB温湿度仪 item = GetTopOneByMachineCode(signName, "DESC");
                CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, signName + "_温度", item.Logs_ChOne.ToString());
                CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, signName + "_湿度", item.Logs_ChTwo.ToString());
            }

            //保存每台设备的化验数据信号点
            int[] res = new int[12];//依次分别为： 量热仪、 水分仪、测硫仪、工业分析仪、天平、元素分析仪

            IList<CMCS.Common.Entities.AssayDevice.CmcsHeatStdAssay> listHeat = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsHeatStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CMCS.Common.Entities.AssayDevice.CmcsMoistureStdAssay> listMoisture = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsMoistureStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CMCS.Common.Entities.AssayDevice.CmcsSulfurStdAssay> listSulfur = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsSulfurStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CMCS.Common.Entities.AssayDevice.CmcsProximateStdAssay> listProximate = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsProximateStdAssay>("where AssayTime>=:st and AssayTime<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CMCS.Common.Entities.AssayDevice.CmcsScaleAssay> listScaleAssay = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsScaleAssay>("where AssayDate>=:st and AssayDate<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });
            IList<CMCS.Common.Entities.AssayDevice.CmcsElementAssay> listElementAssay = Dbers.GetInstance().SelfDber.Entities<CMCS.Common.Entities.AssayDevice.CmcsElementAssay>("where AssayDate>=:st and AssayDate<:et", new { st = DateTime.Now.Date, et = DateTime.Now.Date.AddDays(1) });

            res[0] = Convert.ToInt32((listHeat.Where(a => a.FacilityNumber == "1#量热仪").Count() / 2).ToString("f0"));
            res[1] = Convert.ToInt32((listHeat.Where(a => a.FacilityNumber == "2#量热仪").Count() / 2).ToString("f0"));
            res[2] = Convert.ToInt32((listMoisture.Where(a => a.FacilityNumber == "1#水分仪").Count() / 2).ToString("f0"));
            res[3] = Convert.ToInt32((listMoisture.Where(a => a.FacilityNumber == "2#水分仪").Count() / 2).ToString("f0"));
            res[4] = Convert.ToInt32((listSulfur.Where(a => a.FacilityNumber == "1#测硫仪").Count() / 2).ToString("f0"));
            res[5] = Convert.ToInt32((listSulfur.Where(a => a.FacilityNumber == "2#测硫仪").Count() / 2).ToString("f0"));
            res[6] = Convert.ToInt32((listProximate.Where(a => a.FacilityNumber == "1#工业分析仪").Count() / 2).ToString("f0"));
            res[7] = Convert.ToInt32((listProximate.Where(a => a.FacilityNumber == "1#工业分析仪").Count() / 2).ToString("f0"));
            res[8] = Convert.ToInt32((listScaleAssay.Where(a => a.MachineCode == "1#天平").Count() / 2).ToString("f0"));
            res[9] = Convert.ToInt32((listScaleAssay.Where(a => a.MachineCode == "2#天平").Count() / 2).ToString("f0"));
            res[10] = Convert.ToInt32((listElementAssay.Where(a => a.FacilityNumber == "1#元素分析仪").Count() / 2).ToString("f0"));
            res[11] = Convert.ToInt32((listElementAssay.Where(a => a.FacilityNumber == "2#元素分析仪").Count() / 2).ToString("f0"));
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#量热仪_数量", res[0].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#量热仪_数量", res[1].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#水分仪_数量", res[2].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#水分仪_数量", res[3].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#测硫仪_数量", res[4].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#测硫仪_数量", res[5].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#工业分析仪_数量", res[6].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#工业分析仪_数量", res[7].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#天平_数量", res[8].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#天平_数量", res[9].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "1#元素分析仪_数量", res[10].ToString());
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_AssayManage, "2#元素分析仪_数量", res[11].ToString());
            output(string.Format("结束处理化验室网络信息点"), eOutputType.Normal);
        }

        #endregion

        #region 获取温湿度仪数据 add in 2017-5-10 10:19:14 by xiekun

        /// <summary>
        /// 根据设备编号获取最新或最早一条数据
        /// </summary>
        /// <param name="machineCode">设备编号</param>
        /// <param name="order">排序方式 asc 或者 desc </param>
        /// <returns></returns>
        public CMCS.Common.Entities.AssayDevice.HYTB温湿度仪 GetTopOneByMachineCode(string machineCode, string order)
        {
            CMCS.Common.Entities.AssayDevice.HYTB温湿度仪 item = Dbers.GetInstance().SelfDber.Entity<CMCS.Common.Entities.AssayDevice.HYTB温湿度仪>(string.Format("where MachineCode=:MachineCode order by Logs_Time {0} ", order), new { MachineCode = machineCode });
            if (item == null)
            {
                return new CMCS.Common.Entities.AssayDevice.HYTB温湿度仪();
            }
            return item;
        }

        #endregion


        #region 保存入炉弹筒数据
        /// <summary>
        /// 保存入炉弹筒数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveToSulTanTongAssay(Action<string, eOutputType> output)
        {
            int res = 0;
            var list = DcDbers.GetInstance().AssayDevice_Dber.Entities<入炉弹筒>("where CSTIME>= :CSTIME and HANDNUM is not null", new { CSTIME = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });
            foreach (入炉弹筒 entity in list)
            {
                string pkid = entity.PKID;
                CmcsHeatStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsHeatStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsHeatStdAssay();
                    item.SampleNumber = entity.HANDNUM;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.SYZL;
                    item.Qbad = entity.DTFRL / 1000m;
                    item.AssayUser = entity.HYY;
                    item.AssayTime = entity.CSTIME;
                    item.IsEffective = 1;
                    item.PKID = pkid;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsHeatStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.HANDNUM;
                    item.FacilityNumber = entity.MachineCode;
                    item.ContainerWeight = 0;
                    item.SampleWeight = entity.SYZL;
                    item.Qbad = entity.DTFRL / 1000m;
                    item.AssayUser = entity.HYY;
                    item.AssayTime = entity.CSTIME;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsHeatStdAssay>(item);
                }

            }
            output(string.Format("生成入炉弹筒数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }
        #endregion


        #region 保存入炉水分仪数据
        /// <summary>
        /// 保存入炉水分仪数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SaveShuiFenYiAssay(Action<string, eOutputType> output)
        {
            int res = 0;
            var list = DcDbers.GetInstance().AssayDevice_Dber.Entities<入炉hytb水分仪>("where TESTDATE>= :TESTDATE and MANNUMB is not null", new { TESTDATE = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });
            foreach (入炉hytb水分仪 entity in list)
            {
                string pkid = entity.PKID;

                CmcsMoistureStdAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsMoistureStdAssay>("where PKID=:PKID", new { PKID = pkid });
                if (item == null)
                {
                    item = new CmcsMoistureStdAssay();
                    item.SampleNumber = entity.MANNUMB;
                    item.FacilityNumber = entity.MachineCode;
                    //item.ContainerWeight = entity.POTMASS;
                    item.SampleWeight = entity.SAMPLEMASS;
                    item.WaterPer = entity.WATERPER;
                    item.AssayUser = entity.TESTMAN;
                    item.IsEffective = 1;
                    item.PKID = pkid;
                    item.AssayTime = entity.TESTDATE.AddSeconds(Convert.ToInt32(entity.AUTONUMB.Substring(9)));
                    //item.WaterType = entity.WATERTYPE.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsMoistureStdAssay>(item);
                }
                else
                {
                    item.SampleNumber = entity.MANNUMB;
                    item.FacilityNumber = entity.MachineCode;
                    //item.ContainerWeight = entity.POTMASS;
                    item.SampleWeight = entity.SAMPLEMASS;
                    item.WaterPer = entity.WATERPER;
                    item.AssayUser = entity.TESTMAN;
                    item.AssayTime = entity.TESTDATE.AddSeconds(Convert.ToInt32(entity.AUTONUMB.Substring(9)));
                    //item.WaterType = entity.WATERTYPE.Contains("全水") ? "全水分" : "分析水";
                    res += Dbers.GetInstance().SelfDber.Update<CmcsMoistureStdAssay>(item);
                }
            }
            output(string.Format("生成入炉水分仪数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }
        #endregion

        #region 保存飞灰数据

        public int SaveFeiHuiAssay(Action<string, eOutputType> output)
        {
            int res = 0;

            var list = Dbers.GetInstance().SelfDber.Entities<hytb天平>("where 测试日期>= :TESTDATE and 试样编号 is not null and 试样编号='可燃物分析'", new { TESTDATE = DateTime.Now.AddDays(0 - DcDbers.GetInstance().AssayDevice_Days).Date });

            foreach (var entity in list)
            {
                CmcsFeiHuiAssay item = Dbers.GetInstance().SelfDber.Entity<CmcsFeiHuiAssay>("where PKID=:PKID", new { PKID = entity.PKID });
                if (item == null)
                {
                    item = new CmcsFeiHuiAssay();
                    item.PKID = entity.PKID;
                    item.MachineCode = entity.MACHINECODE;
                    //item.SampleNumber = entity.试样编号;
                    item.SampleNumber = getHyBm(entity.测试日期, entity.测试序号) ;
                    item.AssayDate = entity.测试日期;
                    item.AssayTime = entity.测试时间;
                    item.AssayUser = entity.测试人;
                    item.CeShiXuHao = entity.测试序号;

                    //item.CZZA = entity.瓷舟重A;
                    //item.YZA = entity.样重A;
                    //item.CMZA = entity.残毛重A;

                    //item.CLPZM = entity.称量瓶重M;
                    //item.YZM = entity.样重M;
                    //item.CMZM = entity.残毛重M;

                    //item.GGZV = entity.坩埚重V;
                    //item.YZV = entity.样重V;
                    //item.CMZV = entity.残毛V;
                    item.CZFH = entity.瓷舟飞灰;
                    item.YZFH = entity.样重飞灰;
                    item.RSHFH = entity.燃烧后飞灰;
                    item.IsEffective = 1;
                    res += Dbers.GetInstance().SelfDber.Insert<CmcsFeiHuiAssay>(item);
                }
                else
                {
                    item.MachineCode = entity.MACHINECODE;
                    item.SampleNumber = getHyBm(entity.测试日期, entity.测试序号);
                    item.AssayDate = entity.测试日期;
                    item.AssayTime = entity.测试时间;
                    item.AssayUser = entity.测试人;
                    item.CZFH = entity.瓷舟飞灰;
                    item.YZFH = entity.样重飞灰;
                    item.RSHFH = entity.燃烧后飞灰;
                    item.CeShiXuHao = entity.测试序号;
                    //item.CZZA = entity.瓷舟重A;
                    //item.YZA = entity.样重A;
                    //item.CMZA = entity.残毛重A;

                    //item.CLPZM = entity.称量瓶重M;
                    //item.YZM = entity.样重M;
                    //item.CMZM = entity.残毛重M;

                    //item.GGZV = entity.坩埚重V;
                    //item.YZV = entity.样重V;
                    //item.CMZV = entity.残毛V;
                    res += Dbers.GetInstance().SelfDber.Update<CmcsFeiHuiAssay>(item);
                }
            }
            output(string.Format("生成飞灰数据 {0} 条（集中管控）", res), eOutputType.Normal);
            return res;
        }
        /// <summary>
        /// 根据化验时间和测试序号生成飞灰的化验编码
        /// </summary>
        /// <param name="AssayDate"></param>
        /// <param name="CeShiXuHao"></param>
        /// <returns></returns>
        private String getHyBm(DateTime AssayDate, int CeShiXuHao)
        {
            string assayBillnumber = "";
            //获取班次间隔的时间点：根据测试日期时间判断数据日期和班次，化验时间8点-14点为1班数据，化验室间2点-20点为2班数据，化验室间20点-8点为3班数据
            DateTime statTm1 = DateTime.Parse(AssayDate.AddDays(-1).ToString("yyyy-MM-dd") + " 20:00:00");

            DateTime statTm2 = DateTime.Parse(AssayDate.ToString("yyyy-MM-dd") + " 08:00:00");

            DateTime statTm3 = DateTime.Parse(AssayDate.ToString("yyyy-MM-dd") + " 14:00:00");

            DateTime statTm4 = DateTime.Parse(AssayDate.ToString("yyyy-MM-dd") + " 20:00:00");
            if (AssayDate > statTm1 && AssayDate < statTm2) {

               // assayBillnumber = statTm1.ToString("MMdd") +"3"+ getBianHao(CeShiXuHao);
                assayBillnumber=(statTm1.ToString("MMdd") + getBianHao(CeShiXuHao)).Replace("-", "3-");
            }
            else if (AssayDate > statTm2 && AssayDate < statTm3) {

                assayBillnumber = (statTm2.ToString("MMdd") + getBianHao(CeShiXuHao)).Replace("-", "1-");
            }
            else if (AssayDate > statTm3 && AssayDate < statTm4)
            {

                assayBillnumber = (statTm2.ToString("MMdd") + getBianHao(CeShiXuHao)).Replace("-", "2-");
            }
            else {

                assayBillnumber = (statTm2.ToString("MMdd") + getBianHao(CeShiXuHao)).Replace("-", "3-");
            }
            return assayBillnumber;
        }
        /// <summary>
        /// 根据测试编号判断该飞灰数据是哪个机组和机组的方向
        /// </summary>
        /// 0：#5机组南；
        /// 1：#5机组北；
        /// 2：#6机组南；
        /// 3：#6机组北；
        /// 4：#3机组南；
        /// 5：#3机组北；
        /// 6：#4机组南；
        /// 7：#4机组北；
        /// <param name="CeShiXuHao"></param>
        /// <returns></returns>
        private String getBianHao(int CeShiXuHao)
        {
            String code = "";
            switch (CeShiXuHao) { 
                case 0:
                    code = "330" + "-" + "F5N";
                    break;
                case 1:
                    code = "330" + "-" + "F5B";
                    break;
                case 2:
                    code = "330" + "-" + "F6N";
                    break;
                case 3:
                    code = "330" + "-" + "F6B";
                    break;
                case 4:
                    code = "145" + "-" + "F3N";
                    break;
                case 5:
                    code = "145" + "-" + "F3B";
                    break;
                case 6:
                    code = "145" + "-" + "F4N";
                    break;
                case 7:
                    code = "145" + "-" + "F4B";
                    break;
            }

            return code;
        
        }

        #endregion

        #region 自动生成入炉化验数据
        /// <summary>
        /// 自动生成入炉化验数据
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int AutoRLAssay(Action<string, eOutputType> output)
        {
            int day = String.IsNullOrEmpty(CommonDAO.GetInstance().GetAppletConfigString("入炉化验写入天数")) ? 0 : int.Parse(CommonDAO.GetInstance().GetAppletConfigString("入炉化验写入天数"));
            int res = 0;
            for (DateTime j = DateTime.Now.Date.AddDays(-day); j <= DateTime.Now.Date; j=j.AddDays(1))
            {
                
            List<string> BillNumber = new List<string>();
            //DateTime star = DateTime.Now.Date;
            DateTime star = j;
            //DateTime star =DateTime.Parse("2017-08-03 00:00:00");
            BillNumber.Add(star.AddDays(-1).ToString("MMdd") + "1453");//昨天34机组1班编码
            BillNumber.Add(star.AddDays(-1).ToString("MMdd") + "3303");//昨天56机组1班编码
            BillNumber.Add(star.ToString("MMdd") + "1451");//昨天34机组2班编码
            BillNumber.Add(star.ToString("MMdd") + "3301");//昨天56机组2班编码
            BillNumber.Add(star.ToString("MMdd") + "1452");//昨天34机组3班编码
            BillNumber.Add(star.ToString("MMdd") + "3302");//昨天56机组3班编码

            for (int i = 0; i < BillNumber.Count; i++)
            {
                originaldata = new List<OriginalData>();
                List<RLHYAssay> Assaylist = Dbers.GetInstance().SelfDber.Entities<RLHYAssay>("where mancode=:mancode AND to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') order by ASSAYDATE asc", new { mancode = (star.ToString("yy") + BillNumber[i]) });
                RLHYAssay assay = new RLHYAssay();

                if (Assaylist.Count == 0)
                {
                    #region 采样
                    RLHYSampling samplinginfo = new RLHYSampling();
                    samplinginfo.DataFrom = BillNumber[i].Substring(4, 3) == "145" ? "001009" : "001010";
                    string ClassCyc = "";
                    switch (BillNumber[i].Substring(7, 1))
                    {
                        case "3":
                            ClassCyc = "一班";
                            break;
                        case "1":
                            ClassCyc = "二班";
                            break;
                        case "2":
                            ClassCyc = "三班";
                            break;

                    }
                    samplinginfo.ClassCyc = ClassCyc;
                    samplinginfo.SamplingDate = DateTime.Now;

                    RLHYSamplingDetail samplingDetail = new RLHYSamplingDetail();
                    //samplingDetail.TheRLHYSampling = samplinginfo;
                    samplingDetail.SAMPLINGID = samplinginfo.Id;
                    samplingDetail.DataFrom = BillNumber[i].Substring(4, 3) == "145" ? "001009" : "001010";
                    samplingDetail.BillNumber = GenerateSamplingBillNumber(samplinginfo.SamplingDate);
                    samplingDetail.FirstCode = string.Empty;
                    samplingDetail.SamplingPle = "";
                    samplingDetail.BagNum = 1;
                    #endregion

                    #region 制样
                    RLHYMake make = new RLHYMake();
                    make.MakeDate = DateTime.Now;
                    make.BillNumber = GenerateMakeBillNumber(make.MakeDate);
                    make.SAMPLINGDETAILID = samplingDetail.Id;
                    // make.TheRLHYSamplingDetail = samplingDetail;
                    make.DataFrom = BillNumber[i].Substring(4, 3) == "145" ? "001009" : "001010";

                    make.SecondCode = string.Empty;
                    make.MakePle = "";
                    #endregion

                    MisFuelQuality Quality0 = new MisFuelQuality();
                    assay.BillNumber = star.ToString("yy") + BillNumber[i];
                    assay.DataFrom = BillNumber[i].Substring(4, 3) == "145" ? "001009" : "001010";
                    assay.AssayDate = star.Date;
                    assay.ManCode = star.ToString("yy") + BillNumber[i];
                    MisFuelQuality Quality = getMz(assay, BillNumber[i], Quality0);
                    assay.FuelQualityId = Quality.Id;
                    assay.MAKEID = make.Id;
                    Dbers.GetInstance().SelfDber.Insert<RLHYSampling>(samplinginfo);
                    Dbers.GetInstance().SelfDber.Insert<RLHYSamplingDetail>(samplingDetail);
                    Dbers.GetInstance().SelfDber.Insert<RLHYMake>(make);
                    if (Dbers.GetInstance().SelfDber.Insert<MisFuelQuality>(Quality) > 0)
                        res += Dbers.GetInstance().SelfDber.Insert<RLHYAssay>(assay);

                    //assay.TheFuelQuality = Quality;
                }
                else
                {
                    //if (Assaylist[0].TheFuelQuality.QJ <= 0)
                    //{
                    //MisFuelQuality Quality = Dbers.GetInstance().SelfDber.Entities<MisFuelQuality>("where id=:FuelQualityId", new { FuelQualityId = Assaylist[0].FuelQualityId }).FirstOrDefault();
                    if (Assaylist[0].Type != "有效")
                    {
                        MisFuelQuality Quality = new MisFuelQuality();
                        Quality.Id = Assaylist[0].FuelQualityId;
                        MisFuelQuality Quality2 = getMz(Assaylist[0], BillNumber[i], Quality);
                        Quality = Quality2;
                        Dbers.GetInstance().SelfDber.Update<MisFuelQuality>(Quality);
                    }
                    //}
                }

            }
            }

            output(string.Format("生成入炉化验数据 {0} 条（集中管控）", res), eOutputType.Normal);

            return res;
        }
        #endregion


        #region 汇总指标值
        void SumQbad(string assayBillnumber)
        {
            //var Assaylist = Dbers.GetInstance().SelfDber.Entities<RLHYAssay>("where mancode=:mancode order by ArriveTime asc", new { mancode = BillNumber[i] });
            IList<CmcsHeatStdAssay> heatAssay = Dbers.GetInstance().SelfDber.Entities<CmcsHeatStdAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1 and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();

            if (heatAssay.Count != 0 && heatAssay.Count >= 2)
            {
                string isvalid = "0";
                int isShowRed = heatAssay.Count > 2 ? 1 : 0;
                decimal value1 = heatAssay[0].Qbad;
                decimal value2 = heatAssay[1].Qbad;
                if (AssayCalcUtil.IsEffectiveQbad(value1, value2))
                    isvalid = "1";
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Qbad",
                    AssayFromDevice = heatAssay[0].FacilityNumber,
                    OQbad = heatAssay[0].Qbad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(heatAssay[0].Qbad, heatAssay[1].Qbad, 3),
                    OAssayUser = heatAssay[0].AssayUser,
                    OAssayTime = heatAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Qbad",
                    AssayFromDevice = heatAssay[1].FacilityNumber,
                    OQbad = heatAssay[1].Qbad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(heatAssay[0].Qbad, heatAssay[1].Qbad, 3),
                    OAssayUser = heatAssay[1].AssayUser,
                    OAssayTime = heatAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });

            }
        }

        void SumStad(string assayBillnumber)
        {
            IList<CmcsSulfurStdAssay> stadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsSulfurStdAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1 and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsSulfurStdAssay> stadAssay = singleServiceBiz.GetSpecified<CmcsSulfurStdAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.IsEffective == 1).ToList();

            if (stadAssay.Count != 0 && stadAssay.Count >= 2)
            {
                string isvalid = "0";
                int isShowRed = stadAssay.Count > 2 ? 1 : 0;
                decimal value1 = stadAssay[0].Stad;
                decimal value2 = stadAssay[1].Stad;

                if (AssayCalcUtil.IsEffectiveStad(value1, value2))
                    isvalid = "1";
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Stad",
                    AssayFromDevice = stadAssay[0].FacilityNumber,
                    OStad = stadAssay[0].Stad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(stadAssay[0].Stad, stadAssay[1].Stad, 3),
                    OAssayUser = stadAssay[0].AssayUser,
                    OAssayTime = stadAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Stad",
                    AssayFromDevice = stadAssay[1].FacilityNumber,
                    OStad = stadAssay[1].Stad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(stadAssay[0].Stad, stadAssay[1].Stad, 3),
                    OAssayUser = stadAssay[1].AssayUser,
                    OAssayTime = stadAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumMt(string assayBillnumber)
        {
            IList<CmcsMoistureStdAssay> mtAssay = Dbers.GetInstance().SelfDber.Entities<CmcsMoistureStdAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1 and WaterType='全水分' and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsMoistureStdAssay> mtAssay = singleServiceBiz.GetSpecified<CmcsMoistureStdAssay>(a =>
            // a.SampleNumber.StartsWith(assayBillnumber) && a.WaterType == "全水分" && a.IsEffective == 1).ToList();

            if (mtAssay.Count != 0 && mtAssay.Count >= 2)
            {
                decimal value1 = mtAssay[0].WaterPer;
                decimal value2 = mtAssay[1].WaterPer;

                decimal val1 = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1);
                //默认不合格
                string isvalid = "0";
                int isShowRed = mtAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveMt(value1, value2))
                    //设为合格
                    isvalid = "1";
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mt",
                    AssayFromDevice = mtAssay[0].FacilityNumber,
                    OMt = mtAssay[0].WaterPer,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1),
                    OAssayUser = mtAssay[0].AssayUser,
                    OAssayTime = mtAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mt",
                    AssayFromDevice = mtAssay[1].FacilityNumber,
                    OMt = mtAssay[1].WaterPer,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1),
                    OAssayUser = mtAssay[1].AssayUser,
                    OAssayTime = mtAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumMad(string assayBillnumber)
        {
            #region write by wujie 内水
            //Mad=(干燥前总重量-干燥后总重量)/样重*100
            //（称量瓶重CLPZM+样重YZM-残毛重CMZM）/样重YZM*100
            //得到对应的记录（Mad\Vad\Aad)
            //string sql = "where SAMPLENUMBER like'%{0}%' and IsEffective=1 and CLPZM>=0 and YZM>=0 and CMZM>=0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber";

            IList<CmcsScaleAssay> madAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and CLPZM>0 and YZM>0 and CMZM>0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsScaleAssay> madAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.CLPZM >= 0 && a.YZM >= 0 && a.CMZM >= 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            if (madAssay.Count != 0 && madAssay.Count >= 2)
            {
                decimal mad1 = madAssay[0].Mad;
                decimal mad2 = madAssay[1].Mad;
                string isvalid = "0";
                int isShowRed = madAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveMad(mad1, mad2))
                    isvalid = "1";
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mad",
                    AssayFromDevice = madAssay[0].MachineCode,
                    OMad = madAssay[0].Mad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(madAssay[0].Mad, madAssay[1].Mad, 3),
                    OAssayUser = madAssay[0].AssayUser,
                    OAssayTime = madAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mad",
                    AssayFromDevice = madAssay[1].MachineCode,
                    OMad = madAssay[1].Mad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(madAssay[0].Mad, madAssay[1].Mad, 3),
                    OAssayUser = madAssay[1].AssayUser,
                    OAssayTime = madAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
            #endregion
        }

        void SumVad(string assayBillnumber)
        {
            #region write by wujie
            //Vad=(灼烧前总重量-灼烧后总重量)/样重*100-Mad
            //Vad=(坩埚重GGZV+样重YZV-残毛重CMZV)/YZV*100-(（称量瓶重CLPZM+样重YZM-残毛重CMZM）/样重YZM*100)
            #endregion
            IList<CmcsScaleAssay> vadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and GGZV>0 and YZV>0 and CMZV>0 and CLPZM>0 and YZM>0 and CMZM>0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsScaleAssay> vadAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.GGZV > 0 && a.YZV > 0 && a.CMZV > 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            if (vadAssay.Count != 0 && vadAssay.Count >= 2)
            {
                decimal vad1 = vadAssay[0].Vad;
                decimal vad2 = vadAssay[1].Vad;
                string isvalid = "0";
                int isShowRed = vadAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveVad(vad1, vad2))
                    isvalid = "1";
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Vad",
                    AssayFromDevice = vadAssay[0].MachineCode,
                    OVad = vadAssay[0].Vad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(vadAssay[0].Vad, vadAssay[1].Vad, 3),
                    OAssayUser = vadAssay[0].AssayUser,
                    OAssayTime = vadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Vad",
                    AssayFromDevice = vadAssay[1].MachineCode,
                    OVad = vadAssay[1].Vad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(vadAssay[0].Vad, vadAssay[1].Vad, 3),
                    OAssayUser = vadAssay[1].AssayUser,
                    OAssayTime = vadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumAad(string assayBillnumber)
        {
            #region write by wujie 空干基灰分
            //Aad=(灼烧后总重量-容器重量)/样重*100
            //（残毛重CMZA-瓷舟重CZZA）/样重YZA*100
            //得到对应的记录（Mad\Vad\Aad)
            //IList<CmcsScaleAssay> aadAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.CMZA != 0 && a.CZZA != 0 && a.YZA != 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            IList<CmcsScaleAssay> aadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and CMZA!=0 and CZZA!=0 and YZA!=0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            if (aadAssay.Count != 0 && aadAssay.Count >= 2)
            {
                decimal aad1 = aadAssay[0].Aad;
                decimal aad2 = aadAssay[1].Aad;
                string isvalid = "0";
                int isShowRed = aadAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveAad(aad1, aad2))
                    isvalid = "1";
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Aad",
                    AssayFromDevice = aadAssay[0].MachineCode,
                    OAad = aadAssay[0].Aad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(aadAssay[0].Aad, aadAssay[1].Aad, 3),
                    OAssayUser = aadAssay[0].AssayUser,
                    OAssayTime = aadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Aad",
                    AssayFromDevice = aadAssay[1].MachineCode,
                    OAad = aadAssay[1].Aad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(aadAssay[0].Aad, aadAssay[1].Aad, 3),
                    OAssayUser = aadAssay[1].AssayUser,
                    OAssayTime = aadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
            #endregion

        }

        //氢值的算法和入场不一样,不是从元素里面取,根据公式计算
        void GetHad(MisFuelQuality assay)
        {
            decimal vdaf = 0;
            try
            {
                vdaf = CalcAvgValueS((100 / (100 - assay.Mad - assay.Aad) * assay.Vad), 2);
            }
            catch
            {

                vdaf = 0;
            }

            decimal had = 0;
            #region write by wujie
            #endregion
            if (vdaf < 8)
            {

                had = (100 - assay.Mad - assay.Aad) / 100 * (0.444m * vdaf + 0.494m);

            }
            else if (vdaf < 40 && vdaf >= 24)
            {
                had = (100 - assay.Mad - assay.Aad) / 100 * (0.053m * vdaf + 3.61m);

            }
            else if (vdaf > 40)
            {

                had = (100 - assay.Mad - assay.Aad) / 100 * (0.0835m * vdaf + 1.205m);
            }
            else if (vdaf >= 8 && vdaf < 24)
            {
                had = 0.0605m * assay.Vad + 2.07m;
            }
            else if (vdaf > 8.9m && vdaf < 9.2m)
            {
                had = 2.76m;
            }
            string isvalid = "1";
            int isShowRed = 0;
            originaldata.Add(new OriginalData()
            {
                AssayNum = "公式计算",
                AssayTarget = "Had",
                AssayFromDevice = "公式计算",
                OHad = had,
                OAssayCalValue = AssayCalcUtil.CalcAvgValue(had, had, 2),
                //OAssayUser = base.GetCurrentLoginUser().UserName,
                OAssayTime = DateTime.Now,
                IsShowRed = isShowRed,
                Isvalid = isvalid
            });
        }
        /// <summary>
        /// 计算两个化验指标的平均值，默认保留两位小数,使用四舍五入
        /// </summary>
        /// <param name="value1">值1</param>
        /// <param name="value2">值2</param>
        /// <param name="decimals">保留小数位</param>
        /// <returns></returns>
        public decimal CalcAvgValueS(decimal value, int decimals = 2)
        {
            return (decimal)Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }
        /// <summary>
        /// 读取飞灰化验指标
        /// </summary>
        /// <param name="assayBillnumber">化验编码</param>
        private MisFuelQuality SumFeiHui(string assayBillnumber, MisFuelQuality Quality)
        {
            IList<CmcsFeiHuiAssay> FhAssay = Dbers.GetInstance().SelfDber.Entities<CmcsFeiHuiAssay>("where SampleNumber like'%'||:assayBillnumber||'%' and IsEffective=1  and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();

            foreach (var item in FhAssay)
            {
                string bm = item.SampleNumber.Substring(item.SampleNumber.Length - 2, 2);
                switch (bm) { 
                    case"3N":
                        Quality.OneN = item.YZFH > 0 ? Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100, 2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                    case"3B":
                        Quality.OneS = item.YZFH > 0 ? Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100, 2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                    case "4N":
                        Quality.TwoN = item.YZFH > 0 ? Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100, 2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                    case "4B":
                        Quality.TwoS = item.YZFH > 0 ? Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100, 2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                    case "5N":
                        Quality.OneN = item.YZFH > 0 ? Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100, 2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                    case "5B":
                        Quality.OneS = item.YZFH > 0 ?Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100,2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                    case "6N":
                        Quality.TwoN = item.YZFH > 0 ? Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100, 2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                    case "6B":
                        Quality.TwoS = item.YZFH > 0 ? Math.Round((item.CZFH + item.YZFH - item.RSHFH) / item.YZFH * 100, 2) : 0;//(瓷舟飞灰+样重飞灰-燃烧后飞灰)/样重飞灰*100
                        break;
                }
            }
            return Quality;
        }
        #endregion

        #region 读取入炉煤质
        private MisFuelQuality getMz(RLHYAssay entity, string ManCode, MisFuelQuality Quality)
        {
            // MisFuelQuality Quality = new MisFuelQuality();
            SumQbad(ManCode);
            SumStad(ManCode);
            SumMt(ManCode);
            SumMad(ManCode);
            SumAad(ManCode);
            SumVad(ManCode);
            Quality = SumFeiHui(ManCode, Quality);
            //entity.TheFuelQuality = Quality;
            Quality.Mar = AssayCalcUtil.GetTrunValue(AssayCalcUtil.GetTrunValue(Quality.Mar, 1), 1);
            //if (entity.TheFuelQuality != null)
            //{
            //    Quality.OneN = entity.TheFuelQuality.OneN;
            //    Quality.OneS = entity.TheFuelQuality.OneS;
            //    Quality.TwoN = entity.TheFuelQuality.TwoN;
            //    Quality.TwoS = entity.TheFuelQuality.TwoS;
            //}
            if (entity.IsStatas != "1")
            {
                Quality.QbAd = originaldata.Where(a => a.AssayTarget == "Qbad").ToList().Count == 0 ? 0
              : originaldata.Where(a => a.AssayTarget == "Qbad").FirstOrDefault().OAssayCalValue;
                Quality.Stad = originaldata.Where(a => a.AssayTarget == "Stad").ToList().Count == 0 ? 0
                    : originaldata.Where(a => a.AssayTarget == "Stad").FirstOrDefault().OAssayCalValue;
                Quality.Mar = originaldata.Where(a => a.AssayTarget == "Mt").ToList().Count == 0 ? 0
                    : originaldata.Where(a => a.AssayTarget == "Mt").FirstOrDefault().OAssayCalValue;
                Quality.Mad = AssayCalcUtil.GetTrunValue(originaldata.Where(a => a.AssayTarget == "Mad").ToList().Count == 0 ? 0
                    : originaldata.Where(a => a.AssayTarget == "Mad").FirstOrDefault().OAssayCalValue, 2);
                Quality.Vad = AssayCalcUtil.GetTrunValue(originaldata.Where(a => a.AssayTarget == "Vad").ToList().Count == 0 ? 0
                    : originaldata.Where(a => a.AssayTarget == "Vad").FirstOrDefault().OAssayCalValue, 2);
                Quality.Aad = AssayCalcUtil.GetTrunValue(originaldata.Where(a => a.AssayTarget == "Aad").ToList().Count == 0 ? 0
                    : originaldata.Where(a => a.AssayTarget == "Aad").FirstOrDefault().OAssayCalValue, 2);


                //根据是否有效来判断是否有值
                if (originaldata.Where(a => a.AssayTarget == "Qbad").FirstOrDefault() != null)
                    if (originaldata.Where(a => a.AssayTarget == "Qbad").FirstOrDefault().Isvalid == "0") Quality.QbAd = 0.00m;
                if (originaldata.Where(a => a.AssayTarget == "Stad").FirstOrDefault() != null)
                    if (originaldata.Where(a => a.AssayTarget == "Stad").FirstOrDefault().Isvalid == "0") Quality.Stad = 0.00m;
                if (originaldata.Where(a => a.AssayTarget == "Mt").FirstOrDefault() != null)
                    if (originaldata.Where(a => a.AssayTarget == "Mt").FirstOrDefault().Isvalid == "0") Quality.Mar = 0.00m;
                if (originaldata.Where(a => a.AssayTarget == "Mad").FirstOrDefault() != null)
                    if (originaldata.Where(a => a.AssayTarget == "Mad").FirstOrDefault().Isvalid == "0") Quality.Mad = 0.00m;
                if (originaldata.Where(a => a.AssayTarget == "Vad").FirstOrDefault() != null)
                    if (originaldata.Where(a => a.AssayTarget == "Vad").FirstOrDefault().Isvalid == "0") Quality.Vad = 0.00m;
                if (originaldata.Where(a => a.AssayTarget == "Aad").FirstOrDefault() != null)
                    if (originaldata.Where(a => a.AssayTarget == "Aad").FirstOrDefault().Isvalid == "0") Quality.Aad = 0.00m;
                if (originaldata.Count > 0)
                {
                    //得到氢值,因为氢值通过别的指标计算出来的,所以放在后面统计
                    GetHad(Quality);

                    Quality.Had = AssayCalcUtil.GetTrunValue(originaldata.Where(a => a.AssayTarget == "Had").ToList().Count == 0 ? 0
                        : originaldata.Where(a => a.AssayTarget == "Had").FirstOrDefault().OAssayCalValue, 2);

                    Quality = getQuality(Quality);
                }
                //else
                //{
                //    MisFuelQuality qt = new MisFuelQuality();
                //    qt.Id = Quality.Id;
                //    return qt;
                //}


            }



            return Quality;
        }

        #endregion

        //自动计算值
        public MisFuelQuality getQuality(MisFuelQuality Quality)
        {
            if (100 - Quality.Mad == 0)
            {
                Quality.Ad = 0;
            }
            else
            {
                Quality.Ad = CalcAvgValueS((100 / (100 - Quality.Mad) * Quality.Aad), 2);
            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Var = 0;
            }
            else
            {
                // 收到基会发份(Var)%   Vad(%)*((100-Mt)/(100-Mad))
                Quality.Var = CalcAvgValueS((Quality.Vad) * (100 - Quality.Mar) / (100 - Quality.Mad), 2);

            }
            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Vdaf = 0;
            }
            else
            {
                //干燥无灰基挥发份(Vdaf)%  Vdaf=100/(100-Mad-Aad)*Vad
                Quality.Vdaf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Vad), 2);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Std = 0;
            }
            else
            {

                // 干燥基全硫(St,d)%   St,d=100/(100-Mad)*St,ad
                Quality.Std = CalcAvgValueS((100 / (100 - Quality.Mad) * Quality.Stad), 2);


            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Star = 0;
            }
            else
            {
                //收到基全硫(St,ar)%   St,ar=(100-Mar)/(100-M)*ST
                Quality.Star = CalcAvgValueS(((100 - Quality.Mar) / (100 - Quality.Mad) * Quality.Stad), 2);
            }
            decimal a = 0.001m;
            if (Quality.QbAd <= 16.70m)
                a = 0.001m;
            else
            {
                if (Quality.QbAd > 25.10m)
                    a = 0.0016m;
                else
                    a = 0.0012m;
            }


            //if (Qgrad.Attributes["type"] == "balance")
            Quality.Qgrad = CalcAvgValueS(((Quality.QbAd * 1000 - (94.1m * Quality.Stad + a * Quality.QbAd * 1000m)) / 1000), 3);
            //else
            //    Qgrad.Text = CalcAvgValue(((decimal.Parse(QbAd.Text) * 1000 - (94.1m * decimal.Parse(Stad.Text) + a * decimal.Parse(QbAd.Text) * 1000m)) / 1000), 2).ToString();

            decimal qbj = ((Quality.QbAd * 1000 - (94.1m * Quality.Stad + a * Quality.QbAd * 1000m)) / 1000);
            if (100 - Quality.Mad == 0)
            {
                Quality.QJ = 0;
            }
            else
            {
                //Qnet,ar  MJ/kg

                Quality.QJ = CalcAvgValueS((((qbj * 1000 - 206 * Quality.Had) * ((100 - Quality.Mar) / (100 - Quality.Mad)) - 23 * Quality.Mar) / 1000), 3);

            }
            //(Qnet,ar)Kcal/kg

            Quality.QCal = decimal.Parse(CalcAvgValueS((Quality.QJ * 1000m / 4.1816m), 2).ToString("0"));


            if (100 - Quality.Mad == 0)
            {
                Quality.Qgrd = 0;
            }
            else
            {

                // MJ/kg
                Quality.Qgrd = CalcAvgValueS((Quality.Qgrad * (100 / (100 - Quality.Mad))), 3);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Aar = 0;
            }
            else
            {

                Quality.Aar = CalcAvgValueS((Quality.Aad * ((100 - Quality.Mar) / (100 - Quality.Mad))), 2);


            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Vd = 0;
            }
            else
            {
                Quality.Vd = CalcAvgValueS(Quality.Vad * (100 / (100 - (Quality.Mad))), 2);
            }
            Quality.FCad = (100 - (Quality.Mad + Quality.Aad + Quality.Vad));

            //Har%=[100Vd/(100-Vdaf)]*[7.35/(Vdaf+10)-0.013]*[(100-Ad)/100]*(100-Mar)/100  "Mar就相当于Mt”
            //Had(空气干燥基氢值)=(100-Mar)Har/(100-Mad)

            decimal vd = Quality.Vd;
            decimal vdaf = Quality.Vdaf;
            decimal ad = Quality.Ad;
            decimal mt = Quality.Mar;
            decimal mad = Quality.Mad;
            decimal vad = Quality.Vad;
            decimal pq;
            if (100m - Quality.Mad == 0)
            {
                pq = 0;
            }
            else
            {
                //新： P收到基低位热量（Qnet,ar）kcal/kg  [Qgr,v,ad-212*Had-0.8*（空气干燥基氧值（Nod）+空气干燥基氮值（Nad） ）]* （100-M）/（100-Mad） - 23*M
                pq = (qbj * 1000 - 212m * (Quality.Had) - 0.8m * (Quality.Oad + Quality.Nad)) * (100m - Quality.Mar) / (100m - Quality.Mad) - 24.4m * Quality.Mar;
            }
            Quality.pQCal = decimal.Parse(Math.Round(pq / 4.1816m, 0).ToString("f0"));
            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Hdaf = 0;
            }
            else
            {
                //干燥无灰基氢值(H,daf)%
                Quality.Hdaf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Had), 2);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Har = 0;
            }
            else
            {
                //收到基氢值(H,ar)%
                Quality.Har = CalcAvgValueS((Quality.Had * (100 - Quality.Mar) / (100 - Quality.Mad)), 2);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Hd = 0;
            }
            else
            {
                //干燥基氢值(H,d)%
                Quality.Hd = CalcAvgValueS((100 / (100 - Quality.Mad) * Quality.Had), 2);

            }
            //干燥基固定碳(FC,d)%
            if (100 - Quality.Mad == 0)
            {
                Quality.FCd = 0;
            }
            else
            {
                Quality.FCd = Math.Round((100 / (100 - Quality.Mad) * Quality.FCad), 2);
            }
            if (100 - Quality.Mad == 0)
            {
                Quality.FCar = 0;
            }
            else
            {
                //收到基固定碳(FC,ar)%

                Quality.FCar = CalcAvgValueS((Quality.FCad * (100 - Quality.Mar) / (100 - Quality.Mad)), 2);


            }
            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Qnetdaf = 0;
            }
            else
            {
                //干燥无灰基高位热量(Qnet,daf)MJ/kg

                Quality.Qnetdaf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Qgrad), 2);
            }

            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Stadf = 0;
            }
            else
            {
                // 干燥无灰基硫(St,daf)%

                Quality.Stadf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Stad), 2);


            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Qgrar = 0;
            }
            else
            {
                //收到基高位热量(Qgr,ar)MJ/kg

                Quality.Qgrar = CalcAvgValueS((Quality.Qgrad * (100 - Quality.Mar) / (100 - Quality.Mad)), 2);

            }
            //干燥无灰基氢值（Hadf）= 0.00117*干燥基灰分+0.57*根号干燥无灰基挥发分(V,daf)%  +0.1362*干燥无灰基高位热值/1000-2.806

            Quality.Hdaf = CalcAvgValueS(0.00117m * Quality.Ad + 0.57m * Convert.ToDecimal(Math.Sqrt(double.Parse(Quality.Vdaf.ToString()))) + 0.1362m * Quality.Qnetdaf - 2.806m, 2);


            ////空干基氢值=（100-空干基水分—空干基灰分）/100*干燥无灰基氢值（Hadf）
            //Had.Text = Math.Round(((100 - decimal.Parse(Mad.Text) - decimal.Parse(Aad.Text)) * decimal.Parse(Hdaf.Text)) / 100, 2).ToString();

            if (100 - Quality.Mad == 0)
            {
                Quality.Har = 0;
            }
            else
            {
                //收到基氢值(H, ar) %
                Quality.Har = CalcAvgValueS((Quality.Had * (100 - Quality.Mar) / (100 - Quality.Mad)), 2);

            }
            return Quality;
        }
        //生成采样编码
        public string GenerateSamplingBillNumber(DateTime dt, string prefix = "RLCY")
        {
            string newBillNumber = string.Empty;
            do
            {
                newBillNumber = string.Format("{0}{1}{2}", prefix, dt.ToString("yyMMdd"), new Random().Next(1000).ToString().PadLeft(3, '0'));
            }
            while (Dbers.GetInstance().SelfDber.Entities<RLHYSamplingDetail>("where BillNumber=:BillNumber", new { BillNumber = newBillNumber }).Count() > 0);

            return newBillNumber;
        }
        //生成制样编码
        public string GenerateMakeBillNumber(DateTime dt, string prefix = "RLZY")
        {
            string newBillNumber = string.Empty;

            do
            {
                newBillNumber = string.Format("{0}{1}{2}", prefix, dt.ToString("yyMMdd"), new Random().Next(1000).ToString().PadLeft(3, '0'));
            }
            while (Dbers.GetInstance().SelfDber.Entities<RLHYMake>("where BillNumber=:BillNumber", new { BillNumber = newBillNumber }).Count() > 0);

            return newBillNumber;
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();
        public int SinkAndDrainReport(Action<string, eOutputType> output)
        {
            DateTime startime;
            DateTime time = DateTime.Now;
            string times = commonDAO.GetAppletConfigString("收耗存日报同步开始时间");
            //DateTime dd = DateTime.Parse("2017-08-01");
            if (string.IsNullOrEmpty(times))
            {
                startime = DateTime.Now;
            }
            else {
                startime =DateTime.Parse(times);
            }
            DateTime tm = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            DateTime endtime = tm;
            int count = 0;

            for (DateTime j = startime; j <= endtime; j=j.AddDays(1))
            {
                   

                time = j;
                SinkAndDrain entity = Dbers.GetInstance().SelfDber.Entity<SinkAndDrain>("where Year=:Year and Month=:Month and Day=:Day", new { Year = time.Year, Month = time.Month, Day = time.Day });
                if (entity == null)
                {
                    List<SinkAndDrain> entity2 = Dbers.GetInstance().SelfDber.Entities<SinkAndDrain>("where Year=:Year and Month=:Month order by cast(day as number)  desc", new { Year = time.Year, Month = time.Month });
                    SinkAndDrain item = new SinkAndDrain();
                    if (entity2.Count == 0)
                    {
                        item.Year = time.Year;
                        item.Month = time.Month;
                        item.Day = time.Day;
                    }
                    else {
                        item.Year = time.Year;
                        item.Month = time.Month;
                        item.Day = time.Day;
                        //item.ZongJiDangRiLaiMei = entity2.First().ZongJiDangRiLaiMei;
                        item.ZongJiLeiJiLaiMei = entity2.First().ZongJiLeiJiLaiMei;
                    
                    } 
                  
                    if (Dbers.GetInstance().SelfDber.Insert<SinkAndDrain>(item) > 0)
                    {
                        count++;
                        List<AreaDivide> AreaDividelist = Dbers.GetInstance().SelfDber.Entities<AreaDivide>("where id !='-1' and Valid='有效' order by AreaCode asc");
                        for (int i = 0; i < AreaDividelist.Count; i++)
                        {
                            SinkAndDrainDetail SinkAndDrainDetail = new SinkAndDrainDetail();
                          
                   
                            if (entity2.Count == 0)
                            {
                                SinkAndDrainDetail.DiQu = AreaDividelist[i].Id;
                                SinkAndDrainDetail.SINKANDDRAINID = item.Id;
                            }
                            else
                            {
                                SinkAndDrainDetail entity3 = Dbers.GetInstance().SelfDber.Entity<SinkAndDrainDetail>("where SINKANDDRAINID=:SINKANDDRAINID and DiQu=:DiQu", new { SINKANDDRAINID = entity2.First().Id, DiQu = AreaDividelist[i].Id });
                                SinkAndDrainDetail.DiQu = AreaDividelist[i].Id;
                                SinkAndDrainDetail.SINKANDDRAINID = item.Id;
                                SinkAndDrainDetail.LeiJiDangRiLaiMei =entity3==null?0:entity3.LeiJiDangRiLaiMei;


                            }
                            Dbers.GetInstance().SelfDber.Insert<SinkAndDrainDetail>(SinkAndDrainDetail);
                        }


                    }
                }
                SinkAndDrainChangSun entity1 = Dbers.GetInstance().SelfDber.Entity<SinkAndDrainChangSun>("where Year=:Year and Month=:Month", new { Year = time.Year, Month = time.Month });
                if (entity1 == null)
                {
                    SinkAndDrainChangSun SinkAndDrainChangSun = new SinkAndDrainChangSun();
                    SinkAndDrainChangSun.Year = time.Year;
                    SinkAndDrainChangSun.Month = time.Month;
                    Dbers.GetInstance().SelfDber.Insert<SinkAndDrainChangSun>(SinkAndDrainChangSun);
                }

            //    count++;
            }
                output(string.Format("生成收耗存日报数据 {0} 条（集中管控）", count), eOutputType.Normal);
            return 1;
        }

        #region 根据入帐的批次区域，供煤单位,发站自动合并估收批次主表
        public int EstInFactoryBatchInfo(Action<string, eOutputType> output)
        {
            DataTable tb = getRuZhangPiCi(); //获取所有已经入账没有被合并的批次信息
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                EstInFactoryBatch entity = Dbers.GetInstance().SelfDber.Entity<EstInFactoryBatch>("where id=:gsid", new { gsId = tb.Rows[i]["gsId"].ToString()});
                EstInFactoryBatchDetail entityDetail = new EstInFactoryBatchDetail();
                entityDetail.BatchID = tb.Rows[i]["ID"].ToString();
                entityDetail.BatchNO = tb.Rows[i]["BATCH"].ToString();
                entityDetail.BalanceID = tb.Rows[i]["FAMBALANCEID"].ToString();
                entityDetail.BalanceNO = tb.Rows[i]["FAMBALANCE"].ToString();
                entityDetail.DispatchDate = DateTime.Parse(tb.Rows[i]["RUNDATE"].ToString());
                entityDetail.SupplierID = tb.Rows[i]["SUPPLIERID"].ToString();
                entityDetail.SupplierName = tb.Rows[i]["suppliername"].ToString();
                entityDetail.StationID = tb.Rows[i]["STATIONID"].ToString();
                entityDetail.StationName = tb.Rows[i]["STATIONname"].ToString();
                entityDetail.TransportTypeID = tb.Rows[i]["TRANSPORTTYPEID"].ToString();
                entityDetail.TransportTypeName = tb.Rows[i]["TRANSPORTNAME"].ToString();
                entityDetail.BookingDate = new DateTime(int.Parse(tb.Rows[i]["year"].ToString()), int.Parse(tb.Rows[i]["month"].ToString()), int.Parse(tb.Rows[i]["day"].ToString()));
                entityDetail.AreadivideID = tb.Rows[i]["AreadivideID"].ToString();
                entityDetail.AreadivideName = tb.Rows[i]["AreaName"].ToString();

                entityDetail.TicketQty =Decimal.Parse(tb.Rows[i]["TICKETQTY"].ToString());
                entityDetail.CheckQty = Decimal.Parse(tb.Rows[i]["CHECKQTY"].ToString());
                entityDetail.CarNum = Decimal.Parse(tb.Rows[i]["TRANSPORTNUMBER"].ToString());
                entityDetail.QJ = Decimal.Parse(tb.Rows[i]["qj"].ToString()) < 0 ? 0 : Decimal.Parse(tb.Rows[i]["qj"].ToString());
                entityDetail.Vdaf = Decimal.Parse(tb.Rows[i]["vdaf"].ToString()) < 0 ? 0 : Decimal.Parse(tb.Rows[i]["vdaf"].ToString());
                entityDetail.Std = Decimal.Parse(tb.Rows[i]["std"].ToString()) < 0 ? 0 : Decimal.Parse(tb.Rows[i]["std"].ToString());
                entityDetail.MT = Decimal.Parse(tb.Rows[i]["mar"].ToString()) < 0 ? 0 : Decimal.Parse(tb.Rows[i]["mar"].ToString());
                entityDetail.ST = Decimal.Parse(tb.Rows[i]["st"].ToString()) < 0 ? 0 : Decimal.Parse(tb.Rows[i]["st"].ToString());
                entityDetail.ESTBATCHID = tb.Rows[i]["gsId"].ToString();
                Dbers.GetInstance().SelfDber.Insert<EstInFactoryBatchDetail>(entityDetail);
                IList<EstInFactoryBatchDetail> details = Dbers.GetInstance().SelfDber.Entities<EstInFactoryBatchDetail>("where ESTBATCHID=:ESTBATCHID", new { ESTBATCHID = tb.Rows[i]["gsId"].ToString() });
                SetEstInFactoryBatch(details, entity);
            }
            output(string.Format("合并估收批次数据 {0} 条（估收批次）", tb.Rows.Count), eOutputType.Normal);
            return 1;
        }

        //获取所有已经入账并且可以被合并的批次(批次自动合并:相同供煤单,发站,区域,运输方式,并且矿发日期在估收批次的矿发开始时间和矿发结束时间之前的)
        public DataTable getRuZhangPiCi() {
            string sql = @"select m.id as gsId,n.* from FULTBESTINFACTORYBATCH m,
(select a.* from 
(select a.*,g.id as AreadivideID,g.AreaName,d.qj,d.vdaf,d.std,d.mar,d.st,e.name as suppliername,f.name as STATIONname,b.year,b.month,b.day
 from fultbinfactorybatch  a,CMCSTBSinkAndDrainAccounted b,fultbrchyassay c,fultbfuelquality d,fultbsupplier e,FULTBSTATIONINFO f,FULTBAREADIVIDE g

 where a.batch=b.ruzhangpicihao and a.id=c.infactorybatchid(+) and c.fuelqualityid=d.id(+) and a.SUPPLIERID=e.id and a.STATIONID=f.id and b.RUZHANGDIQU=g.id) a 
left join 
(SELECT * FROM FULTBESTINFACTORYBATCHDETAIL) b  on a.batch=b.batchno where b.batchno is null) n where m.TRANSPORTTYPEID=n.TRANSPORTTYPEID
and m.SupplierID=n.SupplierID and m.StationID=n.StationID and m.AreadivideID=n.AreadivideID and  n.RunDate>= m.DispatchStartDate 
and  n.RunDate<=m.DispatchEndDate";

            DataTable tb = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            return tb;
        }
        /// <summary>
        /// 从表生成主表数据
        /// </summary>
        /// <param name="details"></param>
        private void SetEstInFactoryBatch(IList<EstInFactoryBatchDetail> details, EstInFactoryBatch item)
        {
            IList<EstInFactoryBatchDetail> items = details.Where(a => a.BalanceID == null || a.BalanceID == "").ToList();
            EstInFactoryBatchDetail detail = details.FirstOrDefault();

            if (detail == null)
                detail = new EstInFactoryBatchDetail();

            //EstInFactoryBatch item = new EstInFactoryBatch();
            //item.Id = Id.Text;
            //item.Batch = Batch.Text;
            //item.Abstract = Abstract.Text;
            //item.DispatchStartDate = DispatchStartDate.SelectedDate;
            //item.DispatchEndDate = DispatchEndDate.SelectedDate;
            //item.ContractID = ContractID.Text;
            //item.ContractNO = ContractNO.InnerText;
            item.AreadivideID = detail.AreadivideID;
            item.AreadivideName = detail.AreadivideName != null ? detail.AreadivideName.Replace("&nbsp;", "") : "";
            item.SupplierID = detail.SupplierID;
            item.SupplierName = detail.SupplierName != null ? detail.SupplierName.Replace("&nbsp;", "") : "";
            item.StationID = detail.StationID;
            item.StationName = detail.StationName != null ? detail.StationName.Replace("&nbsp;", "") : "";
            item.TransportTypeID = detail.TransportTypeID;
            item.TransportTypeName = detail.TransportTypeName != null ? detail.TransportTypeName.Replace("&nbsp;", "") : "";
            item.EstQty = items.Sum(a => a.CheckQty);
            item.TicketQty = details.Sum(a => a.TicketQty);
            item.CheckQty = details.Sum(a => a.CheckQty);
            item.CarNum = details.Sum(a => a.CarNum);
            item.QJ = items.Where(a => a.QJ > 0).Sum(a => a.CheckQty) > 0 ?
               Math.Round(items.Where(a => a.QJ > 0).Sum(a => a.CheckQty * a.QJ) / items.Where(a => a.QJ > 0).Sum(a => a.CheckQty), 3) : 0;
            item.Vdaf = items.Where(a => a.Vdaf > 0).Sum(a => a.CheckQty) > 0 ?
               Math.Round(items.Where(a => a.Vdaf > 0).Sum(a => a.CheckQty * a.Vdaf) / items.Where(a => a.Vdaf > 0).Sum(a => a.CheckQty), 2) : 0;
            item.Std = items.Where(a => a.Std > 0).Sum(a => a.CheckQty) > 0 ?
               Math.Round(items.Where(a => a.Std > 0).Sum(a => a.CheckQty * a.Std) / items.Where(a => a.Std > 0).Sum(a => a.CheckQty), 2) : 0;
            item.MT = items.Where(a => a.MT > 0).Sum(a => a.CheckQty) > 0 ?
              Math.Round(items.Where(a => a.MT > 0).Sum(a => a.CheckQty * a.MT) / items.Where(a => a.MT > 0).Sum(a => a.CheckQty), 2) : 0;
            item.ST = items.Where(a => a.ST > 0).Sum(a => a.CheckQty) > 0 ?
               Math.Round(items.Where(a => a.ST > 0).Sum(a => a.CheckQty * a.ST) / items.Where(a => a.ST > 0).Sum(a => a.CheckQty), 0) : 0;
            Dbers.GetInstance().SelfDber.Update<EstInFactoryBatch>(item);

        }
        #endregion


        #region 自动提取入厂抽样化验记录
        /// <summary>
        /// 自动提取入厂抽样化验记录煤质
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int AutoRCCYAssay(Action<string, eOutputType> output)
        {
            List<CmcsRCChouChaAssay> Assaylist = Dbers.GetInstance().SelfDber.Entities<CmcsRCChouChaAssay>("where CREATEDATE>:assayDate order by CREATEDATE asc", new { assayDate = DateTime.Now.AddDays(-7).Date });
            int res = 0;
            foreach (var item in Assaylist)
            {
                originaldata_cy = new List<OriginalData>();
                if (item.IsCheck != "1")
                {
                    CmcsFuelQuality Quality2 = getHyCyMz(item, item.TheFuelQuality);

                    Dbers.GetInstance().SelfDber.Update<CmcsFuelQuality>(Quality2);
                    res++;
                }
                
            }
            output(string.Format("生成抽样化验数据 {0} 条（集中管控）", res), eOutputType.Normal);

            return res;
        }
        /// <summary>
        /// 提取抽样化验煤质
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ManCode"></param>
        /// <param name="Quality"></param>
        /// <returns></returns>
        private CmcsFuelQuality getHyCyMz(CmcsRCChouChaAssay entity, CmcsFuelQuality Quality)
        {

            SumQbad_cy(entity.AssayCode);
            SumStad_cy(entity.AssayCode);
            SumMt_cy(entity.AssayCode);
            SumMad_cy(entity.AssayCode);
            SumAad_cy(entity.AssayCode);
            SumVad_cy(entity.AssayCode);
                //得到氢值
            GetHad_cy(entity.AssayCode);

            Quality.QbAd = originaldata_cy.Where(a => a.AssayTarget == "Qbad").ToList().Count == 0 ? 0
                 : originaldata_cy.Where(a => a.AssayTarget == "Qbad").FirstOrDefault().OAssayCalValue;
            Quality.Stad = originaldata_cy.Where(a => a.AssayTarget == "Stad").ToList().Count == 0 ? 0
                    : originaldata_cy.Where(a => a.AssayTarget == "Stad").FirstOrDefault().OAssayCalValue;
            Quality.Mt = originaldata_cy.Where(a => a.AssayTarget == "Mt").ToList().Count == 0 ? 0
                    : originaldata_cy.Where(a => a.AssayTarget == "Mt").FirstOrDefault().OAssayCalValue;
            Quality.Mad = AssayCalcUtil.GetTrunValue(originaldata_cy.Where(a => a.AssayTarget == "Mad").ToList().Count == 0 ? 0
                    : originaldata_cy.Where(a => a.AssayTarget == "Mad").FirstOrDefault().OAssayCalValue, 2);
            Quality.Vad = AssayCalcUtil.GetTrunValue(originaldata_cy.Where(a => a.AssayTarget == "Vad").ToList().Count == 0 ? 0
                    : originaldata_cy.Where(a => a.AssayTarget == "Vad").FirstOrDefault().OAssayCalValue, 2);
                Quality.Aad = AssayCalcUtil.GetTrunValue(originaldata_cy.Where(a => a.AssayTarget == "Aad").ToList().Count == 0 ? 0
                    : originaldata_cy.Where(a => a.AssayTarget == "Aad").FirstOrDefault().OAssayCalValue, 2);
                Quality.Had = AssayCalcUtil.GetTrunValue(originaldata_cy.Where(a => a.AssayTarget == "Had").ToList().Count == 0 ? 0
                       : originaldata_cy.Where(a => a.AssayTarget == "Had").FirstOrDefault().OAssayCalValue, 2);

                //根据是否有效来判断是否有值
                if (originaldata_cy.Where(a => a.AssayTarget == "Qbad").FirstOrDefault() != null)
                    if (originaldata_cy.Where(a => a.AssayTarget == "Qbad").FirstOrDefault().Isvalid == "0") Quality.QbAd = 0.00m;
                if (originaldata_cy.Where(a => a.AssayTarget == "Stad").FirstOrDefault() != null)
                    if (originaldata_cy.Where(a => a.AssayTarget == "Stad").FirstOrDefault().Isvalid == "0") Quality.Stad = 0.00m;
                if (originaldata_cy.Where(a => a.AssayTarget == "Mt").FirstOrDefault() != null)
                    if (originaldata_cy.Where(a => a.AssayTarget == "Mt").FirstOrDefault().Isvalid == "0") Quality.Mt = 0.00m;
                if (originaldata_cy.Where(a => a.AssayTarget == "Mad").FirstOrDefault() != null)
                    if (originaldata_cy.Where(a => a.AssayTarget == "Mad").FirstOrDefault().Isvalid == "0") Quality.Mad = 0.00m;
                if (originaldata_cy.Where(a => a.AssayTarget == "Vad").FirstOrDefault() != null)
                    if (originaldata_cy.Where(a => a.AssayTarget == "Vad").FirstOrDefault().Isvalid == "0") Quality.Vad = 0.00m;
                if (originaldata_cy.Where(a => a.AssayTarget == "Aad").FirstOrDefault() != null)
                    if (originaldata_cy.Where(a => a.AssayTarget == "Aad").FirstOrDefault().Isvalid == "0") Quality.Aad = 0.00m;


                Quality = getQuality(Quality);

     
            return Quality;
        
        }


        #region 汇总指标值(抽样化验)
        void SumQbad_cy(string assayBillnumber)
        {
            //var Assaylist = Dbers.GetInstance().SelfDber.Entities<RLHYAssay>("where mancode=:mancode order by ArriveTime asc", new { mancode = BillNumber[i] });
            IList<CmcsHeatStdAssay> heatAssay = Dbers.GetInstance().SelfDber.Entities<CmcsHeatStdAssay>("where SAMPLENUMBER=:assayBillnumber and IsEffective=1 and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();

            if (heatAssay.Count != 0 && heatAssay.Count >= 2)
            {
                string isvalid = "0";
                int isShowRed = heatAssay.Count > 2 ? 1 : 0;
                decimal value1 = heatAssay[0].Qbad;
                decimal value2 = heatAssay[1].Qbad;
                if (AssayCalcUtil.IsEffectiveQbad(value1, value2))
                    isvalid = "1";
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Qbad",
                    AssayFromDevice = heatAssay[0].FacilityNumber,
                    OQbad = heatAssay[0].Qbad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(heatAssay[0].Qbad, heatAssay[1].Qbad, 3),
                    OAssayUser = heatAssay[0].AssayUser,
                    OAssayTime = heatAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Qbad",
                    AssayFromDevice = heatAssay[1].FacilityNumber,
                    OQbad = heatAssay[1].Qbad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(heatAssay[0].Qbad, heatAssay[1].Qbad, 3),
                    OAssayUser = heatAssay[1].AssayUser,
                    OAssayTime = heatAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });

            }
        }

        void SumStad_cy(string assayBillnumber)
        {
            IList<CmcsSulfurStdAssay> stadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsSulfurStdAssay>("where SAMPLENUMBER=:assayBillnumber and IsEffective=1 and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsSulfurStdAssay> stadAssay = singleServiceBiz.GetSpecified<CmcsSulfurStdAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.IsEffective == 1).ToList();

            if (stadAssay.Count != 0 && stadAssay.Count >= 2)
            {
                string isvalid = "0";
                int isShowRed = stadAssay.Count > 2 ? 1 : 0;
                decimal value1 = stadAssay[0].Stad;
                decimal value2 = stadAssay[1].Stad;

                if (AssayCalcUtil.IsEffectiveStad(value1, value2))
                    isvalid = "1";
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Stad",
                    AssayFromDevice = stadAssay[0].FacilityNumber,
                    OStad = stadAssay[0].Stad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(stadAssay[0].Stad, stadAssay[1].Stad, 3),
                    OAssayUser = stadAssay[0].AssayUser,
                    OAssayTime = stadAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Stad",
                    AssayFromDevice = stadAssay[1].FacilityNumber,
                    OStad = stadAssay[1].Stad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(stadAssay[0].Stad, stadAssay[1].Stad, 3),
                    OAssayUser = stadAssay[1].AssayUser,
                    OAssayTime = stadAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumMt_cy(string assayBillnumber)
        {
            IList<CmcsMoistureStdAssay> mtAssay = Dbers.GetInstance().SelfDber.Entities<CmcsMoistureStdAssay>("where SAMPLENUMBER=:assayBillnumber and IsEffective=1 and WaterType='全水分' and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsMoistureStdAssay> mtAssay = singleServiceBiz.GetSpecified<CmcsMoistureStdAssay>(a =>
            // a.SampleNumber.StartsWith(assayBillnumber) && a.WaterType == "全水分" && a.IsEffective == 1).ToList();

            if (mtAssay.Count != 0 && mtAssay.Count >= 2)
            {
                decimal value1 = mtAssay[0].WaterPer;
                decimal value2 = mtAssay[1].WaterPer;

                decimal val1 = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1);
                //默认不合格
                string isvalid = "0";
                int isShowRed = mtAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveMt(value1, value2))
                    //设为合格
                    isvalid = "1";
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mt",
                    AssayFromDevice = mtAssay[0].FacilityNumber,
                    OMt = mtAssay[0].WaterPer,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1),
                    OAssayUser = mtAssay[0].AssayUser,
                    OAssayTime = mtAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mt",
                    AssayFromDevice = mtAssay[1].FacilityNumber,
                    OMt = mtAssay[1].WaterPer,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1),
                    OAssayUser = mtAssay[1].AssayUser,
                    OAssayTime = mtAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumMad_cy(string assayBillnumber)
        {
            #region write by wujie 内水
            //Mad=(干燥前总重量-干燥后总重量)/样重*100
            //（称量瓶重CLPZM+样重YZM-残毛重CMZM）/样重YZM*100
            //得到对应的记录（Mad\Vad\Aad)
            //string sql = "where SAMPLENUMBER like'%{0}%' and IsEffective=1 and CLPZM>=0 and YZM>=0 and CMZM>=0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber";

            IList<CmcsScaleAssay> madAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and CLPZM>0 and YZM>0 and CMZM>0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsScaleAssay> madAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.CLPZM >= 0 && a.YZM >= 0 && a.CMZM >= 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            if (madAssay.Count != 0 && madAssay.Count >= 2)
            {
                decimal mad1 = madAssay[0].Mad;
                decimal mad2 = madAssay[1].Mad;
                string isvalid = "0";
                int isShowRed = madAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveMad(mad1, mad2))
                    isvalid = "1";
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mad",
                    AssayFromDevice = madAssay[0].MachineCode,
                    OMad = madAssay[0].Mad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(madAssay[0].Mad, madAssay[1].Mad, 3),
                    OAssayUser = madAssay[0].AssayUser,
                    OAssayTime = madAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mad",
                    AssayFromDevice = madAssay[1].MachineCode,
                    OMad = madAssay[1].Mad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(madAssay[0].Mad, madAssay[1].Mad, 3),
                    OAssayUser = madAssay[1].AssayUser,
                    OAssayTime = madAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
            #endregion
        }

        void SumVad_cy(string assayBillnumber)
        {
            #region write by wujie
            //Vad=(灼烧前总重量-灼烧后总重量)/样重*100-Mad
            //Vad=(坩埚重GGZV+样重YZV-残毛重CMZV)/YZV*100-(（称量瓶重CLPZM+样重YZM-残毛重CMZM）/样重YZM*100)
            #endregion
            IList<CmcsScaleAssay> vadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and GGZV>0 and YZV>0 and CMZV>0 and CLPZM>0 and YZM>0 and CMZM>0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsScaleAssay> vadAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.GGZV > 0 && a.YZV > 0 && a.CMZV > 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            if (vadAssay.Count != 0 && vadAssay.Count >= 2)
            {
                decimal vad1 = vadAssay[0].Vad;
                decimal vad2 = vadAssay[1].Vad;
                string isvalid = "0";
                int isShowRed = vadAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveVad(vad1, vad2))
                    isvalid = "1";
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Vad",
                    AssayFromDevice = vadAssay[0].MachineCode,
                    OVad = vadAssay[0].Vad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(vadAssay[0].Vad, vadAssay[1].Vad, 3),
                    OAssayUser = vadAssay[0].AssayUser,
                    OAssayTime = vadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Vad",
                    AssayFromDevice = vadAssay[1].MachineCode,
                    OVad = vadAssay[1].Vad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(vadAssay[0].Vad, vadAssay[1].Vad, 3),
                    OAssayUser = vadAssay[1].AssayUser,
                    OAssayTime = vadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumAad_cy(string assayBillnumber)
        {
            #region write by wujie 空干基灰分
            //Aad=(灼烧后总重量-容器重量)/样重*100
            //（残毛重CMZA-瓷舟重CZZA）/样重YZA*100
            //得到对应的记录（Mad\Vad\Aad)
            //IList<CmcsScaleAssay> aadAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.CMZA != 0 && a.CZZA != 0 && a.YZA != 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            IList<CmcsScaleAssay> aadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and CMZA!=0 and CZZA!=0 and YZA!=0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            if (aadAssay.Count != 0 && aadAssay.Count >= 2)
            {
                decimal aad1 = aadAssay[0].Aad;
                decimal aad2 = aadAssay[1].Aad;
                string isvalid = "0";
                int isShowRed = aadAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveAad(aad1, aad2))
                    isvalid = "1";
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Aad",
                    AssayFromDevice = aadAssay[0].MachineCode,
                    OAad = aadAssay[0].Aad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(aadAssay[0].Aad, aadAssay[1].Aad, 3),
                    OAssayUser = aadAssay[0].AssayUser,
                    OAssayTime = aadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Aad",
                    AssayFromDevice = aadAssay[1].MachineCode,
                    OAad = aadAssay[1].Aad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(aadAssay[0].Aad, aadAssay[1].Aad, 3),
                    OAssayUser = aadAssay[1].AssayUser,
                    OAssayTime = aadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
            #endregion

        }

        void GetHad_cy(string assayBillnumber)
        {
            #region write by wujie
            #endregion
            //IList<CmcsElementAssay> hadAssay = base.SingleService.GetSpecified<CmcsElementAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.Had > 0 && a.IsEffective == 1).ToList();
            IList<CmcsElementAssay> hadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsElementAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and Had>0", new { assayBillnumber = assayBillnumber }).ToList();

            if (hadAssay == null || hadAssay.Count == 0)
            {
                //取化验时间最近的一条

            }
            if (hadAssay.Count >= 2)
            {
                decimal had1 = hadAssay[0].Had;
                decimal had2 = hadAssay[1].Had;
                string isvalid = "1";
                int isShowRed = hadAssay.Count > 2 ? 1 : 0;
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Had",
                    AssayFromDevice = hadAssay[0].FacilityNumber,
                    OHad = hadAssay[0].Had,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(hadAssay[0].Had, hadAssay[1].Had, 2),
                    OAssayUser = hadAssay[0].AssayUser,
                    OAssayTime = hadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cy.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Had",
                    AssayFromDevice = hadAssay[1].FacilityNumber,
                    OHad = hadAssay[1].Had,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(hadAssay[0].Had, hadAssay[1].Had, 2),
                    OAssayUser = hadAssay[1].AssayUser,
                    OAssayTime = hadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        #endregion


   

        //自动计算值
        public CmcsFuelQuality getQuality(CmcsFuelQuality Quality)
        {
            if (100 - Quality.Mad == 0)
            {
                Quality.Ad = 0;
            }
            else
            {
                Quality.Ad = CalcAvgValueS((100 / (100 - Quality.Mad) * Quality.Aad), 2);
            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Var = 0;
            }
            else
            {
                // 收到基会发份(Var)%   Vad(%)*((100-Mt)/(100-Mad))
                Quality.Var = CalcAvgValueS((Quality.Vad) * (100 - Quality.Mt) / (100 - Quality.Mad), 2);

            }
            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Vdaf = 0;
            }
            else
            {
                //干燥无灰基挥发份(Vdaf)%  Vdaf=100/(100-Mad-Aad)*Vad
                Quality.Vdaf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Vad), 2);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Std = 0;
            }
            else
            {

                // 干燥基全硫(St,d)%   St,d=100/(100-Mad)*St,ad
                Quality.Std = CalcAvgValueS((100 / (100 - Quality.Mad) * Quality.Stad), 2);


            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Star = 0;
            }
            else
            {
                //收到基全硫(St,ar)%   St,ar=(100-Mar)/(100-M)*ST
                Quality.Star = CalcAvgValueS(((100 - Quality.Mt) / (100 - Quality.Mad) * Quality.Stad), 2);
            }
            decimal a = 0.001m;
            if (Quality.QbAd <= 16.70m)
                a = 0.001m;
            else
            {
                if (Quality.QbAd > 25.10m)
                    a = 0.0016m;
                else
                    a = 0.0012m;
            }


            //if (Qgrad.Attributes["type"] == "balance")
            Quality.Qgrad = CalcAvgValueS(((Quality.QbAd * 1000 - (94.1m * Quality.Stad + a * Quality.QbAd * 1000m)) / 1000), 3);
            //else
            //    Qgrad.Text = CalcAvgValue(((decimal.Parse(QbAd.Text) * 1000 - (94.1m * decimal.Parse(Stad.Text) + a * decimal.Parse(QbAd.Text) * 1000m)) / 1000), 2).ToString();

            decimal qbj = ((Quality.QbAd * 1000 - (94.1m * Quality.Stad + a * Quality.QbAd * 1000m)) / 1000);
            if (100 - Quality.Mad == 0)
            {
                Quality.QJ = 0;
            }
            else
            {
                //Qnet,ar  MJ/kg

                Quality.QJ = CalcAvgValueS((((qbj * 1000 - 206 * Quality.Had) * ((100 - Quality.Mt) / (100 - Quality.Mad)) - 23 * Quality.Mt) / 1000), 3);

            }
            //(Qnet,ar)Kcal/kg

            Quality.QCal = decimal.Parse(CalcAvgValueS((Quality.QJ * 1000m / 4.1816m), 2).ToString("0"));


            if (100 - Quality.Mad == 0)
            {
                Quality.Qgrd = 0;
            }
            else
            {

                // MJ/kg
                Quality.Qgrd = CalcAvgValueS((Quality.Qgrad * (100 / (100 - Quality.Mad))), 3);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Aar = 0;
            }
            else
            {

                Quality.Aar = CalcAvgValueS((Quality.Aad * ((100 - Quality.Mt) / (100 - Quality.Mad))), 2);


            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Vd = 0;
            }
            else
            {
                Quality.Vd = CalcAvgValueS(Quality.Vad * (100 / (100 - (Quality.Mad))), 2);
            }
            Quality.FCad = (100 - (Quality.Mad + Quality.Aad + Quality.Vad));

            //Har%=[100Vd/(100-Vdaf)]*[7.35/(Vdaf+10)-0.013]*[(100-Ad)/100]*(100-Mar)/100  "Mar就相当于Mt”
            //Had(空气干燥基氢值)=(100-Mar)Har/(100-Mad)

            decimal vd = Quality.Vd;
            decimal vdaf = Quality.Vdaf;
            decimal ad = Quality.Ad;
            decimal mt = Quality.Mt;
            decimal mad = Quality.Mad;
            decimal vad = Quality.Vad;
            decimal pq;
            if (100m - Quality.Mad == 0)
            {
                pq = 0;
            }
            else
            {
                //新： P收到基低位热量（Qnet,ar）kcal/kg  [Qgr,v,ad-212*Had-0.8*（空气干燥基氧值（Nod）+空气干燥基氮值（Nad） ）]* （100-M）/（100-Mad） - 23*M
                pq = (qbj * 1000 - 212m * (Quality.Had) - 0.8m * (Quality.Oad + Quality.Nad)) * (100m - Quality.Mt) / (100m - Quality.Mad) - 24.4m * Quality.Mt;
            }
            Quality.QCal = decimal.Parse(Math.Round(pq / 4.1816m, 0).ToString("f0"));
            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Hdaf = 0;
            }
            else
            {
                //干燥无灰基氢值(H,daf)%
                Quality.Hdaf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Had), 2);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Har = 0;
            }
            else
            {
                //收到基氢值(H,ar)%
                Quality.Har = CalcAvgValueS((Quality.Had * (100 - Quality.Mt) / (100 - Quality.Mad)), 2);

            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Hd = 0;
            }
            else
            {
                //干燥基氢值(H,d)%
                Quality.Hd = CalcAvgValueS((100 / (100 - Quality.Mad) * Quality.Had), 2);

            }
            //干燥基固定碳(FC,d)%
            if (100 - Quality.Mad == 0)
            {
                Quality.FCd = 0;
            }
            else
            {
                Quality.FCd = Math.Round((100 / (100 - Quality.Mad) * Quality.FCad), 2);
            }
            if (100 - Quality.Mad == 0)
            {
                Quality.FCar = 0;
            }
            else
            {
                //收到基固定碳(FC,ar)%

                Quality.FCar = CalcAvgValueS((Quality.FCad * (100 - Quality.Mt) / (100 - Quality.Mad)), 2);


            }
            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Qnetdaf = 0;
            }
            else
            {
                //干燥无灰基高位热量(Qnet,daf)MJ/kg

                Quality.Qnetdaf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Qgrad), 2);
            }

            if (100 - Quality.Mad - Quality.Aad == 0)
            {
                Quality.Stadf = 0;
            }
            else
            {
                // 干燥无灰基硫(St,daf)%

                Quality.Stadf = CalcAvgValueS((100 / (100 - Quality.Mad - Quality.Aad) * Quality.Stad), 2);


            }
            if (100 - Quality.Mad == 0)
            {
                Quality.Qgrar = 0;
            }
            else
            {
                //收到基高位热量(Qgr,ar)MJ/kg

                Quality.Qgrar = CalcAvgValueS((Quality.Qgrad * (100 - Quality.Mt) / (100 - Quality.Mad)), 2);

            }
            //干燥无灰基氢值（Hadf）= 0.00117*干燥基灰分+0.57*根号干燥无灰基挥发分(V,daf)%  +0.1362*干燥无灰基高位热值/1000-2.806

            Quality.Hdaf = CalcAvgValueS(0.00117m * Quality.Ad + 0.57m * Convert.ToDecimal(Math.Sqrt(double.Parse(Quality.Vdaf.ToString()))) + 0.1362m * Quality.Qnetdaf - 2.806m, 2);


            ////空干基氢值=（100-空干基水分—空干基灰分）/100*干燥无灰基氢值（Hadf）
            //Had.Text = Math.Round(((100 - decimal.Parse(Mad.Text) - decimal.Parse(Aad.Text)) * decimal.Parse(Hdaf.Text)) / 100, 2).ToString();

            if (100 - Quality.Mad == 0)
            {
                Quality.Har = 0;
            }
            else
            {
                //收到基氢值(H, ar) %
                Quality.Har = CalcAvgValueS((Quality.Had * (100 - Quality.Mt) / (100 - Quality.Mad)), 2);

            }
            return Quality;
        }

        #endregion



        #region 自动提取入厂测试管理化验记录
        /// <summary>
        /// 自动提取入厂测试管理化验记录
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int AutoRCCSAssay(Action<string, eOutputType> output)
        {
            List<CmcsTestAssay> Assaylist = Dbers.GetInstance().SelfDber.Entities<CmcsTestAssay>("where CREATEDATE>:assayDate order by CREATEDATE asc", new { assayDate = DateTime.Now.AddDays(-7).Date });
            int res = 0;
            foreach (var item in Assaylist)
            {
                originaldata_cs = new List<OriginalData>();
                if (item.IsCheck != "1")
                {
                    CmcsFuelQuality Quality2 = getHyCsMz(item, item.TheFuelQuality);

                    Dbers.GetInstance().SelfDber.Update<CmcsFuelQuality>(Quality2);
                    res++;
                }

            }
            output(string.Format("生成入厂测试管理化验数据 {0} 条（集中管控）", res), eOutputType.Normal);

            return res;
        }
        /// <summary>
        /// 提取测试管理化验煤质
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ManCode"></param>
        /// <param name="Quality"></param>
        /// <returns></returns>
        private CmcsFuelQuality getHyCsMz(CmcsTestAssay entity, CmcsFuelQuality Quality)
        {

            SumQbad_cs(entity.AssayCode);
            SumStad_cs(entity.AssayCode);
            SumMt_cs(entity.AssayCode);
            SumMad_cs(entity.AssayCode);
            SumAad_cs(entity.AssayCode);
            SumVad_cs(entity.AssayCode);
            //得到氢值
            GetHad_cs(entity.AssayCode);

            Quality.QbAd = originaldata_cs.Where(a => a.AssayTarget == "Qbad").ToList().Count == 0 ? 0
                 : originaldata_cs.Where(a => a.AssayTarget == "Qbad").FirstOrDefault().OAssayCalValue;
            Quality.Stad = originaldata_cs.Where(a => a.AssayTarget == "Stad").ToList().Count == 0 ? 0
                    : originaldata_cs.Where(a => a.AssayTarget == "Stad").FirstOrDefault().OAssayCalValue;
            Quality.Mt = originaldata_cs.Where(a => a.AssayTarget == "Mt").ToList().Count == 0 ? 0
                    : originaldata_cs.Where(a => a.AssayTarget == "Mt").FirstOrDefault().OAssayCalValue;
            Quality.Mad = AssayCalcUtil.GetTrunValue(originaldata_cs.Where(a => a.AssayTarget == "Mad").ToList().Count == 0 ? 0
                    : originaldata_cs.Where(a => a.AssayTarget == "Mad").FirstOrDefault().OAssayCalValue, 2);
            Quality.Vad = AssayCalcUtil.GetTrunValue(originaldata_cs.Where(a => a.AssayTarget == "Vad").ToList().Count == 0 ? 0
                    : originaldata_cs.Where(a => a.AssayTarget == "Vad").FirstOrDefault().OAssayCalValue, 2);
            Quality.Aad = AssayCalcUtil.GetTrunValue(originaldata_cs.Where(a => a.AssayTarget == "Aad").ToList().Count == 0 ? 0
                : originaldata_cs.Where(a => a.AssayTarget == "Aad").FirstOrDefault().OAssayCalValue, 2);
            Quality.Had = AssayCalcUtil.GetTrunValue(originaldata_cs.Where(a => a.AssayTarget == "Had").ToList().Count == 0 ? 0
                   : originaldata_cs.Where(a => a.AssayTarget == "Had").FirstOrDefault().OAssayCalValue, 2);

            //根据是否有效来判断是否有值
            if (originaldata_cs.Where(a => a.AssayTarget == "Qbad").FirstOrDefault() != null)
                if (originaldata_cs.Where(a => a.AssayTarget == "Qbad").FirstOrDefault().Isvalid == "0") Quality.QbAd = 0.00m;
            if (originaldata_cs.Where(a => a.AssayTarget == "Stad").FirstOrDefault() != null)
                if (originaldata_cs.Where(a => a.AssayTarget == "Stad").FirstOrDefault().Isvalid == "0") Quality.Stad = 0.00m;
            if (originaldata_cs.Where(a => a.AssayTarget == "Mt").FirstOrDefault() != null)
                if (originaldata_cs.Where(a => a.AssayTarget == "Mt").FirstOrDefault().Isvalid == "0") Quality.Mt = 0.00m;
            if (originaldata_cs.Where(a => a.AssayTarget == "Mad").FirstOrDefault() != null)
                if (originaldata_cs.Where(a => a.AssayTarget == "Mad").FirstOrDefault().Isvalid == "0") Quality.Mad = 0.00m;
            if (originaldata_cs.Where(a => a.AssayTarget == "Vad").FirstOrDefault() != null)
                if (originaldata_cs.Where(a => a.AssayTarget == "Vad").FirstOrDefault().Isvalid == "0") Quality.Vad = 0.00m;
            if (originaldata_cs.Where(a => a.AssayTarget == "Aad").FirstOrDefault() != null)
                if (originaldata_cs.Where(a => a.AssayTarget == "Aad").FirstOrDefault().Isvalid == "0") Quality.Aad = 0.00m;


            Quality = getQuality(Quality);


            return Quality;

        }


        #region 汇总指标值(抽样化验)
        void SumQbad_cs(string assayBillnumber)
        {
            //var Assaylist = Dbers.GetInstance().SelfDber.Entities<RLHYAssay>("where mancode=:mancode order by ArriveTime asc", new { mancode = BillNumber[i] });
            IList<CmcsHeatStdAssay> heatAssay = Dbers.GetInstance().SelfDber.Entities<CmcsHeatStdAssay>("where SAMPLENUMBER=:assayBillnumber and IsEffective=1 and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();

            if (heatAssay.Count != 0 && heatAssay.Count >= 2)
            {
                string isvalid = "0";
                int isShowRed = heatAssay.Count > 2 ? 1 : 0;
                decimal value1 = heatAssay[0].Qbad;
                decimal value2 = heatAssay[1].Qbad;
                if (AssayCalcUtil.IsEffectiveQbad(value1, value2))
                    isvalid = "1";
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Qbad",
                    AssayFromDevice = heatAssay[0].FacilityNumber,
                    OQbad = heatAssay[0].Qbad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(heatAssay[0].Qbad, heatAssay[1].Qbad, 3),
                    OAssayUser = heatAssay[0].AssayUser,
                    OAssayTime = heatAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Qbad",
                    AssayFromDevice = heatAssay[1].FacilityNumber,
                    OQbad = heatAssay[1].Qbad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(heatAssay[0].Qbad, heatAssay[1].Qbad, 3),
                    OAssayUser = heatAssay[1].AssayUser,
                    OAssayTime = heatAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });

            }
        }

        void SumStad_cs(string assayBillnumber)
        {
            IList<CmcsSulfurStdAssay> stadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsSulfurStdAssay>("where SAMPLENUMBER=:assayBillnumber and IsEffective=1 and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsSulfurStdAssay> stadAssay = singleServiceBiz.GetSpecified<CmcsSulfurStdAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.IsEffective == 1).ToList();

            if (stadAssay.Count != 0 && stadAssay.Count >= 2)
            {
                string isvalid = "0";
                int isShowRed = stadAssay.Count > 2 ? 1 : 0;
                decimal value1 = stadAssay[0].Stad;
                decimal value2 = stadAssay[1].Stad;

                if (AssayCalcUtil.IsEffectiveStad(value1, value2))
                    isvalid = "1";
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Stad",
                    AssayFromDevice = stadAssay[0].FacilityNumber,
                    OStad = stadAssay[0].Stad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(stadAssay[0].Stad, stadAssay[1].Stad, 3),
                    OAssayUser = stadAssay[0].AssayUser,
                    OAssayTime = stadAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Stad",
                    AssayFromDevice = stadAssay[1].FacilityNumber,
                    OStad = stadAssay[1].Stad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(stadAssay[0].Stad, stadAssay[1].Stad, 3),
                    OAssayUser = stadAssay[1].AssayUser,
                    OAssayTime = stadAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumMt_cs(string assayBillnumber)
        {
            IList<CmcsMoistureStdAssay> mtAssay = Dbers.GetInstance().SelfDber.Entities<CmcsMoistureStdAssay>("where SAMPLENUMBER=:assayBillnumber and IsEffective=1 and WaterType='全水分' and  to_char(assaytime,'yyyy')=to_char(sysdate,'yyyy')", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsMoistureStdAssay> mtAssay = singleServiceBiz.GetSpecified<CmcsMoistureStdAssay>(a =>
            // a.SampleNumber.StartsWith(assayBillnumber) && a.WaterType == "全水分" && a.IsEffective == 1).ToList();

            if (mtAssay.Count != 0 && mtAssay.Count >= 2)
            {
                decimal value1 = mtAssay[0].WaterPer;
                decimal value2 = mtAssay[1].WaterPer;

                decimal val1 = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1);
                //默认不合格
                string isvalid = "0";
                int isShowRed = mtAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveMt(value1, value2))
                    //设为合格
                    isvalid = "1";
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mt",
                    AssayFromDevice = mtAssay[0].FacilityNumber,
                    OMt = mtAssay[0].WaterPer,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1),
                    OAssayUser = mtAssay[0].AssayUser,
                    OAssayTime = mtAssay[0].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mt",
                    AssayFromDevice = mtAssay[1].FacilityNumber,
                    OMt = mtAssay[1].WaterPer,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(mtAssay[0].WaterPer, mtAssay[1].WaterPer, 1),
                    OAssayUser = mtAssay[1].AssayUser,
                    OAssayTime = mtAssay[1].AssayTime,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumMad_cs(string assayBillnumber)
        {
            #region write by wujie 内水
            //Mad=(干燥前总重量-干燥后总重量)/样重*100
            //（称量瓶重CLPZM+样重YZM-残毛重CMZM）/样重YZM*100
            //得到对应的记录（Mad\Vad\Aad)
            //string sql = "where SAMPLENUMBER like'%{0}%' and IsEffective=1 and CLPZM>=0 and YZM>=0 and CMZM>=0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber";

            IList<CmcsScaleAssay> madAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and CLPZM>0 and YZM>0 and CMZM>0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsScaleAssay> madAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.CLPZM >= 0 && a.YZM >= 0 && a.CMZM >= 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            if (madAssay.Count != 0 && madAssay.Count >= 2)
            {
                decimal mad1 = madAssay[0].Mad;
                decimal mad2 = madAssay[1].Mad;
                string isvalid = "0";
                int isShowRed = madAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveMad(mad1, mad2))
                    isvalid = "1";
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mad",
                    AssayFromDevice = madAssay[0].MachineCode,
                    OMad = madAssay[0].Mad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(madAssay[0].Mad, madAssay[1].Mad, 3),
                    OAssayUser = madAssay[0].AssayUser,
                    OAssayTime = madAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Mad",
                    AssayFromDevice = madAssay[1].MachineCode,
                    OMad = madAssay[1].Mad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(madAssay[0].Mad, madAssay[1].Mad, 3),
                    OAssayUser = madAssay[1].AssayUser,
                    OAssayTime = madAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
            #endregion
        }

        void SumVad_cs(string assayBillnumber)
        {
            #region write by wujie
            //Vad=(灼烧前总重量-灼烧后总重量)/样重*100-Mad
            //Vad=(坩埚重GGZV+样重YZV-残毛重CMZV)/YZV*100-(（称量瓶重CLPZM+样重YZM-残毛重CMZM）/样重YZM*100)
            #endregion
            IList<CmcsScaleAssay> vadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and GGZV>0 and YZV>0 and CMZV>0 and CLPZM>0 and YZM>0 and CMZM>0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            //IList<CmcsScaleAssay> vadAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.GGZV > 0 && a.YZV > 0 && a.CMZV > 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            if (vadAssay.Count != 0 && vadAssay.Count >= 2)
            {
                decimal vad1 = vadAssay[0].Vad;
                decimal vad2 = vadAssay[1].Vad;
                string isvalid = "0";
                int isShowRed = vadAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveVad(vad1, vad2))
                    isvalid = "1";
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Vad",
                    AssayFromDevice = vadAssay[0].MachineCode,
                    OVad = vadAssay[0].Vad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(vadAssay[0].Vad, vadAssay[1].Vad, 3),
                    OAssayUser = vadAssay[0].AssayUser,
                    OAssayTime = vadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Vad",
                    AssayFromDevice = vadAssay[1].MachineCode,
                    OVad = vadAssay[1].Vad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(vadAssay[0].Vad, vadAssay[1].Vad, 3),
                    OAssayUser = vadAssay[1].AssayUser,
                    OAssayTime = vadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        void SumAad_cs(string assayBillnumber)
        {
            #region write by wujie 空干基灰分
            //Aad=(灼烧后总重量-容器重量)/样重*100
            //（残毛重CMZA-瓷舟重CZZA）/样重YZA*100
            //得到对应的记录（Mad\Vad\Aad)
            //IList<CmcsScaleAssay> aadAssay = singleServiceBiz.GetSpecified<CmcsScaleAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.CMZA != 0 && a.CZZA != 0 && a.YZA != 0 && a.IsEffective == 1).OrderBy(a => a.MachineCode).ThenBy(a => a.SampleNumber).ToList();
            IList<CmcsScaleAssay> aadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsScaleAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and CMZA!=0 and CZZA!=0 and YZA!=0 and  to_char(assaydate,'yyyy')=to_char(sysdate,'yyyy') Order By MachineCode,SampleNumber", new { assayBillnumber = assayBillnumber }).ToList();
            if (aadAssay.Count != 0 && aadAssay.Count >= 2)
            {
                decimal aad1 = aadAssay[0].Aad;
                decimal aad2 = aadAssay[1].Aad;
                string isvalid = "0";
                int isShowRed = aadAssay.Count > 2 ? 1 : 0;
                if (AssayCalcUtil.IsEffectiveAad(aad1, aad2))
                    isvalid = "1";
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Aad",
                    AssayFromDevice = aadAssay[0].MachineCode,
                    OAad = aadAssay[0].Aad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(aadAssay[0].Aad, aadAssay[1].Aad, 3),
                    OAssayUser = aadAssay[0].AssayUser,
                    OAssayTime = aadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Aad",
                    AssayFromDevice = aadAssay[1].MachineCode,
                    OAad = aadAssay[1].Aad,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(aadAssay[0].Aad, aadAssay[1].Aad, 3),
                    OAssayUser = aadAssay[1].AssayUser,
                    OAssayTime = aadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
            #endregion

        }

        void GetHad_cs(string assayBillnumber)
        {
            #region write by wujie
            #endregion
            //IList<CmcsElementAssay> hadAssay = base.SingleService.GetSpecified<CmcsElementAssay>(a => a.SampleNumber.StartsWith(assayBillnumber) && a.Had > 0 && a.IsEffective == 1).ToList();
            IList<CmcsElementAssay> hadAssay = Dbers.GetInstance().SelfDber.Entities<CmcsElementAssay>("where SAMPLENUMBER like'%'||:assayBillnumber||'%' and IsEffective=1  and Had>0", new { assayBillnumber = assayBillnumber }).ToList();

            if (hadAssay == null || hadAssay.Count == 0)
            {
                //取化验时间最近的一条

            }
            if (hadAssay.Count >= 2)
            {
                decimal had1 = hadAssay[0].Had;
                decimal had2 = hadAssay[1].Had;
                string isvalid = "1";
                int isShowRed = hadAssay.Count > 2 ? 1 : 0;
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Had",
                    AssayFromDevice = hadAssay[0].FacilityNumber,
                    OHad = hadAssay[0].Had,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(hadAssay[0].Had, hadAssay[1].Had, 2),
                    OAssayUser = hadAssay[0].AssayUser,
                    OAssayTime = hadAssay[0].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
                originaldata_cs.Add(new OriginalData()
                {
                    AssayNum = assayBillnumber,
                    AssayTarget = "Had",
                    AssayFromDevice = hadAssay[1].FacilityNumber,
                    OHad = hadAssay[1].Had,
                    OAssayCalValue = AssayCalcUtil.CalcAvgValue(hadAssay[0].Had, hadAssay[1].Had, 2),
                    OAssayUser = hadAssay[1].AssayUser,
                    OAssayTime = hadAssay[1].AssayDate,
                    IsShowRed = isShowRed,
                    Isvalid = isvalid
                });
            }
        }

        #endregion


        #endregion

        #region 提取在线全水分析值
        public int AutoAssayOnLineMt(Action<string, eOutputType> output)
        {
            int res = 0;
            List<CmcsZXQSRecord> Assaylist = Dbers.GetInstance().SelfDber.Entities<CmcsZXQSRecord>("where CREATEDATE>:assayDate order by CREATEDATE asc", new { assayDate = DateTime.Now.AddDays(-365).Date });
            foreach (var item in Assaylist)
            {
                CmcsRCMake entity = Dbers.GetInstance().SelfDber.Entity<CmcsRCMake>("where MakeCode=:MakeCode", new { MakeCode = item.MakeCode });
                if (entity != null) {
                    CmcsRCAssay assay = Dbers.GetInstance().SelfDber.Entity<CmcsRCAssay>("where MakeId=:MakeId", new { MakeId = entity.Id });
                    if (assay != null) {
                        assay.OnLine_Mt = item.ResultMt;
                        Dbers.GetInstance().SelfDber.Update<CmcsRCAssay>(assay);
                        res++;
                    }

                }
            }
            output(string.Format("自动提取在线全水分析值 {0} 条（集中管控）", res), eOutputType.Normal);

            return res;

        }
        #endregion
    }

    [Serializable]
    class OriginalData
    {
        /// <summary>
        /// 是否为有效化验
        /// </summary>
        public string Isvalid { get; set; }
        /// <summary>
        /// 化验编码
        /// </summary>
        public string AssayNum { get; set; }

        /// <summary>
        /// 指标
        /// </summary>
        public string AssayTarget { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string AssayFromDevice { get; set; }

        /// <summary>
        /// 弹筒热值Qb,ad(MJ/kg)
        /// </summary>
        public decimal OQbad { get; set; }

        /// <summary>
        /// 空干基全硫（St,ad)
        /// </summary>
        public decimal OStad { get; set; }


        /// <summary>
        /// 水分值（Mt)
        /// </summary>
        public decimal OMt { get; set; }

        /// <summary>
        /// 空干基水分（M,ad)
        /// </summary>
        public decimal OMad { get; set; }

        /// <summary>
        /// 空干基灰分（A,ad)
        /// </summary>
        public decimal OAad { get; set; }

        /// <summary>
        /// 空干基挥发分（V,ad)
        /// </summary>
        public decimal OVad { get; set; }


        /// <summary>
        /// 空干基氢值（H,ad)
        /// </summary>
        public decimal OHad { get; set; }

        /// <summary>
        /// 平均值
        /// </summary>
        public decimal OAssayCalValue { get; set; }

        /// <summary>
        /// 化验用户
        /// </summary>
        public string OAssayUser { get; set; }

        /// <summary>
        /// 化验时间
        /// </summary>
        public DateTime OAssayTime { get; set; }

        /// <summary>
        /// 是否显示红色 如果原始数据大于两条就显示红色 默认为0
        /// </summary>
        public int IsShowRed { get; set; }
    }
}
