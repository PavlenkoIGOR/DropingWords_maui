using MauiLib1.Models;
using Microsoft.EntityFrameworkCore;

namespace MauiLib1.Data;

public class MyAppDbContext : DbContext
{

    public DbSet<Word> Words { get; set; }
    public DbSet<Translation> Translations { get; set; }

    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
        this.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Word>()
            .HasMany(w => w.translations)
            .WithMany(t => t.words)
            .UsingEntity(j => j.ToTable("WordTranslation")); // Создаем новую таблицу для хранения отношений

        builder.Entity<Word>()
            .HasIndex(ind => ind.word)
            .HasDatabaseName("idx_words");
    }
}

