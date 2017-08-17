using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IronChain {
    public partial class Form1 : Form {

        public static Form1 instance;
        public static List<Transaction> TransactionPool;
        public string minerAddress;
        bool miningFlag = true;
        Dictionary<string, int> transHistory;
        public int latestBlock = 0;
        int coinCounter = 0;

        public Dictionary<String, Account> accountList;

        public Form1() {
            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            accountList = new Dictionary<string, Account>();

            textBox4.Text = "KelvinPetry";
            minerAddress = textBox4.Text;

            if (!File.Exists("0.blk")) {
                createGenesisBlock();
            }

            Block b = Utility.loadFile<Block>("0");

            foreach (Block.Coin coin in b.allCoins) {
                if (coin.owner.Equals(minerAddress)) {
                    coinCounter++;
                }
            }


            //create account before
            Account thisAccount = new Account("KelvinPetry", 0);
            accountList.Add(thisAccount.publicKey, thisAccount);
            comboBox1.Items.Add(thisAccount);
            comboBox1.SelectedItem = thisAccount;

            analyseChain(comboBox1.SelectedItem.ToString());

        }

        private void onClickCreateGenesisBlock(object sender, EventArgs e) {
            createGenesisBlock();
        }

        private void createGenesisBlock() {
            Block genesis = new Block(0);
            genesis.hashOfParticle = "genesis";
            genesis.giveSomeCoins(minerAddress, 100);
            Utility.storeFile(genesis, genesis.name + "");
            addToLog("Genesis Block created");
        }

        public void addToLog(string s) {

            String text = textBox1.Text;
            text += s + Environment.NewLine;
            textBox1.Text = @text;

        }

        public void addToLog2(string s) {

            String text = textBox3.Text;
            text += s + Environment.NewLine;
            textBox3.Text = @text;

        }

        private void onClickCreateTransactions(object sender, EventArgs e) {
            Random r = new Random();
            for (int i = 0; i < 10; i++) {
                Transaction t = new Transaction(r, minerAddress);
                Console.WriteLine(t.toString());
                TransactionPool.Add(t);
            }
            updateTransactionPoolWindow();
            addToLog("10 Transactions created");

        }

        private void updateTransactionPoolWindow() {

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

                if (verifyTransaction(trans)) {
                    p.addTransaction(trans);
                }

            }

            //add hash to block before
            p.hashToBlock = Utility.ComputeHash("" + latestBlock);

            particleName = "P" + (latestBlock + 1);

            Utility.storeFile(p, particleName);

            updateTransactionPoolWindow();


            //create light particle
            Particle light = new Particle();
            string lightName = "L" + (latestBlock + 1);
            light.hashToBlock = Utility.ComputeHash("" + latestBlock);
            Utility.storeFile(light, lightName);

        }

        private bool verifyTransaction(Transaction trans) {

            /*
            string owner = trans.owner;

            int num = calculateCoinNumberOfAddress(owner);

            if (!transHistory.ContainsKey(trans.owner)) {
                transHistory.Add(trans.owner, 0);
            }

            if (num - (trans.amount + transHistory[trans.owner]) > 0) {
                transHistory[owner] += trans.amount;
                return true;
            } else {
                return false;
            }*/

            return true;

        }

        private void mineNextBlock(string nonce) {

            Block nextBlock = new Block();
            createParticles();

            Particle p = Utility.loadFile<Particle>("P" + (latestBlock + 1));

            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash(latestBlock);
            nextBlock.numberOfTransactions += p.allTransactions.Count;
            nextBlock.name = (latestBlock + 1);
            nextBlock.nonce = nonce;

            nextBlock.createCoins(minerAddress);

            //Store Block
            Utility.storeFile(nextBlock, (latestBlock + 1) + "");

            addToLog("");

            addToLog("MINER CREATED BLOCK");
            Console.WriteLine(comboBox1.SelectedIndex + " <<<< ");
            analyseChain(comboBox1.SelectedItem.ToString());

        }

        private void onMiningAddressChanged(object sender, EventArgs e) {
            minerAddress = textBox4.Text;
        }

        private void makeTransaction(object sender, EventArgs e) {

            string receiver = textBox5.Text;
            int amount = Convert.ToInt32(textBox6.Text);

            Transaction t = new Transaction();
            t.owner = minerAddress;
            t.receiver = receiver;
            t.amount = amount;
            //TODO SIGN IT;

            TransactionPool.Add(t);
            updateTransactionPoolWindow();

        }

        private void onClickStartMining(object sender, EventArgs e) {

            addToLog("MINING STARTED");
            miningFlag = true;

            int i = 0;
            string hashFromLatestBlock = Utility.ComputeHash(latestBlock + "");
            int difficulty = Convert.ToInt32(textBox7.Text);

            while (miningFlag) {

                string hash = Utility.getHashSha256(i + "" + hashFromLatestBlock);


                if (Utility.verifyHashDifficulty(hash, difficulty)) {
                    Console.WriteLine(latestBlock + " HASH FOUND " + hashFromLatestBlock);
                    mineNextBlock(i + "");
                    break;
                }

                i++;
            }

        }

        private void onClickStopMining(object sender, EventArgs e) {
            miningFlag = false;
            addToLog("MINING STOPPED");
        }

        private void onClickAnalyseChain(object sender, EventArgs e) {
            analyseChain(comboBox1.SelectedItem.ToString());
        }

        private void analyseChain(string accountIndex) {

            Console.WriteLine(accountIndex + "<<< index");

            int difficulty = Convert.ToInt32(textBox7.Text);

            Block b = Utility.loadFile<Block>("0");
            int i = (accountList[accountIndex].analysedBlock) + 1;

            if (i == 1) {
                foreach (Block.Coin coin in b.allCoins) {
                    if (coin.owner.Equals(accountList[accountIndex].publicKey)) {
                        accountList[accountIndex].coinCounter++;
                    }
                }
            }

            while (File.Exists(i + ".blk")) {
                Console.WriteLine(i + " <<<<<<< i");
                b = Utility.loadFile<Block>(i + "");
                string hashOfBlock = Utility.ComputeHash(i + "");
                string proofHash = Utility.getHashSha256(b.nonce + "" + Utility.ComputeHash((i - 1) + ""));

                //checking nonce
                if (!Utility.verifyHashDifficulty(proofHash, difficulty)) {
                    break;
                }

                if (File.Exists("P" + i + ".blk")) {
                    //particle exists
                    Particle p = Utility.loadFile<Particle>("P" + i);

                    //block points to particle
                    if (!Utility.ComputeHash("P" + i).Equals(b.hashOfParticle)) {
                        break;
                    }

                    //particle points to block before
                    if (!Utility.ComputeHash("" + (i - 1)).Equals(p.hashToBlock)) {
                        break;
                    }


                    //after verifying the block, we now count the coins

                    foreach (Block.Coin coin in b.allCoins) {
                        if (coin.owner.Equals(accountList[accountIndex].publicKey)) {
                            accountList[accountIndex].coinCounter++;
                        }
                    }

                    foreach (Transaction trans in p.allTransactions) {
                        if (trans.receiver.Equals(accountList[accountIndex].publicKey)) {
                            accountList[accountIndex].coinCounter += trans.amount;
                        }

                        if (trans.owner.Equals(accountList[accountIndex].publicKey)) {
                            accountList[accountIndex].coinCounter -= trans.amount;
                        }

                    }             

                } else if (File.Exists("L" + i + ".blk")) {

                    Particle p = Utility.loadFile<Particle>("L" + i);

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
                Console.WriteLine(i);
                i++;
            }

            i--;

            label3.Text = accountList[accountIndex].coinCounter + " Coins";

            accountList[accountIndex].analysedBlock = i;

            latestBlock = i;
            label5.Text = "Latest Block" + latestBlock;

        }

        private void onAccountChanged(object sender, EventArgs e) {
            analyseChain(comboBox1.SelectedItem.ToString());
        }

        private void onClickAddAccount(object sender, EventArgs e) {
            Account a = new Account(textBox3.Text.Trim(),0);
            accountList.Add(a.publicKey, a);
            comboBox1.Items.Add(a);
            comboBox1.SelectedItem = a;
            analyseChain(comboBox1.SelectedItem.ToString());
        }

    }
}
