using System;
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
        public MobileResponse Send(Context ctx, MobileMessage message)
        {
            //注:消息推送实现类主要 实现接口IOtherPlatformMessage中的Send方法

            //变量定义
            var apiurl = "";
            var requestData = new object();

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
            }
            //todo:执行调用“待办”转“已办”泛微接口 done
            else if(message.Status.ToString() == "2")
            {
                //泛微（ProcessDoneRequestByJson）接口-->作用:将待办转为已办
                apiurl = "http://172.16.4.29:8888/rest/ofs/ProcessDoneRequestByJson";

                /*
                    参数说明:(共6个)
                     syscode:异构系统标识
                     flowid:流程实例id(自定义)
                     requestname:标题(自定义)
                     workflowname：流程类型名称(自定义)
                     nodename:步骤名称（节点名称）
                     receiver:接收人(K3用户名称)
                */

                requestData = new
                {
                    syscode = "kingdee",
                    flowid = message.SourceId,
                    requestname = message.Title,
                    workflowname = message.Title,
                    nodename = message.Title + "已办",
                    receiver = "梁嘉杰"    //接收者 message.Users 为消息接收者的金碟云星空用户ID
                };

            }


            var a = JsonConvert.SerializeObject(requestData);

            //todo:执行调用API信息
            // 发送API请求  
            var client = new HttpClient();
            var requestContect = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = client.PostAsync(apiurl, requestContect).Result;

            //处理API响应
            if (response.IsSuccessStatusCode)
            {
                // 根据实际情况解析API响应,构建MobileResponse对象并返回  
                var responseContect = response.Content.ReadAsStringAsync().Result;
                var responseData = JsonConvert.DeserializeObject<MobileResponse>(responseContect);
                return responseData;
            }
            else
            {
                //Console.WriteLine("消息发送失败: " + response.StatusCode);
                return null;
            }
        }
    }
}
