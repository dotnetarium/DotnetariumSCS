using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dotnetarium.Analyzers
{
    internal interface IExternalFileAnalyzer
    {
        void AnalyzeFile(AdditionalText file, CompilationAnalysisContext context);
    }
}
