namespace Interfaces
{
    public interface IWpfAppWindow
    {
        string Title { get; }

        void WaitUntilTextIsDisplayedBeforeTimeout(string text, int timeoutSeconds);

        void Close();
    }
}
