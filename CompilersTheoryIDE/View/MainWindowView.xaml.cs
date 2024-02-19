using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace CompilersTheoryIDE.View;

public partial class MainWindowView : Window
{
    private bool _isTextChanged;

    public MainWindowView()
    {
        InitializeComponent();
        CreateMockupErrors();
        Drop += MainWindow_Drop;
        DataContext = this;
        TextEditor.TextChanged += (sender, e) => _isTextChanged = true;
        Closing += WindowClosing;
        
        //  DispatcherTimer setup
        var timer = new DispatcherTimer();
        timer.Tick += _timer_Tick;
        timer.Interval = new TimeSpan(0, 0, 1);
        timer.Start();
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
    
    private DateTime _sessionTime = new(0, 0);
    private void _timer_Tick(object? sender, EventArgs e)
    {
        // Updating the Label which displays the current second
        CurrentTime.Text = DateTime.Now.ToString("H:mm:ss");

        // updating onsession timer
        _sessionTime = _sessionTime.AddSeconds(1);

        // Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested();
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
        const string helpFunctions =
            $"Создать: Создает новый файл или проект." +
            $"\n\nОткрыть: Открывает существующий файл или проект из файловой системы." +
            $"\n\nСохранить: Сохраняет текущий файл." +
            $"\n\nСохранить как: Сохраняет текущий файл с новым именем или в новом месте." +
            $"\n\nВыход: Закрывает IDE." +
            $"\n\nОтменить: Отменяет последнее действие." +
            $"\n\nПовторить: Повторяет отмененное действие." +
            $"\n\nВырезать: Удаляет выделенный текст или элемент и помещает его в буфер обмена." +
            $"\n\nКопировать: Копирует выделенный текст или элемент в буфер обмена." +
            $"\n\nВставить: Вставляет содержимое буфера обмена в текущее место курсора." +
            $"\n\nУдалить: Удаляет выделенный текст или элемент без помещения в буфер обмена." +
            $"\n\nВыделить все: Выделяет весь текст или элемент в текущем окне или документе.";
        MessageBox.Show(helpFunctions, "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void GetAbout_Click(object sender, RoutedEventArgs e)
    {
        //TODO: Заменить на словарь
        MessageBox.Show("Orangutan IDE. Версия: 18.02.2024. Все права защищены.", "О программе");
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