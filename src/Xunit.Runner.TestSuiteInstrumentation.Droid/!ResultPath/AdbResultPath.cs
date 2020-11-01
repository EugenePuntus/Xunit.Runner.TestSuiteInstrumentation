using System;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    internal class AdbResultPath : IResultPath
    {
        private readonly IResultPath _resultPath;

        public AdbResultPath(IResultPath resultPath)
        {
            _resultPath = resultPath;
        }

        public string Path()
        {
            var path = _resultPath.Path();
            string environmentVariable = System.Environment.GetEnvironmentVariable("EMULATED_STORAGE_SOURCE");
            string environmentVariable2 = System.Environment.GetEnvironmentVariable("EMULATED_STORAGE_TARGET");
            if (!string.IsNullOrEmpty(environmentVariable) && !string.IsNullOrEmpty(environmentVariable2) && path.StartsWith(environmentVariable2, StringComparison.Ordinal))
            {
                return path.Replace(environmentVariable2, environmentVariable);
            }
            return path;
        }
    }
}
