using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{

    /// <summary>
    /// 气动传输-命令表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("QD_INTERFACE_TB")]
    public class InfQDBill
    {
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        [CMCS.DapperDber.Attrs.DapperIgnore]
        public int Id { get; set; }

        /// <summary>
        /// 起始站
        /// </summary>
        public Decimal OpStart { get; set; }
        /// <summary>
        /// 目的站
        /// </summary>
        public Decimal OpEnd { get; set; }
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
        public Decimal DataStatus { get; set; }
        /// <summary>
        /// =0：未读
        ///=1：已读
        /// </summary>
        public Decimal readState { get; set; }
    }
}
