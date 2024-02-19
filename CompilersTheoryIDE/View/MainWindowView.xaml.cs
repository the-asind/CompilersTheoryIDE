using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CompilersTheoryIDE.ViewModel;
using Microsoft.Win32;

namespace CompilersTheoryIDE.View;

public partial class MainWindowView : Window
{
    private bool _isTextChanged;
    private MainWindowViewModel _viewModel;

    public MainWindowView()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
        CreateMockupErrors();
        SetupEventHandlers();
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
    
    public class Error
    {
        public Error(int id, string filePath, int line, int column, string message)
        {
            FilePath = filePath;
            Message = message;
            Line = line;
            Column = column;
            Id = id;
        }
        
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Message { get; set; }
    }

    private void CreateMockupErrors()
    {
        var errors = new List<Error>
        {
            new(1, "MainWindow.xaml", 12, 15,
                "The property 'Text' is set more than once."),
            new(2, "MainWindow.xaml.cs", 17, 21,
                "The name 'button1' does not exist in the current context.")
        };

        ErrorsDataGrid.ItemsSource = errors;
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

    private void PasteText_Click(object sender, RoutedEventArgs e) => TextEditor.Paste();

    private void CutText_Click(object sender, RoutedEventArgs e) => TextEditor.Cut();

    private void DeleteText_Click(object sender, RoutedEventArgs e) => TextEditor.Delete();

    private void SelectAllText_Click(object sender, RoutedEventArgs e) => TextEditor.SelectAll();

    private void UndoText_Click(object sender, RoutedEventArgs e) => TextEditor.Undo();

    private void RedoText_Click(object sender, RoutedEventArgs e) => TextEditor.Redo();
    
    private void SaveAs_Click(object sender, RoutedEventArgs e) => SaveFile();

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
        string? helpFunctions = TryFindResource("HelpWindowContent") as string;
        MessageBox.Show(helpFunctions, TryFindResource("m_Help") as string, 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void GetAbout_Click(object sender, RoutedEventArgs e)
    {
        string? about = TryFindResource("AboutWindowContent") as string;
        about += Assembly.GetExecutingAssembly().GetName().Version;
        //TODO: Заменить на словарь
        MessageBox.Show(about, 
            "TryFindResource(\"m_About\") as string");
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