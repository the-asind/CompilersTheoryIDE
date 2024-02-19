using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using System.ComponentModel;

namespace CompilersTheoryIDE.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private DispatcherTimer _timer;
    public event PropertyChangedEventHandler PropertyChanged;
    private bool _isTextChanged;
    private string _currentFilePath;

    // Time display formatting
    private string _currentTime;
    public string CurrentTime
    {
        get => _currentTime;
        set 
        { 
            _currentTime = value; 
            OnPropertyChanged(nameof(CurrentTime)); 
        }
    }

    // Constructor
    public MainWindowViewModel()
    {
        SetupTimer();
    }

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

    // Commands and logic related to text editing, file operations, etc., should be implemented here
    // as part of transitioning to MVVM. Example commands can include file open, file save, editing commands,
    // and so on.
}