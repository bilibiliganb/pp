using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPserver
{
    class Dao
    {
        SqlConnection sc;
        public SqlConnection connect()  //默认连接的pp库
        {
            string str = @"Data Source=(local)\HQ;Initial Catalog=pp;Integrated Security=True";//数据库链接字符串
            sc = new SqlConnection(str);//创建数据库链接对象
            sc.Open();//打开数据库
            return sc;//返回数据库
        }
        
        public SqlCommand command(string sql)
        {
            SqlCommand cmd;
            if (sc == null)
                cmd = new SqlCommand(sql, connect());
            else
                cmd = new SqlCommand(sql, sc);
            return cmd;
        }
        public int Execute(string sql)//更新操作
        {
            return command(sql).ExecuteNonQuery();
        }
        public SqlDataReader read(string sql)//读取操作
        {
            return command(sql).ExecuteReader();
        }

        public void query(string[] s,out DataSet ds)
        {
            ds = new DataSet();
            foreach (string sql in s)
            {
                string[] ms = sql.ToLower().Split(' ');
                for (int i = 0; i < ms.Length; i++)
                {
                    if (ms[i].Equals("from"))
                    {
                        ms[0] = ms[i + 1];
                        break;
                    }
                }
                SqlDataAdapter adapter = new SqlDataAdapter(sql, sc);
                adapter.Fill(ds, ms[0]);
            }
        }
        public void close()
        {
            sc.Close();//关闭数据库链接
        }
    }
}
