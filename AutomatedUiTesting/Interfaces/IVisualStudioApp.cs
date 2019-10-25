using System;

namespace Interfaces
{
    public interface IVisualStudioApp : IDisposable
    {
        void RemoveFolder(string folderName, string folderPath);

        void CreateNewWpfApp(ProjectInfo projectInfo);

        IMainWindow GetMainWindow();

        IWpfAppWindow GetWpfAppWindow(string title);

        void WaitUntilWindowClosed(string title);
    }
}
