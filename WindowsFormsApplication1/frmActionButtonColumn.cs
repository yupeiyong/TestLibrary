using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;


namespace WindowsFormsApplication1
{
    public partial class frmActionButtonColumn : Form
    {
        
        public frmActionButtonColumn()
        {
            InitializeComponent();
        }

        private void frmActionButtonColumn_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ProductLib.GetProducts();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var name = "actionButtonColumn";
            var column=new DataGridViewActionButtonColumn {Name= name ,Width=130};
            if (!dataGridView1.Columns.Contains(name))
            {
                dataGridView1.Columns.Add(column);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //用户单击DataGridView“操作”列中的“修改”按钮。
            if (DataGridViewActionButtonCell.IsModifyButtonClick(sender, e))
            {
                string objectId = dataGridView1[nameof(Product.Id), e.RowIndex].Value.ToString(); // 获取所要修改关联对象的主键。
                MessageBox.Show(objectId);
            }

            //用户单击DataGridView“操作”列中的“删除”按钮。
            if (DataGridViewActionButtonCell.IsDeleteButtonClick(sender, e))
            {
                string objectId = dataGridView1[nameof(Product.Id), e.RowIndex].Value.ToString(); // 获取所要删除关联对象的主键。
                MessageBox.Show(objectId);
            }
        }
    }

    internal class ProductLib
    {

        public static List<Product> GetProducts()
        {
            return new List<Product>()
            {
                new Product {Id=1,Name = "Lili",Sex = "女"},
                new Product {Id=2,Name = "Lili",Sex = "女"},
                new Product {Id=3,Name = "Lili",Sex = "女"},
                new Product {Id=4,Name = "Lili",Sex = "女"},
                new Product {Id=5,Name = "Lili",Sex = "女"},
            };
        }

    }
}
