namespace Xunit.Runners.TestSuiteInstrumentation
{
    public static class TestRunnerExtensions
    {
        public static async Task SaveTo(this InstrumentationDeviceRunner testRunner, IResultChannel resultChannel, string message)
        {
            if (await resultChannel.OpenChannel(message))
            {
                try
                {
                    foreach (var testResult in testRunner.Results)
                    {
                        resultChannel.RecordResult(testResult);
                    }
                }
                finally
                {
                    await resultChannel.CloseChannel();
                }
            }
        }
    }
}
