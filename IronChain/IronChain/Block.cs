using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class Block {

        public string minerAddress;
        public int difficulty { get; set; }
        public string nonce { get; set; }
        public int name { get; set; }
        public string hashOfParticle { get; set; }
        public string hashOfLightParticle { get; set; }
        public string hashOfBlockBefore { get; set; }

        public Block() {
            name = 0;
            hashOfParticle = "";
        }

        public Block(int n) {
            name = n;
            hashOfParticle = "";
        }

        public void addHash(int i) {
            hashOfParticle = Utility.ComputeHash(Form1.instance.globalChainPath + "P" + i);
            hashOfBlockBefore = Utility.ComputeHash(Form1.instance.globalChainPath + (i - 1));
        }

        public void createCoins(Account acc) {
            minerAddress = acc.publicKey;
        }

    }
}
