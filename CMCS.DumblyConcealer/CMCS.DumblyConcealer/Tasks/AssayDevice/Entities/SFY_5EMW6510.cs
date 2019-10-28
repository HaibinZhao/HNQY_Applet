using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.DapperDber.Attrs;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice.Entities
{
    /// <summary>
    /// .水分仪 型号：5E-MW6510
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("HYTBSFY_5EMW6510")]
    public class SFY_5EMW6510
    {
        [DapperPrimaryKey]
        public string PKID { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 自动编号
        /// </summary>
        public string Index { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 样品索引
        /// </summary>
        public int SampleID { get; set; }
        /// <summary>
        /// 样品编号
        /// </summary>
        public string SampleName { get; set; }
        /// <summary>
        /// 化验类型
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 化验方法
        /// </summary>
        public string Method { get; set; }
        public decimal Tare { get; set; }
        public decimal Sample { get; set; }
        public decimal Leftover { get; set; }
        public decimal Calibration { get; set; }
        /// <summary>
        /// 水分值
        /// </summary>
        public decimal Moisture { get; set; }
        public string TypeFlag { get; set; }
        public string Operator { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Numbering { get; set; }
        public decimal Temperature { get; set; }
        public decimal KeepTime { get; set; }
        public decimal Precision { get; set; }
        public decimal IntervalTime { get; set; }
        public decimal Times { get; set; }
        public string Tray { get; set; }
        public decimal CountNumber { get; set; }
    }
}
