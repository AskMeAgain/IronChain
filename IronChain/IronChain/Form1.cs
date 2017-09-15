using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace IronChain {

    public partial class Form1 : Form {

        public string globalChainPath;
        public static Form1 instance;
        public bool mineASingleBlockFlag = false;

        public List<Transaction> TransactionPool;

        bool miningFlag = true;
        public int latestBlock = 0;

        public string minerAccountName;
        public string mainAccount;

        public Dictionary<string, Account> accountList;
        bool dontAnalyseYetFlag = true;

        string ip;

        public Form1() {

            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            accountList = new Dictionary<string, Account>();
            userTransactionHistory = new List<Transaction>();
            Form1.instance.Text = "IronChain";
            usedTransactions = new List<Transaction>();

            Directory.CreateDirectory("C:\\IronChain\\");

            Utility.loadSettings();
            try {
                ip = new WebClient().DownloadString("http://icanhazip.com");
            } catch (Exception e) {

            }

            textBox1.Text = ip + ":::4712";
            comboBox4.SelectedIndex = 0;

            updateAccountList();


            if (!File.Exists(globalChainPath + "0.blk")) {
                createGenesisBlock();
            }

            timer1.Tick += new EventHandler(requestFilesEvery10Sec);
            timer1.Interval = 10000;
            timer1.Start();

            analyseChain();

        }

        public void requestFilesEvery10Sec(Object myObject, EventArgs myEventArgs) {
            if (manager2 != null && automaticFileRequestFlag)
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
            genesis.difficulty = 4;
            Utility.storeFile(genesis, globalChainPath + genesis.name + ".blk");
        }

        public List<Transaction> usedTransactions;

        private void createParticles(int height) {

            Particle p = new Particle();
            ExtendedParticle extParticle = new ExtendedParticle();
            extParticle.proof = new List<string>();

            for (int i = 0; i < TransactionPool.Count; i++) {

                Transaction trans = TransactionPool[i];

                if (verifyTransactionHashNoSegWit(trans)) {

                    //converting now to segit transaction
                    extParticle.proof.Add(trans.proofOfOwnership);
                    Transaction temp = trans;
                    temp.proofOfOwnership = "_";

                    p.allTransactions.Add(temp);
                } else {
                    Console.WriteLine("TRANSACTION NOT CORRECT!");
                    TransactionPool.Remove(trans);
                }
            }

            if (transactionPoolWindow != null)
                transactionPoolWindow.updateTransactionPool();

            //add hash to block before
            p.hashToBlock = Utility.ComputeHash(globalChainPath + (height - 1));

            Utility.storeFile(extParticle, globalChainPath + "E" + height + ".blk");
            Utility.storeFile(p, globalChainPath + "P" + height + ".blk");

        }


        private bool verifyTransactionHashNoSegWit(Transaction trans) {

            string original = Utility.getHashSha256(trans.id + "");
            string publ = Utility.buildRealPublicKey(trans.owner);
            string signedHash = trans.proofOfOwnership;

            if (Utility.verifyData(original, publ, signedHash)) {
                return true;
            } else {
                return false;
            }
        }

        private bool verifyTransactionHashSegWit(Transaction trans, string signedHash) {

            string original = Utility.getHashSha256(trans.id + "");
            string publ = Utility.buildRealPublicKey(trans.owner);

            if (Utility.verifyData(original, publ, signedHash)) {
                Console.WriteLine("trans is legit!");
                return true;
            } else {
                Console.WriteLine("trans is WRONG!");
                return false;
            }
        }

        private void mineNextBlock(string nonce, int diff) {

            Block nextBlock = new Block();

            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash((latestBlock + 1));
            nextBlock.name = (latestBlock + 1);
            nextBlock.nonce = nonce;
            nextBlock.difficulty = diff;

            nextBlock.createCoins(accountList[minerAccountName]);

            //Store Block
            Utility.storeFile(nextBlock, globalChainPath + (latestBlock + 1) + ".blk");

            Particle p = Utility.loadFile<Particle>(globalChainPath + "P" + (latestBlock + 1) + ".blk");
            foreach (Transaction trans in p.allTransactions) {
                for (int i = 0; i < TransactionPool.Count; i++) {
                    if (trans.id.Equals(TransactionPool[i].id))
                        TransactionPool.RemoveAt(i);
                }
            }

            analyseChain();

            if (manager2 != null)
                manager2.pushFile();

            if (miningFlag && !mineASingleBlockFlag) {
                mine();
            }

        }

        public void mine() {

            if (InvokeRequired) {
                Invoke(new Action(() => {
                    //button1.Enabled = false;
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

            int count = TransactionPool.Count;
            createParticles(latestBlock + 1);

            string hashFromLatestBlock = Utility.ComputeHash(globalChainPath + latestBlock + "");
            string hashFromParticle = hashFromLatestBlock;

            Console.WriteLine("getting hash of particle " + (latestBlock + 1));
            hashFromParticle = Utility.ComputeHash(globalChainPath + "P" + (latestBlock + 1));

            int difficulty = miningDifficulty;

            while (miningFlag) {

                if (TransactionPool.Count > count) {
                    count = TransactionPool.Count;
                    createParticles(latestBlock + 1);
                    hashFromParticle = Utility.ComputeHash(globalChainPath + "P" + (latestBlock + 1));
                }

                string hashToProof = nonce + hashFromLatestBlock + hashFromParticle;

                string hash = Utility.getHashSha256(hashToProof);

                if (Utility.verifyHashDifficulty(hash, difficulty)) {
                    Console.WriteLine("calculated > " + nonce + "__" + hashFromLatestBlock + "__" + hashFromParticle);
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
        public List<Transaction> userTransactionHistory;

        public void analyseChain() {

            if (dontAnalyseYetFlag)
                return;


            if (!File.Exists(globalChainPath + "0.blk"))
                createGenesisBlock();


            Block b = Utility.loadFile<Block>(globalChainPath + "0.blk");
            int i = 1;
            bool errorFlag = false;
            int difficulty = b.difficulty;
            userTransactionHistory.Clear();

            //get genesis block too
            foreach (Account acc in accountList.Values) {
                acc.coinCounter = 0;
                if (acc.publicKey.Equals(b.minerAddress))
                    acc.coinCounter += 3;
            }

            List<bool> listOfMissingExtBlocks = new List<bool>();

            while (File.Exists(globalChainPath + i + ".blk")) {

                b = Utility.loadFile<Block>(globalChainPath + i + ".blk");

                if (b == null) {
                    errorFlag = true;
                    i++;
                    break;
                }

                string hashOfThisBlock = Utility.ComputeHash(globalChainPath + i);
                string hashOfBlockBefore = Utility.ComputeHash(globalChainPath + (i - 1));
                string proofHash = Utility.getHashSha256(b.nonce + hashOfBlockBefore + b.hashOfParticle);

                //checking nonce
                if (!Utility.verifyHashDifficulty(proofHash, difficulty)) {
                    Console.WriteLine("WRONG SHIT! HASH DIFFICULTY WRONG!");
                    //Console.WriteLine("analyseChain > " + b.nonce + "__" + hashOfBlockBefore + "__" + b.hashOfParticle);
                    break;
                }

                if (File.Exists(globalChainPath + "P" + i + ".blk")) {

                    //particle exists
                    Particle p = Utility.loadFile<Particle>(globalChainPath + "P" + i + ".blk");
                    ExtendedParticle ext = null;

                    if (File.Exists(globalChainPath + "E" + i + ".blk"))
                        ext = Utility.loadFile<ExtendedParticle>(globalChainPath + "E" + i + ".blk");

                    //block points to particle
                    if (!Utility.ComputeHash(globalChainPath + "P" + i).Equals(b.hashOfParticle)) {
                        break;
                    }

                    //block points to block before
                    if (!hashOfBlockBefore.Equals(b.hashOfBlockBefore)) {
                        break;
                    }

                    //particle points to block before
                    if (!hashOfBlockBefore.Equals(p.hashToBlock)) {
                        break;
                    }

                    Dictionary<string, int> listOfAllOwners = new Dictionary<string, int>();
                    listOfAllOwners.Add(b.minerAddress, 0);
                    listOfMissingExtBlocks.Add(ext != null ? true : false);

                    for (int index = 0; index < p.allTransactions.Count; index++) {

                        Transaction trans = p.allTransactions[index];

                        //verify each transaction
                        if (ext != null)
                            if (!verifyTransactionHashSegWit(trans, ext.proof[index]))
                                break;

                        //add transaction to history for user
                        if (trans.owner.Equals(accountList[mainAccount].publicKey) || trans.receiver.Equals(accountList[mainAccount].publicKey)) {
                            userTransactionHistory.Add(trans);
                        }

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



                } else {
                    break;
                }

                i++;
            }

            i--;

            if (errorFlag) {
                Console.WriteLine("ERROR BLOCK<this?");

                File.Delete(globalChainPath + i + ".blk");
                File.Delete(globalChainPath + "P" + i + ".blk");

                i--;
            }

            latestBlock = i;

            if (i > 3) {
                for (int index = listOfMissingExtBlocks.Count - 1; index > 1; index--) {
                    if (listOfMissingExtBlocks[index] && listOfMissingExtBlocks[index - 1]) {
                        break;
                    } else {
                        Console.WriteLine("deleting from " + globalChainPath + "P" + index + 1 + ".blk");
                        File.Delete(globalChainPath + "P" + (index + 1) + ".blk");
                        File.Delete(globalChainPath + (index + 1) + ".blk");
                        latestBlock = index;
                    }
                }
            }

            if (InvokeRequired) {
                Invoke(new Action(() => {
                    updateCoinBalanceGUI();
                }));
            } else {
                updateCoinBalanceGUI();
            }
        }

        private void updateCoinBalanceGUI() {
            label5.Text = "Block " + latestBlock;
            label3.Text = "" + checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock) + " Iron";
            displayTransactionHistory();
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
                    if (p != null) {
                        foreach (Transaction trans in p.allTransactions) {
                            if (trans.receiver.Equals(owner)) {
                                coinbalance += trans.amount;
                            }

                            if (trans.owner.Equals(owner)) {
                                coinbalance -= trans.amount + trans.transactionfee;

                            }

                            if (b.minerAddress.Equals(owner)) {
                                coinbalance += trans.transactionfee;
                            }
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

        public PeerNetworking manager2;

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
            mineASingleBlockFlag = false;
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
            Transaction t = new Transaction(thisAccount.publicKey, receiver, amount, thisAccount.privateKey, latestBlock);
            t.data = TransData;
            t.transactionfee = comboBox5.SelectedIndex + 1;

            TransactionPool.Add(t);

            if (transactionPoolWindow != null)
                transactionPoolWindow.updateTransactionPool();


            if (manager2 != null) {
                manager2.pushTransactionToServers();
            }

            textBox6.Clear();
            textBox2.Clear();

        }

        private void onAmountChanged(object sender, EventArgs e) {
            calculateTransactionAmount();
        }

        private void calculateTransactionAmount() {

            int amount = 0;
            int.TryParse(textBox6.Text, out amount);

            label17.Text = (1 + amount + comboBox5.SelectedIndex) + " Iron";
        }

        private void onDifficultyChanged(object sender, EventArgs e) {
            miningDifficulty = comboBox4.SelectedIndex + 4;
        }



        private void button8_Click(object sender, EventArgs e) {

            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] t = entry.AddressList;

            manager2 = new PeerNetworking();
            manager2.ConnectToListener(t[0], 3000);
        }

        public bool automaticFileRequestFlag = true;

        private void button10_Click(object sender, EventArgs e) {
            manager2.pushTransactionToServers();
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
            displayTransactionHistory();
        }

        int transPage = 0;

        private void displayTransactionHistory() {
            label19.Text = "";

            if (transPage == 0) {
                button18.Enabled = false;
            } else {
                button18.Enabled = true;
            }


            for (int i = 0 + 5 * transPage; i < 5 + 5 * transPage && i < userTransactionHistory.Count; i++) {
                Transaction t = userTransactionHistory[i];

                string s = accountList[mainAccount].publicKey.Equals(t.owner) ?
                    s = "Sending " + t.amount + " Iron to " + t.receiver :
                    s = "Receiving " + t.amount + " Iron from " + t.owner;

                label19.Text += s + Environment.NewLine;
            }

            button17.Enabled = (transPage + 1) * 5 < userTransactionHistory.Count ? true : false;

        }

        private void onClickShowPreviousPage(object sender, EventArgs e) {
            transPage--;
            displayTransactionHistory();
        }

        private void onClickShowNextPages(object sender, EventArgs e) {
            transPage++;
            displayTransactionHistory();
        }

        private void addAccountToolStripMenuItem_Click(object sender, EventArgs e) {
            addAccount a = new addAccount();
            a.Show();
        }

        private void onBarOpenTestWindow(object sender, EventArgs e) {
            TestWindow a = new TestWindow();
            a.Show();
        }

        private void onBarOpenTransactionPool(object sender, EventArgs e) {

            transactionPoolWindow = new TransactionPool();
            transactionPoolWindow.Show();

        }

        public TransactionPool transactionPoolWindow;
        public string TransData;

        private void onClickAddData(object sender, EventArgs e) {
            addData a = new addData();
            a.Show();
        }

        private void onTransactionFeeChanged(object sender, EventArgs e) {
            calculateTransactionAmount();
        }

        private void onClickChainGlobalPath(object sender, EventArgs e) {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            globalChainPath = dialog.SelectedPath + "\\";
            analyseChain();
        }
    }
}
