using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Inf
{
    /// <summary>
    /// 第三方设备接口 - 气动传输中间表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("inftbInterface")]
    public class InfInterface : EntityBase1
    {
        /// <summary>
        /// 起始站
        /// </summary>
        public string OpStart { get; set; }
        /// <summary>
        /// 目的站
        /// </summary>
        public string OpEnd { get; set; }
        /// <summary>
        /// 操作人员代码
        /// </summary>
        public Decimal Operator_Code { get; set; }
        /// <summary>
        /// 样瓶编码
        /// </summary>
        public string SampleId { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Send_Time { get; set; }
        /// <summary>
        /// 数据发送状态
        /// </summary>
        public string DataStatus { get; set; }
    }
}
