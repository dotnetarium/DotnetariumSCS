﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Dotnetarium.Analyzers.Utils
{
    internal static class ISymbolExtensions
    {
        public static int? FindArgumentIndex(this IMethodSymbol method, int sourceIndex, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax arg)
        {
            if (method == null)
                return null;

            if(arg.NameColon != null)
            {
                var argName = arg.NameColon.Name.Identifier.ValueText;

                return method.FindArgumentIndexByName(argName);
            }

            return sourceIndex;
        }

        public static int? FindArgumentIndex(this IMethodSymbol method, int sourceIndex, Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax arg)
        {
            if (method == null)
                return null;

            if (arg.IsNamed)
            {
                var argName = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax)arg).NameColonEquals.Name.Identifier.ValueText;

                return method.FindArgumentIndexByName(argName);
            }

            return sourceIndex;
        }

        public static int? FindArgumentIndexByName(this IMethodSymbol method, string argName)
        {
            if (method == null)
                return null;

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                var p = method.Parameters[i];
                if (p.Name.Equals(argName))
                {
                    return i;
                }
            }

            // something is nuts, we're not going to find anything sensible
            return null;
        }

        public static bool IsType(this ISymbol symbol)
        {
            return symbol is ITypeSymbol typeSymbol && typeSymbol.IsType;
        }

        public static bool IsAccessorMethod(this ISymbol symbol)
        {
            return symbol is IMethodSymbol accessorSymbol &&
                   (accessorSymbol.MethodKind == MethodKind.PropertySet || accessorSymbol.MethodKind == MethodKind.PropertyGet ||
                    accessorSymbol.MethodKind == MethodKind.EventRemove || accessorSymbol.MethodKind == MethodKind.EventAdd);
        }

        public static bool IsPublic(this ISymbol symbol)
        {
            return symbol.DeclaredAccessibility == Accessibility.Public;
        }

        public static bool IsErrorType(this ISymbol symbol)
        {
            return symbol is ITypeSymbol typeSymbol && typeSymbol.TypeKind == TypeKind.Error;
        }

        public static bool IsDestructor(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.Destructor;
        }

        public static bool IsIndexer(this ISymbol symbol)
        {
            return (symbol as IPropertySymbol)?.IsIndexer == true;
        }

        public static bool IsUserDefinedOperator(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.UserDefinedOperator;
        }

        public static ImmutableArray<IParameterSymbol> GetParameters(this ISymbol symbol)
        {
            return symbol.TypeSwitch(
                (IMethodSymbol m) => m.Parameters,
                (IPropertySymbol p) => p.Parameters,
                _ => ImmutableArray.Create<IParameterSymbol>());
        }

        public static SymbolVisibility GetResultantVisibility(this ISymbol symbol)
        {
            // Start by assuming it's visible.
            SymbolVisibility visibility = SymbolVisibility.Public;

            switch (symbol.Kind)
            {
            case SymbolKind.Alias:
                // Aliases are uber private.  They're only visible in the same file that they
                // were declared in.
                return SymbolVisibility.Private;

            case SymbolKind.Parameter:
                // Parameters are only as visible as their containing symbol
                return GetResultantVisibility(symbol.ContainingSymbol);

            case SymbolKind.TypeParameter:
                // Type Parameters are private.
                return SymbolVisibility.Private;
            }

            while (symbol != null && symbol.Kind != SymbolKind.Namespace)
            {
                switch (symbol.DeclaredAccessibility)
                {
                // If we see anything private, then the symbol is private.
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return SymbolVisibility.Private;

                // If we see anything internal, then knock it down from public to
                // internal.
                case Accessibility.Internal:
                case Accessibility.ProtectedAndInternal:
                    visibility = SymbolVisibility.Internal;
                    break;

                // For anything else (Public, Protected, ProtectedOrInternal), the
                // symbol stays at the level we've gotten so far.
                }

                symbol = symbol.ContainingSymbol;
            }

            return visibility;
        }

        public static bool MatchMemberDerivedByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && member.ContainingType.DerivesFrom(type) && member.MetadataName == name;
        }

        public static bool MatchMethodDerivedByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && member.Kind == SymbolKind.Method && member.MatchMemberDerivedByName(type, name);
        }

        public static bool MatchMethodByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && member.Kind == SymbolKind.Method && member.MatchMemberByName(type, name);
        }

        public static bool MatchPropertyDerivedByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && member.Kind == SymbolKind.Property && member.MatchMemberDerivedByName(type, name);
        }

        public static bool MatchFieldDerivedByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && member.Kind == SymbolKind.Field && member.MatchMemberDerivedByName(type, name);
        }

        public static bool MatchMemberByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && ReferenceEquals(member.ContainingType, type) && member.MetadataName == name;
        }

        public static bool MatchPropertyByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && member.Kind == SymbolKind.Property && member.MatchMemberByName(type, name);
        }

        public static bool MatchFieldByName(this ISymbol member, INamedTypeSymbol type, string name)
        {
            return member != null && member.Kind == SymbolKind.Field && member.MatchMemberByName(type, name);
        }
    }

    internal enum SymbolVisibility
    {
        Public,
        Internal,
        Private,
    }
}
