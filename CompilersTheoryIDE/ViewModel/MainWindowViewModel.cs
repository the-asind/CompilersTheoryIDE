using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using CompilersTheoryIDE.Model;

namespace CompilersTheoryIDE.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    // Time display formatting
    private string _currentTime;
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

    public void ParseTokens(ObservableCollection<Lexeme> tokens)
    {
        Parser parser = new Parser();
        ParserGrid = new ObservableCollection<ParserError>(parser.Parse(tokens));
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
    private void TimerTick(object sender, EventArgs e)
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