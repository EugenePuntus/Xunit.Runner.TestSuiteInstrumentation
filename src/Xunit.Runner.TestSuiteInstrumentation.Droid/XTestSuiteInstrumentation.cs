using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit.Runners.ResultChannels;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public abstract class XunitTestSuiteInstrumentation : Instrumentation
    {
        private InstrumentationDeviceRunner _instrumentDeviceRunner;
        private IResultPath _resultsPath;

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer)
            : this(handle, transfer, new TrxResultPath())
        {
        }

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer, IResultPath resultPath)
            : this(handle, transfer, resultPath, new TrxResultChannel(resultPath.Path()))
        {
        }

        protected XunitTestSuiteInstrumentation(IntPtr handle, JniHandleOwnership transfer, IResultPath resultPath, IResultChannel resultChannel)
            : base(handle, transfer)
        {
            _resultsPath = resultPath;
            _instrumentDeviceRunner = new InstrumentationDeviceRunner(new List<Assembly>(), null, resultChannel);
        }

        public override void OnCreate(Bundle arguments)
        {
            base.OnCreate(arguments);
            Setup();
            Start();
        }

        public virtual void Setup()
        {
            var assembly = Assembly.GetAssembly(typeof(DeviceRunner));

            var platformHelpersType = assembly.GetType("Xunit.Runners.PlatformHelpers");
            var assetsProperty = platformHelpersType.GetProperty("Assets");

            assetsProperty.SetValue(platformHelpersType, Application.Context.Assets);
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
                Log.Info("xUnit", "xUnit automated tests started");
                var testAssemblyViewModels = _instrumentDeviceRunner.Discover().GetAwaiter().GetResult();

                var testCases = testAssemblyViewModels.SelectMany(x => x.TestCases).ToList();

                Log.Info("xUnit", $"xUnit found testCases {testCases.Count()}");

                _instrumentDeviceRunner.Run(testCases, "Run Everything").GetAwaiter().GetResult();

                if (_resultsPath != null)
                {
                    resultsBundle.PutString("xunit-results-path", new AdbResultPath(_resultsPath).Path());
                }
                Log.Info("xUnit", "xUnit automated tests completed");

                var totalCount = testCases.Count();
                var passedCount = testCases.Count(x => x.Result == TestState.Passed);
                failedCount = testCases.Count(x => x.Result == TestState.Failed);
                var skippedCount = testCases.Count(x => x.Result == TestState.Skipped);
                var notRunningCount = testCases.Count(x => x.Result == TestState.NotRun);

                resultsBundle.PutInt("total", totalCount);
                resultsBundle.PutInt("passed", passedCount);
                resultsBundle.PutInt("failed", failedCount);
                resultsBundle.PutInt("skipped", skippedCount);
                resultsBundle.PutInt("notRun", notRunningCount);
                string finalResults = $"Total: {totalCount}, Passed: {passedCount}, Failed: {failedCount}, Skipped: {skippedCount}, NotRun: {notRunningCount}";
                Log.Info("xUnit", finalResults);
            }
            catch (Exception ex)
            {
                Log.Error("xUnit", "Error: {0}", ex);
                resultsBundle.PutString("error", ex.ToString());
            }
            Finish((failedCount == 0) ? Result.Ok : Result.Canceled, resultsBundle);
        }

        protected abstract void AddTests();

        protected void AddTestAssembly(Assembly assembly)
        {
            _instrumentDeviceRunner = _instrumentDeviceRunner.AddTestAssembly(assembly);
        }
    }
}
