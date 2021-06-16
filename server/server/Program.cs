
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System;
namespace myServer
{
    class Program
    {
        static List<ForClient> clientlist = new List<ForClient>();
        static void Main(string[] args)
        {

            Socket TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            String ip = GetLocalIPv4();
            if (ip!=null) {
                Console.WriteLine("服务器开启，本机IP： "+ip+"\n"+"请告知客户端。");
            }
            TcpServer.Bind(new IPEndPoint(IPAddress.Parse(ip), 7788));
            TcpServer.Listen(100);
            

            while (true)
            {
                Socket clientSocket = TcpServer.Accept();
                Console.WriteLine("新接入一个客户端");
                ForClient cc = new ForClient(clientSocket);//每个客户端通信逻辑放到client
                clientlist.Add(cc);
            }

        }

        public static string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName(); //得到主机名
            IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < iPEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    return iPEntry.AddressList[i].ToString();
            }
            return null;
        }

        public static void BroadcastMessage(string message)
        {
            var notConnectedList = new List<ForClient>();

            foreach (var clients in clientlist)
            {
                if (clients.Connected)
                    clients.SendMessage(message);
                else
                {
                    notConnectedList.Add(clients);
                }
            }
            foreach (var temp in notConnectedList)
            {
                clientlist.Remove(temp);
            }
        }
    }
}

