using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.DapperDber.Attrs;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice.Entities
{
    /// <summary>
    /// .测硫仪 型号：SDS212
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("HNQYCly_SDS212")]
    public class HNQYCly_SDS212
    {
        [DapperPrimaryKey]
        public string PKID { get; set; }
        /// <summary>
        /// 自动编号 主键
        /// </summary>
        public string AutoNmb { get; set; }
        /// <summary>
        /// 化验设备
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 试样编号
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 试样重量(mg)
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 分析水分(%)
        /// </summary>
        public decimal Mad { get; set; }
        /// <summary>
        /// 分析基硫(%)
        /// </summary>
        public decimal Stad { get; set; }
        /// <summary>
        /// 干基硫(%)
        /// </summary>
        public decimal Std { get; set; }
        /// <summary>
        /// 实验员
        /// </summary>
        public string Tester { get; set; }
        /// <summary>
        /// 实验日期
        /// </summary>
        public DateTime TestDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string TestTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 硫面积
        /// </summary>
        public decimal SArea { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Bake { get; set; }
        /// <summary>
        /// 是否是标样1-是0-不是
        /// </summary>
        public decimal StandardFlag { get; set; }
        /// <summary>
        /// 是低硫还是高硫
        /// </summary>
        public String SType { get; set; }
        /// <summary>
        /// 压力AD值
        /// </summary>
        public decimal PressAD { get; set; }
        /// <summary>
        /// 低硫初始AD
        /// </summary>
        public decimal LowSInitAD { get; set; }
        /// <summary>
        /// 高硫初始AD
        /// </summary>
        public decimal HighSInitAD { get; set; }
        /// <summary>
        /// 低硫累加值
        /// </summary>
        public decimal LowSSum { get; set; }
        /// <summary>
        /// 高硫累加值
        /// </summary>
        public decimal HighSSum { get; set; }
        /// <summary>
        /// 低硫峰值
        /// </summary>
        public decimal LowSPeak { get; set; }
        /// <summary>
        /// 高硫峰值
        /// </summary>
        public decimal HighSPeak { get; set; }
        /// <summary>
        /// 积分个数
        /// </summary>
        public decimal AddCount { get; set; }
        /// <summary>
        /// 低硫面积
        /// </summary>
        public decimal LowSArea { get; set; }
        /// <summary>
        /// 高硫面积
        /// </summary>
        public decimal HighSArea { get; set; }
        /// <summary>
        /// 漂移系数
        /// </summary>
        public decimal DriftPara { get; set; }
        /// <summary>
        /// 测试方法
        /// </summary>
        public String TestMethod { get; set; }
        /// <summary>
        /// 校正曲线
        /// </summary>
        public String Calibration { get; set; }
        /// <summary>
        /// 硬件卡ID
        /// </summary>
        public String CodeId { get; set; }
        /// <summary>
        /// 漂移前的分析基硫
        /// </summary>
        public decimal PreStad { get; set; }
        /// <summary>
        /// 空白样值
        /// </summary>
        public decimal BlankValue { get; set; }
        /// <summary>
        /// 大气压力值
        /// </summary>
        public decimal GasPress { get; set; }
        /// <summary>
        /// 样品名称
        /// </summary>
        public String SampleName { get; set; }
       
    }
}
