using System.Reflection;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public abstract class XunitTestSuiteInstrumentation : Instrumentation
    {
        private InstrumentationDeviceRunner _instrumentDeviceRunner;
        private IResultPath _resultsPath;
        //private readonly IResultChannel _originalResultChannel;

        //private readonly ICachedResultChannel _cachedResultChannel;
        private readonly IInstrumentationProgress _progress;

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer)
            : this(handle, transfer, new TrxResultPath())
        {
        }

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer, IResultPath resultPath)
            : this(handle, transfer, resultPath, new InstrumentationProgress())
        {
        }

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer, IResultPath resultPath, IInstrumentationProgress progress)
            : base(handle, transfer)
        {
            _resultsPath = resultPath;
            _progress = progress;
            //_cachedResultChannel = new CachedResultChannel();
            _instrumentDeviceRunner = new InstrumentationDeviceRunner(new List<Assembly>(), null, null);
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

            var assembly = Assembly.GetAssembly(typeof(DeviceRunner));
            var platformHelpersType = assembly.GetType("Xunit.Runners.PlatformHelpers");
            var assetsProperty = platformHelpersType.GetProperty("Assets");
            assetsProperty.SetValue(platformHelpersType, Android.App.Application.Context.Assets);
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
                //_cachedResultChannel.SaveTo(_originalResultChannel, "RunEverything").GetAwaiter().GetResult();
                //_cachedResultChannel.Clear();

                failedCount = testCases.Count(x => x.Result == TestState.Failed);

                //resultsBundle.WithValue("Results", ResultsToString(testCases));
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

        private string ResultsToString(List<TestCaseViewModel> testCases)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Empty);

            var totalCount = testCases.Count();
            var passedCount = testCases.Count(x => x.Result == TestState.Passed);
            var failedCount = testCases.Count(x => x.Result == TestState.Failed);
            var skippedCount = testCases.Count(x => x.Result == TestState.Skipped);
            var notRunningCount = testCases.Count(x => x.Result == TestState.NotRun);

            stringBuilder
                    .AppendLine($"total={totalCount}")
                    .AppendLine($"passed={passedCount}")
                    .AppendLine($"failed={failedCount}")
                    .AppendLine($"skipped={skippedCount}")
                    .AppendLine($"notRun={notRunningCount}");

            if (_resultsPath != null)
            {
                stringBuilder.AppendLine($"xunit-results-path={new AdbResultPath(_resultsPath).Path()}");
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
