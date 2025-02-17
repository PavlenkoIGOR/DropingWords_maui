using dwWithEFAndDll.ViewModels;
using MauiLib1.Data;
using MauiLib1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace dwWithEFAndDll.Pages;

public partial class DictionaryPage : ContentPage
{
	MyAppDbContext _dbContext;
	public WordsWithTranslationsViewModel wwtVM {  get; set; }
	//public WordAndTranslations watVM { get; set; }
	public List<WordAndTranslations>? watTranslations { get; set; } = null;

	public List<Word> words { get; set; }
	public StringBuilder? translaitonString { get; set; } = null;

    public DictionaryPage(MyAppDbContext myAppDb)
	{
		_dbContext = myAppDb;
		wwtVM = new WordsWithTranslationsViewModel();
		//watVM = new WordAndTranslations();
		watTranslations = new List<WordAndTranslations>();
		words = new List<Word>();

        InitializeComponent();
		FillDictionary();
        BindingContext = this;
	}

	public async void FillDictionary()
	{
		wwtVM.wordsInVM = await _dbContext.Words.ToListAsync();
		wwtVM.translationInVM = await _dbContext.Translations.ToListAsync();

		//var currList = _dbContext.Words.Include(id => id.translations).ToListAsync();

		words = await _dbContext.Words.OrderBy(w => w.word).Include(w => w.translations).Select(a=>a).ToListAsync();
		foreach (Word w in words)
		{
            WordAndTranslations wat = new WordAndTranslations();
            wat.word = w;
            foreach (Translation translation in w.translations)
			{               
                wat.translationsString?.Append(translation.translation + "; ");
                
            }
            watTranslations?.Add(wat);
        }
    }

    private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        WordAndTranslations selectedWord = e.SelectedItem as WordAndTranslations;
		string action = await DisplayActionSheet(selectedWord.word.word, "Cancel", null, "�������������", "�������");
		switch (action)
		{
			case "�������������":
				await Navigation.PushAsync(new AddWordPage(_dbContext, selectedWord));
				break;
			case "�������":
				Word wordDorDelete = await _dbContext.Words.Where(w => w.id == selectedWord.word.id).FirstAsync();
				_dbContext.Words.Remove(wordDorDelete);
				await _dbContext.SaveChangesAsync();
				break;
			default:
				break;
		}
	}
}
/*
 "CREATE INDEX idx_words_first_letter ON Words (LOWER(SUBSTR(word, 1, 1)));"
 * */