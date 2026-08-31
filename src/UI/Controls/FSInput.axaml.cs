using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Collections;

namespace Diorama;

public class FSInput : LabelledInput
{
    public static readonly StyledProperty<string> ValueProperty =
    AvaloniaProperty.Register<FSInput, string>(nameof(Value));

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private Button _button;
    private TextBox _textbox;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_button is not null)
            _button.Click -= ButtonClicked;

        _button = e.NameScope.Find<Button>("PART_Button");
        _textbox = e.NameScope.Find<TextBox>("PART_TextBox");

        if (_button is not null)
            _button.Click += ButtonClicked;
    }

    public Func<string, string> Check;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            ValidatePath();
        }
    }

    private void ValidatePath()
    {
        if (_textbox is null)
            return;

        if (Directory.Exists(Value))
        {
            DataValidationErrors.ClearErrors(_textbox);

            string? result = Check?.Invoke(Value);

            if (!string.IsNullOrEmpty(result))
            {
                DataValidationErrors.SetError(
                    _textbox,
                    new DataValidationException(result));
            }
        }
        else
        {
            DataValidationErrors.SetError(
            _textbox,
                new DataValidationException("Invalid directory"));
        }
    }

    private async void ButtonClicked(object? sender, RoutedEventArgs e)
    {
        TopLevel topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null || !topLevel.StorageProvider.CanPickFolder)
            return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select Folder",
                AllowMultiple = false
            });

        if (folders.Count == 0)
            return;

        Value = folders[0].Path.LocalPath;
    }
}