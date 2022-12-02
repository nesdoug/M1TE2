using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M1TE2
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }


        public static bool skipTextChange = false;


        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.close_it3();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // set the information
            textBox1.Text = Form1.dither_factor.ToString();
            if(Form1.f3_cb1 == false)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
            if (Form1.f3_cb2 == false)
            {
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }
            if (Form1.f3_cb3 == false)
            {
                checkBox3.Checked = false;
            }
            else
            {
                checkBox3.Checked = true;
            }
            skipTextChange = false;
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked == false)
            {
                Form1.f3_cb1 = false;
            }
            else
            {
                Form1.f3_cb1 = true;
            }
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                Form1.f3_cb2 = false;
            }
            else
            {
                Form1.f3_cb2 = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (skipTextChange == true) return;

            string str = textBox1.Text;
            if (str == "") return;


            skipTextChange = true;
            
            int value = 0;
            int.TryParse(str, out value);
            if (value > 12) value = 12; // max value
            if (value < 0) value = 0; // min value
            str = value.ToString();
            textBox1.Text = str;
            Form1.dither_factor = value;
            skipTextChange = false;
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked == false)
            {
                Form1.f3_cb3 = false;
            }
            else
            {
                Form1.f3_cb3 = true;
            }
        }
    }
}
