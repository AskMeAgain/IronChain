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
    public partial class Form1 : Form {

        public static Form1 instance;

        public Form1() {
            InitializeComponent();
            instance = this;
        }

        private void onClickStoreFile(object sender, EventArgs e) {

            Block b1 = new Block(3333);

            Utility.storeFile(b1, "test");

        }

        private void onClickLoadFile(object sender, EventArgs e) {

            Block b = Utility.loadFile<Block>("test");

            if (b != null) {
                addToLog("Block loaded: " + b.name);
            } else {
                addToLog("Failure loading block!");

            }

        }

        public void addToLog(string s) {

            String text = textBox1.Text;
            text += s + Environment.NewLine;
            textBox1.Text = @text;

        }

    }
}
