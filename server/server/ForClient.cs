﻿
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace myServer
{

    class ForClient
    {

        private byte[] data = new byte[1024];//这个是一个数据容器
        private Socket socketclient;
        private Thread tt;
        private String name;
        private bool registed;
        private byte op0 = 0;//握手
        private byte op1 = 1;//发送信息
        private byte op2 = 2;//系统消息，无用户名
        private byte op6 = 6;//服务端报错码

        public ForClient(Socket s)
        {
            registed = false;
            socketclient = s;
            //启动一个线程，处理客户端数据接收
            tt = new Thread(ReceiveMessage);
            tt.Start();
        }

        void ReceiveMessage()
        {
            //一直接收客户端的数据
            while (true)
            {
                //在接收数据之前  判断一下socket连接是否断开
                if (socketclient.Poll(10, SelectMode.SelectRead))
                {
                    Console.WriteLine("tcp 客户端已断开");
                    socketclient.Close();
                    break;//跳出循环 终止线程的执行
                }
                else
                {

                    try
                    {
                        int length = socketclient.Receive(data);
                        if (data[1]==op0) {//用户名申请
                            String userName =Encoding.UTF8.GetString(data, 2, length);
                            if (!Program.nameIsExit(userName)) {
                                byte[] temp = new byte[] {0,0};
                                SendMessage(temp);
                                this.name = userName;
                                registed = true;
                                Console.WriteLine("客户端：" + socketclient.RemoteEndPoint +"  注册用户名成功："+name);
                            }
                            
                        }

                        if (data[1]==op1) {//接受聊天数据
                            if (!registed) {//未注册不得聊天
                                Console.WriteLine("客户端未注册用户名，不得聊天");
                                continue;
                            }
                            int i = 2;
                            while (data[i]!=op0) {
                                i++;
                            }
                            String name = Encoding.UTF8.GetString(data, 2, i - 1);
                            String message = Encoding.UTF8.GetString(data,i+1,length);
                            Console.WriteLine("收到了客户端：" + socketclient.RemoteEndPoint + "  " + "消息: " + message+" "+"用户名: "+name);
                        }
                        //广播这个消息
                        Program.BroadcastMessage(data);
                        data = new byte[1024];
                    }

                    catch
                    {
                        Console.WriteLine("出错了");
                    }
                }
            }
        }


        
        public bool nameIsMacth(String userName) {
            if (userName == name)
                return true;
            return false;
        }

        public void SendMessage(byte[] data)
        {
            try {
                socketclient.Send(data);
            }
            catch (Exception ex) { }
            
        }

        public bool Connected
        {
            get { return socketclient.Connected; }
        }
    }
}
