using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace IronChain {

    public partial class Form1 : Form {

        private static ManualResetEvent waitForAccountCreation = new ManualResetEvent(false);

        public string globalChainPath;
        public static Form1 instance;

        public List<Transaction> TransactionPool;

        bool miningFlag = true;
        Dictionary<string, int> transHistory;
        public int latestBlock = 0;

        public string minerAccountName;

        public Dictionary<String, Account> accountList;
        bool dontAnalyseYetFlag = true;


        public Form1() {

            //waitForAccountCreation.Reset();

            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            accountList = new Dictionary<string, Account>();

            Utility.loadSettings();

            updateAccountList();


            if (!File.Exists(globalChainPath + "0.blk")) {
               createGenesisBlock();
            }

            analyseChain();
        }

        public void updateAccountList() {

            Account a = new Account("Add a new Account", 0);

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

            foreach (Account acc in accountList.Values) {

                comboBox1.Items.Add(acc);

            }

            //select later
            comboBox1.SelectedItem = a;

            dontAnalyseYetFlag = false;

        }

        private void onClickCreateGenesisBlock(object sender, EventArgs e) {
            createGenesisBlock();
        }

        private void createGenesisBlock() {
            Block genesis = new Block(0);
            genesis.hashOfParticle = "genesis";
            //genesis.giveSomeCoins(accountList["KelvinPetry"].publicKey, 100);
            Utility.storeFile(genesis, globalChainPath + genesis.name + ".blk");
        }

        public void updateTransactionPoolWindow() {

            textBox2.Text = "";

            foreach (Transaction t in TransactionPool) {
                textBox2.AppendText(t.toString() + Environment.NewLine);
            }
        }

        private void createParticles() {

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
            p.hashToBlock = Utility.ComputeHash(globalChainPath + latestBlock);

            particleName = "P" + (latestBlock + 1);

            Utility.storeFile(p,globalChainPath + particleName + ".blk");

            updateTransactionPoolWindow();

            //create light particle
            Particle light = new Particle();
            string lightName =globalChainPath + "L" + (latestBlock + 1);
            light.hashToBlock = Utility.ComputeHash(globalChainPath + "" + latestBlock);
            Utility.storeFile(light,lightName + ".blk");

        }

        private bool verifyTransactionHash(Transaction trans) {

            string hashOfEverything = trans.id + "" + trans.amount + "" + trans.receiver + "" + trans.owner;
            string original = "" + hashOfEverything.GetHashCode();

            string publ = trans.owner;
            string signedHash = trans.proofOfOwnership;

            if (Utility.verifyData(original, publ, signedHash)) {
                return true;
            } else {
                return false;
            }
        }

        private void mineNextBlock(string nonce) {

            Block nextBlock = new Block();
            createParticles();

            Particle p = Utility.loadFile<Particle>(globalChainPath + "P" + (latestBlock + 1) + ".blk");

            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash((latestBlock + 1));
            nextBlock.numberOfTransactions += p.allTransactions.Count;
            nextBlock.name = (latestBlock + 1);
            nextBlock.nonce = nonce;

            nextBlock.createCoins(accountList[minerAccountName]);

            //Store Block
            Utility.storeFile(nextBlock,globalChainPath + (latestBlock + 1) + ".blk");

            analyseChain();

        }

        private void onClickStartMining(object sender, EventArgs e) {

            if (minerAccountName.Equals("")) {

                if (accountList.Count == 1) {
                    minerAccountName = accountList[comboBox1.Text].name;
                } else {

                    string message = "You did not select a Miner Account, go to Settings and select it";
                    string caption = "Error";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.

                    result = MessageBox.Show(message, caption, buttons);

                    return;
                }
            }

            miningFlag = true;

            int i = 0;
            string hashFromLatestBlock = Utility.ComputeHash(globalChainPath + latestBlock + "");
            int difficulty = miningDifficulty;
            while (miningFlag) {

                string hashToProof = i + hashFromLatestBlock + accountList[minerAccountName].publicKey;

                string hash = Utility.getHashSha256(hashToProof);

                if (Utility.verifyHashDifficulty(hash, difficulty)) {
                    mineNextBlock(i + "");
                    break;
                }

                i++;
            }

        }

        private void onClickStopMining(object sender, EventArgs e) {
            miningFlag = false;
        }

        private void onClickAnalyseChain(object sender, EventArgs e) {
            analyseChain();
        }

        public int miningDifficulty;

        public void analyseChain() {

            if (dontAnalyseYetFlag)
                return;

            int difficulty = miningDifficulty;

            if (!File.Exists(globalChainPath + "0.blk")) {
                createGenesisBlock();
            }

            Block b = Utility.loadFile<Block>(globalChainPath + "0.blk");

            //get genesis block too
            foreach (Account acc in accountList.Values) {
                acc.coinCounter = 0;
                foreach (Block.Coin coin in b.allCoins) {
                    if (acc.publicKey.Equals(coin.owner)) {
                        acc.coinCounter += coin.amount;
                    }
                }
            }

            int i = 1;
            bool errorFlag = false;

            while (File.Exists(globalChainPath + i + ".blk")) {

                b = Utility.loadFile<Block>(globalChainPath + i + ".blk");
                string hashOfBlock = Utility.ComputeHash(globalChainPath + i + "");
                string hashOfBlockBefore = Utility.ComputeHash(globalChainPath + (i - 1) + "");

                string hashToProof = b.nonce + hashOfBlockBefore + b.allCoins[0].owner;
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
                Invoke(new Action(() => updateUI()));
            } else {
                label5.Text = "Block " + latestBlock;
                label3.Text = "" + checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock);
            }
        }

        private void updateUI() {
            label5.Text = "Block " + latestBlock;
            label3.Text = "" + checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock);
        }

        private int checkCoinBalance(string owner, int blockheight) {

            int coinbalance = 0;
            for (int i = 0; i <= blockheight; i++) {

                if (!File.Exists(globalChainPath + i + ".blk")) {
                    break;
                }

                Block b = Utility.loadFile<Block>(globalChainPath + i + ".blk");

                foreach (Block.Coin coin in b.allCoins) {
                    if (coin.owner.Equals(owner)) {
                        coinbalance += coin.amount;
                    }
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
            analyseChain();
            label3.Text = "" + checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock);
        }

        private void onClickAddAccount(object sender, EventArgs e) {
            Form f2 = new addAccount();
            f2.ShowDialog();
        }

        private void onClickSendIron(object sender, EventArgs e) {
            Form f2 = new sendIron();
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

        private void onClickDeleteAllAccounts(object sender, EventArgs e) {

            string[] allFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.acc");
            foreach (string s in allFiles) {
                string[] splitted = s.Split('\\');
                string nameOfFile = splitted[splitted.Length - 1];
                File.Delete("C:\\IronChain\\" + nameOfFile);
            }

            updateAccountList();
        }

        private void onClickOpenSettings(object sender, EventArgs e) {
            Settings s = new Settings();
            s.ShowDialog();
        }

        private void onClickCreateServerListener(object sender, EventArgs e) {
            manager = new Networking();
            manager.StartServer();
        }

        Networking manager;

        private void onClickConnectClient(object sender, EventArgs e) {
            manager = new Networking();
            manager.StartClient();
        }

        private void onClickRequestFile(object sender, EventArgs e) {

            int requestHeight = latestBlock+1;

            if (!textBox3.Text.Equals("")){
                requestHeight = Convert.ToInt32(textBox3.Text);
            }
            Console.WriteLine(requestHeight);
            manager.RequestFile(requestHeight);

        }

        private void button4_Click(object sender, EventArgs e) {
            Console.WriteLine(globalChainPath);
        }


        private void button6_Click(object sender, EventArgs e) {
            globalChainPath = @"C:\Users\kelvi\Desktop\IronChain\";
        }
    }
}
