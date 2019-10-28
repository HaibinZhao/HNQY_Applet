using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.DAO;
using CMCS.Common.Entities.CarTransport;
using CMCS.DapperDber.Dbs.OracleDb;
using CMCS.Common;
using CMCS.Common.Entities;
using CMCS.Common.Views;
using CMCS.DapperDber.Util;
using CMCS.Common.Enums;

namespace CMCS.CarTransport.DAO
{
    /// <summary>
    /// 汽车煤仓业务
    /// </summary>
    public class OrderDAO
    {
        private static OrderDAO instance;

        public static OrderDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new OrderDAO();
            }

            return instance;
        }

        private OrderDAO()
        { }

        public OracleDapperDber SelfDber
        {
            get { return Dbers.GetInstance().SelfDber; }
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();
        CarTransportDAO carTransportDAO = CarTransportDAO.GetInstance();

    }
}
