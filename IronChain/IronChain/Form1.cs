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
        public int latestBlock;
        public string minerAddress;
        bool miningFlag = true;


        public Form1() {
            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            verifyChain();

            textBox4.Text = "KelvinPetry";
            minerAddress = textBox4.Text;

        }

        private void createGenesisBlock(object sender, EventArgs e) {
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
                Transaction t = new Transaction(r);
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

        Dictionary<string, int> transHistory;

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
            }

        }

        private void onClickMineNextBlock(object sender, EventArgs e) {
            mineNextBlock("LOL");

        }

        private void mineNextBlock(string nonce) {
            verifyChain();

            Block nextBlock = new Block();
            createParticles();


            Particle p = Utility.loadFile<Particle>("P" + (latestBlock + 1));
            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash(latestBlock);
            nextBlock.numberOfTransactions += p.allTransactions.Count;
            nextBlock.name = (latestBlock + 1);
            nextBlock.nonce = nonce;


            addToLog("");

            addToLog("MINER CREATED PARTICLES");

            nextBlock.createCoins(minerAddress);

            //Store Block
            Utility.storeFile(nextBlock, (latestBlock + 1) + "");

            calculateCoinNumberOfAddress(minerAddress);
        }

        private void printHashes() {

            textBox1.Text = "";

            //display all hashes:
            int i = 1;
            while (File.Exists(i + ".blk")) {

                int counter = 0;

                if (File.Exists("P" + i + ".blk")) {
                    addToLog2("P" + i + ".blk = " + Utility.ComputeHash("P" + i));
                } else {
                    counter++;
                }

                if (File.Exists("L" + i + ".blk")) {
                    addToLog2("L" + i + ".blk = " + Utility.ComputeHash("L" + i));
                } else {
                    counter++;
                }

                if (counter == 2) {
                    break;
                }

                i++;
            }

        }

        private void onClickPrintChain(object sender, EventArgs e) {
            printHashes();
        }

        //simple means we just check for the first particle
        private void onClickVerifyChain(object sender, EventArgs e) {

            textBox1.Text = "";
            verifyChain();

        }

        private bool verifyChain() {

            int i = 1;
            int difficulty = Convert.ToInt32(textBox7.Text);

            while (File.Exists(i + ".blk")) {

                Block b = Utility.loadFile<Block>(i + "");
                string hashOfBlock = Utility.ComputeHash(i + "");

                string proofHash = Utility.getHashSha256(b.nonce + "" + Utility.ComputeHash(i + ""));

                if (Utility.verifyHashDifficulty(proofHash,difficulty)) {
                    addToLog2("Block" + i + " has correct nonce:" + proofHash);
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

            addToLog("VERIFIED UNTIL" + i);
            latestBlock = i;
            return true;
        }

        private void clearLog(object sender, EventArgs e) {
            textBox1.Text = "";
            textBox3.Text = "";
        }

        private void onMiningAddressChanged(object sender, EventArgs e) {
            minerAddress = textBox4.Text;
        }

        private void onClickCalculateCoins(object sender, EventArgs e) {
            calculateCoinNumberOfAddress(minerAddress);
        }

        private int calculateCoinNumberOfAddress(string addresse) {

            int coinCounter = 0;

            verifyChain();
            addresse = addresse.Trim();

            for (int i = 0; i <= latestBlock; i++) {

                Block b = Utility.loadFile<Block>(i + "");

                if (File.Exists("P" + i + ".blk")) {
                    Particle p = Utility.loadFile<Particle>("P" + i);


                    foreach (Transaction trans in p.allTransactions) {
                        if (trans.receiver.Equals(addresse)) {
                            coinCounter += trans.amount;
                        }

                        if (trans.owner.Equals(addresse)) {
                            coinCounter -= trans.amount;
                        }

                    }
                }

                foreach (Block.Coin coin in b.allCoins) {
                    if (coin.owner.Equals(addresse)) {
                        coinCounter++;
                    }
                }
            }

            addToLog("Your Coins:" + coinCounter);
            label3.Text = coinCounter + " Coins";

            return coinCounter;
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

            while (miningFlag ) {

                string hash = Utility.getHashSha256(i + "" + hashFromLatestBlock);

                if (Utility.verifyHashDifficulty(hash,difficulty)) {
                    Console.WriteLine(hash + " hash = " + i);
                    mineNextBlock(i+"");
                    break;
                }

                i++;
            }

        }

        private void onClickStopMining(object sender, EventArgs e) {
            miningFlag = false;
            addToLog("MINING STOPPED");
        }
    }
}
