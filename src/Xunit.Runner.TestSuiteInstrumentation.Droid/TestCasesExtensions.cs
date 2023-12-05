using System.Text;
using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public static class TestCasesExtensions
    {
        public static string ResultsToString(this List<TestCaseViewModel> testCases, IResultPath? resultsPath)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Empty);
            stringBuilder
                .AppendLine($"total={testCases.Count}")
                .AppendLine($"passed={testCases.Count(x => x.Result == TestState.Passed)}")
                .AppendLine($"failed={testCases.Count(x => x.Result == TestState.Failed)}")
                .AppendLine($"skipped={testCases.Count(x => x.Result == TestState.Skipped)}")
                .AppendLine($"notRun={testCases.Count(x => x.Result == TestState.NotRun)}");

            if (resultsPath != null)
            {
                stringBuilder.AppendLine($"xunit-results-path={new AdbResultPath(resultsPath).Path()}");
            }

            return stringBuilder.ToString();
        }
        
        public static string AsString(this List<TestCaseViewModel> testCases)
        {
            var count = testCases.Count;
            var stringBuilder = new StringBuilder(count);
            stringBuilder.AppendLine(string.Empty);

            for (var i = 1; i <= count; i++)
            {
                var testCase = testCases[i - 1];
                stringBuilder.AppendLine($"{i}/{count} {testCase.TestCase.DisplayName}");
            }

            return stringBuilder.ToString();
        }
    }
}

