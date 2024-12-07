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
}