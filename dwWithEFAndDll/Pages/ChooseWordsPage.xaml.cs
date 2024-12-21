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
        return await _db.Words.Include(i=>i.translations)
            .Where(w => w.word.StartsWith(searchText))
            .ToListAsync();
    }

    private async void OnWordSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Обработка выбора слова, если необходимо
        if (e.SelectedItem is Word selectedWord)
        {
            // Действия при выборе слова, например, переход на другую страницу или отображение информации о слове
            bool answer = await DisplayAlert("Окно выбора", "Выбрать это слово?", "Да", "Нет");
            if (answer)
            {

                Word choosingWord = e.SelectedItem as Word;
                _choosingWords.Add(choosingWord); //удалить?


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
        // очищаем существующие кнопки в сетке
        GridForCoosenWords.Children.Clear();
        int rowCount = 0;
        int columnCount = 0;
        int columns = 2; // Задайте количество столбцов

        // Проходим по словам и добавляем кнопки в сетку
        for (int i = 0; i < _choosingWords.Count; i++)
        {
            // Создаем кнопку
            Button button = new Button();
            button.Text = _choosingWords[i].word;

            // Сохраняем текущее значение индекса i в локальной переменной
            int index = i;

            // Обработчик нажатия кнопки
            button.Clicked += async (s, e) =>
            {
                bool answer2 = await DisplayAlert("Окно выбора", "Удалить это слово?", "Да", "Нет");
                if (answer2)
                {
                    // Удаляем слово из списка
                    _choosingWords.RemoveAt(index);

                    // Обновляем Grid после удаления
                    FillGridForChoosenWords();
                }
            };

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
        /* вкоде ниже ошибка System.InvalidOperationException: "The LINQ expression 's => s.translations' could not be translated. Either rewrite the query in a form that can be translated, 
         * or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'. 
         * See https://go.microsoft.com/fwlink/?linkid=2101038 for more information."
        List<string> translations = await _db.Translations
            .Where(t=>t.translation != null &&
                                !_wordAndTranslationsLP_list.SelectMany(s=>s.translations).Contains(t.translation))
            .Select(t=>t.translation)
            .ToListAsync(); // из-за что Entity Framework не может перевести такой запрос с использованием SelectMany на SQL. поэтому надо разложить на два запроса
        */

        // Сначала получим все переводы в список
        var excludedTranslations = _wordAndTranslationsLP_list
            .SelectMany(s => s.translations)
            .ToList();

        // Теперь используем этот список в запросе
        List<string> translations = await _db.Translations
            .Where(t => t.translation != null && !excludedTranslations.Contains(t.translation))
            .Select(t => t.translation)
            .ToListAsync();
        Random random = new Random();
        List<string> randomTranslations = translations.OrderBy(x => random.Next()).Take(50).ToList();



        await Navigation.PushAsync(new LearnPage(_choosingWords, _wordAndTranslationsLP_list, randomTranslations));
    }
}

//CommandParameter="{Binding .}" позволяет передать весь объект Word, когда кнопка нажата.