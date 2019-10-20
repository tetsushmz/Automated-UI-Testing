using System;
using System.IO;
using System.Reflection;
using Interfaces;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace AutomatedUiTesting
{
    [Binding]
    public class BuildingWpfAppSteps
    {
        private static ILog Logger => LogManager.GetLogger(typeof(BuildingWpfAppSteps));

        private string mainWindowTitle;
        private string wpfAppWindowTitle;

        public IVisualStudioApp App { get; set; }

        public IMainWindow MainWindow { get; private set; }

        public IWpfAppWindow WpfAppWindow { get; private set; }

        [Given(@"I have removed ""(.*)"" folder, if any, in ""(.*)""")]
        public void GivenIHaveRemovedFolderIfAnyIn(string folderName, string folderPath)
        {
            this.App.RemoveFolder(folderName, folderPath);
        }

        [Given(@"I have started Visual Studio (.*)")]
        public void GivenIHaveStartedVisualStudio(int p0)
        {
            Logger.Info($"Creating VisualStudioApp object.");
            this.App = this.CreateVisualStudioApp();
            Logger.Info($"Successfully created VisualStudioApp object.");
        }

        [When(@"I create a new WPF app with the following info")]
        public void WhenICreateANewWpfAppWithTheFollowingInfo(Table table)
        {
            var projectInfo = new ProjectInfo
            {
                ProjectName = table.Rows[0]["Project name"],
                Location = table.Rows[0]["Location"],
                SolutionName = table.Rows[0]["Solution name"],
                Framework = table.Rows[0]["Framework"],
            };
            this.App.CreateNewWpfApp(projectInfo);
        }
        
        [Then(@"Visual Studio main window opens")]
        public void ThenVisualStudioMainWindowOpens()
        {
            this.MainWindow = this.App.GetMainWindow();
        }

        [When(@"I enter the following code to ""(.*)""")]
        public void WhenIEnterTheFollowingTextTo(string filename, string code)
        {
            this.MainWindow.LoadCode(filename, code);
        }

        [When(@"I start debugging the app")]
        public void WhenIStartDebuggingTheApp()
        {
            this.MainWindow.StartDebugging();
        }
        
        [Then(@"A window with ""(.*)"" as its title opens")]
        public void ThenAWindowWithAsItsTitleOpens(string title)
        {
            this.WpfAppWindow = this.App.RetryWaitForWpfAppWindow(title);
            this.wpfAppWindowTitle = this.WpfAppWindow.Title;
        }

        [Then(@"the window starts counting down and shows ""(.*)"" within (.*) seconds")]
        public void ThenTheWindowStartsCountingDownToWithinSeconds(string text, int timeoutSeconds)
        {
            this.WpfAppWindow.WaitUntilTextIsDisplayedBeforeTimeout(text, timeoutSeconds);
        }

        [When(@"I close the app")]
        public void WhenICloseTheApp()
        {
            this.WpfAppWindow.Close();
        }
        
        [Then(@"the window closes")]
        public void ThenTheWindowCloses()
        {
            this.App.RetryWaitForWindowClosed(this.wpfAppWindowTitle);
        }

        [When(@"Visual Studio (.*) finishes debugging")]
        public void WhenVisualStudioFinishesDebugging(int p0)
        {
            this.MainWindow.WaitUntilFinishesDebugging();
            this.mainWindowTitle = this.MainWindow.Title;
        }

        [When(@"I close Visual Studio (.*)")]
        public void WhenICloseVisualStudio(int p0)
        {
            this.MainWindow.Close();
        }

        [Then(@"Visual Studio (.*) closes")]
        public void ThenVisualStudioCloses(int p0)
        {
            Logger.Info($"Waiting for Visual Studio main window closed.");
            this.App.RetryWaitForWindowClosed(this.mainWindowTitle);
            Logger.Info($"Visual Studio main window is closed.");
            this.App.Dispose();
        }

        private IVisualStudioApp CreateVisualStudioApp()
        {
            const string AssemblyString = "TestFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            Assembly.Load(AssemblyString);
            Logger.Info("Loaded TestFramework.dll.");

            var type = Type.GetType($"TestFramework.VisualStudioApp, {AssemblyString}");
            Assert.IsNotNull(type, "Failed to get TestFramework.VisualStudioApp type.");

            var obj = Activator.CreateInstance(type);
            Assert.IsNotNull(obj, "Failed to instantiate VisualStudioApp object.");

            var result = obj as IVisualStudioApp;
            Assert.IsNotNull(result, "Instantiated object does not implement IVisualStudioApp interface.");
            return result;
        }
    }
}
