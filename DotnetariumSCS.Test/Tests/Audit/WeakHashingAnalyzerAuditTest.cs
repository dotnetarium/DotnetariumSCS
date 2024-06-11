using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dotnetarium.Analyzers;
using DotnetariumSCS.Test.Config;
using DotnetariumSCS.Test.Helpers;

namespace DotnetariumSCS.Test.Audit
{
    [TestClass]
    public class WeakHashingAnalyzerAuditTest : DiagnosticVerifier
    {
        protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers(string language)
        {
            if (language == LanguageNames.CSharp)
                return new DiagnosticAnalyzer[] { new WeakHashingAnalyzerCSharp() };
            else
                return new DiagnosticAnalyzer[] { new WeakHashingAnalyzerVisualBasic() };
        }

        [DataRow("CryptoConfig.CreateFromName(name)", true)]
        [DataRow("CryptoConfig.CreateFromName(name)", false)]
        [DataTestMethod]
        public async Task HashCreateAuditMode(string create, bool auditMode)
        {
            var cSharpTest = $@"
using System.Security.Cryptography;

public class WeakHashing
{{
    static void Foo(string name)
    {{
        var sha = {create};
    }}
}}
";

            var visualBasicTest = $@"
Imports System.Security.Cryptography

Public Class WeakHashing
    Private Shared Sub Foo(name As System.String)
        Dim sha As HashAlgorithm = {create}
    End Sub
End Class
";

            var expected = new DiagnosticResult
            {
                Id       = "SCS0006",
                Severity = DiagnosticSeverity.Warning,
                Message  = "Possibly weak hashing function."
            };

            var testConfig = $@"
AuditMode: {auditMode}
";

            var optionsWithProjectConfig = ConfigurationTest.CreateAnalyzersOptionsWithConfig(testConfig);

            await VerifyCSharpDiagnostic(cSharpTest,
                                         auditMode ? new [] {expected} : null,
                                         auditMode ? optionsWithProjectConfig : null).ConfigureAwait(false);
            await VerifyVisualBasicDiagnostic(visualBasicTest,
                                              auditMode ? new[] { expected } : null,
                                              auditMode ? optionsWithProjectConfig : null).ConfigureAwait(false);
        }
    }
}
