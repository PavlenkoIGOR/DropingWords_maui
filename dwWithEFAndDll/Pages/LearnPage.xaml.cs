using MauiLib1.Models;
using Microsoft.Maui.Layouts;
using System.ComponentModel;
using System.Diagnostics;

namespace dwWithEFAndDll.Pages;

public partial class LearnPage : ContentPage, INotifyPropertyChanged
{
    List<Word> _choosenWords;
    IDispatcherTimer timer1;

    private string _currentWord; // Изменили поля для использования понятия INotifyPropertyChanged
    public string currentWord // Свойство для привязки
    {
        get => _currentWord;
        set
        {
            if (_currentWord != value)
            {
                _currentWord = value;
                OnPropertyChanged(nameof(currentWord)); // Уведомляем об изменениях
            }
        }
    }

    public BoxView? bisqueBV;
    public Label? labelTimer = new Label();

    public float fillMode_XStart { get; set; } = 0;
    public float fillMode_YStart { get; set; } = 0;
    public float fillMode_XEnd { get; set; } = 0;
    public float fillMode_YEnd { get; set; } = 1.0f;

    public float timeRemain { get; set; } = 3.0f;

    public LearnPage(List<Word> choosenWords)
    {
        _choosenWords = choosenWords;
        InitializeComponent();
        WordToLearn();
        BindingContext = this;
        BoxViewBisque();
        AnimateBisqueBV();
        FillGridForChoosenWords();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async void WordToLearn()
    {
        Random rnd = new Random();
        int randomWordIndex = rnd.Next(0, _choosenWords.Count);
        currentWord = _choosenWords[randomWordIndex].word;
        wordLabel.BackgroundColor = Colors.Yellow;
    }

    private async void FillGridForChoosenWords()
    {
        // очищаем существующие кнопки в сетке
        GridForTranslations.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // Задайте количество столбцов

        // Проходим по словам и добавляем кнопки в сетку
        for (int i = 0; i < _choosenWords.Count; i++)
        {
            // Создаем кнопку
            Button button = new Button();
            button.Text = _choosenWords[i].word;

            // Сохраняем текущее значение индекса i в локальной переменной
            int index = i;

            // Обработчик нажатия кнопки
            button.Clicked += async (s, e) =>
            {
                if ((s as Button).Text != currentWord)
                {
                    //wordLabel.BackgroundColor = Colors.Red;
                    timer1.Stop(); // Запускаем таймер
                    await RedBGOfLabel(wordLabel);
                }
                else
                {
                    timer1.Stop(); // Запускаем таймер
                }
                fillMode_XEnd = 0;
                timeRemain = 3.0f;
                UpdateTimer();
                WordToLearn();
            };

            // Устанавливаем ячейку для кнопки
            Grid.SetRow(button, rowCount);
            Grid.SetColumn(button, columnCount);

            // Добавляем кнопку в сетку
            GridForTranslations.Children.Add(button);

            // Обновляем индексы строк и столбцов
            columnCount++;
            if (columnCount >= columns) // Если достигли максимума столбцов
            {
                columnCount = 0;
                rowCount++; // Переходим на следующую строку
            }
        }
    }


    void BoxViewBisque()
    {
        bisqueBV = new BoxView()
        {
            BackgroundColor = Colors.SeaGreen
        };

        UpdateBoxViewLayout(fillMode_XEnd);
        UpdateTimer();

        absLayOut.Children.Add(bisqueBV);
        absLayOut.Children.Add(labelTimer);
    }
    void UpdateBoxViewLayout(float XEnd)
    {
        // x = 0 (слева), y = 0 (вверху), width = 0.5 (50% ширины), height = 1 (100% высоты)
        AbsoluteLayout.SetLayoutBounds(bisqueBV, new Rect(fillMode_XStart, fillMode_YStart, XEnd, fillMode_YEnd));
        // Устанавливаем флаг для того, чтобы BoxView занимал выделенный диапазон
        AbsoluteLayout.SetLayoutFlags(bisqueBV, AbsoluteLayoutFlags.All);

        AbsoluteLayout.SetLayoutBounds(labelTimer, new Rect(0, 0, 1, 1)); // Задаем границы (0-1 для ширины и высоты)
        AbsoluteLayout.SetLayoutFlags(labelTimer, AbsoluteLayoutFlags.All); // Указываем, что границы относительно родителя
    }
    void AnimateBisqueBV()
    {
        // Используем 50 миллисекунд для каждого шага
        int steps = (int)(timeRemain * 1000 / 100);
        float stepSize = 1.0f / steps; // Увеличение на каждый шаг
        float elapsedTime = 0;

        var timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(100);
        timer.Tick += (s, e) =>
        {
            // Уменьшаем оставшееся время
            timeRemain -= 0.1f;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Увеличиваем fillMode_XEnd на каждый шаг
                if (fillMode_XEnd <= 1.0f)
                {
                    fillMode_XEnd += stepSize; // Увеличиваем значение

                    // Обновляем метку с оставшимся временем
                    elapsedTime += 50 / 1000f; // Измеряем прошедшее время

                    if (fillMode_XEnd > 1.0f) fillMode_XEnd = 1.0f; // Ограничение значения
                    UpdateBoxViewLayout(fillMode_XEnd); // Обновляем отображение
                }
            });
        };

        timer.Start();
    }

    private void UpdateTimer()
    {
        TimeSpan countdown = TimeSpan.FromSeconds(timeRemain);
        timer1 = Dispatcher.CreateTimer();
        timer1.Interval = TimeSpan.FromMilliseconds(100);
        timer1.Start(); // Запускаем таймер

        timer1.Tick += (s, e) =>
        {
            if (countdown.TotalMilliseconds <= 0)
            {
                labelTimer.Text = "00:00 cек.";
                labelTimer.TextColor = Colors.Red;
                labelTimer.FontAttributes = FontAttributes.Bold;
                labelTimer.Opacity = 0.8f;
                labelTimer.FontSize = 20;
                labelTimer.HorizontalOptions = LayoutOptions.Center; // Центрируем по горизонтали
                labelTimer.VerticalOptions = LayoutOptions.Center; // Центрируем по вертикали
                timer1.Stop(); // Останавливаем таймер, когда время истекло
            }
            else
            {
                countdown = countdown.Subtract(TimeSpan.FromMilliseconds(100)); // Уменьшаем оставшееся время на 100 мс
                labelTimer.Text = $"{countdown.Seconds:D2}:{countdown.Milliseconds / 10:D2} сек."; // Обновите текст метки
                labelTimer.TextColor = Colors.White; // Цвет метки
                labelTimer.FontAttributes = FontAttributes.Bold;
                labelTimer.Opacity = 0.8f;
                labelTimer.FontSize = 20;
                labelTimer.HorizontalOptions = LayoutOptions.Center; // Центрируем по горизонтали
                labelTimer.VerticalOptions = LayoutOptions.Center; // Центрируем по вертикали
            }
        };
    }

    async Task RedBGOfLabel(Label currLabel)
    {
        float duration = 0.2f;
        int interval = 10;
        TimeSpan redBG_CountDown = TimeSpan.FromSeconds(duration); //TimeSpan: Это структура в .NET, представляющая интервал времени. Она позволяет нам
                                                                   //работать с временными интервалами, такими как продолжительность.

        IDispatcherTimer redBG_dispatcher = Dispatcher.CreateTimer();
        redBG_dispatcher.Interval = TimeSpan.FromMilliseconds(interval);

        Color startColor = Colors.Red;
        Color endColor = Colors.Yellow;
        int steps = (int)(duration * 1000) / interval;
        int stepCounter = 0;

        // Создаем задачу для ожидания завершения
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        redBG_dispatcher.Tick += async (s, e) =>
        {
            if (stepCounter >= steps)
            {
                redBG_dispatcher.Stop();
                currLabel.BackgroundColor = endColor; // Установка конечного цвета
                tcs.SetResult(true); // Завершаем задачу
                return; // Выход из метода
            }
            // Вычисление текущего цвета
            float progress = (float)stepCounter / steps;

            Color currentColor = Color.FromRgb(
                startColor.Red + (endColor.Red - startColor.Red) * progress,
                startColor.Green + (endColor.Green - startColor.Green) * progress,
                startColor.Blue + (endColor.Blue - startColor.Blue) * progress);

                currLabel.BackgroundColor = currentColor;

                await Task.Delay(interval);
                stepCounter++;
        };

        // Убедитесь, что цвет точно установлен в конечный цвет
        currLabel.BackgroundColor = endColor;
        redBG_dispatcher.Start();
        await tcs.Task; // Ожидание завершения работы метода
    }
}






/*

using MauiLib1.Models;
using Microsoft.Maui.Layouts;

namespace dwWithEFAndDll.Pages;

public partial class LearnPage : ContentPage
{
    List<Word> _choosenWords;
    IDispatcherTimer timer1;
    public string currentWord { get; set; }
    public BoxView? bisqueBV;
    public Label? labelTimer = new Label() ;

    public float fillMode_XStart { get; set; } = 0;
    public float fillMode_YStart { get; set; } = 0;
    public float fillMode_XEnd { get; set; } = 0;
    public float fillMode_YEnd { get; set; } = 1.0f;

    public float timeRemain { get; set; } = 3.0f;

    public LearnPage(List<Word> choosenWords)
    {
        _choosenWords = choosenWords;
        InitializeComponent();
        WordToLearn();
        BindingContext = this;
        BoxViewBisque();
        AnimateBisqueBV();
        FillGridForChoosenWords();
    }
    public async void WordToLearn()
    {
        Random rnd = new Random();
        int randomWordIndex = rnd.Next(0, _choosenWords.Count);
        currentWord = _choosenWords[randomWordIndex].word;
        wordLabel.BackgroundColor = Colors.Yellow;
    }

    private async void FillGridForChoosenWords()
    {
        // очищаем существующие кнопки в сетке
        GridForTranslations.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // Задайте количество столбцов

        // Проходим по словам и добавляем кнопки в сетку
        for (int i = 0; i < _choosenWords.Count; i++)
        {
            // Создаем кнопку
            Button button = new Button();
            button.Text = _choosenWords[i].word;

            // Сохраняем текущее значение индекса i в локальной переменной
            int index = i;

            // Обработчик нажатия кнопки
            button.Clicked += async (s, e) =>
            {
                if ((s as Button).Text == currentWord)
                {
                    //await DisplayAlert("ww", "Правильно", "Cansel");
                    WordToLearn();
                }
                else
                {
                    wordLabel.BackgroundColor = Colors.Red;

                }

                timer1.Stop(); // Запускаем таймер
                fillMode_XEnd = 0;
                timeRemain = 3.0f;
                UpdateTimer();
            };

            // Устанавливаем ячейку для кнопки
            Grid.SetRow(button, rowCount);
            Grid.SetColumn(button, columnCount);

            // Добавляем кнопку в сетку
            GridForTranslations.Children.Add(button);

            // Обновляем индексы строк и столбцов
            columnCount++;
            if (columnCount >= columns) // Если достигли максимума столбцов
            {
                columnCount = 0;
                rowCount++; // Переходим на следующую строку
            }
        }
    }
}
 
 */