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
    public partial class TransactionPool : Form {
        public TransactionPool() {
            InitializeComponent();
        }

        public void updateTransactionPool() {
            textBox1.Text = "";

            Console.WriteLine("exeuting" + Form1.instance.TransactionPool.Count);

            foreach (Transaction trans in Form1.instance.TransactionPool) {
                Console.WriteLine("adding trans");
                textBox1.Text += trans.toString() + Environment.NewLine;
            }
        }
    }
}
