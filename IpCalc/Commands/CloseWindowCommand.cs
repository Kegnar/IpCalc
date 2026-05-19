using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IpCalc.Commands
{
    public class CloseWindowCommand : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var window = Window.GetWindow(AssociatedObject);
            window.Close();
        }
    }
}
