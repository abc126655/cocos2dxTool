using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<string> files = Common.GetFileList(textBox1.Text);
            DataTable dt = new DataTable();
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("val", typeof(bool));
            //dt.Columns.Add("fullname", typeof(string));
            foreach (string file in files)
            {
                if(file.EndsWith(".xlsx") && file.IndexOf("~$")<0)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = file.Replace(textBox1.Text, "");
                    dr[1] = true;
                    dt.Rows.Add(dr);
                }
            }
            dataGridView1.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strpatj = textBox1.Text+"\\tolua";
            if (Directory.Exists(strpatj))
                Directory.Delete(strpatj, true);

            Directory.CreateDirectory(strpatj);
            string file,tofile;           
            for (int i =0; i< this.dataGridView1.Rows.Count ;i++)
            {
                if (Convert.ToBoolean( dataGridView1.Rows[i].Cells[1].Value))
                {
                    file = textBox1.Text + dataGridView1.Rows[i].Cells[0].Value.ToString();
                    tofile = strpatj+"\\" + System.IO.Path.GetFileName(file);
                    tofile = tofile.Replace(".xlsx",".lua");
                    Common.MyReadTo(file, tofile);
                }
            }
            MessageBox.Show("已成功生成，可打开目录查看");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool isAllCheck = true;
        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = isAllCheck;
            }
            isAllCheck = !isAllCheck;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = Form1.excelPath;
        }

        private void Form2_Leave(object sender, EventArgs e)
        {
            
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.excelPath = textBox1.Text;
        }
    }
}
