using MauiLib1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

namespace MauiLib1.Data;

public class MyAppDbContext : DbContext
{

    public DbSet<Word> Words { get; set; }
    public DbSet<Translation> Translations { get; set; }

    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
        this.Database.EnsureCreated();
    }

    protected async override void OnModelCreating(ModelBuilder builder)
    {
        //var assembly = Assembly.GetExecutingAssembly();
        //var resourceName = "Dictionary.json"; // Убедитесь, что путь к ресурсу указан правильно

        //var file = Path.Combine(FileSystem.AppDataDirectory, resourceName);
        //using var streamReader = new StreamReader(file);


        //base.OnModelCreating(builder);
        builder.Entity<Word>()
            .HasMany(w => w.translations)
            .WithMany(t => t.words)
            .UsingEntity(j => j.ToTable("WordTranslation")); // Создаем новую таблицу для хранения отношений

        builder.Entity<Word>()
            .HasIndex(ind => ind.word)
            .HasDatabaseName("idx_words");
    }
}

