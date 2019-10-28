using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.DapperDber.Attrs;

namespace CMCS.DumblyConcealer.Tasks.AssayDevice.Entities
{
    /// <summary>
    /// .量热仪 型号：5E-C5500A双控
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("HYTBLRY_5EC5500A")]
    public class LRY_5EC5500A
    {
        [DapperPrimaryKey]
        public string PKID { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 试样自动编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 测试方法：0--常规法;1--快速法;2--普通法
        /// </summary>
        public int Method { get; set; }
        /// <summary>
        /// 试样手工编号
        /// </summary>
        public string Mancoding { get; set; }
        /// <summary>
        /// 实验时间
        /// </summary>
        public DateTime TestTime { get; set; }
        /// <summary>
        /// 恒容干基高位热值
        /// </summary>
        public string Qgrd { get; set; }
        /// <summary>
        /// 恒容空干基高位热值
        /// </summary>
        public string Qgrad { get; set; }
        /// <summary>
        /// 恒容收到基低位热值
        /// </summary>
        public string Qnetar { get; set; }
        /// <summary>
        /// 添加物重量
        /// </summary>
        public string Tianjia { get; set; }
        /// <summary>
        /// 主期开始温度
        /// </summary>
        public string Tmabe { get; set; }
        /// <summary>
        /// 主期结束温度
        /// </summary>
        public string Tmaen { get; set; }
        /// <summary>
        /// 全硫含量(滴定过程中计算出S元素物质的量已经转换成质量百分比)
        /// </summary>
        public string St { get; set; }
        /// <summary>
        /// 空干基水分
        /// </summary>
        public string Mad { get; set; }
        /// <summary>
        /// 收到基水分
        /// </summary>
        public string Mar { get; set; }
        /// <summary>
        /// 空干基氢含量
        /// </summary>
        public string Had { get; set; }
        /// <summary>
        /// 试样名称
        /// </summary>
        public string Mingchen { get; set; }
        /// <summary>
        /// 试样重量
        /// </summary>
        public string Weight { get; set; }
        /// <summary>
        /// 弹筒发热量
        /// </summary>
        public string Qb { get; set; }
        /// <summary>
        /// 化验员
        /// </summary>
        public string Testman { get; set; }
        /// <summary>
        /// 空干基氧含量
        /// </summary>
        public string Oad { get; set; }
        /// <summary>
        /// 空干基氮含量
        /// </summary>
        public string Nad { get; set; }
    }
}
