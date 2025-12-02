// language: csharp
using System.Linq;
using Microsoft.Maui.Controls;

namespace BasisUrenregistratie.Views;

public class NumericMaxBehavior : Behavior<Entry>
{
    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.Create(nameof(MaxValue), typeof(int), typeof(NumericMaxBehavior), 24);

    public int MaxValue
    {
        get => (int)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    private string _lastValid = string.Empty;

    protected override void OnAttachedTo(Entry bindable)
    {
        bindable.TextChanged += OnTextChanged;
        base.OnAttachedTo(bindable);
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
        bindable.TextChanged -= OnTextChanged;
        base.OnDetachingFrom(bindable);
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        var newText = e.NewTextValue ?? string.Empty;

        // allow empty (user clearing)
        if (newText.Length == 0)
        {
            _lastValid = string.Empty;
            return;
        }

        // digits only
        if (!newText.All(char.IsDigit))
        {
            entry.Text = _lastValid;
            return;
        }

        // parse and enforce max
        if (int.TryParse(newText, out var value))
        {
            if (value <= MaxValue)
            {
                _lastValid = newText;
            }
            else
            {
                // revert; optionally clamp to MaxValue
                entry.Text = _lastValid.Length > 0 ? _lastValid : MaxValue.ToString();
                _lastValid = entry.Text;
            }
        }
        else
        {
            entry.Text = _lastValid;
        }
    }
}
