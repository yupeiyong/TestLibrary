using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1.Properties;
using ServiceStack;
using ServiceStack.Redis;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //var dgv = dataGridView1;
            //dgv.Columns.Add(new DataGridViewColumn { HeaderText = Resources.Form1_Form1_Load__111111 });
            //dgv.Columns.Add(new DataGridViewActionButtonColumn {HeaderText="66666666666"});
            //dgv.Rows.Add("2222222222222222");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var frm = new frmActionButtonColumn();
            frm.Show();
        }
    }
}
