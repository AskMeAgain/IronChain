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
            updateTransactionPool();
        }

        public void updateTransactionPool() {

            if (InvokeRequired) {
                Invoke(new Action(() => {
                    textBox1.Text = "";
                    foreach (Transaction trans in Form1.instance.TransactionPool) {
                        textBox1.Text += trans.proofOfOwnership + Environment.NewLine;
                    }
                }));
            } else {
                textBox1.Text = "";
                foreach (Transaction trans in Form1.instance.TransactionPool) {
                    textBox1.Text += trans.proofOfOwnership + Environment.NewLine;
                }
            }

        }
    }
}
