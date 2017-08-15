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

        public Form1() {
            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            calculateLatestBlock();

            textBox4.Text = "KelvinPetry";
            minerAddress = textBox4.Text;

        }

        private void createGenesisBlock(object sender, EventArgs e) {
            Block genesis = new Block(0);
            genesis.hashOfParticles = new List<string>() { "genesis" };
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

        private List<string> createParticles() {

            List<string> nameList = new List<string>();

            string particleName = "";
            int i = 0;



            //transaction particle
            Particle p = new Particle();
            while (TransactionPool.Count > 0) {

                p.addTransaction(TransactionPool[0]);
                TransactionPool.RemoveAt(0);
            }

            //add hash to block before
            p.hashToBlock = Utility.ComputeHash("" + latestBlock);

            particleName = "P" + (latestBlock + 1);

            Utility.storeFile(p, particleName);
            nameList.Add(particleName);

            updateTransactionPoolWindow();

            addToLog("Particle created:" + nameList.Count);


            //create light particle
            Particle light = new Particle();
            string lightName = "L" + (latestBlock + 1);
            light.hashToBlock = Utility.ComputeHash("" + latestBlock);
            Utility.storeFile(light, lightName);
            nameList.Add(lightName);
            addToLog("Light Particle created:" + nameList.Count);

            return (nameList);

        }

        private void mineNextBlock(object sender, EventArgs e) {

            calculateLatestBlock();

            Block nextBlock = new Block();
            List<string> namesOfParticle = createParticles();

            foreach (string s in namesOfParticle) {
                Particle p = Utility.loadFile<Particle>(s);

                //GET HASHES NOW INTO BLOCK
                nextBlock.addHash(Utility.ComputeHash(s));
                nextBlock.numberOfTransactions += p.allTransactions.Count;
            }



            addToLog("");

            addToLog("MINER CREATED PARTICLES");

            nextBlock.createCoins(minerAddress);

            //Store Block
            Utility.storeFile(nextBlock, (latestBlock + 1) + "");
            calculateLatestBlock();

        }

        private void onClickCalculateLatestBlock(object sender, EventArgs e) {
            calculateLatestBlock();
        }

        private void calculateLatestBlock() {
            int i = 1;

            while (File.Exists(i + "") && File.Exists("P" + 1 + "_0")) {
                Console.WriteLine("yeha");
                i++;
            }

            addToLog("Latest Block: " + (i - 1));

            latestBlock = i - 1;
        }

        private void printHashes() {

            textBox1.Text = "";
            addToLog("Block    0: " + Utility.ComputeHash("0"));
            addToLog("");

            calculateLatestBlock();

            for (int i = 1; i <= latestBlock; i++) {
                addToLog("Block    " + i + ": " + Utility.ComputeHash(i + ""));

                if (File.Exists("P" + i) || File.Exists("L"+i)) {
                    addToLog("Particle " + i + ": " + Utility.ComputeHash("P" + i));
  
                }

                addToLog("");

            }

            addToLog("----FINISHED----");

            addToLog2("----PRINTLINK----");
            textBox3.Text = "";

            for (int i = 1; i <= latestBlock; i++) {

                int ii = 0;

                Block b = Utility.loadFile<Block>(i + "");

                foreach (string s in b.hashOfParticles) {
                    addToLog2("Block:" + i + " hashes to " + s);
                }

                addToLog2("");

                if (File.Exists("P" + i) || File.Exists("L" + i)) {

                    Particle p = Utility.loadFile<Particle>("P" + i);

                    addToLog2("Particle:" + i + " hashes to " + p.hashToBlock);
                    ii++;
                }

                addToLog2("");

            }

            addToLog2("----FINISHED----");

        }

        private void onClickPrintChain(object sender, EventArgs e) {
            printHashes();
        }

        //simple means we just check for the first particle
        private void onClickVerifyChainSimple(object sender, EventArgs e) {

            textBox1.Text = "";
            calculateLatestBlock();
            addToLog("Block chain is legit: " + verfiyChainSimple() + "   Latest Block:" + latestBlock);

        }

        private bool verfiyChainSimple() {
            int i = 1;

            while (File.Exists(i + "")) {

                Block b = Utility.loadFile<Block>(i + "");
                Particle p = Utility.loadFile<Particle>("P" + i + "_0");

                Console.WriteLine(i);
                if (!b.hashOfParticles[0].Equals(Utility.ComputeHash("P" + i + "_0")) || !p.hashToBlock.Equals(Utility.ComputeHash((i - 1) + ""))) {
                    return false;
                }
                i++;
            }
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
            calculateLatestBlock();
            addresse = addresse.Trim();

            for (int i = 0; i < latestBlock; i++) {

                Block b = Utility.loadFile<Block>(1 + "");

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
    }
}
