using System;
using System.Reflection;
using Android.App;
using Android.Runtime;
using Xamarin.Forms;
using Xunit.Runners.TestSuiteInstrumentation;

namespace Droid.IntegrationTests
{
    [Instrumentation(Name = "sample.testSuiteInstrumentation")]
    public class TestInstrumentation : XunitTestSuiteInstrumentation
    {
        public TestInstrumentation(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        protected override void AddTests()
        {
            Xamarin.Forms.Mocks.MockForms.Init(Device.Android);

            AddTestAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
