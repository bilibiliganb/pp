using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PPClient;


namespace pp
{
    public partial class PP : Form
    {
        public PP()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                login();
            }
            else
            {
                MessageBox.Show("输入有空项，请重新输入");
            }
        }

        public void login()
        {

            Dao dao = new Dao();
            string sql = "select * from t_userbase where id='" + textBox1.Text + "'and psw='" + textBox2.Text + "'";
            IDataReader dc = dao.read(sql);
            if (dc.Read())
            {
                Data.UID = dc["id"].ToString();
                Data.UName = dc["name"].ToString();
                MessageBox.Show("登录成功");
                this.Hide();
                //Usertalk a = new Usertalk();
                //a.ShowDialog();
                Form1 a = new Form1();
                a.ShowDialog();

                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败");
            }
            dao.Daoclose();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox2.PasswordChar = '\0';
            else
                textBox2.PasswordChar = '*';
        }
    }
}
