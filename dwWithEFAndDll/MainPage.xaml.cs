using dwWithEFAndDll.Pages;
using MauiLib1.Data;

namespace dwWithEFAndDll;

public partial class MainPage : ContentPage
{
    MyAppDbContext _myAppDbContext;
    public MainPage(MyAppDbContext dbContext)
    {
        _myAppDbContext = dbContext;
        InitializeComponent();
    }

    async void ToDictionaryPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DictionaryPage(_myAppDbContext));
    }
    async void ToAddWordPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddWordPage(_myAppDbContext));
    }

    async void OnstartLearnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChooseWordsPage());
    }
}
