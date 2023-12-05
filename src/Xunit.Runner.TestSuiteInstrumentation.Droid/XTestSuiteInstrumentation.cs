using System.Reflection;
using Android.App;
using Android.OS;
using Android.Runtime;
using Microsoft.Extensions.Logging;
using Xunit.Runners.Maui.VisualRunner;
using Xunit.Runners.ResultChannels;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public abstract class XunitTestSuiteInstrumentation : Instrumentation
    {
        private InstrumentationDeviceRunner _instrumentDeviceRunner;
        private readonly IResultPath _resultsPath;
        private readonly IResultChannel _originalResultChannel;
        private readonly IInstrumentationProgress _progress;

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer)
            : this(handle, transfer, new TrxResultPath())
        {
        }

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer, IResultPath resultPath)
            : this(handle, transfer, resultPath, new TrxResultChannel(resultPath.Path()))
        {
        }

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer, IResultPath resultPath, IResultChannel resultChannel)
            : this(handle, transfer, resultPath, resultChannel, new InstrumentationProgress())
        {
        }

        protected XunitTestSuiteInstrumentation(
            IntPtr handle, 
            JniHandleOwnership transfer,
            IResultPath resultPath,
            IResultChannel resultChannel,
            IInstrumentationProgress progress)
            : base(handle, transfer)
        {
            _resultsPath = resultPath;
            _originalResultChannel = resultChannel;
            _progress = progress;
            _instrumentDeviceRunner = new InstrumentationDeviceRunner(
                new List<Assembly>(),
                null,
                new Logger<string>(new LoggerFactory())
            );
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

        public override async void OnStart()
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
                
                var testAssemblyViewModels = await _instrumentDeviceRunner.DiscoverAsync();
                var testCases = testAssemblyViewModels.SelectMany(x => x.TestCases).ToList();

                _progress.Send(testCases.AsString(), $"{testCases.Count} test cases were found.");

                for (var i = 1; i <= testCases.Count; i++)
                {
                    var testCase = testCases[i - 1];
                    await _instrumentDeviceRunner.RunAsync(testCase);
                    _progress.Send($"{i}/{testCases.Count} [{testCase.Result.ToString().ToUpper()}] {testCase.TestCase.DisplayName}");
                }

                _progress.Send("Saving test results...");
                await _instrumentDeviceRunner.SaveTo(_originalResultChannel, "RunEverything");
                _instrumentDeviceRunner.Clear();
                
                failedCount = testCases.Count(x => x.Result == TestState.Failed);
                resultsBundle.WithValue("Results", testCases.ResultsToString(_resultsPath));
            }
            catch (Exception ex)
            {
                resultsBundle.WithValue("error", ex.ToString());
            }
            Finish(failedCount == 0 ? Result.Ok : Result.Canceled, resultsBundle);
        }

        protected abstract void AddTests();

        protected void AddTestAssembly(Assembly assembly)
        {
            _instrumentDeviceRunner = _instrumentDeviceRunner.AddTestAssembly(assembly);
        }
    }
}
