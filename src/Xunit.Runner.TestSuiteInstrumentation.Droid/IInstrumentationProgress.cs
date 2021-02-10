using Android.App;
using Android.OS;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public interface IInstrumentationProgress
    {
        void InitializeFor(Instrumentation instrumentation);

        void Send(string message, string header = "");
    }

    public class InstrumentationProgress : IInstrumentationProgress
    {
        private Instrumentation _instrumentation;

        public void InitializeFor(Instrumentation instrumentation)
        {
            _instrumentation = instrumentation;
        }

        public void Send(string message, string header = "")
        {
            _instrumentation.SendStatus(Result.FirstUser, new Bundle().WithValue(header, message));
        }
    }
}
