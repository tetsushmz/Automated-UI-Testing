Feature: Building WPF app
    In order to meet stakeholder requirements
    As a C# programmer
    I want to be able to build a WPF app

Background:
    Given I have started Visual Studio 2019
    And I have removed "WpfAppForAutomatedUiTesting" folder, if any, in "C:\temp"

@tag1
Scenario: Creating and running a new WPF app
    When I create a new WPF app with the following info
    | Project name                | Location | Solution name               | Framework          |
    | WpfAppForAutomatedUiTesting | C:\temp  | WpfAppForAutomatedUiTesting | .NET Framework 4.6 |
    Then Visual Studio main window opens
    When I enter the following code to "MainWindow.xaml"
"""
<Window
    x:Class="WpfAppForAutomatedUiTesting.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="MainWindow"
    Width="800"
    Height="450"
    mc:Ignorable="d">
    <Grid>
        <TextBlock
            x:Name="TextBlock"
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            FontSize="70" />
    </Grid>
</Window>
"""
    And I enter the following code to "MainWindow.xaml.cs"
"""
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace WpfAppForAutomatedUiTesting
{
    public partial class MainWindow
    {
        private readonly TimeSpan duration = TimeSpan.FromSeconds(5);
        private readonly Stopwatch stopwatch;
        private readonly DispatcherTimer timer;

        public MainWindow()
        {
            this.InitializeComponent();
            this.stopwatch = Stopwatch.StartNew();
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.1) };
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var remaining = (this.duration - this.stopwatch.Elapsed).TotalSeconds;
            if (remaining < 0)
            {
                remaining = 0;
                this.timer.Stop();
            }

            this.TextBlock.Text = remaining.ToString("0.0");
        }
    }
}
"""
    And I start debugging the app
    Then A window with "MainWindow" as its title opens
    And the window starts counting down and shows "0.0" within 10 seconds
    When I close the app
    Then the window closes
    When Visual Studio 2019 finishes debugging
    And I close Visual Studio 2019
    Then Visual Studio 2019 closes
