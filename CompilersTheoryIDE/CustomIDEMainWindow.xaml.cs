using System.ComponentModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace CompilersTheoryIDE
{
    public partial class CustomIDEMainWindow : Window
    {
        private string _currentFilePath;
        private bool isTextChanged = false;
        
        public CustomIDEMainWindow()
        {
            InitializeComponent();
            textEditor.TextChanged += (sender, e) => isTextChanged = true;
            // Определить обработчик событий Closing
            Closing += WindowClosing;
        }
        
        private int CheckIfTextWasChanged()
        {
            if (!isTextChanged) return 0; // No changes were made, proceed with any intended action
            var result = MessageBox.Show("Сохранить изменения в файле?", 
                "Сохранение", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        
            // Check user's response and act accordingly
            return result switch
            {
                MessageBoxResult.Yes => 1 // Proceed to save changes
                ,
                MessageBoxResult.No => 0 // Do not save changes
                ,
                MessageBoxResult.Cancel => -1 // Cancel the current action (exit or new file)
                ,
                _ => 0 // No changes were made, proceed with any intended action
            };
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            switch (CheckIfTextWasChanged())
            {
                case 1:
                    SaveFile();
                    break;
                case 0:
                    break;
                case -1:
                    return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Orangutan files (*.orangutan)|*.orangutan";

            if (openFileDialog.ShowDialog() == true)
            {
                textEditor.Load(openFileDialog.FileName);
                isTextChanged = false; // Reset flag as we just loaded a new file
                _currentFilePath = openFileDialog.FileName;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
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
            switch (CheckIfTextWasChanged())
            {
                case 1:
                    SaveFile();
                    break;
                case 0:
                    Close();
                    break;
                case -1:
                    break;
            }
        }

        private void CreateNewFile_Click(object sender, RoutedEventArgs e)
        {
            switch (CheckIfTextWasChanged())
            {
                case 1:
                    SaveFile();
                    break;
                case 0:
                    break;
                case -1:
                    return;
            }

            textEditor.Clear();
            isTextChanged = false; // Reset flag because text is now cleared
        }

        private void CopySelectedText_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            Clipboard.SetText(selectedText);
        }
        
        private void PasteText_Click(object sender, RoutedEventArgs e) => textEditor.Paste();
        private void CutText_Click(object sender, RoutedEventArgs e) => textEditor.Cut();
        private void DeleteText_Click(object sender, RoutedEventArgs e) => textEditor.Delete();
        private void SelectAllText_Click(object sender, RoutedEventArgs e) => textEditor.SelectAll();
        private void UndoText_Click (object sender, RoutedEventArgs e) => textEditor.Undo();
        private void RedoText_Click (object sender, RoutedEventArgs e) => textEditor.Redo();
        
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
                SaveAs_Click(sender, e);
            else
                textEditor.Save(_currentFilePath);
        }
        
        private void SaveAs_Click(object sender, RoutedEventArgs e) => SaveFile();

        private void SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "Orangutan files (*.orangutan)|*.orangutan" 
            };
            if (saveFileDialog.ShowDialog() != true) return;
            textEditor.Save(saveFileDialog.FileName);
            _currentFilePath = saveFileDialog.FileName;
        }

        private void GetHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpFunctions = // TODO: Заменить на словарь
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
        { //TODO: Заменить на словарь
            MessageBox.Show("Orangutan IDE. Версия: 18.02.2024. Все права защищены.", "О программе");
        }
    }
}