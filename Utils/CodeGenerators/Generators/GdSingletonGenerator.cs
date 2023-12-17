using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Abro.CodeGenerators.Extensions;
using CodeGenerators;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Abro.CodeGenerators.Generators;
[Generator]
public class GdSingletonGenerator : IIncrementalGenerator
{
    private static string ClassAttributeName = typeof(GdSingletonAttribute).FullName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sources = context.SyntaxProvider.ForAttributeWithMetadataName(ClassAttributeName, IsCandidate, Transform);
        context.AddSources(sources);
    }

    private static bool IsCandidate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    private static GeneratorResult Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.TargetNode;
        if (!classDeclaration.TryValidateType(out var @namespace, out var diagnostic))
        {
            return new GeneratorResult(diagnostic);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var singletonSourceText = CreateSingletonClass(@namespace, classDeclaration);

        return new GeneratorResult(classDeclaration.GetHintName(@namespace), singletonSourceText);
    }

    private static SourceText CreateSingletonClass(NameSyntax @namespace, ClassDeclarationSyntax classDeclaration)
    {
        var className = classDeclaration.Identifier.Text;
        
        var newMembers = new List<string>();
        var code = new StringBuilder();
        
        // Extract using directives from the syntax tree
        var usingDirectives = @namespace.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
        foreach (var usingDirective in usingDirectives)
        {
            code.AppendLine(usingDirective.NormalizeWhitespace().ToFullString());
        }
        
        code.AppendLine($"namespace {@namespace.GetText()};");
        if (classDeclaration.BaseList != null)
        {
            code.AppendLine($"public partial class {className} : {classDeclaration.BaseList.Types.First()}");
        }
        else
        {
            code.AppendLine($"public partial class {className}");
        }
        code.AppendLine("{");
        code.AppendLine($"\tprivate static {className} _instance;");
       // code.AppendLine("#if GODOT");
       // code.AppendLine("\tpublic override void _Ready() => _instance = this;");
       // code.AppendLine("#else");
        code.AppendLine($"\tpublic {className}() => _instance = this;");
        //code.AppendLine("#endif");
        
        
        foreach (var origMember in classDeclaration.Members)
        {
            // Perform public static accessor for every private field like this:
            // private string _name;
            // becomes
            // public static string Name => _instance._name;
            if (origMember is FieldDeclarationSyntax fieldDeclaration && fieldDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                foreach (var variable in fieldDeclaration.Declaration.Variables)
                {
                    var fieldName = variable.Identifier.Text;
                    newMembers.Add($"public static {fieldDeclaration.Declaration.Type} {GetNameFromField(fieldName)} => _instance.{fieldName};");
                }
            }
        }
        
        foreach (var newMember in newMembers)
        {
            code.AppendLine($"\t{newMember}");
        }
        
        code.AppendLine("}");
        var source = code.ToString();
        return SourceText.From(source, Encoding.UTF8);
    }
    
    private static string GetNameFromField(string field)
    {
        // Split the input field by underscore
        string[] words = field.Split('_');

        // Capitalize each word and concatenate them
        StringBuilder result = new StringBuilder();
        foreach (string word in words)
        {
            if (!string.IsNullOrEmpty(word))
            {
                result.Append(char.ToUpper(word[0]));
                result.Append(word.Substring(1));
            }
        }

        return result.ToString();
    }
}