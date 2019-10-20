using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework
{
    public class OpeningWizardPage2
    {
        private readonly Window window;

        public OpeningWizardPage2(Window window)
        {
            this.window = window;
        }

        private ListBox ProjectTemplateListBox => this.window.RetryFindListBox("/Custom/Custom/List");

        private Button NextButton => this.window.RetryFindButton("/Button[3]");

        public void CreateNewWpfApp(ProjectInfo projectInfo)
        {
            this.SelectWpfProjectTemplate();
            this.NextButton.Click();

            var page3 = new OpeningWizardPage3(this.window);
            page3.CreateNewWpfApp(projectInfo);
        }

        private void SelectWpfProjectTemplate()
        {
            const string WpfProject = "WPF App (.NET Framework)";
            var item = this.FindListBoxItemRecursively(WpfProject);
            Assert.IsNotNull(item, $"'{WpfProject}' cannot be found in the list.");

            item.Select();
        }

        private ListBoxItem FindListBoxItemRecursively(string projectType)
        {
            var items = this.ProjectTemplateListBox.Items;
            foreach (var item in items)
            {
                if (item.Name == projectType)
                {
                    return item;
                }
            }

            if (items[items.Length - 1].Name == "Not finding what you're looking for? Install more tools and features")
            {
                // We have scrolled down to the bottom.
                return null;
            }

            this.ProjectTemplateListBox.Patterns.Scroll.Pattern.Scroll(
                ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
            var result = this.FindListBoxItemRecursively(projectType);
            return result;
        }
    }
}
