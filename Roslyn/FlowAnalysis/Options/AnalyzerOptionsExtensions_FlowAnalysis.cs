﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.PointsToAnalysis;

namespace Analyzer.Utilities
{
    public static partial class AnalyzerOptionsExtensions
    {
        public static InterproceduralAnalysisKind GetInterproceduralAnalysisKindOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            ISymbol symbol,
            Compilation compilation,
            InterproceduralAnalysisKind defaultValue,
            CancellationToken cancellationToken)
        => TryGetSyntaxTreeForOption(symbol, out var tree)
            ? options.GetInterproceduralAnalysisKindOption(rule, tree, compilation, defaultValue)
            : defaultValue;

        public static InterproceduralAnalysisKind GetInterproceduralAnalysisKindOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            SyntaxTree tree,
            Compilation compilation,
            InterproceduralAnalysisKind defaultValue)
            => options.GetNonFlagsEnumOptionValue(optionName: EditorConfigOptionNames.InterproceduralAnalysisKind, rule: rule, tree: tree, compilation: compilation, defaultValue: defaultValue);

        public static DisposeAnalysisKind GetDisposeAnalysisKindOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            ISymbol symbol,
            Compilation compilation,
            DisposeAnalysisKind defaultValue)
        => TryGetSyntaxTreeForOption(symbol, out var tree)
            ? options.GetDisposeAnalysisKindOption(rule, tree, compilation, defaultValue)
            : defaultValue;

        public static DisposeAnalysisKind GetDisposeAnalysisKindOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            SyntaxTree tree,
            Compilation compilation,
            DisposeAnalysisKind defaultValue)
            => options.GetNonFlagsEnumOptionValue(EditorConfigOptionNames.DisposeAnalysisKind, rule, tree, compilation, defaultValue);

        public static bool GetDisposeOwnershipTransferAtConstructorOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            ISymbol symbol,
            Compilation compilation,
            bool defaultValue,
            CancellationToken cancellationToken)
        => TryGetSyntaxTreeForOption(symbol, out var tree)
            ? options.GetDisposeOwnershipTransferAtConstructorOption(rule, tree, compilation, defaultValue, cancellationToken)
            : defaultValue;

        public static bool GetDisposeOwnershipTransferAtConstructorOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            SyntaxTree tree,
            Compilation compilation,
            bool defaultValue,
            CancellationToken cancellationToken)
            => options.GetBoolOptionValue(EditorConfigOptionNames.DisposeOwnershipTransferAtConstructor, rule, tree, compilation, defaultValue, cancellationToken);

        public static bool GetDisposeOwnershipTransferAtMethodCall(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            ISymbol symbol,
            Compilation compilation,
            bool defaultValue,
            CancellationToken cancellationToken)
        => TryGetSyntaxTreeForOption(symbol, out var tree)
            ? options.GetDisposeOwnershipTransferAtMethodCall(rule, tree, compilation, defaultValue, cancellationToken)
            : defaultValue;

        public static bool GetDisposeOwnershipTransferAtMethodCall(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            SyntaxTree tree,
            Compilation compilation,
            bool defaultValue,
            CancellationToken cancellationToken)
            => options.GetBoolOptionValue(EditorConfigOptionNames.DisposeOwnershipTransferAtMethodCall, rule, tree, compilation, defaultValue, cancellationToken);

        public static bool GetCopyAnalysisOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            ISymbol symbol,
            Compilation compilation,
            bool defaultValue,
            CancellationToken cancellationToken)
        => TryGetSyntaxTreeForOption(symbol, out var tree)
            ? options.GetCopyAnalysisOption(rule, tree, compilation, defaultValue, cancellationToken)
            : defaultValue;

        public static bool GetCopyAnalysisOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            SyntaxTree tree,
            Compilation compilation,
            bool defaultValue,
            CancellationToken cancellationToken)
            => options.GetBoolOptionValue(EditorConfigOptionNames.CopyAnalysis, rule, tree, compilation, defaultValue, cancellationToken);

        public static PointsToAnalysisKind GetPointsToAnalysisKindOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            ISymbol symbol,
            Compilation compilation,
            PointsToAnalysisKind defaultValue,
            CancellationToken cancellationToken)
        => TryGetSyntaxTreeForOption(symbol, out var tree)
            ? options.GetPointsToAnalysisKindOption(rule, tree, compilation, defaultValue, cancellationToken)
            : defaultValue;

        public static PointsToAnalysisKind GetPointsToAnalysisKindOption(
            this AnalyzerOptions options,
            DiagnosticDescriptor rule,
            SyntaxTree tree,
            Compilation compilation,
            PointsToAnalysisKind defaultValue,
            CancellationToken cancellationToken)
            => options.GetNonFlagsEnumOptionValue(optionName: EditorConfigOptionNames.PointsToAnalysisKind, rule, tree, compilation, defaultValue);
    }
}
