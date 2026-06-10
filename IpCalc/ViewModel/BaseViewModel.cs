using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IpCalc.ViewModel;

public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    //InotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    //INotifyDataErrorInfo
    private Dictionary<string, List<string>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public bool HasErrors => _errors.Any();
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            return Enumerable.Empty<string>();
        return _errors[propertyName];
    }

    protected void AddError(string error, [CallerMemberName] string? propertyName = null)
    {
        if(string.IsNullOrEmpty(propertyName))return;
        
        if (!_errors.ContainsKey(propertyName))
        {
            _errors[propertyName] = new List<string>();
            
        }

        if (!_errors[propertyName].Contains(error))
        {
            _errors[propertyName].Add(error);
            ErrorsChanged?.Invoke(this,new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }
    }

    protected void ClearErrors([CallerMemberName] string? propertyName = null)
    {
        if(string.IsNullOrEmpty(propertyName)) return;
        if (_errors.Remove(propertyName))
        {
            ErrorsChanged?.Invoke(this,new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(propertyName));
        }
    }


}