namespace Interfaces
{
    public interface IMainWindow
    {
        string Title { get; }

        void LoadCode(string filename, string code);

        void StartDebugging();

        void WaitUntilFinishesDebugging();

        void Close();
    }
}
