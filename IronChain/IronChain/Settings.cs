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
    public partial class Settings : Form {

        [Serializable]
        public class SettingsObject {

            public string defaultMiningAccountName;
            public int defaultMiningDifficulty;
            public bool isLightMode;
            public string defaultMainAccount;
            public string globalChainPath;

            public SettingsObject() {
                defaultMiningAccountName = "";
                defaultMiningDifficulty = 4;
                defaultMainAccount = "";
                isLightMode = false;
                globalChainPath = "C:\\IronChain\\";
            }
        }

        public SettingsObject settings;

        public Settings() {
            InitializeComponent();

            settings = Utility.loadFile<SettingsObject>("C:\\IronChain\\settings.set");

            if (settings == null) {
                settings = new SettingsObject();
            }

            //setting account addresse dropdown list
            comboBox1.Items.Clear();
            bool flag1 = true;
            foreach (Account acc in Form1.instance.accountList.Values) {
                comboBox1.Items.Add(acc);
                if (settings.defaultMiningAccountName.Equals(acc.name)) {
                    comboBox1.SelectedItem = acc;
                    flag1 = false;
                }
            }
            if (flag1) {
                comboBox1.SelectedIndex = 0;
            }



            comboBox3.Items.Clear();
            bool flag3 = true;
            foreach (Account acc in Form1.instance.accountList.Values) {
                comboBox3.Items.Add(acc);
                if (settings.defaultMainAccount.Equals(acc.name)) {
                    comboBox3.SelectedItem = acc;
                }
            }

            if (flag3) {
                comboBox3.SelectedIndex = 0;
            }

            comboBox2.SelectedIndex = settings.defaultMiningDifficulty - 4;
            checkBox1.Checked = settings.isLightMode;

        }

        private void onClickSafeSettings(object sender, EventArgs e) {
            storeValues();
            Utility.loadSettings();
            this.Close();
            Form1.instance.analyseChain();
        }

        private void storeValues() {

            settings = new SettingsObject();

            settings.defaultMiningAccountName = comboBox1.Text;
            settings.defaultMiningDifficulty = Convert.ToInt32(comboBox2.Text);
            settings.isLightMode = checkBox1.Checked;
            settings.defaultMainAccount = comboBox3.Text;

            if (path.Equals("")) {
                path = "C:\\IronChain\\";
            } else {
                settings.globalChainPath = path;
            }

            Utility.storeFile(settings,"C:\\IronChain\\settings.set");

        }

        private void onClickCancelSettings(object sender, EventArgs e) {
            this.Close();
        }

        string path = "";

        private void onClickSelectFilePath(object sender, EventArgs e) {

            var fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                path = fbd.SelectedPath + "\\";
            }

        }
    }
}
