using FlaUI.Core.AutomationElements;
using Interfaces;

namespace TestFramework
{
    public class OpeningWizardPage1
    {
        private readonly Window window;

        public OpeningWizardPage1(Window window)
        {
            this.window = window;
        }

        private Button CreateNewAppButton => this.window.RetryFindButton("/Custom/Pane[2]/Button[4]");
        
        public void CreateNewWpfApp(ProjectInfo projectInfo)
        {
            this.CreateNewAppButton.Click();
            var page2 = new OpeningWizardPage2(this.window);
            page2.CreateNewWpfApp(projectInfo);
        }
    }
}
