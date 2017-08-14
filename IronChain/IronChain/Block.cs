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
        public List<string> hashOfParticles { get; set; }
        public int numberOfTransactions { get; set; }
        public List<Coin> allCoins { get; set; }

        public Block() {
            name = 0;
            hashOfParticles = new List<string>();
            numberOfTransactions = 0;
            allCoins = new List<Coin>();
        }

        public Block(int n) {
            name = n;
            hashOfParticles = new List<string>();
            numberOfTransactions = 0;
            allCoins = new List<Coin>();
        }

        public void addHash(string hash) {
            hashOfParticles.Add(hash);
        }

        public void createCoins(string minerAddress) {

            for (int i = 0; i < numberOfTransactions; i++) {
                string name = "Coin_" + (Form1.instance.latestBlock+1)+"_"+i;
                allCoins.Add(new Coin(name,minerAddress));
            }

            Console.WriteLine("lulaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" + allCoins.Count);
        }

    }
}
