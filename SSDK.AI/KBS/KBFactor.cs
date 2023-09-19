using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SSDK.AI.KBS.Arithmetic;
using SSDK.AI.KBS.Logic;
using SSDK.Core.Structures.Primitive;

namespace SSDK.AI.KBS
{
    /// <summary>
    /// An abstract class that contains methods for solving
    /// terms and basic arithmetic, for example (AND(0,0,0))...
    /// or (a -> b = b - 10.0) or (a -> b = AND(a,c))
    /// </summary>
    public abstract class KBFactor
    {
        /// <summary>
        /// Gets the null factor
        /// </summary>
        public static KBNull Null { get; private set; } = new KBNull();

        private double _Probability = 0;

        /// <summary>
        /// Gets or sets the general probability (GP) that the factor holds true.
        /// </summary>
        /// <remarks>
        /// For example, suppose p -> q.
        /// If GP of p = 0.2, and GP of q = 0.5, we know that q occurs 2.5 time more often than p.
        /// So, if q is true (P(q) = 1), and suppose p -> q is true 0.8 of the time, then
        /// we can find p using the general probability (bayesian statistics). In this example, we
        /// calculate p to be 0.8 * 0.3 / 0.5 or (P(p -> q) * p / q)
        /// </remarks>
        public double GeneralProbability
        {
            get;
            set;
        }
        

        /// <summary>
        /// Gets or sets the probability that the factor holds true.
        /// If at any time the factor is solved, and asserted to be
        /// a certain value, then probability defaults to 1 or 0.
        /// </summary>
        public double Probability { 
            get
            {
                if (Solved)
                {
                    return Assertion ? 1 : 0;
                }
                return _Probability;
            }
            set
            {
                _Probability = value;
            }
        }

        /// <summary>
        /// If true, then the probability has been solved and should be accurate.
        /// </summary>
        public bool ProbabilitySolved { get; set; } = false;

        private bool _ProbabilityEnforced = false;
        /// <summary>
        /// If true, then the probability has been solved completely, and should
        /// not change (i.e. assert P(x) = 0.2, P(x) should always be 0.2)
        /// </summary>
        public bool ProbabilityEnforced { 
            get
            {
                return _ProbabilityEnforced;
            }
            set
            {
                _ProbabilityEnforced = value;
            }
        }

        public bool IsBooleanTrue
        {
            get
            {
                return this is KBBooleanSymbol && ((KBBooleanSymbol)this).Bit;
            }
        }
        public bool IsBooleanFalse
        {
            get
            {
                return this is KBBooleanSymbol && !((KBBooleanSymbol)this).Bit;
            }
        }
        /// <summary>
        /// Returns true if the factor holds true.
        /// </summary>
        /// <returns>true/false based on how the factor calculates.</returns>
        public abstract bool Holds();

        /// <summary>
        /// Calculate the result of the current object.
        /// </summary>
        /// <param name="kb"></param>
        /// <returns></returns>
        public virtual KBFactor Calculate() {
            if (!Solved) return KBFactor.Null;
            if (HasAltAssertion) return AltAssertion;
            return new KBBooleanSymbol(Holds());
        }

        /// <summary>
        /// Returns true if there is a conflict within this factor.
        /// i.e. this is asserted to be true, but the children calculations do not match.
        /// </summary>
        /// <returns>null if no conflict, or the first factor responsible for the conflict</returns>
        public virtual KBFactor HasConflict()
        {
            foreach(KBFactor factor in GetChildren())
            {
                KBFactor conflict = factor.HasConflict();
                if (conflict as object != null) return conflict;
            }
            return null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (!Solved) return base.Equals(obj);
            if(obj is KBFactor)
            {
                return Calculate().Equals(((KBFactor)obj).Calculate());
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns true if the inverse of the factor holds true.
        /// </summary>
        /// <returns>true/false based on how the factor calculates.</returns>
        public virtual bool InverseHolds() { return !Holds(); }
        /// <summary>
        /// Gets whether the factor/symbol is being interpreted as a class.
        /// </summary>
        public bool IsClass { get; internal set; } = false;
        public override string ToString()
        {
            return "[logic]";
        }
        public string ToString(bool includeSolution, bool bracketsOnAssertValue=true)
        {
            return (includeSolution && Solved && !IsClass ? (bracketsOnAssertValue ? "(" : "") + ToString() + (bracketsOnAssertValue ? ")" : "") + "=" + (AltAssertion as object != null ? AltAssertion : Assertion ? "T" : "F") : ToString()) + PString;
        }

        /// <summary>
        /// Returns the probability chance as a string if it is not one.
        /// </summary>
        private string PString
        {
            get
            {
                return Probability < 1 ? "("+ (Probability * 100).ToString("0.0") + "%)" : "";
            }
        }

        /// <summary>
        /// Gets all symbols used in this factor.
        /// </summary>
        /// <returns>all symbols in a set</returns>
        public abstract HashSet<KBSymbol> GetSymbols();

        /// <summary>
        /// Gets the direct children under this factor
        /// </summary>
        /// <returns>all direct children in a list</returns>
        public abstract List<KBFactor> GetChildren();

        /// <summary>
        /// If true, then the factor has been solved according to a KB.
        /// This means that the Assertion variable is accurate.
        /// </summary>
        public bool Solved { get; internal set; } = false;

        /// <summary>
        /// If Solved is true, then this value is accurate within the KB.
        /// </summary>
        public bool Assertion { get; internal set; } = false;

        /// <summary>
        /// If Solved is true, then this value is accurate within the KB.
        /// This is an alternate assertion, used for non-logic (i.e. arithmetic)
        /// logic.
        /// </summary>
        public KBFactor AltAssertion { get; internal set; } = null;

        public bool HasAltAssertion { 
            get
            {
                return AltAssertion as object != null && !(AltAssertion.Equals(Null));
            }
        }

        /// <summary>
        /// Returns SolveTrue or SolveFalse if the child can be asserted to a particular value based
        /// on the parent type.
        /// </summary>
        /// <param name="kb"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public abstract KBSolveType CanSolveForChild(KB kb, KBFactor child);

        /// <summary>
        /// Gets the alternate solution for children (i.e. the non-boolean value it must be).
        /// </summary>
        /// <param name="kb"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public virtual KBFactor GetAltSolutionForChild(KB kb, KBFactor child) { return null; }

        /// <summary>
        /// Applies a given operation to the given factor, returning a new factor that results from that operation.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="terms"></param>
        /// <returns></returns>
        public virtual KBFactor Apply(char op, params KBFactor[] terms) { return Null; }

        /// <summary>
        /// Solves the current factor as an assertion if possible within the current knowledge, and returns
        /// the number of operations / symbols solved.
        /// </summary>
        /// <returns>number of operations / symbols solved</returns>
        public abstract int SolveAssertion(KB kb, KBFactor parent);

        /// <summary>
        /// Solves the current factor's probability, using what is known in the knowledge base, and returns
        /// the number of operations / symbols solved.
        /// </summary>
        /// <returns>number of operations / symbols solved</returns>
        public abstract int SolveProbability(KB kb, KBFactor parent);
        
        /// <summary>
        /// Asserts that this statement is a value, when solving.
        /// </summary>
        /// <param name="kb"></param>
        public virtual void SolveAssertOther(KB kb, KBFactor other)
        {
            AltAssertion = other;
            Solved = true;
            ProbabilityEnforced = true;
        }
        /// <summary>
        /// Asserts that this statement is true, when solving.
        /// </summary>
        /// <param name="kb"></param>
        public virtual void SolveAssertTrue(KB kb)
        {
            Assertion = true;
            Solved = true;
            ProbabilitySolved = true;
            Probability = 1;
            ProbabilityEnforced = true;
        }
        /// <summary>
        /// Asserts that this statement is false, when solving.
        /// </summary>
        /// <param name="kb"></param>
        public virtual void SolveAssertFalse(KB kb)
        {
            Assertion = false;
            Solved = true;
            ProbabilitySolved = true;
            Probability = 0;
            ProbabilityEnforced = true;
        }
        /// <summary>
        /// Resets the current solution.
        /// </summary>
        public virtual void ResetSolution()
        {
            Solved = false;
            Assertion = false;
            Probability = 0; // Must start off as zero.
            GeneralProbability = 0;
            ProbabilitySolved = false;
            ProbabilityEnforced = false;


            foreach (KBFactor child in GetChildren())
            {
                child.ResetSolution();
            }
        }

        /// <summary>
        /// Enforces the probability to ensure it is not edited.
        /// </summary>
        public virtual void EnforceProbability()
        {
            ProbabilityEnforced = true;
        }

        /// <summary>
        /// Updates the general probability of the factor.
        /// </summary>
        /// <param name="newProbability">the new general probability of the factor</param>
        /// <returns>true if the general probability changed</returns>
        public bool UpdateGeneralProbability(double newProbability)
        {
            double lastProbability = GeneralProbability;
            GeneralProbability = newProbability;
            return newProbability != lastProbability;
        }

        /// <summary>
        /// Updates the probability of the factor.
        /// </summary>
        /// <param name="newProbability">the new probability of the factor</param>
        /// <returns>true if the probability changed</returns>
        public bool UpdateProbability(double newProbability)
        {
            if (ProbabilityEnforced) return false;

            double lastProbability = Probability;
            Probability = newProbability;
            ProbabilitySolved = true;
            return newProbability != lastProbability;
        }

        /// <summary>
        /// Returns true if the other factor is of the same set as this factor.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IsOfSameSet(KBFactor other)
        {
            return false;
        }

        /// <summary>
        /// Simplifies the current factor.
        /// </summary>
        /// <returns></returns>
        public virtual KBFactor Simplify()
        {
            return this;
        }

        #region Operator Overloading

        /// <summary>
        /// Negation of the sentence
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static KBNot operator !(KBFactor sentence)
        {
            return new KBNot(sentence);
        }

        /// <summary>
        /// Negation of the sentence
        /// </summary>
        /// <returns></returns>
        public static KBNot operator ~(KBFactor sentence)
        {
            return new KBNot(sentence);
        }

        /// <summary>
        /// AND concatenation of the sentences
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static KBAnd operator &(KBFactor lhs, KBFactor rhs)
        {
            return new KBAnd(lhs, rhs);
        }

        /// <summary>
        /// OR concatenation of the sentences
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static KBOr operator |(KBFactor lhs, KBFactor rhs)
        {
            return new KBOr(lhs, rhs);
        }

        /// <summary>
        /// Agreement (both values match) between the sentences.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static KBAgreement operator ==(KBFactor lhs, KBFactor rhs)
        {
            return new KBAgreement(lhs, rhs);
        }

        /// <summary>
        /// Agreement (both values do not match) between the sentences
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static KBAgreement operator !=(KBFactor lhs, KBFactor rhs)
        {
            return new KBAgreement(!lhs, rhs);
        }

        /// <summary>
        /// Implication (lhs implies rhs)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static KBImplication operator >=(KBFactor lhs, KBFactor rhs)
        {
            return new KBImplication(lhs, rhs);
        }

        /// <summary>
        /// Implication (rhs implies lhs)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static KBImplication operator <=(KBFactor lhs, KBFactor rhs)
        {
            return new KBImplication(rhs, lhs);
        }

        public static implicit operator KBFactor(UncontrolledNumber number) => new KBNumericSymbol(number);
        public static implicit operator KBFactor(double number) => new KBNumericSymbol(number);
        public static implicit operator KBFactor(bool boolean) => new KBBooleanSymbol(boolean);

        #endregion
    }
}
