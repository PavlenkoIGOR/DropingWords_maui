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

        // Получаем слова из базы данных на основе введенного текста
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var words = await SearchWordsAsync(searchText);
            wordsListView.ItemsSource = words;
        }
        else
        {
            wordsListView.ItemsSource = null; // Очистить список, если строка поиска пустая
        }
    }

    private async Task<List<Word>> SearchWordsAsync(string searchText)
    {
        return await _db.Words
            .Where(w => w.word.Contains(searchText))
            .ToListAsync();
    }

    private void OnWordSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Обработка выбора слова, если необходимо
        if (e.SelectedItem is Word selectedWord)
        {
            // Действия при выборе слова, например, переход на другую страницу или отображение информации о слове
        }
    }

    private void OnAddButtonClicked(object sender, EventArgs e)
    {
        // Получаем объект слова из параметра команды
        var button = (Button)sender;
        var word = (Word)button.CommandParameter;

        _choosingWords.Add(word);
        IsAddBttnEnabled = false;
        IsCanselBttnEnabled = true;
        // Здесь можно добавить логику для обработки добавления слова
        // Например, добавить в список любимых слов или вызвать метод добавления в БД
        //DisplayAlert("Добавлено", $"Слово '{word.word}' добавлено.", "OK");
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

//CommandParameter="{Binding .}" позволяет передать весь объект Word, когда кнопка нажата.