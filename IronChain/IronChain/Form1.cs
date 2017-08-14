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
        private int latestBlock;

        public Form1() {
            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
            calculateLatestBlock();
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

        private string createParticles() {

            Particle p = new Particle();

            while (TransactionPool.Count > 0) {
                p.addTransaction(TransactionPool[0]);
                TransactionPool.RemoveAt(0);
            }


            //add hash to block before
            p.hashToBlock = Utility.ComputeHash("" + latestBlock);

            updateTransactionPoolWindow();

            addToLog("Particle created:" + p.allTransactions.Count);

            Utility.storeFile(p, "P" + (latestBlock+1));

            return ("P" + (latestBlock+1));

        }

        private void mineNextBlock(object sender, EventArgs e) {

            calculateLatestBlock();

            Block nextBlock = new Block();
            string namesOfParticle = createParticles();

            Particle p = Utility.loadFile<Particle>(namesOfParticle);



            p.hashToBlock = Utility.ComputeHash(latestBlock + "");


            addToLog("");

            addToLog("MINER CREATED PARTICLES");

            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash(Utility.ComputeHash(namesOfParticle));

            //Store Block
            Utility.storeFile(nextBlock, (latestBlock + 1) + "");
            calculateLatestBlock();

        }

        private void onClickCalculateLatestBlock(object sender, EventArgs e) {
            calculateLatestBlock();
        }

        private void calculateLatestBlock() {
            int i = 0;

            while (File.Exists(i + "")) {
                Console.WriteLine("yeha");
                i++;
            }

            addToLog("Head calculated: " + (i - 1));

            latestBlock =  i - 1;
        }

        private void printHashes() {

            textBox1.Text = "";

            for (int i = 0; i < latestBlock; i++) {
                addToLog("Block    " + i + ": " + Utility.ComputeHash(i + ""));
                addToLog("Particle " + (i+1) + ": " + Utility.ComputeHash("P" + (i+1)));
                addToLog("");
            }

            addToLog("----FINISHED----");
            addToLog("");
            addToLog("");
            addToLog("");
            addToLog("----PRINTLINK----");

            for (int i = 1; i <= latestBlock; i++) {

                Particle p = Utility.loadFile<Particle>("P" + i);
                Block b = Utility.loadFile<Block>(i + "");

                addToLog("Block:" + i + " hashes to " + b.hashOfParticles[0]);
                addToLog("Particle:" + i + " hashes to " + p.hashToBlock);
                addToLog("");
            }

            addToLog("----FINISHED----");
            addToLog("Latest Block: " + latestBlock);


        }

        private void onClickPrintChain(object sender, EventArgs e) {
            printHashes();
        }

        private void onClickVerifyChain(object sender, EventArgs e) {

            textBox1.Text = "";
            calculateLatestBlock();
            addToLog("Block chain is legit: " + verifyChain() + "   Latest Block:" + latestBlock);        

        }

        private bool verifyChain() {
            int i = 1;

            while (File.Exists(i + "")) {

                Block b = Utility.loadFile<Block>(i + "");
                Particle p = Utility.loadFile<Particle>("P"+i);

                Console.WriteLine(i);
                if (!b.hashOfParticles[0].Equals(Utility.ComputeHash("P" + i)) || !p.hashToBlock.Equals(Utility.ComputeHash((i-1) + ""))) {
                    return false;
                }
                i++;
            }

            return true;
        }

        private void clearLog(object sender, EventArgs e) {
            textBox1.Text = "";
        }
    }
}
