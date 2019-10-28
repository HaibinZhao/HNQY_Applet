using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.DapperDber.Attrs;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice.Entities
{
    /// <summary>
    /// 灰融仪
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("HNQYHry_SDAF2000")]
    public class HNQYHry_SDAF2000
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
        public string MANNO { get; set; }
        /// <summary>
        /// DT
        /// </summary>
        public Decimal DTWD { get; set; }
        /// <summary>
        /// ST
        /// </summary>
        public Decimal STWD { get; set; }
        /// <summary>
        /// HT
        /// </summary>
        public Decimal HTWD { get; set; }
        /// <summary>
        /// FT
        /// </summary>
        public Decimal FTWD { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Decimal sybh { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime SYRQ { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string SYSJ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CSGD { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TXML { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MIDHZ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SBFS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MBGD { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal HZKD { get; set; }
    }
}
