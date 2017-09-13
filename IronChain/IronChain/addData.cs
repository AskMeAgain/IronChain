using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IronChain {
    public partial class addData : Form {
        public addData() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Form1.instance.TransData = textBox1.Text;

            this.Close();
        }
    }
}
