using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.DapperDber.Attrs;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice.Entities
{
    /// <summary>
    /// 元素分析仪器
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("HNQYysfxy_Data")]
    public class HNQYysfxy_Data
    {
        [DapperPrimaryKey]
        public string PKID { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 试样自动编号
        /// </summary>
        public string 自动编号 { get; set; }
        /// <summary>
        /// 手动编号
        /// </summary>
        public string SampleNo { get; set; }
        /// <summary>
        /// 实验时间
        /// </summary>
        public DateTime TestDate { get; set; }
        /// <summary>
        /// 试样重量
        /// </summary>
        public string SAMPLEWEIGHT { get; set; }
        /// <summary>
        /// 氮Nad
        /// </summary>
        public string NAD { get; set; }
        /// <summary>
        /// 化验员
        /// </summary>
        public string TESTMAN { get; set; }
        /// <summary>
        /// 氢HAD
        /// </summary>
        public string HAD { get; set; }
        /// <summary>
        /// 碳CAD
        /// </summary>
        public string CAD { get; set; }
    }
}
