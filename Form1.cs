using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static string fileName = "save.xml";
        public static  string saveEXCEL = "saveEXCEL.xml";
        public Form1()
        {
            InitializeComponent();
            fileName = ConfigurationManager.AppSettings["saveFile"];
            saveEXCEL = ConfigurationManager.AppSettings["saveExel"];
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "这里显款输出信息 \r\n";

            ReadXmlFile(fileName);
        }

        private void ReadXmlFile(string file)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("num", typeof(string));
            dt.Columns.Add("val", typeof(string));
            try
            {
                XDocument xml = XDocument.Load(file);
                Console.WriteLine(xml);
                textBox1.Text = xml.Root.Element("workPath").Value;
                foreach (XElement item in xml.Root.Element("data").Descendants("row"))
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.Attribute("num").Value;
                    dr[1] = item.Attribute("val").Value;
                    dt.Rows.Add(dr);
                }
                label3.Text = fileName;
                //StreamReader sr = new StreamReader(fileName, Encoding.Default);
                //while (!sr.EndOfStream)
                //{
                //    string[] items = sr.ReadLine().Split(':');
                //    if (items.Length > 1)
                //    {
                //        DataRow dr = dt.NewRow();
                //        dr[0] = items[0];
                //        dr[1] = items[1];
                //        dt.Rows.Add(dr);
                //    }

                //}
                //sr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //将datatable绑定到datagridview上显示结果  
            dataGridView1.DataSource = dt;
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            bool isEncry = btn.Name == "button4";
            string path = textBox1.Text;
            if(isEncry)
            {
                path = path.Replace("\\src", "\\src_luac\\src");

            }
            string lastVerion = "";
            string workPath =  System.Environment.CurrentDirectory;
            string[] cmdStr;
            button1.Enabled = false;
            label1.Text = "";
            string tempExe;
            for (int i = this.dataGridView1.Rows.Count - 1; i >=0; i--)
            {
                if (lastVerion.Length<=1)
                {
                    if (dataGridView1.Rows[i].Cells[1].Value != null)
                    {
                        lastVerion = dataGridView1.Rows[i].Cells[1].Value.ToString();
                        lastVerion += " ";
                    }
                }
                else
                {
                    cmdStr = new string[] {"",""};
                    cmdStr[0] = "cd " + path;
                    cmdStr[1] = "git diff " + lastVerion + dataGridView1.Rows[i].Cells[1].Value.ToString();
                    cmdStr[1] += " --name-only ";
                    tempExe = Common.execCMD(cmdStr);
                    label1.Text += tempExe;
                    panel1.AutoScrollPosition = new Point(0, panel1.VerticalScroll.Maximum);
                    Application.DoEvents();
                    if (!tempExe.StartsWith("[error]"))
                    {
                        cmdStr[1] = workPath;
                        cmdStr[1] += "\\zip ud" + dataGridView1.Rows[i].Cells[0].Value.ToString() + ".zip";
                        string[] temp = label1.Text.Split('\n');
                        for (int k = 0; k < temp.Length; k++)
                        {
                            if (temp[k].Contains("/src/"))
                            {
                                int index = temp[k].IndexOf("src");
                                temp[k] = temp[k].Remove(0, index + 4);
                                if (isEncry) temp[k] += "c";
                                cmdStr[1] += " ";
                                cmdStr[1] += temp[k];
                            }
                        }
                        label1.Text += Common.execCMD(cmdStr);
                        panel1.AutoScrollPosition = new Point(0, panel1.VerticalScroll.Maximum);
                        Application.DoEvents();
                    }
                    
                }
            }
            button1.Enabled = true;
        }
        public void SaveXmlFile()
        {
            XDocument xml = new XDocument();

            XElement rootFirst =new XElement("base");
            //rootFirst.SetAttributeValue("lastSave",fileName);
            XElement ele = new XElement("workPath");
            ele.Value = textBox1.Text;
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
            xml.Save(fileName);
        }
        public void SaveTxtFile()
        {
            try { 
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
                //实例化一个StreamWriter-->与fs相关联
                StreamWriter sw = new StreamWriter(fs);
                //开始写入
                if (this.dataGridView1.Rows.Count <= 1)
                {
                    MessageBox.Show("没有数据！导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string str = "";
                    for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
                    {
                        str = this.dataGridView1.Rows[i].Cells[0].Value.ToString();
                        if(str.Length>1)
                        {
                            str += ":";
                            str += this.dataGridView1.Rows[i].Cells[1].Value.ToString();
                            sw.WriteLine(str);
                        }
                    }
                    //sw.Write(this.textBox1.Text);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                    MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try {
                System.Diagnostics.Process.Start(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void 保存XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveXmlFile();
            MessageBox.Show("已成保存到程序当前目录");
        }

        private void 打开XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//允许打开多个文件
            dialog.DefaultExt = ".xml";//打开文件时显示的可选文件类型
            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter =  "xlsx文件|*.xml" ;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
                ReadXmlFile(fileName);

            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void eXCELToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fm = new Form2();
            fm.ShowDialog();
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] cmdStr = new string[] { "", ""};
            cmdStr[0] = "cd " + Directory.GetParent(textBox1.Text);
            cmdStr[1] = "git pull";
            string tempExe = Common.execCMD(cmdStr);
            label1.Text += tempExe;
            panel1.AutoScrollPosition = new Point(0, panel1.VerticalScroll.Maximum);
            Application.DoEvents();
            if (!tempExe.StartsWith("[error]"))
            {
                cmdStr[1] = "cocos luacompile -s src -d src_luac/src -e -k demoKey -b demoSign --disable-compile";
                label1.Text += Common.execCMD(cmdStr);
                MessageBox.Show("work sucess");
            }
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 fm = new Form3();
            fm.ShowDialog();
        }
    }
}
