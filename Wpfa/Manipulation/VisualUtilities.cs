using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using WindowsInput;

namespace Wpfa.Manipulation
{
    public static class VisualUtilities
    {
        private const int MaxAbsoluteValue = 65535;

        public static void Wait(TimeSpan period)
        {
            WaitUntil(DateTime.Now.Add(period));
        }

        private static void WaitUntil(DateTime finalDate)
        {
            var frame = new DispatcherFrame();

            WaitOperation(finalDate, frame);

            Dispatcher.PushFrame(frame);
        }

        private static void WaitOperation(DateTime finalDate, DispatcherFrame frame)
        {
            if (DateTime.Now.CompareTo(finalDate) > 0)
            {
                frame.Continue = false;
            }
            else
            {
                frame.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => WaitOperation(finalDate, frame)));
            }
        }

        public static void Click(this FrameworkElement element)
        {
            var point = element.PointToScreen(new Point(element.ActualWidth / 2, element.ActualHeight / 2));
            var abslutePoint = ConvertToAbsolute(point);
            var inputSimulator = new InputSimulator();

            inputSimulator.Mouse.MoveMouseTo(abslutePoint.X, abslutePoint.Y);
            inputSimulator.Mouse.LeftButtonClick();
        }

        private static Point ConvertToAbsolute(Point screenPoint)
        {
            var bounds = Screen.PrimaryScreen.Bounds;

            return new Point(screenPoint.X / bounds.Width * MaxAbsoluteValue, screenPoint.Y / bounds.Height * MaxAbsoluteValue);
        }

        public static T FindByAutomationId<T>(Window window, string automationId)
            where T : DependencyObject
        {
            return EnumerateVisualTree(window).OfType<T>().Single(i => AutomationProperties.GetAutomationId(i) == automationId);
        }

        public static T FindByName<T>(Window window, string name)
            where T : FrameworkElement
        {
            return EnumerateVisualTree(window).OfType<T>().Single(i => i.Name == name);
        }

        public static IEnumerable<DependencyObject> EnumerateVisualTree(DependencyObject root)
        {
            yield return root;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);

                foreach (var element in EnumerateVisualTree(child))
                {
                    yield return element;
                }
            }
        }
    }
}