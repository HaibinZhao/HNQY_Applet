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
    [CMCS.DapperDber.Attrs.DapperBind("INFTBCYGBILLRECORD")]
    public class EquCYGBillRecord : EntityBase2
    {
        /// <summary>
        /// 操作任务表Id
        /// </summary>
        public String BillId { get; set; }
        
        /// <summary>
        /// 操作票类型
        /// </summary>
        public Decimal CZPLX { get; set; }
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
        /// 开始时间
        /// </summary>
        public DateTime KSSJ { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime JSSJ { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public String CZRY { get; set; }
        /// <summary>
        /// 操作票结果
        /// </summary>
        public Decimal CZPJG { get; set; }
        /// <summary>
        /// 同步标志
        /// </summary>
        public Decimal DATAFLAG { get; set; }
        [DapperDber.Attrs.DapperIgnore]
        public EquCYGBill TheInfCYGBill
        {
            get
            {
                return Dbers.GetInstance().SelfDber.Get<EquCYGBill>(this.BillId);
            }
        }
    }
}
