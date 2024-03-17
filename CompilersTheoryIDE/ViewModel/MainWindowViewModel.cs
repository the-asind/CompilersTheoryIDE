using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using CompilersTheoryIDE.Model;

namespace CompilersTheoryIDE.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    // Time display formatting
    private string _currentTime;
    private int _errorsCount = 0;
    private DispatcherTimer _timer;

    // Constructor
    public MainWindowViewModel()
    {
        SetupTimer();
    }

    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            OnPropertyChanged(nameof(CurrentTime));
        }
    }
    
    public string ErrorsCount
    {
        get => _errorsCount.ToString();
        private set
        {
            _errorsCount = int.Parse(value);
            OnPropertyChanged(nameof(ErrorsCount));
        }
    }
    
    public IEnumerable<(string, int, string)> NeutralizeErrors()
    {
        foreach (var error in ParserGrid)
        {
            switch (error.ErrorName)
            {
                case "Unclosed MultiLineDoubleQuotesComment":
                {
                    // Add """ at the error location
                    var errorLocation = int.Parse(error.ErrorLocation!.Split('-')[1]);
                    yield return ("insert", errorLocation, "\"\"\"");
                    break;
                }
                case "Unclosed MultiLineSingleQuotesComment":
                {
                    // Add ''' at the error location
                    var errorLocation = int.Parse(error.ErrorLocation!.Split('-')[1]);
                    yield return ("insert", errorLocation, "'''");
                    break;
                }
                case "Unexpected Symbol Sequence":
                {
                    // Remove the unexpected symbols
                    var errorStart = int.Parse(error.ErrorLocation!.Split('-')[0]);
                    var errorEnd = int.Parse(error.ErrorLocation.Split('-')[1]);
                    var length = errorEnd - errorStart;
                    yield return ("remove", errorStart, length.ToString());
                    break;
                }
            }
        }
    }
    
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
    
    private ObservableCollection<Lexeme> _lexemes;
    private Lexeme _selectedLexeme;

    public ObservableCollection<Lexeme> Lexemes
    {
        get => _lexemes;
        set
        {
            _lexemes = value;
            OnPropertyChanged(nameof(Lexemes));
        }
    }

    public Lexeme SelectedLexeme
    {
        get => _selectedLexeme;
        set
        {
            _selectedLexeme = value;
            OnPropertyChanged(nameof(SelectedLexeme));
        }
    }
    
    private ObservableCollection<ParserError> _parserGrid;

    public ObservableCollection<ParserError> ParserGrid
    {
        get => _parserGrid;
        set
        {
            _parserGrid = value;
            OnPropertyChanged(nameof(ParserGrid));
        }
    }

    public void ParseTokens(IEnumerable<Lexeme> tokens)
    {
        var parser = new Parser();
        var errors = parser.Parse(tokens);
        ParserGrid = new ObservableCollection<ParserError>(errors);
        ErrorsCount = ParserGrid.Count.ToString();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // Initializes and starts the timer.
    private void SetupTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += TimerTick;
        _timer.Start();
    }

    // Handles the timer's tick event.
    private void TimerTick(object? sender, EventArgs e)
    {
        CurrentTime = DateTime.Now.ToString("H:mm:ss");
        CommandManager.InvalidateRequerySuggested();
    }

    // Method to signal property changed.
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}