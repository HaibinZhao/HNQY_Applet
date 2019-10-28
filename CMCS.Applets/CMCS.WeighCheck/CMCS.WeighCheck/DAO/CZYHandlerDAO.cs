using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CMCS.Common;
using CMCS.Common.Entities;
using CMCS.DapperDber.Util;
using CMCS.Common.Enums;
using CMCS.Common.Entities.Fuel;

namespace CMCS.WeighCheck.DAO
{
    public class CZYHandlerDAO
    {
        private static CZYHandlerDAO instance;

        public static CZYHandlerDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new CZYHandlerDAO();
            }

            return instance;
        }

        private CZYHandlerDAO()
        { }

        #region 获取配置信息

        #endregion

        #region 采样后样桶称重登记
        /// <summary>
        /// 查找样桶登记记录
        /// </summary>
        /// <param name="BarrelCode">样桶编码</param>
        public CmcsRCSampleBarrel GetRCSampleBarrel(string BarrelCode, out string message)
        {
            message = string.Empty;
            CmcsRCSampleBarrel entity = Dbers.GetInstance().SelfDber.Entity<CmcsRCSampleBarrel>(" where BarrelCode='" + BarrelCode + "' order by BarrellingTime desc");
            if (entity == null)
                message = "未找到编码【" + BarrelCode + "】的样桶登记记录";
            return entity;
        }

        /// <summary>
        /// 保存样桶登记记录(人工样 采样第一次称重)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveRCSampleBarrel(CmcsRCSampleBarrel entity)
        {
            return Dbers.GetInstance().SelfDber.Insert<CmcsRCSampleBarrel>(entity) > 0 ? true : false;
        }

        /// <summary>
        /// 记录样桶校验记录(机器样 采样第一次称重)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateRCSampleBarrelSampleWeight(string rCSampleBarrelId, double weight)
        {
            return Dbers.GetInstance().SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsRCSampleBarrel>() + " set SampleWeight=:SampleWeight where Id=:Id", new { SampleWeight = weight, Id = rCSampleBarrelId }) > 0;
        }

        /// <summary>
        /// 获取采样单信息
        /// </summary>
        /// <param name="dtStart">开始时间</param>
        /// <param name="dtEnd">结束时间</param>
        /// <returns></returns>
        public DataTable GetSampleInfo(DateTime dtStart, DateTime dtEnd, string sampleCode = "")
        {
            string sql = string.Format(@" select a.batch,a.id as batchid,
                                 b.name as suppliername,
                                 c.name as minename,
                                 d.fuelname as kindname,
                                 e.name as stationname,
                                 a.factarrivedate,
                                 t.id,
                                 t.samplecode,
                                 t.samplingdate,
                                 t.samplingtype
                            from cmcstbrcsampling t
                            left join fultbinfactorybatch a on t.infactorybatchid = a.id
                            left join fultbsupplier b on a.supplierid = b.id
                            left join fultbmine c on a.mineid = c.id
                            left join fultbfuelkind d on a.fuelkindid = d.id
                            left join fultbstationinfo e on a.stationid = e.id
                       where (a.id is not null) and t.samplingdate >= '" + dtStart + "' and t.samplingdate < '" + dtEnd + "' {0}", string.IsNullOrEmpty(sampleCode) ? "" : "and t.samplecode='" + sampleCode + "'");
            return Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
        }

        #endregion

        #region 制样前样桶称重校验
        /// <summary>
        /// 根据入厂煤采样单Id获取制样记录
        /// </summary>
        /// <param name="sampleId">采样单Id</param>
        /// <returns></returns>
        public CmcsRCMake GetRCMakeBySampleId(string sampleId)
        {
            return Dbers.GetInstance().SelfDber.Entity<CmcsRCMake>("where SamplingId=:SamplingId", new { SamplingId = sampleId });
        }

        /// <summary>
        /// 查找同一采样单的样桶登记记录
        /// </summary>
        /// <param name="BarrelCode"></param>
        /// <returns></returns>
        public List<CmcsRCSampleBarrel> GetRCSampleBarrels(string BarrelCode, out string message)
        {
            message = string.Empty;
            List<CmcsRCSampleBarrel> list = new List<CmcsRCSampleBarrel>();
            CmcsRCSampleBarrel entity = GetRCSampleBarrel(BarrelCode, out message);
            if (entity != null)
            {
                list = Dbers.GetInstance().SelfDber.Entities<CmcsRCSampleBarrel>(" where SamplingId='" + entity.SamplingId + "'");
                message = "扫码成功，该批次采样桶共" + list.Count + "桶";
            }
            return list;
        }

        /// <summary>
        /// 记录样桶校验记录(采样第二次称重)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateRCSampleBarrelCheckSampleWeight(string rCSampleBarrelId, double weight)
        {
            return Dbers.GetInstance().SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsRCSampleBarrel>() + " set CheckSampleWeight=:CheckSampleWeight where Id=:Id", new { CheckSampleWeight = weight, Id = rCSampleBarrelId }) > 0;
        }
        #endregion

        #region 制样后样品称重登记
        /// <summary>
        /// 根据制样码获取制样主表信息
        /// </summary>
        /// <param name="makeCode">制样码</param>
        /// <returns></returns>
        public CmcsRCMake GetRCMake(string makeCode)
        {
            return Dbers.GetInstance().SelfDber.Entity<CmcsRCMake>(" where MakeCode=:MakeCode order by CreateDate desc", new { MakeCode = makeCode });
        }

        /// <summary>
        /// 根据制样码获取制样从表明细集合
        /// </summary>
        /// <param name="makeCode">制样码</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<CmcsRCMakeDetail> GetRCMakeDetails(string makeCode, out string message)
        {
            message = "";

            List<CmcsRCMakeDetail> list = new List<CmcsRCMakeDetail>();
            CmcsRCMake rcmake = GetRCMake(makeCode);
            if (rcmake != null)
            {
                list = Dbers.GetInstance().SelfDber.Entities<CmcsRCMakeDetail>(" where MakeId=:MakeId order by AutoId asc", new { MakeId = rcmake.Id });
                if (list.Count == 0) message = "未找到制样明细记录";
            }
            else
                message = "未找到制样记录，制样码：" + makeCode;

            return list;
        }

        /// <summary> 
        /// 更新制样明细记录的样重和样罐编码
        /// </summary>
        /// <param name="rCMakeDetailId">制样明细记录Id</param>
        /// <param name="weight">重量</param>
        /// <param name="barrelCode">样罐编码</param>
        /// <returns></returns>
        public bool UpdateMakeDetailWeightAndBarrelCode(string rCMakeDetailId, double weight, string barrelCode)
        {
            return Dbers.GetInstance().SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsRCMakeDetail>() + " set Weight=:Weight,BarrelCode=:BarrelCode where Id=:Id", new { Id = rCMakeDetailId, Weight = weight, BarrelCode = barrelCode }) > 0;
        }
        #endregion

        #region 化验前样品称重校验
        /// <summary>
        /// 获取制样从表明细记录
        /// </summary>
        /// <param name="MakeCode">制样码</param>
        /// <returns></returns>
        public CmcsRCMakeDetail GetRCMakeDetail(string barrelCode, out string message)
        {
            CmcsRCMakeDetail rcMakeDetail = null;
            try
            {
                rcMakeDetail = Dbers.GetInstance().SelfDber.Entity<CmcsRCMakeDetail>(" where BarrelCode=:BarrelCode order by CreateDate desc", new { BarrelCode = barrelCode });
                if (rcMakeDetail == null)
                {
                    message = "未找到【" + barrelCode + "】制样登记记录";
                    return null;
                }
            }
            catch (Exception ex)
            {
                message = "程序异常！" + ex.Message;
                return null;
            }
            message = "扫码成功";
            return rcMakeDetail;
        }

        /// <summary>
        /// 根据制样明细Id查找化验记录
        /// </summary>
        /// <param name="rCMakeDetailId">制样明细Id</param>
        /// <returns></returns>
        public CmcsRCAssay GetRCAssayByMakeCode(string rCMakeDetailId)
        {
            CmcsRCMakeDetail rCMakeDetail = Dbers.GetInstance().SelfDber.Get<CmcsRCMakeDetail>(rCMakeDetailId);
            if (rCMakeDetail != null)
            {
                // 三级编码化验查询
                if (rCMakeDetail.SampleType == eMakeSampleType.Type1 || rCMakeDetail.SampleType == eMakeSampleType.Type5 || rCMakeDetail.SampleType == eMakeSampleType.Type6 || rCMakeDetail.SampleType == eMakeSampleType.Type3)
                    return Dbers.GetInstance().SelfDber.Entity<CmcsRCAssay>("where AssayType=:AssayType and MakeId=:MakeId order by CreateDate desc", new { AssayType = eAssayType.三级编码化验.ToString(), MakeId = rCMakeDetail.MakeId });
                // 不同类型的化验查询
                //else if(rCMakeDetail.SampleType==eMakeSampleType.Type2)
            }

            return null;
        }

        /// <summary> 
        /// 更新制样明细记录的校验样重
        /// </summary>
        /// <param name="rCMakeDetailId">制样明细记录Id</param>
        /// <param name="weight">重量</param>
        /// <returns></returns>
        public bool UpdateMakeDetailCheckWeight(string rCMakeDetailId, double weight)
        {
            return Dbers.GetInstance().SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsRCMakeDetail>() + " set CheckWeight=:CheckWeight where Id=:Id", new { Id = rCMakeDetailId, CheckWeight = weight }) > 0;
        }
        #endregion
    }
}
