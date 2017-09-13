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

        private void panel1_DragOver(object sender, DragEventArgs e) {

            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string[] splitted = FileList[0].Split('\\');
            string nameWithEnding = splitted[splitted.Length - 1];
            string[] name = nameWithEnding.Split('.');
             
            if (FileList[0].EndsWith(".acc")) {
                File.Copy(FileList[0], "C:\\IronChain\\" + nameWithEnding, true);

                Form1.instance.mainAccount = name[0];
                Form1.instance.minerAccountName = name[0];
                Form1.instance.updateAccountList();

                this.Close();
            }


        }
    }
}
