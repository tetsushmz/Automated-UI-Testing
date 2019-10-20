using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Interfaces;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework
{
    public class WpfAppWindow : IWpfAppWindow
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WpfAppWindow));

        private readonly Window window;

        public WpfAppWindow(Window window)
        {
            this.window = window;
        }

        public string Title => this.window.Title;

        private Label CenterText => this.window.RetryFindLabel("/Text");

        public void WaitUntilTextIsDisplayedBeforeTimeout(string text, int timeoutSeconds)
        {
            var retryResult = Retry.WhileFalse(
                () => this.IsCenterTextEqualTo(text),
                TimeSpan.FromSeconds(timeoutSeconds),
                TimeSpan.FromSeconds(0.1));
            Assert.IsTrue(
                retryResult.Success,
                $"Window text '{text}' was not displayed within {timeoutSeconds} seconds.");
        }

        public void Close()
        {
            this.window.Close();
        }

        private bool IsCenterTextEqualTo(string text)
        {
            var currentText = this.CenterText.Text;
            var result = currentText == text;
            if (!result)
            {
                Logger.Info($"Retrying until '{text}' is displayed. Currently it is {currentText}");
            }

            return result;
        }
    }
}