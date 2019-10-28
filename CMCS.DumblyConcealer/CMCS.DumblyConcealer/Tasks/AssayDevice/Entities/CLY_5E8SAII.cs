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
    [CMCS.DapperDber.Attrs.DapperBind("HYTBCLY_5E8SAII")]
    public class CLY_5E8SAII
    {
        [DapperPrimaryKey]
        public string PKID { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public int NID { get; set; }
        /// <summary>
        /// 试样编号
        /// </summary>
        public string SYBH { get; set; }
        /// <summary>
        /// 试样样名
        /// </summary>
        public string SHYM { get; set; }
        /// <summary>
        /// 坩埚重
        /// </summary>
        public decimal GGZ { get; set; }
        /// <summary>
        /// 空干基水分
        /// </summary>
        public decimal KGJSF { get; set; }
        /// <summary>
        /// 空干基全硫
        /// </summary>
        public decimal KGJQL { get; set; }
        /// <summary>
        /// 干基全硫
        /// </summary>
        public decimal GJQL { get; set; }
        /// <summary>
        /// 化验员
        /// </summary>
        public string HYY { get; set; }
        /// <summary>
        /// 测试日期
        /// </summary>
        public DateTime CSRQ { get; set; }
    }
}
