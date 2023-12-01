using System.Reflection;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using Microsoft.Extensions.Logging;
using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public abstract class XunitTestSuiteInstrumentation : Instrumentation
    {
        private InstrumentationDeviceRunner _instrumentDeviceRunner;
        private readonly IInstrumentationProgress _progress;

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer)
            : this(handle, transfer, new InstrumentationProgress())
        {
        }

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer, IInstrumentationProgress progress)
            : base(handle, transfer)
        {
            _progress = progress;
            _instrumentDeviceRunner = new InstrumentationDeviceRunner(new List<Assembly>(), null, new Logger<string>(new LoggerFactory()));
        }

        public override void OnCreate(Bundle arguments)
        {
            base.OnCreate(arguments);
            Setup();
            Start();
        }

        public virtual void Setup()
        {
            _progress.InitializeFor(this);
        }

        public override void OnStart()
        {
            base.OnStart();
            // is needed for synchronized running tests.
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            AddTests();

            var resultsBundle = new Bundle();
            int failedCount = -1;

            try
            {
                _progress.Send("Getting a list of tests...");
                var testAssemblyViewModels = _instrumentDeviceRunner.DiscoverAsync().GetAwaiter().GetResult();
                var testCases = testAssemblyViewModels.SelectMany(x => x.TestCases).ToList();

                _progress.Send(TestCasesToString(testCases), $"{testCases.Count()} test cases were found.");

                for (var i = 1; i <= testCases.Count; i++)
                {
                    var testCase = testCases[i - 1];
                    _instrumentDeviceRunner.RunAsync(testCase).GetAwaiter().GetResult();
                    _progress.Send($"{i}/{testCases.Count} [{testCase.Result.ToString().ToUpper()}] {testCase.TestCase.DisplayName}");
                }

                _progress.Send("Saving test results...");

                failedCount = testCases.Count(x => x.Result == TestState.Failed);
            }
            catch (Exception ex)
            {
                resultsBundle.WithValue("error", ex.ToString());
            }
            Finish((failedCount == 0) ? Result.Ok : Result.Canceled, resultsBundle);
        }

        private string TestCasesToString(List<TestCaseViewModel> testCases)
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

        protected abstract void AddTests();

        protected void AddTestAssembly(Assembly assembly)
        {
            _instrumentDeviceRunner = _instrumentDeviceRunner.AddTestAssembly(assembly);
        }
    }
}
