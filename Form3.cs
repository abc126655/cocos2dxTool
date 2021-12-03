using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);//获取Configuration对象
            if (!string.IsNullOrEmpty(textBox4.Text))
            {
                Form1.saveEXCEL = textBox4.Text;
                config.AppSettings.Settings["saveExel"].Value = textBox4.Text;
            }
            if (!string.IsNullOrEmpty(textBox5.Text))
            {
                Form1.fileName = textBox5.Text;
                config.AppSettings.Settings["saveFile"].Value = textBox5.Text;
            }

            config.Save(ConfigurationSaveMode.Modified);       //保存，写不带参数的config.Save()也可以
            ConfigurationManager.RefreshSection("appSettings");//刷新，否则程序读取的还是之前的值（可能已装入内存）
            MessageBox.Show("保存成功");
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            textBox4.Text = Form1.saveEXCEL;
            textBox5.Text = Form1.fileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string [] strs = Common.execCMD("where git").Split('\n');
            for (int k = 0; k < strs.Length; k++)
            {
                if(strs[k].Contains("git.exe"))
                {
                    textBox1.Text = strs[k];
                    break;
                }
            }
            strs = Common.execCMD("where zip").Split('\n');
            for (int k = 0; k < strs.Length; k++)
            {
                if (strs[k].Contains("zip.exe"))
                {
                    textBox2.Text = strs[k];
                    break;
                }
            }
            strs = Common.execCMD("where cocos").Split('\n');
            for (int k = 0; k < strs.Length; k++)
            {
                if (strs[k].Contains("cocos.bat"))
                {
                    textBox3.Text = strs[k];
                    break;
                }
            }

        }
    }
}
