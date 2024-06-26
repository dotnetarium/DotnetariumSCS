﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;

namespace Analyzer.Utilities.FlowAnalysis.Analysis.TaintedDataAnalysis
{
    /// <summary>
    /// Info for a tainted data sanitizer type, which makes tainted data untainted.
    /// </summary>
    internal sealed class SanitizerInfo : ITaintedDataInfo, IEquatable<SanitizerInfo>
    {
        public SanitizerInfo(
            string fullTypeName,
            bool isInterface,
            bool isConstructorSanitizing,
            ImmutableHashSet<(MethodMatcher methodMatcher, ImmutableHashSet<(string IfTaintedParameter, string ThenUnTaintedTarget)>)> sanitizingMethods,
            ImmutableHashSet<(MethodMatcher methodMatcher, ValueContentCheck valueContentCheck, ImmutableHashSet<(string IfTaintedParameter, string ThenUnTaintedTarget)>)> sanitizingMethodsNeedsValueContentAnalysis,
            ImmutableHashSet<string> sanitizingInstanceMethods)
        {
            FullTypeName = fullTypeName ?? throw new ArgumentNullException(nameof(fullTypeName));
            IsInterface = isInterface;
            IsConstructorSanitizing = isConstructorSanitizing;
            SanitizingMethods = sanitizingMethods ?? throw new ArgumentNullException(nameof(sanitizingMethods));
            SanitizingMethodsNeedsValueContentAnalysis = sanitizingMethodsNeedsValueContentAnalysis ?? throw new ArgumentNullException(nameof(sanitizingMethodsNeedsValueContentAnalysis));
            SanitizingInstanceMethods = sanitizingInstanceMethods ?? throw new ArgumentNullException(nameof(sanitizingInstanceMethods));
        }

        /// <summary>
        /// Full type name of the...type (namespace + type).
        /// </summary>
        public string FullTypeName { get; }

        /// <summary>
        /// Indicates that this sanitizer type is an interface.
        /// </summary>
        public bool IsInterface { get; }

        /// <summary>
        /// Indicates that any tainted data entering a constructor becomes untainted.
        /// </summary>
        public bool IsConstructorSanitizing { get; }

        /// <summary>
        /// Methods that untaint tainted data.
        /// </summary>
        /// <remarks>
        /// MethodMatcher determines if the outermost tuple applies, based on the method names and arguments.
        /// (IfTaintedParameter, ThenUnTaintedTarget) determines if the ThenUnTaintedTarget is untainted, based on if the IfTaintedParameter is tainted.
        ///
        /// Example:
        /// (
        ///   (methodName, argumentOperations) => methodName == "Bar",  // MethodMatcher
        ///   {
        ///      ("a", "b")
        ///   }
        /// )
        ///
        /// will treat the parameter "b" as untainted when parameter "a" is tainted of the "Bar" method.
        /// </remarks>
        public ImmutableHashSet<(MethodMatcher MethodMatcher, ImmutableHashSet<(string IfTaintedParameter, string ThenUnTaintedTarget)>)> SanitizingMethods { get; }

        public ImmutableHashSet<(MethodMatcher MethodMatcher, ValueContentCheck valueContentCheck, ImmutableHashSet<(string IfTaintedParameter, string ThenUnTaintedTarget)>)> SanitizingMethodsNeedsValueContentAnalysis { get; }

        /// <summary>
        /// Methods that untaint tainted instance.
        /// </summary>
        public ImmutableHashSet<string> SanitizingInstanceMethods { get; }

        /// <summary>
        /// Indicates that this <see cref="SanitizerInfo"/> uses <see cref="ValueContentAbstractValue"/>s.
        /// </summary>
        public bool RequiresValueContentAnalysis => !this.SanitizingMethodsNeedsValueContentAnalysis.IsEmpty;

        /// <summary>
        /// Indicates that <see cref="OperationKind.ParameterReference"/> is required.
        /// </summary>
        public bool RequiresParameterReferenceAnalysis => false;

        public bool RequiresFieldReferenceAnalysis => false;

        /// <summary>
        /// Qualified names of the optional dependency types.
        /// </summary>
        public ImmutableArray<string> DependencyFullTypeNames => ImmutableArray<string>.Empty;

        public override int GetHashCode()
        {
            var hashCode = new RoslynHashCode();
            hashCode.Add(StringComparer.Ordinal.GetHashCode(this.FullTypeName));
            hashCode.Add(this.IsInterface.GetHashCode());
            hashCode.Add(this.IsConstructorSanitizing.GetHashCode());
            HashUtilities.Combine(this.SanitizingMethods, ref hashCode);
            HashUtilities.Combine(this.SanitizingMethodsNeedsValueContentAnalysis, ref hashCode);
            HashUtilities.Combine(this.SanitizingInstanceMethods, ref hashCode);
            return hashCode.ToHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is SanitizerInfo other && this.Equals(other);
        }

        public bool Equals(SanitizerInfo other)
        {
            return other != null
                && this.FullTypeName == other.FullTypeName
                && this.IsInterface == other.IsInterface
                && this.IsConstructorSanitizing == other.IsConstructorSanitizing
                && this.SanitizingMethods == other.SanitizingMethods
                && this.SanitizingMethodsNeedsValueContentAnalysis == other.SanitizingMethodsNeedsValueContentAnalysis
                && this.SanitizingInstanceMethods == other.SanitizingInstanceMethods;
        }
    }
}
