﻿namespace Microsoft.VisualStudio.Threading.Analyzers.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using Xunit.Abstractions;

    public class AvoidImpliedTaskSchedulerCurrentAnalyzerTests : CodeFixVerifier
    {
        private DiagnosticResult expect = new DiagnosticResult
        {
            Id = "VSSDK011",
            SkipVerifyMessage = true,
            Severity = DiagnosticSeverity.Warning,
        };

        public AvoidImpliedTaskSchedulerCurrentAnalyzerTests(ITestOutputHelper logger)
            : base(logger)
        {
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new AvoidImpliedTaskSchedulerCurrentAnalyzer();

        [Fact]
        public void ContinueWith_NoTaskScheduler_GeneratesWarning()
        {
            var test = @"
using System.Threading.Tasks;

class Test {
    void F() {
        Task t = null;
        t.ContinueWith(_ => { });
    }
}
";

            this.expect.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 11, 7, 23) };
            this.VerifyCSharpDiagnostic(test, this.expect);
        }

        [Fact]
        public void StartNew_NoTaskScheduler_GeneratesWarning()
        {
            var test = @"
using System.Threading.Tasks;

class Test {
    void F() {
        Task.Factory.StartNew(() => { });
    }
}
";

            this.expect.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 22, 6, 30) };
            this.VerifyCSharpDiagnostic(test, this.expect);
        }

        [Fact]
        public void ContinueWith_WithTaskScheduler_GeneratesNoWarning()
        {
            var test = @"
using System.Threading.Tasks;

class Test {
    void F() {
        Task t = null;
        t.ContinueWith(_ => { }, TaskScheduler.Default);
    }
}
";

            this.VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void StartNew_WithTaskScheduler_GeneratesNoWarning()
        {
            var test = @"
using System.Threading;
using System.Threading.Tasks;

class Test {
    void F() {
        Task.Factory.StartNew(() => { }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
    }
}
";

            this.VerifyCSharpDiagnostic(test);
        }
    }
}
