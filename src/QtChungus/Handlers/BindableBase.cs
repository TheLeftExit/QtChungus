using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QtChungus;

public class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> eventArgsCache = new();

    protected void RaisePropertyChanged(string? propertyName = null)
    {
        var args = eventArgsCache.GetOrAdd(propertyName ?? "$null", x => new PropertyChangedEventArgs(x));
        PropertyChanged?.Invoke(this, args);
    }
    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null, params ReadOnlySpan<string> propertyNames)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        RaisePropertyChanged(propertyName);
        foreach (var name in propertyNames)
        {
            RaisePropertyChanged(name);
        }
    }
}
