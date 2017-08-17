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
    public partial class addAccount : Form {
        public addAccount() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Account a = new Account(textBox1.Text.Trim(), 0);
            Console.WriteLine(a.publicKey);
            Form1.instance.accountList.Add(a.publicKey, a);

            Form1.instance.comboBox1.Items.Add(a);
            Form1.instance.comboBox1.SelectedItem = a;
            Form1.instance.analyseChain(Form1.instance.comboBox1.SelectedItem.ToString());

            this.Close();

        }
    }
}
