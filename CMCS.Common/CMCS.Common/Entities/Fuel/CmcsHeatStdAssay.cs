﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys; 

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 化验数据-量热仪
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("FulTbHeatStdAssay")]
    public class CmcsHeatStdAssay : EntityBase1
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
        /// 弹桶热值
        /// </summary>
        public decimal Qbad { get; set; }

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
