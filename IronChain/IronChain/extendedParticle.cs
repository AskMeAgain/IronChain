using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class ExtendedParticle {

        public List<string> proof;

        public ExtendedParticle() {
            proof = new List<string>();
        }

    }
}
