using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IronChain {
    public partial class TestWindow : Form {
        public TestWindow() {
            InitializeComponent();
        }

        private void onClickHostServer3000(object sender, EventArgs e) {
            Form1.instance.manager2 = new PeerNetworking();
            Form1.instance.manager2.ListenForConnections(3000);
        }

        private void onClickHostServer3001(object sender, EventArgs e) {
            Form1.instance.manager2 = new PeerNetworking();
            Form1.instance.manager2.ListenForConnections(3001);
        }

        private void onClickConnectTo3000(object sender, EventArgs e) {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] t = entry.AddressList;

            Form1.instance.manager2 = new PeerNetworking();
            Form1.instance.manager2.ConnectToListener(t[0], 3000);
        }

        private void onClickConnectTo3001(object sender, EventArgs e) {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] t = entry.AddressList;

            Form1.instance.manager2 = new PeerNetworking();
            Form1.instance.manager2.ConnectToListener(t[0], 3001);
        }

        private void onClickRequestBlock(object sender, EventArgs e) {
            Form1.instance.manager2.requestFileInfo();
        }

        private void onClickPushBlock(object sender, EventArgs e) {
            Form1.instance.manager2.pushFile();
        }

        private void onClickChangeGlobalPath(object sender, EventArgs e) {
            Form1.instance.globalChainPath = "C:\\IronChain\\TestChain\\";
        }

        private void disableEnableAutomaticBlockDownload(object sender, EventArgs e) {

            if (Form1.instance.automaticFileRequestFlag) {
                button1.Text = "DISABLE automatic block download";
            } else {
                button1.Text = "ENABLE automatic block download";
            }

            Form1.instance.automaticFileRequestFlag = !Form1.instance.automaticFileRequestFlag;
            Console.WriteLine(Form1.instance.automaticFileRequestFlag);
        }
    }
}
