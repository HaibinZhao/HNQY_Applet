using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.DapperDber.Attrs;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice.Entities
{
    /// <summary>
    /// .测硫仪 型号：5E-8SAII
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("HNQYLry_SDC")]
    public class HNQYLry_SDC
    {

        [DapperPrimaryKey]
        public string PKID { get; set; }
        /// <summary>
        /// 自动编号
        /// </summary>
        public string AutoNo { get; set; }
        /// <summary>
        /// 化验设备
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 手动编号
        /// </summary>
        public string ManualNumber { get; set; }
        /// <summary>
        /// 试样质量(g)
        /// </summary>
        public decimal SampleWeight { get; set; }
        /// <summary>
        /// 弹筒发热量(J/g)
        /// </summary>
        public decimal Qbad { get; set; }
        /// <summary>
        /// 空干基高位(J/g)
        /// </summary>
        public decimal Qgrad { get; set; }
        /// <summary>
        /// 干基高位(J/g)
        /// </summary>
        public decimal Qgrd { get; set; }
        /// <summary>
        /// 收到基低位(J/g)
        /// </summary>
        public decimal Qnet_var { get; set; }
        /// <summary>
        /// 仪器热容量(J/K)
        /// </summary>
        public decimal EEValue { get; set; }
        /// <summary>
        /// 点火热(J)
        /// </summary>
        public decimal q1 { get; set; }
        /// <summary>
        /// 添加热(J)
        /// </summary>
        public decimal q2 { get; set; }
        /// <summary>
        /// 全水分(%)
        /// </summary>
        public decimal Mt { get; set; }
        /// <summary>
        /// 分析水分(%)
        /// </summary>
        public decimal Mad { get; set; }
        /// <summary>
        /// 全硫(%)
        /// </summary>
        public decimal Sbad { get; set; }
        /// <summary>
        /// 分析氢(%)
        /// </summary>
        public decimal Had { get; set; }
        /// <summary>
        /// 外水(%)
        /// </summary>
        public decimal Mar { get; set; }
        /// <summary>
        /// 挥发分(%)
        /// </summary>
        public decimal Vad { get; set; }
        /// <summary>
        /// 灰分(%)
        /// </summary>
        public decimal Aad { get; set; }
        /// <summary>
        /// 焦渣特性
        /// </summary>
        public decimal CokeSlag { get; set; }
        /// <summary>
        /// 测试日期
        /// </summary>
        public DateTime TestDate { get; set; }
        /// <summary>
        /// 最后修改
        /// </summary>
        public DateTime FinalUpdate { get; set; }
        /// <summary>
        /// 化验员
        /// </summary>
        public string Tester { get; set; }
        /// <summary>
        /// 试样类型
        /// </summary>
        public string SampleType { get; set; }
        /// <summary>
        /// 测试方法
        /// </summary>
        public string TestingMethod { get; set; }
        /// <summary>
        /// 氧弹编号
        /// </summary>
        public string BombNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 测V0第一个温度
        /// </summary>
        public decimal T00 { get; set; }
        /// <summary>
        /// T0
        /// </summary>
        public decimal T0 { get; set; }
        /// <summary>
        /// V0
        /// </summary>
        public decimal V0 { get; set; }
        /// <summary>
        /// Tn
        /// </summary>
        public decimal Tn { get; set; }
        /// <summary>
        /// 主期温升
        /// </summary>
        public decimal TempRise { get; set; }
        /// <summary>
        /// 主期时间
        /// </summary>
        public decimal Duration { get; set; }
        /// <summary>
        /// 主期积分值
        /// </summary>
        public decimal F_ti { get; set; }
        /// <summary>
        /// 末期温度
        /// </summary>
        public decimal TerminalTemp { get; set; }
        /// <summary>
        /// Vn
        /// </summary>
        public decimal Vn { get; set; }
        /// <summary>
        /// C
        /// </summary>
        public decimal C { get; set; }
        /// <summary>
        /// DeviceModel
        /// </summary>
        public String DeviceModel { get; set; }
        /// <summary>
        /// CANID
        /// </summary>
        public string CANID { get; set; }
        /// <summary>
        /// RSD1
        /// </summary>
        public string RSD1 { get; set; }
        /// <summary>
        /// RSD2
        /// </summary>
        public string RSD2 { get; set; }
        /// <summary>
        /// RSD3
        /// </summary>
        public string RSD3 { get; set; }
        /// <summary>
        /// 控温点
        /// </summary>
        public string JacketT { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// adjust
        /// </summary>
        public decimal adjust { get; set; }
        /// <summary>
        /// 碳（%）
        /// </summary>
        public decimal Cad { get; set; }
        /// <summary>
        /// 氧（%）
        /// </summary>
        public decimal Oad { get; set; }
        /// <summary>
        /// 弹筒发热量(Cal/g)
        /// </summary>
        public decimal Nad { get; set; }
        /// <summary>
        /// 空干基高位(Cal/g)
        /// </summary>
        public decimal Qgrad_Cal { get; set; }
        /// <summary>
        /// 干基高位(Cal/g)
        /// </summary>
        public decimal Qgrd_Cal { get; set; }
        /// <summary>
        /// 收到基低位(Cal/g)
        /// </summary>
        public decimal Qnet_var_Cal { get; set; }
        /// <summary>
        /// 弹筒发热量(MJ/kg)
        /// </summary>
        public decimal Qbad_MJ { get; set; }
        /// <summary>
        /// 空干基高位(MJ/kg)
        /// </summary>
        public decimal Qgrad_MJ { get; set; }
        /// <summary>
        /// 干基高位(MJ/kg)
        /// </summary>
        public decimal Qgrd_MJ { get; set; }
        /// <summary>
        /// 收到基低位(MJ/kg)
        /// </summary>
        public decimal Qnet_var_MJ { get; set; }
        /// <summary>
        /// 恒压低位（J/g)
        /// </summary>
        public decimal Qnet_par { get; set; }
        /// <summary>
        /// 恒压低位（MJ/kg)
        /// </summary>
        public decimal Qnet_par_MJ { get; set; }
        /// <summary>
        /// 恒压低位（Cal/g)
        /// </summary>
        public decimal Qnet_par_Cal { get; set; }
        /// <summary>
        /// 石油总热值
        /// </summary>
        public decimal Qd { get; set; }
        /// <summary>
        /// 石油净热值
        /// </summary>
        public decimal Qj { get; set; }
        /// <summary>
        /// 添加热(Cal)
        /// </summary>
        public decimal q2_Cal { get; set; }
        /// <summary>
        /// 仪器热容量(Cal/K)
        /// </summary>
        public decimal EEValue_Cal { get; set; }

    }
}
