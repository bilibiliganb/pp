using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using PPserver.Data;
using System.Web.Script.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace PPserver
{
    namespace Correspondence  //名称空间
    {

        [Serializable]
        public enum Type { login, respond, alive, notify, msg };//第一次连接，回应用户数据,保活,通知,消息
        [Serializable]
        public class Command
        {
       
            public Type command_type { get; }
            public Command(Type a)
            {
                command_type = a;
            }
        }
        [Serializable]
        public class Login : Command
        {
         
            public string userid;
      
            public string psw;
            public Login(string userid,string psw):base(Type.login)
            {
                this.userid = userid;
                this.psw = psw;
            }
        }
        [Serializable]
        public class Alive : Command
        {
      
            public string userid;
            public Alive(string userid) : base(Type.alive)
            {
                this.userid = userid;
            }
        }
        [Serializable]
        public class Respond : Command
        {
      
            public UserData user_data;
        
            public UserFrid user_frid;
  
            public MsgBox msg_box;
         
            public NoteBox note_box;
       
            public FridState fridstate;
            public Respond(UserData user_data,UserFrid user_frid,MsgBox msg_box,NoteBox note_box,FridState fridstate):base(Type.respond)
            {
                this.user_data = user_data;
                this.user_frid = user_frid;
                this.msg_box = msg_box;
                this.note_box = note_box;
                this.fridstate = fridstate;
            }
        }
        [Serializable]
        public class Message : Command
        {

            public string from_user;
     
            public string to_user;
         
            public string msg;
       
            public DateTime time;
          
            public bool style;             //类型，0为单发给用户，1为群发消息
            public Message(string from_user,string to_user,string msg,DateTime time,bool style):base(Type.msg)
            {
                this.from_user = from_user;
                this.to_user = to_user;
                this.msg = msg;
                this.time = time;
                this.style = style;
            }

        }
        [Serializable]
        public class Notify : Command
        {
    
            public FridState fridstate;
            public Notify(FridState fridstate) : base(Type.notify)
            {
                this.fridstate = fridstate;
            }
        }
        [Serializable]
        public class Interpreter     //将对客户端发送的command对象序列化，将收到的反序列化，可以
        {
            Command command;
            public Interpreter(Command command)
            {
                this.command = command;
            }

            public string getSerialization()              //将Command对象序列化
            {
                string jsonStr = @"F:\PP\runtime\" + DateTime.Now.ToString("yyMMddhhmmss") + ".dat";             //将序列化的文件名作为序列化的结果
                FileStream fileStream = new FileStream(jsonStr, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fileStream, command);
                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
                return jsonStr;
            }

            public static Command getData(string jsonStr)    //将jsonstr转化成Command对象，静态共有方法
            {
                FileStream fileStream = new FileStream(jsonStr, FileMode.Open,FileAccess.Read, FileShare.ReadWrite);
                BinaryFormatter b = new BinaryFormatter();
                Command rs = b.Deserialize(fileStream) as Command;
                return rs;
            }
        }

        class MySocket
        {
            Socket socketSend;
            string ip;
            string point;
            private class Entity
            {
                public Command command;
                public DateTime datetime;
            }
            Entity entity;
            public MySocket(string ip, string point)
            {
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                entity = new Entity();
                entity.datetime = DateTime.Now;
                this.ip = ip; this.point = point;
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
                        entity.command = Interpreter.getData(str);
                        entity.datetime = DateTime.Now;

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
                DateTime dt = DateTime.Now.AddSeconds(-30);                               //30s内接收的消息为新消息
                if (entity.command == null || dt.CompareTo(entity.datetime) > 0)
                {
                    return false;
                }
                return true;
            }

            public Command getData()
            {
                return entity.command;
            }
        }
    }
}
