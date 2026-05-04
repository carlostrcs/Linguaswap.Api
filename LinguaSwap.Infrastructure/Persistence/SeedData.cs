using LinguaSwap.Domain.Vocabulary;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static void EnsureSeeded(LinguaSwapDbContext db)
        {
            // 1) Si ya hay alguna library pública, asumimos seed hecho
            if (db.Libraries.Any(l => l.IsPublic))
                return;

            // 2) Crear libraries públicas
            var basic = new Library
            {
                Name = "Basic (Demo)",
                IsPublic = true,
                UserId = null
            };

            var travel = new Library
            {
                Name = "Travel (Demo)",
                IsPublic = true,
                UserId = null
            };

            db.Libraries.AddRange(basic, travel);
            db.SaveChanges();

            // 3) Helper para crear un vocab item con términos
            void AddItem(Library lib, params (string lang, string text)[] terms)
            {
                var item = new VocabItem
                {
                    LibraryId = lib.Id
                };
                db.VocabItems.Add(item);
                db.SaveChanges();

                foreach (var (lang, text) in terms)
                {
                    db.VocabTerms.Add(new VocabTerm
                    {
                        VocabItemId = item.Id,
                        LanguageCode = lang.Trim().ToLowerInvariant(),
                        Text = text.Trim()
                    });
                }
            }

            // 4) Meter vocab (poco pero suficiente para demo)
            AddItem(basic, ("es", "hola"), ("en", "hello"), ("fr", "bonjour"));
            AddItem(basic, ("es", "adiós"), ("en", "goodbye"), ("fr", "au revoir"));
            AddItem(basic, ("es", "gracias"), ("en", "thank you"), ("fr", "merci"));
            AddItem(basic, ("es", "por favor"), ("en", "please"), ("fr", "s'il vous plaît"));
            AddItem(basic, ("es", "agua"), ("en", "water"), ("fr", "eau"));
            AddItem(basic, ("es", "comida"), ("en", "food"), ("fr", "nourriture"));

            AddItem(travel, ("es", "aeropuerto"), ("en", "airport"), ("fr", "aéroport"));
            AddItem(travel, ("es", "hotel"), ("en", "hotel"), ("fr", "hôtel"));
            AddItem(travel, ("es", "billete"), ("en", "ticket"), ("fr", "billet"));
            AddItem(travel, ("es", "¿dónde está el baño?"), ("en", "where is the bathroom?"), ("fr", "où sont les toilettes ?"));

            db.SaveChanges();
        }
    }
}
