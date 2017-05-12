using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;
using Wpfa;
using Wpfa.Manipulation;

namespace WPFCalculator.Tests
{
    [ProcessIsolation("WPFCalculator.exe")]
    public class TestFixture : ProcessIsolationTestBase
    {
        [Test]
        public void ClickOn1IsDisplayedInResultsBox()
        {
            // Given
            Window mainWindow = Application.Current.MainWindow;
            Button b1 = VisualUtilities.FindByAutomationId<Button>(mainWindow, "B1_automation_id");

            // When
            VisualUtilities.Click(b1);
            VisualUtilities.Wait(TimeSpan.FromSeconds(1));

            // Then
            MyTextBox resultsBox = VisualUtilities.FindByName<MyTextBox>(mainWindow, "DisplayBox");
            Assert.AreEqual(resultsBox.Text, "1");
        }
    }
}
