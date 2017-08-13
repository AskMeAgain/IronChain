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

        public Form1() {
            InitializeComponent();
            instance = this;
            TransactionPool = new List<Transaction>();
        }

        private void onClickStoreFile(object sender, EventArgs e) {

            Block b1 = new Block(3333);

            Utility.storeFile(b1, "test");

        }

        private void onClickLoadFile(object sender, EventArgs e) {

            Block b = Utility.loadFile<Block>("test");

            if (b != null) {
                addToLog("Block loaded: " + b.name);
            } else {
                addToLog("Failure loading block!");

            }

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

        private void onClickCreateParticles(object sender, EventArgs e) {
            createParticlesFromTP();

        }

        private string createParticlesFromTP() {
            Particle p = new Particle();

            while (TransactionPool.Count > 0) {

                p.addTransaction(TransactionPool[0]);
                TransactionPool.RemoveAt(0);
            }

            updateTransactionPoolWindow();

            addToLog("Particle created:" + p.allTransactions.Count);

            Utility.storeFile(p, "Particle");

            return "Particle";

        }

        private void mineNextBlock(object sender, EventArgs e) {

            Block nextBlock = new Block();
            string namesOfParticle = createParticlesFromTP();

            Particle p = Utility.loadFile<Particle>(namesOfParticle);

            addToLog("MINER CREATED PARTICLE WITH " + p.allTransactions.Count);

            //SENDING BLOCKS BLABLA

            //GET HASHES NOW INTO BLOCK
            nextBlock.addHash(ComputeHash(namesOfParticle));

            Utility.storeFile(nextBlock, nextBlock.name+"");

        }

        private string ComputeHash(string filename) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(filename)) {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }

        private void ShowParticles(object sender, EventArgs e) {

            Particle p = Utility.loadFile<Particle>("Particle");

            addToLog(p.allTransactions.Count + " inside Particle");

        }
    }
}
