﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Dotnetarium.Analyzers.Utils
{
    internal static class ITypeSymbolExtensions
    {
        public static bool IsPrimitiveType(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
            case SpecialType.System_Boolean:
            case SpecialType.System_Byte:
            case SpecialType.System_Char:
            case SpecialType.System_Double:
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_IntPtr:
            case SpecialType.System_UIntPtr:
            case SpecialType.System_SByte:
            case SpecialType.System_Single:
                return true;
            default:
                return false;
            }
        }

        public static bool Inherits(this ITypeSymbol type, ITypeSymbol possibleBase)
        {
            if (type == null || possibleBase == null)
            {
                return false;
            }

            if (type.Equals(possibleBase))
            {
                return true;
            }

            switch (possibleBase.TypeKind)
            {
            case TypeKind.Class:
                for (ITypeSymbol t = type.BaseType; t != null; t = t.BaseType)
                {
                    if (t.Equals(possibleBase))
                    {
                        return true;
                    }
                }

                return false;

            case TypeKind.Interface:
                foreach (INamedTypeSymbol i in type.AllInterfaces)
                {
                    if (i.Equals(possibleBase))
                    {
                        return true;
                    }
                }

                return false;

            default:
                return false;
            }
        }

        public static IEnumerable<INamedTypeSymbol> GetBaseTypes(this ITypeSymbol type)
        {
            INamedTypeSymbol current = type.BaseType;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
        {
            ITypeSymbol current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool DerivesFrom(this ITypeSymbol symbol, INamedTypeSymbol candidateBaseType, bool baseTypesOnly = false)
        {
            if (candidateBaseType == null || symbol == null)
            {
                return false;
            }

            if (!baseTypesOnly && symbol.AllInterfaces.Contains(candidateBaseType))
            {
                return true;
            }

            while (symbol != null)
            {
                if (symbol.Equals(candidateBaseType))
                {
                    return true;
                }

                symbol = symbol.BaseType;
            }

            return false;
        }

        public static IEnumerable<AttributeData> GetApplicableAttributes(this INamedTypeSymbol type)
        {
            var attributes = new List<AttributeData>();

            while (type != null)
            {
                attributes.AddRange(type.GetAttributes());

                type = type.BaseType;
            }

            return attributes;
        }

        public static bool IsAttribute(this ITypeSymbol symbol)
        {
            for (INamedTypeSymbol b = symbol.BaseType; b != null; b = b.BaseType)
            {
                if (b.MetadataName == "Attribute" &&
                    b.ContainingType == null &&
                    b.ContainingNamespace != null &&
                    b.ContainingNamespace.Name == "System" &&
                    b.ContainingNamespace.ContainingNamespace != null &&
                    b.ContainingNamespace.ContainingNamespace.IsGlobalNamespace)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
