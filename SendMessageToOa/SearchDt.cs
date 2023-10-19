using System;
using System.Data;
using System.Data.SqlClient;

namespace SendMessageToOa
{
    public class SearchDt
    {
        ConnDb conDb = new ConnDb();
        SqlList sqlList = new SqlList();

        /// <summary>
        /// 根据SQL语句查询得出对应的DT
        /// </summary>
        /// <param name="sqlscript">各SQL语句</param>
        /// <returns></returns>
        private DataTable UseSqlSearchIntoDt(string sqlscript)
        {
            var resultdt = new DataTable();

            try
            {
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, conDb.GetK3CloudConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }

            return resultdt;
        }

        /// <summary>
        /// 根据userid(用户ID）获取对应的用户名称信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataTable SearchUserRecord(string userid)
        {
            var dt = UseSqlSearchIntoDt(sqlList.SearchUserRecord(userid));
            return dt;
        }

        /// <summary>
        /// 根据sourceid,获取相关UserName的值
        /// </summary>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public DataTable SearchRecord(string sourceid)
        {
            var dt = UseSqlSearchIntoDt(sqlList.SearchRecord(sourceid));
            return dt;
        }



    }
}
