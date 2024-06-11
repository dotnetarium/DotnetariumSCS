using System;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dotnetarium.Analyzers.Locale;

namespace DotnetariumSCS.Test.Locale
{
    [TestClass]
    public class LocaleTest
    {
        [TestMethod]
        public void LoadDiagnosticLocale()
        {
            DiagnosticDescriptor desc = LocaleUtil.GetDescriptor("SCS0001");
            Console.WriteLine("Description: " + desc.Description);
        }
    }
}
