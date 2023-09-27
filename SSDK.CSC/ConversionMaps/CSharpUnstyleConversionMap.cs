﻿using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents;
using SSDK.CSC.ScriptComponents.Expressions;
using SSDK.CSC.ScriptComponents.Statements;
using SSDK.CSC.ScriptComponents.Trivia;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Conversions
{

    public sealed class CSharpUnstyleConversionMap : CSharpConversionMap
    {
        public bool IncludeTrivia = true;
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
            expression.Left.ProcessMap(this, result);
            result.Append($" {expression.Operator} ");
            expression.Right.ProcessMap(this, result);
        }

        public override void ProcessAttribute(CSharpAttribute attribute, StringBuilder result)
        {
            result.Append(attribute.Name);
           
            if(attribute.Parameters.Length > 0)
            {
                result.Append("(");

                ProcessExpressions(attribute.Parameters, result);

                result.Append(")");
            }
        }

        public override void ProcessAttributes(CSharpAttribute[] attributes, StringBuilder result)
        {
            if (attributes.Length > 0)
            {
                result.StartNewLine();
                result.Continue();
                result.Append("[");
                ProcessAttribute(attributes[0], result);

                for(int i = 1; i<attributes.Length; i++) 
                {
                    result.Append(", ");
                    result.StartNewLine();
                    result.Continue();
                    ProcessAttribute(attributes[i], result);
                }
                result.AppendLine("]");
            }
        }

        public override void ProcessBinaryExpression(CSharpBinaryExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessClass(CSharpClass @class, StringBuilder result)
        {
            foreach (CSharpEnum subenum in @class.Subenums)
            {
                result.StartNewLine();
                ProcessEnumHeader(subenum, result);
                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessEnum(subenum, result);
                result.ContinueWithAsClose("}");
            }

            foreach (CSharpClass subclass in @class.Subclasses)
            {
                result.StartNewLine();
                ProcessClassHeader(subclass, result);
                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessClass(subclass, result);
                result.ContinueWithAsClose("}");
            }

            foreach(CSharpStruct substruct in @class.Substructs)
            {
                result.StartNewLine();
                ProcessStructHeader(substruct, result);
                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessStruct(substruct, result);
                result.ContinueWithAsClose("}");
            }
            
            foreach (CSharpDelegate @delegate in @class.Delegates)
            {
                result.StartNewLine();
                ProcessDelegate(@delegate, result);
            }

            // Static fields and properties.
            foreach(CSharpVariable variable in @class.StaticFields)
            {
                result.StartNewLine();
                result.Continue();
                ProcessVariable(variable, result);
                result.AppendLine(";");
            }

            foreach (CSharpProperty property in @class.StaticProperties)
            {
                result.StartNewLine();
                ProcessProperty(property, result);
            }

            // Instance fields and properties.
            foreach (CSharpVariable variable in @class.InstanceFields)
            {
                result.StartNewLine();
                result.Continue();
                ProcessVariable(variable, result);
                result.AppendLine(";");
            }

            foreach (CSharpProperty property in @class.InstanceProperties)
            {
                result.StartNewLine();
                ProcessProperty(property, result);
            }

            // Constructors and destructors
            foreach(CSharpMethod method in @class.InstanceConstructors)
            {
                result.StartNewLine();
                ProcessMethod(method, result);
            }

            if (@class.InstanceDestructor != null)
            {
                result.StartNewLine();
                ProcessMethod(@class.InstanceDestructor, result);
            }

            if (@class.StaticConstructor != null)
            {
                result.StartNewLine();
                ProcessMethod(@class.StaticConstructor, result);
            }

            if (@class.StaticDestructor != null)
            {
                result.StartNewLine();
                ProcessMethod(@class.StaticDestructor, result);
            }

            // Indexers
            foreach (CSharpIndexer indexer in @class.Indexers)
            {
                result.StartNewLine();
                ProcessIndexer(indexer, result);
            }

            // Static methods
            foreach (CSharpMethod method in @class.StaticMethods)
            {
                result.StartNewLine();
                ProcessMethod(method, result);
            }

            // Instance methods
            foreach (CSharpMethod method in @class.InstanceMethods)
            {
                result.StartNewLine();
                ProcessMethod(method, result);
            }
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
            ProcessAttributes(@delegate.Attributes, result);

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
                
                foreach(string typeparam in @delegate.TypeParameters)
                {
                    CSharpType[] constraints = null;
                    if (@delegate.TypeConstraints.TryGetValue(typeparam, out constraints))
                    {
                        result.Append(" where ");
                        bool appended = false;
                        foreach (CSharpType type in constraints)
                        {
                            if (appended)
                                result.Append(", ");
                            result.Append($"{typeparam} : ");
                            ProcessType(type, result);
                            appended = true;
                        }
                        
                    }
                }
            }

            result.Append(";");
        }

        public override void ProcessEnum(CSharpEnum @enum, StringBuilder result)
        {
            if(@enum.Values.Length > 0)
            {
                result.StartNewLine();
                result.Continue();
                ProcessEnumValue(@enum.Values[0], result);
                for(int i = 1; i < @enum.Values.Length; i++)
                {
                    result.Append(", ");
                    result.StartNewLine();
                    result.Continue();
                    ProcessEnumValue(@enum.Values[i], result);
                }
                result.AppendLine();
            }
        }

        public override void ProcessEnumValue(CSharpEnumValue enumValue, StringBuilder result)
        {
            ProcessAttributes(enumValue.Attributes, result);
            if (enumValue.Attributes.Length > 0)
            {
                result.StartNewLine();
                result.Continue();
            }
            result.Append(enumValue.Name);
            if(enumValue.Expression != null)
            {
                result.Append(" = ");
                enumValue.Expression.ProcessMap(this, result);
            }
        }

        public override void ProcessExpressionStatement(CSharpExpressionStatement statement, StringBuilder result)
        {
            statement.Expression.ProcessMap(this, result);
            result.Append(";");
        }

        

        public override void ProcessIdentifierExpression(CSharpIdentifierExpression expression, StringBuilder result)
        {
            result.Append(expression.Name);
        }

        public override void ProcessIndexer(CSharpIndexer indexer, StringBuilder result)
        {
            ProcessAttributes(indexer.Attributes, result);

            result.Continue();
            ProcessAccessModifier(indexer.AccessModifier, result);

            result.StartNewWord();
            ProcessGeneralModifier(indexer.GeneralModifier, result);

            result.StartNewWord();
            ProcessType(indexer.Type, result);

            result.StartNewWord();
            result.Append("this[");
            ProcessParameters(indexer.Parameters, result);
            result.AppendLine("]");
            result.ContinueWithAndOpen("{");
            if (indexer.HasGetAccess)
            {
                if (indexer.Get == null)
                {
                    result.Continue();
                    ProcessAccessModifier(indexer.GetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("get;");
                }
                else
                {
                    result.Continue();
                    ProcessAccessModifier(indexer.GetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("get");
                    ProcessStatementBlock(indexer.Get, result);
                }
            }
            if (indexer.HasSetAccess)
            {
                if (indexer.Set == null)
                {
                    result.Continue();
                    ProcessAccessModifier(indexer.SetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("set;");
                }
                else
                {
                    result.Continue();
                    ProcessAccessModifier(indexer.SetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("set");
                    ProcessStatementBlock(indexer.Set, result);
                }
            }
            result.ContinueWithAsClose("}");
        }

        public override void ProcessInvocationExpression(CSharpInvocationExpression expression, StringBuilder result)
        {
            expression.Member.ProcessMap(this, result);
            result.Append("(");
            ProcessExpressions(expression.Arguments, result);
            result.Append(")");
        }

        public override void ProcessInstantiationExpression(CSharpInstantiationExpression expression, StringBuilder result)
        {
            result.Append("new ");
            ProcessType(expression.Type, result);
            result.Append("(");
            ProcessExpressions(expression.Arguments, result);
            result.Append(")");
            if (expression.Initializer.Length > 0)
            {
                result.Append("{ ");
                ProcessExpressions(expression.Initializer, result);
                result.Append(" }");
            }
        }

        public override void ProcessLiteralValueExpression(CSharpLiteralValueExpression expression, StringBuilder result)
        {
            result.Append(((LiteralExpressionSyntax)expression.Syntax).Token.Text);
        }

        public override void ProcessMemberAccessExpression(CSharpMemberAccessExpression expression, StringBuilder result)
        {
            expression.Target.ProcessMap(this, result);
            result.Append($".{expression.Member}");
        }

        public override void ProcessMethod(CSharpMethod method, StringBuilder result)
        {
            ProcessAttributes(method.Attributes, result);

            result.Continue();
            ProcessAccessModifier(method.AccessModifier, result);

            result.StartNewWord();
            ProcessGeneralModifier(method.GeneralModifier, result);

            if(!method.IsConstructor) {
                result.StartNewWord();
                ProcessType(method.ReturnType, result);
            }
            result.StartNewWord();
            result.Append(method.Name);

            if (method.TypeParameters.Length > 0)
            {
                result.Append("<");
                ProcessTypeParameters(method.TypeParameters, result);
                result.Append(">");
            }
            result.Append("(");
            ProcessParameters(method.Parameters, result);
            result.Append(")");

            if (method.TypeConstraints.Count > 0)
            {
                foreach (string typeparam in method.TypeParameters)
                {
                    CSharpType[] constraints = null;
                    if (method.TypeConstraints.TryGetValue(typeparam, out constraints))
                    {
                        result.Append(" where ");
                        bool appended = false;
                        foreach (CSharpType type in constraints)
                        {
                            if (appended)
                                result.Append(", ");
                            result.Append($"{typeparam} : ");
                            ProcessType(type, result);
                            appended = true;
                        }
                    }
                }
            }
            result.AppendLine();
            ProcessStatementBlock(method.Block, result);
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

            foreach (CSharpEnum @enum in @namespace.Enums)
            {
                ProcessEnumHeader(@enum, result);

                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessEnum(@enum, result);
                result.ContinueWithAsClose("}");
            }

            foreach (CSharpClass @class in @namespace.Classes)
            {
                ProcessClassHeader(@class, result);

                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessClass(@class, result);
                result.ContinueWithAsClose("}");
            }

            foreach (CSharpStruct @struct in @namespace.Structs)
            {
                ProcessStructHeader(@struct, result);

                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessStruct(@struct, result);
                result.ContinueWithAsClose("}");
            }
        }

        public override void ProcessProperty(CSharpProperty property, StringBuilder result)
        {
            ProcessAttributes(property.Attributes, result);

            result.Continue();
            ProcessAccessModifier(property.AccessModifier, result);

            result.StartNewWord();
            ProcessGeneralModifier(property.GeneralModifier, result);

            result.StartNewWord();
            ProcessType(property.Type, result);

            result.StartNewWord();
            result.AppendLine(property.Name);
            result.ContinueWithAndOpen("{");
            if (property.HasGetAccess)
            {
                if(property.Get == null)
                {
                    result.Continue();
                    ProcessAccessModifier(property.GetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("get;");
                }
                else
                {
                    result.Continue();
                    ProcessAccessModifier(property.GetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("get");
                    ProcessStatementBlock(property.Get, result);
                }
            }
            if (property.HasSetAccess)
            {
                if (property.Set == null)
                {
                    result.Continue();
                    ProcessAccessModifier(property.SetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("set;");
                }
                else
                {
                    result.Continue();
                    ProcessAccessModifier(property.SetAccess, result);
                    result.StartNewWord();
                    result.AppendLine("set");
                    ProcessStatementBlock(property.Set, result);
                }
            }
            result.Close();
            result.ContinueWith("}");
            if (property.Expression != null)
            {
                result.Append(" = ");
                property.Expression.ProcessMap(this, result);
                result.Append(";");
            }
            result.AppendLine();

        }

        public override void ProcessReturnStatement(CSharpReturnStatement statement, StringBuilder result)
        {
            result.Append("return");
            if (statement.Expression != null)
            {
                result.Append(" ");
                statement.Expression.ProcessMap(this, result);
            }
            result.Append(";");
        }

        public override void ProcessScript(CSharpScript script, StringBuilder result)
        {
            ProcessNamespace(script.RootNamespace, result);
        }

        public override void ProcessJointStatement(CSharpJointStatement statement, StringBuilder result)
        {
            foreach (CSharpStatement s in statement.Statements)
            {
                s.ProcessMap(this, result);
                result.Append(";");
            }
        }
        public override void ProcessStatementBlock(CSharpStatementBlock statementBlock, StringBuilder result, bool opened = false)
        {
            if(!opened)
            result.ContinueWithAndOpen("{");
            foreach(CSharpStatement statement in statementBlock.Statements)
            {
                result.StartNewLine();
                result.Continue();
                statement.ProcessMap(this, result);
            }
            result.StartNewLine();
            if(!opened)
            result.ContinueWithAsClose("}");
        }

        public override void ProcessStruct(CSharpStruct @struct, StringBuilder result)
        {
            foreach (CSharpEnum subenum in @struct.Subenums)
            {
                result.StartNewLine();
                ProcessEnumHeader(subenum, result);
                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessEnum(subenum, result);
                result.ContinueWithAsClose("}");
            }

            foreach (CSharpClass subclass in @struct.Subclasses)
            {
                result.StartNewLine();
                ProcessClassHeader(subclass, result);
                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessClass(subclass, result);
                result.ContinueWithAsClose("}");
            }

            foreach (CSharpStruct substruct in @struct.Substructs)
            {
                result.StartNewLine();
                ProcessStructHeader(substruct, result);
                result.AppendLine();
                result.ContinueWithAndOpen("{");
                ProcessStruct(substruct, result);
                result.ContinueWithAsClose("}");
            }

            foreach (CSharpDelegate @delegate in @struct.Delegates)
            {
                result.StartNewLine();
                ProcessDelegate(@delegate, result);
            }

            // Static fields and properties.
            foreach (CSharpVariable variable in @struct.StaticFields)
            {
                result.StartNewLine();
                result.Continue();
                ProcessVariable(variable, result);
                result.Append(";");
            }

            foreach (CSharpProperty property in @struct.StaticProperties)
            {
                result.StartNewLine();
                ProcessProperty(property, result);
            }

            // Instance fields and properties.
            foreach (CSharpVariable variable in @struct.InstanceFields)
            {
                result.StartNewLine();
                result.Continue();
                ProcessVariable(variable, result);
                result.Append(";");
            }

            foreach (CSharpProperty property in @struct.InstanceProperties)
            {
                result.StartNewLine();
                ProcessProperty(property, result);
            }

            // Constructors and destructors
            foreach (CSharpMethod method in @struct.InstanceConstructors)
            {
                result.StartNewLine();
                ProcessMethod(method, result);
            }

            if (@struct.InstanceDestructor != null)
            {
                result.StartNewLine();
                ProcessMethod(@struct.InstanceDestructor, result);
            }

            if (@struct.StaticConstructor != null)
            {
                result.StartNewLine();
                ProcessMethod(@struct.StaticConstructor, result);
            }

            if (@struct.StaticDestructor != null)
            {
                result.StartNewLine();
                ProcessMethod(@struct.StaticDestructor, result);
            }

            // Indexers
            foreach (CSharpIndexer indexer in @struct.Indexers)
            {
                result.StartNewLine();
                ProcessIndexer(indexer, result);
            }

            // Static methods
            foreach (CSharpMethod method in @struct.StaticMethods)
            {
                result.StartNewLine();
                ProcessMethod(method, result);
            }

            // Instance methods
            foreach (CSharpMethod method in @struct.InstanceMethods)
            {
                result.StartNewLine();
                ProcessMethod(method, result);
            }
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
                if (type.Name == "new")
                    result.Append("()"); // New type constraint
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

        /// <summary>
        /// Additional word seperators that a word can have behind it.
        /// </summary>
        public static char[] WordSeperators = new char[] { '(', '[' };
        public override void ProcessVariable(CSharpVariable variable, StringBuilder result)
        {
            ProcessAttributes(variable.Attributes, result);

            if(variable.Attributes.Length > 0)
            {
                result.StartNewLine();
                result.Continue();
            }

            ProcessAccessModifier(variable.AccessModifier, result);

            result.StartNewWord(additionalWordSeperators: WordSeperators);
            ProcessGeneralModifier(variable.GeneralModifier, result);

            result.StartNewWord(additionalWordSeperators: WordSeperators);
            ProcessType(variable.Type, result);

            result.StartNewWord();
            result.Append(variable.Name);
            
            if (variable.Expression != null)
            {
                result.Append(" = ");
                variable.Expression.ProcessMap(this, result);
            }
        }

        public override void ProcessClassHeader(CSharpClass @class, StringBuilder result)
        {
            ProcessAttributes(@class.Attributes, result);

            result.Continue();
            ProcessAccessModifier(@class.AccessModifier, result);
            result.StartNewWord();

            ProcessGeneralModifier(@class.GeneralModifier, result);
            result.StartNewWord();

            result.Append($"class {@class.Name}");
            if (@class.TypeParameters.Length > 0)
            {
                result.Append("<");
                ProcessTypeParameters(@class.TypeParameters, result);
                result.Append(">");
            }

            if (@class.Inherits.Length > 0)
            {
                result.Append(" : ");
                ProcessTypeParameters(@class.Inherits, result);
            }

            if (@class.TypeConstraints.Count > 0)
            {

                foreach (string typeparam in @class.TypeParameters)
                {
                    CSharpType[] constraints = null;
                    if (@class.TypeConstraints.TryGetValue(typeparam, out constraints))
                    {
                        result.Append(" where ");
                        bool appended = false;
                        foreach (CSharpType type in constraints)
                        {
                            if (appended)
                                result.Append(", ");
                            result.Append($"{typeparam} : ");
                            ProcessType(type, result);
                            appended = true;
                        }
                    }
                }
            }
        }

        public override void ProcessStructHeader(CSharpStruct @struct, StringBuilder result)
        {
            ProcessAttributes(@struct.Attributes, result);

            result.Continue();
            ProcessAccessModifier(@struct.AccessModifier, result);
            result.StartNewWord();

            ProcessGeneralModifier(@struct.GeneralModifier, result);
            result.StartNewWord();

            result.Append($"struct {@struct.Name}");
            if (@struct.TypeParameters.Length > 0)
            {
                result.Append("<");
                ProcessTypeParameters(@struct.TypeParameters, result);
                result.Append(">");
            }

            if (@struct.TypeConstraints.Count > 0)
            {
                foreach (string typeparam in @struct.TypeParameters)
                {
                    CSharpType[] constraints = null;
                    if (@struct.TypeConstraints.TryGetValue(typeparam, out constraints))
                    {
                        result.Append(" where ");
                        bool appended = false;
                        foreach (CSharpType type in constraints)
                        {
                            if (appended)
                                result.Append(", ");
                            result.Append($"{typeparam} : ");
                            ProcessType(type, result);
                            appended = true;
                        }
                    }
                }
            }
        }

        public override void ProcessEnumHeader(CSharpEnum @enum, StringBuilder result)
        {
            ProcessAttributes(@enum.Attributes, result);

            result.Continue();
            ProcessAccessModifier(@enum.AccessModifier, result);
            result.StartNewWord();

            result.Append($"enum {@enum.Name}");

            if (@enum.Inherits.Length > 0)
            {
                result.Append(" : ");
                ProcessTypeParameters(@enum.Inherits, result);
            }
        }

        public override void ProcessThisExpression(CSharpThisExpression expression, StringBuilder result)
        {
            result.Append("this");
        }

        public override void ProcessTriviaDocumentation(CSharpDoc doc, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTriviaComment(CSharpComment comment, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessSwitchStatement(CSharpSwitchStatement statement, StringBuilder result)
        {
            result.Append("switch (");
            statement.Expression.ProcessMap(this, result);
            result.Append(")");
            result.StartNewLine();
            result.ContinueWithAndOpen("{");
            foreach(CSharpSwitchSection @case in statement.Sections)
            {
                foreach(CSharpExpression labelExpressison in @case.Labels)
                {
                    result.StartNewLine();
                    result.ContinueWith("case ");
                    labelExpressison.ProcessMap(this, result);
                    result.Append(":");
                }
                result.Open();
                ProcessStatementBlock(@case.Block, result, true);
                result.Close();
                result.StartNewLine();
            }
            result.ContinueWithAsClose("}");
        }

        public override void ProcessIfStatement(CSharpIfStatement statement, StringBuilder result)
        {
            result.Append("if (");
            statement.Condition.ProcessMap(this, result);
            result.Append(")");
            ProcessStatementBlock(statement.Block, result);
        }

        public override void ProcessClosedExpression(CSharpClosedExpression expression, StringBuilder result)
        {
            result.Append("(");
            expression.Expression.ProcessMap(this, result);
            result.Append(")");
        }

        public override void ProcessCastExpression(CSharpCastExpression expression, StringBuilder result)
        {
            result.Append("(");
            ProcessType(expression.Type, result);
            result.Append(")");
            expression.Expression.ProcessMap(this, result);
        }

        public override void ProcessPrefixUnaryExpression(CSharpPrefixUnaryExpression expression, StringBuilder result)
        {
            result.Append(expression.Operator);
            expression.On.ProcessMap(this, result);
        }

        public override void ProcessInterpolatedStringExpression(CSharpInterpolatedStringExpression expression, StringBuilder result)
        {
            result.Append("$\"");
            foreach(CSharpExpression expr in expression.Contents)
            {
                if(expr.Syntax is InterpolatedStringTextSyntax)
                {
                    expr.ProcessMap(this, result);
                }
                else
                {
                    // Content expression
                    result.Append('{');
                    expr.ProcessMap(this, result);
                    result.Append('}');
                }
            }
            result.Append("\"");
        }

        public override void ProcessArrayCreationExpression(CSharpArrayCreationExpression expression, StringBuilder result)
        {
            result.Append("new ");
            ProcessType(expression.Type, result);
            result.StartNewWord();
            foreach (CSharpExpression[] rank in expression.Ranks)
            {
                result.Append("[");
                if(rank != null)
                    ProcessExpressions(rank, result);
                result.Append("]");
            }
            result.Append(" { ");
            ProcessExpressions(expression.Initializer, result);
            result.Append(" }");
        }

        public override void ProcessIndexAccessExpression(CSharpIndexAccessExpression expression, StringBuilder result)
        {
            expression.Member.ProcessMap(this, result);
            result.Append('[');
            ProcessExpressions(expression.Arguments, result);
            result.Append(']');
        }

        public override void ProcessPostfixUnaryExpression(CSharpPostfixUnaryExpression expression, StringBuilder result)
        {
            expression.On.ProcessMap(this, result);
            result.Append(expression.Operator);
        }

        public override void ProcessWhileStatement(CSharpWhileStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessForStatement(CSharpForStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessForeachStatement(CSharpForeachStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTupleExpression(CSharpTupleExpression expression, StringBuilder result)
        {
            result.Append("(");
            if(expression.Designations != null)
            {
                if (expression.Designations.Length > 0)
                {
                    if (expression.Designations[0] != null)
                        ProcessVariable(expression.Designations[0], result);

                    for(int i = 1; i<expression.Designations.Length; i++)
                    {
                        result.Append(", ");
                        if (expression.Designations[i] != null)
                            ProcessVariable(expression.Designations[i], result);
                    }
                }
            }
            else
            {
                ProcessExpressions(expression.TupleElements, result);
            }
            result.Append(")");
        }

        public override void ProcessTryStatement(CSharpTryStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessCheckedContextExpression(CSharpCheckedContextExpression expression, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessDoWhileStatement(CSharpDoWhileStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessThrowStatement(CSharpThrowStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessBreakStatement(CSharpBreakStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessContinueStatement(CSharpContinueStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessYieldStatement(CSharpYieldStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessGotoStatement(CSharpGotoStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessLabelStatement(CSharpLabelStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessCheckedContextStatement(CSharpCheckedContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessUnsafeContextStatement(CSharpUnsafeContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessFixedContextStatement(CSharpFixedContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessLockedContextStatement(CSharpLockedContextStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public override void ProcessUsingStatement(CSharpUsingStatement statement, StringBuilder result)
        {
            throw new NotImplementedException();
        }
    }
}
