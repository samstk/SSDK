using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents;
using SSDK.CSC.ScriptComponents.Expressions;
using SSDK.CSC.ScriptComponents.Statements;
using SSDK.CSC.ScriptComponents.Trivia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC
{
    /// <summary>
    /// A base class for a conversion map, which can be used to
    /// run through every part of the script and convert csharp to another language/style.
    /// </summary>
    /// <remarks>
    /// All processing functions are done in pre-order traversal (i.e. the parent is traversed before its children),
    /// and should produce a single string for a given component. All processing functions must individually call
    /// other processing functions for children of a component.
    /// </remarks>
    public abstract class CSharpConversionMap
    {
        #region Basic Component Types
        public abstract void ProcessScript(CSharpScript script, StringBuilder result);
        public abstract void ProcessAccessModifier(CSharpAccessModifier modifier, StringBuilder result);
        public abstract void ProcessGeneralModifier(CSharpGeneralModifier modifier, StringBuilder result);
        public abstract void ProcessAttribute(CSharpAttribute attribute, StringBuilder result);
        public abstract void ProcessAttributes(CSharpAttribute[] attributes, StringBuilder result);
        public abstract void ProcessType(CSharpType type, StringBuilder result);
        public abstract void ProcessClassHeader(CSharpClass @class, StringBuilder result);
        public abstract void ProcessClass(CSharpClass @class, StringBuilder result);

        public abstract void ProcessStructHeader(CSharpStruct @struct, StringBuilder result);
        public abstract void ProcessStruct(CSharpStruct @struct, StringBuilder result);
        public abstract void ProcessDelegate(CSharpDelegate @delegate, StringBuilder result);
        public abstract void ProcessEnumHeader(CSharpEnum @enum, StringBuilder result);
        public abstract void ProcessEnum(CSharpEnum @enum, StringBuilder result);
        public abstract void ProcessEnumValue(CSharpEnumValue enumValue, StringBuilder result);
        public abstract void ProcessIndexer(CSharpIndexer indexer, StringBuilder result);
        public abstract void ProcessMethod(CSharpMethod method, StringBuilder result);
        public abstract void ProcessNamespace(CSharpNamespace @namespace, StringBuilder result);
        public abstract void ProcessProperty(CSharpProperty property, StringBuilder result);
        public abstract void ProcessStatementBlock(CSharpStatementBlock statementBlock, StringBuilder result);
        public abstract void ProcessVariable(CSharpVariable variable, StringBuilder result);
        public abstract void ProcessParameters(CSharpVariable[] parameters, StringBuilder result);
        public virtual void ProcessExpressions(CSharpExpression[] expressions, StringBuilder result)
        {
            if(expressions.Length > 0)
            {
                expressions[0].ProcessMap(this, result);

                for(int i = 1; i<expressions.Length; i++)
                {
                    result.Append(", ");
                    expressions[i].ProcessMap(this, result);
                }
            }
        }

        public abstract void ProcessTypeParameters(string[] typeparams, StringBuilder result);
        public abstract void ProcessTypeParameters(CSharpType[] typeparams, StringBuilder result);
        #endregion
        #region Expressions
        public abstract void ProcessThisExpression(CSharpThisExpression expression, StringBuilder result);
        public abstract void ProcessAssignmentExpression(CSharpAssignmentExpression expression, StringBuilder result);
        public abstract void ProcessBinaryExpression(CSharpBinaryExpression expression, StringBuilder result);
        public abstract void ProcessClosedExpression(CSharpClosedExpression expression, StringBuilder result);
        public abstract void ProcessCastExpression(CSharpCastExpression expression, StringBuilder result);
        public abstract void ProcessPrefixUnaryExpression(CSharpPrefixUnaryExpression expression, StringBuilder result);
        public abstract void ProcessIdentifierExpression(CSharpIdentifierExpression expression, StringBuilder result);
        public abstract void ProcessInvocationExpression(CSharpInvocationExpression expression, StringBuilder result);
        public abstract void ProcessInstantiationExpression(CSharpInstantiationExpression expression, StringBuilder result);
        public abstract void ProcessLiteralValueExpression(CSharpLiteralValueExpression expression, StringBuilder result);
        public abstract void ProcessMemberAccessExpression(CSharpMemberAccessExpression expression, StringBuilder result);
        public abstract void ProcessInterpolatedStringExpression(CSharpInterpolatedStringExpression expression, StringBuilder result);
        public abstract void ProcessArrayCreationExpression(CSharpArrayCreationExpression expression, StringBuilder result);
        public abstract void ProcessIndexAccessExpression(CSharpIndexAccessExpression expression, StringBuilder result);
        public abstract void ProcessPostfixUnaryExpression(CSharpPostfixUnaryExpression expression, StringBuilder result);
        public abstract void ProcessTupleExpression(CSharpTupleExpression expression, StringBuilder result);
        #endregion
        #region Statements
        public abstract void ProcessUsingDirective(CSharpUsingDirective usingDirective, StringBuilder result);
        public abstract void ProcessExpressionStatement(CSharpExpressionStatement statement, StringBuilder result);
        public abstract void ProcessReturnStatement(CSharpReturnStatement statement, StringBuilder result);
        public abstract void ProcessJointStatement(CSharpJointStatement statement, StringBuilder result);
        public abstract void ProcessSwitchStatement(CSharpSwitchStatement statement, StringBuilder result);
        public abstract void ProcessIfStatement(CSharpIfStatement statement, StringBuilder result);

        public abstract void ProcessWhileStatement(CSharpWhileStatement statement, StringBuilder result);
        public abstract void ProcessForStatement(CSharpForStatement statement, StringBuilder result);
        public abstract void ProcessForeachStatement(CSharpForeachStatement statement, StringBuilder result);
        public abstract void ProcessTryStatement(CSharpTryStatement statement, StringBuilder result);
        #endregion

        #region Trivia
        public abstract void ProcessTriviaDocumentation(CSharpDoc doc, StringBuilder result);
        public abstract void ProcessTriviaComment(CSharpComment comment, StringBuilder result);
        #endregion
    }
}
