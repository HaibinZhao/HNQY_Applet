using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 化验数据-元素分析仪
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("FULTBELEMENTSTDASSAY")]
    public class CmcsElementStdAssay : EntityBase1
    {
        public string PKID { get; set; }
        /// <summary>
        /// 化验编码
        /// </summary>
        public string SAMPLENUMBER { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string FACILITYNUMBER { get; set; }
        /// <summary>
        /// 坩埚重量
        /// </summary>
        public decimal CONTAINERWEIGHT { get; set; }
        /// <summary>
        /// 样品重量
        /// </summary>
        public decimal SAMPLEWEIGHT { get; set; }
        /// <summary>
        /// 氮Nad
        /// </summary>
        public decimal NAD { get; set; }
        /// <summary>
        /// 氢Had
        /// </summary>
        public decimal HAD { get; set; }
        /// <summary>
        /// 碳Cad
        /// </summary>
        public decimal CAD { get; set; }
        /// <summary>
        /// 化验用户
        /// </summary>
        public string ASSAYUSER { get; set; }
        /// <summary>
        /// 化验日期
        /// </summary>
        public DateTime ASSAYTIME { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public int IsEffective { get; set; }


    }
}
