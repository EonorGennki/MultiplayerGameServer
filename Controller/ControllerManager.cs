using MultiplayerGameServer.Servers;
using SocketGameProtocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.Controller
{
    internal class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controlDic = new Dictionary<RequestCode, BaseController>();
        private Server server;
        
        public ControllerManager()
        {
            UserController userController = new UserController();
            controlDic.Add(userController.GetRequestCode, userController);
        }

        /// <summary>
        /// 根据请求码和行为码，调用对应的controller方法
        /// </summary>
        /// <param name="pack">消息包</param>
        /// <param name="client">发送消息的客户端对象</param>
        public void HandleRequest(MainPack pack, Client client)
        {
            if (controlDic.TryGetValue(pack.RequestCode, out BaseController controller))
            {
                string methodName = pack.ActionCode.ToString();
                MethodInfo? method = controller.GetType().GetMethod(methodName);
                if (method is null)
                {
                    Console.WriteLine("未找到指定方法：" + pack.ActionCode.ToString());
                    return;
                }

                object[] obj = new object[] { server, client, pack };
                object ret = method.Invoke(controller, obj);
                if (ret is not null)
                {
                    
                }
            }
            else
            {
                Console.WriteLine("未找到指定controller："+ pack.RequestCode.ToString());
            }
        }
    }
}
