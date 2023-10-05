using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Exceptions
{
    /// <summary>
    /// Represents an exception occurring the invalid syntax detected
    /// in certain types of statements or expressions.
    /// </summary>
    public class SyntaxOrSemanticException : Exception
    {
        public SyntaxOrSemanticException(StatementSyntax statement) 
            : base($"A critical semantic or syntax error was detected in the statement at {statement.GetLocation()}\nPlease use a pre-processor (or native semantic checker) to rid all errors from the CSharp before using the tool.")
        {

        }

        public SyntaxOrSemanticException(ExpressionSyntax expression)
            : base($"A critical semantic or syntax error was detected in the expression at {expression.GetLocation()}\nPlease use a pre-processor (or native semantic checker) to rid all errors from the CSharp before using the tool.")
        {

        }
    }
}
