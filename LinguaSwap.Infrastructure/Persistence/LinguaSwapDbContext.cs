using LinguaSwap.Domain.Practice;
using LinguaSwap.Domain.Users;
using LinguaSwap.Domain.Vocabulary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Infrastructure.Persistence
{
    public sealed class LinguaSwapDbContext : DbContext
    {
        public LinguaSwapDbContext(DbContextOptions<LinguaSwapDbContext> options) : base(options) { }

        public DbSet<PracticeSession> PracticeSessions => Set<PracticeSession>();
        public DbSet<Attempt> Attempts => Set<Attempt>();
        public DbSet<Library> Libraries => Set<Library>();
        public DbSet<VocabItem> VocabItems => Set<VocabItem>();
        public DbSet<VocabTerm> VocabTerms => Set<VocabTerm>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserVocabStats> UserVocabStats => Set<UserVocabStats>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PracticeSession>(entity =>
            {
                entity.ToTable("practice_sessions");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserId).IsRequired(false);

                entity.Property(x => x.StartedAt).IsRequired();

                entity.Property(x => x.Direction)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(x => x.Difficulty)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(x => x.LibraryId)
                    .IsRequired(false);

                entity.Property(x => x.SourceLanguage)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(x => x.TargetLanguage)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.HasIndex(x => x.LibraryId);
            });

            modelBuilder.Entity<Attempt>(entity =>
            {
                entity.ToTable("attempts");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.SessionId).IsRequired();
                entity.Property(x => x.WordId).IsRequired();

                entity.Property(x => x.UserAnswer)
                    .HasMaxLength(300)
                    .IsRequired();

                entity.Property(x => x.IsCorrect).IsRequired();
                entity.Property(x => x.CreatedAt).IsRequired();

                // Relación Attempt -> PracticeSession
                entity.HasOne<PracticeSession>()
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.SessionId);
                entity.HasIndex(x => x.WordId);
            });

            modelBuilder.Entity<Library>(entity =>
            {
                entity.ToTable("libraries");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserId).IsRequired(false);

                entity.Property(x => x.Name)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.HasIndex(x => x.UserId);
            });

            modelBuilder.Entity<VocabItem>(entity =>
            {
                entity.ToTable("vocab_items");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.LibraryId).IsRequired();

                entity.HasOne<Library>()
                    .WithMany()
                    .HasForeignKey(x => x.LibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.LibraryId);
            });

            modelBuilder.Entity<VocabTerm>(entity =>
            {
                entity.ToTable("vocab_terms");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.VocabItemId).IsRequired();

                entity.Property(x => x.LanguageCode)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(x => x.Text)
                    .HasMaxLength(300)
                    .IsRequired();

                entity.HasOne<VocabItem>()
                    .WithMany()
                    .HasForeignKey(x => x.VocabItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Para que sea rápido buscar términos por item/idioma:
                entity.HasIndex(x => new { x.VocabItemId, x.LanguageCode });

                // Opcional: evitar duplicados exactos del mismo idioma+texto por item:
                entity.HasIndex(x => new { x.VocabItemId, x.LanguageCode, x.Text }).IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.CreatedAt).IsRequired();

                entity.HasIndex(x => x.Email).IsUnique();
            });

        }
    }
}
