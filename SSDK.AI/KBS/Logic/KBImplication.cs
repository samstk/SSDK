using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Logic
{
    /// <summary>
    /// A logical connective that is asserted true on P->Q when P is false, or Q.
    /// When using probabilities
    /// </summary>
    public class KBImplication : KBFactor
    {
        /// <summary>
        /// Gets the symbol or logical sentence that implies a certain assertion.
        /// </summary>
        public KBFactor Condition { get; private set; }

        /// <summary>
        /// Gets the symbol or logical sentence that is implied by the condition.
        /// </summary>
        public KBFactor Implication { get; private set; }

        public KBImplication(KBFactor condition,KBFactor implication)
        {
            Condition = condition;
            Implication = implication;
        }
        public override bool Holds()
        {
            return !Condition.Holds() || Implication.Holds();
        }


        public override KBFactor HasConflict()
        {
            KBFactor r = null;
            if ((r = Condition.HasConflict()) as object != null) return r;
            if ((r = Implication.HasConflict()) as object != null) return r;

            return Solved && Assertion && !Holds() ? this : null;
        }

        public override string ToString()
        {
            return $"({Condition} -> {Implication})";
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> condSymbols = Condition.GetSymbols();
            HashSet<KBSymbol> impSymbols = Implication.GetSymbols();

            HashSet<KBSymbol> set = new HashSet<KBSymbol>();
            foreach (KBSymbol symbol in condSymbols) set.Add(symbol);
            foreach (KBSymbol symbol in impSymbols) set.Add(symbol);
            return set;
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { Condition, Implication };
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            if (child.Equals(Condition)) return KBSolveType.NoSolution; // Cannot solve for condition, p -> q only affects q on different values of p
            return Solved && Condition.Solved && Condition.Holds() ? KBSolveType.SolveTrue : KBSolveType.NoSolution;
        }
        // Can only solve children if condition has been solved and it is true (i.e. implication is true)

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = Condition.SolveAssertion(kb, this) + Implication.SolveAssertion(kb, this);

            if (!Solved)
            {
                KBSolveType type = parent as object== null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
          
                if (type == KBSolveType.SolveTrue)
                {
                    SolveAssertTrue(kb); changes++;
                }
                else if (type == KBSolveType.SolveFalse)
                {
                    SolveAssertFalse(kb); changes++;
                }
            }
            

            return changes;
        }

        public override int SolveProbability(KB kb, KBFactor parent)
        {
            int changes = Condition.SolveProbability(kb, this) + Implication.SolveProbability(kb, this);

            // 80% of the times it rained in the afternoon, it was cloudy in the morning.
            // 10% of days it rained in the afternoon.
            // 40% of days it was cloudy in the morning.

            // Given the information, what is the probability that it is cloudy in the morning, and raining in the afternoon.
            // 1. cloudy = 1: general 0.4
            // 2. raining = ?: general 0.1
            // 3. raining >= cloudy 80% of the time.
            // 
            // To answer this problem, visualise the set
            //
            // correct adjusting for minimum cases for the implication (50 entries):
            // cloudy  : T T T T T T T T T T | T T T T T T T T T T | F F F F F F F F F F | F F F F F F F F F F | F F F F F F F F F
            // raining : T T T T F F F F F F | F F F F F F F F F F | T F F F F F F F F F | F F F F F F F F F F | F F F F F F F F F
            //
            // we can see here that raining & cloudy occurs 4/50 times (8%)
            // how would we work it out without populating the table ?
            //
            // First: the probabilities:
            // cloudy = 0.4
            // raining = 0.1
            // on so on...

            // suppose we want to know raining given cloudy.
            // what is our estimated probability vs general probability?
            // 
            // In our implication, we know that raining >= cloudy 80% of the time.
            // i.e. raining * 0.8 = 8% of the samples for cloudy.
            // however, cloudy was 40% of the samples.
            //
            // if we want the chance it is not raining given cloudy, then eliminate 8% of the samples (i.e. 40% - 8%).
            // if we want the chance it is raining given cloudy, then simply invert this figure.

            if (ProbabilitySolved)
            {
                if(Condition.ProbabilitySolved && Implication.UpdateProbability(Condition.Probability * Probability))
                {
                    changes++;
                }
                if(Condition.GeneralProbability > 0 && Implication.GeneralProbability > 0
                    && Condition.IsOfSameSet(Implication))
                {
                    // Use bayesian rule
                    if(Condition.UpdateProbability((Probability * Condition.GeneralProbability) / Implication.GeneralProbability))
                    {
                        changes++;
                    }
                }
            }

            return changes;
        }

        public override KBFactor Simplify()
        {
            Condition = Condition.Simplify();
            Implication = Implication.Simplify();
            return this;
        }
    }
}
