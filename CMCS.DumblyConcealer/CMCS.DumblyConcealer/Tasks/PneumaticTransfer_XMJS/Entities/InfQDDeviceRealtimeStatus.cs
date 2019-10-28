using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{
    /// <summary>
    /// 气动传输 
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("DeviceRealtimeStatus")]
    public class InfQDDeviceRealtimeStatus
    {
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        public int Id { get; set; }

        public int LineId { get; set; }

        /// <summary>
        /// 站点Id
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string DeviceName { get; set; }
        public int DeviceStatus1 { get; set; }

        /// <summary>
        /// 是否检测到样瓶
        /// </summary>
        public int DeviceStatus2 { get; set; }

        public int DeviceStatus3 { get; set; }
        public int LineStatus { get; set; }
        public int mainid { get; set; }

        public int DeviceStyle { get; set; }
    }
}
