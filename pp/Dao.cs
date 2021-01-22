using System.Data.SqlClient;

namespace pp
{
    class Dao
    {
        SqlConnection sc;
        public SqlConnection connect()
        {
            string str = @"Data Source=(local)\XA;Initial Catalog=pp;Integrated Security=True";//数据库链接字符串
            sc = new SqlConnection(str);//创建数据库链接对象
            sc.Open();//打开数据库
            return sc;//返回数据库
        }

        public SqlCommand command(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, connect());
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

        public void Daoclose()
        {
            sc.Close();//关闭数据库链接
        }
    }
}
