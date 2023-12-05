using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public interface ICachedResultChannel : IResultChannel
    {
        IReadOnlyCollection<TestResultViewModel> TestResults { get; }

        void Clear();
    }
}