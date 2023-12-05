using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public class CachedResultChannel : ICachedResultChannel
    {
        private readonly List<TestResultViewModel> _testResultViewModels = new();

        public IReadOnlyCollection<TestResultViewModel> TestResults => _testResultViewModels;

        public Task CloseChannel()
        {
            return Task.CompletedTask;
        }

        public Task<bool> OpenChannel(string? message = null)
        {
            return Task.FromResult(true);
        }

        public void RecordResult(TestResultViewModel result)
        {
            _testResultViewModels.Add(result);
        }

        public void Clear()
        {
            _testResultViewModels.Clear();
        }
    }
}