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
        public int txtBox2, txtBox3;
        public float txtBox1;

        public Setting()
        {
            InitializeComponent();
             StreamReader sw = new StreamReader("Setting.ini");
             Alph.Text = sw.ReadLine();
             Epohs.Text = sw.ReadLine();

             txtBox1 = (float)Convert.ToDouble(Alph.Text);
             txtBox3 = Convert.ToInt32(Epohs.Text);

             sw.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter sR = new StreamWriter("Setting.ini");
           
            sR.WriteLine(Alph.Text);
            sR.WriteLine(Epohs.Text);
            sR.Close();            
            
            txtBox1 = (float)Convert.ToDouble((string)Alph.Text);
            txtBox3 = Convert.ToInt32((string)Epohs.Text);
 
            this.Hide();
        }
    }
}
