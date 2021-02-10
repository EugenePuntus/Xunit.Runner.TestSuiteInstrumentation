using System.Collections.Generic;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public interface ICachedResultChannel : IResultChannel
    {
        IReadOnlyCollection<TestResultViewModel> TestResults { get; }

        void Clear();
    }

}
