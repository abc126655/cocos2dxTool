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
using System.Xml.Linq;

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
            string strpatj = textBox2.Text;
            if (!Directory.Exists(strpatj))
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

            DataTable dt = new DataTable();
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("val", typeof(bool));
            try
            {
                XDocument xml = XDocument.Load(Form1.saveEXCEL);
                textBox1.Text = xml.Root.Element("workPath").Value;
                textBox2.Text = xml.Root.Element("exportPath").Value;
                foreach (XElement item in xml.Root.Element("data").Descendants("row"))
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.Attribute("num").Value;
                    dr[1] = item.Attribute("val").Value;
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //将datatable绑定到datagridview上显示结果  
            dataGridView1.DataSource = dt;
        }

        private void Form2_Leave(object sender, EventArgs e)
        {
            
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            XDocument xml = new XDocument();

            XElement rootFirst = new XElement("base");
            XElement ele = new XElement("workPath");
            ele.Value = textBox1.Text;
            rootFirst.Add(ele);
            ele = new XElement("exportPath");
            ele.Value = textBox2.Text;
            rootFirst.Add(ele);
            ele = new XElement("data");
            rootFirst.Add(ele);
            string temp;
            for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
            {
                temp = dataGridView1.Rows[i].Cells[0].Value.ToString();
                if (temp.Length > 0)
                {
                    XElement newEle = new XElement("row");
                    newEle.SetAttributeValue("num", temp);
                    newEle.SetAttributeValue("val", dataGridView1.Rows[i].Cells[1].Value.ToString());
                    ele.Add(newEle);
                }
            }
            xml.Add(rootFirst);
            xml.Save(Form1.saveEXCEL);
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.ColumnIndex ==1)
            {
                //button4_Click(sender, null);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
