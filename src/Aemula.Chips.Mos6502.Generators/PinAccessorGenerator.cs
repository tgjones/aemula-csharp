using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Aemula.Chips.Mos6502.Generators
{
    [Generator]
    public sealed class PinAccessorGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
            {
                return;
            }

            var pinAttributeSymbol = context.Compilation.GetTypeByMetadataName("Aemula.PinAttribute");
            var handleAttributeSymbol = context.Compilation.GetTypeByMetadataName("Aemula.HandleAttribute");

            // Gather all the fields decorated with [Pin(...)]
            var fieldSymbols = new List<(IFieldSymbol, AttributeData)>();
            foreach (var fieldSyntax in receiver.CandidateFields)
            {
                var semanticModel = context.Compilation.GetSemanticModel(fieldSyntax.SyntaxTree);
                foreach (var variableSyntax in fieldSyntax.Declaration.Variables)
                {
                    var fieldSymbol = (IFieldSymbol)semanticModel.GetDeclaredSymbol(variableSyntax);
                    var pinAttributeData = fieldSymbol.GetAttributes().FirstOrDefault(x => x.AttributeClass.Equals(pinAttributeSymbol, SymbolEqualityComparer.Default));
                    if (pinAttributeData != null)
                    {
                        fieldSymbols.Add((fieldSymbol, pinAttributeData));
                    }
                }
            }

            // Group the fields by class, and generate the source
            foreach (var group in fieldSymbols.GroupBy(f => f.Item1.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList(), handleAttributeSymbol);
                context.AddSource($"{group.Key.Name}.PinAccessors.g.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<(IFieldSymbol, AttributeData)> fields, ISymbol handleAttributeSymbol)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // Create properties for each field
            var generatedProperties = new StringBuilder();
            foreach (var fieldSymbol in fields)
            {
                ProcessField(generatedProperties, fieldSymbol, handleAttributeSymbol);
            }

            return $@"
namespace {namespaceName}
{{
    partial class {classSymbol.Name}
    {{
        {generatedProperties}
    }}
}}";
        }

        private void ProcessField(StringBuilder source, (IFieldSymbol, AttributeData) fieldSymbol, ISymbol handleAttributeSymbol)
        {
            // Get the name and type of the field.
            var fieldName = fieldSymbol.Item1.Name;
            var fieldType = fieldSymbol.Item1.Type;

            // Get the Pin attribute from the field, and pin direction.
            var attributeData = fieldSymbol.Item2;
            var direction = (int)attributeData.ConstructorArguments[0].Value;

            string propertyName = ChooseName(fieldName);

            // Get the Handle attribute from the field, if any.
            var handleAttributeData = fieldSymbol.Item1.GetAttributes().FirstOrDefault(x => x.AttributeClass.Equals(handleAttributeSymbol, SymbolEqualityComparer.Default));
            var handlers = string.Empty;
            var needsOldValue = true;
            if (handleAttributeData != null)
            {
                var changeTypes = handleAttributeData.ConstructorArguments[0].Values;
                foreach (var changeType in changeTypes)
                {
                    switch ((int)changeType.Value)
                    {
                        case 0: // Always
                            handlers += $"On{propertyName}Set();\n        ";
                            needsOldValue = false;
                            break;

                        case 1: // Changed
                            handlers += $"if (oldValue != value) {{ On{propertyName}Changed(); }}\n        ";
                            break;

                        case 2: // TransitionedLoToHi
                            handlers += $"if (!oldValue && value) {{ On{propertyName}TransitionedLoToHi(); }}\n        ";
                            break;

                        case 3: // TransitionedHiToLo
                            handlers += $"if (oldValue && !value) {{ On{propertyName}TransitionedHiToLo(); }}\n        ";
                            break;
                    }
                }
            }

            var oldValueFragment = needsOldValue
                ? $"var oldValue = {fieldName};"
                : string.Empty;

            var hasGetter = direction == 1 || direction == 2;
            var hasSetter = direction == 0 || direction == 2;

            var getter = hasGetter
                ? $"get => {fieldName};"
                : string.Empty;

            var setter = hasSetter
                ? @$"set {{
        {oldValueFragment}
        {fieldName} = value;
        {handlers}
    }}"
                : string.Empty;

            source.Append($@"
public {fieldType} {propertyName} 
{{
    {getter}
    {setter}
}}
");

            static string ChooseName(string fieldName)
            {
                fieldName = fieldName.TrimStart('_');

                if (fieldName.Length < 3)
                {
                    return fieldName.ToUpper();
                }

                return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
            }
        }

        private sealed class SyntaxReceiver : ISyntaxReceiver
        {
            public List<FieldDeclarationSyntax> CandidateFields { get; } = new List<FieldDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.AttributeLists.Count > 0)
                {
                    CandidateFields.Add(fieldDeclarationSyntax);
                }
            }
        }
    }
}
