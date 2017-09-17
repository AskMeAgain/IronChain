using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IronChain {
    public partial class TestWindow : Form {
        public TestWindow() {
            InitializeComponent();
        }

        private void onClickHostServer3000(object sender, EventArgs e) {
            if (button7.Enabled) {
                Form1.instance.networkManager = new PeerNetworking();
                Form1.instance.networkManager.ListenForConnections(3000);
            }
            button7.Enabled = false;
        }

        private void onClickHostServer3001(object sender, EventArgs e) {

            if (button6.Enabled) {
                Form1.instance.networkManager = new PeerNetworking();
                Form1.instance.networkManager.ListenForConnections(3001);
            }
            button6.Enabled = false;

        }

        private void onClickConnectTo3001(object sender, EventArgs e) {

            if (button9.Enabled) {
                IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress[] t = entry.AddressList;
                Form1.instance.networkManager = new PeerNetworking();
                Form1.instance.networkManager.ConnectToListener(t[0], 3001);
            }

            button9.Enabled = false;
        }

        private void onClickConnectTo3000(object sender, EventArgs e) {

            if (button8.Enabled) {
                IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress[] t = entry.AddressList;
                Form1.instance.networkManager = new PeerNetworking();
                Form1.instance.networkManager.ConnectToListener(t[0], 3000);
            }

            button8.Enabled = false;
        }

        private void onClickRequestBlock(object sender, EventArgs e) {
            Form1.instance.networkManager.requestFileInfo();
        }

        private void onClickPushBlock(object sender, EventArgs e) {
            Form1.instance.networkManager.pushFile();
        }

        private void onClickChangeGlobalPath(object sender, EventArgs e) {
            button15.Enabled = false;
            button15.Text = "Path changed";

            Directory.CreateDirectory("C:\\IronChain\\TestChain\\");

            Form1.instance.globalChainPath = "C:\\IronChain\\TestChain\\";

            Form1.instance.analyseChain();
        }

        private void disableEnableAutomaticBlockDownload(object sender, EventArgs e) {

            if (!Form1.instance.automaticFileRequestFlag) {
                button1.Text = "DISABLE automatic block download";
            } else {
                button1.Text = "ENABLE automatic block download";
            }

            Form1.instance.automaticFileRequestFlag = !Form1.instance.automaticFileRequestFlag;
        }

        private System.Windows.Forms.Timer timer1;

        private void onClickAddRandomTrans(object sender, EventArgs e) {

            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 2000;
            timer1.Tick += new EventHandler(createTrans);
            timer1.Start();    

        }

        public void createTrans(object sender, EventArgs e) {
            Transaction t = new Transaction(Form1.instance.accountList[Form1.instance.mainAccount].publicKey, "TestAccount", 1, Form1.instance.accountList[Form1.instance.mainAccount].privateKey, Form1.instance.latestBlock);
            t.data = "";
            t.transactionfee = Form1.instance.comboBox5.SelectedIndex + 1;

            Form1.instance.TransactionPool.Add(t);

            if (Form1.instance.transactionPoolWindow != null)
                Form1.instance.transactionPoolWindow.updateTransactionPool();
        }

        private void onClickMineASingleBlock(object sender, EventArgs e) {
            Form1.instance.mineASingleBlockFlag = true;
            Thread a = new Thread(Form1.instance.mine);
            a.Name = "Mining";
            a.Start();
        }
    }
}
