using Xunit.Runners.Maui.VisualRunner;

namespace Xunit.Runners
{
    public interface IResultChannel : ITestListener
    {
        Task CloseChannel();
        
        Task<bool> OpenChannel(string message = null);
    }
}