using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class Block {

        [Serializable]
        public struct Coin {
            public string owner { get; set; }
            public string name { get; set; }

            public Coin(string n, string o) {
                owner = o;
                name = n;
            }
        };

        public int name { get; set; }
        public string hashOfParticle { get; set; }
        public string hashOfLightParticle { get; set; }

        public int numberOfTransactions { get; set; }
        public List<Coin> allCoins { get; set; }

        public Block() {
            name = 0;
            hashOfParticle = "";
            numberOfTransactions = 0;
            allCoins = new List<Coin>();
        }

        public Block(int n) {
            name = n;
            hashOfParticle = "";
            numberOfTransactions = 0;
            allCoins = new List<Coin>();
        }

        public void addHash(int i) {
            hashOfParticle = Utility.ComputeHash("P"+ (i+1));
            hashOfLightParticle = Utility.ComputeHash("L" + (i+1));

        }

        public void createCoins(string minerAddress) {

            for (int i = 0; i < numberOfTransactions; i++) {
                string name = "Coin_" + (Form1.instance.latestBlock+1)+"_"+i;
                allCoins.Add(new Coin(name,minerAddress));
            }

            allCoins.Add(new Coin("Bonus1", minerAddress));
            allCoins.Add(new Coin("Bonus2", minerAddress));
            allCoins.Add(new Coin("Bonus3", minerAddress));

        }

    }
}
