using CMCS.Common.Entities.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Entities.TrainInFactory
{
    /// <summary>
    /// 火车车厢入厂记录表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("fultbtrainweighter")]
    public class FulTrainWeightRecord : EntityBase1
    {
        private string _PKID;
        /// <summary>
        /// 第三方主键
        /// </summary>
        public virtual string PKID { get { return _PKID; } set { _PKID = value; } }

        private Int32 _OrderNumber;
        /// <summary>
        /// 入厂顺序
        /// </summary>
        public virtual Int32 OrderNumber { get { return _OrderNumber; } set { _OrderNumber = value; } }

        private String _CarNumber;
        /// <summary>
        /// 车号
        /// </summary>
        public virtual String CarNumber { get { return _CarNumber; } set { _CarNumber = value; } }

        private String _CarModel;
        /// <summary>
        /// 车型
        /// </summary>
        public virtual String CarModel { get { return _CarModel; } set { _CarModel = value; } }

        private Decimal _TicketWeight;
        /// <summary>
        /// 票重(吨)
        /// </summary>
        public virtual Decimal TicketWeight { get { return _TicketWeight; } set { _TicketWeight = value; } }

        private Decimal _GrossWeight;
        /// <summary>
        /// 毛重(吨)
        /// </summary>
        public virtual Decimal GrossWeight { get { return _GrossWeight; } set { _GrossWeight = value; } }

        private Decimal _TareWeight;
        /// <summary>
        /// 皮重(吨)
        /// </summary>
        public virtual Decimal TareWeight { get { return _TareWeight; } set { _TareWeight = value; } }

        private Decimal _SuttleWeight;
        /// <summary>
        /// 净重(吨)
        /// </summary>
        public virtual Decimal SuttleWeight { get { return _SuttleWeight; } set { _SuttleWeight = value; } }

        private Decimal _DeductWeight;
        /// <summary>
        /// 扣吨(吨)
        /// </summary>
        public virtual Decimal DeductWeight { get { return _DeductWeight; } set { _DeductWeight = value; } }

        private Decimal _MarginWeight;
        /// <summary>
        /// 盈亏(吨)
        /// </summary>
        public virtual Decimal MarginWeight { get { return _MarginWeight; } set { _MarginWeight = value; } }

        private DateTime _GrossDate;
        /// <summary>
        /// 过衡时间、毛重时间
        /// </summary>
        public virtual DateTime GrossDate { get { return _GrossDate; } set { _GrossDate = value; } }

        private DateTime _TareDate;
        /// <summary>
        /// 回皮时间
        /// </summary>
        public virtual DateTime TareDate { get { return _TareDate; } set { _TareDate = value; } }

        private Decimal _Speed;
        /// <summary>
        /// 车速(米/秒)
        /// </summary>
        public virtual Decimal Speed { get { return _Speed; } set { _Speed = value; } }

        private String _MesureMan;
        /// <summary>
        /// 过衡人
        /// </summary>
        public virtual String MesureMan { get { return _MesureMan; } set { _MesureMan = value; } }

        private String _SupplierName;
        /// <summary>
        /// 供煤单位
        /// </summary>
        public virtual String SupplierName { get { return _SupplierName; } set { _SupplierName = value; } }

        private String _MineName;
        /// <summary>
        /// 矿点
        /// </summary>
        public virtual String MineName { get { return _MineName; } set { _MineName = value; } }

        private String _FuelKindName;
        /// <summary>
        /// 煤种（货名）
        /// </summary>
        public virtual String FuelKindName { get { return _FuelKindName; } set { _FuelKindName = value; } }

        private String _ReceiveName;
        /// <summary>
        /// 收货单位
        /// </summary>
        public virtual String ReceiveName { get { return _ReceiveName; } set { _ReceiveName = value; } }

        private String _CargoName;
        /// <summary>
        /// 货名
        /// </summary>
        public virtual String CargoName { get { return _CargoName; } set { _CargoName = value; } }

        private String _ApparatusNumber;
        /// <summary>
        /// 衡器编号
        /// </summary>
        public virtual String ApparatusNumber { get { return _ApparatusNumber; } set { _ApparatusNumber = value; } }

        private String _Type;
        /// <summary>
        /// 衡器类型 火车衡 动态衡
        /// </summary>
        public virtual String Type { get { return _Type; } set { _Type = value; } }

        private String _DataFrom;
        /// <summary>
        /// 数据来源
        /// </summary>
        public virtual String DataFrom { get { return _DataFrom; } set { _DataFrom = value; } }
    }
}
