﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Views
{
    /// <summary>
    /// 视图-入厂煤运输记录视图
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("View_BuyFuelTransport")]
    public class View_BuyFuelTransport : EntityBase2
    {
        private String _AutotruckId;
        /// <summary>
        /// 关联：车辆管理
        /// </summary>
        public virtual String AutotruckId { get { return _AutotruckId; } set { _AutotruckId = value; } }

        private String _InFactoryBatchId;
        /// <summary>
        /// 入厂煤批次Id
        /// </summary>
        public virtual String InFactoryBatchId { get { return _InFactoryBatchId; } set { _InFactoryBatchId = value; } }

        private String _SamplingId;
        /// <summary>
        /// 入厂煤采样Id
        /// </summary>
        public virtual String SamplingId { get { return _SamplingId; } set { _SamplingId = value; } }
        private string _SerialNumber;
        /// <summary>
        /// 流水号
        /// </summary>
        public virtual string SerialNumber { get { return _SerialNumber; } set { _SerialNumber = value; } }

        private String _CarNumber;
        /// <summary>
        /// 车号
        /// </summary>
        public virtual String CarNumber { get { return _CarNumber; } set { _CarNumber = value; } }

        private Decimal _GrossWeight;
        /// <summary>
        /// 毛重(吨)
        /// </summary>
        public virtual Decimal GrossWeight { get { return _GrossWeight; } set { _GrossWeight = value; } }

        private Decimal _DeductWeight;
        /// <summary>
        /// 皮重(吨)
        /// </summary>
        public virtual Decimal DeductWeight { get { return _DeductWeight; } set { _DeductWeight = value; } }

        private Decimal _TareWeight;
        /// <summary>
        /// 扣吨(吨)
        /// </summary>
        public virtual Decimal TareWeight { get { return _TareWeight; } set { _TareWeight = value; } }

        private Decimal _SuttleWeight;
        /// <summary>
        /// 净重(吨)
        /// </summary>
        public virtual Decimal SuttleWeight { get { return _SuttleWeight; } set { _SuttleWeight = value; } }

        private Decimal _TicketWeight;
        /// <summary>
        /// 矿发量(吨)
        /// </summary>
        public virtual Decimal TicketWeight { get { return _TicketWeight; } set { _TicketWeight = value; } }

        private DateTime _InFactoryTime;
        /// <summary>
        /// 入厂时间
        /// </summary>
        public virtual DateTime InFactoryTime { get { return _InFactoryTime; } set { _InFactoryTime = value; } }



        private DateTime _GrossTime;
        /// <summary>
        /// 重车时间
        /// </summary>
        public virtual DateTime GrossTime { get { return _GrossTime; } set { _GrossTime = value; } }


        private DateTime _TareTime;
        /// <summary>
        /// 空车时间
        /// </summary>
        public virtual DateTime TareTime { get { return _TareTime; } set { _TareTime = value; } }



        private DateTime _OutFactoryTime;
        /// <summary>
        /// 出厂时间
        /// </summary>
        public virtual DateTime OutFactoryTime { get { return _OutFactoryTime; } set { _OutFactoryTime = value; } }

        private string _SupplierName;
        /// <summary>
        /// 供煤单位
        /// </summary>
        public virtual string SupplierName { get { return _SupplierName; } set { _SupplierName = value; } }

        private string _SupplierId;
        /// <summary>
        /// 供煤单位
        /// </summary>
        public virtual string SupplierId { get { return _SupplierId; } set { _SupplierId = value; } }

        private string _FuelKindName;
        /// <summary>
        /// 煤种
        /// </summary>
        public virtual string FuelKindName { get { return _FuelKindName; } set { _FuelKindName = value; } }

        private string _FuelKindId;
        /// <summary>
        /// 煤种
        /// </summary>
        public virtual string FuelKindId { get { return _FuelKindId; } set { _FuelKindId = value; } }

        private string _MineName;
        /// <summary>
        /// 矿点
        /// </summary>
        public virtual string MineName { get { return _MineName; } set { _MineName = value; } }

        private string _MineId;
        /// <summary>
        /// 矿点
        /// </summary>
        public virtual string MineId { get { return _MineId; } set { _MineId = value; } }

        private string _TransportCompanyName;
        /// <summary>
        /// 运输单位
        /// </summary>
        public virtual string TransportCompanyName { get { return _TransportCompanyName; } set { _TransportCompanyName = value; } }

        private string _TransportCompanyId;
        /// <summary>
        /// 运输单位
        /// </summary>
        public virtual string TransportCompanyId { get { return _TransportCompanyId; } set { _TransportCompanyId = value; } }

        private int _IsUse;
        /// <summary>
        /// 有效
        /// </summary>
        public virtual int IsUse { get { return _IsUse; } set { _IsUse = value; } }

        private int _IsFinish;
        /// <summary>
        /// 已完结
        /// </summary>
        public virtual int IsFinish { get { return _IsFinish; } set { _IsFinish = value; } }

        private string _SamplingType;
        /// <summary>
        /// 采样方式
        /// </summary>
        public virtual string SamplingType { get { return _SamplingType; } set { _SamplingType = value; } }

        private string _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get { return _Remark; } set { _Remark = value; } }

        private string _Batch;
        /// <summary>
        /// 批次编号
        /// </summary>
        public virtual string Batch { get { return _Batch; } set { _Batch = value; } }

        private string _PrevPlace;
        /// <summary>
        /// 上一次地点
        /// </summary>
        public virtual string PrevPlace { get { return _PrevPlace; } set { _PrevPlace = value; } }

        private string _UnFinishTransportId;
        /// <summary>
        /// 未完成运输记录Id
        /// </summary>
        public virtual string UnFinishTransportId { get { return _UnFinishTransportId; } set { _UnFinishTransportId = value; } }

        /// <summary>
        /// 有效2
        /// </summary>
        public virtual bool IsUse2 { get { return this.IsUse == 1; } set { this.IsUse = Convert.ToInt16(value); } }

        private DateTime _SamplingTime;
        /// <summary>
        /// 采样时间
        /// </summary>
        public virtual DateTime SamplingTime { get { return _SamplingTime; } set { _SamplingTime = value; } }

        private Int32 _IsAutoDeduct;
        /// <summary>
        /// 是否自动扣吨
        /// </summary>
        public virtual Int32 IsAutoDeduct { get { return _IsAutoDeduct; } set { _IsAutoDeduct = value; } }

        private string _FpSamplePlace;
        /// <summary>
        /// 采样地点
        /// </summary>
        public virtual string FpSamplePlace { get { return _FpSamplePlace; } set { _FpSamplePlace = value; } }

    }
}
