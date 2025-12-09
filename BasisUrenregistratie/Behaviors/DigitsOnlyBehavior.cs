using System.Linq;
using Microsoft.Maui.Controls;

namespace BasisUrenregistratie.Views;

// Behavior that keeps only digits; rejects any non-digit instantly.
public class DigitsOnlyBehavior : Behavior<Entry>
{
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
        if (string.IsNullOrEmpty(e.NewTextValue))
        {
            _lastValid = string.Empty;
            return;
        }

        if (e.NewTextValue.All(char.IsDigit))
        {
            _lastValid = e.NewTextValue;
        }
        else
        {
            ((Entry)sender).Text = _lastValid;
        }
    }
}
