﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;

namespace Analyzer.Utilities.FlowAnalysis.Analysis.TaintedDataAnalysis
{
    /// <summary>
    /// Info for a tainted data sink type.
    /// </summary>
    /// <remarks>It's bad if tainted data reaches a sink.</remarks>
    internal sealed class SinkInfo : ITaintedDataInfo, IEquatable<SinkInfo>
    {
        public SinkInfo(
            string fullTypeName,
            ImmutableHashSet<SinkKind> sinkKinds,
            bool isInterface,
            bool isAnyStringParameterInConstructorASink,
            ImmutableHashSet<string> sinkProperties,
            ImmutableHashSet<(MethodMatcher, ImmutableHashSet<string>)> sinkMethodMatchingParameters,
            ImmutableDictionary<string, ImmutableHashSet<string>> sinkMethodParameters)
        {
            FullTypeName = fullTypeName ?? throw new ArgumentNullException(nameof(fullTypeName));
            SinkKinds = sinkKinds ?? throw new ArgumentNullException(nameof(sinkKinds));
            IsInterface = isInterface;
            IsAnyStringParameterInConstructorASink = isAnyStringParameterInConstructorASink;
            SinkProperties = sinkProperties ?? throw new ArgumentNullException(nameof(sinkProperties));
            SinkMethodMatchingParameters = sinkMethodMatchingParameters ?? throw new ArgumentNullException(nameof(sinkMethodMatchingParameters));
            SinkMethodParameters = sinkMethodParameters ?? throw new ArgumentNullException(nameof(sinkMethodParameters));
        }

        /// <summary>
        /// Full name of the type that can lead to sinks.
        /// </summary>
        public string FullTypeName { get; }

        /// <summary>
        /// Type of sink.
        /// </summary>
        public ImmutableHashSet<SinkKind> SinkKinds { get; }

        /// <summary>
        /// Indicates this sink type is an interface.
        /// </summary>
        public bool IsInterface { get; }

        /// <summary>
        /// Indicates that any string parameter in the constructor is a sink.
        /// </summary>
        public bool IsAnyStringParameterInConstructorASink { get; }

        /// <summary>
        /// Set of properties on the type that are sinks.
        /// </summary>
        public ImmutableHashSet<string> SinkProperties { get; }

        /// <summary>
        /// Mapping of method name to parameter names that are sinks.
        /// </summary>
        public ImmutableDictionary<string, ImmutableHashSet<string>> SinkMethodParameters { get; }

        public ImmutableHashSet<(MethodMatcher methodMatcher, ImmutableHashSet<string>)> SinkMethodMatchingParameters { get; }

        /// <summary>
        /// Indicates that this <see cref="SinkInfo"/> uses <see cref="ValueContentAbstractValue"/>s.
        /// </summary>
        public bool RequiresValueContentAnalysis => false;

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
            HashUtilities.Combine(this.SinkKinds, ref hashCode);
            hashCode.Add(this.IsInterface.GetHashCode());
            hashCode.Add(this.IsAnyStringParameterInConstructorASink.GetHashCode());
            HashUtilities.Combine(this.SinkProperties, ref hashCode);
            HashUtilities.Combine(this.SinkMethodParameters, ref hashCode);
            HashUtilities.Combine(this.SinkMethodMatchingParameters, ref hashCode);
            return hashCode.ToHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is SinkInfo other && this.Equals(other);
        }

        public bool Equals(SinkInfo other)
        {
            return other != null
                && this.FullTypeName == other.FullTypeName
                && this.SinkKinds == other.SinkKinds
                && this.IsInterface == other.IsInterface
                && this.IsAnyStringParameterInConstructorASink == other.IsAnyStringParameterInConstructorASink
                && this.SinkProperties == other.SinkProperties
                && this.SinkMethodParameters == other.SinkMethodParameters
                && this.SinkMethodMatchingParameters == other.SinkMethodMatchingParameters;
        }
    }
}
