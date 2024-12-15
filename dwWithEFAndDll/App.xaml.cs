using MauiLib1.Data;
using mmc = Microsoft.Maui.Controls;

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

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 400;
            const int newHeight = 700;

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }
    }
}
