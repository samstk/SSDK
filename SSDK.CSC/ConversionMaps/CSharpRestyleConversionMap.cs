using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents;
using SSDK.CSC.ScriptComponents.Expressions;
using SSDK.CSC.ScriptComponents.Statements;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Conversions
{

    public sealed class CSharpRestyleConversionMap : CSharpConversionMap
    {
        public override void ProcessAccessModifier(CSharpAccessModifier modifier, StringBuilder result)
        {
            switch(modifier)
            {
                case CSharpAccessModifier.Public:
                    result.Append("public");
                    return;
                case CSharpAccessModifier.Private:
                    result.Append("private");
                    return;
                case CSharpAccessModifier.Internal:
                    result.Append("internal");
                    return;
                case CSharpAccessModifier.Protected:
                    result.Append("protected");
                    return;
                case CSharpAccessModifier.ProtectedInternal:
                    result.Append("protected internal");
                    return;
                case CSharpAccessModifier.PrivateProtected:
                    result.Append("private protected");
                    return;
            }
        }

        public override void ProcessGeneralModifier(CSharpGeneralModifier modifier, StringBuilder result)
        {
            if (modifier.HasFlag(CSharpGeneralModifier.Abstract))
                result.Append("abstract");

            if (modifier.HasFlag(CSharpGeneralModifier.Async))
                result.Append("async");

            if (modifier.HasFlag(CSharpGeneralModifier.Const))
                result.Append("const");

            if (modifier.HasFlag(CSharpGeneralModifier.Event))
                result.Append("event");

            if (modifier.HasFlag(CSharpGeneralModifier.Extern))
                result.Append("extern");

            if (modifier.HasFlag(CSharpGeneralModifier.In))
                result.Append("in");

            if (modifier.HasFlag(CSharpGeneralModifier.New))
                result.Append("new");

            if (modifier.HasFlag(CSharpGeneralModifier.Out))
                result.Append("out");

            if (modifier.HasFlag(CSharpGeneralModifier.Override))
                result.Append("override");

            if (modifier.HasFlag(CSharpGeneralModifier.Readonly))
                result.Append("readonly");

            if (modifier.HasFlag(CSharpGeneralModifier.Sealed))
                result.Append("sealed");

            if (modifier.HasFlag(CSharpGeneralModifier.Static))
                result.Append("static");

            if (modifier.HasFlag(CSharpGeneralModifier.Unsafe))
                result.Append("unsafe");

            if (modifier.HasFlag(CSharpGeneralModifier.Virtual))
                result.Append("virtual");

            if (modifier.HasFlag(CSharpGeneralModifier.Volatile))
                result.Append("volatile");
        }

        public override void ProcessAssignmentExpression(CSharpAssignmentExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessAttribute(CSharpAttribute attribute, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessBinaryExpression(CSharpBinaryExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessClass(CSharpClass @class, StringBuilder result)
        {
            throw new NotImplementedException();
        }
        public override void ProcessParameters(CSharpVariable[] parameters, StringBuilder result)
        {
            if (parameters.Length > 0)
                ProcessVariable(parameters[0], result);
            for (int i = 1; i < parameters.Length; i++)
            {
                result.Append(", ");
                ProcessVariable(parameters[i], result);
            }
        }
        public override void ProcessTypeParameters(string[] typeparams, StringBuilder result)
        {
            if (typeparams.Length > 0)
                result.Append(typeparams[0]);
            for (int i = 1; i<typeparams.Length; i++)
            {
                result.Append(", "+typeparams[i]);
            }
        }
        public override void ProcessTypeParameters(CSharpType[] typeparams, StringBuilder result)
        {
            if (typeparams.Length > 0)
                ProcessType(typeparams[0], result);
            for (int i = 1; i < typeparams.Length; i++)
            {
                result.Append(", "); 
                ProcessType(typeparams[i], result);
            }
        }

        public override void ProcessDelegate(CSharpDelegate @delegate, StringBuilder result)
        {
            result.Continue();
            ProcessAccessModifier(@delegate.AccessModifier, result);
            
            result.StartNewWord();
            result.Append("delegate ");

            ProcessType(@delegate.ReturnType, result);

            result.StartNewWord();
            result.Append(@delegate.Name);

            if (@delegate.TypeParameters.Length > 0)
            {
                result.Append("<");
                ProcessTypeParameters(@delegate.TypeParameters, result);
                result.Append(">");
            }
            result.Append("(");
            ProcessParameters(@delegate.Parameters, result);
            result.Append(")");

            if (@delegate.TypeConstraints.Count > 0)
            {
                result.Append(" where ");
                bool appended = false;
                foreach(string typeparam in @delegate.TypeParameters)
                {
                    CSharpType[] constraints = null;
                    if (@delegate.TypeConstraints.TryGetValue(typeparam, out constraints))
                    {
                        if (appended)
                            result.Append(", ");
                        foreach(CSharpType type in constraints)
                        {
                            result.Append($"{typeparam} : ");
                            ProcessType(type, result);
                        }
                        appended = true;
                    }
                }
            }

            result.Append(";");
        }

        public override void ProcessEnum(CSharpEnum @enum, StringBuilder result)
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

        

        public override void ProcessIdentifierExpression(CSharpIdentifierExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessIndexer(CSharpIndexer indexer, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessInvocationExpression(CSharpInvocationExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessLiteralValueExpression(CSharpLiteralValueExpression expression, StringBuilder result)
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
            foreach(CSharpUsingDirective usingDirective in @namespace.UsingDirectives)
            {
                result.StartNewLine();
                ProcessUsingDirective(usingDirective, result);
            }

            foreach (CSharpDelegate @delegate in @namespace.Delegates)
            {
                result.StartNewLine();
                ProcessDelegate(@delegate, result);
            }

            foreach (CSharpNamespace childNamespace in @namespace.Namespaces)
            {
                result.StartNewLine();
                result.ContinueWithLine($"namespace {childNamespace.Name}");
                result.ContinueWithAndOpen("{");
                ProcessNamespace(childNamespace, result);
                result.ContinueWithAsClose("}");
            }

            foreach (CSharpClass @class in @namespace.Classes)
            {
                result.StartNewLine();
                result.Continue();

                ProcessAccessModifier(@class.AccessModifier, result);
                result.StartNewWord();

                ProcessGeneralModifier(@class.GeneralModifier, result);
                result.StartNewWord();

                result.Append($"class {@class.Name}");
                if(@class.TypeParameters.Length > 0)
                {
                    result.Append("<");
                    ProcessTypeParameters(@class.TypeParameters, result);
                    result.Append(">");
                }
            }
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

        public override void ProcessStatementBlock(CSharpStatementBlock statementBlock, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessStruct(CSharpStruct @struct, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessType(CSharpType type, StringBuilder result)
        {
            if (type.ArrayDimensions > 0)
            {
                ProcessType(type.ElementType, result);
                result.Append("[");
                for (int i = 1; i < type.ArrayDimensions; i++)
                {
                    result.Append(",");
                }
                result.Append("]");
            }
            else
            {
                result.Append(type.Name);
                if (type.HasGenericTypes)
                {
                    result.Append("<");
                    ProcessTypeParameters(type.GenericTypes, result);
                    result.Append(">");
                }
            }
        }

        public override void ProcessUsingDirective(CSharpUsingDirective usingDirective, StringBuilder result)
        {
            result.ContinueWith("using ");
            if (usingDirective.IsStatic)
                result.Append("static ");
            if (usingDirective.Alias != null)
            {
                result.Append($"{usingDirective.Alias} = ");
            }
            result.Append(usingDirective.Target);
            result.Append(";");
        }

        public override void ProcessVariable(CSharpVariable variable, StringBuilder result)
        {
            throw new NotImplementedException();
        }
    }
}
