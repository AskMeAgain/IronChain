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

            Form1.instance.accountList.Add(a.name, a);

            Form1.instance.comboBox1.Items.Add(a);
            Form1.instance.comboBox1.SelectedItem = a;
            Form1.instance.analyseChain(Form1.instance.comboBox1.SelectedItem.ToString());

            Form1.instance.comboBox2.Items.Add(a);
            Form1.instance.comboBox2.SelectedItem = a;

            a.addKeys(Utility.generateKeyFiles());

            Utility.storeFile(a, a.name + ".acc");

            this.Close();

        }
    }
}
