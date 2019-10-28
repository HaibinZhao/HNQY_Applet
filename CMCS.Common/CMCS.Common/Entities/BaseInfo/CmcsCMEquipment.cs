﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;
using CMCS.Common.DAO;

namespace CMCS.Common.Entities.BaseInfo
{
    /// <summary>
    /// 集中管控设备
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("cmcstbcmequipment")]
    public class CmcsCMEquipment : EntityBase1
    {
        private string interfaceType;
        /// <summary>
        /// 接口类型
        /// </summary>
        public string InterfaceType
        {
            get { return interfaceType; }
            set { interfaceType = value; }
        }

        private string _EquipmentName;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName
        {
            get { return _EquipmentName; }
            set { _EquipmentName = value; }
        }

        private string _EquipmentCode;
        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentCode
        {
            get { return _EquipmentCode; }
            set { _EquipmentCode = value; }
        }

        private string _SampleMachine;
        /// <summary>
        /// 厂家名称
        /// </summary>
        public string SampleMachine
        {
            get { return _SampleMachine; }
            set { _SampleMachine = value; }
        }

        private string _Sequence;
        /// <summary>
        /// 顺序
        /// </summary>
        public string Sequence
        {
            get { return _Sequence; }
            set { _Sequence = value; }
        }

        private string _Parentid;
        /// <summary>
        /// 父节点
        /// </summary>
        public string Parentid
        {
            get { return _Parentid; }
            set { _Parentid = value; }
        }

        /// <summary>
        /// 上级设备
        /// </summary>
        [CMCS.DapperDber.Attrs.DapperIgnore]
        public CmcsCMEquipment TheParentEquipment
        {
            get { return CommonDAO.GetInstance().SelfDber.Get<CmcsCMEquipment>(this.Parentid); }
        }
    }
}
