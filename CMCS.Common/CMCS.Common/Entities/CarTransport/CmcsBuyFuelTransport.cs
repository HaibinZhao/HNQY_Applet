// 此代码由 NhGenerator v1.0.9.0 工具生成。

using System;
using System.Collections;
using CMCS.Common.DAO;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.CarTransport
{
    /// <summary>
    /// 汽车智能化-入厂煤运输记录
    /// </summary>
    [Serializable]
    [CMCS.DapperDber.Attrs.DapperBind("CmcsTbBuyFuelTransport")]
    public class CmcsBuyFuelTransport : EntityBase1
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

        /// <summary>
        /// 入厂煤批次
        /// </summary>
        [CMCS.DapperDber.Attrs.DapperIgnore]
        public virtual CmcsInFactoryBatch TheInFactoryBatch
        {
            get { return CommonDAO.GetInstance().SelfDber.Get<CmcsInFactoryBatch>(this.InFactoryBatchId); }
        }

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
        /// 扣吨(吨)
        /// </summary>
        public virtual Decimal DeductWeight { get { return _DeductWeight; } set { _DeductWeight = value; } }

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

        private Decimal _CheckWeight;
        /// <summary>
        /// 验收量(吨)
        /// </summary>
        public virtual Decimal CheckWeight { get { return _CheckWeight; } set { _CheckWeight = value; } }

        private Decimal _ProfitWeight;
        /// <summary>
        /// 盈亏量(吨)
        /// </summary>
        public virtual Decimal ProfitWeight { get { return _ProfitWeight; } set { _ProfitWeight = value; } }

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
        /// 毛重时间
        /// </summary>
        public virtual DateTime GrossTime { get { return _GrossTime; } set { _GrossTime = value; } }

        private DateTime _TareTime;
        /// <summary>
        /// 皮重时间
        /// </summary>
        public virtual DateTime TareTime { get { return _TareTime; } set { _TareTime = value; } }

        private DateTime _SamplingTime;
        /// <summary>
        /// 采样时间
        /// </summary>
        public virtual DateTime SamplingTime { get { return _SamplingTime; } set { _SamplingTime = value; } }

        private DateTime _UploadTime;
        /// <summary>
        /// 卸煤时间
        /// </summary>
        public virtual DateTime UploadTime { get { return _UploadTime; } set { _UploadTime = value; } }

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

        private Decimal _IsUse;
        /// <summary>
        /// 有效
        /// </summary>
        public virtual Decimal IsUse { get { return _IsUse; } set { _IsUse = value; } }

        private Decimal _IsFinish;
        /// <summary>
        /// 已完结
        /// </summary>
        public virtual Decimal IsFinish { get { return _IsFinish; } set { _IsFinish = value; } }

        private string _GrossPlace;
        /// <summary>
        /// 毛重地点
        /// </summary>
        public virtual string GrossPlace { get { return _GrossPlace; } set { _GrossPlace = value; } }

        private string _TarePlace;
        /// <summary>
        /// 皮重地点
        /// </summary>
        public virtual string TarePlace { get { return _TarePlace; } set { _TarePlace = value; } }

        private string _SamplePlace;
        /// <summary>
        /// 采样地点
        /// </summary>
        public virtual string SamplePlace { get { return _SamplePlace; } set { _SamplePlace = value; } }

        private string _FPSamplePlace;
        /// <summary>
        /// 分配的采样地点
        /// </summary>
        public virtual string FPSamplePlace { get { return _FPSamplePlace; } set { _FPSamplePlace = value; } }

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

        private string _StepName;
        /// <summary>
        /// 所处流程的步骤  入厂、重车、采样、轻车、出厂、成品仓
        /// </summary>
        public virtual string StepName { get { return _StepName; } set { _StepName = value; } }

        private Int32 _IsNormal;
        /// <summary>
        /// 车辆是否正常 正常的车辆才允许回皮
        /// </summary>
        public virtual Int32 IsNormal { get { return _IsNormal; } set { _IsNormal = value; } }

        private Int32 _IsAutoDeduct;
        /// <summary>
        /// 盈亏是否自动扣吨
        /// </summary>
        public virtual Int32 IsAutoDeduct { get { return _IsAutoDeduct; } set { _IsAutoDeduct = value; } }

        public virtual String UnLoadArea { get; set; }

        private Decimal _IsPrint;
        /// <summary>
        /// 是否已打印
        /// </summary>
        public virtual Decimal IsPrint { get { return _IsPrint; } set { _IsPrint = value; } }

        private string _BARREL;
        /// <summary>
        /// 采样所在采样桶编号
        /// </summary>
        public virtual string BARREL { get { return _BARREL; } set { _BARREL = value; } }

        private Decimal _SAMPLEWEIGHT;
        /// <summary>
        /// 当前车采样量
        /// </summary>
        public virtual Decimal SAMPLEWEIGHT { get { return _SAMPLEWEIGHT; } set { _SAMPLEWEIGHT = value; } }

        private string _ImgUrl;
        /// <summary>
        /// 采样图片路径
        /// </summary>
        public virtual string ImgUrl { get { return _ImgUrl; } set { _ImgUrl = value; } }

        private Object _SAMPLEIMAGE;
        /// <summary>
        /// 采样图片
        /// </summary>
        public virtual Object SAMPLEIMAGE { get { return _SAMPLEIMAGE; } set { _SAMPLEIMAGE = value; } }

        private string _LMYBId;
        /// <summary>
        /// 来煤预报ID
        /// </summary>
        public virtual string LMYBId { get { return _LMYBId; } set { _LMYBId = value; } }

        private string _YbNum;
        /// <summary>
        /// 来煤预报编号
        /// </summary>
        public virtual string YbNum { get { return _YbNum; } set { _YbNum = value; } }

    }
}
