using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{

    /// <summary>
    /// 气动传输-站点表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("TransmissionCode")]
    public class InfQDDeviceCode
    {
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        public int Id { get; set; }
        /// <summary>
        /// 站点编号
        /// </summary>
        public int ListIndex { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string DeviceName { get; set; }
        
    }
}
