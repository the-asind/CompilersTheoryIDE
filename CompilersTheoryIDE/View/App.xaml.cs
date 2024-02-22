using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using CompilersTheoryIDE.Resources;

namespace CompilersTheoryIDE.View;

public partial class App
{
    public static List<CultureInfo> Languages { get; } = new();

    public App()
    {
        InitializeComponent();
        LanguageChanged += App_LanguageChanged;

        Languages.Clear();
        Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
        Languages.Add(new CultureInfo("ru-RU"));

        Language = Settings.Default.DefaultLanguage;
    }

    //Евент для оповещения всех окон приложения
    public static event EventHandler? LanguageChanged;

    public static CultureInfo Language
    {
        get => Thread.CurrentThread.CurrentUICulture;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (Equals(value, Thread.CurrentThread.CurrentUICulture)) return;

            //1. Меняем язык приложения:
            Thread.CurrentThread.CurrentUICulture = value;

            //2. Создаём ResourceDictionary для новой культуры
            var dict = new ResourceDictionary
            {
                Source = value.Name == "ru-RU"
                    ? new Uri($"Resources/lang.{value.Name}.xaml", UriKind.Relative)
                    : new Uri("Resources/lang.xaml", UriKind.Relative)
            };
            // TODO: вставить корректный запомненный язык, не по умолчанию 
            //3. Находим старую ResourceDictionary и удаляем его, добавляем новую ResourceDictionary
            var oldDict = (from d in Current.Resources.MergedDictionaries
                where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                select d).FirstOrDefault(); // Используем FirstOrDefault вместо First
            if (oldDict != null) // Проверяем, найден ли старый словарь ресурсов
            {
                var ind = Current.Resources.MergedDictionaries.IndexOf(oldDict);
                Current.Resources.MergedDictionaries.Remove(oldDict);
                Current.Resources.MergedDictionaries.Insert(ind, dict);
            }
            else // Если старый словарь ресурсов не был найден, просто добавляем новый
            {
                Current.Resources.MergedDictionaries.Add(dict);
            }


            //4. Вызываем евент для оповещения всех окон.
            LanguageChanged?.Invoke(Current, EventArgs.Empty);
        }
    }

    private static void App_LanguageChanged(object? sender, EventArgs e)
    {
        Settings.Default.DefaultLanguage = Language;
        Settings.Default.Save();
    }
}