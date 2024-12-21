using dwWithEFAndDll.ViewModels;
using MauiLib1.Data;
using MauiLib1.Models;
using Microsoft.EntityFrameworkCore;

namespace dwWithEFAndDll.Pages;

public partial class ChooseWordsPage : ContentPage
{
	MyAppDbContext _db;
    List<Word> _choosingWords;
    List<WordAndTranslationsLP> _wordAndTranslationsLP_list;

    public bool IsAddBttnEnabled { get; set; } = true;
    public bool IsCanselBttnEnabled { get; set; } = false;
    public ChooseWordsPage(MyAppDbContext myAppDb)
	{
        _wordAndTranslationsLP_list = new List<WordAndTranslationsLP>();
		_db = myAppDb;
        _choosingWords = new List<Word>();
        InitializeComponent();
	}

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue;

        // �������� ����� �� ���� ������ �� ������ ���������� ������
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var words = await SearchWordsAsync(searchText);
            wordsListView.ItemsSource = words;
        }
        else
        {
            wordsListView.ItemsSource = null; // �������� ������, ���� ������ ������ ������
        }
    }

    private async Task<List<Word>> SearchWordsAsync(string searchText)
    {
        return await _db.Words.Include(i=>i.translations)
            .Where(w => w.word.StartsWith(searchText))
            .ToListAsync();
    }

    private async void OnWordSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // ��������� ������ �����, ���� ����������
        if (e.SelectedItem is Word selectedWord)
        {
            // �������� ��� ������ �����, ��������, ������� �� ������ �������� ��� ����������� ���������� � �����
            bool answer = await DisplayAlert("���� ������", "������� ��� �����?", "��", "���");
            if (answer)
            {

                Word choosingWord = e.SelectedItem as Word;
                _choosingWords.Add(choosingWord); //�������?


                WordAndTranslationsLP watLP = new WordAndTranslationsLP();
                watLP.word = selectedWord.word;
                watLP.translations = selectedWord.translations.Select(t=>t.translation).ToList();
                _wordAndTranslationsLP_list.Add(watLP);
            }
        }
        FillGridForChoosenWords();
    }

    private async void FillGridForChoosenWords()
    {
        // ������� ������������ ������ � �����
        GridForCoosenWords.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // ������� ���������� ��������

        // �������� �� ������ � ��������� ������ � �����
        for (int i = 0; i < _choosingWords.Count; i++)
        {
            // ������� ������
            Button button = new Button();
            button.Text = _choosingWords[i].word;

            // ��������� ������� �������� ������� i � ��������� ����������
            int index = i;

            // ���������� ������� ������
            button.Clicked += async (s, e) =>
            {
                bool answer2 = await DisplayAlert("���� ������", "������� ��� �����?", "��", "���");
                if (answer2)
                {
                    // ������� ����� �� ������
                    _choosingWords.RemoveAt(index);

                    // ��������� Grid ����� ��������
                    FillGridForChoosenWords();
                }
            };

            // ������������� ������ ��� ������
            Grid.SetRow(button, rowCount);
            Grid.SetColumn(button, columnCount);

            // ��������� ������ � �����
            GridForCoosenWords.Children.Add(button);

            // ��������� ������� ����� � ��������
            columnCount++;
            if (columnCount >= columns) // ���� �������� ��������� ��������
            {
                columnCount = 0;
                rowCount++; // ��������� �� ��������� ������
            }
        }
    }

    private void OnAddButtonClicked(object sender, EventArgs e)
    {
        // �������� ������ ����� �� ��������� �������
        var button = (Button)sender;
        var word = (Word)button.CommandParameter;

        _choosingWords.Add(word);
        IsAddBttnEnabled = false;
        IsCanselBttnEnabled = true;
        // ����� ����� �������� ������ ��� ��������� ���������� �����
        // ��������, �������� � ������ ������� ���� ��� ������� ����� ���������� � ��
        //DisplayAlert("���������", $"����� '{word.word}' ���������.", "OK");
    }

    private void OnCancelButtonClicked(object sender, EventArgs e)
    {
        if (_choosingWords.Contains((Word)sender))
        {
            _choosingWords.Remove((Word)sender);
        }
    }

    async void GoToLearnPage(object sender, EventArgs e)
    {
        /* ����� ���� ������ System.InvalidOperationException: "The LINQ expression 's => s.translations' could not be translated. Either rewrite the query in a form that can be translated, 
         * or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'. 
         * See https://go.microsoft.com/fwlink/?linkid=2101038 for more information."
        List<string> translations = await _db.Translations
            .Where(t=>t.translation != null &&
                                !_wordAndTranslationsLP_list.SelectMany(s=>s.translations).Contains(t.translation))
            .Select(t=>t.translation)
            .ToListAsync(); // ��-�� ��� Entity Framework �� ����� ��������� ����� ������ � �������������� SelectMany �� SQL. ������� ���� ��������� �� ��� �������
        */

        // ������� ������� ��� �������� � ������
        var excludedTranslations = _wordAndTranslationsLP_list
            .SelectMany(s => s.translations)
            .ToList();

        // ������ ���������� ���� ������ � �������
        List<string> translations = await _db.Translations
            .Where(t => t.translation != null && !excludedTranslations.Contains(t.translation))
            .Select(t => t.translation)
            .ToListAsync();
        Random random = new Random();
        List<string> randomTranslations = translations.OrderBy(x => random.Next()).Take(50).ToList();



        await Navigation.PushAsync(new LearnPage(_choosingWords, _wordAndTranslationsLP_list, randomTranslations));
    }
}

//CommandParameter="{Binding .}" ��������� �������� ���� ������ Word, ����� ������ ������.