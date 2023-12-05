using System.Reflection;
using Android.Util;
using Microsoft.Extensions.Logging;
using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public class InstrumentationDeviceRunner : DeviceRunner, ITestListener
    {
        private readonly ITestNavigation _navigation;
        private readonly ILogger _logger;

        public InstrumentationDeviceRunner(
            IReadOnlyCollection<Assembly> testAssemblies,
            ITestNavigation navigation,
            ILogger logger)
            : base(testAssemblies, navigation, logger)
        {
            _navigation = navigation;
            _logger = logger;
        }

        public IList<TestResultViewModel> Results { get; } = new List<TestResultViewModel>();

        void ITestListener.RecordResult(TestResultViewModel result)
        {
            Results.Add(result);
            this.RecordResult(result);
        }

        public void Clear()
        {
            Results.Clear();
        }
        
        public InstrumentationDeviceRunner AddTestAssembly(Assembly assembly)
        {
            var testAssemblies = TestAssemblies.ToList();
            testAssemblies.Add(assembly);

            Log.Info("xUnit", $"xUnit add tests from {assembly.FullName}");

            return new InstrumentationDeviceRunner(testAssemblies, _navigation, _logger);
        }
    }
}
