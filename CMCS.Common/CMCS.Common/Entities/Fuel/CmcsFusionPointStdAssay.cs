﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 化验数据-灰融仪
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("FulTbFusionPointStdAssay")]
    public class CmcsFusionPointStdAssay : EntityBase1
    {
        /// <summary>
        /// 化验编码
        /// </summary>
        public string SampleNumber { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string FacilityNumber { get; set; }
        /// <summary>
        /// 坩埚重量
        /// </summary>
        public decimal ContainerWeight { get; set; }
        /// <summary>
        /// 样品重量
        /// </summary>
        public decimal SampleWeight { get; set; }
        /// <summary>
        /// 变形温度DT
        /// </summary>
        public decimal DT { get; set; }
        /// <summary>
        /// 半球温度HT
        /// </summary>
        public decimal ST { get; set; }
        /// <summary>
        /// 软化温度ST
        /// </summary>
        public decimal HT { get; set; }
        /// <summary>
        /// 流动温度FT
        /// </summary>
        public decimal FT { get; set; }
        /// <summary>
        /// 化验用户
        /// </summary>
        public string AssayUser { get; set; }
        /// <summary>
        /// 化验日期
        /// </summary>
        public DateTime AssayTime { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public int IsEffective { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PKID { get; set; }
    }
}
