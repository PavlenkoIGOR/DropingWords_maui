using dwWithEFAndDll.ViewModels;
using MauiLib1.Data;
using MauiLib1.Models;
using Microsoft.EntityFrameworkCore;

namespace dwWithEFAndDll.Pages;

public partial class DictionaryPage : ContentPage
{
	MyAppDbContext _dbContext;
	public WordsWithTranslationsViewModel wwtVM {  get; set; }
	public WordAndTranslations watVM { get; set; }
	public DictionaryPage(MyAppDbContext myAppDb)
	{
		_dbContext = myAppDb;
		wwtVM = new WordsWithTranslationsViewModel();
		watVM = new WordAndTranslations();
		InitializeComponent();
		FillDictionary();
        BindingContext = this;
	}

	public void FillDictionary()
	{
		wwtVM.wordsInVM = _dbContext.Words.ToList();
		wwtVM.translationInVM = _dbContext.Translations.ToList();

		var currList = _dbContext.Words.Include(id => id.translations);

		List<WordAndTranslations> wordAndTranslations = new List<WordAndTranslations>();
		var arr = _dbContext.Words.Include(w => w.translations).Select(a=>a).ToListAsync();
    }
}