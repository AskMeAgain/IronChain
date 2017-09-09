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

            Utility.storeFile(a, Form1.instance.globalChainPath + a.name + ".acc");

            Form1.instance.mainAccount = a.name;
            Form1.instance.minerAccountName = a.name;
            Form1.instance.updateAccountList();


            Console.WriteLine("test!");

            this.Close();

        }
    }
}
