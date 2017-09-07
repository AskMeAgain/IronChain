using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace IronChain {

    public partial class Form1 : Form {

        public string globalChainPath;
        public static Form1 instance;

        public List<Transaction> TransactionPool;

        bool miningFlag = true;
        Dictionary<string, int> transHistory;
        public int latestBlock = 0;

        public string minerAccountName;
        public string mainAccount;

        public Dictionary<String, Account> accountList;
        bool dontAnalyseYetFlag = true;

        string ip;

        public Form1() {

            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            accountList = new Dictionary<string, Account>();

            Directory.CreateDirectory("C:\\IronChain\\");

            Utility.loadSettings();

            ip = new WebClient().DownloadString("http://icanhazip.com");
            textBox1.Text = ip + ":::4712";
            comboBox4.SelectedIndex = 0;

            updateAccountList();


            if (!File.Exists(globalChainPath + "0.blk")) {
                createGenesisBlock();
            }

            timer1.Tick += new EventHandler(requestFilesEvery30Sec);
            timer1.Interval = 10000;
            timer1.Start();

            analyseChain();
        }

        public void requestFilesEvery30Sec(Object myObject, EventArgs myEventArgs) {
            Console.WriteLine("checking files!");
            if (manager2 != null)
                manager2.requestFileInfo();
        }

        public void updateAccountList() {

            Account a = new Account("Add a new Account", 0);
            Account addAccount = a;

            string[] allAccountNames = Directory.GetFiles("C:\\IronChain\\", "*.acc");

            accountList = new Dictionary<string, Account>();

            foreach (string s in allAccountNames) {

                string[] splitted = s.Split('\\');
                string nameOfFile = splitted[splitted.Length - 1];
                a = Utility.loadFile<Account>("C:\\IronChain\\" + nameOfFile);

                string[] onlyName = nameOfFile.Split('.');

                accountList.Add(onlyName[0], a);

            }

            if (accountList.Count == 0) {
                addAccount accWindow = new addAccount();
                accWindow.Show(this);
                return;
            }

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();

            foreach (Account acc in accountList.Values) {

                comboBox1.Items.Add(acc);
                comboBox2.Items.Add(acc);
                comboBox3.Items.Add(acc);

                if (acc.name.Equals(minerAccountName)) {
                    comboBox2.SelectedItem = acc;
                }

                if (acc.name.Equals(mainAccount)) {
                    comboBox1.SelectedItem = acc;
                    comboBox3.SelectedItem = acc;
                }

            }

            comboBox1.Items.Add(addAccount);

            dontAnalyseYetFlag = false;

        }

        private void onClickCreateGenesisBlock(object sender, EventArgs e) {
            createGenesisBlock();
        }

        private void createGenesisBlock() {
            Block genesis = new Block(0);
            genesis.hashOfParticle = "genesis";
            genesis.minerAddress = "";
            Utility.storeFile(genesis, globalChainPath + genesis.name + ".blk");
        }

        private void createParticles(int height) {

            string particleName = "";

            //transaction particle
            Particle p = new Particle();
            transHistory = new Dictionary<string, int>();

            while (TransactionPool.Count > 0) {

                Transaction trans = TransactionPool[0];
                TransactionPool.RemoveAt(0);

                if (verifyTransactionHash(trans)) {
                    p.addTransaction(trans);
                }
            }

            //add hash to block before
            p.hashToBlock = Utility.ComputeHash(globalChainPath + (height-1));

            particleName = "P" + height;

            Utility.storeFile(p, globalChainPath + particleName + ".blk");

            //create light particle
            Particle light = new Particle();
            string lightName = globalChainPath + "L" + height;
            light.hashToBlock = Utility.ComputeHash(globalChainPath + "" + (height-1));
            Utility.storeFile(light, lightName + ".blk");

        }

        private bool verifyTransactionHash(Transaction trans) {

            string original = Utility.getHashSha256(trans.id + "");

            string publ = trans.owner;
            string signedHash = trans.proofOfOwnership;

            if (Utility.verifyData(original, publ, signedHash)) {
                return true;
            } else {
                return false;
            }
        }

        private void mineNextBlock(string nonce, int diff) {

            Block nextBlock = new Block();

            Particle p = Utility.loadFile<Particle>(globalChainPath + "P" + (latestBlock + 1) + ".blk");

            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash((latestBlock + 1));
            nextBlock.name = (latestBlock + 1);
            nextBlock.nonce = nonce;
            nextBlock.difficulty = diff;

            nextBlock.createCoins(accountList[minerAccountName]);

            //Store Block
            Utility.storeFile(nextBlock, globalChainPath + (latestBlock + 1) + ".blk");

            analyseChain();

            if (manager2 != null)
                manager2.pushFile();

            if (miningFlag) {
                mine();
            }

        }

        private void mine() {
            if (InvokeRequired) {
                Invoke(new Action(() => {
                    button1.Enabled = false;
                    button1.Text = "Mining!";
                    button3.Enabled = true;
                }));
            }


            if (minerAccountName.Equals("")) {

                if (accountList.Count == 1) {
                    minerAccountName = accountList[comboBox1.Text].name;
                } else {

                    string message = "You did not select a Miner Account, go to Settings and select it";
                    string caption = "Error";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);

                    return;
                }
            }

            miningFlag = true;

            int nonce = 0;

            string hashFromLatestBlock = Utility.ComputeHash(globalChainPath + latestBlock + "");
            string hashFromParticle = Utility.ComputeHash(globalChainPath + "P" + latestBlock);

            int difficulty = miningDifficulty;
            bool firstTime = true;
            while (miningFlag) {

                if (TransactionPool.Count > 0 || firstTime) {
                    createParticles(latestBlock+1);
                    firstTime = false;
                }

                string hashToProof = nonce + hashFromLatestBlock + hashFromParticle;

                string hash = Utility.getHashSha256(hashToProof);

                if (Utility.verifyHashDifficulty(hash, difficulty)) {
                    mineNextBlock(nonce + "", difficulty);
                    break;
                }

                nonce++;
            }
        }

        private void onClickStopMining(object sender, EventArgs e) {
            miningFlag = false;
        }

        public int miningDifficulty;
        public bool isLightMode;

        public void analyseChain() {

            if (dontAnalyseYetFlag)
                return;


            if (!File.Exists(globalChainPath + "0.blk")) {
                createGenesisBlock();
            }

            Block b = Utility.loadFile<Block>(globalChainPath + "0.blk");

            int difficulty = b.difficulty;

            //get genesis block too
            foreach (Account acc in accountList.Values) {
                acc.coinCounter = 0;
                if (acc.publicKey.Equals(b.minerAddress))
                    acc.coinCounter += 3;
            }

            int i = 1;
            bool errorFlag = false;

            while (File.Exists(globalChainPath + i + ".blk")) {

                b = Utility.loadFile<Block>(globalChainPath + i + ".blk");

                if (b == null) {
                    errorFlag = true;
                    i++;
                    break;
                }

                string hashOfBlock = Utility.ComputeHash(globalChainPath + i + "");
                string hashOfBlockBefore = Utility.ComputeHash(globalChainPath + (i - 1) + "");

                string hashToProof = b.nonce + hashOfBlockBefore + b.hashOfParticle;
                string proofHash = Utility.getHashSha256(hashToProof);

                //checking nonce
                if (!Utility.verifyHashDifficulty(proofHash, difficulty)) {
                    Console.WriteLine("WRONG SHIT!");
                    break;
                }

                if (File.Exists(globalChainPath + "P" + i + ".blk")) {

                    //particle exists
                    Particle p = Utility.loadFile<Particle>(globalChainPath + "P" + i + ".blk");

                    //block points to particle
                    if (!Utility.ComputeHash(globalChainPath + "P" + i).Equals(b.hashOfParticle)) {
                        break;
                    }

                    //particle points to block before
                    if (!Utility.ComputeHash(globalChainPath + (i - 1)).Equals(p.hashToBlock)) {
                        break;
                    }

                    Dictionary<string, int> listOfAllOwners = new Dictionary<string, int>();
                    foreach (Transaction trans in p.allTransactions) {

                        //verify each transaction
                        if (verifyTransactionHash(trans)) {

                            //add transactions to a list
                            if (listOfAllOwners.ContainsKey(trans.owner)) {
                                listOfAllOwners[trans.owner] += trans.amount;
                            } else {
                                listOfAllOwners.Add(trans.owner, trans.amount);
                            }

                            // check if each transaction is possible
                            foreach (string owner in listOfAllOwners.Keys) {
                                if (checkCoinBalance(owner, i - 1) < listOfAllOwners[owner]) {
                                    errorFlag = true;
                                    break;
                                }
                            }
                        }
                    }

                } else if (File.Exists(globalChainPath + "L" + i + ".blk")) {

                    Particle p = Utility.loadFile<Particle>(globalChainPath + "L" + i + ".blk");

                    //block points to particle
                    if (!Utility.ComputeHash("L" + i).Equals(b.hashOfLightParticle)) {
                        break;
                    }

                    //particle points to block before
                    if (!Utility.ComputeHash("" + (i - 1)).Equals(p.hashToBlock)) {
                        break;
                    }

                } else {
                    break;
                }

                i++;
            }

            i--;

            if (errorFlag) {
                Console.WriteLine("ERROR BLOCK");

                File.Delete(globalChainPath + i + ".blk");
                File.Delete(globalChainPath + "P" + i + ".blk");
                File.Delete(globalChainPath + "L" + i + ".blk");

                i--;
            }

            Console.WriteLine("latest Block!" + i);

            latestBlock = i;

            if (InvokeRequired) {
                Invoke(new Action(() => {
                    label5.Text = "Block " + latestBlock;
                    label3.Text = "" + checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock) + " Iron";
                }));
            } else {
                label5.Text = "Block " + latestBlock;
                label3.Text = checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock) + " Iron";
            }
        }

        public void isServerUI() {
            if (InvokeRequired) {
                Invoke(new Action(() => label1.Text = "Listening for Connections"));
            } else {
                label1.Text = "Listening for Connections";
            }
        }

        private int checkCoinBalance(string owner, int blockheight) {

            int coinbalance = 0;
            for (int i = 0; i <= blockheight; i++) {

                if (!File.Exists(globalChainPath + i + ".blk")) {
                    break;
                }

                Block b = Utility.loadFile<Block>(globalChainPath + i + ".blk");

                if (b.minerAddress.Equals(owner)) {
                    coinbalance += 3;
                }


                if (i > 0) {
                    Particle p = Utility.loadFile<Particle>(globalChainPath + "P" + i + ".blk");

                    foreach (Transaction trans in p.allTransactions) {
                        if (trans.receiver.Equals(owner)) {
                            coinbalance += trans.amount;
                        }

                        if (trans.owner.Equals(owner)) {
                            coinbalance -= trans.amount;
                        }
                    }
                }
            }

            return coinbalance;

        }

        private void onAccountChanged(object sender, EventArgs e) {

            if (comboBox1.Text.Equals("Add a new Account")) {

                addAccount a = new addAccount();
                a.Show();

            } else {

                mainAccount = comboBox1.Text;
                Utility.Settings set = Utility.loadFile<Utility.Settings>("C:\\IronChain\\settings.set");
                set.mainAccount = mainAccount;
                Utility.storeFile(set, "C:\\IronChain\\settings.set");

                analyseChain();
                label3.Text = checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock) + " Iron";

            }
        }

        private void onClickAddAccount(object sender, EventArgs e) {
            Form f2 = new addAccount();
            f2.ShowDialog();
        }

        private void onClickDeleteIronChain(object sender, EventArgs e) {

            string[] allFiles = Directory.GetFiles(globalChainPath, "*.blk");
            foreach (string s in allFiles) {
                string[] splitted = s.Split('\\');
                string nameOfFile = splitted[splitted.Length - 1];

                if (!nameOfFile.Equals("0.blk"))
                    File.Delete(globalChainPath + nameOfFile);
            }

            analyseChain();
        }

        private void onClickCreateServerListener(object sender, EventArgs e) {
            manager2 = new PeerNetworking();
            manager2.ListenForConnections(4712);
        }

        PeerNetworking manager2;

        private void onClickConnectClient(object sender, EventArgs e) {

            manager2.ConnectToListener(IPAddress.Parse(textBox5.Text), Convert.ToInt32(textBox5.Text));
        }

        private void onClickStartServer(object sender, EventArgs e) {
            button4.Enabled = false;
            button4.Text = "Hosting Server";
            manager2 = new PeerNetworking();
            manager2.ListenForConnections(30000);
        }

        private void onClickMineBlock(object sender, EventArgs e) {
            Thread a = new Thread(mine);
            a.Name = "Mining";
            a.Start();
        }

        private void onClickStopMineBlock(object sender, EventArgs e) {
            miningFlag = false;
            button1.Enabled = true;
            button3.Enabled = false;
            button1.Text = "Start Mining";
        }

        private void onMinerAddressChanged(object sender, EventArgs e) {
            minerAccountName = comboBox2.Text;
            Utility.Settings set = Utility.loadFile<Utility.Settings>("C:\\IronChain\\settings.set");
            Console.WriteLine("mineracocunt" + minerAccountName);
            set.minerAccount = minerAccountName;
            Utility.storeFile(set, "C:\\IronChain\\settings.set");
        }

        private void onClickSendIron(object sender, EventArgs e) {

            int amount = Convert.ToInt32(textBox6.Text);
            string receiver = textBox2.Text;

            Account thisAccount = accountList[comboBox3.Text];


            //SIGN TRANSACTION
            Transaction t = new Transaction(thisAccount.publicKey, receiver, amount);
            t.giveID(latestBlock);

            t.proofOfOwnership = Utility.signData(Utility.getHashSha256(t.id + ""), thisAccount.privateKey);

            TransactionPool.Add(t);

            if (!PeerNetworking.isServer && manager2 != null) {
                manager2.pushTransactionToServer();
            }

            textBox6.Clear();
            textBox2.Clear();

        }

        private void onAmountChanged(object sender, EventArgs e) {

            label17.Text = textBox6.Text + " Iron";

            if (textBox6.Text.Equals("")) {
                label17.Text = "0 Iron";
            }
        }

        private void onDifficultyChanged(object sender, EventArgs e) {
            miningDifficulty = comboBox4.SelectedIndex + 4;
        }

        private void button6_Click(object sender, EventArgs e) {
            manager2 = new PeerNetworking();
            manager2.ListenForConnections(3000);
        }

        private void button7_Click(object sender, EventArgs e) {
            manager2 = new PeerNetworking();
            manager2.ListenForConnections(3001);
        }

        private void button8_Click(object sender, EventArgs e) {

            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] t = entry.AddressList;

            manager2 = new PeerNetworking();
            manager2.ConnectToListener(t[0], 3000);
        }

        private void button10_Click(object sender, EventArgs e) {
            manager2.pushTransactionToServer();
        }

        private void button11_Click(object sender, EventArgs e) {
            foreach (Transaction t in TransactionPool) {
                Console.WriteLine("transaction" + t.amount + " " + t.owner);
            }
        }

        private void onBarDeleteAccounts(object sender, EventArgs e) {

            string[] allFiles = Directory.GetFiles("C:\\IronChain\\", "*.acc");
            foreach (string s in allFiles) {
                string[] splitted = s.Split('\\');
                string nameOfFile = splitted[splitted.Length - 1];
                File.Delete("C:\\IronChain\\" + nameOfFile);
            }

            updateAccountList();
        }

        private void button14_Click(object sender, EventArgs e) {
            manager2.requestFileInfo();
        }

        private void button13_Click(object sender, EventArgs e) {
        }

        private void button12_Click(object sender, EventArgs e) {
            manager2.pushFile();
        }

        private void button15_Click(object sender, EventArgs e) {
            globalChainPath = "C:\\IronChain\\TestChain\\";
            analyseChain();
        }

        private void button16_Click(object sender, EventArgs e) {
            Console.WriteLine(PeerNetworking.executerList.Count);
        }

        private void button9_Click(object sender, EventArgs e) {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] t = entry.AddressList;

            manager2 = new PeerNetworking();
            manager2.ConnectToListener(t[0], 3001);
        }
    }
}
