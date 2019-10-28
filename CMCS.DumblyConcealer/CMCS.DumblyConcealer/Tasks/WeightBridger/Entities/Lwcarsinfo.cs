using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.WeightBridger.Entities
{
    /// <summary>
    /// 轨道衡数据库的过衡数据表 -Lwtrainsinfo
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("Lwcarsinfo")]
    public class Lwcarsinfo : EntityBase2
    {
        public string ID { get; set; }

        public int Number { get; set; }

        public int SuDu { get; set; }

        public string CheXing { get; set; }

        public string CheHao { get; set; }

        public decimal ZiZhong { get; set; }

        public decimal YunXuZaiZhong { get; set; }

        public decimal PiaoZhong { get; set; }

        public decimal MaoZhong { get; set; }

        public decimal JingZhong { get; set; }

        public decimal LuSun { get; set; }

        public decimal YingKui { get; set; }

        public decimal HanShui { get; set; }

        public string FaHuoDanWei { get; set; }

        public string ShowHuoDanWei { get; set; }

        public string FaZhan { get; set; }

        public string DaoZhan { get; set; }

        public string HuoMing { get; set; }

        public int HuiPiTiShi { get; set; }

        public decimal HuiPiZhongLiang { get; set; }

        public int BiaoJi { get; set; }

        public int BuLuWanCheng { get; set; }

        public string JianJinDanNumber { get; set; }

        public string JiLiangYuan { get; set; }

        public DateTime TrainsDate { get; set; }

        public string TrainsTime { get; set; }

        public DateTime LastModify { get; set; }

        public decimal PianZhong { get; set; }

        public int PianZai { get; set; }

        public decimal PiZhong { get; set; }

        public decimal BiaoJingZhong { get; set; }
    }
}
