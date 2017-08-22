﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Net.PeerToPeer;
using System.Net;


namespace IronChain {

    public partial class Form1 : Form {

        public static Form1 instance;

        public List<Transaction> TransactionPool;

        bool miningFlag = true;
        Dictionary<string, int> transHistory;
        public int latestBlock = 0;

        public string minerAccountName;

        public Dictionary<String, Account> accountList;

        public Form1() {
            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            accountList = new Dictionary<string, Account>();



            //update account list

            updateAccountList();

            if (!File.Exists("0.blk")) {
                createGenesisBlock();
            }

            Utility.loadSettings();

            analyseChain();
        }

        public void updateAccountList() {

            Account a = new Account("Add a new Account", 0);

            string[] allAccountNames = Directory.GetFiles(Environment.CurrentDirectory, "*.acc");

            accountList = new Dictionary<string, Account>();

            foreach (string s in allAccountNames) {

                string[] splitted = s.Split('\\');
                string nameOfFile = splitted[splitted.Length - 1];
                a = Utility.loadFile<Account>(nameOfFile);

                string[] onlyName = nameOfFile.Split('.');

                accountList.Add(onlyName[0], a);

            }

            comboBox1.Items.Clear();

            foreach (Account acc in accountList.Values) {

                comboBox1.Items.Add(acc);

            }

            //select later
            //comboBox1.SelectedItem = a;

        }

        private void onClickCreateGenesisBlock(object sender, EventArgs e) {
            createGenesisBlock();
        }

        private void createGenesisBlock() {
            Block genesis = new Block(0);
            genesis.hashOfParticle = "genesis";
            genesis.giveSomeCoins(accountList["KelvinPetry"].publicKey, 100);
            Utility.storeFile(genesis, genesis.name + ".blk");
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
            p.hashToBlock = Utility.ComputeHash("" + latestBlock);

            particleName = "P" + (latestBlock + 1);

            Utility.storeFile(p, particleName + ".blk");

            updateTransactionPoolWindow();

            //create light particle
            Particle light = new Particle();
            string lightName = "L" + (latestBlock + 1);
            light.hashToBlock = Utility.ComputeHash("" + latestBlock);
            Utility.storeFile(light, lightName + ".blk");

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

            Particle p = Utility.loadFile<Particle>("P" + (latestBlock + 1) + ".blk");

            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash((latestBlock + 1));
            nextBlock.numberOfTransactions += p.allTransactions.Count;
            nextBlock.name = (latestBlock + 1);
            nextBlock.nonce = nonce;

            nextBlock.createCoins(accountList[minerAccountName]);

            //Store Block
            Utility.storeFile(nextBlock, (latestBlock + 1) + ".blk");

            analyseChain();

        }

        private void onClickStartMining(object sender, EventArgs e) {

            miningFlag = true;

            int i = 0;
            string hashFromLatestBlock = Utility.ComputeHash(latestBlock + "");
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

            int difficulty = miningDifficulty;

            Block b = Utility.loadFile<Block>("0.blk");

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

            while (File.Exists(i + ".blk")) {

                b = Utility.loadFile<Block>(i + ".blk");
                string hashOfBlock = Utility.ComputeHash(i + "");
                string hashOfBlockBefore = Utility.ComputeHash((i - 1) + "");

                string hashToProof = b.nonce + hashOfBlockBefore + b.allCoins[0].owner;
                string proofHash = Utility.getHashSha256(hashToProof);

                Console.WriteLine("i" + b.nonce + " hashFromlatest" + hashOfBlockBefore + " __");
                Console.WriteLine(proofHash + " proof? what");

                //checking nonce
                if (!Utility.verifyHashDifficulty(proofHash, difficulty)) {
                    Console.WriteLine("WRONG SHIT!");
                    break;
                }

                if (File.Exists("P" + i + ".blk")) {
                    //particle exists
                    Particle p = Utility.loadFile<Particle>("P" + i + ".blk");

                    //block points to particle
                    if (!Utility.ComputeHash("P" + i).Equals(b.hashOfParticle)) {
                        break;
                    }

                    //particle points to block before
                    if (!Utility.ComputeHash("" + (i - 1)).Equals(p.hashToBlock)) {
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

                } else if (File.Exists("L" + i + ".blk")) {

                    Particle p = Utility.loadFile<Particle>("L" + i + ".blk");

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

                File.Delete(i + ".blk");
                File.Delete("P" + i + ".blk");
                File.Delete("L" + i + ".blk");

                i--;
            }

            latestBlock = i;
            label5.Text = "Block " + latestBlock;
            label3.Text = "" + checkCoinBalance(accountList[comboBox1.Text].publicKey, latestBlock);

        }

        private int checkCoinBalance(string owner, int blockheight) {

            int coinbalance = 0;
            for (int i = 0; i <= blockheight; i++) {

                if (!File.Exists(i + ".blk")) {
                    break;
                }

                Block b = Utility.loadFile<Block>(i + ".blk");



                foreach (Block.Coin coin in b.allCoins) {
                    if (coin.owner.Equals(owner)) {
                        coinbalance += coin.amount;
                    }
                }

                if (i > 0) {
                    Particle p = Utility.loadFile<Particle>("P" + i + ".blk");

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

            string[] allFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.blk");
            foreach (string s in allFiles) {
                string[] splitted = s.Split('\\');
                string nameOfFile = splitted[splitted.Length - 1];

                if (!nameOfFile.Equals("0.blk"))
                    File.Delete(nameOfFile);
            }

            analyseChain();
        }

        private void onClickDeleteAllAccounts(object sender, EventArgs e) {

            string[] allFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.acc");
            foreach (string s in allFiles) {
                string[] splitted = s.Split('\\');
                string nameOfFile = splitted[splitted.Length - 1];
                File.Delete(nameOfFile);
            }

            updateAccountList();
        }

        private void onClickOpenSettings(object sender, EventArgs e) {
            Settings s = new Settings();
            s.ShowDialog();
        }

        PeerNameRegistration pnReg;

        private void onClickStartPNRP(object sender, EventArgs e) {

            // Creates a secure (not spoofable) PeerName

            PeerName peerName = new PeerName("MikesWebServer", PeerNameType.Secured);
            pnReg = new PeerNameRegistration();

            pnReg.PeerName = peerName;
            pnReg.Port = 80;

            //OPTIONAL

            //The properties set below are optional.  You can register a PeerName without setting these properties

            pnReg.Comment = "up to 39 unicode char comment";
            pnReg.Data = System.Text.Encoding.UTF8.GetBytes("A data blob associated with the name");

            /*
             * OPTIONAL
             *The properties below are also optional, but will not be set (ie. are commented out) for this example
             *pnReg.IPEndPointCollection = // a list of all {IPv4/v6 address, port} pairs to associate with the peername
             *pnReg.Cloud = //the scope in which the name should be registered (local subnet, internet, etc)
            */


            //Starting the registration means the name is published for others to resolve

            pnReg.Start();
            textBox1.Text = peerName.ToString();
            Console.WriteLine();

        }

        private void onClickEndPNRP(object sender, EventArgs e) {
            pnReg.Stop();
            label1.Text = "Stopped Peer to peer";
        }

        private void onClickDiscoverPNRP(object sender, EventArgs e) {

            // create a resolver object to resolve a peername
            PeerNameResolver resolver = new PeerNameResolver();
            // the peername to resolve must be passed as the first command line argument to the application

            PeerName peerName = new PeerName(textBox1.Text.Trim());
            // resolve the PeerName - this is a network operation and will block until the resolve completes

            PeerNameRecordCollection results = resolver.Resolve(peerName);
            // Display the data returned by the resolve operation

            Console.WriteLine("Results for PeerName: {0}", peerName);
            Console.WriteLine();
            label1.Text = "Result working! Found:" + results.Count;

            int count = 1;
            foreach (PeerNameRecord record in results) {

                Console.WriteLine("Record #{0} results...", count);
                Console.Write("Comment:");

                if (record.Comment != null) {
                    Console.Write(record.Comment);
                }

                Console.WriteLine();
                Console.Write("Data:");

                if (record.Data != null) {
                    Console.Write(System.Text.Encoding.ASCII.GetString(record.Data));
                }

                Console.WriteLine();
                Console.WriteLine("Endpoints:");

                foreach (IPEndPoint endpoint in record.EndPointCollection) {

                    Console.WriteLine("\t Endpoint:{0}", endpoint);
                    Console.WriteLine();

                }
                count++;
            }
        }

    }
}
