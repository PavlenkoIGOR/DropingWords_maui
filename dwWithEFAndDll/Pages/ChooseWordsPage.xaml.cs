using MauiLib1.Data;
using MauiLib1.Models;
using Microsoft.EntityFrameworkCore;

namespace dwWithEFAndDll.Pages;

public partial class ChooseWordsPage : ContentPage
{
	MyAppDbContext _db;
    List<Word> _choosingWords;

    public bool IsAddBttnEnabled { get; set; } = true;
    public bool IsCanselBttnEnabled { get; set; } = false;
    public ChooseWordsPage(MyAppDbContext myAppDb)
	{
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
        return await _db.Words
            .Where(w => w.word.Contains(searchText))
            .ToListAsync();
    }

    private async void OnWordSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // ������� ������������ ������ � �����
        GridForCoosenWords.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // ������� ���������� ��������
        // ��������� ������ �����, ���� ����������
        if (e.SelectedItem is Word selectedWord)
        {
            // �������� ��� ������ �����, ��������, ������� �� ������ �������� ��� ����������� ���������� � �����
            bool answer =  await  DisplayAlert("���� ������", "������� ��� �����?", "��", "���");
            if (answer)
            {
                Word choosingWord = e.SelectedItem as Word;
                _choosingWords.Add(choosingWord);
            }
        }

        // �������� �� ������ � ��������� ������ � �����
        for (int i = 0; i < _choosingWords.Count; i++)
        {
            // ������� ������
            Button button = new Button();
            button.Text = _choosingWords[i].word;
            

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
        await Navigation.PushAsync(new LearnPage());
    }
}

//CommandParameter="{Binding .}" ��������� �������� ���� ������ Word, ����� ������ ������.