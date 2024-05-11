using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompilersTheoryIDE.Model;
using CompilersTheoryIDE.ViewModel;
using Microsoft.Win32;

namespace CompilersTheoryIDE.View;

public partial class MainWindowView
{
    private readonly LexicalScanner _lexicalScanner;
    private bool _isTextChanged;
    private readonly MainWindowViewModel _viewModel;

    public MainWindowView()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
        _lexicalScanner = new LexicalScanner();
        SetupEventHandlers();
        // Handle the SelectionChanged event of the DataGrid
        LexemeDataGrid.SelectionChanged += LexemeDataGrid_SelectionChanged;

        App.LanguageChanged += LanguageChanged;

        var currLang = App.Language;

        //Заполняем меню смены языка:
        MenuLanguage.Items.Clear();
        foreach (var menuLang in App.Languages.Select(lang => new MenuItem
                 {
                     Header = lang.DisplayName,
                     Tag = lang,
                     IsChecked = lang.Equals(currLang)
                 }))
        {
            menuLang.Click += ChangeLanguageClick;
            MenuLanguage.Items.Add(menuLang);
        }
        
        _viewModel.Lexemes = new ObservableCollection<Lexeme>();
        _viewModel.ParseTokens(new ObservableCollection<Lexeme>());
    }
    
    private void LexemeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LexemeDataGrid.SelectedItem is not Lexeme selectedLexeme) 
            return;
        
        // Calculate the text range that needs to be selected in the TextEditor
        var selectionStart = selectedLexeme.IndexStart;
        var selectionLength = selectedLexeme.IndexEnd - selectedLexeme.IndexStart + 1;

        // Set the SelectionStart and SelectionLength properties of the TextEditor
        TextEditor.SelectionLength = 0;
        TextEditor.SelectionStart = selectionStart;
        TextEditor.SelectionLength = selectionLength;
    }
    
    private void LanguageChanged(object? sender, EventArgs e)
    {
        var currLang = App.Language;

        //Отмечаем нужный пункт смены языка как выбранный язык
        foreach (MenuItem i in MenuLanguage.Items)
        {
            var ci = i.Tag as CultureInfo;
            i.IsChecked = ci != null && ci.Equals(currLang);
        }
    }
    
    private void TaskStatement_Click(object sender, RoutedEventArgs e)
    {
        // Путь к HTML-файлу
        string htmlFilePath = @"Resources\task_statement.html";

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(htmlFilePath)
        {
            UseShellExecute = true
        };
        p.Start();
    }
    
    private void Grammar_Click(object sender, RoutedEventArgs e)
    {
        // Путь к HTML-файлу
        string htmlFilePath = @"Resources\grammar.html";

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(htmlFilePath)
        {
            UseShellExecute = true
        };
        p.Start();
    }
    
    private void GrammarClassification_Click(object sender, RoutedEventArgs e)
    {
        // Путь к HTML-файлу
        string htmlFilePath = @"Resources\grammar_classification.html";

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(htmlFilePath)
        {
            UseShellExecute = true
        };
        p.Start();
    }
    
    private void AnalysisMethod_Click(object sender, RoutedEventArgs e)
    {
        // Путь к HTML-файлу
        string htmlFilePath = @"Resources\analysis_method.html";

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(htmlFilePath)
        {
            UseShellExecute = true
        };
        p.Start();
    }
    
    private void LiteratureList_Click(object sender, RoutedEventArgs e)
    {
        // Путь к HTML-файлу
        string htmlFilePath = @"Resources\literature_list.html";

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(htmlFilePath)
        {
            UseShellExecute = true
        };
        p.Start();
    }
    
    private void SourceCode_Click(object sender, RoutedEventArgs e)
    {
        // Путь к HTML-файлу
        string htmlFilePath = @"https://github.com/the-asind/CompilersTheoryIDE";

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(htmlFilePath)
        {
            UseShellExecute = true
        };
        p.Start();
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
        TextEditor.TextChanged += TextEditor_TextChanged;
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

    private void FixErrors_Click(object sender, RoutedEventArgs e)
    {
        var removeOffset = 0;
        foreach (var action in _viewModel.NeutralizeErrors())
        {
            switch (action.Item1)
            {
                case "insert":
                    TextEditor.Text = TextEditor.Text.Insert(action.Item2+removeOffset, action.Item3);
                    removeOffset += 3;
                    break;
                case "remove":
                    TextEditor.Text = TextEditor.Text.Remove
                        (action.Item2+removeOffset, int.Parse(action.Item3));
                    removeOffset -= int.Parse(action.Item3);
                    break;
            }
        }
    }

    private void TextEditor_TextChanged(object? sender, EventArgs eventArgs)
    {
        var tokens = new ObservableCollection<Lexeme>(_lexicalScanner.Analyze(TextEditor.Text));
        _viewModel.Lexemes = tokens;
        
        // Parse the tokens and update the ParserGrid
        _viewModel.ParseTokens(tokens);
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
        _viewModel.CurrentFilePath = filePath;
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
        if (string.IsNullOrEmpty(_viewModel.CurrentFilePath))
            SaveAs_Click(sender, e);
        else
            TextEditor.Save(_viewModel.CurrentFilePath);
    }

    private void SaveFile()
    {
        var saveFileDialog = new SaveFileDialog { Filter = "Orangutan files (*.orangutan)|*.orangutan" };
        if (saveFileDialog.ShowDialog() != true) return;
        TextEditor.Save(saveFileDialog.FileName);
        _viewModel.CurrentFilePath = saveFileDialog.FileName;
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

    private void TestExample_Click(object sender, RoutedEventArgs e)
    {
        TextEditor.Text = "Hello # 123''[]\"\"\"\n\nWorld'''\nGenre\n'''error # correct \n #";
    }

    private void StartAntlr_Click(object sender, RoutedEventArgs e)
    {
    }
}