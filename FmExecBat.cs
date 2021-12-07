using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class FmExecBat : Form
    {
        public FmExecBat()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string file = textBox1.Text;
            switch(btn.Tag.ToString())
            {
                case "2":
                    file = textBox2.Text;
                    break;
                case "3":
                    file = textBox3.Text;
                    break;
                default:
                    break;
            }
            if(string.IsNullOrEmpty(file))
            {
                MessageBox.Show("bat file is not allow to be empty");
                return;
            }
            Common.execBatFile(file);
        }
    }
}
