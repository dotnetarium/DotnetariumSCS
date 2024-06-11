using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Dotnetarium.Analyzers.Locale;
using Dotnetarium.Config;

namespace Dotnetarium.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class CompilationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SCS0000";

        public static readonly DiagnosticDescriptor Rule = LocaleUtil.GetDescriptor(DiagnosticId);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (!Debugger.IsAttached) // prefer single thread for debugging in development
                context.EnableConcurrentExecution();

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(ctx =>
            {
                var configuration = Configuration.GetOrCreate(ctx);
                if (configuration.ReportAnalysisCompletion)
                {
                    ctx.RegisterCompilationEndAction(ctx =>
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(Rule, Location.None, ctx.Compilation.AssemblyName));
                    });
                }
            });
        }
    }
}
