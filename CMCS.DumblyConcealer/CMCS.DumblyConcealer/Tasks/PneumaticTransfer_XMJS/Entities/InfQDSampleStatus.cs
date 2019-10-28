using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{

    /// <summary>
    /// 气动工作站状态
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("SampleSate")]
    public class InfQDSampleStatus
    {
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        public Int32 Id { get; set; }

        /// <summary>
        /// 制样码
        /// </summary>
        public String SampleId { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 是否读取
        /// </summary>
        public Int32 IsRead { get; set; }

        /// <summary>
        /// 工作站id
        /// </summary>
        public Int32 DeviceId { get; set; }

        /// <summary>
        /// 目的地id
        /// </summary>
        public Int32 Dest { get; set; }
    }
}
