using FlaUI.Core.AutomationElements;
using Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework
{
    public class OpeningWizardPage3
    {
        private readonly Window window;

        public OpeningWizardPage3(Window window)
        {
            this.window = window;
        }

        private TextBox ProjectNameTextBox => this.window.RetryFindTextBox("/Custom/Custom/Pane/Edit[1]");

        private ComboBox LocationComboBox => this.window.RetryFindComboBox("/Custom/Custom/Pane/Custom/ComboBox");
        
        private TextBox SolutionNameTextBox => this.window.RetryFindTextBox("/Custom/Custom/Pane/Edit[2]");

        private ComboBox FrameworkComboBox => this.window.RetryFindComboBox("/Custom/Custom/Pane/ComboBox");

        private Button CreateButton => this.window.RetryFindButton("/Button[3]");

        public void CreateNewWpfApp(ProjectInfo projectInfo)
        {
            this.ProjectNameTextBox.Text = projectInfo.ProjectName;
            this.LocationComboBox.Value = projectInfo.Location;
            this.SolutionNameTextBox.Text = projectInfo.SolutionName;
            this.SelectFramework(projectInfo.Framework);

            this.CreateButton.WaitUntilEnabled();
            this.CreateButton.Click();
        }

        private void SelectFramework(string framework)
        {
            var found = false;
            this.FrameworkComboBox.Expand();
            foreach (var item in this.FrameworkComboBox.Items)
            {
                if (item.Text == framework)
                {
                    item.Select();
                    found = true;
                }
            }

            this.FrameworkComboBox.Collapse();
            Assert.IsTrue(found, $"'{framework}' cannot be found.");
        }
    }
}
