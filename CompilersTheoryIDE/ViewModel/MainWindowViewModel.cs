using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

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