using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPserver.UserManager;

namespace PPserver
{
    namespace Data
    {

        [Serializable]
        public class UserData                              //用户信息类
        {
            public string userid { get; set; }
            public string psw { get; set; }
            public string name { get; set; }
            public string sex { get; set; }
            public string image { get; set; }
            public DateTime birth { get; set; }
            public string phone { get; set; }        //用于登录验证
       
        }

        [Serializable]
        public class UserFrid                               //用户好友类
        {
            public string userid { get; set; }
            public string fridid { get; set; }
            public string fridname { get; set; }    //相当于好友备注，默认是好友的name
        }

        public class Msg                                     //用于用户向服务器发送消息
        {
            public string fromuser { get; set; }
            public string touser { get; set; }
            public string msg { get; set; }
            public DateTime datetime { get; set; }
            public bool tag { get; set; }                   //tag=true表示单发，false表示群发
        }

        [Serializable]
        public class MsgBox                                 //消息盒子类，用于服务器给发给用户的历史消息记录
        {
            public string userid { get; set; }
            public string fridid { get; set; }
            public string msg { get; set; }
            public DateTime time { get; set; }
            public bool tag { get; set; }                  //tag=true表示好友消息，false表示群消息
        }

        [Serializable]
        public class NoteBox                                //日志类
        {
            public string userid { get; set; }
            public string note { get; set; }
            public DateTime time{ get; set;}
        }

        [Serializable]
        public class FridState                             //好友状态类
        {
            public string fridid { get; set; }
            public UserState fridstate { get; set; }
        }

        [Serializable]
        public class authority                             //用户权限列表
        {
            public string userid { get; set; }
            public bool login { get; set; }
            public bool fridctl { get; set; }
            public bool talk { get; set; }
        }
    }
}
