namespace SendMessageToOa
{
    //执行数据SQL
    public class SqlList
    {
        private string _result;

        /// <summary>
        /// 根据userid(用户ID）获取对应的用户名称信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string SearchUserRecord(string userid)
        {
            _result = $@"
                            SELECT DISTINCT A.FNAME 
                            FROM dbo.V_USERS A
                            WHERE A.FUSERID='{userid}' 
                       ";

            return _result;
        }

        /// <summary>
        /// 根据sourceid,获取相关UserName的值
        /// </summary>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public string SearchRecord(string sourceid)
        {
            _result = $@"
                           SELECT DISTINCT a.UserName 
                            FROM T_BD_InsertSourceid A
                            WHERE A.Sourceid='{sourceid}'
                            AND A.UserId is not null
                        ";

            return _result;
        }

    }
}
