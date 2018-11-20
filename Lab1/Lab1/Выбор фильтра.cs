using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form4 fr4 = new Form4();
            fr4.Show();
            Hide();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Мультипликативный мультипликативный = new Мультипликативный();
            мультипликативный.Show();
            Hide();
         
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Медианный медианный = new Медианный();
            медианный.Show();
            Hide();
        }
    }
}
