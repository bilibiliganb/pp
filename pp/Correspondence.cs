using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PPserver.Correspondence;

namespace pp
{
    namespace Correspondence
    {
       
        class MySocket                     
        {
            Socket socketSend;
            string ip;
            string point;
            Command command;
            public MySocket(string ip,string point)
            {
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.ip = ip;this.point = point;
            }

            public void connect()
            {
                IPAddress ip = IPAddress.Parse(this.ip);
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(this.point));
                socketSend.Connect(point);

                Thread th = new Thread(receive);
                th.IsBackground = true;
                th.Start();
            }

            public void receive()                                                     //接收command对象 
            {
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[1024 * 1024 * 3];
                        int r = socketSend.Receive(buffer);
                        if (r == 0) break;                                                
                        string str = Encoding.UTF8.GetString(buffer, 0, r);
                        command=Interpreter.getData(str);
                        
                    }
                    catch { }
                }
            }
            public void send(Command comd)                                              //将command对象发送出去
            {
                try
                {
                    Interpreter interpreter = new Interpreter(comd);
                    string str = interpreter.getSerialization();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                    socketSend.Send(buffer);
                }
                catch { }
            }
            public void close()
            {
                socketSend.Close();
            }
            public bool received()                                                 
            {
                if(command==null)
                {
                    return false;
                }
                return true;
            }

            public Command getData()
            {
                return command;
            }
        }
    }
}
