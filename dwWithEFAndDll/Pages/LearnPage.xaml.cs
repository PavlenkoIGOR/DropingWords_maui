using MauiLib1.Models;
using Microsoft.Maui.Layouts;
using System.ComponentModel;
using System.Diagnostics;

namespace dwWithEFAndDll.Pages;

public partial class LearnPage : ContentPage, INotifyPropertyChanged
{
    List<Word> _choosenWords;
    IDispatcherTimer timer1;

    private string _currentWord; // �������� ���� ��� ������������� ������� INotifyPropertyChanged
    public string currentWord // �������� ��� ��������
    {
        get => _currentWord;
        set
        {
            if (_currentWord != value)
            {
                _currentWord = value;
                OnPropertyChanged(nameof(currentWord)); // ���������� �� ����������
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
        // ������� ������������ ������ � �����
        GridForTranslations.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // ������� ���������� ��������

        // �������� �� ������ � ��������� ������ � �����
        for (int i = 0; i < _choosenWords.Count; i++)
        {
            // ������� ������
            Button button = new Button();
            button.Text = _choosenWords[i].word;

            // ��������� ������� �������� ������� i � ��������� ����������
            int index = i;

            // ���������� ������� ������
            button.Clicked += async (s, e) =>
            {
                if ((s as Button).Text != currentWord)
                {
                    //wordLabel.BackgroundColor = Colors.Red;
                    timer1.Stop(); // ��������� ������
                    await RedBGOfLabel(wordLabel);
                }
                else
                {
                    timer1.Stop(); // ��������� ������
                }
                fillMode_XEnd = 0;
                timeRemain = 3.0f;
                UpdateTimer();
                WordToLearn();
            };

            // ������������� ������ ��� ������
            Grid.SetRow(button, rowCount);
            Grid.SetColumn(button, columnCount);

            // ��������� ������ � �����
            GridForTranslations.Children.Add(button);

            // ��������� ������� ����� � ��������
            columnCount++;
            if (columnCount >= columns) // ���� �������� ��������� ��������
            {
                columnCount = 0;
                rowCount++; // ��������� �� ��������� ������
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
        // x = 0 (�����), y = 0 (������), width = 0.5 (50% ������), height = 1 (100% ������)
        AbsoluteLayout.SetLayoutBounds(bisqueBV, new Rect(fillMode_XStart, fillMode_YStart, XEnd, fillMode_YEnd));
        // ������������� ���� ��� ����, ����� BoxView ������� ���������� ��������
        AbsoluteLayout.SetLayoutFlags(bisqueBV, AbsoluteLayoutFlags.All);

        AbsoluteLayout.SetLayoutBounds(labelTimer, new Rect(0, 0, 1, 1)); // ������ ������� (0-1 ��� ������ � ������)
        AbsoluteLayout.SetLayoutFlags(labelTimer, AbsoluteLayoutFlags.All); // ���������, ��� ������� ������������ ��������
    }
    void AnimateBisqueBV()
    {
        // ���������� 50 ����������� ��� ������� ����
        int steps = (int)(timeRemain * 1000 / 100);
        float stepSize = 1.0f / steps; // ���������� �� ������ ���
        float elapsedTime = 0;

        var timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(100);
        timer.Tick += (s, e) =>
        {
            // ��������� ���������� �����
            timeRemain -= 0.1f;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // ����������� fillMode_XEnd �� ������ ���
                if (fillMode_XEnd <= 1.0f)
                {
                    fillMode_XEnd += stepSize; // ����������� ��������

                    // ��������� ����� � ���������� ��������
                    elapsedTime += 50 / 1000f; // �������� ��������� �����

                    if (fillMode_XEnd > 1.0f) fillMode_XEnd = 1.0f; // ����������� ��������
                    UpdateBoxViewLayout(fillMode_XEnd); // ��������� �����������
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
        timer1.Start(); // ��������� ������

        timer1.Tick += (s, e) =>
        {
            if (countdown.TotalMilliseconds <= 0)
            {
                labelTimer.Text = "00:00 c��.";
                labelTimer.TextColor = Colors.Red;
                labelTimer.FontAttributes = FontAttributes.Bold;
                labelTimer.Opacity = 0.8f;
                labelTimer.FontSize = 20;
                labelTimer.HorizontalOptions = LayoutOptions.Center; // ���������� �� �����������
                labelTimer.VerticalOptions = LayoutOptions.Center; // ���������� �� ���������
                timer1.Stop(); // ������������� ������, ����� ����� �������
            }
            else
            {
                countdown = countdown.Subtract(TimeSpan.FromMilliseconds(100)); // ��������� ���������� ����� �� 100 ��
                labelTimer.Text = $"{countdown.Seconds:D2}:{countdown.Milliseconds / 10:D2} ���."; // �������� ����� �����
                labelTimer.TextColor = Colors.White; // ���� �����
                labelTimer.FontAttributes = FontAttributes.Bold;
                labelTimer.Opacity = 0.8f;
                labelTimer.FontSize = 20;
                labelTimer.HorizontalOptions = LayoutOptions.Center; // ���������� �� �����������
                labelTimer.VerticalOptions = LayoutOptions.Center; // ���������� �� ���������
            }
        };
    }

    async Task RedBGOfLabel(Label currLabel)
    {
        float duration = 0.2f;
        int interval = 10;
        TimeSpan redBG_CountDown = TimeSpan.FromSeconds(duration); //TimeSpan: ��� ��������� � .NET, �������������� �������� �������. ��� ��������� ���
                                                                   //�������� � ���������� �����������, ������ ��� �����������������.

        IDispatcherTimer redBG_dispatcher = Dispatcher.CreateTimer();
        redBG_dispatcher.Interval = TimeSpan.FromMilliseconds(interval);

        Color startColor = Colors.Red;
        Color endColor = Colors.Yellow;
        int steps = (int)(duration * 1000) / interval;
        int stepCounter = 0;

        // ������� ������ ��� �������� ����������
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        redBG_dispatcher.Tick += async (s, e) =>
        {
            if (stepCounter >= steps)
            {
                redBG_dispatcher.Stop();
                currLabel.BackgroundColor = endColor; // ��������� ��������� �����
                tcs.SetResult(true); // ��������� ������
                return; // ����� �� ������
            }
            // ���������� �������� �����
            float progress = (float)stepCounter / steps;

            Color currentColor = Color.FromRgb(
                startColor.Red + (endColor.Red - startColor.Red) * progress,
                startColor.Green + (endColor.Green - startColor.Green) * progress,
                startColor.Blue + (endColor.Blue - startColor.Blue) * progress);

                currLabel.BackgroundColor = currentColor;

                await Task.Delay(interval);
                stepCounter++;
        };

        // ���������, ��� ���� ����� ���������� � �������� ����
        currLabel.BackgroundColor = endColor;
        redBG_dispatcher.Start();
        await tcs.Task; // �������� ���������� ������ ������
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
        // ������� ������������ ������ � �����
        GridForTranslations.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // ������� ���������� ��������

        // �������� �� ������ � ��������� ������ � �����
        for (int i = 0; i < _choosenWords.Count; i++)
        {
            // ������� ������
            Button button = new Button();
            button.Text = _choosenWords[i].word;

            // ��������� ������� �������� ������� i � ��������� ����������
            int index = i;

            // ���������� ������� ������
            button.Clicked += async (s, e) =>
            {
                if ((s as Button).Text == currentWord)
                {
                    //await DisplayAlert("ww", "���������", "Cansel");
                    WordToLearn();
                }
                else
                {
                    wordLabel.BackgroundColor = Colors.Red;

                }

                timer1.Stop(); // ��������� ������
                fillMode_XEnd = 0;
                timeRemain = 3.0f;
                UpdateTimer();
            };

            // ������������� ������ ��� ������
            Grid.SetRow(button, rowCount);
            Grid.SetColumn(button, columnCount);

            // ��������� ������ � �����
            GridForTranslations.Children.Add(button);

            // ��������� ������� ����� � ��������
            columnCount++;
            if (columnCount >= columns) // ���� �������� ��������� ��������
            {
                columnCount = 0;
                rowCount++; // ��������� �� ��������� ������
            }
        }
    }
}
 
 */