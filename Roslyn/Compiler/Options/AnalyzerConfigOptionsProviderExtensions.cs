// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.


using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer.Utilities
{
    public static class AnalyzerConfigOptionsProviderExtensions
    {
        public static bool IsEmpty(this AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
        {
            // Reflection based optimization for empty analyzer config options.
            // Ideally 'AnalyzerConfigOptionsProvider.IsEmpty' would be exposed in the API.
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            return analyzerConfigOptionsProvider.GetType().GetField("_treeDict", flags)?.GetValue(analyzerConfigOptionsProvider) is ImmutableDictionary<object, AnalyzerConfigOptions> perTreeOptionsMap
                && perTreeOptionsMap.IsEmpty;
        }
    }
}

