using MauiLib1.Data;

namespace dwWithEFAndDll
{
    public partial class App : Application
    {
        public App(MyAppDbContext dbContext)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage(dbContext));
            //MainPage = new AppShell();
        }
    }
}
