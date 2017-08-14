using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class Block {

        public int name;
        public List<string> hashOfParticles;
        public int numberOfTransactions;

        public string[] allCoins;

        public Block() {
            name = 0;
            hashOfParticles = new List<string>();
            numberOfTransactions = 0;
        }

        public Block(int n) {
            name = n;
            hashOfParticles = new List<string>();
            numberOfTransactions = 0;
            allCoins = new string[0];
        }

        public void addHash(string hash) {
            hashOfParticles.Add(hash);
        }

        public void createCoins(string minerAddress) {

            List<string> temp = new List<Coin>();

            for (int i = 0; i < numberOfTransactions; i++) {
                string name = "Coin_" + (Form1.instance.latestBlock+1)+"_"+i;
                temp.Add(new Coin(name,minerAddress));
            }

        }

    }
}
