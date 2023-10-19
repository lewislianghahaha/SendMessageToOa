using System;
using System.Data;

namespace SendMessageToOa
{
    //临时表
    public class Tempdt
    {
        /// <summary>
        /// 将信息插入至T_BD_InsertSourceid表内--“待办”流程时记录当前用户ID 及 Sourceid
        /// </summary>
        /// <returns></returns>
        public DataTable UpdateDtTemp()
        {
            var dt = new DataTable();
            for (var i = 0; i < 4; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //待办任务ID
                    case 0:
                        dc.ColumnName = "Sourceid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //用户ID
                    case 1:
                        dc.ColumnName = "Userid";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //用户名称
                    case 2:
                        dc.ColumnName = "UserName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //插入时间
                    case 3:
                        dc.ColumnName = "InsertDt";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }
    }
}
