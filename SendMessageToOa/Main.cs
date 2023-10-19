using System;
using System.Data;
using System.Net.Http;
using System.Text;
using Kingdee.BOS;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Mobile.Contract;
using Kingdee.BOS.Mobile.DataEnties;
using Newtonsoft.Json;

namespace SendMessageToOa
{
    public class Main : IOtherPlatformMessage
    {
        Tempdt tempdt = new Tempdt();
        InsertDt insertDt = new InsertDt();
        SearchDt searchDt = new SearchDt();

        public MobileResponse Send(Context ctx, MobileMessage message)
        {
            //注:消息推送实现类主要 实现接口IOtherPlatformMessage中的Send方法
            //变量定义
            var apiurl = "";
            var requestData = new object();
            var loopid = 0;
            //定义返回对像(初始化)
            MobileResponse responseData = null;

            //todo:通过message.Status判断当前流程是“待办” 或 “已办”
            var mes = message.Status;
            var a1 = message.SourceId;
            var a2 = message.Title;
            var a3 = ctx.UserName;
            var a4 = message.Users;

            //todo:执行调用生成“待办”泛微接口 TODO
            if (message.Status.ToString() == "1")
            {
                // 构造API请求参数  
                //泛微（ReceiveRequestInfoByJson）接口-->接收流程数据(Json格式):  可接收待办、已办、归档的流程数据--(no need)
                //泛微（ReceiveTodoRequestByJson）接口-->接收待办数据(Json格式)： 只接收待办流程数据
                apiurl = "http://172.16.4.29:8888/rest/ofs/ReceiveTodoRequestByJson";

                #region 参数说明:(共15个)
                /*
                 参数说明:(共15个)
                 syscode:异构系统标识
                 flowid:流程实例id(自定义)
                 requestname:标题(自定义)
                 workflowname：流程类型名称(自定义)
                 nodename:步骤名称（节点名称）
                 pcurl:PC 中转地址(第三⽅系统中流程处理界⾯的PC端地址)
                 appurl:APP 中转地址(第三⽅系统中流程处理界⾯的移动端地址)
                 creator:创建人(K3用户名称)
                 createdatetime:创建日期时间
                 receiver:接收人(K3用户名称)
                 receivedatetime:接收日期时间

                 isremark:流程处理状态 0：待办 2：已办 4：办结 8：抄送（待阅）--(注:ReceiveTodoRequestByJson 接口不需要)
                 viewtype:流程查看状态 0：未读 （注：isremark = 0 且 viewtype = 0 会给OA消息中心推送消息，对接钉钉、企业微信）1：已读; --(注:ReceiveTodoRequestByJson 接口不需要)
                 receivets:时间戳字段，客户端使用线程调用接口的时候，根据此字段判断是否需要更新数据，防止后发的请求数据被之前的覆盖例 (可设置固定值:1602817491990) --(注:ReceiveTodoRequestByJson 接口不需要)
                 
                 value:自定义参数-用于保存message.SourceId （目的:中转页-跳转至K3业务待办审批列表-单据明细使用）
                 */
                #endregion

                requestData = new
                {
                    syscode = "kingdee",
                    flowid = message.SourceId,
                    requestname = message.Title,
                    workflowname = message.Title,
                    nodename = message.Title+"待办",
                    pcurl = $"/interface/K3CloudEntranceV5.jsp",
                    appurl = $"/interface/K3CloudEntranceV5.jsp",
                    creator = "梁嘉杰",   //ctx.UserName 创建者名称
                    createdatetime = DateTime.Now.ToString(),
                    receiver = "梁嘉杰",  //message.Users 为消息接收者的金碟云星空用户ID
                    receivedatetime = DateTime.Now.ToString(),
                    //isremark = "0",
                    //viewtype = "0",
                    //receivets = "1602817491990",
                    custom = new JSONObject
                    {
                        {"value",$"{message.SourceId}"}   //若需要多参数,需要用","分隔
                    }
                };

                //todo:整理相关信息并插入至T_BD_InsertSourceid表内 message.Users
                var b3a = message.Users[0];

                InsertDtToK3Table(message.SourceId,message.Users[0],ctx.UserName,message.Title);

                //todo:调用泛微API
                var response = SendApi(apiurl, requestData);

                //处理API响应
                if (response.IsSuccessStatusCode)
                {
                    // 根据实际情况解析API响应,构建MobileResponse对象并返回  
                    var responseContect = response.Content.ReadAsStringAsync().Result;
                    responseData = JsonConvert.DeserializeObject<MobileResponse>(responseContect);
                }
            }
            //todo:执行调用“待办”转“已办”泛微接口 done
            else if(message.Status.ToString() == "2")
            {
                var responseContect = "[";

                var resu = message.SourceId;
                var res1 = message.Users;

                //泛微（ProcessDoneRequestByJson）接口-->作用:将待办转为已办
                apiurl = "http://172.16.4.29:8888/rest/ofs/ProcessDoneRequestByJson";

                //todo:根据message.SourceId获取T_BD_InsertSourceid表中对应的UserName(接收者名称),并作为接口的receiver(接收者)参数,循环调用泛微接口
                var searchdt = SearchReceiveUserName(message.SourceId);

                if (searchdt.Rows.Count > 0)
                {
                    foreach (DataRow rows in searchdt.Rows)
                    {


                        #region 参数说明:(共6个)
                        /*
                            参数说明:(共6个)
                             syscode:异构系统标识
                             flowid:流程实例id(自定义)
                             requestname:标题(自定义)
                             workflowname：流程类型名称(自定义)
                             nodename:步骤名称（节点名称）
                             receiver:接收人(K3用户名称)
                        */
                        #endregion

                        requestData = new
                        {
                            syscode = "kingdee",
                            flowid = message.SourceId,
                            requestname = message.Title,
                            workflowname = message.Title,
                            nodename = message.Title + "已办",
                            receiver = Convert.ToString(rows[0])    //接收者 message.Users 为消息接收者的金碟云星空用户ID
                        };

                        var response = SendApi(apiurl, requestData);



                        //处理API响应--循环收集成功与失败的信息
                        if (response.IsSuccessStatusCode)
                        {
                            // 根据实际情况解析API响应,构建MobileResponse对象并返回  
                            responseContect += response.Content.ReadAsStringAsync().Result;

                        }
                        //TODO:当response返回失败,结合返回信息
                        else
                        {
                            
                        }

                        loopid++;
                    }

                    //todo:结束时添加]
                    responseContect += "]";

                    responseData = JsonConvert.DeserializeObject<MobileResponse>(responseContect);
                }
            }

            return responseData;
        }

        /// <summary>
        /// 根据apiurl 及 requestData(参数) 传输到泛微API并返回执行结果
        /// </summary>
        /// <param name="apiurl"></param>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private HttpResponseMessage SendApi(string apiurl,object requestData)
        {
            var a = JsonConvert.SerializeObject(requestData);

            //todo:执行调用API信息
            // 发送API请求  
            var client = new HttpClient();
            var requestContect = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            //利用API及参数发送到泛微
            var response = client.PostAsync(apiurl, requestContect).Result;

            return response;
        }

        /// <summary>
        /// 根据sourceid放到T_BD_InsertSourceid表进行查找,返回DT
        /// </summary>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        private DataTable SearchReceiveUserName(string sourceid)
        {
            var dt = searchDt.SearchRecord(sourceid).Copy();
            return dt;
        }

        /// <summary>
        /// 将相关记录插入至临时表,并最后将相关记录插入至T_BD_InsertSourceid表内
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="userid"></param>
        /// <param name="createname"></param>
        /// <param name="projecttitle"></param>
        private void InsertDtToK3Table(string sourceid,string userid,string createname,string projecttitle)
        {
            //定义用户名称
            var username = "";

            //todo:根据userid获取到对应的username
            var userdt = searchDt.SearchUserRecord(userid).Copy();
            username = userdt.Rows.Count == 0 ? "" : Convert.ToString(userdt.Rows[0][0]);

            //todo:整合相关数据至insertdt临时表(插入前准备)
            var insertdt = tempdt.InsertDtTemp();

            var newrow = insertdt.NewRow();
            newrow[0] = sourceid;                //待办任务ID
            newrow[1] = userid;                  //用户ID--接收者ID
            newrow[2] = username;                //用户名称--接收者名称
            newrow[3] = createname;              //创建者名称
            newrow[4] = projecttitle;            //待办任务标题
            newrow[5] = DateTime.Now.ToString(); //插入时间

            insertdt.Rows.Add(newrow);

            //todo:将insertdt插入至T_BD_InsertSourceid表
            insertDt.ImportDtToDb("T_BD_InsertSourceid", insertdt);
        }

    }
}
