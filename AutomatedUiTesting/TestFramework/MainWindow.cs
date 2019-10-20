using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Interfaces;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework
{
    public class MainWindow : IMainWindow
    {
        private static ILog Logger => LogManager.GetLogger(typeof(MainWindow));

        private readonly Window window;

        private Button debugStartButton;

        public MainWindow(Window window)
        {
            this.window = window;
        }

        public string Title => this.window.Title;

        private Button DebugStartButton =>
            this.debugStartButton ?? (this.debugStartButton = this.RetryFindDebugStartButton());

        private Button ExitButton => this.window.RetryFindButton("/TitleBar/Button[3]");

        public void LoadCode(string filename, string code)
        {
            var textEditor = new TextEditor(this.window, filename, code);
            textEditor.LoadCode();
        }

        public void WaitUntilFinishesDebugging()
        {
            const string Running = "Running";
            var retryResult = Retry.WhileTrue(
                () => this.Title.Contains(Running),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(0.1));
            Assert.IsTrue(
                retryResult.Success,
                $"Main window title still contains '{Running}'.");
        }

        public void Close()
        {
            /* For some reason, this.window.Close() does not close Visual Studio,
             * if it is maximized.
             * Clicking the X button seems to work regardless of the window state.
             */
            this.ExitButton.Click();
            Logger.Info($"Clicked X button on Visual Studio.");
        }

        public void StartDebugging()
        {
            this.DebugStartButton.Click();
            Logger.Info("Clicked Start button.");
        }

        private Button RetryFindDebugStartButton()
        {
            var retryResult = Retry.WhileNull(
                this.FindStartButton,
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(0.1));
            Assert.IsTrue(
                retryResult.Success,
                $"{nameof(this.DebugStartButton)} cannot be found.");
            var result = retryResult.Result;
            return result;
        }

        private Button FindStartButton()
        {
            var result = this.window.FindFirstDescendant(cf => cf.ByName("Debug Target")).AsButton();
            if (result == null)
            {
                Logger.Info($"Retrying getting {nameof(this.DebugStartButton)}.");
            }

            return result;
        }
    }
}