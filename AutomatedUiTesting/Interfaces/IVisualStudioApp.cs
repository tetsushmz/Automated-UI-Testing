using System;

namespace Interfaces
{
    public interface IVisualStudioApp : IDisposable
    {
        void RemoveFolder(string folderName, string folderPath);

        void CreateNewWpfApp(ProjectInfo projectInfo);

        IMainWindow GetMainWindow();

        IWpfAppWindow RetryWaitForWpfAppWindow(string title);

        void RetryWaitForWindowClosed(string title);
    }
}
