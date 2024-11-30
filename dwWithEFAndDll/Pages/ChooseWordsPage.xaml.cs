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

    private async void OnWordSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // очищаем существующие кнопки в сетке
        GridForCoosenWords.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // Задайте количество столбцов
        // Обработка выбора слова, если необходимо
        if (e.SelectedItem is Word selectedWord)
        {
            // Действия при выборе слова, например, переход на другую страницу или отображение информации о слове
            bool answer =  await  DisplayAlert("Окно выбора", "Выбрать это слово?", "Да", "Нет");
            if (answer)
            {
                Word choosingWord = e.SelectedItem as Word;
                _choosingWords.Add(choosingWord);
            }
        }

        // Проходим по словам и добавляем кнопки в сетку
        for (int i = 0; i < _choosingWords.Count; i++)
        {
            // Создаем кнопку
            Button button = new Button();
            button.Text = _choosingWords[i].word;
            

            // Устанавливаем ячейку для кнопки
            Grid.SetRow(button, rowCount);
            Grid.SetColumn(button, columnCount);

            // Добавляем кнопку в сетку
            GridForCoosenWords.Children.Add(button);

            // Обновляем индексы строк и столбцов
            columnCount++;
            if (columnCount >= columns) // Если достигли максимума столбцов
            {
                columnCount = 0;
                rowCount++; // Переходим на следующую строку
            }
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