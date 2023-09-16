using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Arithmetic
{
    public sealed class KBNull : KBSymbol
    {
        public KBNull()
        {

        }
        public override bool Holds()
        {
            return true;
        }
        public override KBFactor Calculate()
        {
            return this;
        }
        public override KBFactor Apply(char op, params KBFactor[] terms)
        {
            return this;
        }
    }
}
