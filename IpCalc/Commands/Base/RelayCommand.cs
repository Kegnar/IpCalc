using System.Windows.Input;

namespace IpCalc.Commands.Base;

public class RelayCommand : Command
{
    private readonly Action<object> _execute;
    private readonly Func<Object, bool> _canExecute;
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    public override void Execute(object? parameter) => _execute(parameter);

    public override bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;


}
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;
    public void Execute(object? parameter) => _execute((T?)parameter);
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}