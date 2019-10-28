using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Inf
{
    /// <summary>
    /// 第三方设备接口 - 气动传输记录表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("inftbRecord")]
    public class infRecord : EntityBase1
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
        /// 操作结果=1：成功 =2：失败
        /// </summary>
        public string SampleStatus { get; set; }

        /// <summary>
        /// 传输开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 传输完成时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 样瓶编码
        /// </summary>
        public string Code { get; set; }

    }
}
