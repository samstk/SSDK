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
        public abstract void ProcessStatementBlock(CSharpStatementBlock statementBlock, StringBuilder result, bool opened=false, bool allowSingleLineBlock=true);
        public abstract void ProcessVariable(CSharpVariable variable, StringBuilder result, bool includeType=true);
        public abstract void ProcessParameters(CSharpVariable[] parameters, StringBuilder result);
        public virtual void ProcessExpressions(CSharpExpression[] expressions, StringBuilder result)
        {
            if(expressions.Length > 0)
            {
                if (expressions != null && expressions[0] != null)
                    expressions[0].ProcessMap(this, result);

                for(int i = 1; i<expressions.Length; i++)
                {
                    result.Append(", ");
                    if (expressions[i] != null)
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
        public abstract void ProcessCheckedContextExpression(CSharpCheckedContextExpression expression, StringBuilder result);
        public abstract void ProcessPatternExpression(CSharpPatternExpression expression, StringBuilder result);
        public abstract void ProcessSwitchExpression(CSharpSwitchExpression expression, StringBuilder result);
        public abstract void ProcessThrowExpression(CSharpThrowExpression expression, StringBuilder result);
        public abstract void ProcessConditionalExpression(CSharpConditionalExpression expression, StringBuilder result);
        public abstract void ProcessConditionalAccessExpression(CSharpConditionalAccessExpression expression, StringBuilder result);
        #endregion
        #region Statements
        public abstract void ProcessUsingDirective(CSharpUsingDirective usingDirective, StringBuilder result);
        public abstract void ProcessExpressionStatement(CSharpExpressionStatement statement, StringBuilder result);
        public abstract void ProcessReturnStatement(CSharpReturnStatement statement, StringBuilder result);
        public abstract void ProcessJointStatement(CSharpJointStatement statement, StringBuilder result);
        public abstract void ProcessSwitchStatement(CSharpSwitchStatement statement, StringBuilder result);
        public abstract void ProcessIfStatement(CSharpIfStatement statement, StringBuilder result);
        public abstract void ProcessWhileStatement(CSharpWhileStatement statement, StringBuilder result);
        public abstract void ProcessDoWhileStatement(CSharpDoWhileStatement statement, StringBuilder result);
        public abstract void ProcessForStatement(CSharpForStatement statement, StringBuilder result);
        public abstract void ProcessForeachStatement(CSharpForeachStatement statement, StringBuilder result);
        public abstract void ProcessTryStatement(CSharpTryStatement statement, StringBuilder result);
        public abstract void ProcessThrowStatement(CSharpThrowStatement statement, StringBuilder result);
        public abstract void ProcessBreakStatement(CSharpBreakStatement statement, StringBuilder result);
        public abstract void ProcessContinueStatement(CSharpContinueStatement statement, StringBuilder result);
        public abstract void ProcessYieldStatement(CSharpYieldStatement statement, StringBuilder result);
        public abstract void ProcessGotoStatement(CSharpGotoStatement statement, StringBuilder result);
        public abstract void ProcessLabelStatement(CSharpLabelStatement statement, StringBuilder result);
        public abstract void ProcessCheckedContextStatement(CSharpCheckedContextStatement statement, StringBuilder result);
        public abstract void ProcessUnsafeContextStatement(CSharpUnsafeContextStatement statement, StringBuilder result);
        public abstract void ProcessFixedContextStatement(CSharpFixedContextStatement statement, StringBuilder result);
        public abstract void ProcessLockedContextStatement(CSharpLockedContextStatement statement, StringBuilder result);
        public abstract void ProcessEmptyStatement(CSharpEmptyStatement statement, StringBuilder result);
        public abstract void ProcessUsingStatement(CSharpUsingStatement statement, StringBuilder result);
        #endregion

        #region Trivia
        /// <summary>
        /// As of this version, no documentation is supported.
        /// </summary>
        public abstract void ProcessTriviaDocumentation(CSharpDoc doc, StringBuilder result);
        /// <summary>
        /// As of this version, no documentation is supported.
        /// </summary>
        public abstract void ProcessTriviaComment(CSharpComment comment, StringBuilder result);
        #endregion
    }
}
