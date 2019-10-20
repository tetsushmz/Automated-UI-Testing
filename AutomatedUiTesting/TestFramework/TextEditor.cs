using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.Core.WindowsAPI;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework
{
    public class TextEditor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TextEditor));

        private readonly Window window;

        private TextBox textEditor;
        private TabItem currentTab;
        private string filename;
        private string code;

        public TextEditor(Window window, string filename, string code)
        {
            this.window = window;
            this.filename = filename;
            this.code = code;
        }

        public void LoadCode()
        {
            Logger.Info($"Loading code to '{this.filename}'");
            this.currentTab = this.RetryFindTabItem();
            this.currentTab.Select();
            this.textEditor = this.RetryFindTextEditor();

            var retryResult = Retry.WhileFalse(
                this.DoLoadCode,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(0.1));
            Assert.IsTrue(
                retryResult.Success,
                $"Failed to load code to {this.filename}.");
        }

        private TabItem RetryFindTabItem()
        {
            var retryResult = Retry.WhileNull(
                this.FindTabItem,
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(0.1));
            Assert.IsTrue(
                retryResult.Success,
                $"Tab item for '{this.filename}' cannot be found.");
            var result = retryResult.Result;
            return result;
        }

        private TextBox RetryFindTextEditor()
        {
            var retryResult = Retry.WhileNull(
                this.FindTextEditor,
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(0.1));
            Assert.IsTrue(
                retryResult.Success,
                $"Text editor for '{this.filename} cannot be found.");
            var result = retryResult.Result;
            return result;
        }

        private bool DoLoadCode()
        {
            Logger.Debug($"Clearing Text Editor for '{this.filename}'.");
            this.RetryClearEditor();
            Logger.Debug($"Setting code for '{this.filename}'.");
            var result = this.SetCode();
            return result;
        }

        private void RetryClearEditor()
        {
            var retryResult = Retry.WhileFalse(
                this.ClearEditor,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(0.1),
                false,
                ignoreException: true);
            Assert.IsTrue(retryResult.Success, "Failed to clear Text Editor.");
        }

        private bool ClearEditor()
        {
            this.textEditor.Focus();

            // Select all existing text.
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);

            // Hit Del key.
            Keyboard.Type(VirtualKeyShort.DELETE);

            var result = this.IsTextEditorCleared();
            Logger.Debug(result
                ? $"{nameof(this.ClearEditor)}() succeeded."
                : $"Retrying {nameof(this.ClearEditor)}(). Current text is:\n\n{this.textEditor.Text}\n");

            return result;
        }

        private bool SetCode()
        {
            System.Windows.Clipboard.SetText(this.code);

            this.textEditor.Focus();

            // Paste from clipboard.
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_V);
            
            /* We should wait a second at maximum before being able to see
             * the text editor content changed.
             */
            Retry.WhileTrue(
                this.IsTextEditorCleared,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(0.1));

            var currentText = this.textEditor.Text;
            var result = currentText == this.code;
            Logger.Debug(result
                ? $"{nameof(this.SetCode)}() succeeded with the following code:\n\n{currentText}\n"
                : $"Retrying {nameof(this.SetCode)}(Current text is:\n\n{currentText}\n\n");

            return result;
        }

        private bool IsTextEditorCleared()
        {
            /* The Text property contains "1 ", rather than an empty string when "Line numbers" option
             * (Tools/Options/Text Editor/C#/General and Tools/Options/Text Editor/XAML/General)
             * is enabled.
             */
            Wait.UntilResponsive(this.textEditor);
            var currentText = this.textEditor.Text;
            var result = currentText == "1 " || currentText == string.Empty;
            return result;
        }

        private TextBox FindTextEditor()
        {
            var result = this.currentTab
                .FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit).And(cf.ByName("Text Editor")))
                .AsTextBox();
            if (result == null)
            {
                Logger.Info($"Retrying getting Text Editor for '{this.filename}.");
            }

            return result;
        }

        private TabItem FindTabItem()
        {
            var result = this.window.FindFirstDescendant(
                cf => cf.ByControlType(ControlType.TabItem)
                    .And(cf.ByName(this.filename))).AsTabItem();
            if (result == null)
            {
                Logger.Info($"Retrying getting the tab for '{this.filename}'.");
            }

            return result;
        }
    }
}
