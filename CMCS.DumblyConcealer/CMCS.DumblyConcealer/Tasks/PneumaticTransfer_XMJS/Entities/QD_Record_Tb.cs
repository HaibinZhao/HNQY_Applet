using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{
    [CMCS.DapperDber.Attrs.DapperBind("QD_Record_Tb")]
    public class QD_Record_Tb
    {
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
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
        /// 数据发送状态=0：未读取  =1：已读取

        /// </summary>
        public Decimal DataStatus { get; set; }

        /// <summary>
        /// 样瓶编码
        /// </summary>
        public string SampleId { get; set; }

    }
}
