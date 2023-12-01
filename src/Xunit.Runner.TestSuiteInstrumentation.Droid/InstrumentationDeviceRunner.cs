using System.Reflection;
using Android.Util;
using Microsoft.Extensions.Logging;
using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public class InstrumentationDeviceRunner : DeviceRunner
    {
        private readonly IReadOnlyCollection<Assembly> _testAssemblies;
        private readonly ITestNavigation _navigation;
        private readonly ILogger _logger;

        public InstrumentationDeviceRunner(
            IReadOnlyCollection<Assembly> testAssemblies,
            ITestNavigation navigation,
            ILogger logger)
            : base(testAssemblies, navigation, logger)
        {
            _testAssemblies = testAssemblies;
            _navigation = navigation;
            _logger = logger;
        }

        public InstrumentationDeviceRunner AddTestAssembly(Assembly assembly)
        {
            var testAsseblies = _testAssemblies.ToList();
            testAsseblies.Add(assembly);

            Log.Info("xUnit", $"xUnit add tests from {assembly.FullName}");

            return new InstrumentationDeviceRunner(testAsseblies, _navigation, _logger);
        }
    }
}
