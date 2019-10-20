using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework
{
    public static class AutomationElementExtensions
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AutomationElementExtensions));

        public static Button RetryFindButton(this AutomationElement element, string xpath)
        {
            var result = element.RetryFindFirstByXPath(xpath).AsButton();
            Assert.IsNotNull(result, $"Failed to find a Button at '{xpath}'.");
            return result;
        }

        public static ListBox RetryFindListBox(this AutomationElement element, string xpath)
        {
            var result = element.RetryFindFirstByXPath(xpath).AsListBox();
            Assert.IsNotNull(result, $"Failed to find a ListBox at '{xpath}'.");
            return result;
        }

        public static TextBox RetryFindTextBox(this AutomationElement element, string xpath)
        {
            var result = element.RetryFindFirstByXPath(xpath).AsTextBox();
            Assert.IsNotNull(result, $"Failed to find a TextBox at '{xpath}'.");
            return result;
        }

        public static ComboBox RetryFindComboBox(this AutomationElement element, string xpath)
        {
            var result = element.RetryFindFirstByXPath(xpath).AsComboBox();
            Assert.IsNotNull(result, $"Failed to find a ComboBox at '{xpath}'.");
            return result;
        }

        public static Label RetryFindLabel(this AutomationElement element, string xpath)
        {
            var result = element.RetryFindFirstByXPath(xpath).AsLabel();
            Assert.IsNotNull(result, $"Failed to find a Label at '{xpath}'.");
            return result;
        }

        private static AutomationElement RetryFindFirstByXPath(this AutomationElement element, string xpath)
        {
            var retryResult = Retry.WhileNull(() =>
            {
                var result = element.FindFirstByXPath(xpath);
                if (result == null)
                {
                    Logger.Info($"Retrying '{nameof(AutomationElement.FindFirstByXPath)}('{xpath}')'.");
                }

                return result;
            });

            return retryResult.Result;
        }
    }
}
