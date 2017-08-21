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

            public SettingsObject() {
                defaultMiningAccountName = "";
                defaultMiningDifficulty = 4;
                defaultMainAccount = "";
                isLightMode = false;

            }
        }

        public SettingsObject settings;

        public Settings() {
            InitializeComponent();

            settings = Utility.loadFile<SettingsObject>("settings.set");

            if (settings == null) {
                settings = new SettingsObject();
            }

            //setting account addresse dropdown list
            comboBox1.Items.Clear();
            foreach (Account acc in Form1.instance.accountList.Values) {
                comboBox1.Items.Add(acc);
                if (settings.defaultMiningAccountName.Equals(acc.name)) {
                    comboBox1.SelectedItem = acc;
                }
            }

            comboBox3.Items.Clear();
            foreach (Account acc in Form1.instance.accountList.Values) {
                comboBox3.Items.Add(acc);
                if (settings.defaultMainAccount.Equals(acc.name)) {
                    comboBox3.SelectedItem = acc;
                }
            }


            comboBox2.SelectedIndex = settings.defaultMiningDifficulty - 4;
            checkBox1.Checked = settings.isLightMode;

        }

        private void onClickSafeSettings(object sender, EventArgs e) {
            storeValues();
            Utility.loadSettings();
            this.Close();
        }

        private void storeValues() {

            settings = new SettingsObject();

            settings.defaultMiningAccountName = comboBox1.Text;
            settings.defaultMiningDifficulty = Convert.ToInt32(comboBox2.Text);
            settings.isLightMode = checkBox1.Checked;
            settings.defaultMainAccount = comboBox3.Text;

            Utility.storeFile(settings, "settings.set");

        }

        private void onClickCancelSettings(object sender, EventArgs e) {
            this.Close();
        }

    }
}
