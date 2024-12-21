using dwWithEFAndDll.ViewModels;
using MauiLib1.Models;
using Microsoft.Maui.Layouts;
using System.ComponentModel;

namespace dwWithEFAndDll.Pages;

public partial class LearnPage : ContentPage, INotifyPropertyChanged
{

    List<string> _randomTranslations;
    List<WordAndTranslationsLP> _choosenWordAndTranslationsLP_list;
    IDispatcherTimer timer1;

    private WordAndTranslationsLP _currentWord; // �������� ���� ��� ������������� ������� INotifyPropertyChanged
    public LearnPage(List<Word> choosenWords, List<WordAndTranslationsLP> watLP, List<string> randomTranslations)
    {
        currentWord = new WordAndTranslationsLP();
        _choosenWordAndTranslationsLP_list = watLP;
        _randomTranslations = randomTranslations;
        tmpTimeRemain = timeRemain;

        InitializeComponent();
        PauseBttnCreation();// ��������� ������ �� ������ ���������
        WordToLearn();
        BindingContext = this;
        BoxViewBisque();
        AnimateBisqueBV();
        FillGridForChoosenWords();
    }
    public WordAndTranslationsLP currentWord // �������� ��� ��������
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
    public WordAndTranslationsLP wordAndTranslationsLP { get; set; }
    public BoxView? bisqueBV;
    public Label? labelTimer = new Label();

    public float fillMode_XStart { get; set; } = 0;
    public float fillMode_YStart { get; set; } = 0;
    public float fillMode_XEnd { get; set; } = 0;
    public float fillMode_YEnd { get; set; } = 1.0f;

    public float timeRemain { get; set; } = 3.0f;
    private float tmpTimeRemain;



    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async void WordToLearn()
    {
        Random rnd = new Random();
        int randomWordIndex = rnd.Next(0, _choosenWordAndTranslationsLP_list.Count);
        //currentWord = _choosenWords[randomWordIndex];
        currentWord = _choosenWordAndTranslationsLP_list[randomWordIndex];
        if (currentWord.translations.Count > 1)
        {
            currentWord.translation = currentWord.translations[rnd.Next(currentWord.translations.Count)];
        }
        else
        {
            currentWord.translation = currentWord.translations[0];
        }
        wordLabel.BackgroundColor = Colors.Yellow;
    }

    /// <summary>
    /// zapolnenie yacheiki c knopkami dlya variantov otveta
    /// </summary>
    async void FillGridForChoosenWords()
    {
        byte[][] cellCoord = { [0, 0], [0, 1], [1, 0], [1, 1], [2, 0], [2, 1], [3, 0], [3, 1] };

        Random random = new Random();
        byte[][] shuffledCellCoords = cellCoord.Cast<byte[]>().OrderBy(x => random.Next()).ToArray();
        //var shuffledCellCoords = cellCoord.Cast<byte[]>().OrderBy(x => random.Next()).ToArray();

        // ������� ������������ ������ � �����
        GridForTranslations.Children.Clear();
        /*
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // ������� ���������� ��������
        */
        #region create a list with random translations values
        Random random1 = new Random();
        List<string> rndSevenTranslationsAndCurrentTranslation = _randomTranslations.OrderBy(t => random1.Next(_randomTranslations.Count)).Take(7).ToList();
        rndSevenTranslationsAndCurrentTranslation.Add(currentWord.translation);
        #endregion

        // �������� �� ������ � ��������� ������ � �����
        for (int i = 0; i < rndSevenTranslationsAndCurrentTranslation.Count; i++)
        {
            // ������� ������
            Button button = new Button();

            button.Text = rndSevenTranslationsAndCurrentTranslation[i];
            // ��������� ������� �������� ������� i � ��������� ����������
            int index = i;
            // ���������� ������� ������
            button.Clicked += async (s, e) =>
            {
                if ((s as Button).Text != currentWord.translation)
                {
                    //wordLabel.BackgroundColor = Colors.Red;
                    timer1.Stop(); // ������������� ������
                    await RedBGOfLabel(wordLabel);
                }
                else
                {
                    timer1.Stop(); // ������������� ������
                }

                fillMode_XEnd = 0;
                timeRemain = tmpTimeRemain;
                UpdateTimer();
                WordToLearn();
                FillGridForChoosenWords();
            };

            #region ��� �������� �������� ������� 8 ����� � 2 �������
            /*
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
            */
            #endregion

            #region ��� ���������� ������������� ������ 
            // ������������� ���������� ������ �� ������������� ������� ���������
            int row = shuffledCellCoords[i][0];
            int column = shuffledCellCoords[i][1];
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            GridForTranslations.Children.Add(button);
            #endregion
        }
    }

    #region zapolnenie yacheiki v absolute layout cell dlya progressbar'a
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
        double myInterval = 100;
        // ���������� 50 ����������� ��� ������� ����
        int steps = (int)(timeRemain * 1000 / myInterval);
        float stepSize = 1.0f / steps; // ���������� �� ������ ���
        float elapsedTime = 0;

        var timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(myInterval);
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

        double interval = 100;
        TimeSpan countdown = TimeSpan.FromSeconds(timeRemain);
        timer1 = Dispatcher.CreateTimer();
        timer1.Interval = TimeSpan.FromMilliseconds(interval);


        timer1.Tick += (s, e) =>
        {
            countdown = countdown.Subtract(timer1.Interval); // ��������� ���������� ����� �� 100 ��

            if (countdown.TotalMilliseconds <= 0.1)
            {
                labelTimer.Text = "00:00 c��.";
                labelTimer.TextColor = Colors.Red;
                timer1.Stop(); // ������������� ������, ����� ����� �������
                               // ��������� ������ �����
                timeRemain = tmpTimeRemain;
                UpdateTimer();
            }
            else
            {
                labelTimer.Text = $"{countdown.Seconds:D2}:{countdown.Milliseconds / 10:D2} ���."; // �������� ����� �����
                labelTimer.TextColor = Colors.White; // ���� �����
            }
            labelTimer.FontAttributes = FontAttributes.Bold;
            //labelTimer.Opacity = 0.8f;
            labelTimer.FontSize = 30;
            labelTimer.HorizontalOptions = LayoutOptions.Center; // ���������� �� �����������
            labelTimer.VerticalOptions = LayoutOptions.Center; // ���������� �� ���������
        };
        timer1.Start(); // ��������� ������
    }
    #endregion

    /// <summary>
    /// method dlya izmeneniya fona c klychevym slovom v sluchae nepravilnogo otveta
    /// </summary>
    /// <param name="currLabel">nujnyi Label</param>
    /// <returns></returns>
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

            await Task.Delay(redBG_dispatcher.Interval);
            stepCounter++;
        };

        // ��������, ��� ���� ����� ���������� � �������� ����
        currLabel.BackgroundColor = endColor;
        redBG_dispatcher.Start();
        await tcs.Task; // �������� ���������� ������ ������
    }

    /// <summary>
    /// method for pause buttn creation
    /// </summary>
    void PauseBttnCreation()
    {
        var pauseButton = new ToolbarItem()
        {
            Text = "Pause",
            Priority = 0,
            Order = ToolbarItemOrder.Primary
        };
        pauseButton.Clicked += async (s, e) =>
        {
            await DisplayAlert("Pause", "Pause menu", "Cancel");
        };
        this.ToolbarItems.Add(pauseButton); // ��������� ������ �� ������ ���������
    }
}