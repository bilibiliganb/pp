using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pp
{
    public partial class Usertalk : Form
    {
        public Usertalk()
        {
            InitializeComponent();
        }
        Socket socketSend;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //创建负责通信的Socket
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(txtServer.Text);
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtport.Text));
                //获得要连接的原创服务器应用的IP地址和端口号
                socketSend.Connect(point);



                Showmsg("连接成功");

                //开启一个新线程不停接收服务端发来的消息
                Thread th = new Thread(Recive);
                th.IsBackground = true;
                th.Start();
            }
            catch { }

        }

        void Showmsg(string str)
        {
            txtlog.AppendText(str + "\r\n");
        }

        /// <summary>
        /// 客户端给服务器发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string str = txtMsg.Text.Trim();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                socketSend.Send(buffer);
                Showmsg("我:" + str);
                txtMsg.Text = "";
            }
            catch { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        void Recive()
        {
            while(true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    int r = socketSend.Receive(buffer);
                    if (r == 0) break;
                    string str = Encoding.UTF8.GetString(buffer, 0, r);

                    Showmsg(socketSend.RemoteEndPoint + ":" + str);
                }
                catch { }
            }
        }

    }
}
