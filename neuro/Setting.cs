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

namespace neuro
{
    public partial class Setting : Form
    {
        public int txtBox1, txtBox2, txtBox3;
        public Setting()
        {
            InitializeComponent();
             StreamReader sw = new StreamReader("Setting.ini");
             textBox2.Text = sw.ReadLine();
             textBox1.Text = sw.ReadLine();
             textBox3.Text = sw.ReadLine();

             txtBox1 = Convert.ToInt32(textBox1.Text);
             txtBox2 = Convert.ToInt32(textBox2.Text);
             txtBox3 = Convert.ToInt32(textBox3.Text);

             sw.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter sR = new StreamWriter("Setting.ini");
            sR.WriteLine(textBox2.Text);
            sR.WriteLine(textBox1.Text);
            sR.WriteLine(textBox3.Text);
            sR.Close();
            
            
            txtBox1 = Convert.ToInt32((string)textBox1.Text);
            txtBox2 = Convert.ToInt32((string)textBox2.Text);
            txtBox3 = Convert.ToInt32((string)textBox3.Text);

            this.Hide();
        }
    }
}
