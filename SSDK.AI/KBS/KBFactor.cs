using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSDK.AI.KBS.Arithmetic;
using SSDK.AI.KBS.Logic;

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
        /// <returns></returns>
        public virtual bool HasConflict() => false;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (!Solved) return base.Equals(obj);
            if(obj is KBFactor)
            {
                return Calculate() == ((KBFactor)obj).Calculate();
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
            return includeSolution && Solved && !IsClass ? (bracketsOnAssertValue ? "(" : "") + ToString() + (bracketsOnAssertValue ? ")" : "") + "=" + (AltAssertion!=null ? AltAssertion : Assertion ? "T" : "F") : ToString();
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
                return AltAssertion != null && AltAssertion != Null;
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
        /// Asserts that this statement is a value, when solving.
        /// </summary>
        /// <param name="kb"></param>
        public virtual void SolveAssertOther(KB kb, KBFactor other)
        {
            AltAssertion = other;
            Solved = true;
        }
        /// <summary>
        /// Asserts that this statement is true, when solving.
        /// </summary>
        /// <param name="kb"></param>
        public virtual void SolveAssertTrue(KB kb)
        {
            Assertion = true;
            Solved = true;
        }
        /// <summary>
        /// Asserts that this statement is false, when solving.
        /// </summary>
        /// <param name="kb"></param>
        public virtual void SolveAssertFalse(KB kb)
        {
            Assertion = false;
            Solved = true;
        }
        /// <summary>
        /// Resets the current solution.
        /// </summary>
        public virtual void ResetSolution()
        {
            Solved = false;
            Assertion = false;
            foreach (KBFactor child in GetChildren())
            {
                child.ResetSolution();
            }
        }

        /// <summary>
        /// Simplifies the current factor.
        /// </summary>
        /// <returns></returns>
        public virtual KBFactor Simplify()
        {
            return this;
        }
    }
}
