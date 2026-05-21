using System;
using System.Collections.Generic;
using System.Text;
using IpCalc.Commands.Base;

namespace IpCalc.Commands
{
    internal class CalculateCommand :RelayCommand
    {
        public CalculateCommand(Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute)
        {
            throw new NotImplementedException(message:nameof(execute));
        }
    }
}
