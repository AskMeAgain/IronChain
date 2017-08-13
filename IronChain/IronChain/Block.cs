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

        public Block() {
            name = 0;
            hashOfParticles = new List<string>();

        }

        public Block(int n) {
            name = n;
            hashOfParticles = new List<string>();
        }

        public void addHash(string hash) {

            hashOfParticles.Add(hash);

        }

    }
}
