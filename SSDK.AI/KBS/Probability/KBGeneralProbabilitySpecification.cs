using SSDK.AI.KBS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Probability
{
    /// <summary>
    /// A general probability specification onto a symbol or connective.
    /// Unlike the probability specification, any assertions made to this do not enforce
    /// that probability to the variable, and will never be reset.
    /// 
    /// Add using directive for static KBProbabilitySpecification for quick use of GP function.
    /// e.g. GP(rain) = 0.98
    /// </summary>
    public class KBGeneralProbabilitySpecification : KBFactor
    {
        /// <summary>
        /// The symbol that this general probability specifies.
        /// </summary>
        public KBSymbol On { get; private set; }

        /// <summary>
        /// The probability value that is defined (0-1).
        /// </summary>
        public KBFactor Value { get; private set; }

        /// <summary>
        /// The set which the general probability is defined on
        /// </summary>
        public KBSymbol Set { get; private set; }

        public KBGeneralProbabilitySpecification(KBSymbol on, KBSymbol set, KBFactor val)
        {
            On = on;
            Value = val;
            Set = set;
        }

        /// <summary>
        /// Creates a probability specification
        /// </summary>
        /// <param name="factor">the factor to specify the probability on</param>
        /// <param name="value">a value between 0 and 1</param>
        /// <returns>the specification for the probability of the factor</returns>
        public static KBProbabilitySpecification P(KBFactor factor, KBFactor value)
        {
            return new KBProbabilitySpecification(factor, value);
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> set = new HashSet<KBSymbol>();
            foreach (KBSymbol symbol in On.GetSymbols())
                set.Add(symbol);
            foreach (KBSymbol symbol in Value.GetSymbols())
                set.Add(symbol);
            return set;
        }
        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { On, Value };
        }

        public override bool Holds()
        {
            KBNumericSymbol probability = Value.Calculate() as KBNumericSymbol;
            return probability as object != null && On.Probability == probability.Number;
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            // No child solving.
            return KBSolveType.NoSolution;
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = 0;

            if (!Solved)
            {
                KBNumericSymbol probability = Value.Calculate() as KBNumericSymbol;
                if (probability as object == null) return changes;

                // We can solve a symbol classification when either ~class or class appears in the about list.
                KBSolveType solveType = parent as object == null ? KBSolveType.SolveTrue
                    : parent.CanSolveForChild(kb, this);

                if (solveType == KBSolveType.NoSolution)
                {
                    // Perhaps the property already exists on the target.
                }
                else if (solveType == KBSolveType.SolveTrue)
                {
                    SolveAssertTrue(kb); changes++;
                }
                else if (solveType == KBSolveType.SolveFalse)
                {
                    SolveAssertFalse(kb); changes++;
                }
            }

            return changes;
        }

        public override int SolveProbability(KB kb, KBFactor parent)
        {
            KBNumericSymbol probability = Value.Calculate() as KBNumericSymbol;
            if (probability as object != null)
            {
                int changes = On.SolveProbability(kb, parent);
                double lastProbability = On.Probability;

                // Assume the general probability is set correctly.
                double newGeneralProbability = Math.Max(On.GeneralProbability, probability.Number.ToDouble());
                if(newGeneralProbability != On.GeneralProbability)
                {
                    On.GeneralProbability = newGeneralProbability;
                    Set.UpdateGeneralProbability(On, On.GeneralProbability);
                    On.AddToSet(Set);
                    changes++;
                }
                if (!ProbabilitySolved)
                {
                    On.ProbabilitySolved = true;
                    On.Probability = On.GeneralProbability;
                    return changes + 1;
                }

                return changes + (lastProbability != On.Probability ? 1 : 0);
            }
            return 0;
        }

        public override void SolveAssertTrue(KB kb)
        {
            base.SolveAssertTrue(kb);
        }

        public override void SolveAssertFalse(KB kb)
        {
            base.SolveAssertFalse(kb);
        }

        public override string ToString()
        {
            return $"P({On}) at least {Value} of {Set}";
        }

    }
}
