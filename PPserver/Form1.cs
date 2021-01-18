using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Data.SqlClient;
using PPserver.Correspondence;
using PPserver.Data;

namespace PPserver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                //点击开始监听 在服务端创建一个负责监视IP地址和端口号的Socket
                Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Any;
                //创建端口对象
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtport.Text));
                //监听
                socketWatch.Bind(point);
                ShowMsg("监听成功");
                socketWatch.Listen(10);
                Thread th = new Thread(Listen);
                th.IsBackground = true;
                th.Start(socketWatch);
            }
            catch {  }

        }

        Socket socketSend;

        /// 等待客户端连接，并且创建与之通信的socket
        void Listen(object o)
        {
            Socket socketWatch = o as Socket;
            //等待客户端连接
            while (true)
            {
                try
                {
                    socketSend = socketWatch.Accept();
                    //192.168.0.6:连接成功
                    ShowMsg(socketSend.RemoteEndPoint.ToString() + ":" + "连接成功");
                    //开启新线程接收客户端不断发来的消息
                    Thread th = new Thread(Recive);
                    th.IsBackground = true;
                    th.Start(socketSend);
                }
                catch { }



            }
        }

        /// <summary>
        /// 服务器端不停接收客户端发送的数据
        /// </summary>
        /// <param name="o"></param>
        void Recive(object o)
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 2];

                    //实际接收的有效字节数
                    int r = socketSend.Receive(buffer);
                    if (r == 0) break;
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    Command command = Interpreter.getData(str);
                    if(command.command_type== Correspondence.Type.login)
                    {
                        Login lg = (Login)command;
                        if (verify(lg))
                        {
                            ShowMsg("用户" + lg.userid + "登录成功！");
                            response(lg.userid, socketSend,true);
                        }
                        else{
                            response(lg.userid, socketSend,false);
                        }
                    }
                }
                catch { }
            }
        }

        void ShowMsg(string str)
        {
            txtlog.AppendText(str + "\r\n");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str = txtMsg.Text.Trim();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            socketSend.Send(buffer);
            ShowMsg("我:" + str);
            txtMsg.Text = "";
        }

        /// <summary>
        /// 发送respond报文
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="socket"></param>
        private void response(string userid,Socket socket,bool tag)      
        {
            if(tag==true)
            {
                UserData userdata = new UserData();
                string sql = "select * from dbo.t_userbase where id='" + userid + "'";
                Dao dao = new Dao();
                dao.connect();
                SqlDataReader reader = dao.read(sql);
                while (reader.Read())
                {
                    userdata.userid = reader["id"].ToString();
                    userdata.psw = reader["psw"].ToString();
                    userdata.name = reader["name"].ToString();
                    userdata.sex = reader["sex"].ToString();
                    userdata.birth = DateTime.Parse(reader["birth"].ToString());
                    userdata.image = reader["image"].ToString();
                    userdata.phone = reader["phone"].ToString();

                    Respond respond = new Respond(userdata, null, null, null, null);
                    Interpreter interpreter = new Interpreter(respond);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(interpreter.getSerialization());
                    socket.Send(buffer);
                }
                dao.close();
            }
            else
            {
                Respond respond = new Respond(null, null, null, null, null);
                Interpreter interpreter = new Interpreter(respond);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(interpreter.getSerialization());
                socket.Send(buffer);
            }
        }
        private bool verify(Login lg)
        {
            string sql = "select * from dbo.t_userbase where id='" + lg.userid + "' and psw='" + lg.psw + "'";
            Dao dao = new Dao();
            dao.connect();
            SqlDataReader reader = dao.read(sql);
            if (reader.Read())
            {
                dao.close();
                return true;
            }
            dao.close();
            return false;
        }
    }
}
