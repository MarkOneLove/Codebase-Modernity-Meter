﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeModernityMeter1
{
    internal class Program
    {

        static void TraverseAST(SyntaxNode node, AnalysisData data, SemanticModel model, string indent)
        {
            // Print the type of the current node with indentation
            Console.WriteLine($"{indent}Node kind: {node.Kind()}");

            // ===== FEATURES INTODUCED IN 7.1 =====

            // C# 7.1: Pattern-matching with generics (_ is & as type Identifier), previously no Identifier
            if (node is IsPatternExpressionSyntax isPatternExpression)
            {
                Console.WriteLine("Found pattern matching in generic method --> 7.1");
                data.dataStruct[7.1] += 1;
            }

            // C# 7.1: Infer tuple names (NOT DONE -> Complex)
            if (node.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                var assignmentExpression = (AssignmentExpressionSyntax)node;

                // Check if the right-hand side of the assignment is a tuple deconstruction
                if (assignmentExpression.Right.IsKind(SyntaxKind.TupleExpression))
                {
                    var tupleExpression = (TupleExpressionSyntax)assignmentExpression.Right;

                    // Check if tuple elements have explicitly specified names
                    if (tupleExpression.Arguments.All(argument => argument.NameColon == null))
                    {
                        Console.WriteLine($"Inferred tuple element names detected --> 7.1");
                        data.dataStruct[7.1] += 1;
                    }
                }
            }

            // C# 7.1: Default expressions
            if (node is DefaultExpressionSyntax)
            {
                Console.WriteLine("Found default expression --> 7.1");
                data.dataStruct[7.1] += 1;
            }
            else if (node is LiteralExpressionSyntax literalExpression1 && literalExpression1.ToString().Equals("default"))
            {
                Console.WriteLine("Found default expression --> 7.1");
                data.dataStruct[7.1] += 1;
            }



            // C# 7.1: Async Main (return Task/Task<int>)
            if (node is MethodDeclarationSyntax methodDeclaration)
            {
                if (methodDeclaration.ReturnType.ToString().Contains("Task"))
                {
                    //Console.WriteLine($"{methodDeclaration.ReturnType.ToString()}");
                    Console.WriteLine("Found Task as return --> 7.1");
                    data.dataStruct[7.1] += 1;
                }
            }

            // ===== FEATURES INTODUCED IN 7.2 =====

            // C# 7.2: private protected for member declarations
            if (node is MemberDeclarationSyntax memberDeclaration)
            {
                // Check if the modifiers contain both "private" and "protected"
                bool hasPrivate = false;
                bool hasProtected = false;

                foreach (var modifier in memberDeclaration.Modifiers)
                {
                    hasPrivate = modifier.IsKind(SyntaxKind.PrivateKeyword);
                    hasProtected = modifier.IsKind(SyntaxKind.ProtectedKeyword);
                }

                if (hasPrivate && hasProtected)
                {
                    // Do SMTH
                    Console.WriteLine("Field with 'private protected' modifier --> 7.2");
                    data.dataStruct[7.2] += 1;
                }
            }

            // C# 7.2: Non-trailing named arguments (If its n-1 params like this its old version, if its less than that and code works means its 7.2???) (FIX WITH WHAT VADIM SAID)
            if (node is InvocationExpressionSyntax invocationDeclaration)
            {
                int argumentCount = 0;
                int colonCount = 0;
                // Console.WriteLine($"{invocationDeclaration.ChildNodes}");
                foreach (SyntaxNode child in node.ChildNodes()) // Argumentlist before argument
                {
                    if (child is ArgumentListSyntax argumentList)
                    {
                        foreach (SyntaxNode argumentListChild in child.ChildNodes())
                        {
                            argumentCount++;
                            foreach (SyntaxNode childChild in argumentListChild.ChildNodes())
                            {
                                if (childChild is NameColonSyntax)
                                {
                                    colonCount++;
                                }
                            }
                        }
                    }
                }
                // Console.WriteLine($"{argumentCount} vs {colonCount}");
                if (colonCount < argumentCount - 1)
                {
                    Console.WriteLine("Non-trailing named arguments --> 7.2");
                    data.dataStruct[7.2] += 1;
                }
            }

            // C# 7.2: Allow digit separator after 0b or 0x
            if (node is LiteralExpressionSyntax literalExpression)
            {
                // Get the text representation of the literal
                string literalText = literalExpression.Token.Text;

                // Check if contains an underscore after 0x or 0b
                if (literalText.Contains("0b_") || literalText.Contains("0x_"))
                {
                    Console.WriteLine("Found digit separator after 0b or 0x --> 7.2");
                    data.dataStruct[7.2] += 1;
                }
            }

            // C# 7.2: Conditional ref expressions
            if (node is ConditionalExpressionSyntax conditionalExpression)
            {
                // Check if the conditional expression is used as a ref
                if (IsRefConditionalExpression(conditionalExpression))
                {
                    Console.WriteLine("Found conditional ref expression --> 7.2");
                    data.dataStruct[7.2] += 1;
                }
            }

            // C# 7.2: Readonly references: in parameters, ref readonly returns, readonly structs,
            // ref/in extension methods, ref readonly locals, ref conditional expressions

            // C# 7.2: in keyword as a modifier in the parameter signature
            if (node is ParameterSyntax parameterExpression)
            {
                // Check if the parameter has the 'in' modifier
                if (parameterExpression.Modifiers.Any(SyntaxKind.InKeyword))
                {
                    Console.WriteLine("Found IN keyword in parameter --> 7.2");
                    data.dataStruct[7.2] += 1;
                }
            }

            // C# 7.2: in keyword in arguments to match with in params
            if (node is ArgumentSyntax argumentExpr)
            {
                // Check if the argument has the 'in' modifier
                if (argumentExpr.RefKindKeyword.IsKind(SyntaxKind.InKeyword))
                {
                    Console.WriteLine("Found IN keyword in argument --> 7.2");
                    data.dataStruct[7.2] += 1;
                }
            }

            // C# 7.2: Returning by readonly reference


            // ===== FEATURES INTODUCED IN 7.3 =====

            // C# 7.3: Unmanaged type constraint
            if (node is TypeParameterConstraintClauseSyntax constraintClause)
            {
                foreach (var constraint in constraintClause.Constraints)
                {
                    if (constraint is TypeConstraintSyntax typeConstraint &&
                        typeConstraint.Type is IdentifierNameSyntax identifier &&
                        identifier.Identifier.Text == "unmanaged")
                    {
                        Console.WriteLine("Found unmanaged constraint --> 7.3");
                        data.dataStruct[7.3] += 1;
                    }
                }
            }

            // C# 7.3: Support == and != for tuples
            if (node is BinaryExpressionSyntax binaryExpression)
            {
                // Check if the binary expression uses '==' or '!=' operators
                if (node.IsKind(SyntaxKind.EqualsExpression) || node.IsKind(SyntaxKind.NotEqualsExpression))
                {
                    //var test = model.GetTypeInfo(binaryExpression.Left);
                    //Console.WriteLine($"{test.Type.IsTupleType}");
                    //Console.WriteLine($"{test.Type.Name is Tuple}");

                    // Check if both sides of the binary expression are tuples with semantic model
                    if (model.GetTypeInfo(binaryExpression.Left).Type.IsTupleType && model.GetTypeInfo(binaryExpression.Right).Type.IsTupleType)
                    {
                        Console.WriteLine($"Found != or == for tuples --> 7.3");
                        data.dataStruct[7.3] += 1;
                    }
                }
            }

            // C# 7.3: Auto-Implemented Property Field-Targeted Attributes: smth like that [field: NonSerialized]
            if (node is PropertyDeclarationSyntax propertyDeclaration)
            {
                // Check the attributes of the property
                foreach (var attributeList in propertyDeclaration.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        // Remove attribute from attribute list and see if it contains field
                        if (attributeList.ToString().Replace(attribute.ToString(), "").Contains("field"))
                        {
                            Console.WriteLine($"Found field-targeted attribute --> 7.3");
                            data.dataStruct[7.3] += 1;
                        }
                    }
                }
            }

            // C# 7.3: Stackalloc array initializers
            if (node is StackAllocArrayCreationExpressionSyntax stackAllocExpression)
            {
                // Check if the stackalloc expression has an initializer
                if (stackAllocExpression.Initializer != null)
                {
                    Console.WriteLine("Found stackalloc array initializers --> 7.3");
                    data.dataStruct[7.3] += 1;
                }
            }

            // C# 7.3: fixed-sized buffers
            // List of types newly supported in fixed-size buffers in C# 7.3
            var newFixedBufferTypes = new[] { "bool", "char", "short", "ushort" };

            if (node is StructDeclarationSyntax structDeclaration)
            {
                // Traverse the members of the struct
                foreach (var member in structDeclaration.Members)
                {
                    // Check if the member is a field declaration with fixed-size buffer
                    if (member is FieldDeclarationSyntax fieldDeclaration)
                    {
                        // Get the type of the fixed-size buffer
                        var variableType = fieldDeclaration.Declaration.Type;

                        //Console.WriteLine($"TEST: {fieldDeclaration.Declaration.Type}");
                        if (variableType != null)
                        {
                            if (newFixedBufferTypes.Contains(variableType.ToString()))
                            {
                                Console.WriteLine($"Found fixed-size buffer --> 7.3");
                                data.dataStruct[7.3] += 1;
                                /*foreach (var variable in fieldDeclaration.Declaration.Variables)
                                {
                                    if (variable is VariableDeclaratorSyntax variableDeclarator && variableDeclarator.ArgumentList != null)
                                    {
                                        Console.WriteLine($"Found fixed-size buffer --> 7.3");
                                    }
                                }*/
                            }
                        }
                    }
                }
            }

            // C# 7.3: Expression variables in initializers
            // Check if the current node is an object initializer
            if (node is InitializerExpressionSyntax initializer && initializer.Kind().ToString().Equals("ObjectInitializerExpression"))
            {
                // Traverse the expressions within the initializer
                foreach (var expression in initializer.Expressions)
                {
                    //Console.WriteLine($"{node.Kind()}");
                    //Console.WriteLine($"{node.GetType()}");
                    // Check if the 2nd part of expression is not a simple assignment or variable reference
                    try
                    {
                        if (!(expression.ChildNodes().Last() is IdentifierNameSyntax) && !(expression.ChildNodes().Last() is AssignmentExpressionSyntax))
                        {
                            Console.WriteLine("Found expression in object initializer --> 7.3");
                            data.dataStruct[7.3] += 1;
                        }
                    }
                    catch (Exception _) { }
                }
            }

            // ===== FEATURES INTODUCED IN 8.0 =====

            // C# 8.0: Readonly Instance Members
            if (node is MethodDeclarationSyntax method && method.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
            {
                Console.WriteLine($"Readonly method found --> 8.0");
                data.dataStruct[8.0] += 1;
            }
            else if (node is PropertyDeclarationSyntax property && property.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
            {
                Console.WriteLine($"Readonly property found --> 8.0");
                data.dataStruct[8.0] += 1;
            }

            // C# 8.0: Static local functions
            // Check if the current node is a local function declaration with a static modifier
            if (node is LocalFunctionStatementSyntax localFunction && localFunction.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                Console.WriteLine($"Static local function found --> 8.0");
                data.dataStruct[8.0] += 1;
            }

            // C# 8.0: Unmanaged constructed types
            // Check if the current node is a type parameter constraint clause
            // !!! ADDED IN 7.3
            /*if (node is TypeParameterConstraintClauseSyntax constraintClause1)
            {
                foreach (var constraint in constraintClause1.Constraints)
                {
                    if (constraint is TypeConstraintSyntax typeConstraint && typeConstraint.Type is IdentifierNameSyntax identifierName)
                    {
                        if (identifierName.Identifier.Text == "unmanaged")
                        {
                            Console.WriteLine($"Unmanaged constraint found --> 8.0");
                        }
                    }
                }
            }*/

            // C# 8.0: Stackalloc in nested contexts
            // Check if the current node is a stackalloc array creation expression
            if (node is StackAllocArrayCreationExpressionSyntax stackAlloc)
            {
                var parent = stackAlloc.Parent;
                while (parent != null && !(parent is BlockSyntax))
                {
                    // If stackalloc is found within an expression context, report it
                    if (parent is ExpressionSyntax)
                    {
                        Console.WriteLine($"stackalloc found in nested expression --> 8.0");
                        data.dataStruct[8.0] += 1;
                        break;
                    }
                    parent = parent.Parent;
                }
            }

            // C# 8.0: Alternative interpolated verbatim strings
            if (node is InterpolatedStringExpressionSyntax interpolatedString)
            {
                var check = interpolatedString.GetFirstToken();
                if (check.Text.StartsWith("@$") || check.Text.StartsWith("$@"))
                {
                    Console.WriteLine($"$@ or @$ string declaration found --> 8.0");
                    data.dataStruct[8.0] += 1;
                }
            }

            // C# 8.0: Ranges: System.Index and System.Range, and ^
            // Check for range expressions
            if (node is RangeExpressionSyntax rangeExpression)
            {
                Console.WriteLine($"Detected range expression --> 8.0");
                data.dataStruct[8.0] += 1;
            }

            // Check for Index type
            if (node is IdentifierNameSyntax indexType && indexType.Identifier.Text == "Index")
            {
                Console.WriteLine($"Detected Index type --> 8.0");
                data.dataStruct[8.0] += 1;
            }

            // Check for Range type
            if (node is IdentifierNameSyntax rangeType && rangeType.Identifier.Text == "Range")
            {
                Console.WriteLine($"Detected Range type --> 8.0");
                data.dataStruct[8.0] += 1;
            }

            // C# 8.0: Enhanced using: "pattern-based using" and "using declarations"
            if (node is UsingStatementSyntax usingStatement)
            {
                Console.WriteLine($"Detected pattern-based using --> 8.0");
                data.dataStruct[8.0] += 1;
            }
            if (node is LocalDeclarationStatementSyntax localDeclaration && localDeclaration.UsingKeyword.ToString().Equals("using"))
            {
                Console.WriteLine($"Detected using declaration --> 8.0");
                data.dataStruct[8.0] += 1;
            }

            // C# 8.0: Recursive Pattern Matching
            // Check if the node is a switch statement
            if (node is SwitchStatementSyntax switchStatement)
            {
                // Check for case patterns with nested patterns
                var casePatterns = switchStatement.DescendantNodes().OfType<CasePatternSwitchLabelSyntax>();
                foreach (var casePattern in casePatterns)
                {
                    if (casePattern.Pattern.DescendantNodes().OfType<DeclarationPatternSyntax>().Any())
                    {
                        Console.WriteLine("Recursive pattern matching detected in switch statement --> 8.0");
                        data.dataStruct[8.0] += 1;
                        return;
                    }
                }
            }

            // C# 8.0: default interface methods
            if (node is InterfaceDeclarationSyntax interfaceDeclaration)
            {
                foreach (var member in interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    if (member.Body != null)
                    {
                        Console.WriteLine($"Default interface method detected --> 8.0");
                        data.dataStruct[8.0] += 1;
                    }
                }
            }

            // ===== FEATURES INTODUCED IN 9.0 =====

            // C# 9.0: Target-typed new expression
            if (node is ImplicitObjectCreationExpressionSyntax implicitNew)
            {
                Console.WriteLine("Target-typed new expression detected --> 9.0");
                data.dataStruct[9.0] += 1;
            }

            // C# 9.0: SkipLocalsInit Attribute
            if (node is AttributeSyntax attributeArgument && attributeArgument.ToString().Equals("SkipLocalsInit"))
            {
                //Console.WriteLine($"{attributeArgument}");
                Console.WriteLine("SkipLocalsInit detected  --> 9.0");
                data.dataStruct[9.0] += 1;
            }

            // C# 9.0: Lambda discard parameters
            if (node is SimpleLambdaExpressionSyntax simpleLambda)
            {
                // Check if the parameter is a discard
                if (simpleLambda.Parameter.Identifier.Text == "_")
                {
                    Console.WriteLine("Discard parameter detected in simple lambda expression --> 9.0");
                    data.dataStruct[9.0] += 1;
                }
            }
            else if (node is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                // Check if any of the parameters are discards
                foreach (var parameter in parenthesizedLambda.ParameterList.Parameters)
                {
                    if (parameter.Identifier.Text == "_")
                    {
                        Console.WriteLine("Discard parameter detected in parenthesized lambda expression --> 9.0");
                        data.dataStruct[9.0] += 1;
                        break;
                    }
                }
            }

            // C# 9.0: Native-sized integers
            if (node is VariableDeclarationSyntax variableDeclaration)
            {
                foreach (var variable in variableDeclaration.Variables)
                {
                    if (variableDeclaration.Type.ToString() == "nint" || variableDeclaration.Type.ToString() == "nuint")
                    {
                        Console.WriteLine($"Native-sized integer detected --> 9.0");
                        data.dataStruct[9.0] += 1;
                    }
                }
            }

            // C# 9.0: Attributes on local functions
            if (node is LocalFunctionStatementSyntax localFunction1)
            {
                if (localFunction1.AttributeLists.Count > 0)
                {
                    Console.WriteLine($"Local function '{localFunction1.Identifier}' has attributes -->  9.0");
                    data.dataStruct[9.0] += 1;
                }
            }

            // C# 9.0: Function pointers
            if (node is FunctionPointerTypeSyntax functionPointerType)
            {
                Console.WriteLine($"Function pointer detected --> 9.0");
                data.dataStruct[9.0] += 1;
            }

            // C# 9.0: Pattern matching improvements
            /*if (node is IsPatternExpressionSyntax isPattern)
            {
                Console.WriteLine($"Pattern match detected -->  9.0");
                data.dataStruct[9.0] += 1;
            }*/

            // C# 9.0: Static lambdas
            if (node is ParenthesizedLambdaExpressionSyntax lambda && lambda.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                Console.WriteLine($"Static lambda detected");
                data.dataStruct[9.0] += 1;
            }
            if (node is SimpleLambdaExpressionSyntax simpleLambda1 && simpleLambda1.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                Console.WriteLine($"Static lambda detected --> 9.0");
                data.dataStruct[9.0] += 1;
            }

            // C# 9.0: Records
            if (node is RecordDeclarationSyntax recordDeclaration)
            {
                Console.WriteLine($"Record detected --> 9.0");
                data.dataStruct[9.0] += 1;
            }

            // C# 9.0: Targeted type expressions
            if (node is ConditionalExpressionSyntax conditionalExpression1)
            {
                // Check if the conditional expression uses target-typed conditional expression syntax
                if (conditionalExpression1.Condition is LiteralExpressionSyntax &&
                    conditionalExpression1.WhenTrue is LiteralExpressionSyntax &&
                    conditionalExpression1.WhenFalse is LiteralExpressionSyntax)
                {
                    Console.WriteLine("Target-typed conditional expression detected --> 9.0");
                    data.dataStruct[9.0] += 1;
                }
            }

            // C# 9.0: Covariant returns
            if (node is MethodDeclarationSyntax method1)
            {
                // Get the symbol for the method
                var methodSymbol = model.GetDeclaredSymbol(method1);
                //Console.WriteLine($"{methodSymbol}");

                // Check if the method overrides a base method
                var baseMethodSymbol = methodSymbol?.OverriddenMethod;
                // Console.WriteLine($"{baseMethodSymbol}");
                if (baseMethodSymbol != null && methodSymbol != null)
                {
                    // Console.WriteLine($"{methodSymbol}");
                    // Console.WriteLine($"{methodSymbol.OverriddenMethod}");
                    // Console.WriteLine($"{baseMethodSymbol}");
                    if (methodSymbol.OverriddenMethod == baseMethodSymbol && methodSymbol.ReturnType == baseMethodSymbol.ReturnType)
                    {
                        Console.WriteLine($"Covariant return detected in method --> 9.0");
                        data.dataStruct[9.0] += 1;
                    }
                }
            }

            // C# 9.0: Extension GetEnumerator Recognition in foreach
            // Check if the node is a foreach loop
            if (node is ForEachStatementSyntax foreachSyntax)
            {
                // Get the type of the collection being iterated over
                var collectionType = model.GetTypeInfo(foreachSyntax.Expression).Type;

                if (collectionType != null)
                {
                    // Check if there are any extension methods providing GetEnumerator() for the collection type
                    var extensions = GetExtensionMethods(model, collectionType, "GetEnumerator");
                    //Console.WriteLine($"{extensions.Count()}");
                    if (extensions.Any())
                    {
                        Console.WriteLine("Foreach loop using extension GetEnumerator --> 9.0");
                        data.dataStruct[9.0] += 1;
                        //Console.WriteLine(foreachSyntax);
                    }
                }
            }

            // C# 9.0: Module initializers
            if (node is MethodDeclarationSyntax methodSyntax)
            {
                // Check if the method has ModuleInitializerAttribute
                if (HasModuleInitializerAttribute(methodSyntax))
                {
                    Console.WriteLine($"Module initializer found --> 9.0");
                    data.dataStruct[9.0] += 1;
                }
            }

            // C# 9.0: Top level statements
            // Check if the node represents a global statement (top-level statement)
            if (node is GlobalStatementSyntax globalStatement)
            {
                Console.WriteLine("Top-level statement found --> 9.0");
                data.dataStruct[9.0] += 1;
            }

            // ===== FEATURES INTODUCED IN 10.0 =====

            // C# 10.0: record struct declaration
            if (node is RecordDeclarationSyntax recordDeclaration1 &&
                recordDeclaration1.Kind() == SyntaxKind.RecordStructDeclaration)
            {
                Console.WriteLine($"Record struct found --> 10.0");
                data.dataStruct[10.0] += 1;
            }

            // C# 10.0: global using
            if (node is UsingDirectiveSyntax usingDirective && usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
            {
                Console.WriteLine($"Global using directive found --> 10.0");
                data.dataStruct[10.0] += 1;
            }

            // C# 10.0: Constant interpolated strings
            if (node is FieldDeclarationSyntax fieldDeclaration1 && fieldDeclaration1.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                // Check if the field has an interpolated string initializer
                foreach (var variable in fieldDeclaration1.Declaration.Variables)
                {
                    if (variable.Initializer?.Value is InterpolatedStringExpressionSyntax interpolatedString1)
                    {
                        Console.WriteLine($"Constant interpolated string found --> 10.0");
                        data.dataStruct[10.0] += 1;
                    }
                }
            }

            // C# 10.0: Extended property patterns | AM I SURE?????????
            if (node is RecursivePatternSyntax recursivePattern)
            {
                Console.WriteLine($"Extended property pattern found --> 10.0");
                data.dataStruct[10.0] += 1;
            }

            // C# 10.0: Records with sealed base toString override
            if (node is RecordDeclarationSyntax recordDeclaration2)
            {
                // Iterate over the members of the record declaration
                foreach (var member in recordDeclaration2.Members)
                {
                    // Check if the member is a method declaration for ToString
                    if (member is MethodDeclarationSyntax methodDeclaration2 &&
                        methodDeclaration2.Identifier.Text == "ToString" &&
                        methodDeclaration2.Modifiers.Any(SyntaxKind.SealedKeyword) &&
                        methodDeclaration2.Modifiers.Any(SyntaxKind.OverrideKeyword))
                    {
                        Console.WriteLine($"Sealed ToString method found in record --> 10.0");
                        data.dataStruct[10.0] += 1;
                    }
                }
            }

            // C# 10.0: Mix Declarations and Variables in Deconstruction
            // Ton of different mixex declarations
            /*if (node is AssignmentExpressionSyntax assignment && assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                if (assignment.Left is TupleExpressionSyntax tuple)
                {
                    bool hasDeclaration = false;
                    bool hasIdentifier = false;

                    foreach (var element in tuple.Arguments)
                    {
                            hasDeclaration = element.Expression is DeclarationExpressionSyntax | false;
                            hasIdentifier = element.Expression is IdentifierNameSyntax | false;
                    }

                    if (hasDeclaration && hasIdentifier)
                    {
                        Console.WriteLine("Mixed declarations and variables found in deconstruction --> 10.0");
                        data.dataStruct[10.0] += 1;
                    }
                }
            }*/

            // C# 10.0: Allow [AsyncMethodBuilder(...)] on methods
            // Check if the node is a method declaration
            if (node is MethodDeclarationSyntax methodDeclaration3)
            {
                // Check if the method has an attribute list
                foreach (var attributeList in methodDeclaration3.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        // Check if the attribute is AsyncMethodBuilder
                        if (attribute.Name.ToString() == "AsyncMethodBuilder" ||
                            attribute.Name is QualifiedNameSyntax qn && qn.Right.Identifier.Text == "AsyncMethodBuilder")
                        {
                            Console.WriteLine($"AsyncMethodBuilder attribute found on method --> 10.0");
                            data.dataStruct[10.0] += 1;
                        }
                    }
                }
            }

            // C# 10.0: Enhanced #line directives
            // Check if the node is a line directive
            /*if (node is LineDirectiveTriviaSyntax lineDirective)
            {
                if (lineDirective.Line.Kind() == SyntaxKind.HiddenKeyword)
                {
                    Console.WriteLine("#line hidden directive found --> 10.0");
                    data.dataStruct[10.0] += 1;
                }
                else if (lineDirective.Line.Kind() == SyntaxKind.DefaultKeyword)
                {
                    Console.WriteLine("#line default directive found --> 10.0");
                    data.dataStruct[10.0] += 1;
                }
            }*/


            // Just printing the tree with more details
            // Print additional details of the current node (if any)
            if (node is MethodDeclarationSyntax methodNode)
            {
                Console.WriteLine($"{indent}  Method name: {methodNode.Identifier}");
                //Console.WriteLine($"{indent}  Method name: TestName");
            }
            else if (node is BaseTypeDeclarationSyntax typeNode)
            {
                Console.WriteLine($"{indent}  Type name: {typeNode.Identifier}");
            }
            else if (node is NamespaceDeclarationSyntax namespaceNode)
            {
                Console.WriteLine($"{indent}  Namespace name: {namespaceNode.Name}");
            }

            // Traverse the children of the current node (INTERPETER PART)
            foreach (var child in node.ChildNodes())
            {
                TraverseAST(child, data, model, indent + "  ");
            }
        }

        static async Task Main(string[] args)
        {
            // <|><|><|> EDIT TEST CASE HERE <|><|><|>
            string testCase = TestProgrames.lineProgramme;

            SyntaxTree tree = CSharpSyntaxTree.ParseText(testCase);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            // Attempt to get version
            CSharpParseOptions parseOptions = (CSharpParseOptions)tree.Options;
            LanguageVersion languageVersion = parseOptions.LanguageVersion;

            // Create a compilation for the syntax tree
            var compilation = CSharpCompilation.Create("MyCompilation",
                new[] { tree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            // Get the semantic model
            var semanticModel = compilation.GetSemanticModel(tree);

            // Add all language versions
            AnalysisData analysisData = new AnalysisData();
            analysisData.dataStruct.Add(7.1, 0);
            analysisData.dataStruct.Add(7.2, 0);
            analysisData.dataStruct.Add(7.3, 0);
            analysisData.dataStruct.Add(8.0, 0);
            analysisData.dataStruct.Add(9.0, 0);
            analysisData.dataStruct.Add(10.0, 0);

            // Some queries for specific features

            // C# 8.0: Null-coalescing Assignment - addition of ??=
            var nullCoalescingAssignments = root.DescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .Where(aes => aes.Kind() == SyntaxKind.CoalesceAssignmentExpression);

            foreach (var assignment in nullCoalescingAssignments)
            {
                Console.WriteLine($"Detected '??=' --> 8.0");
                analysisData.dataStruct[8.0] += 1;
            }

            // C# 8.0: Async Streams: Find methods returning IAsyncEnumerable<T>
            var asyncEnumerableMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(method => method.ReturnType.ToString() == "IAsyncEnumerable<int>");

            foreach (var method in asyncEnumerableMethods)
            {
                Console.WriteLine($"Method returning IAsyncEnumerable<T> detected --> 8.0");
                analysisData.dataStruct[8.0] += 1;
            }

            // Find await foreach statements (NOT WORKING)
            var awaitExpressionsInsideForeach = root.DescendantNodes()
                .OfType<ForEachStatementSyntax>()
                .SelectMany(foreachStatement => foreachStatement.DescendantNodes().OfType<AwaitExpressionSyntax>());

            foreach (var awaitExpression in awaitExpressionsInsideForeach)
            {
                Console.WriteLine("await foreach statement detected --> 8.0");
                analysisData.dataStruct[8.0] += 1;
            }

            // Start interpreter
            TraverseAST(root, analysisData, semanticModel, "");

            // Print results
            Console.WriteLine("Language version : Feature count");
            foreach (double key in analysisData.dataStruct.Keys)
            {
                Console.WriteLine($"{key} : {analysisData.dataStruct[key]}");
            }
            //Console.WriteLine($"The tree is a {root.Kind()} node.");
            //Console.WriteLine($"The tree has {root.Members.Count} elements in it.");
            //Console.WriteLine($"The tree has {root.Usings.Count} using statements. They are:");
            //Console.WriteLine($"TEST: {root}");
            //foreach (UsingDirectiveSyntax element in root.Usings)
            //Console.WriteLine($"\t{element.Name}");

        }

        // Class to store feature versions and all the data -> ToBeDeveloped
        public class AnalysisData
        {
            public Dictionary<double, int> dataStruct = new Dictionary<double, int>();
        }

        static bool IsRefConditionalExpression(ConditionalExpressionSyntax conditionalExpression)
        {
            // Check if the conditional expression is used in a ref context and not without it
            return (conditionalExpression.WhenTrue is RefExpressionSyntax) && (conditionalExpression.WhenFalse is RefExpressionSyntax);
        }

        static IEnumerable<IMethodSymbol> GetExtensionMethods(SemanticModel semanticModel, ITypeSymbol type, string methodName)
        {
            var methods = semanticModel.Compilation.GetSymbolsWithName(methodName, SymbolFilter.Member)
                .OfType<IMethodSymbol>()
                .Where(m => m.IsExtensionMethod &&
                            m.Parameters.Length == 1 &&
                            m.Parameters[0].Type.Equals(type));

            return methods;
        }
        static bool HasModuleInitializerAttribute(MethodDeclarationSyntax methodSyntax)
        {
            return methodSyntax.AttributeLists
                .SelectMany(attrList => attrList.Attributes)
                .Any(attr => attr.Name.ToString() == "ModuleInitializer");
        }

    }
}