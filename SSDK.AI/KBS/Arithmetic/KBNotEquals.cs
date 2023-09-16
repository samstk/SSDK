﻿using SSDK.AI.KBS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Arithmetic
{
    /// <summary>
    /// Represents a not equals operation on a symbol (e.g. 1 != -1)
    /// Not to be confused with the logical agreement, although this
    /// can be considered a logical connective to be used for arithmetic purposes.
    /// 
    /// This is just a simpler way of saying KBNot(KBEquals(t1,t2));
    /// </summary>
    public sealed class KBNotEquals : KBFactor
    {
        /// <summary>
        /// The left-side term which is compared
        /// </summary>
        public KBFactor LHS { get; private set; }

        /// <summary>
        /// The right-side term which is compared.
        /// </summary>
        public KBFactor RHS { get; private set; }

        public KBNotEquals (KBFactor lhs, KBFactor rhs)
        {
            LHS = lhs;
            RHS = rhs;
        }

        public override KBSolveType CanSolveForChild(KB kb, KBFactor child)
        {
            if (Solved && !Assertion && (LHS.Solved && child == RHS || RHS.Solved && child == LHS))
            {
                return KBSolveType.SolveArithmetic;
            }
            return KBSolveType.NoSolution;
        }

        public override KBFactor GetAltSolutionForChild(KB kb, KBFactor child)
        {
            if (Solved && !Assertion)
            {
                if (child == LHS)
                    return RHS; // Child is definitely equal to the other
                return LHS;
            }
            return null;
        }
        public override bool HasConflict()
        {
            KBFactor left = LHS.Calculate();
            KBFactor right = RHS.Calculate();
            return Solved && Assertion && left != null && right != null && left.Equals(right);
        }

        public override KBFactor Calculate()
        {
            return new KBBooleanSymbol(LHS.Calculate() == RHS.Calculate());
        }

        public override bool Holds()
        {
            return Solved && Assertion; // Exchangable with logical operations.
        }

        public override HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> symbols = new HashSet<KBSymbol>();
            foreach (KBSymbol symbol in LHS.GetSymbols())
                symbols.Add(symbol);
            foreach (KBSymbol symbol in RHS.GetSymbols())
                symbols.Add(symbol);
            return symbols;
        }

        public override List<KBFactor> GetChildren()
        {
            return new List<KBFactor>() { LHS, RHS };
        }

        public override int SolveAssertion(KB kb, KBFactor parent)
        {
            int changes = LHS.SolveAssertion(kb, this) + RHS.SolveAssertion(kb, this);
            if(!Solved)
            {
                KBSolveType type = parent == null ? KBSolveType.SolveTrue : parent.CanSolveForChild(kb, this);
                if (type == KBSolveType.NoSolution || type == KBSolveType.Other)
                {
                    if (LHS.Solved && RHS.Solved) {
                        KBFactor left = LHS.Calculate();
                        KBFactor right = RHS.Calculate();
                        type = !left.Equals(right) ? KBSolveType.SolveTrue : KBSolveType.SolveFalse;
                    }
                }
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

        public override string ToString()
        {
            return $"({LHS} != {RHS})";
        }
    }
}
