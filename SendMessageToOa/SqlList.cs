namespace SendMessageToOa
{
    //执行数据SQL
    public class SqlList
    {
        private string _result;


        public string SearchUserRecord(string userid)
        {
            _result = @"
                            SELECT * FROM dbo.V_USERS A
WHERE A.FUSERID='100006' 
                       ";


            return _result;
        }
    }
}
