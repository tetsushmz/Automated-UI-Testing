using System;

namespace Interfaces
{
    public interface IVisualStudioApp : IDisposable
    {
        void CreateNewWpfApp(ProjectInfo projectInfo);

        IMainWindow GetMainWindow();

        IWpfAppWindow RetryWaitForWpfAppWindow(string title);

        void RetryWaitForWindowClosed(string title);
    }
}
