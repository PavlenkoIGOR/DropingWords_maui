using MauiLib1.Data;
using MauiLib1.Models;

namespace dwWithEFAndDll.Pages;

public partial class AddWordPage : ContentPage
{
    MyAppDbContext _dbContext;
    public AddWordPage(MyAppDbContext myAppDb)
    {
        BindingContext = this;
        _dbContext = myAppDb;
        InitializeComponent();
    }
    async void AddNewWordToDictionary(object sender, EventArgs e)
    {
        Word newWord = new Word() { word = wordEntryField.Text.ToLower() };
        
        if (_dbContext.Words.Any(w=>w.word.ToLower() == wordEntryField.Text.ToLower()))
        {
            //показать модальное окно с текстом "Такое слово есть в словаре!"
            await DisplayAlert("Предупреждение", "Такое слово есть в словаре!", "Ok");
            
            return;
        }
        List<Translation> newTranslations = new List<Translation>();

        string[] tr = translationEntryField.Text.ToLower().Split(new char[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in tr)
        {
            Translation newTranslation = new Translation() { translation = item };
            newTranslations.Add(newTranslation);
            newWord.translations.Add(newTranslation);
        }
        await _dbContext.Words.AddAsync(newWord);

        foreach (var item in newTranslations)
        {
            await _dbContext.Translations.AddAsync(item);
        }
        await _dbContext.SaveChangesAsync();

        wordEntryField.Text = string.Empty;
        translationEntryField.Text = string.Empty;
    }
}