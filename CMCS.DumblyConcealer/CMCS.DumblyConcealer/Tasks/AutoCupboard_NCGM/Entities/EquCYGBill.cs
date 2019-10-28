using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities
{

    /// <summary>
    /// 南昌光明存样柜 
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("INFTBCYGBILL")]
    public class EquCYGBill : EntityBase2
    {
        /// <summary>
        /// 操作票类型
        /// </summary>
        public int CZPLX { get; set; }
        /// <summary>
        /// 操作票发送时间
        /// </summary>
        public DateTime CZPFSSJ { get; set; }
        /// <summary>
        /// 制样编码
        /// </summary>
        public String ZYBM { get; set; }
        /// <summary>
        /// 样瓶类型
        /// </summary>
        public Decimal YPLX { get; set; }
        /// <summary>
        /// 样瓶RFID编码
        /// </summary>
        public String YPRFIDBM { get; set; }
        /// <summary>
        /// 操作模式
        /// </summary>
        public Decimal CZMS { get; set; }
        /// <summary>
        /// 同步标志
        /// </summary>
        public Decimal DATAFLAG { get; set; }

        /// <summary>
        /// 操作记录
        /// </summary>
        [DapperDber.Attrs.DapperIgnore]
        public List<EquCYGBillRecord> InfCYGBillRecords
        {
            get
            {
                return DcDbers.GetInstance().AutoCupboard_Dber.Entities<EquCYGBillRecord>(" where BillId='" + this.Id + "'");
            }
        }
    }
}
