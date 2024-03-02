using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CompilersTheoryIDE.ViewModel;
using Microsoft.Win32;

namespace CompilersTheoryIDE.View;

public partial class MainWindowView : Window
{
    private bool _isTextChanged;
    private readonly MainWindowViewModel _viewModel;

    public MainWindowView()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
        SetupEventHandlers();

        App.LanguageChanged += LanguageChanged;

        var currLang = App.Language;

        //Заполняем меню смены языка:
        menuLanguage.Items.Clear();
        foreach (var lang in App.Languages)
        {
            var menuLang = new MenuItem
            {
                Header = lang.DisplayName,
                Tag = lang,
                IsChecked = lang.Equals(currLang)
            };
            menuLang.Click += ChangeLanguageClick;
            menuLanguage.Items.Add(menuLang);
        }
    }

    private void LanguageChanged(object sender, EventArgs e)
    {
        var currLang = App.Language;

        //Отмечаем нужный пункт смены языка как выбранный язык
        foreach (MenuItem i in menuLanguage.Items)
        {
            var ci = i.Tag as CultureInfo;
            i.IsChecked = ci != null && ci.Equals(currLang);
        }
    }

    private static void ChangeLanguageClick(object sender, EventArgs e)
    {
        if (sender is not MenuItem mi) return;
        if (mi.Tag is CultureInfo lang) App.Language = lang;
    }

    // Sets up event handlers for UI elements.
    private void SetupEventHandlers()
    {
        Drop += MainWindow_Drop;
        Closing += WindowClosing;
    }

    private void MainWindow_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files.Length <= 0) return;
        var filePath = files[0];
        if (!Path.GetExtension(filePath).Equals(".orangutan", StringComparison.InvariantCultureIgnoreCase))
            return;
        if (SaveFileCheckIsInterrupted()) return;
        OpenAndProcessFile(filePath);
    }

    private bool SaveFileCheckIsInterrupted()
    {
        switch (CheckIfTextWasChanged())
        {
            case 1:
                SaveFile();
                return false;
            case 0:
                return false;
            default:
                return true;
        }
    }

    private void OpenAndProcessFile(string filePath)
    {
        TextEditor.Load(filePath);
        _isTextChanged = false; // Reset flag as we just loaded a new file
        CurrentFilePath = filePath;
    }

    private int CheckIfTextWasChanged()
    {
        if (!_isTextChanged) return 0;
        var result = MessageBox.Show("Сохранить изменения в файле?",
            "Сохранение", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

        return result switch
        {
            MessageBoxResult.Yes => 1, // Proceed to save changes
            MessageBoxResult.No => 0, // Do not save changes
            MessageBoxResult.Cancel => -1, // Cancel the current action (exit or new file)
            _ => 0 // No changes were made, proceed with any intended action
        };
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        if (SaveFileCheckIsInterrupted()) return;

        var openFileDialog = new OpenFileDialog { Filter = "Orangutan files (*.orangutan)|*.orangutan" };

        if (openFileDialog.ShowDialog() == true) OpenAndProcessFile(openFileDialog.FileName);
    }

    private void WindowClosing(object? sender, CancelEventArgs e)
    {
        switch (CheckIfTextWasChanged())
        {
            case 1:
                SaveFile();
                break;
            case 0:
                break;
            case -1:
                e.Cancel = true;
                break;
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        if (SaveFileCheckIsInterrupted()) return;
        Close();
    }

    private void CreateNewFile_Click(object sender, RoutedEventArgs e)
    {
        if (SaveFileCheckIsInterrupted()) return;

        TextEditor.Clear();
        _isTextChanged = false;
    }

    private void CopySelectedText_Click(object sender, RoutedEventArgs e)
    {
        var selectedText = TextEditor.SelectedText;
        Clipboard.SetText(selectedText);
    }

    private void PasteText_Click(object sender, RoutedEventArgs e)
    {
        TextEditor.Paste();
    }

    private void CutText_Click(object sender, RoutedEventArgs e)
    {
        TextEditor.Cut();
    }

    private void DeleteText_Click(object sender, RoutedEventArgs e)
    {
        TextEditor.Delete();
    }

    private void SelectAllText_Click(object sender, RoutedEventArgs e)
    {
        TextEditor.SelectAll();
    }

    private void UndoText_Click(object sender, RoutedEventArgs e)
    {
        TextEditor.Undo();
    }

    private void RedoText_Click(object sender, RoutedEventArgs e)
    {
        TextEditor.Redo();
    }

    private void SaveAs_Click(object sender, RoutedEventArgs e)
    {
        SaveFile();
    }

    private void SaveFile_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
            SaveAs_Click(sender, e);
        else
            TextEditor.Save(_currentFilePath);
    }

    private void SaveFile()
    {
        var saveFileDialog = new SaveFileDialog { Filter = "Orangutan files (*.orangutan)|*.orangutan" };
        if (saveFileDialog.ShowDialog() != true) return;
        TextEditor.Save(saveFileDialog.FileName);
        _currentFilePath = saveFileDialog.FileName;
    }

    private void GetHelp_Click(object sender, RoutedEventArgs e)
    {
        var helpFunctions = TryFindResource("m_HelpWindowContent") as string;
        MessageBox.Show(helpFunctions, TryFindResource("m_Help") as string,
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void GetAbout_Click(object sender, RoutedEventArgs e)
    {
        var about = TryFindResource("m_AboutWindowContent") as string;
        about += Assembly.GetExecutingAssembly().GetName().Version;
        MessageBox.Show(about,
            TryFindResource("m_GetAbout") as string);
    }
}

public partial class MainWindowView : INotifyPropertyChanged
{
    private string _currentFilePath = "Новый документ";

    public string CurrentFilePath
    {
        get => _currentFilePath;
        set
        {
            _currentFilePath = value;
            OnPropertyChanged(nameof(CurrentFilePath));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}