using Android.Util;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public class InstrumentationDeviceRunner : DeviceRunner
    {
        private readonly IReadOnlyCollection<Assembly> _testAssemblies;
        private readonly INavigation _navigation;
        private readonly IResultChannel _resultChannel;

        public InstrumentationDeviceRunner(
            IReadOnlyCollection<Assembly> testAssemblies,
            INavigation navigation,
            IResultChannel resultChannel)
            : base(testAssemblies, navigation, resultChannel)
        {
            _testAssemblies = testAssemblies;
            _navigation = navigation;
            _resultChannel = resultChannel;
        }

        public InstrumentationDeviceRunner AddTestAssembly(Assembly assembly)
        {
            var testAsseblies = _testAssemblies.ToList();
            testAsseblies.Add(assembly);

            Log.Info("xUnit", $"xUnit add tests from {assembly.FullName}");

            return new InstrumentationDeviceRunner(testAsseblies, _navigation, _resultChannel);
        }
    }
}
