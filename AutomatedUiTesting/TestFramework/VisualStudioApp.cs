using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA2;
using Interfaces;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework
{
    public class VisualStudioApp : IVisualStudioApp
    {
        private const string MicrosoftVisualStudio = "Microsoft Visual Studio";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(VisualStudioApp));

        private string expectedMainWindowTitle;

        public VisualStudioApp()
        {
            Logger.Info($"Entered {nameof(VisualStudioApp)} constructor.");
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var fullPath = $"{path}\\Microsoft\\Windows\\Start Menu\\Programs\\Visual Studio 2019";
            var process = Process.Start(fullPath);
            Logger.Info($"Started Visual Studio 2019.");

            this.Automation = new UIA2Automation();
            this.Application = new Application(process);
            Logger.Info($"Created {nameof(Application)} object.");

            Retry.Timeout = Constants.DefaultTimeout;
        }

        public AutomationBase Automation { get; }

        public Application Application { get; }

        public void CreateNewWpfApp(ProjectInfo projectInfo)
        {
            var window = this.RetryWaitForWindowByTitle(MicrosoftVisualStudio);
            Assert.IsNotNull(window, "Opening window not found.");
            var openingWizardPage1 = new OpeningWizardPage1(window);
            openingWizardPage1.CreateNewWpfApp(projectInfo);
            this.expectedMainWindowTitle = $"{projectInfo.ProjectName} - {MicrosoftVisualStudio}";
        }

        public IMainWindow GetMainWindow()
        {
            var window = this.RetryWaitForWindowByTitle(this.expectedMainWindowTitle);
            Assert.IsNotNull(
                window,
                $"Visual Studio main window '{this.expectedMainWindowTitle}' not found.");
            var result = new MainWindow(window);
            return result;
        }

        public IWpfAppWindow GetWpfAppWindow(string title)
        {
            var retryResult = Retry.WhileNull(
                () => this.FindNewWindow(title),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(0.1));
            var result = retryResult.Result;
            Assert.IsNotNull(result, $"A window with '{title}' as title not found.");
            return new WpfAppWindow(result);
        }

        public void WaitUntilWindowClosed(string title)
        {
            var retryResult = Retry.WhileFalse(
                () => this.IsWindowClosed(title),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(0.1));
            Assert.IsTrue(
                retryResult.Success,
                $"A window with '{title}' as title did not close.");
        }

        public Window RetryWaitForWindowByTitle(string partialWindowTitle, int timeoutSeconds = 20)
        {
            var retryResult = Retry.WhileNull(
                () => this.FindWindowOnDesktopByTitle(partialWindowTitle),
                TimeSpan.FromSeconds(timeoutSeconds),
                TimeSpan.FromSeconds(0.1));

            var result = retryResult.Result;
            Logger.Info($"Found window '{result.Title}'.");
            return result;
        }

        public void Dispose()
        {
            this.Automation.Dispose();
            this.Application.Dispose();
        }

        private Window FindWindowOnDesktopByTitle(string partialWindowTitle)
        {
            var windows = this.Application.GetAllTopLevelWindows(this.Automation);
            foreach (var window in windows)
            {
                if (this.ContainsPartialTitle(window, partialWindowTitle))
                {
                    return window;
                }
            }

            Logger.Info($"Retrying {nameof(this.FindWindowOnDesktopByTitle)}('{partialWindowTitle}').");
            return null;
        }

        private bool ContainsPartialTitle(Window window, string partialWindowTitle)
        {
            var title = this.GetWindowTitle(window);
            var result = title != null && title.Contains(partialWindowTitle);
            return result;
        }

        private Window FindNewWindow(string title)
        {
            var windows = this.FindWindows(title);
            if (windows.Count == 0)
            {
                Logger.Info($"Retrying {nameof(this.FindNewWindow)}('{title}')");
                return null;
            }

            return windows[0];
        }

        private bool IsWindowClosed(string title)
        {
            var windows = this.FindWindows(title);
            if (windows.Count > 0)
            {
                Logger.Info($"Retrying {nameof(this.IsWindowClosed)}('{title}')");
                return false;
            }

            return true;
        }

        private IList<Window> FindWindows(string title)
        {
            var desktop = this.Automation.GetDesktop();
            var elements = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

            var result = (from element in elements
                let window = element.AsWindow()
                where this.GetWindowTitle(window) == title
                select window).ToList();

            return result;
        }

        private string GetWindowTitle(Window window)
        {
            try
            {
                return window.Title;
            }
            catch (Exception)
            {
                // Either the window is null or does not have the Title property.
                return null;
            }
        }
    }
}
