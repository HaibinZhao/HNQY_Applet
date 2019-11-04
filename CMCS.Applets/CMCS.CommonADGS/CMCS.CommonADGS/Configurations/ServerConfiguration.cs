using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CMCS.CommonADGS.Configurations
{
    /// <summary>
    /// 服务端配置类
    /// </summary>
    public class ServerConfiguration
    {
        private static Object locker = new Object();
        private static ServerConfiguration instance;

        private static string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Server.AppConfig.json");

        public static ServerConfiguration Instance
        {
            get
            {
                if (ServerConfiguration.instance == null)
                {
                    lock (locker)
                    {
                        if (File.Exists(FilePath))
                            ServerConfiguration.instance = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfiguration>(File.ReadAllText(FilePath, Encoding.UTF8));
                        else
                        {
                            ServerConfiguration.instance = new ServerConfiguration();
                            ServerConfiguration.instance.Save();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            ServerConfiguration.instance = null;
            var configuration = ServerConfiguration.Instance;
        }

        /// <summary>
        /// 存储
        /// </summary>
        public void Save()
        {
            try
            {
                File.WriteAllText(FilePath, Newtonsoft.Json.JsonConvert.SerializeObject(this), Encoding.UTF8);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ServerConfiguration()
        { }

        //*************************************配置项******************************************//

        private string appIdentifier = "Default";
        /// <summary>
        /// 程序唯一标识
        /// </summary>
        public string AppIdentifier
        {
            get { return appIdentifier; }
            set { appIdentifier = value; }
        }

        private int port = 5000;
        /// <summary>
        /// 程序唯一标识
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private string selfConnStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=bieos_ynxx;Password=1234;";
        /// <summary>
        /// Oracle数据库连接字符串
        /// </summary>
        public string SelfConnStr
        {
            get { return selfConnStr; }
            set { selfConnStr = value; }
        }

        private string oracleKeywords;
        /// <summary>
        /// Oracle关键字,多个使用“|”分割
        /// </summary>
        public string OracleKeywords
        {
            get { return oracleKeywords; }
            set { oracleKeywords = value; }
        }

        private bool startup;
        /// <summary>
        /// 开机启动
        /// </summary>
        public bool Startup
        {
            get { return startup; }
            set { startup = value; }
        }

        private bool isSeccetRunning;
        /// <summary>
        /// 是否最小化运行
        /// </summary>
        public bool IsSeccetRunning
        {
            get { return isSeccetRunning; }
            set { isSeccetRunning = value; }
        }

        private bool verifyBeforeClose;
        /// <summary>
        /// 是否关闭验证
        /// </summary>
        public bool VerifyBeforeClose
        {
            get { return verifyBeforeClose; }
            set { verifyBeforeClose = value; }
        }

        private List<MachineCodes> details = new List<MachineCodes>();
        /// <summary>
        /// 明细
        /// </summary>
        public List<MachineCodes> Details
        {
            get { return details; }
            set { details = value; }
        }

    }

    public class MachineCodes
    {
        private string upLoadIdentifer = "";
        /// <summary>
        /// 标识
        /// </summary>
        public string UpLoadIdentifer
        {
            get { return upLoadIdentifer; }
            set { upLoadIdentifer = value; }
        }

        private string dataTableName = "";
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string DataTableName
        {
            get { return dataTableName; }
            set { dataTableName = value; }
        }

        private string primarKeys = "";
        /// <summary>
        /// 主键
        /// </summary>
        public string PrimarKeys
        {
            get { return primarKeys; }
            set { primarKeys = value; }
        }

    }
}
