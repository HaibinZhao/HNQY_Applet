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
    public class ClientConfiguration
    {
        private static Object locker = new Object();
        private static ClientConfiguration instance;

        private static string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ADGS.AppConfig.json");

        public static ClientConfiguration Instance
        {
            get
            {
                if (ClientConfiguration.instance == null)
                {
                    lock (locker)
                    {
                        if (File.Exists(FilePath))
                            ClientConfiguration.instance = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientConfiguration>(File.ReadAllText(FilePath, Encoding.UTF8));
                        else
                        {
                            ClientConfiguration.instance = new ClientConfiguration();
                            ClientConfiguration.instance.Save();
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
            ClientConfiguration.instance = null;
            var configuration = ClientConfiguration.Instance;
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

        public ClientConfiguration()
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

        private string upLoadIdentifier = "Default";
        /// <summary>
        /// 上传标识
        /// </summary>
        public string UpLoadIdentifier
        {
            get { return upLoadIdentifier; }
            set { upLoadIdentifier = value; }
        }

        private string serverIp;
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIp
        {
            get { return serverIp; }
            set { serverIp = value; }
        }

        private int serverPort = 5000;
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        private string connStr = "";
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnStr
        {
            get { return connStr.Replace("{yyyy}", System.DateTime.Now.Year.ToString()); }
            set { connStr = value; }
        }

        private string sQL;
        /// <summary>
        /// 数据查询语句
        /// </summary>
        public string SQL
        {
            get { return sQL; }
            set { sQL = value; }
        }

        private string dbType;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

		private string processName;
		/// <summary>
		/// 进程名称
		/// </summary>
		public string ProcessName
		{
			get { return processName; }
			set { processName = value; }
		}

		private decimal grabInterval = 5;
        /// <summary>
        /// 取数间隔 单位：分钟
        /// </summary>
        public decimal GrabInterval
        {
            get { return grabInterval; }
            set { grabInterval = value; }
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

    }

}
