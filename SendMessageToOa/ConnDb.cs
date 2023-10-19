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
        /// 0:连接K3 1:连接OA
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            var strcon = @"Data Source='192.168.1.228';Initial Catalog='AIS20230809113406';Persist Security Info=True;User ID='sa'; Password='kingdee';
                       Pooling=true;Max Pool Size=40000;Min Pool Size=0";

            return strcon;
        }
    }
}
