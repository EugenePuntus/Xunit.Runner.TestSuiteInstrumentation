using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public class CachedResultChannel : ICachedResultChannel
    {
        private readonly List<TestResultViewModel> _testResultViewModels;

        public CachedResultChannel()
        {
            _testResultViewModels = new List<TestResultViewModel>();
        }

        public IReadOnlyCollection<TestResultViewModel> TestResults => _testResultViewModels;

        public Task CloseChannel()
        {
            return Task.CompletedTask;
        }

        public Task<bool> OpenChannel(string message = null)
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
