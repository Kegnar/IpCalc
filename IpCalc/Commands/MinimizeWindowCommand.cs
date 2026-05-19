using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IpCalc.Commands
{
    internal class MinimizeWindowCommand : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
    }
}
