using System.Threading.Tasks;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public static class ICachedResultChannelExtensions
    {
        public static async Task SaveTo(this ICachedResultChannel cachedResultChannel, IResultChannel resultChannel, string message)
        {
            if (await resultChannel.OpenChannel(message))
            {
                try
                {
                    foreach (var testResult in cachedResultChannel.TestResults)
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
