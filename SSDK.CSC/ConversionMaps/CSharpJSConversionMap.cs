using Microsoft.CodeAnalysis;
using SSDK.CSC.ScriptComponents;
using SSDK.CSC.ScriptComponents.Expressions;
using SSDK.CSC.ScriptComponents.Statements;
using SSDK.CSC.ScriptComponents.Trivia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ConversionMaps
{
    /// <summary>
    /// A conversion map for C# to JavaScript.
    /// A C# project that wishes to be converted to JavaScript,
    /// may define the following attribute for a class, function, or
    /// variable, in order to rename C# classes to global JS references.<br/>
    /// [CSCRedirect("redirect")]<br/>
    /// where redirect is a single identifier.<br/>
    /// <br/>
    /// Additionally, a namespace may have the following attribute, which
    /// allows the namespace to be ignored.<br/>
    /// [CSCIgnore()]
    /// </summary>
    public class CSharpJSConversionMap : CSharpConversionMap
    {
        public override void ProcessAccessModifier(CSharpAccessModifier modifier, StringBuilder result)
        {
            // No access modifiers are supported.
        }

        public override void ProcessArrayCreationExpression(CSharpArrayCreationExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessAssignmentExpression(CSharpAssignmentExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessAttribute(CSharpAttribute attribute, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessAttributes(CSharpAttribute[] attributes, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessBinaryExpression(CSharpBinaryExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessBreakStatement(CSharpBreakStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessCastExpression(CSharpCastExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessCheckedContextExpression(CSharpCheckedContextExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessCheckedContextStatement(CSharpCheckedContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessClass(CSharpClass @class, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessClassHeader(CSharpClass @class, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessClosedExpression(CSharpClosedExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessConditionalAccessExpression(CSharpConditionalAccessExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessConditionalExpression(CSharpConditionalExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessContinueStatement(CSharpContinueStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessDelegate(CSharpDelegate @delegate, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessDoWhileStatement(CSharpDoWhileStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessEmptyStatement(CSharpEmptyStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessEnum(CSharpEnum @enum, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessEnumHeader(CSharpEnum @enum, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessEnumValue(CSharpEnumValue enumValue, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessExpressionStatement(CSharpExpressionStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessFixedContextStatement(CSharpFixedContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessForeachStatement(CSharpForeachStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessForStatement(CSharpForStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessGeneralModifier(CSharpGeneralModifier modifier, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessGotoStatement(CSharpGotoStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessIdentifierExpression(CSharpIdentifierExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessIfStatement(CSharpIfStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessIndexAccessExpression(CSharpIndexAccessExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessIndexer(CSharpIndexer indexer, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessInstantiationExpression(CSharpInstantiationExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessInterpolatedStringExpression(CSharpInterpolatedStringExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessInvocationExpression(CSharpInvocationExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessJointStatement(CSharpJointStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessLabelStatement(CSharpLabelStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessLiteralValueExpression(CSharpLiteralValueExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessLockedContextStatement(CSharpLockedContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessMemberAccessExpression(CSharpMemberAccessExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessMethod(CSharpMethod method, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNamespace(CSharpNamespace @namespace, StringBuilder result)
        {
            if(!@namespace.Attributes.Any((a) => a.Type.Name == "CSCIgnore"))
            {

            }
        }

        public override void ProcessParameters(CSharpVariable[] parameters, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessPatternExpression(CSharpPatternExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessPostfixUnaryExpression(CSharpPostfixUnaryExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessPrefixUnaryExpression(CSharpPrefixUnaryExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessProperty(CSharpProperty property, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessReturnStatement(CSharpReturnStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessScript(CSharpScript script, StringBuilder result)
        {
            ProcessNamespace(script.RootNamespace, result);
        }

        public override void ProcessStatementBlock(CSharpStatementBlock statementBlock, StringBuilder result, bool opened = false, bool allowSingleLineBlock = true)
        {
            throw new NotImplementedException();
        }

        public override void ProcessStruct(CSharpStruct @struct, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessStructHeader(CSharpStruct @struct, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessSwitchExpression(CSharpSwitchExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessSwitchStatement(CSharpSwitchStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessThisExpression(CSharpThisExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessThrowExpression(CSharpThrowExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessThrowStatement(CSharpThrowStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTriviaComment(CSharpComment comment, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTriviaDocumentation(CSharpDoc doc, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTryStatement(CSharpTryStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTupleExpression(CSharpTupleExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessType(CSharpType type, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTypeParameters(string[] typeparams, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTypeParameters(CSharpType[] typeparams, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessUnsafeContextStatement(CSharpUnsafeContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessUsingDirective(CSharpUsingDirective usingDirective, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessUsingStatement(CSharpUsingStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessVariable(CSharpVariable variable, StringBuilder result, bool includeType = true)
        {
            throw new NotImplementedException();
        }

        public override void ProcessWhileStatement(CSharpWhileStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessYieldStatement(CSharpYieldStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }
    }
}
