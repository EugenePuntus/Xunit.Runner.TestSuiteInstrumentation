# Xunit.Runner.TestSuiteInstrumentation
Test suite instrumentation for running xUnit integration tests on emulator or device. In addition, this instrumentation allows run tests in pipelines.

```cs
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
```

For running tests, you should install application with integration tests to emulator/device.
After that you can invoke your instrumentation using the following bash script:

```bash
    # Start integration tests
    $ANDROID_PLATFORM_TOOLS/adb -s $EMULATOR_NAME shell am instrument -w $APP_PACKAGE_NAME/$TEST_INSTRUMENTATION_NAME
    
    # Copy tests result to your directory
    $ANDROID_PLATFORM_TOOLS/adb -s $EMULATOR_NAME pull /storage/emulated/0/Android/data/$APP_PACKAGE_NAME/files/Documents/TestResults.trx $TARGET_DIR
```
