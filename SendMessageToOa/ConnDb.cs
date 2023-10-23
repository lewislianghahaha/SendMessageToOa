using System.Data.SqlClient;

namespace SendMessageToOa
{
    public class ConnDb
    {
        /// <summary>
        /// 获取连接返回信息
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetK3CloudConn()
        {
            var sqlcon = new SqlConnection(GetConnectionString());
            return sqlcon;
        }

        /// <summary>
        /// AIS20231023091946--测试账
        /// AIS20181204095717--正式账
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            var strcon = @"Data Source='192.168.1.228';Initial Catalog='AIS20231023091946';Persist Security Info=True;User ID='sa'; Password='kingdee';
                       Pooling=true;Max Pool Size=40000;Min Pool Size=0";

            return strcon;
        }
    }
}
