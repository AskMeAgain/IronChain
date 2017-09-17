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
        public string hashOfBlockBefore { get; set; }
        public string hashOfExtendedParticle { get; set; }

        public Block() {
            name = 0;
            hashOfParticle = "";
        }


        public Block(int height, string nonc, int diff, string key) {
            name = height;
            nonce = nonc;
            difficulty = diff;
            minerAddress = key;

            addHash();
        }

        private void addHash() {
            hashOfParticle = Utility.ComputeHash(Form1.instance.globalChainPath + "P" + name);
            hashOfBlockBefore = Utility.ComputeHash(Form1.instance.globalChainPath + (name - 1));
            hashOfExtendedParticle = Utility.ComputeHash(Form1.instance.globalChainPath + "E" + name);
        }

    }
}
