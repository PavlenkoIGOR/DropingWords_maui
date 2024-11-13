using MauiLib1.Data;
using MauiLib1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dwWithEFAndDll
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddDbContext<MyAppDbContext>(opt=>opt.UseSqlite(MyConnection.GetConnection()));
#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddScoped<MainPage>();

            return builder.Build();
        }
    }
}
